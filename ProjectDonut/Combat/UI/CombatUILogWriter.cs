using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            switch (turn.Action)
            {
                case CombatTurnAction.PhysicalAttack:
                    break;

                case CombatTurnAction.MagicAttack:
                    break;

                case CombatTurnAction.UseItem:
                    break;

                case CombatTurnAction.UseCombatAction:
                    break;
            }
        }
    }
}
