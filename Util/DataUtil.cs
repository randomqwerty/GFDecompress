using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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

    class GunData {
        int id;
        int rank;
        string type;
        int buildTime;
        JArray skins;
        JObject stats = new JObject()
        {
            {"hp", 0},
            {"pow", 0},
            {"hit", 0},
            {"dodge", 0},
            {"speed", 0},
            {"rate", 0},
            {"armorPiercing", 0},
            {"criticalPercent", 20}
        };
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


        }
    }
}
