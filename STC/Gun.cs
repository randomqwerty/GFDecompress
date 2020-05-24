using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFDecompress.STC
{
    public class Gun
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public int id;
        public string name;
        public string en_name; 
        public string code;
        public string introduce;
        public string dialogue;
        public string extra;
        public string en_introduce;
        public string character;
        public int type;
        public int rank;
        public int develop_duration;
        public int baseammo;
        public int basemre;
        public int ammo_add_withnumber;
        public int mre_add_withnumber;
        public int retiremp;
        public int retireammo;
        public int retiremre;
        public int retirepart;
        public int ratio_life;
        public int ratio_pow;
        public int ratio_rate;
        public int ratio_speed;
        public int ratio_hit;
        public int ratio_dodge;
        public int ratio_armor;
        public int armor_piercing;
        public int crit;
        public int special;
        public int eat_ratio;
        public int ratio_range;
        public int skill1;
        public int skill2;
        public int normal_attack;
        public string passive_skill;
        public string dynamic_passive_skill;
        public int effect_grid_center;
        public string effect_guntype;
        public string effect_grid_pos;
        public string effect_grid_effect;
        public int max_equip;
        public string type_equip1;
        public string type_equip2;
        public string type_equip3;
        public string type_equip4;
        public int ai;
        public byte is_additional;
        public string launch_time;
        public string obtain_ids;
        public int rank_display;
        public int prize_id;
        public string mindupdate_consume;
        public string __1;                 
        public string nation;                   

        public Gun(StcBinaryReader reader)
        {
            id = reader.ReadInt();
            name = reader.ReadString();
            en_name = reader.ReadString();
            code = reader.ReadString();
            introduce = reader.ReadString();
            dialogue = reader.ReadString();
            extra = reader.ReadString();
            en_introduce = reader.ReadString();
            character = reader.ReadString();
            type = reader.ReadInt();
            rank = reader.ReadInt();
            develop_duration = reader.ReadInt();
            baseammo = reader.ReadInt();
            basemre = reader.ReadInt();
            ammo_add_withnumber = reader.ReadInt();
            mre_add_withnumber = reader.ReadInt();
            retiremp = reader.ReadInt();
            retireammo = reader.ReadInt();
            retiremre = reader.ReadInt();
            retirepart = reader.ReadInt();
            ratio_life = reader.ReadInt();
            ratio_pow = reader.ReadInt();
            ratio_rate = reader.ReadInt();
            ratio_speed = reader.ReadInt();
            ratio_hit = reader.ReadInt();
            ratio_dodge = reader.ReadInt();
            ratio_armor = reader.ReadInt();
            armor_piercing = reader.ReadInt();
            crit = reader.ReadInt();
            special = reader.ReadInt();
            eat_ratio = reader.ReadInt();
            ratio_range = reader.ReadInt();
            skill1 = reader.ReadInt();
            skill2 = reader.ReadInt();
            normal_attack = reader.ReadInt();
            passive_skill = reader.ReadString();
            dynamic_passive_skill = reader.ReadString();
            effect_grid_center = reader.ReadInt();
            effect_guntype = reader.ReadString();
            effect_grid_pos = reader.ReadString();
            effect_grid_effect = reader.ReadString();
            max_equip = reader.ReadInt();
            type_equip1 = reader.ReadString();
            type_equip2 = reader.ReadString();
            type_equip3 = reader.ReadString();
            type_equip4 = reader.ReadString();
            ai = reader.ReadInt();
            is_additional = reader.ReadByte();
            launch_time = reader.ReadString();
            obtain_ids = reader.ReadString();
            rank_display = reader.ReadInt();
            prize_id = reader.ReadInt();
            mindupdate_consume = reader.ReadString();
            __1 = reader.ReadString();
            nation = reader.ReadString();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
