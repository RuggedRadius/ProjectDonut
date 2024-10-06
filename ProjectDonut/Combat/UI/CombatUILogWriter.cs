using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ProjectDonut.Combat.Combatants;
using ProjectDonut.Core.SceneManagement.SceneTypes;

namespace ProjectDonut.Combat.UI
{
    public class CombatUILogWriter
    {
        private CombatUILog _log;
        public CombatUILogWriter()
        {
            _log = CombatScene.Instance.LogUI;
        }

        public void WriteLog(CombatTurn turn)
        {
            var log = "";

            switch (turn.Action)
            {
                case CombatTurnAction.PhysicalAttack:
                    log = WritePhysicalAttackLog(turn);
                    break;

                case CombatTurnAction.MagicAttack:
                    log = WriteAbilityAttackLog(turn);
                    break;

                case CombatTurnAction.UseItem:
                    log = WriteUseItemLog(turn);
                    break;

                case CombatTurnAction.StrategyAction:
                    log = WriteStrategicActionLog(turn);
                    break;
            }

            CombatScene.Instance.LogUI.AddLogEntry(log);

            if (turn.Target != null && turn.Target.IsKOd)
            {
                if (turn.Target.Team == TeamType.Player)
                {
                    CombatScene.Instance.LogUI.AddLogEntry($"[#green]{turn.Target.Details.Name}[/] has been [#gray]KO'd[/]");
                }
                else
                {
                    CombatScene.Instance.LogUI.AddLogEntry($"[#red]{turn.Target.Details.Name}[/] has been [#gray]KO'd[/]");
                }
            }
        }

        private string WriteStrategicActionLog(CombatTurn turn)
        {                    
            var log = "";

            // Attack text
            if (turn.Attacker.Team == TeamType.Player)
            {
                log += $"[#green]{turn.Attacker.Details.Name}[/]";
            }
            else
            {
                log += $"[#red]{turn.Attacker.Details.Name}[/]";
            }

            log += $" used [#yellow]{GetEnumDescription(turn.StrategyAction)}[/]";

            if (turn.Target != null)
            {
                log += " on ";

                if (turn.Target.Team == TeamType.Player)
                {
                    log += $"[#green]{turn.Target.Details.Name}[/].";
                }
                else
                {
                    log += $"[#red]{turn.Target.Details.Name}[/].";
                }
            }

            return log;
        }

        private string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        private string WriteUseItemLog(CombatTurn turn)
        {
            var log = "";

            // Attack text
            if (turn.Attacker.Team == TeamType.Player)
            {
                log += $"[#green]{turn.Attacker.Details.Name}[/]";
            }
            else
            {
                log += $"[#red]{turn.Attacker.Details.Name}[/]";
            }

            log += " used ";

            log += $"[#yellow]{turn.Item.Name}[/] on ";

            if (turn.Target.Team == TeamType.Player)
            {
                log += $"[#green]{turn.Target.Details.Name}[/]";
            }
            else
            {
                log += $"[#red]{turn.Target.Details.Name}[/]";
            }

            log += $".";
            //log += $" for [#cyan]{turn.DamageDealt}[/] damage.";

            return log;
        }

        private string WritePhysicalAttackLog(CombatTurn turn)
        {
            var log = "";

            // Attack text
            if (turn.Attacker.Team == TeamType.Player)
            {
                log += $"[#green]{turn.Attacker.Details.Name}[/]";
            }
            else
            {
                log += $"[#red]{turn.Attacker.Details.Name}[/]";
            }

            log += " melee attacked ";

            if (turn.Target.Team == TeamType.Player)
            {
                log += $"[#green]{turn.Target.Details.Name}[/]";
            }
            else
            {
                log += $"[#red]{turn.Target.Details.Name}[/]";
            }

            log += $" for [#cyan]{turn.DamageDealt}[/] damage.";

            return log;
        }

        private string WriteAbilityAttackLog(CombatTurn turn)
        {
            var log = "";

            // Attack text
            if (turn.Attacker.Team == TeamType.Player)
            {
                log += $"[#green]{turn.Attacker.Details.Name}[/]";
            }
            else
            {
                log += $"[#red]{turn.Attacker.Details.Name}[/]";
            }

            log += $" used [#yellow]{turn.Ability?.Name}[/] on ";

            if (turn.Target.Team == TeamType.Player)
            {
                log += $"[#green]{turn.Target.Details.Name}[/]";
            }
            else
            {
                log += $"[#red]{turn.Target.Details.Name}[/]";
            }

            log += $" for [#cyan]{turn.DamageDealt}[/] damage.";

            return log;
        }
    }
}
