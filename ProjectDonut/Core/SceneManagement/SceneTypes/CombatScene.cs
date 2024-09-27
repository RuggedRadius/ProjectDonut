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



        private CombatUI _combatUI;
        private CombatManager _manager;
        private CombatTerrain _terrain;

        public static int SceneScale = 4;


        public CombatScene(List<Combatant> playerTeam, List<Combatant> enemyTeam)
        {
            SceneType = SceneType.Combat;

            _manager = new CombatManager(playerTeam, enemyTeam);
            _combatUI = new CombatUI(_manager);
            _terrain = new CombatTerrain();
        }

        public void Initialize()
        {
            _combatUI.Initialize();
        }

        public void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            _manager.Update(gameTime);
            _combatUI.Update(gameTime);
            _terrain.Update(gameTime);
        }



        public void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Begin();

            _terrain.DrawTerrainLayer(gameTime, "base");
            _terrain.DrawTerrainLayer(gameTime, "obstacle");
            _terrain.DrawTerrainLayer(gameTime, "decorative");
            _terrain.DrawGrid(gameTime);

            // Draw player team
            foreach (var combatant in _manager.PlayerTeam)
            {
                combatant.Draw(gameTime);
                combatant.DrawBounds();
            }

            // Draw enemy team
            foreach (var combatant in _manager.EnemyTeam)
            {
                combatant.Draw(gameTime);
                combatant.DrawBounds();
            }

            // Draw combat UI
            _combatUI.Draw(gameTime);

            Global.SpriteBatch.End();
        }

        public void PrepareForPlayerEntry()
        {
            
        }

        public void PrepareForPlayerExit()
        {
            
        }
    }
}
