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
                if (value["id"].ToString() == value["code"].ToString() || value["id"].ToObject<int>()  == 97 || value["id"].ToObject<int>() == 98)
                    continue;
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
                fairyData = JObject.Parse(System.IO.File.ReadAllText("output\\catchdata\\fairy_info.json"));
                fairySkinData = JObject.Parse(System.IO.File.ReadAllText("output\\catchdata\\fairy_skin_info.json"));
            }
            catch{
                Console.WriteLine("ERROR: File does not exist");
                return;
            }

            foreach (var value in fairyData["fairy_info"].ToObject<JArray>())
            {
                FairyData data = new FairyData(value.ToObject<JObject>(), skills, fairySkinData);
                if (data.category == "dummy")
                    continue;
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
                if (item.Name == "desktop.ini")
                    continue;
                Console.WriteLine("Converting " + item.Name);
                JObject json = new JObject();
                StreamReader file = new StreamReader(dir + "\\" + item.Name);
                try
                {
                    while ((data = file.ReadLine()) != null)
                    {
                        if (data == "\n")
                            continue;

                        string[] str = { "", "" };
                        str = data.Split(',');
                        if (json.ContainsKey(str[0]))
                            continue;
                        json.Add(str[0], str[1].Replace("//c",",").Replace("//n","\n"));
                    }
                }
                catch {
                    Console.WriteLine("error");
                }
                Encoding utf8 = new UTF8Encoding(false);
                if (!File.Exists($"results\\text\\{_location}"))
                    Directory.CreateDirectory($"results\\text\\{_location}");

                File.WriteAllText($"results\\text\\{_location}\\{item.Name.Split('.')[0]}.json", json.ToString(),utf8);
            }
        }

        public static void getDialogueText(string _location) {
            string data;
            JObject json = new JObject();

            StreamReader file = new StreamReader($"Extra\\{_location}\\NewCharacterVoice.txt");
            try {
                while ((data = file.ReadLine()) != null) {
                    // [0] : codename, [1] : DialogueType, [2]: text
                    string[] line = data.Split('|');

                    //if (line[1].StartsWith("DIALOGUE") || line[1] == "GAIN" || line[1] == "Introduce" || line[1] == "SOULCONTRACT")
                    line[1] = line[1].ToLower();

                    if (!json.ContainsKey(line[0])) {
                        json.Add(line[0], new JObject());
                    }

                    if (json[line[0]].ToObject<JObject>().ContainsKey(line[1]))
                        continue;
                    ((JObject)json[line[0]]).Add(line[1], new JArray() {line[2]});
                }  
            } 
            catch (Exception e){
                Console.WriteLine(e);
            }

            if (!File.Exists($"results\\Extra"))
                Directory.CreateDirectory($"results\\Extra");

            if (!File.Exists($"results\\Extra\\{_location}"))
                Directory.CreateDirectory($"results\\Extra\\{_location}");

            File.WriteAllText($"results\\Extra\\{_location}\\NewCharacterVoice.json", json.ToString());
        }
    }
}
