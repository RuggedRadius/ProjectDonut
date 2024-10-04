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
        public Queue<Combatant> TurnOrder { get; set; } = new Queue<Combatant>();
        public List<Combatant> PlayerTeam { get; set; }
        public List<Combatant> EnemyTeam { get; set; }

        private Random _random = new Random();

        public CombatManager(List<Combatant> playerTeam, List<Combatant> enemyTeam) 
        {
            PlayerTeam = playerTeam;
            EnemyTeam = enemyTeam;

            AllocateCombatantsPositions(PlayerTeam, true);
            AllocateCombatantsPositions(EnemyTeam, false);
        }

        private void PopulateTurnOrder()
        {
            var allCombatants = new List<Combatant>();
            allCombatants.AddRange(PlayerTeam);
            allCombatants.AddRange(EnemyTeam);

            allCombatants = allCombatants.OrderByDescending(x => x.Stats.Speed).ToList();

            foreach (var combatant in allCombatants)
            {
                TurnOrder.Enqueue(combatant);
            }
        }
        
        public void Update(GameTime gameTime)
        {
            TESTINPUTS();

            PlayerTeam.Where(x => x.Stats.Health <= 0).ToList().ForEach(x => PlayerTeam.Remove(x));
            EnemyTeam.Where(x => x.Stats.Health <= 0).ToList().ForEach(x => EnemyTeam.Remove(x));

            foreach (var combatant in PlayerTeam)
            {
                combatant.Update(gameTime);
            }

            foreach (var combatant in EnemyTeam)
            {
                combatant.Update(gameTime);
            }
        }

        public void TESTDoRandomPlayerTeamAttack()
        {
            if (PlayerTeam.Count == 0 || EnemyTeam.Count == 0)
            {
                return;
            }

            var character = PlayerTeam[_random.Next(PlayerTeam.Count)];
            var target = EnemyTeam[_random.Next(EnemyTeam.Count)];

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

        private async void TESTINPUTS()
        {
            if (PlayerTeam.Count == 0 || EnemyTeam.Count == 0)
            {
                return;
            }

            if (InputManager.IsKeyPressed(Keys.D1))
            {
                if (_random.Next(100) >= 50)
                {
                    TESTDoRandomPlayerTeamAttack();
                }
                else
                {
                    TESTDoRandomEnemyTeamAttack();
                }
            }

            if (InputManager.IsKeyPressed(Keys.D2))
            {
                if (_random.Next(100) >= 50)
                {
                    var character = PlayerTeam[_random.Next(PlayerTeam.Count)];
                    var target = EnemyTeam[_random.Next(EnemyTeam.Count)];

                    character.AttackCombatant(target, AttackType.Melee);
                }
                else
                {
                    var character = EnemyTeam[_random.Next(EnemyTeam.Count)];
                    var target = PlayerTeam[_random.Next(PlayerTeam.Count)];

                    character.AttackCombatant(target, AttackType.Melee);
                }
            }

            if (InputManager.IsKeyPressed(Keys.D3))
            {
                if (_random.Next(100) >= 50)
                {
                    var character = PlayerTeam[_random.Next(PlayerTeam.Count)];
                    var target = EnemyTeam[_random.Next(EnemyTeam.Count)];

                    character.AttackCombatant(target, AttackType.Ranged);
                }
                else
                {
                    var character = EnemyTeam[_random.Next(EnemyTeam.Count)];
                    var target = PlayerTeam[_random.Next(PlayerTeam.Count)];

                    character.AttackCombatant(target, AttackType.Ranged);
                }
            }

            if (InputManager.IsKeyPressed(Keys.D4))
            {
                if (_random.Next(100) >= 50)
                {
                    var character = PlayerTeam[_random.Next(PlayerTeam.Count)];
                    var target = EnemyTeam[_random.Next(EnemyTeam.Count)];

                    character.AttackCombatant(target, AttackType.Magic);
                }
                else
                {
                    var character = EnemyTeam[_random.Next(EnemyTeam.Count)];
                    var target = PlayerTeam[_random.Next(PlayerTeam.Count)];

                    character.AttackCombatant(target, AttackType.Magic);
                }
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
