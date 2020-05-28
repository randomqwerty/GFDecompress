using Newtonsoft.Json.Linq;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFDecompress
{
    class JsonUtil
    {
        public static void getDollJson(JArray _arr, JArray _skinList, JArray _skillList) {
            JArray json = new JArray();
            foreach (var value in _arr) {
                //임시로 id값 필터링했는데 나중에 인형 추가됨에 따라 필터링 변경해야 할 수도 있음
                if (value.ToObject<JObject>()["id"].ToObject<int>() > 2000 && value.ToObject<JObject>()["id"].ToObject<int>() < 20000)
                    continue;
                if (value.ToObject<JObject>()["id"].ToObject<int>() > 30000)
                    continue;
                GunData data = new GunData(value.ToObject<JObject>(), _skinList, _skillList);
                //Console.WriteLine(data.ToString());
                json.Add(JObject.Parse(data.ToString()));
            }
            //Console.WriteLine(json.ToString());
            File.WriteAllText("results\\doll.json",json.ToString());
        }

        public static void getEquipJson(JArray _arr) {
            JArray json = new JArray();

            foreach (var value in _arr) {
                EquipData data = new EquipData(value.ToObject<JObject>());
                json.Add(JObject.Parse(data.ToString()));
            }
            File.WriteAllText("results\\equip.json", json.ToString());
        }

        public static void getFairyJson(JArray _skillList, JArray _subSkillList) {
            JArray json = new JArray();
            JObject fairyData;
            JObject fairySkinData;
            JArray skills = new JArray() {
                _skillList,
                _subSkillList
            };

            try
            {
                fairyData = JObject.Parse(System.IO.File.ReadAllText("output_catchdata\\fairy_info.json"));
                fairySkinData = JObject.Parse(System.IO.File.ReadAllText("output_catchdata\\fairy_skin_info.json"));
            }
            catch{
                Console.WriteLine("ERROR: 파일이 존재하지 않음");
                return;
            }

            foreach (var value in fairyData["fairy_info"].ToObject<JArray>())
            {
                FairyData data = new FairyData(value.ToObject<JObject>(), skills, fairySkinData);
                json.Add(JObject.Parse(data.ToString()));
            }
            File.WriteAllText("results\\fairy.json", json.ToString());
        }

        //이하 text asset to json

        public static void getTextAsset(string _location) {
            string dir = $"./Assets/{_location}";
            string data;

            if (!File.Exists("results\\text"))
                Directory.CreateDirectory("results\\text");
            
            foreach (var item in new DirectoryInfo(dir).GetFiles()) {
                Console.WriteLine(item.Name + "변환 중");
                JObject json = new JObject();
                StreamReader file = new StreamReader(dir + "\\" + item.Name);
                try
                {
                    while ((data = file.ReadLine()) != null)
                    {
                        string[] str = { "", "" };
                        str = data.Split(',');
                        json.Add(str[0], str[1]);
                    }
                }
                catch { }
                Encoding utf8 = new UTF8Encoding(false);
                File.WriteAllText($"results\\text\\{item.Name.Split('.')[0]}.json", json.ToString(),utf8);
            }
        }
    }


}
