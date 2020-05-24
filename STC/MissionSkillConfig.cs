using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFDecompress.STC
{
    public class MissionSkillConfig
    {
        public int id;
        public string name;
        public string code;
        public string description;
        public string lvup_description;
        public int skill_group_id;
        public int level;
        public int train_coin_type;
        public int train_coin_number;
        public int skill_up_time;
        public string spot_type;
        public string spot_belong;
        public string spot_echelon;
        public int cd_time;
        public int __1;
        public int consumption;
        public string start_range;
        public int is_night;
        public string data_pool;
        public int is_airborne;
        public int airborne_mission_buff_config_id;
        public string effect_cast;
        public string effect_self;
        public string effect_target;
        public string __2;
        public string __3;
        public string special_spot_config_id;
        public string __4;
        public string __5;
        public string __6;
        public string __7;
        public int __8;
        public int __9;
        public int __10;

        public MissionSkillConfig(StcBinaryReader reader)
        {
            id = reader.ReadInt();
            name = reader.ReadString();
            code = reader.ReadString();
            description = reader.ReadString();
            lvup_description = reader.ReadString();
            skill_group_id = reader.ReadInt();
            level = reader.ReadInt();
            train_coin_type = reader.ReadInt();
            train_coin_number = reader.ReadInt();
            skill_up_time = reader.ReadInt();
            spot_type = reader.ReadString();
            spot_belong = reader.ReadString();
            spot_echelon = reader.ReadString();
            cd_time = reader.ReadInt();
            __1 = reader.ReadInt();
            consumption = reader.ReadInt();
            start_range = reader.ReadString();
            is_night = reader.ReadInt();
            data_pool = reader.ReadString();
            is_airborne = reader.ReadInt();
            airborne_mission_buff_config_id = reader.ReadInt();
            effect_cast = reader.ReadString();
            effect_self = reader.ReadString();
            effect_target = reader.ReadString();
            __2 = reader.ReadString();
            __3 = reader.ReadString();
            special_spot_config_id = reader.ReadString();
            __4 = reader.ReadString();
            __5 = reader.ReadString();
            __6 = reader.ReadString();
            __7 = reader.ReadString();
            __8 = reader.ReadInt();
            __9 = reader.ReadInt();
            __10 = reader.ReadInt();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
