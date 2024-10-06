namespace ProjectDonut.Combat
{
    public class CombatItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }

        // Figure out an elegant way to do this...
        public int DamageMin { get; set; }
        public int DamageMax { get; set; }
        public int HealthAmount { get; set; }
        public int ManaAmount { get; set; }
    }
}