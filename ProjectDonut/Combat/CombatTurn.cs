namespace ProjectDonut.Combat
{
    public class CombatTurn
    {
        public Combatant Attacker { get; set; }
        public Combatant Target { get; set; }

        public CombatTurnAction Action { get; set; }

        public CombatAbility Ability { get; set; }
        public CombatItem Item { get; set; }
        public CombatAction CombatAction { get; set; }

        public CombatTurn(Combatant attacker)
        {
            Attacker = attacker;

            Action = CombatTurnAction.Undecided;
            CombatAction = CombatAction.Undecided;
        }

        public bool IsTurnComplete()
        {
            bool hasAttacker = Attacker != null;
            bool hasTarget = Target != null;
            bool hasAbility = Ability != null;
            bool hasItem = Item != null;

            switch (Action)
            {
                case CombatTurnAction.PhysicalAttack:
                    return hasAttacker && hasTarget;

                case CombatTurnAction.MagicAttack:
                    return hasAttacker && hasTarget && hasAbility;

                case CombatTurnAction.UseItem:
                    return hasAttacker && hasTarget && hasItem;

                case CombatTurnAction.UseCombatAction:
                    return hasAttacker && hasAttacker && CombatAction != CombatAction.Undecided;

                default:
                    return false;
            }
        }
    }
}
