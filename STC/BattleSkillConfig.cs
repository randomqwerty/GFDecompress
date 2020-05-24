using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFDecompress.STC
{
    public class BattleSkillConfig
    {
        public int id;
        public string name;
        public int skill_group_id;
        public int level;
        public int type;
        public int skill_priority;
        public int cd_type;
        public int cd_time;
        public int start_cd_time;
        public string trigger_id;
        public int trigger_type;
        public int trigger_target;
        public string trigger_parameter;
        public int trigger_buff_id;
        public int target_select_ai;
        public byte is_re_target;
        public int action_id;
        public string skill_duration;
        public byte is_form_action;
        public string skin_action;
        public string __1;
        public string buff_id_target;
        public string buff_id_self;
        public int buff_delay;
        public string description;
        public string lvup_description;
        public string data_pool_1;
        public string data_pool_2;
        public string night_data_pool_1;
        public string night_data_pool_2;
        public string sp_data_pool_1;
        public string sp_data_pool_2;
        public string sppool_trigger_id;
        public int sppool_trigger_type;
        public int sppool_trigger_target;
        public int sppool_trigger_parameter;
        public int sppool_trigger_buff_id;
        public string code;
        public int train_coin_type;
        public int train_coin_number;
        public int target_lost;
        public int daynight_only;
        public int interrupt_type;
        public int interrupt_damage_limit;
        public int creation_number;
        public byte is_switch;
        public string passive_name;
        public int weight;
        public int consumption;
        public byte is_rare;
        public int skill_up_time;
        public int rank;
        public byte is_mindupdate;
        public byte is_manual;
        public int __2;
        public int __3;

        public BattleSkillConfig(StcBinaryReader reader)
        {
            id = reader.ReadInt();
            name = reader.ReadString();
            skill_group_id = reader.ReadInt();
            level = reader.ReadInt();
            type = reader.ReadInt();
            skill_priority = reader.ReadInt();
            cd_type = reader.ReadInt();
            cd_time = reader.ReadInt();
            start_cd_time = reader.ReadInt();
            trigger_id = reader.ReadString();
            trigger_type = reader.ReadInt();
            trigger_target = reader.ReadInt();
            trigger_parameter = reader.ReadString();
            trigger_buff_id = reader.ReadInt();
            target_select_ai = reader.ReadInt();
            is_re_target = reader.ReadByte();
            action_id = reader.ReadInt();
            skill_duration = reader.ReadString();
            is_form_action = reader.ReadByte();
            skin_action = reader.ReadString();
            __1 = reader.ReadString();
            buff_id_target = reader.ReadString();
            buff_id_self = reader.ReadString();
            buff_delay = reader.ReadInt();
            description = reader.ReadString();
            lvup_description = reader.ReadString();
            data_pool_1 = reader.ReadString();
            data_pool_2 = reader.ReadString();
            night_data_pool_1 = reader.ReadString();
            night_data_pool_2 = reader.ReadString();
            sp_data_pool_1 = reader.ReadString();
            sp_data_pool_2 = reader.ReadString();
            sppool_trigger_id = reader.ReadString();
            sppool_trigger_type = reader.ReadInt();
            sppool_trigger_target = reader.ReadInt();
            sppool_trigger_parameter = reader.ReadInt();
            sppool_trigger_buff_id = reader.ReadInt();
            code = reader.ReadString();
            train_coin_type = reader.ReadInt();
            train_coin_number = reader.ReadInt();
            target_lost = reader.ReadInt();
            daynight_only = reader.ReadInt();
            interrupt_type = reader.ReadInt();
            interrupt_damage_limit = reader.ReadInt();
            creation_number = reader.ReadInt();
            is_switch = reader.ReadByte();
            passive_name = reader.ReadString();
            weight = reader.ReadInt();
            consumption = reader.ReadInt();
            is_rare = reader.ReadByte();
            skill_up_time = reader.ReadInt();
            rank = reader.ReadInt();
            is_mindupdate = reader.ReadByte();
            is_manual = reader.ReadByte();
            __2 = reader.ReadInt();
            __3 = reader.ReadInt();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
