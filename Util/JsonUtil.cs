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
                //임시로 id값 필터링했는데 나중에 인형 추가됨에 따라 필터링 변경해야 할 수도 있음
                if (value.ToObject<JObject>()["id"].ToObject<int>() > 2000 && value.ToObject<JObject>()["id"].ToObject<int>() < 20000)
                    continue;
                GunData data = new GunData(value.ToObject<JObject>());
                //Console.WriteLine(data.ToString());
                json.Add(JObject.Parse(data.ToString()));
            }
            Console.WriteLine(json.ToString());
            File.WriteAllText("output_stc\\doll.json",json.ToString());
        }
    }


}
