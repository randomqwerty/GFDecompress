using Newtonsoft.Json;
using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFDecompress.STC
{
    public class Squad
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
        public int type;
        public int assist_type;
        public int population;
        public int cpu_id;
        public int hp;
        public int assist_damage;
        public int assist_reload;
        public int assist_hit;
        public int assist_def_break;
        public int damage;
        public int atk_speed;
        public int hit;
        public int __1;
        public int cpu_rate;
        public int crit_rate;
        public int crit_damage;
        public int armor_piercing;
        public int dodge;
        public int move;
        public int assist_armor_piercing;
        public string battle_assist_range;
        public int display_assist_damage_area;
        public int display_assist_area_coef;
        public int __2;
        public int __3;
        public int skill1;
        public int skill2;
        public int skill3;
        public int performance_skill;
        public string passive_skill;
        public string __4;
        public int normal_attack;
        public int advanced_bonus;
        public int deploy_round;
        public int assist_attack_round;
        public int attack_round;
        public int baseammo;
        public int basemre;
        public int ammo_part;
        public int mre_part;
        public byte is_additional;
        public string launch_time;
        public string obtain_ids;
        public int piece_item_id;
        public int destroy_coef;
        public int __5;
        public string mission_skill_repair;
        public int develop_duration;
        public int basic_rate;
        public string dorm_ai;

        // constructor
        public Squad(StcBinaryReader reader)
        {
            id = reader.ReadInt();
            name = reader.ReadString();
            en_name = reader.ReadString();
            code = reader.ReadString();
            introduce = reader.ReadString();
            dialogue = reader.ReadString();
            extra = reader.ReadString();
            en_introduce = reader.ReadString();
            type = reader.ReadInt();
            assist_type = reader.ReadInt();
            population = reader.ReadInt();
            cpu_id = reader.ReadInt();
            hp = reader.ReadInt();
            assist_damage = reader.ReadInt();
            assist_reload = reader.ReadInt();
            assist_hit = reader.ReadInt();
            assist_def_break = reader.ReadInt();
            damage = reader.ReadInt();
            atk_speed = reader.ReadInt();
            hit = reader.ReadInt();
            __1 = reader.ReadInt();
            cpu_rate = reader.ReadInt();
            crit_rate = reader.ReadInt();
            crit_damage = reader.ReadInt();
            armor_piercing = reader.ReadInt();
            dodge = reader.ReadInt();
            move = reader.ReadInt();
            assist_armor_piercing = reader.ReadInt();
            battle_assist_range = reader.ReadString();
            display_assist_damage_area = reader.ReadInt();
            display_assist_area_coef = reader.ReadInt();
            __2 = reader.ReadInt();
            __3 = reader.ReadInt();
            skill1 = reader.ReadInt();
            skill2 = reader.ReadInt();
            skill3 = reader.ReadInt();
            performance_skill = reader.ReadInt();
            passive_skill = reader.ReadString();
            __4 = reader.ReadString();
            normal_attack = reader.ReadInt();
            advanced_bonus = reader.ReadInt();
            deploy_round = reader.ReadInt();
            assist_attack_round = reader.ReadInt();
            attack_round = reader.ReadInt();
            baseammo = reader.ReadInt();
            basemre = reader.ReadInt();
            ammo_part = reader.ReadInt();
            mre_part = reader.ReadInt();
            is_additional = reader.ReadByte();
            launch_time = reader.ReadString();
            obtain_ids = reader.ReadString();
            piece_item_id = reader.ReadInt();
            destroy_coef = reader.ReadInt();
            __5 = reader.ReadInt();
            mission_skill_repair = reader.ReadString();
            develop_duration = reader.ReadInt();
            dorm_ai = reader.ReadString();
            basic_rate = reader.ReadInt();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
