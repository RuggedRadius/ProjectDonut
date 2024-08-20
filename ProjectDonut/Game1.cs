using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.GameObjects;
using System.Collections.Generic;
using System.Linq;
using ProjectDonut.ProceduralGeneration.World;
using ProjectDonut.ProceduralGeneration;
using System.Threading.Tasks;
using ProjectDonut.Interfaces;
using ProjectDonut.UI.DialogueSystem;
using ProjectDonut.UI.ScrollDisplay;
using ProjectDonut.Debugging;

namespace ProjectDonut
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Dictionary<string, IGameObject> _gameObjects;
        private Dictionary<string, IScreenObject> _screenObjects;

        private SpriteFont _font;

        private WorldMapSettings worldMapSettings;
        private FogOfWar fog;

        private SpriteLibrary spriteLib;

        private Camera _camera;
        private Player player;
        private DialogueManager dialogue;
        private GameObjects.MouseCursor cursor;

        private WorldChunkManager worldChunks;
        private const int ChunkSize = 100;

        public static Debugger debugger;

        private ScrollDisplayer testScroll;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferredBackBufferHeight = 1440;
        }

        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _gameObjects = new Dictionary<string, IGameObject>();
            _screenObjects = new Dictionary<string, IScreenObject>();

            worldMapSettings = CreateWorldMapSettings();
            //worldChunks = new WorldChunk[3,3];

            spriteLib = new SpriteLibrary(Content, GraphicsDevice);

            // Fog of ware
            fog = new FogOfWar(worldMapSettings.Width, worldMapSettings.Height);

            // Camera
            _camera = new Camera();
            _gameObjects.Add("camera", _camera);

            // Player
            player = new Player(_graphics, GraphicsDevice, Content, _spriteBatch, _camera, fog, worldMapSettings);
            _gameObjects.Add("player", player);

            // World map
            worldChunks = new WorldChunkManager(
                new List<object>()
                {
                    Content,
                    _graphics,
                    _spriteBatch,
                    _camera,
                    player,
                    fog,
                    GraphicsDevice,
                    spriteLib
                },
                worldMapSettings
                );
            _gameObjects.Add("chunkmanager", worldChunks);

            dialogue = new DialogueManager(spriteLib, _spriteBatch, _camera, Content);
            _screenObjects.Add("dialogue", dialogue);

            cursor = new GameObjects.MouseCursor(this, spriteLib, _spriteBatch, GraphicsDevice, _camera);
            //_screenObjects.Add("cursor", cursor);

            debugger = new Debugger(_spriteBatch, Content, GraphicsDevice, _camera);
            _screenObjects.Add("debugger", debugger);

            testScroll = new ScrollDisplayer(Content, _spriteBatch, GraphicsDevice);
            _screenObjects.Add("testScroll", testScroll);


            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());

            Task.Run(() =>
            {
                var test = dialogue.CreateTestDialogue();
                dialogue.ExecuteMultipleLines(test);
            });


            // Position player in middle of the map
            //player.PositionPlayerInMiddleOfMap(worldMapSettings);



            base.Initialize();
        }

        private WorldMapSettings CreateWorldMapSettings()
        {
            var s = new WorldMapSettings();

            // Dimensions
            s.Width = ChunkSize;
            s.Height = ChunkSize;
            s.TileSize = 32;

            // Heights
            s.DeepWaterHeightMin = 0;
            s.DeepWaterHeightMax = 19;
            s.WaterHeightMin = 20;
            s.WaterHeightMax = 29;
            s.GroundHeightMin = 30;
            s.GroundHeightMax = 94;
            s.MountainHeightMin = 95;
            s.MountainHeightMax = 100;

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
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent());

            _font = Content.Load<SpriteFont>("Fonts/Default");
            //debugger.LoadContent();
            cursor.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var kbState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || kbState.IsKeyDown(Keys.Escape))
                Exit();

            if(kbState.IsKeyDown(Keys.O))
            {
                testScroll.DisplayScroll(500, 300, "Flandaria");
            }

            if (kbState.IsKeyDown(Keys.P))
            {
                testScroll.HideScroll();
            }

            ((Camera)_gameObjects["camera"]).Position = _gameObjects["player"].Position;

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));

            //debugger.Update(gameTime);
            cursor.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(transformMatrix: _camera.GetTransformationMatrix(GraphicsDevice, GraphicsDevice.Viewport));

            _gameObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));

            cursor.Draw(gameTime);

            _spriteBatch.End();

            _screenObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));


            base.Draw(gameTime);
        }
    }
}
