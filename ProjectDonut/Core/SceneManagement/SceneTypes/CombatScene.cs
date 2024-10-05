using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using ProjectDonut.Combat;
using ProjectDonut.Combat.UI;

namespace ProjectDonut.Core.SceneManagement.SceneTypes
{
    public class CombatScene : IScene
    {
        public SceneType SceneType { get; set; }
        public Vector2 Position { get; set; }

        private CombatUI _combatUI;
        public CombatManager Manager;
        private CombatTerrain _terrain;
        private CombatUIOptions _uiOptions;
        private CombatUITurnOrder _turnOrder;

        private Texture2D _background;

        public AnimatedSprite Background { get; set; }
        private SpriteSheet _spriteSheet;

        public static int SceneScale = 4;

        private IScene PreviousScene { get; set; }


        public CombatScene(IScene previousScene)
        {
            SceneType = SceneType.Combat;

            Manager = new CombatManager();
            _combatUI = new CombatUI(Manager);
            _terrain = new CombatTerrain();
            _turnOrder = new CombatUITurnOrder(Manager);

            _uiOptions = new CombatUIOptions(Manager);
            PreviousScene = previousScene;
        }

        public void Initialize()
        {
            _combatUI.Initialize();

            var sheetTexture = Global.ContentManager.Load<Texture2D>("Sprites/Combat/Backgrounds/bg-dungeon");
            var atlas = Texture2DAtlas.Create("combatant", sheetTexture, 480, 270);
            _spriteSheet = new SpriteSheet("SpriteSheet/combatant", atlas);

            var frameTime = 0.1f;

            _spriteSheet.DefineAnimation("idle", builder =>
            {
                builder.IsLooping(true)
                    .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(frameTime))
                    .AddFrame(regionIndex: 1, duration: TimeSpan.FromSeconds(frameTime))
                    .AddFrame(regionIndex: 2, duration: TimeSpan.FromSeconds(frameTime))
                    .AddFrame(regionIndex: 3, duration: TimeSpan.FromSeconds(frameTime))
                    .AddFrame(regionIndex: 4, duration: TimeSpan.FromSeconds(frameTime))
                    .AddFrame(regionIndex: 5, duration: TimeSpan.FromSeconds(frameTime))
                    .AddFrame(regionIndex: 6, duration: TimeSpan.FromSeconds(frameTime))
                    .AddFrame(regionIndex: 7, duration: TimeSpan.FromSeconds(frameTime));
            });

            Background = new AnimatedSprite(_spriteSheet, "idle");
            Background.SetAnimation("idle");
        }

        public void LoadContent()
        {
            //_background = Global.ContentManager.Load<Texture2D>("Sprites/Combat/Backgrounds/bg-dungeon");
        }

        public void Update(GameTime gameTime)
        {
            Manager.Update(gameTime);
            //_combatUI.Update(gameTime);
            _terrain.Update(gameTime);

            Background.Update(gameTime);

            _uiOptions.Update(gameTime);
            _turnOrder.Update(gameTime);

            if (Manager.PlayerTeam.Count == 0)
            {
                // Game over
                //...

            }
            else if (Manager.EnemyTeam.Count == 0)
            {
                // Victory
                
                // Display victory screen
                // EXP, Items, etc..

                Global.SceneManager.SetCurrentScene(PreviousScene);
            }
        }

        public void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Begin();

            // Draw background
            //Global.SpriteBatch.Draw(_background, new Rectangle(0, 0, Global.GraphicsDeviceManager.PreferredBackBufferWidth, Global.GraphicsDeviceManager.PreferredBackBufferHeight), Color.White);
            Global.SpriteBatch.Draw(Background, Vector2.Zero, 0.0f, Vector2.One * CombatScene.SceneScale);

            _terrain.DrawTerrainLayer(gameTime, "base");
            _terrain.DrawTerrainLayer(gameTime, "obstacle");
            _terrain.DrawTerrainLayer(gameTime, "decorative");
            //_terrain.DrawGrid(gameTime);

            // Draw player team
            foreach (var combatant in Manager.PlayerTeam)
            {
                combatant.Draw(gameTime);
                //combatant.DrawBounds();
            }

            // Draw enemy team
            foreach (var combatant in Manager.EnemyTeam)
            {
                combatant.Draw(gameTime);
                //combatant.DrawBounds();
            }

            // Draw combat UI
            //_combatUI.Draw(gameTime);

            // Draw combat UI options
            _uiOptions.Draw(gameTime);
            _turnOrder.Draw(gameTime);

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
