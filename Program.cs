using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GFDecompress
{
    class Program
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        #region .dat 복호화

        /// <summary>
        /// .dat 복호화
        /// </summary>
        /// <param name="data">바이트 데이터</param>
        /// <param name="key">바이트 키</param>
        /// <returns></returns>
        public static string DatFileDecompress(byte[] data, byte[] key)
        {
            byte[] temp = Xor(data, key);
            byte[] output;
            using (var compressedStream = new MemoryStream(temp))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                output = resultStream.ToArray();
            }
            return Encoding.UTF8.GetString(output);
        }

        public static byte[] Xor(byte[] text, byte[] key)
        {
            byte[] xor = new byte[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                xor[i] = (byte)(text[i] ^ key[i % key.Length]);
            }
            return xor;
        }

        #endregion

        #region .stc 파싱

        /// <summary>
        /// .stc 파싱
        /// </summary>
        /// <param name="stcFile">stc 파일명</param>
        /// <param name="startOffset">시작 오프셋 (강제 설정)</param>
        /// <returns></returns>
        public static JArray ParseStc(string stcFile, string clientVersion, int startOffset = 0, bool isLong = false)
        {
            JArray output = new JArray();

            // stc 읽기
            byte[] stcStream = File.ReadAllBytes("stc\\" + stcFile);
            StcBinaryReader reader = new StcBinaryReader(stcStream);

            int code = reader.ReadUShort();         // 코드 (예: 5005)
            reader.ReadUShort();                    // ??

            if (isLong) {
                reader.ReadUShort();
            }

            //log.Debug("file: {0}, code: {1}", stcFile, code);

            int row = 0;
            if (!isLong) {
                row = reader.ReadUShort();
            }
            else {
                row = reader.ReadInt();
            }

            int col = reader.ReadByte();
            //log.Debug("row: {0} | col: {1}", row, col);

            if (row > 0 && col > 0)
            {
                // 컬럼별 크기
                List<string> colTypes = new List<string>();
                for (int i = 0; i < col; i++)
                {
                    int size = reader.ReadByte();
                    switch (size)
                    {
                        case 1:
                            colTypes.Add("byte");
                            break;
                        case 5:
                            colTypes.Add("int");
                            break;
                        case 8:
                            colTypes.Add("long");
                            break;
                        case 9:
                            colTypes.Add("single");
                            break;
                        case 11:
                            colTypes.Add("string");
                            break;
                        default:
                            colTypes.Add("unknown(" + size + ")");
                            break;
                    }
                }
                //log.Debug("column_info >> {0}", string.Join("|", colTypes));

                // 실제 정보가 있는 오프셋으로 이동
                if (startOffset <= 0)
                {
                    // 오프셋 찾기
                    reader.ReadInt();               // ??
                    startOffset = reader.ReadInt(); // 오프셋
                    //log.Debug("start_offset >> {0}", startOffset);
                }
                reader._offset = startOffset;

                // 컬럼명 가져오기
                List<string> colNames = null;
                if (File.Exists(@"STCFormat\" + clientVersion + "\\" + Path.GetFileNameWithoutExtension(stcFile) + ".format"))
                    colNames = File.ReadAllLines(@"STCFormat\" + clientVersion + "\\" + Path.GetFileNameWithoutExtension(stcFile) + ".format").ToList();
                else
                    log.Warn("Format file does not exist >> {0}", @"STCFormat\" + clientVersion + "\\" + Path.GetFileNameWithoutExtension(stcFile) + ".format");

                try
                {
                    for (int r = 0; r < row; r++)
                    {
                        JObject item = new JObject();
                        // 컬럼별 데이터 추출
                        for (int c = 0; c < col; c++)
                        {
                            string key = "";
                            string type = "";
                            if (colNames != null && c < colNames.Count())
                                key = colNames[c];                  // 컬럼명 설정
                            if (string.IsNullOrEmpty(key))
                                key = string.Format("__{0}", c);    // 컬럼명 알 수 없음
                            if (colTypes != null && c < colTypes.Count())
                                type = colTypes[c];
                            switch (type)
                            {
                                case "byte":
                                    item.Add(key, reader.ReadByte());
                                    break;
                                case "int":
                                    item.Add(key, reader.ReadInt());
                                    break;
                                case "long":
                                    item.Add(key, reader.ReadLong());
                                    break;
                                case "single":
                                    item.Add(key, Math.Round(reader.ReadSingle(), 2)); // 소수점 2자리까지 표시
                                    break;
                                case "string":
                                    item.Add(key, reader.ReadString());
                                    break;
                                case "unknown":
                                default:
                                    // 알 수 없는 타입이 발견될 경우 Hex 구조 확인 후 케이스 추가할 것
                                    log.Warn("unknown type >> {0}", c);
                                    break;
                            }
                        }
                        output.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            return output;
        }

        #endregion

        static void Main(string[] args)
        {

            bool validRegion = false;
            string region = null;

            while (!validRegion)
            {
                Console.WriteLine("Enter one of the following regions to download data for: kr, en, jp, ch, tw");
                region = Console.ReadLine();

                if (new[] { "kr", "en", "jp", "ch", "tw" }.Contains(region))
                {
                    validRegion = true;
                }
                else
                {
                    Console.WriteLine("Invalid region, please try again.");
                }
            }

            Stopwatch swh = new Stopwatch();
            swh.Start();

            var clientVersion = "";
            switch (region)
            {
                case "kr":
                    Console.WriteLine("\n====KR Data download====");
                    Downloader kr = new Downloader("kr");
                    kr.downloadStc();
                    kr.downloadAsset("kr");
                    clientVersion = "3080";
                    break;
                case "en":
                    Console.WriteLine("\n====EN Data download====");
                    Downloader en = new Downloader("en");
                    en.downloadStc();
                    en.downloadAsset("en");
                    clientVersion = "3070";
                    break;
                case "jp":
                    Console.WriteLine("\n====JP Data download====");
                    Downloader jp = new Downloader("jp");
                    jp.downloadStc();
                    jp.downloadAsset("jp");
                    clientVersion = "3070";
                    break;
                case "ch":
                    Console.WriteLine("\n====CN Data download====");
                    Downloader ch = new Downloader("ch");
                    ch.downloadStc();
                    ch.downloadAsset("ch");
                    clientVersion = "3080";
                    break;
				case "tw":
                    Console.WriteLine("\n====TW Data download====");
                    Downloader tw = new Downloader("tw");
                    tw.downloadStc();
                    tw.downloadAsset("tw");
                    clientVersion = "3080";
                    break;
            }

            #region NLog Configuration
            var config = new LoggingConfiguration();

            var logconsole = new ColoredConsoleTarget("logconsole");
            logconsole.Layout = new SimpleLayout() { Text = "${message} ${exception:format=tostring}" };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);

            LogManager.Configuration = config;
            #endregion

            Console.OutputEncoding = Encoding.UTF8;

            // catchdata.dat 복호화
            try
            {
                // 복호화
                log.Info(".dat decrypt >> {0}", "catchdata.dat");
                byte[] data = File.ReadAllBytes("stc\\catchdata.dat");
                byte[] key = Encoding.ASCII.GetBytes("c88d016d261eb80ce4d6e41a510d4048");
                string output = DatFileDecompress(data, key);

                // 폴더 생성
                if (!Directory.Exists("output\\" + region + "\\catchdata"))
                    Directory.CreateDirectory("output\\" + region + "\\catchdata");
                File.WriteAllText("output\\" + region + "\\catchdata\\catchdata.txt", output);

                log.Info("\n Exporting dat file contents");

                // 정보별 추출
                string[] lines = output.Split('\n').Select(p => p.Trim()).ToArray();
                foreach (string line in lines)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(line))
                            continue;
                        JObject json = JObject.Parse(line);
                        string jKey = json.Properties().Select(p => p.Name).FirstOrDefault();

                        log.Debug(".dat export >> " + jKey);
                        File.WriteAllText("output\\" + region + "\\catchdata\\" + jKey + ".json", json.ToString());
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }

                // 폴더 열기
                //Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\output_catchdata");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            // *.stc 파싱
            try
            {
                // 폴더 생성
                if (!Directory.Exists("output\\" + region + "\\stc"))
                    Directory.CreateDirectory("output\\" + region + "\\stc");

                // .stc 파일 목록
                // [input_file_name, output_file_name]
                // ※ 항목 추가 시 STCFormat\\*.format 파일에 컬럼 목록 추가할 것!
                Dictionary<string, string> stcFiles = new Dictionary<string, string>()
                {
					{ "5000.stc", "item" },
					{ "5001.stc", "battle_skill_config" },
					{ "5002.stc", "spot" },
					{ "5003.stc", "enemy_in_team" },
					{ "5004.stc", "gun_in_ally" },
					{ "5005.stc", "gun" },
					{ "5006.stc", "squad" },
					{ "5007.stc", "squad_advanced_bonus" },
					{ "5008.stc", "squad_chip" },
					{ "5009.stc", "squad_cpu" },
					{ "5010.stc", "squad_color" },
					{ "5011.stc", "squad_grid" },
					{ "5012.stc", "squad_data_daily" },
					{ "5013.stc", "squad_exp" },
					{ "5014.stc", "live2d" },
					{ "5015.stc", "building" },
					{ "5016.stc", "mission" },
					{ "5017.stc", "battle_creation" },
					{ "5018.stc", "squad_chip_exp" },
					{ "5019.stc", "squad_cpu_completion" },
					{ "5020.stc", "squad_rank" },
					{ "5021.stc", "squad_standard_attribution" },
					{ "5022.stc", "squad_type" },
					{ "5023.stc", "battle_buff" },
					{ "5024.stc", "battle_hurt_config" },
					{ "5025.stc", "enemy_character_type" },
					{ "5026.stc", "enemy_standard_attribute" },
					{ "5027.stc", "summoner" },
					{ "5028.stc", "battle_trigger" },
					{ "5029.stc", "battle_target_select_ai" },
					{ "5030.stc", "spot_buff_config" },
					{ "5031.stc", "special_spot_config" },
					{ "5032.stc", "mission_hurt_config" },
					{ "5033.stc", "carnival_task_type" },
					{ "5034.stc", "bingo_task_type" },
					{ "5035.stc", "enemy_team" },
					{ "5036.stc", "battle_formula" },
					{ "5037.stc", "live2d_motions" },
					{ "5038.stc", "equip" },
					{ "5039.stc", "auto_mission" },
					{ "5040.stc", "theater" },
					{ "5041.stc", "theater_area" },
					{ "5042.stc", "theater_construction" },
					{ "5043.stc", "theater_event" },
					{ "5044.stc", "story_util" },
					{ "5045.stc", "trigger_index" },
					{ "5046.stc", "mission_skill_config" },
					{ "5047.stc", "mission_mapped" },
					{ "5048.stc", "skin" },
					{ "5049.stc", "extra_spine" },
					{ "5050.stc", "explore_script" },
					{ "5051.stc", "explore_affair_client" },
					{ "5052.stc", "explore_area" },
					{ "5053.stc", "explore_destination" },
					{ "5054.stc", "explore_item" },
					{ "5055.stc", "explore_mall" },
					{ "5056.stc", "explore_time_type" },
					{ "5057.stc", "theater_effect" },
					{ "5058.stc", "battle_action_config" },
					{ "5059.stc", "theater_selection" },
					{ "5060.stc", "explore_affair_server" },
					{ "5061.stc", "ally_team" },
					{ "5062.stc", "theater_incident" },
					{ "5063.stc", "manual_ui" },
					{ "5064.stc", "dorm_ai" },
					{ "5065.stc", "dorm_action" },
					{ "5066.stc", "theater_reward" },
					{ "5067.stc", "mission_buff_config" },
					{ "5068.stc", "recommended_formula" },
					{ "5069.stc", "achievement" },
					{ "5070.stc", "friend_cosmetic" },
					{ "5071.stc", "furniture" },
					{ "5072.stc", "furniture_interact_point" },
					{ "5073.stc", "mission_win_type_config" },
					{ "5074.stc", "mission_event" },
					{ "5075.stc", "tutorial_guide" },
					{ "5076.stc", "tutorial_manual" },
					{ "5079.stc", "prize" },
					{ "5080.stc", "mall" },
					{ "5081.stc", "commander_class" },
					{ "5082.stc", "commander_emoji" },
					{ "5083.stc", "commander_uniform" },
					{ "5084.stc", "function_skill_config" },
					{ "5086.stc", "draw_event" },
					{ "5087.stc", "gift_item" },
					{ "5088.stc", "unit_character" },
					{ "5089.stc", "team_ai" },
					{ "5090.stc", "enemy_illustration" },
					{ "5091.stc", "fetter_skill" },
					{ "5092.stc", "enemy_illustration_skill" },
					{ "5093.stc", "sangvis_chip" },
					{ "5094.stc", "sangvis" },
					{ "5095.stc", "sangvis_advance" },
					{ "5096.stc", "sangvis_resolution" },
					{ "5097.stc", "sangvis_type" },
					{ "5098.stc", "sangvis_chip_skill" },
					{ "5099.stc", "sangvis_gasha" },
					{ "5100.stc", "sangvis_gasha_reward" },
					{ "5101.stc", "sangvis_sign" },
					{ "5102.stc", "sangvis_charavoice" },
					{ "5103.stc", "sangvis_character_type" },
					{ "5104.stc", "sangvis_exchange_mall" },
					{ "5105.stc", "sangvis_exp" },
					{ "5106.stc", "mission_win_step_control" },
					{ "5107.stc", "sangvis_in_ally" },
					{ "5108.stc", "mail_content" },
					{ "5109.stc", "auto_formation" },
					{ "5110.stc", "fetter" },
					{ "5111.stc", "fetter_story" },
					{ "5112.stc", "fetter_bounty" },
					{ "5113.stc", "organization" },
					{ "5114.stc", "organization_bounty" },
					{ "5115.stc", "fairy" },
					{ "5116.stc", "mission_targettrain" },
					{ "5117.stc", "mission_targettrain_battlesetting" },
					{ "5118.stc", "mission_targettrain_enemy" },
					{ "5119.stc", "equip_group" },
					{ "5120.stc", "mission_echo_info" },
					{ "5121.stc", "rank" },
					{ "5122.stc", "mission_draw_bonus" },
					{ "5123.stc", "event_prize_level" },
					{ "5124.stc", "bondage_lines" },
					{ "5125.stc", "gun_charavoice" },
					{ "5126.stc", "guild_emoji" },
					{ "5127.stc", "guild_emoji_group" },
					{ "5128.stc", "chat_fix_phrases" },
					{ "5129.stc", "chat_channel" },
					{ "5130.stc", "item_access" },
					{ "5131.stc", "npc_charavoice" },
					{ "5132.stc", "npc" },
					{ "5133.stc", "skin_class" },
					{ "5134.stc", "chess_gun_type" },
					{ "5135.stc", "chess_camp_type" },
					{ "5136.stc", "mall_classification" },
					{ "5137.stc", "point_mall" },
					{ "5138.stc", "chess_buff" },
					{ "5139.stc", "chess_chip" },
					{ "5140.stc", "chess_chip_target_select" },
					{ "5141.stc", "chess_creation_perform" },
					{ "5142.stc", "chess_enemy" },
					{ "5143.stc", "chess_skill" },
					{ "5144.stc", "chess_skill_trigger" },
					{ "5145.stc", "chess_spot" },
					{ "5146.stc", "chess_game_config" },
					{ "5147.stc", "chess_map" },
					{ "5148.stc", "chess_model" },
					{ "5149.stc", "chess_mission" },
					{ "5150.stc", "mission_entrance_package" },
					{ "5151.stc", "chess_creation_logic" },
					{ "5152.stc", "chess_scorelevel" },
					{ "5153.stc", "chess_random_enemy" },
					{ "5154.stc", "chess_random_spot" },
					{ "5155.stc", "chess_selectframe" },
					{ "5156.stc", "rouge_sk" },
					{ "5157.stc", "chess_seasonevent" },
					{ "5158.stc", "fight_success_condition" },
					{ "5159.stc", "chess_choice_stage" },
					{ "5160.stc", "equip_type" },
					{ "5161.stc", "equip_category" },
					{ "5162.stc", "fight_environment_skill" },
					{ "5163.stc", "fight_type" },
					{ "5164.stc", "chess_voice" },
					{ "5165.stc", "squad_in_ally" },
					{ "5166.stc", "mission_effect_config" },
					{ "5167.stc", "career_quest" },
					{ "5168.stc", "career_quest_group" },
					{ "5169.stc", "career_quest_grade" },
					{ "5170.stc", "fairy_talent" },
					{ "5171.stc", "fairy_talent_type" },
					{ "5172.stc", "furniture_classes" },
					{ "5173.stc", "fight_environment_config" },
					{ "5174.stc", "commander_ranking_scores" },
					{ "5175.stc", "commander_ranking_types" },
					{ "5176.stc", "battle_skill_type_config" },
					{ "5177.stc", "medal" },
					{ "5178.stc", "event_rank" },
					{ "5179.stc", "chess_gasha_reward" },
					{ "5180.stc", "present" },
					{ "5181.stc", "gun_obtain" },
					{ "5182.stc", "drop_content" },
					{ "5183.stc", "drop_package" },
					{ "5184.stc", "organization_level" },
					{ "5185.stc", "related_story" },
					{ "5186.stc", "story_playback" },
					{ "5187.stc", "event_battlepass" },
					{ "5188.stc", "daily" },
					{ "5189.stc", "weekly" },
					{ "5190.stc", "battle_watch" },
					{ "5191.stc", "battle_watch_trigger" },
					{ "5192.stc", "fairy_type" },
					{ "5193.stc", "fairy_charavoice" },
					{ "5194.stc", "fairy_in_ally" },
					{ "5195.stc", "daily_map" },
					{ "5196.stc", "gun_tag" },
					{ "5197.stc", "auto_formation_sangvis" },
					{ "5198.stc", "recommended_team" },
					{ "5199.stc", "team_tag" },
					{ "5200.stc", "daily_map_light" },
					{ "5201.stc", "daily_map_model" },
					{ "5202.stc", "daily_mission" },
					{ "5203.stc", "daily_mission_group" },
					{ "5204.stc", "return_player" },
					{ "5205.stc", "return_player_task" },
					{ "5206.stc", "daily_note" },
					{ "5207.stc", "daily_gunequip_obtain" },
					{ "5208.stc", "daily_map_spot" },
					{ "5209.stc", "daily_pass" },
					{ "5210.stc", "daily_event" },
					{ "5211.stc", "daily_event_option" },
					{ "5212.stc", "daily_mission_difficulty_group" },
					{ "5213.stc", "event_lottery" },
					{ "5214.stc", "lottery" },
					{ "5215.stc", "vehicle" },
					{ "5216.stc", "vehicle_component_roll" },
					{ "5217.stc", "vehicle_component_att_value" },
					{ "5218.stc", "vehicle_component" },
					{ "5219.stc", "vehicle_component_type" },
					{ "5220.stc", "vehicle_crew" },
					{ "5221.stc", "vehicle_tech_tree" },
					{ "5222.stc", "vehicle_skin" },
					{ "5223.stc", "vehicle_component_roll_type" },
					{ "5224.stc", "vehicle_in_ally" },
					{ "5225.stc", "vehicle_component_in_ally" },
					{ "5226.stc", "vehicle_unlock_task" },
					{ "5227.stc", "vehicle_component_group" },
					{ "5228.stc", "vehicle_type" },
					{ "5229.stc", "adjutant_skin" },
					{ "5230.stc", "daily_map_route" },
					{ "5231.stc", "daily_exmission_group" },
					{ "5232.stc", "mission_intensify_item_config" },
					{ "5233.stc", "normal_activity" },
					{ "5234.stc", "carnival_task" },
					{ "5235.stc", "simulation_battle_reward" },
					{ "5236.stc", "vehicle_skin_class" },
					{ "5237.stc", "letter" },
					{ "5238.stc", "list_to_do" },
					{ "5239.stc", "breakout_drop" },
					{ "5240.stc", "breakout_enemy" },
					{ "5241.stc", "breakout_gun" },
					{ "5242.stc", "breakout_item" },
					{ "5243.stc", "breakout_phase" },
					{ "5244.stc", "breakout_skill" },
					{ "5245.stc", "breakout_trigger" },
					{ "5246.stc", "breakout_talk" },
					{ "5247.stc", "exmall_classification" },
					{ "5248.stc", "mission_skip" },
					{ "5249.stc", "cg_collection" },
					{ "5250.stc", "music_collection" },
                };

                log.Info("\n Parsing stc files");

                foreach (KeyValuePair<string, string> stcFile in stcFiles)
                {
                    if(File.Exists("stc\\" + stcFile.Key))
                    {
                        log.Info(".stc parse >> file: {0} | type: {1}", stcFile.Key, stcFile.Value);

                        JArray jArr = ParseStc(stcFile.Key, clientVersion, 0, int.Parse(clientVersion) >= 3020);
                        string outputName = stcFile.Value;
                        string stcJSON = stcFile.Key.Replace(".stc", ".json");
                        if (string.IsNullOrEmpty(outputName))
                            outputName = Path.GetFileNameWithoutExtension(stcFile.Key);
                        File.WriteAllText("output\\" + region + "\\stc\\" + outputName + ".json", jArr.ToString());
                        File.Delete("output\\" + region + "\\stc\\" + stcJSON);
                    }
                    else
                    {
                        log.Info(stcFile.Key + " does not exist. Moving to next file.");
                    }
                }

                // 변환 작업에 필요한 정보
                JArray GunList = JArray.Parse(File.ReadAllText("output\\" + region + "\\stc\\gun.json"));
                JArray SkinList = JArray.Parse(File.ReadAllText("output\\" + region + "\\stc\\skin.json"));
                JArray BattleSkillConfigList = JArray.Parse(File.ReadAllText("output\\" + region + "\\stc\\battle_skill_config.json"));
                JArray MissionSkillConfigList = JArray.Parse(File.ReadAllText("output\\" + region + "\\stc\\mission_skill_config.json"));
                JArray EquipList = JArray.Parse(File.ReadAllText("output\\" + region + "\\stc\\equip.json"));

                Console.WriteLine("Obtaining text output");
                string exeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
                string Path1 = exeFolder + "Assets_raw\\" + region + "\\asset_textes.ab";
                string Path2 = exeFolder + "Assets_raw\\" + region + "\\asset_texttable.ab";
                string Path3 = exeFolder + "Assets_raw\\" + region + "\\asset_textavg.ab";
                string Path4 = exeFolder + "Assets_raw\\" + region + "\\asset_textlpatch.ab";

                if (!Directory.Exists("ResourceExtract\\text"))
                    Directory.CreateDirectory("ResourceExtract\\text");

                Process process = new Process();
                // Configure the process using the StartInfo properties.
                process.StartInfo.FileName = $"ResourceExtract\\girlsfrontline-resources-extract.exe";
                if (File.Exists("Assets_raw\\" + region + "\\asset_textlpatch.ab"))
                {
                    process.StartInfo.Arguments = Path1 + " " + Path2 + " " + Path3 + " " + Path4;
                }
                else
                {
                    process.StartInfo.Arguments = Path1 + " " + Path2 + " " + Path3;
                }
                process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                process.Start();
                process.WaitForExit();// Waits here for the process to exit.

                /* decrypt lua files
                 *
                 * Girls' Frontline .lua patch files are simply XOR-encrypted.
                 *
                 * BUT the software used to extract text assets does some 𝒻𝓊𝓃𝓀𝓎 business.
                 * Text files are either exported as .bytes or as .txt.
                 * Files exported as .txt have extra bytes that appear out of thin air,
                 * other asset bundle extractors (e.g. AssetStudio) do not have this issue.
                 * In particular, 0D (CR) is replaced with 0D0A (CRLF, Windows-style).
                 *
                 * We have to revert this "feature" before XOR-decrypting (not for .bytes files).
                 */
                var luaFiles = Directory.GetFiles("ResourceExtract\\text\\luapatch", "*.*", SearchOption.AllDirectories);
                var luaKey = Encoding.UTF8.GetBytes("lvbb3zfc3faa8mq1rx0r0gl61b4338fa");
                foreach (var luaFile in luaFiles)
                {
                    var luaBytes = File.ReadAllBytes(luaFile);

                    var decryptedBytes = Enumerable.Range(0, luaBytes.Length)
                                                   .Where(byteIndex => luaFile.EndsWith(".bytes") || luaBytes.Length - 1 <= byteIndex || 0xD != luaBytes[byteIndex] || 0xA != luaBytes[byteIndex + 1])
                                                   .Select((byteIndex, i) => (byte) (luaBytes[byteIndex] ^ luaKey[i % 32]))
                                                   .ToArray();

                    File.WriteAllBytes(luaFile, decryptedBytes);
                }

                try
                {
                    Directory.Move($"ResourceExtract\\text", "output\\" + region + "\\text");
                }
                catch
                {
                    Directory.Delete("output\\" + region + "\\text", true);
                    Directory.Move($"ResourceExtract\\text", "output\\" + region + "\\text");
                }



                /*
                //폴더생성
                if (!Directory.Exists("results"))
                    Directory.CreateDirectory("results");
                //doll.json 생성
                log.Info("\n Creating doll JSON");
                JsonUtil.getDollJson(GunList, SkinList, BattleSkillConfigList);
                //fairy.json 생성
                log.Info("\n Creating fairy JSON");
                JsonUtil.getFairyJson(BattleSkillConfigList, MissionSkillConfigList, region);
                //equip.json 생성
                log.Info("\n Creating equip JSON");
                JsonUtil.getEquipJson(EquipList);

                //textAsset2json
                //Console.WriteLine("\n==한섭 데이터 변환==");
                //JsonUtil.getTextAsset("kr");
                //JsonUtil.getDialogueText("kr");

                //Console.WriteLine("\n==글섭 데이터 변환==");
                //JsonUtil.getTextAsset("en");
                //JsonUtil.getDialogueText("en");

                //Console.WriteLine("\n==일섭 데이터 변환==");
                //JsonUtil.getTextAsset("jp");
                //JsonUtil.getDialogueText("jp");

                //Console.WriteLine("\n==중섭 데이터 변환==");
                //JsonUtil.getTextAsset("ch");
                //JsonUtil.getDialogueText("ch");

                // 폴더 열기
                //Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\output_stc");
                */
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            swh.Stop();
            Console.WriteLine("Completed in: " + swh.Elapsed.ToString());
            //Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            Console.ReadKey();
        }
    }
}
