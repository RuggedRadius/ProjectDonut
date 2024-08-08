using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectGorilla.GameObjects;
using System.Collections.Generic;
using System.Linq;
using ProjectDonut.ProceduralGeneration.World;
using ProjectDonut.GameObjects;
using System;
using ProjectDonut.ProceduralGeneration;

namespace ProjectDonut
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Dictionary<string, GameObject> _gameObjects;

        private SpriteFont _font;

        private WorldMapSettings worldMapSettings;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
        }

        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _gameObjects = new Dictionary<string, GameObject>();

            // Camera
            _gameObjects.Add("camera", new Camera());

            // Player
            _gameObjects.Add("player", new Player(
                _graphics,
                GraphicsDevice, 
                Content, 
                _spriteBatch, 
                (Camera)_gameObjects["camera"]
                ));

            // World map
            worldMapSettings = CreateWorldMapSettings();
            _gameObjects.Add("worldmap", new WorldMap(
                worldMapSettings.Width,
                worldMapSettings.Height, 
                new List<object>()
                { 
                    Content, 
                    _graphics, 
                    _spriteBatch, 
                    _gameObjects["camera"],
                    GraphicsDevice 
                },
                worldMapSettings
                ));

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());

            // Position player in middle of the map
            var playerStartPosX = (worldMapSettings.Width * worldMapSettings.TileSize) / 2;
            var playerStartPosY = (worldMapSettings.Height * worldMapSettings.TileSize) / 2;
            _gameObjects["player"].position = new Vector2(playerStartPosX, playerStartPosY);

            base.Initialize();
        }

        private WorldMapSettings CreateWorldMapSettings()
        {
            var s = new WorldMapSettings();

            // Dimensions
            s.Width = 1000;
            s.Height = 1000;
            s.TileSize = 32;

            // Heights
            s.WaterHeightMin = 0;
            s.WaterHeightMax = 2;
            s.GroundHeightMin = 2;
            s.GroundHeightMax = 8;
            s.MountainHeightMin = 8;
            s.MountainHeightMax = 10;

            // Forest
            s.ForestCount = 250;
            s.MinWalk = 250;
            s.MaxWalk = 1000;
            s.WalkRadius = 5;

            // Rivers
            s.RiverForkChance = 0.0025f;
            s.MinForkLength = 5;

            return s;
        }

        protected override void LoadContent()
        {
            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent());

            _font = Content.Load<SpriteFont>("Fonts/Default");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ((Camera)_gameObjects["camera"]).Position = _gameObjects["player"].position;

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(transformMatrix: ((Camera)_gameObjects["camera"]).GetTransformationMatrix(GraphicsDevice, GraphicsDevice.Viewport));

            _gameObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
