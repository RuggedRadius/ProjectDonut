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
    public class CombatManager : Interfaces.IUpdateable
    {
        public static CombatManager Instance { get; private set; }
        public List<Combatant> TurnOrder { get; set; } = new List<Combatant>();
        public List<Combatant> PlayerTeam { get; set; }
        public List<Combatant> EnemyTeam { get; set; }

        public int ExperienceEarnedTotal { get; set; } = 0;

        private Random _random = new Random();

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

            var largestTeamSize = Math.Max(PlayerTeam.Count, EnemyTeam.Count);

            while (TurnOrder.Count < 10)
            {
                for (int i = 0; i < largestTeamSize; i++)
                {
                    if (i < PlayerTeam.Count && PlayerTeam[i] != null)
                        TurnOrder.Add(PlayerTeam[i]);

                    if (i < EnemyTeam.Count && EnemyTeam[i] != null)
                        TurnOrder.Add(EnemyTeam[i]);
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
            PopulateTurnOrder();

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
            PopulateTurnOrder();
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
            var target = possibleEnemies[_random.Next(possibleEnemies.Count)];

            character.AttackCombatant(target, (AttackType)_random.Next(3));
        }

        public void TESTDoRandomEnemyTeamAttack()
        {
            if (PlayerTeam.Count == 0 || EnemyTeam.Count == 0)
            {
                return;
            }

            var character = EnemyTeam[_random.Next(EnemyTeam.Count)];
            var target = PlayerTeam[_random.Next(PlayerTeam.Count)];

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
