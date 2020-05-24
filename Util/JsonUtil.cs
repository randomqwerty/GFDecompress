using Newtonsoft.Json.Linq;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFDecompress
{
    class JsonUtil
    {
        public static void getDollJson(JArray _arr) {
            JArray json = new JArray();
            foreach (var value in _arr) {
                if (value.ToObject<JObject>()["id"].ToObject<int>() > 2000 && value.ToObject<JObject>()["id"].ToObject<int>() < 20000)
                    continue;
                GunData data = new GunData(value.ToObject<JObject>());
                Console.WriteLine(data.ToString());
                json.Add(JObject.Parse(data.ToString()));
            }
            Console.WriteLine(json.ToString());
            File.WriteAllText("output_stc\\doll.json",json.ToString());
        }
    }


}
