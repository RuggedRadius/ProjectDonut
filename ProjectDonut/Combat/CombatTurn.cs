using ProjectDonut.Combat.Combatants;

namespace ProjectDonut.Combat
{
    public class CombatTurn
    {
        public Combatant Attacker { get; set; }
        public Combatant Target { get; set; }

        public CombatTurnAction Action { get; set; }

        public CombatAbility Ability { get; set; }
        public CombatItem Item { get; set; }
        public StrategyAction StrategyAction { get; set; }

        public int DamageDealt { get; set; }
        public int HealingDealt { get; set; }

        public bool TurnComplete { get; set; }

        public CombatTurn(Combatant attacker)
        {
            Attacker = attacker;

            Action = CombatTurnAction.Undecided;
            StrategyAction = StrategyAction.Undecided;
        }

        public bool ReadyToExecute()
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
                    return hasAttacker && hasAttacker && StrategyAction != StrategyAction.Undecided;

                default:
                    return false;
            }
        }
    }
}
