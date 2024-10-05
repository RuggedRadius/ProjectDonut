using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Combat
{
    public class CombatantStats
    {
        public int Speed { get; set; }
        public int Strength { get; set; }
        public int Defence { get; set; }
        public int Magic { get; set; }
        public int Resistance { get; set; }
        public int Luck { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Mana { get; set; }
        public int MaxMana { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }

        /*
         * public int Level { get; set; }
        public int Health { get; set; }
        public int Mana { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int Speed { get; set; }
        public int Defense { get; set; }
        public int MagicDefense { get; set; }
        public int Experience { get; set; }
        public int ExperienceToNextLevel { get; set; }
        public int Gold { get; set; }
        public int Damage { get; set; }
        public int MagicDamage { get; set; }
        public int Armor { get; set; }
        public int MagicArmor { get; set; }
        public int Accuracy { get; set; }
        public int Evasion { get; set; }
        public int CriticalChance { get; set; }
        public int CriticalDamage { get; set; }
        public int HealthRegen { get; set; }
        public int ManaRegen { get; set; }
        public int HealthRegenRate { get; set; }
        public int ManaRegenRate { get; set; }
        public int HealthRegenAmount { get; set; }
        public int ManaRegenAmount { get; set; }
        public int HealthRegenRateAmount { get; set; }
        public int ManaRegenRateAmount { get; set; }
        public int HealthRegenAmountRate { get; set; }
        public int ManaRegenAmountRate { get; set; }
        public int HealthRegenRateAmountRate { get; set; }
        public int ManaRegenRateAmountRate { get; set; }
        public int HealthRegenAmountAmount { get; set; }
        public int ManaRegenAmountAmount { get; set; }
        public int HealthRegenRateRate { get; set; }
        public int ManaRegenRateRate { get; set; }
        public int HealthRegenAmountRateRate { get; set; }
        public int ManaRegenAmountRateRate { get; set; }
        public int HealthRegenRateAmountRateRate { get; set; }
        public int ManaRegenRateAmountRateRate { get; set;
         * */

        public CombatantStats()
        {
            // TEMP
            MaxHealth = 100;
            Health = MaxHealth;
        }
    }
}
