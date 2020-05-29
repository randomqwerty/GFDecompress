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
        public static JArray ParseStc(string stcFile, int startOffset = 0)
        {
            JArray output = new JArray();

            // stc 읽기
            byte[] stcStream = File.ReadAllBytes("stc\\" + stcFile);
            StcBinaryReader reader = new StcBinaryReader(stcStream);

            int code = reader.ReadUShort();         // 코드 (예: 5005)
            reader.ReadUShort();                    // ??
            log.Debug("file: {0}, code: {1}", stcFile, code);

            int row = reader.ReadUShort();
            int col = reader.ReadByte();
            log.Debug("row: {0} | col: {1}", row, col);

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
                log.Debug("column_info >> {0}", string.Join("|", colTypes));

                // 실제 정보가 있는 오프셋으로 이동
                if (startOffset <= 0)
                {
                    // 오프셋 찾기
                    reader.ReadInt();               // ??
                    startOffset = reader.ReadInt(); // 오프셋
                    log.Debug("start_offset >> {0}", startOffset);
                }
                reader._offset = startOffset;

                // 컬럼명 가져오기
                List<string> colNames = null;
                if (File.Exists(@"STCFormat\" + Path.GetFileNameWithoutExtension(stcFile) + ".format"))
                    colNames = File.ReadAllLines(@"STCFormat\" + Path.GetFileNameWithoutExtension(stcFile) + ".format").ToList();
                else
                    log.Warn("Format not exists >> {0}", @"STCFormat\" + Path.GetFileNameWithoutExtension(stcFile) + ".format");

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
            Stopwatch swh = new Stopwatch();
            swh.Start();

            Downloader dl = new Downloader();
            dl.downloadStc();
            dl.downloadAsset();

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
                if (!Directory.Exists("output\\catchdata"))
                    Directory.CreateDirectory("output\\catchdata");
                File.WriteAllText("output\\catchdata\\catchdata.txt", output);

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
                        File.WriteAllText("output\\catchdata\\" + jKey + ".json", json.ToString());
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
                if (!Directory.Exists("output\\stc"))
                    Directory.CreateDirectory("output\\stc");

                // .stc 파일 목록
                // [input_file_name, output_file_name]
                // ※ 항목 추가 시 STCFormat\\*.format 파일에 컬럼 목록 추가할 것!
                Dictionary<string, string> stcFiles = new Dictionary<string, string>()
                {
                    { "5000.stc", "item" },                         // 아이템
                    { "5001.stc", "battle_skill_config" },          // 전투 스킬
                    { "5002.stc", "spot" },                         // 거점
                    { "5003.stc", "enemy_in_team" },                // 적 제대 멤버
                    { "5004.stc", "gun_in_ally" },                  // 아군 제대 멤버
                    { "5005.stc", "gun" },                          // 인형

                    { "5006.stc", "squad" },                        // 화력소대
                    { "5007.stc", "squad_advanced_bonus" },         // 화력소대 승진
                    { "5008.stc", "squad_chip" },                   // 화력소대 칩셋
                    { "5009.stc", "squad_cpu" },                    // 화력소대 회로?
                    { "5010.stc", "squad_color" },                  // 화력소대 칩셋 색상
                    { "5011.stc", "squad_grid" },                   // 화력소대 칩셋 모양
                    { "5012.stc", "squad_data_daily" },             // 화력소대 정보임무
                    { "5013.stc", "squad_exp" },                    // 화력소대 경험치

                    { "5015.stc", "building" },                     // 건물
                    { "5016.stc", "mission" },                      // 전역
                    { "5017.stc", "battle_creation" },
                    { "5018.stc", "squad_chip_exp" },               // 화력소대 칩셋 경험치
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
                    { "5036.stc", "" },
                    { "5037.stc", "" },
                    { "5038.stc", "equip" },                        // 장비
                    { "5039.stc", "" },                             // 자율작전?
                    { "5040.stc", "theater" },
                    { "5041.stc", "theater_area" },
                    { "5042.stc", "theater_construction" },
                    { "5043.stc", "theater_event" },
                    { "5044.stc", "" },
                    { "5045.stc", "" },
                    { "5046.stc", "mission_skill_config" },
                    { "5047.stc", "" },
                    { "5048.stc", "skin" },                         // 스킨
                    { "5049.stc", "" },
                    { "5050.stc", "" },
                    { "5051.stc", "explore_affair_client" },
                    { "5052.stc", "explore_area" },
                    { "5053.stc", "" },
                    { "5054.stc", "" },
                    { "5055.stc", "" },
                    { "5056.stc", "" },
                    { "5057.stc", "theater_effect" },
                    { "5058.stc", "" },
                    { "5059.stc", "theater_selection" },
                    { "5060.stc", "explore_affair_server" },
                    { "5061.stc", "" },
                    { "5062.stc", "theater_incident" },
                    { "5063.stc", "" },
                    { "5064.stc", "" },
                    { "5065.stc", "" },
                    { "5066.stc", "" },
                    { "5067.stc", "mission_buff_config" },
                    { "5068.stc", "recommend_formula" },
                    { "5069.stc", "achivement" },
                    { "5070.stc", "" },
                    { "5071.stc", "" },
                    { "5072.stc", "" },
                    { "5073.stc", "mission_win_type_config" },
                    { "5074.stc", "" },
                    { "5075.stc", "" },
                    { "5076.stc", "" },
                    { "5077.stc", "" },
                    { "5078.stc", "guild_level" },
                    { "5079.stc", "prize" },
                    { "5080.stc", "mall" },
                    { "5081.stc", "commander_class" },
                    { "5082.stc", "emoji" },
                    { "5083.stc", "commander_uniform" },
                    { "5084.stc", "function_skill_config" },
                    { "5085.stc", "" },
                    { "5086.stc", "" },
                    { "5087.stc", "gift" },
                    { "5088.stc", "" },
                    { "5089.stc", "" },
                    { "5090.stc", "" },
                    { "5092.stc", "enemy_illustration_skill" },
                    /*
                     * 혼합세력 능력치/수복 관련자료 링크
                     * 계산식, 엑셀시트
                     * https://bbs.nga.cn/read.php?tid=20891117
                     */
                    { "5093.stc", "sangvis_chip" },                 // 혼합세력 칩셋
                    { "5094.stc", "sangvis" },                      // 혼합세력
                    { "5095.stc", "sangvis_advance" },              // 혼합세력 분석(편확)
                    { "5096.stc", "sangvis_resolution" },           // 혼합세력 개발(강화)
                    { "5097.stc", "sangvis_type" },                 // 혼합세력 종류
                    { "5098.stc", "" },
                    { "5099.stc", "sangvis_gasha" },
                    { "5100.stc", "" },
                    { "5101.stc", "sangvis_logo" },
                    { "5102.stc", "sangvis_char_voice" },
                    { "5103.stc", "sangvis_character_type" },
                    { "5104.stc", "" },
                    { "5105.stc", "" }
                };

                foreach (KeyValuePair<string, string> stcFile in stcFiles)
                {
                    log.Info(".stc parse >> file: {0} | type: {1}", stcFile.Key, stcFile.Value);

                    JArray jArr = ParseStc(stcFile.Key);
                    string outputName = stcFile.Value;
                    if (string.IsNullOrEmpty(outputName))
                        outputName = Path.GetFileNameWithoutExtension(stcFile.Key);
                    File.WriteAllText("output\\stc\\" + outputName + ".json", jArr.ToString());
                }

                // 변환 작업에 필요한 정보
                JArray GunList = JArray.Parse(File.ReadAllText("output\\stc\\gun.json"));
                JArray SkinList = JArray.Parse(File.ReadAllText("output\\stc\\skin.json"));
                JArray BattleSkillConfigList = JArray.Parse(File.ReadAllText("output\\stc\\battle_skill_config.json"));
                JArray MissionSkillConfigList = JArray.Parse(File.ReadAllText("output\\stc\\mission_skill_config.json"));
                JArray EquipList = JArray.Parse(File.ReadAllText("output\\stc\\equip.json"));

                //폴더생성
                if (!Directory.Exists("results"))
                    Directory.CreateDirectory("results");
                //doll.json 생성
                JsonUtil.getDollJson(GunList, SkinList, BattleSkillConfigList);
                //fairy.json 생성
                JsonUtil.getFairyJson(BattleSkillConfigList, MissionSkillConfigList);
                //equip.json 생성
                JsonUtil.getEquipJson(EquipList);

                //textAsset2json
                JsonUtil.getTextAsset("kr");


                // 폴더 열기
                //Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\output_stc");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            swh.Stop();
            Console.WriteLine("소요시간: " + swh.Elapsed.ToString());
            //Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }
    }
}
