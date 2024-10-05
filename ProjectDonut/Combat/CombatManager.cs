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
        
        public void Update(GameTime gameTime)
        {
            TESTINPUTS();

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

                if (possibleTargets.Count > 0)
                {
                    var target = possibleTargets[_random.Next(possibleTargets.Count)];
                    enemy.AttackCombatant(target, (AttackType)_random.Next(3));
                }
            }

            // Re-populate turn order
            ReplenishTurnOrder();

            if (State != CombatState.CombatFinished)
            {
                var activePlayerMembers = PlayerTeam.Where(x => x.ActionState == CombatantActionState.TurnInProgress).ToList().Count;
                var activeEnemyMembers = EnemyTeam.Where(x => x.ActionState == CombatantActionState.TurnInProgress).ToList().Count;

                if (activePlayerMembers == 0 && activeEnemyMembers == 0)
                {
                    if (PlayerTeam.Where(x => x.IsKOd).Count() == PlayerTeam.Count)
                    {
                        CombatScene.Instance.Log.AddLogEntry("[#magenta]The battle has been lost, and your journey ended.[/]");
                        State = CombatState.CombatFinished;
                    }
                    else if (EnemyTeam.Where(x => x.IsKOd).Count() == EnemyTeam.Count)
                    {
                        CombatScene.Instance.Log.AddLogEntry($"[#yellow]You have gained[/] [#cyan]{EnemyTeam.Sum(x => x.ExperienceGiven)}[/] [#yellow]experience points.[/]");
                        CombatScene.Instance.Log.AddLogEntry("[#magenta]The battle has been won, and your journey continues.[/]");
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

        public void TESTDoRandomPlayerTeamAttack()
        {
            if (PlayerTeam.Count == 0 || EnemyTeam.Count == 0)
            {
                return;
            }

            var character = TurnOrder[0];// PlayerTeam[_random.Next(PlayerTeam.Count)];

            var possibleEnemies = EnemyTeam.Where(x => x.IsKOd == false).ToList();

            if (possibleEnemies.Count == 0)
            {
                return;
            }

            var target = possibleEnemies[_random.Next(possibleEnemies.Count)];
            character.AttackCombatant(target, (AttackType)_random.Next(3));
        }

        private void TESTINPUTS()
        {
            if (PlayerTeam.Count == 0 || EnemyTeam.Count == 0)
            {
                return;
            }

            if (TurnOrder[0].Team == TeamType.Enemy)
            {
                return;
            }

            // TEST MELEE
            if (InputManager.IsKeyPressed(Keys.D1))
            {
                var character = PlayerTeam[_random.Next(PlayerTeam.Count)];
                var target = EnemyTeam[_random.Next(EnemyTeam.Count)];

                character.AttackCombatant(target, AttackType.Melee);
            }

            // TEST RANGED
            if (InputManager.IsKeyPressed(Keys.D2))
            {
                var character = PlayerTeam[_random.Next(PlayerTeam.Count)];
                var target = EnemyTeam[_random.Next(EnemyTeam.Count)];

                character.AttackCombatant(target, AttackType.Ranged);
            }

            // TEST MAGIC
            if (InputManager.IsKeyPressed(Keys.D3))
            {
                var character = PlayerTeam[_random.Next(PlayerTeam.Count)];
                var target = EnemyTeam[_random.Next(EnemyTeam.Count)];

                character.AttackCombatant(target, AttackType.Magic);
            }
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
