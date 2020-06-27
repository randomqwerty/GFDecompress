using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GFDecompress
{
    //타입 정보
    class TypeData
    {
        public static string getDollType(int _code)
        {
            switch (_code)
            {
                case 0:
                    return "all";
                case 1:
                    return "hg";
                case 2:
                    return "smg";
                case 3:
                    return "rf";
                case 4:
                    return "ar";
                case 5:
                    return "mg";
                case 6:
                    return "sg";
                default:
                    return "undefined";
            }
        }

        public static string getEquipType(int _code) {
            switch (_code) {
                case 0:
                    return "none";
                case 1:
                    return "optical";
                case 2:
                    return "holo";
                case 3:
                    return "reddot";
                case 4:
                    return "nightvision";
                case 5:
                    return "apAmmo";
                case 6:
                    return "hpAmmo";
                case 7:
                    return "shotgunShell";
                case 8:
                    return "hvAmmo";
                case 9:
                    return "chip";
                case 10:
                    return "exoSkeleton";
                case 11:
                    return "armorPlate";
                case 12:
                    return "medal";
                case 13:
                    return "suppressor";
                case 14:
                    return "ammunitionBox";
                case 15:
                    return "cloak";
                case 16:
                    return "spPart";
                case 17:
                    return "spClip";
                default:
                    return "undefined";
            }
        }

        public static string getEquipCategory(int _code) {
            switch (_code) {
                case 1:
                    return "accessory";
                case 2:
                    return "ammo";
                case 3:
                    return "doll";
                default:
                    return "undefined";

            }
        }

        public static string getFairyCatetory(int _code) {
            switch (_code) {
                case 1:
                    return "battle";
                case 2:
                    return "strategy";
                default:
                    return "dummy";
            }
        }

        public static string getSquadType(int _code) {
            switch (_code) {
                case 1:
                    return "ATW";
                case 2:
                    return "MTR";
                case 3:
                    return "AGL";
                default:
                    return "undefined";
            }
        }
    }

    class Grid {
        public static int readPos(int _clientValue) {
            switch (_clientValue) {
                case 7:
                    return 1;
                case 8:
                    return 4;
                case 9:
                    return 7;
                case 12:
                    return 2;
                case 13:
                    return 5;
                case 14:
                    return 8;
                case 17:
                    return 3;
                case 18:
                    return 6;
                case 19:
                    return 9;
                default:
                    return -1; //에러코드임 아무튼 에러코드임
            }
        }

        public static int gridCenter(int _dataValue) {
            switch (_dataValue)
            {
                case 7:
                    return 9;
                case 8:
                    return 6;
                case 9:
                    return 3;
                case 12:
                    return 8;
                case 13:
                    return 5;
                case 14:
                    return 2;
                case 17:
                    return 7;
                case 18:
                    return 4;
                case 19:
                    return 1;
                default:
                    return -1; //에러코드임 아무튼 에러코드임
            }
        }

        public static string readEffectType(int _value) {
            switch (_value) {
                case 1:
                    return "pow";
                case 2:
                    return "rate";
                case 3:
                    return "hit";
                case 4:
                    return "dodge";
                case 5:
                    return "criticalPercent";
                case 6:
                    return "cooldown";
                case 7:
                    return "bullet";
                case 8:
                    return "armor";
                case 9:
                    return "nightview";
                default:
                    return "undefined";
            }
        }
    }
    //인형 데이터 클래스
    public class GunData {
        public int id;
        public int rank;
        public string type;
        public int buildTime;
        public JArray skins = new JArray();
        public JObject stats = new JObject();
        public JObject effect = new JObject()
        {
            {"effectType", "all"},
            {"effectCenter",5},
            {"effectPos", new JArray()},
            {"gridEffect", new JObject()}
        };
        public int grow;
        public string codename;
        public JObject skill1 = new JObject()
        {
            {"id",0},
            {"codename", ""},
            {"cooldownType",""},
            {"initialCooldown",0},
            {"dataPool", new JArray()},
            {"consumption", 0},
        };
        public JObject skill2 = new JObject()
        {
            {"id",0},
            {"codename", ""},
            {"cooldownType",""},
            {"initialCooldown",0},
            {"dataPool", new JArray()},
            {"consumption", 0},
        };
        public JArray obtain = new JArray();
        public JArray equip1 = new JArray();
        public JArray equip2 = new JArray();
        public JArray equip3 = new JArray();
        public JArray mindupdate = new JArray() {
            new JObject(){
                {"core",0},
                {"mempiece",0}
            },
            new JObject(){
                {"core",0},
                {"mempiece",0}
            },
            new JObject(){
                {"core",0},
                {"mempiece",0}
            }
        };
        public string tag;

        //생성자
        public GunData(JObject _gunList, JArray _skinList, JArray _skillList) {
            //id
            id = _gunList["id"].ToObject<int>();
            //레어도
            rank = _gunList["rank_display"].ToObject<int>();
            //병종
            type = TypeData.getDollType(_gunList["type"].ToObject<int>());
            //제조시간
            buildTime = _gunList["develop_duration"].ToObject<int>();

            Console.WriteLine(id.ToString() + "번 인형데이터 변환 중...");

            //스킨정보
            foreach (JToken element in _skinList) {
                if (element.ToObject<JObject>()["fit_gun"].ToObject<int>() == id)
                    skins.Add(element.ToObject<JObject>()["id"].ToObject<int>());
            }

            //스탯정보
            stats.Add("hp", _gunList["ratio_life"]); //화력
            stats.Add("pow", _gunList["ratio_pow"]); //사속
            stats.Add("hit", _gunList["ratio_hit"]); //명중
            stats.Add("dodge", _gunList["ratio_dodge"]); //회피
            stats.Add("speed", _gunList["ratio_speed"]); //이동속도
            stats.Add("rate", _gunList["ratio_rate"]); //사속
            stats.Add("armorPiercing", _gunList["armor_piercing"]); //장갑관통
            stats.Add("criticalPercent", _gunList["crit"]); //치명률
            if (_gunList["ratio_armor"].ToObject<int>() != 0)
                stats.Add("armor", _gunList["ratio_armor"]); //장갑

            //진형버프
            try
            {
                effect["effectType"] = TypeData.getDollType(_gunList["effect_guntype"].ToObject<int>());
            }
            catch {
                //복수 버프시 배열로 저장
                JArray typeArr = new JArray();
                string[] typeTmp = _gunList["effect_guntype"].ToString().Split(',');
                foreach (string str in typeTmp) {
                    typeArr.Add(TypeData.getDollType(int.Parse(str)));
                }
                effect["effectType"] = typeArr;
            }
            effect["effectCenter"] = Grid.gridCenter(_gunList["effect_grid_center"].ToObject<int>());

            string[] effectPos = _gunList["effect_grid_pos"].ToString().Split(',');
            JArray tmpArr = new JArray();
            foreach (string str in effectPos) {
                tmpArr.Add(Grid.readPos(int.Parse(str) + 13 - _gunList["effect_grid_center"].ToObject<int>()));
                //effect["effectPos"].ToObject<JArray>().Add(Grid.readPos(int.Parse(str))); //진형버프 위치
            }
            effect["effectPos"] = tmpArr;

            string[] gridEffect = _gunList["effect_grid_effect"].ToString().Split(';');
            JObject tmpObj = new JObject();
            foreach (string str in gridEffect) {
                string[] statPair = str.Split(',');
                tmpObj.Add(Grid.readEffectType(int.Parse(statPair[0])), int.Parse(statPair[1]));
                //effect["gridEffect"].ToObject<JObject>().Add(Grid.readEffectType(int.Parse(statPair[0])),int.Parse(statPair[1])); //진형버프 스탯 및 수치
            }
            effect["gridEffect"] = tmpObj;

            //성장계수
            grow = _gunList["eat_ratio"].ToObject<int>();
            //코드명
            codename = _gunList["code"].ToString();
            //스킬1
            skill1["id"] = _gunList["skill1"].ToString();
            //스킬1 데이터
            try
            {
                JArray datapool = new JArray();
                var items = _skillList.SelectTokens($"$[?(@.skill_group_id== {skill1["id"].ToString()})]");

                foreach (var element in items) {
                    skill1["codename"] = element.ToObject<JObject>()["code"];
                    skill1["initialCooldown"] = element.ToObject<JObject>()["start_cd_time"].ToObject<int>();
                    if (element.ToObject<JObject>()["cd_type"].ToObject<int>() == 1)
                        skill1["cooldownType"] = "frame";
                    else
                        skill1["cooldownType"] = "turn";

                    JObject skill = new JObject();
                    skill.Add("level", element.ToObject<JObject>()["level"].ToObject<int>());
                    skill.Add("cooldown", element.ToObject<JObject>()["cd_time"].ToObject<int>());
                    datapool.Add(skill);

                    skill["consumption"] = element.ToObject<JObject>()["consumption"].ToObject<int>();
                }

                /*foreach (JToken element in _skillList)
                {
                    if (element.ToObject<JObject>()["id"].ToString().StartsWith(skill1["id"].ToString())) {
                        //Console.WriteLine(element.ToObject<JObject>()["id"].ToString());
                        skill1["codename"] = element.ToObject<JObject>()["code"];
                        skill1["initialCooldown"] = element.ToObject<JObject>()["start_cd_time"].ToObject<int>();
                        if (element.ToObject<JObject>()["cd_type"].ToObject<int>() == 1)
                            skill1["cooldownType"] = "frame";
                        else
                            skill1["cooldownType"] = "turn";

                        JObject skill = new JObject();
                        skill.Add("level", element.ToObject<JObject>()["level"].ToObject<int>());
                        skill.Add("cooldown", element.ToObject<JObject>()["cd_time"].ToObject<int>());
                        datapool.Add(skill);
                    }
                }*/
                skill1["dataPool"] = datapool;
            }
            catch { }

            if (id > 20000)
            {
                //스킬2
                skill2["id"] = _gunList["skill2"].ToString();
                //스킬2 데이터
                try
                {
                    JArray datapool = new JArray();
                    var items = _skillList.SelectTokens($"$[?(@.skill_group_id== {skill2["id"].ToString()})]");

                    foreach (var element in items)
                    {
                        skill2["codename"] = element.ToObject<JObject>()["code"];
                        skill2["initialCooldown"] = element.ToObject<JObject>()["start_cd_time"].ToObject<int>();
                        if (element.ToObject<JObject>()["cd_type"].ToObject<int>() == 1)
                            skill2["cooldownType"] = "frame";
                        else
                            skill2["cooldownType"] = "turn";

                        JObject skill = new JObject();
                        skill.Add("level", element.ToObject<JObject>()["level"].ToObject<int>());
                        skill.Add("cooldown", element.ToObject<JObject>()["cd_time"].ToObject<int>());
                        datapool.Add(skill);

                        skill["consumption"] = element.ToObject<JObject>()["consumption"].ToObject<int>();
                    }
                    /*foreach (JToken element in _skillList)
                    {
                        if (element.ToObject<JObject>()["id"].ToString().StartsWith(skill2["id"].ToString()))
                        {
                            //Console.WriteLine(element.ToObject<JObject>()["id"].ToString());
                            skill2["codename"] = element.ToObject<JObject>()["code"];
                            skill2["initialCooldown"] = element.ToObject<JObject>()["start_cd_time"].ToObject<int>();
                            if (element.ToObject<JObject>()["cd_type"].ToObject<int>() == 1)
                                skill2["cooldownType"] = "frame";
                            else
                                skill2["cooldownType"] = "turn";

                            JObject skill = new JObject();
                            skill.Add("level", element.ToObject<JObject>()["level"].ToObject<int>());
                            skill.Add("cooldown", element.ToObject<JObject>()["cd_time"].ToObject<int>());
                            datapool.Add(skill);
                        }
                    }*/
                    skill2["dataPool"] = datapool;
                }
                catch { }
            }
            else {
                skill2 = null;
            }

            //획득경로
            string[] _obtain = _gunList["obtain_ids"].ToString().Split(',');
            foreach (string str in _obtain) {
                obtain.Add(int.Parse(str));
            }

            //장비슬롯1
            string[] _equipt1 = _gunList["type_equip1"].ToString().Split(';')[1].Split(',');
            foreach (string str in _equipt1) {
                equip1.Add(TypeData.getEquipType(int.Parse(str)));
            }
            //장비슬롯2
            string[] _equipt2 = _gunList["type_equip2"].ToString().Split(';')[1].Split(',');
            foreach (string str in _equipt2)
            {
                equip2.Add(TypeData.getEquipType(int.Parse(str)));
            }
            //장비슬롯3
            string[] _equipt3 = _gunList["type_equip3"].ToString().Split(';')[1].Split(',');
            foreach (string str in _equipt3)
            {
                equip3.Add(TypeData.getEquipType(int.Parse(str)));
            }

            //<todo> 개조 소모재료 데이터 추가
            try {
                string[] consume = _gunList["mindupdate_consume"].ToString().Split(';');
                int cnt = 0;
                foreach (string str in consume)
                {
                    string[] tmp = str.Split(',');
                    mindupdate[cnt]["core"] = int.Parse(tmp[0].Split(':')[1]);
                    mindupdate[cnt]["mempiece"] = int.Parse(tmp[1].Split(':')[1]);
                    cnt++;
                }
            } catch {
                mindupdate = null;
            }

            //소속 태그
            if (_gunList["tag"].ToString() == "")
                tag = "team_griffin";
            else
                tag = _gunList["tag"].ToString();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }

    //요정 데이터 클래스
    public class FairyData {
        public int id;
        public string category;
        public JObject stats = new JObject();
        public JObject skill = new JObject()
        {
            {"id",0},
            {"codename", ""},
            {"cooldownType",""},
            {"initialCooldown",0},
            {"consumption", 0},
        };
        public int grow;
        public int buildTime;
        public string codename;
        public JObject powerup = new JObject();
        public int retireExp;
        public JArray qualityExp = new JArray();
        public JArray skins = new JArray();

        //생성자
        public FairyData(JObject _fairyList, JArray _skillLIst, JObject _skinList)
        {
            string[,] statList = {
                {
                    "pow",
                    "hit",
                    "dodge",
                    "armor",
                    "critical_harm_rate",
                    "armor_piercing"
                },
                {
                    "pow",
                    "hit",
                    "dodge",
                    "armor",
                    "criticalHarmRate",
                    "armorPiercing"
                }
            };

            id = _fairyList["id"].ToObject<int>();
            category = TypeData.getFairyCatetory(_fairyList["category"].ToObject<int>());
            Console.WriteLine(id.ToString() + "번 요정데이터 변환 중...");
            //스탯
            try
            {
                for (int i = 0; i < 6; ++i)
                {
                    if (_fairyList[statList[0, i]].ToString().Equals("0"))
                        continue;
                    string[] value = _fairyList[statList[0, i]].ToString().Split(',');
                    stats.Add(statList[1, i], _fairyList[statList[0, i]]);
                }
            }
            catch { }
            //스킬
            skill["id"] = _fairyList["skill_id"].ToString();
            try
            {
                JArray datapool = new JArray();
                if (skill["id"].ToString().StartsWith("*")) {
                    var items = _skillLIst[1].SelectTokens($"$[?(@.skill_group_id== {skill["id"].ToString().Substring(1)})]");
                    skill.Add("dataPool", new JArray());
                    foreach (var element in items)
                    {
                        skill["codename"] = element.ToObject<JObject>()["code"].ToString();
                        skill["initialCooldown"] = 0;
                        skill["cooldownType"] = "turn";

                        JObject tmpSkill = new JObject();
                        tmpSkill.Add("level", element.ToObject<JObject>()["level"].ToObject<int>());
                        tmpSkill.Add("cooldown", element.ToObject<JObject>()["cd_time"].ToObject<int>());
                        datapool.Add(tmpSkill);
                        skill["dataPool"] = datapool;

                        skill["consumption"] = element.ToObject<JObject>()["consumption"].ToObject<int>();
                    }
                }
                else {
                    var items = _skillLIst[0].SelectTokens($"$[?(@.skill_group_id== {skill["id"].ToString()})]");

                    foreach (var element in items)
                    {
                        skill["codename"] = element.ToObject<JObject>()["code"].ToString();
                        skill["initialCooldown"] = element.ToObject<JObject>()["start_cd_time"].ToObject<int>();
                        if (element.ToObject<JObject>()["cd_type"].ToObject<int>() == 1)
                            skill["cooldownType"] = "frame";
                        else
                            skill["cooldownType"] = "turn";

                        JObject tmpSkill = new JObject();
                        tmpSkill.Add("level", element.ToObject<JObject>()["level"].ToObject<int>());
                        tmpSkill.Add("cooldown", element.ToObject<JObject>()["cd_time"].ToObject<int>());
                        datapool.Add(tmpSkill);
                        skill["dataPool"] = datapool;

                        skill["consumption"] = element.ToObject<JObject>()["consumption"].ToObject<int>();
                    }
                }

                if (_fairyList["id"].ToObject<int>() == 15)
                    skill["id"] = "*13";
                if (_fairyList["id"].ToObject<int>() == 16)
                    skill["id"] = "*14";


                /*foreach (JToken element in _skillLIst)
                {
                    if (element.ToObject<JObject>()["skill_group_id"].ToString().Equals(this.skill["id"].ToString()))
                    {
                        //Console.WriteLine(element.ToObject<JObject>()["id"].ToString());
                        skill["codename"] = element.ToObject<JObject>()["code"].ToString();
                        skill["initialCooldown"] = element.ToObject<JObject>()["start_cd_time"].ToObject<int>();
                        if (element.ToObject<JObject>()["cd_type"].ToObject<int>() == 1)
                            skill["cooldownType"] = "frame";
                        else
                            skill["cooldownType"] = "turn";

                        JObject tmpSkill = new JObject();
                        tmpSkill.Add("level", element.ToObject<JObject>()["level"].ToObject<int>());
                        tmpSkill.Add("cooldown", element.ToObject<JObject>()["cd_time"].ToObject<int>());
                        datapool.Add(tmpSkill);
                    }
                }*/
            }
            catch {
                Console.WriteLine(id.ToString() + "번 요정 스킬 누락");
            }

            grow = _fairyList["grow"].ToObject<int>();
            buildTime = _fairyList["develop_duration"].ToObject<int>();
            codename = _fairyList["code"].ToString();
            //powerup
            powerup.Add("mp", _fairyList["powerup_mp"]);
            powerup.Add("ammo", _fairyList["powerup_ammo"]);
            powerup.Add("mre", _fairyList["powerup_mre"]);
            powerup.Add("part", _fairyList["powerup_part"]);

            retireExp = _fairyList["quality_exp"].ToObject<int>();

            foreach (string str in _fairyList["quality_need_number"].ToString().Split(',')) {
                qualityExp.Add(int.Parse(str.Split(':')[1]));
            }
            //스킨 추가
            foreach (JToken element in _skinList["fairy_skin_info"].ToObject<JArray>()) {
                if (element.ToObject<JObject>()["gift_fairy"].ToString().Equals(id.ToString())) {
                    JObject skin = new JObject()
                    {
                        {"id",element["id"]},
                        {"codename",element["pic_id"] }
                    };
                    this.skins.Add(skin);
                }
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }

    //장비 데이터 클래스
    public class EquipData {
        public int id;
        public string codename;
        public int rank;
        public string category;
        public string type;
        public string company;
        public JArray fitGuns = new JArray();
        public double exclusiveRate;
        public int maxLevel;
        public int buildTime;
        public JObject stats = new JObject();
        public JObject powerup = new JObject();

        //생성자
        public EquipData(JObject _obj) {
            string[] statList = {
                "pow",
                "hit",
                "dodge",
                "speed",
                "rate",
                "critical_harm_rate",
                "critical_percent",
                "armor_piercing",
                "armor",
                "shield",
                "damage_amplify",
                "damage_reduction",
                "night_view_percent",
                "bullet_number_up"
            };

            id = _obj["id"].ToObject<int>();
            codename = _obj["code"].ToString();
            rank = _obj["rank"].ToObject<int>();
            category = TypeData.getEquipCategory(_obj["category"].ToObject<int>());
            type = TypeData.getEquipType(_obj["type"].ToObject<int>());
            company = _obj["company"].ToString();

            Console.WriteLine(id.ToString() + "번 장비데이터 변환 중");
            try
            {
                if (_obj["fit_guns"].ToString().Equals(""))
                    fitGuns = null;
                string[] strFitgun = _obj["fit_guns"].ToString().Split(',');
                foreach (string str in strFitgun)
                {
                    fitGuns.Add(int.Parse(str));
                }
            }
            catch {
                fitGuns = null;
            }

            exclusiveRate = _obj["exclusive_rate"].ToObject<double>();
            //최대 레벨?
            maxLevel = _obj["max_level"].ToObject<int>();
            //제조 시간
            buildTime = _obj["develop_duration"].ToObject<int>();
            //스탯
            try
            {
                foreach (string statName in statList)
                {
                    if (_obj[statName].ToString().Equals(""))
                        continue;
                    string[] value = _obj[statName].ToString().Split(',');
                    JObject stat = new JObject();
                    stat.Add("min", value[0]);
                    stat.Add("max", value[1]);
                    if (!(_obj["bonus_type"].ToString().Equals("")))
                        foreach (string bonus in _obj["bonus_type"].ToString().Split(',')) {
                            if (bonus.StartsWith(statName)) {
                                stat.Add("upgrade", int.Parse(bonus.Split(':')[1]));
                            }
                        }
                    stats.Add(getStatKey(statName), stat);
                }
            }
            catch { }
            powerup.Add("mp", _obj["powerup_mp"]);
            powerup.Add("ammo", _obj["powerup_ammo"]);
            powerup.Add("mre", _obj["powerup_mre"]);
            powerup.Add("part", _obj["powerup_part"]);
        }

        public string getStatKey(string _str) {
            switch (_str) {
                case "critical_harm_rate":
                    return "criticalHarmRate";
                case "critical_percent":
                    return "criticalPercent";
                case "armor_piercing":
                    return "armorPiercing";
                case "damage_amplify":
                    return "damageAmplify";
                case "damage_reduction":
                    return "damageReduction";
                case "night_view_percent":
                    return "nightview";
                case "bullet_number_up":
                    return "bullet";
                default:
                    return _str;
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    // 화력소대 데이터
    public class SquadData {
        public int id;
        public string codename;
        public string type;
        public int assistType;
        public JArray assistRange;
        public JObject stats = new JObject() {
            {"hp", 0},
            {"damage", 0},
            {"reload", 0},
            {"hit", 0},
            {"defBreak", 0},
            {"armorPiercing", 0}
        };
        public JObject skill1 = new JObject {
            {"id", 0},
            //{""}
        };
        public JObject skill2;
        public JObject skill3;
    }
}
