using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFDecompress.STC
{
    public class Skin
    {
        public int id;
        public string name;
        public int extra;
        public int fit_gun;
        public int ai;
        public int voice;
        public int is_hidden;
        public string substitute_voice;
        public string dialog;
        public string note;
        public string __1;
        public string __2;
        public string __3;

        public Skin(StcBinaryReader reader)
        {
            id = reader.ReadInt();
            name = reader.ReadString();
            extra = reader.ReadInt();
            fit_gun = reader.ReadInt();
            ai = reader.ReadInt();
            voice = reader.ReadInt();
            is_hidden = reader.ReadInt();
            substitute_voice = reader.ReadString();
            dialog = reader.ReadString();
            note = reader.ReadString();
            __1 = reader.ReadString();
            __2 = reader.ReadString();
            __3 = reader.ReadString();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
