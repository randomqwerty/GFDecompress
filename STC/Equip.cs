using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFDecompress.STC
{
    public class Equip
    {
        public int id;
        public string name;
        public string description;
        public int rank;
        public int category;
        public int type;
        public string pow;
        public string hit;
        public string dodge;
        public string speed;
        public string rate;
        public string critical_harm_rate;
        public string critical_percent;
        public string armor_piercing;
        public string armor;
        public string shield;
        public string damage_amplify;
        public string damage_reduction;
        public string night_view_percent;
        public string bullet_number_up;
        public int __1;
        public int __2;
        public int slow_down_percent;
        public int slow_down_rate;
        public int slow_down_time;
        public int dot_percent;
        public int dot_damage;
        public int dot_time;
        public int retire_mp;
        public int retire_ammo;
        public int retire_mre;
        public int retire_part;
        public string code;
        public int develop_duration;
        public string company;
        public int skill_level_up;
        public string fit_guns;
        public string equip_introduction;
        public double powerup_mp;
        public double powerup_ammo;
        public double powerup_mre;
        public double powerup_part;
        public double exclusive_rate;
        public string bonus_type;
        public int skill;
        public int __3;
        public int max_level;

        public Equip(StcBinaryReader reader)
        {
            id = reader.ReadInt();
            name = reader.ReadString();
            description = reader.ReadString();
            rank = reader.ReadInt();
            category = reader.ReadInt();
            type = reader.ReadInt();
            pow = reader.ReadString();
            hit = reader.ReadString();
            dodge = reader.ReadString();
            speed = reader.ReadString();
            rate = reader.ReadString();
            critical_harm_rate = reader.ReadString();
            critical_percent = reader.ReadString();
            armor_piercing = reader.ReadString();
            armor = reader.ReadString();
            shield = reader.ReadString();
            damage_amplify = reader.ReadString();
            damage_reduction = reader.ReadString();
            night_view_percent = reader.ReadString();
            bullet_number_up = reader.ReadString();
            __1 = reader.ReadInt();
            __2 = reader.ReadInt();
            slow_down_percent = reader.ReadInt();
            slow_down_rate = reader.ReadInt();
            slow_down_time = reader.ReadInt();
            dot_percent = reader.ReadInt();
            dot_damage = reader.ReadInt();
            dot_time = reader.ReadInt();
            retire_mp = reader.ReadInt();
            retire_ammo = reader.ReadInt();
            retire_mre = reader.ReadInt();
            retire_part = reader.ReadInt();
            code = reader.ReadString();
            develop_duration = reader.ReadInt();
            company = reader.ReadString();
            skill_level_up = reader.ReadInt();
            fit_guns = reader.ReadString();
            equip_introduction = reader.ReadString();
            powerup_mp = Math.Round(reader.ReadSingle(), 2);
            powerup_ammo = Math.Round(reader.ReadSingle(), 2);
            powerup_mre = Math.Round(reader.ReadSingle(), 2);
            powerup_part = Math.Round(reader.ReadSingle(), 2);
            exclusive_rate = Math.Round(reader.ReadSingle(), 2);
            bonus_type = reader.ReadString();
            skill = reader.ReadInt();
            __3 = reader.ReadInt();
            max_level = reader.ReadInt();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
