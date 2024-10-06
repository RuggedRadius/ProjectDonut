using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Core;
using ProjectDonut.Core.Input;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using ProjectDonut.Interfaces;

namespace ProjectDonut.Combat
{
    public enum CombatState
    {
        InCombat,
        CombatFinished
    }

    public class CombatManager : Interfaces.IUpdateable
    {
        public static CombatManager Instance { get; private set; }
        public List<Combatant> TurnOrder { get; set; } = new List<Combatant>();
        public List<Combatant> PlayerTeam { get; set; }
        public List<Combatant> EnemyTeam { get; set; }

        public CombatState State { get; set; } = CombatState.InCombat;

        public int ExperienceEarnedTotal { get; set; } = 0;

        private Random _random = new Random();

        private List<Combatant> baseTurnOrder;
        private int baseTurnOrderIndex = 0;

        public CombatTurn CombatTurnCurrent { get; set; }
        public List<CombatTurn> CombatTurnHistory { get; set; } = new List<CombatTurn>();

        public bool IsExecutingTurn { get; set; }

        public CombatManager() 
        {
            if (Instance != null)
            {
                throw new Exception("CombatManager is a singleton and should only be instantiated once.");
            }

            Instance = this;
        }

        public void AddTeam(List<Combatant> team, bool isPlayerTeam)
        {
            if (isPlayerTeam)
            {
                PlayerTeam = team;
                AllocateCombatantsPositions(PlayerTeam, true);
            }
            else
            {
                EnemyTeam = team;
                AllocateCombatantsPositions(EnemyTeam, false);
            }

            TurnOrder.Clear();
            PopulateTurnOrder();
        }

        private void PopulateTurnOrder()
        {
            if (PlayerTeam == null || EnemyTeam == null)
                return;

            baseTurnOrder = new List<Combatant>();

            var orderedPlayerTeam = PlayerTeam.OrderByDescending(x => x.Stats.Speed).ToList();
            var orderedEnemyTeam = EnemyTeam.OrderByDescending(x => x.Stats.Speed).ToList();

            var largestTeamSize = Math.Max(PlayerTeam.Count, EnemyTeam.Count);

            for (int i = 0; i < largestTeamSize; i++)
            {
                if (i < PlayerTeam.Count && PlayerTeam[i] != null)
                    baseTurnOrder.Add(PlayerTeam[i]);

                if (i < EnemyTeam.Count && EnemyTeam[i] != null)
                    baseTurnOrder.Add(EnemyTeam[i]);
            }

            baseTurnOrderIndex = 0;

            TurnOrder = new List<Combatant>(baseTurnOrder);
            CombatTurnCurrent = new CombatTurn(TurnOrder[0]);
        }

        private void ReplenishTurnOrder()
        {
            while (TurnOrder.Count < 10)
            {
                TurnOrder.Add(baseTurnOrder[baseTurnOrderIndex]);

                baseTurnOrderIndex++;

                if (baseTurnOrderIndex >= baseTurnOrder.Count)
                {
                    baseTurnOrderIndex = 0;
                }
            }
        }

        private int TMP_TurnsExecuted = 0;
        public void ExecuteTurn(CombatTurn turn)
        {
            if (IsExecutingTurn)
                return;

            Task.Run(() =>
            {
                if (turn.ReadyToExecute() == false)
                {
                    return;
                }

                IsExecutingTurn = true;

                switch (turn.Action)
                {
                    case CombatTurnAction.PhysicalAttack:
                        turn.Attacker.PhysicalAttack(ref turn);
                        break;

                    case CombatTurnAction.MagicAttack:
                        turn.Attacker.UseAbility(ref turn);
                        break;

                    case CombatTurnAction.UseItem:
                        turn.Attacker.UseItem(ref turn);
                        break;

                    case CombatTurnAction.UseCombatAction:
                        turn.Attacker.UseCombatAction(ref turn);
                        break;
                }

                
                CombatTurnCurrent.TurnComplete = true;
                

                CombatScene.Instance.LogWriter.WriteLog(turn);

                TMP_TurnsExecuted++;
                IsExecutingTurn = false;
            });
        }
                
        public void Update(GameTime gameTime)
        {
            if (CombatTurnCurrent.TurnComplete)
            {
                CombatTurnHistory.Add(CombatTurnCurrent);
                TurnOrder.RemoveAt(0);
                CombatTurnCurrent = new CombatTurn(TurnOrder[0]);
            }

            //if (CombatTurnCurrent == null)
            //{
            //    CombatTurnCurrent = new CombatTurn(TurnOrder[0]);
            //}

            //TESTINPUTS();

            PlayerTeam.Where(x => x.Stats.Health <= 0).ToList().ForEach(x => HandleCombatantDeath(x));
            EnemyTeam.Where(x => x.Stats.Health <= 0).ToList().ForEach(x => HandleCombatantDeath(x));

            foreach (var combatant in PlayerTeam)
            {
                combatant.Update(gameTime);
            }

            foreach (var combatant in EnemyTeam)
            {
                combatant.Update(gameTime);
            }

            // Re-populate turn order
            ReplenishTurnOrder();

            // Handle enemy turn
            var nonIdleCombatants = TurnOrder.Where(x => x.ActionState != CombatantActionState.Idle).ToList();
            if (TurnOrder[0].Team == TeamType.Enemy && nonIdleCombatants.Any() == false)
            {
                var enemy = TurnOrder[0];
                var possibleTargets = PlayerTeam.Where(x => x.IsKOd == false).ToList();

                if (CombatTurnCurrent != null && possibleTargets.Count > 0)
                {
                    CombatTurnCurrent.Target= possibleTargets[_random.Next(possibleTargets.Count)];
                }

                var randomAction = _random.Next(2);
                switch (randomAction)
                {
                    case 0:// Physical attack
                        CombatTurnCurrent.Action = CombatTurnAction.PhysicalAttack;
                        CombatTurnCurrent.Ability = null;
                        break;

                    case 1: // Magical attack
                        CombatTurnCurrent.Action = CombatTurnAction.MagicAttack;
                        CombatTurnCurrent.Ability = enemy.Abilities[_random.Next(enemy.Abilities.Count)];
                        break;
                }
            }

            // Re-populate turn order
            ReplenishTurnOrder();

            if (CombatTurnCurrent != null && CombatTurnCurrent.ReadyToExecute())
            {
                ExecuteTurn(CombatTurnCurrent);
            }

            if (State != CombatState.CombatFinished)
            {
                var activePlayerMembers = PlayerTeam.Where(x => x.ActionState == CombatantActionState.TurnInProgress).ToList().Count;
                var activeEnemyMembers = EnemyTeam.Where(x => x.ActionState == CombatantActionState.TurnInProgress).ToList().Count;

                if (activePlayerMembers == 0 && activeEnemyMembers == 0)
                {
                    if (PlayerTeam.Where(x => x.IsKOd).Count() == PlayerTeam.Count)
                    {
                        CombatScene.Instance.LogUI.AddLogEntry("[#magenta]The battle has been lost, and your journey ended.[/]");
                        State = CombatState.CombatFinished;
                    }
                    else if (EnemyTeam.Where(x => x.IsKOd).Count() == EnemyTeam.Count)
                    {
                        CombatScene.Instance.LogUI.AddLogEntry($"[#yellow]You have gained[/] [#cyan]{EnemyTeam.Sum(x => x.ExperienceGiven)}[/] [#yellow]experience points.[/]");
                        CombatScene.Instance.LogUI.AddLogEntry("[#magenta]The battle has been won, and your journey continues.[/]");
                        State = CombatState.CombatFinished;
                    }
                }
            }
        }

        private void HandleCombatantDeath(Combatant combatant)
        {
            if (PlayerTeam.Contains(combatant))
            {
                //PlayerTeam.Remove(combatant);
            }
            else if (EnemyTeam.Contains(combatant))
            {
                //EnemyTeam.Remove(combatant);
                ExperienceEarnedTotal += combatant.Stats.Experience;
            }

            TurnOrder.RemoveAll(x => x == combatant);
        }

        private void AllocateCombatantsPositions(List<Combatant> combatants, bool isPlayerTeam)
        {
            var tileSize = Global.TileSize * CombatScene.SceneScale;

            if (isPlayerTeam)
            {
                for (int i = 0; i < combatants.Count; i++)
                {
                    var x = 4 * tileSize - (i * 0.5f * tileSize);
                    var y = (2 * tileSize) + (i * (tileSize * 1));
                    combatants[i].ScreenPosition = new Vector2(x, y);
                    combatants[i].BaseScreenPosition = new Vector2(x, y);
                }
            }
            else
            {
                var screenWidth = Global.GraphicsDeviceManager.PreferredBackBufferWidth;
                var curSpriteWidth = CombatScene.SceneScale * Global.TileSize;

                for (int i = 0; i < combatants.Count; i++)
                {
                    var x = screenWidth - (4 * tileSize) - curSpriteWidth + (i * 0.5f * tileSize); 
                    var y = (2 * tileSize) + (i * (tileSize * 1));
                    combatants[i].ScreenPosition = new Vector2(x, y);
                    combatants[i].BaseScreenPosition = new Vector2(x, y);
                }
            }
        }        
    }
}
