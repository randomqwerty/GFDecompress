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
        public int cpu_rate;
        public int crit_rate;
        public int crit_damage;
        public int armor_piercing;
        public int dodge;
        public int move;
        public int assist_armor_piercing;
        public int unknown1;
        public string battle_assist_range;
        public int display_assist_damage_area;
        public int display_assist_area_coef;
        public int skill1;
        public int skill2;
        public int skill3;
        public int performance_skill;
        public int unknown3;
        public int unknown4;
        public string passive_skill;
        public string unknown2;
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
        public int unknown5;
        public string mission_skill_repair;
        public int develop_duration;
        public int basic_rate;
        public string dorm_ai;

        // constructor
        public Squad(StcBinaryReader reader)
        {
            log.Trace("id");
            id = reader.ReadInt();
            name = reader.ReadString();
            en_name = reader.ReadString();
            log.Trace("code");
            code = reader.ReadString();
            introduce = reader.ReadString();
            dialogue = reader.ReadString();
            extra = reader.ReadString();
            en_introduce = reader.ReadString();
            log.Trace("type");
            type = reader.ReadInt();
            assist_type = reader.ReadInt();
            population = reader.ReadInt();
            log.Trace("cpu_id");
            cpu_id = reader.ReadInt();
            hp = reader.ReadInt();
            log.Trace("assist_damage");
            assist_damage = reader.ReadInt();
            assist_reload = reader.ReadInt();
            assist_hit = reader.ReadInt();
            assist_def_break = reader.ReadInt();
            log.Trace("damage");
            damage = reader.ReadInt();
            atk_speed = reader.ReadInt();
            hit = reader.ReadInt();
            cpu_rate = reader.ReadInt();
            log.Trace("crit_rate");
            crit_rate = reader.ReadInt();
            crit_damage = reader.ReadInt();
            log.Trace("armor_piercing");
            armor_piercing = reader.ReadInt();
            dodge = reader.ReadInt();
            move = reader.ReadInt();
            log.Trace("assist_armor_piercing");
            assist_armor_piercing = reader.ReadInt();
            log.Trace("unknown1");
            unknown1 = reader.ReadInt();                        // 추가된 컬럼
            battle_assist_range = reader.ReadString();
            display_assist_damage_area = reader.ReadInt();
            display_assist_area_coef = reader.ReadInt();
            log.Trace("skill1");
            skill1 = reader.ReadInt();
            skill2 = reader.ReadInt();
            skill3 = reader.ReadInt();
            performance_skill = reader.ReadInt();
            unknown3 = reader.ReadInt();                        // 추가된 컬럼
            unknown4 = reader.ReadInt();                        // 추가된 컬럼
            passive_skill = reader.ReadString();
            unknown2 = reader.ReadString();                     // 추가된 컬럼
            normal_attack = reader.ReadInt();
            advanced_bonus = reader.ReadInt();
            deploy_round = reader.ReadInt();
            assist_attack_round = reader.ReadInt();
            attack_round = reader.ReadInt();
            log.Trace("baseammo");
            baseammo = reader.ReadInt();
            basemre = reader.ReadInt();
            ammo_part = reader.ReadInt();
            mre_part = reader.ReadInt();
            is_additional = reader.ReadByte();
            log.Trace("launch_time");
            launch_time = reader.ReadString();
            obtain_ids = reader.ReadString();
            piece_item_id = reader.ReadInt();
            destroy_coef = reader.ReadInt();
            unknown5 = reader.ReadInt();                        // 추가된 컬럼
            mission_skill_repair = reader.ReadString();
            log.Trace("develop_duration");
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
