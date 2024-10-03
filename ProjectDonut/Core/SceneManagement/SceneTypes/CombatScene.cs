using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using ProjectDonut.Combat;

namespace ProjectDonut.Core.SceneManagement.SceneTypes
{
    public class CombatScene : IScene
    {
        public SceneType SceneType { get; set; }
        public Vector2 Position { get; set; }

        private CombatUI _combatUI;
        private CombatManager _manager;
        private CombatTerrain _terrain;

        private Texture2D _background;

        public AnimatedSprite Background { get; set; }
        private SpriteSheet _spriteSheet;

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
            _manager.Update(gameTime);
            _combatUI.Update(gameTime);
            _terrain.Update(gameTime);

            Background.Update(gameTime);
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
            foreach (var combatant in _manager.PlayerTeam)
            {
                combatant.Draw(gameTime);
                //combatant.DrawBounds();
            }

            // Draw enemy team
            foreach (var combatant in _manager.EnemyTeam)
            {
                combatant.Draw(gameTime);
                //combatant.DrawBounds();
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
