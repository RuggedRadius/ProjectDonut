using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Debugging;
using ProjectDonut.GameObjects;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.World;
using ProjectDonut.Tools;
using ProjectDonut.UI.ScrollDisplay;

namespace ProjectDonut.Core.SceneManagement
{

    public class Scene : IGameObject
    {
        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphicsDevice;

        private Dictionary<string, IGameObject> _gameObjects;
        private Dictionary<string, IScreenObject> _screenObjects;

        private SpriteLibrary _spriteLib;
        private Camera _camera;
        private ScrollDisplayer _scrollDisplay;
        private WorldChunkManager worldChunks;
        private const int ChunkSize = 100;

        private WorldMapSettings worldMapSettings;
        private FogOfWar _fog;
        private Random random;

        private Player _player;

        public SceneType SceneType { get; private set; }
        public Vector2 Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ZIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Scene(SceneType sceneType, Player player, ContentManager content, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Camera camera, SpriteLibrary spriteLibray)
        {
            SceneType = sceneType;
            random = new Random();
            _player = player;

            _content = content;
            _spriteBatch = spriteBatch;
            _graphicsDevice = graphicsDevice;
            _camera = camera;
            _spriteLib = spriteLibray;
        }

        public void Initialize()
        {
            _gameObjects = new Dictionary<string, IGameObject>();
            _screenObjects = new Dictionary<string, IScreenObject>();

            worldMapSettings = CreateWorldMapSettings();
            _fog = new FogOfWar(worldMapSettings.Width, worldMapSettings.Height, _player);

            _scrollDisplay = new ScrollDisplayer(_content, _spriteBatch, _graphicsDevice);
            _screenObjects.Add("scrollDisplay", _scrollDisplay);

            worldChunks = new WorldChunkManager(
                new List<object>()
                {
                                _content,
                                _spriteBatch,
                                _camera,
                                _player,
                                _graphicsDevice,
                                _spriteLib,
                                _scrollDisplay,
                },
                worldMapSettings
                );
            _gameObjects.Add("chunkmanager", worldChunks);

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());

        }

        public void LoadContent(ContentManager content)
        {
            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent(content));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent());
        }

        public void Update(GameTime gameTime)
        {
            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));


            var kbState = Keyboard.GetState();
            if (kbState.IsKeyDown(Keys.O))
            {
                //testScroll.DisplayScroll(500, 300, "Flandaria");                
                _scrollDisplay.DisplayScroll(new ProceduralGeneration.World.Structures.StructureData()
                {
                    Name = NameGenerator.GenerateRandomName(random.Next(3, 4)),
                    //Name = "Flandaria",
                    Bounds = new Rectangle(800, 100, 100, 100)
                });
            }

            if (kbState.IsKeyDown(Keys.P))
            {
                _scrollDisplay.HideScroll();
            }

            var structure = worldChunks.GetCurrentChunk().Structures.FirstOrDefault();
            if (structure != null)
            {
                Debugger.Lines[5] = $"Structure: {structure.Bounds.X},{structure.Bounds.Y}";
            }
            else
            {
                Debugger.Lines[5] = "Structure: null";
            }

            Debugger.Lines[6] = $"Camera Position: {_camera.Position}";

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _spriteBatch.Begin(transformMatrix: _camera.GetTransformationMatrix());
            _gameObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime, spriteBatch));
            _spriteBatch.End();

            // ScreenObjects
            _screenObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));
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
    }
}
