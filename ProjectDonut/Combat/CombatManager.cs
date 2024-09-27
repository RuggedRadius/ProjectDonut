using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public CombatManager(List<Combatant> playerTeam, List<Combatant> enemyTeam) 
        {
            PlayerTeam = playerTeam;
            EnemyTeam = enemyTeam;

            AllocateCombatantsPositions(PlayerTeam, true);
            AllocateCombatantsPositions(EnemyTeam, false);
        }
        
        public void Update(GameTime gameTime)
        {
            TESTINPUTS();

            foreach (var combatant in PlayerTeam)
            {
                combatant.Update(gameTime);
            }

            foreach (var combatant in EnemyTeam)
            {
                combatant.Update(gameTime);
            }
        }

        private void TESTINPUTS()
        {
            if (InputManager.IsKeyPressed(Keys.D1))
            {
                PlayerTeam[0]?.Attack(AttackType.Melee);
                EnemyTeam[0]?.TakeDamage(10);
            }

            if (InputManager.IsKeyPressed(Keys.D2))
            {
                PlayerTeam[1]?.Attack(AttackType.Melee);
                EnemyTeam[1]?.TakeDamage(10);
            }

            if (InputManager.IsKeyPressed(Keys.D3))
            {
                PlayerTeam[2]?.Attack(AttackType.Melee);
                EnemyTeam[2]?.TakeDamage(10);
            }

            if (InputManager.IsKeyPressed(Keys.D4))
            {
                EnemyTeam[0]?.Attack(AttackType.Melee);
                PlayerTeam[0]?.TakeDamage(10);
            }

            if (InputManager.IsKeyPressed(Keys.D5))
            {
                EnemyTeam[1]?.Attack(AttackType.Melee);
                PlayerTeam[1]?.TakeDamage(10);
            }

            if (InputManager.IsKeyPressed(Keys.D6))
            {
                EnemyTeam[2]?.Attack(AttackType.Melee);
                PlayerTeam[2]?.TakeDamage(10);
            }
        }

        private void AllocateCombatantsPositions(List<Combatant> combatants, bool isPlayerTeam)
        {
            var tileSize = Global.TileSize * CombatScene.SceneScale;

            if (isPlayerTeam)
            {
                for (int i = 0; i < combatants.Count; i++)
                {
                    var x = 2 * tileSize;
                    var y = tileSize + (i * (tileSize * 1));
                    combatants[i].ScreenPosition = new Vector2(x, y);
                }
            }
            else
            {
                var screenWidth = Global.GraphicsDeviceManager.PreferredBackBufferWidth;
                var curSpriteWidth = CombatScene.SceneScale * Global.TileSize;

                for (int i = 0; i < combatants.Count; i++)
                {
                    var x = screenWidth - (2 * tileSize) - curSpriteWidth;
                    var y = tileSize + (i * (tileSize * 1));
                    combatants[i].ScreenPosition = new Vector2(x, y);
                }
            }
        }
    }
}
