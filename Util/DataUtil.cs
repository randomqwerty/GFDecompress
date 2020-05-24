using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
    class GunData {
        int id;
        int rank;
        string type;
        int buildTime;
        JArray skins;
        JObject stats;
        JObject effect = new JObject()
        {
            {"effectType", "all"},
            {"effectCenter",5},
            {"effectPos", new JArray()},
            {"gridEffect", new JObject()}
        };
        int grow;
        string codename;
        JObject skill1 = new JObject()
        {
            {"id",0},
            {"codename", ""},
            {"cooldownType",""},
            {"initialCooldown",0},
            {"dataPool", new JArray()},
            {"consumption", 0},
        };
        JArray obtain;
        JArray equip1;
        JArray equip2;
        JArray equip3;
        JArray mindupdata = new JArray() {
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

        //생성자
        public GunData(JObject _obj) {
            //id
            id = _obj["id"].ToObject<int>();
            //레어도
            rank = _obj["rank_display"].ToObject<int>();
            //병종
            type = TypeData.getDollType(_obj["type"].ToObject<int>());
            //제조시간
            buildTime = _obj["develop_duration"].ToObject<int>();
            //<todo> 스킨정보 jArray

            //스탯정보
            stats.Add("hp", _obj["ratio_life"]); //화력
            stats.Add("pow", _obj["ratio_pow"]); //사속
            stats.Add("hit", _obj["ratio_hit"]); //명중
            stats.Add("dodge", _obj["ratio_dodge"]); //회피
            stats.Add("speed", _obj["ratio_speed"]); //이동속도
            stats.Add("rate", _obj["ratio_rate"]); //사속
            stats.Add("armorPiercing", _obj["armor_piercing"]); //장갑관통
            stats.Add("criticalPercent", _obj["crit"]); //치명률
            if (_obj["ratio_armor"].ToObject<int>() != 0)
                stats.Add("armor", _obj["ratio_armor"]); //장갑

            //진형버프
            effect["effectType"] = TypeData.getDollType(_obj["effect_guntype"].ToObject<int>());
            effect["effectCenter"] = Grid.readPos(_obj["effect_grid_center"].ToObject<int>());

            string[] effectPos = _obj["gridPos"].ToString().Split(',');
            foreach (string str in effectPos) {
                effect["effectPos"].ToObject<JArray>().Add(int.Parse(str)); //진형버프 위치
            }

            string[] gridEffect = _obj["effect_grid_effect"].ToString().Split(';');
            foreach (string str in gridEffect) {
                string[] statPair = str.Split(',');
                effect["gridEffect"].ToObject<JObject>().Add(Grid.readEffectType(int.Parse(statPair[0])),int.Parse(statPair[1])); //진형버프 스탯 및 수치
            }

            //성장계수
            grow = _obj["eat_ratio"].ToObject<int>();
            //코드명
            codename = _obj["code"].ToString();
            //스킬1
            skill1["id"] = _obj["skill1"].ToObject<int>();
            //<todo> 스킬 데이터 추가

            //획득경로
            string[] _obtain = _obj["obtain_ids"].ToString().Split(',');
            foreach (string str in _obtain) {
                obtain.Add(int.Parse(str));
            }

            //장비슬롯1
            string[] _equipt1 = _obj["type_equip1"].ToString().Split(';')[1].Split(',');
            foreach (string str in _equipt1) {
                equip1.Add(TypeData.getEquipType(int.Parse(str)));
            }
            //장비슬롯2
            string[] _equipt2 = _obj["type_equip2"].ToString().Split(';')[1].Split(',');
            foreach (string str in _equipt1)
            {
                equip2.Add(TypeData.getEquipType(int.Parse(str)));
            }
            //장비슬롯3
            string[] _equipt3 = _obj["type_equip3"].ToString().Split(';')[1].Split(',');
            foreach (string str in _equipt1)
            {
                equip3.Add(TypeData.getEquipType(int.Parse(str)));
            }

            //<todo> 개조 소모재료 데이터 추가
        }
    }
}
