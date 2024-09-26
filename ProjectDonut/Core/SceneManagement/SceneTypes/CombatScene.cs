using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Combat;
using ProjectDonut.Core.Input;
using ProjectDonut.Interfaces;

namespace ProjectDonut.Core.SceneManagement.SceneTypes
{
    public class CombatScene : IScene
    {
        public SceneType SceneType { get; set; }
        public Vector2 Position { get; set; }

        public List<Combatant> PlayerTeam { get; set; }
        public List<Combatant> EnemyTeam { get; set; }

        public CombatScene(List<Combatant> playerTeam, List<Combatant> enemyTeam)
        {
            SceneType = SceneType.Combat;
            EnemyTeam = enemyTeam;
            PlayerTeam = playerTeam;

            AllocateCombatantsPositions(PlayerTeam, true);
            AllocateCombatantsPositions(EnemyTeam, false);
        }

        public void Initialize()
        {

        }

        public void LoadContent()
        {

        }

        public void Update(GameTime gameTime)
        {
            foreach (var combatant in PlayerTeam)
            {
                combatant.Update(gameTime);
            }

            foreach (var combatant in EnemyTeam)
            {
                combatant.Update(gameTime);
            }

            if (InputManager.IsKeyPressed(Keys.G))
            {
                foreach (var combatant in PlayerTeam)
                {
                    combatant.Attack(AttackType.Melee);
                }                
            }

            if (InputManager.IsKeyPressed(Keys.H))
            {
                foreach (var combatant in EnemyTeam)
                {
                    combatant.Attack(AttackType.Melee);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Begin();

            // Draw player team
            foreach (var combatant in PlayerTeam)
            {
                combatant.Draw(gameTime);
            }

            // Draw enemy team
            foreach (var combatant in EnemyTeam)
            {
                combatant.Draw(gameTime);
            }

            // Draw combat UI

            Global.SpriteBatch.End();
        }

        public void PrepareForPlayerEntry()
        {
            
        }

        public void PrepareForPlayerExit()
        {
            
        }

        private void AllocateCombatantsPositions(List<Combatant> combatants, bool isPlayerTeam)
        {

        }
    }
}
