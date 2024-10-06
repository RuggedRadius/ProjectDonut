namespace ProjectDonut.Combat
{
    public class CombatAbility
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int ManaCost { get; set; }
        public int EnergyCost { get; set; }
        public int DamageMin { get; set; }
        public int DamageMax { get; set; }
        public DamageType DamageType { get; set; }
    }
}
