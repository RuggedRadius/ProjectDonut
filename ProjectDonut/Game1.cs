using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.GameObjects;
using System.Collections.Generic;
using System.Linq;
using ProjectDonut.ProceduralGeneration.World;
using ProjectDonut.GameObjects;
using System;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.UI;
using System.Threading.Tasks;
using System.Threading;

namespace ProjectDonut
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Dictionary<string, GameObject> _gameObjects;

        private SpriteFont _font;

        private WorldMapSettings worldMapSettings;
        private FogOfWar fog;

        private SpriteLibrary spriteLib;

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

            worldMapSettings = CreateWorldMapSettings();

            spriteLib = new SpriteLibrary(Content, GraphicsDevice);

            // Fog of ware
            fog = new FogOfWar(worldMapSettings.Width, worldMapSettings.Height);

            // Camera
            _gameObjects.Add("camera", new Camera());

            // Player
            _gameObjects.Add("player", new Player(
                _graphics,
                GraphicsDevice, 
                Content, 
                _spriteBatch, 
                (Camera)_gameObjects["camera"],
                fog
                ));

            // World map
            _gameObjects.Add("worldmap", new WorldMap(
                worldMapSettings.Width,
                worldMapSettings.Height, 
                new List<object>()
                { 
                    Content, 
                    _graphics, 
                    _spriteBatch, 
                    _gameObjects["camera"],
                    _gameObjects["player"],
                    fog,
                    GraphicsDevice,
                    spriteLib
                },
                worldMapSettings
                ));

            _gameObjects.Add("dialogue", new DialogueSystem(spriteLib, _spriteBatch, (Camera)_gameObjects["camera"], Content));

            

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());

            Task.Run(() =>
            {
                var dialogue = (DialogueSystem)_gameObjects["dialogue"];
                var lines = new Dictionary<string, int>()
                {
                    { "Hello, welcome to Flandaria! A place of nonsense and whimsical adventure!", 3000 },
                    { "I hope you enjoy your stay!", 3000 },
                    { "Goodbye!", 3000 }
                };

                var width = 22;
                var height = 3;
                var startX = -(width * 32) / 2;
                var startY = -(height * 32) / 2;
                var rect = new Rectangle(startX, startY, width, height);

                foreach (var line in lines)
                {
                    dialogue.CreateDialogue(rect, line.Key);
                    Thread.Sleep(line.Value);
                    dialogue.CloseAllDialogues();
                }
            });


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
            s.Width = 100;
            s.Height = 100;
            s.TileSize = 32;

            // Heights
            s.DeepWaterHeightMin = 0;
            s.DeepWaterHeightMax = 1;
            s.WaterHeightMin = 2;
            s.WaterHeightMax = 3;
            s.GroundHeightMin = 4;
            s.GroundHeightMax = 7;
            s.MountainHeightMin = 8;
            s.MountainHeightMax = 10;

            // Forest
            s.ForestCount = 250;
            s.MinWalk = 250;
            s.MaxWalk = 1000;
            s.WalkRadius = 5;

            // Rivers
            //s.RiverCount = 50;
            s.MinLength = 50;
            s.MaxLength = 500;
            s.RiverForkChance = 0.0025f;
            s.MinForkLength = 5;
            s.MinRiverRadius = 1;
            s.MaxRiverRadius = 3;
            s.RiverRadiusDegradationChance = 0.1f;

            // Erosion
            s.CoastErosionMin = 2;
            s.CoastErosionMax = 10;
            s.BiomeErosionMin = 100;
            s.BiomeErosionMax = 500;
            s.DeepWaterErosionMin = 30;
            s.DeepWaterErosionMax = 80;
            s.DeepWaterErosionWidthMin = 10;
            s.DeepWaterErosionWidthMax = 20;
            
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
