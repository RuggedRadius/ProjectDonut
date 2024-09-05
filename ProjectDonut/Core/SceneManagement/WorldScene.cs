using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Debugging;
using ProjectDonut.GameObjects;
using ProjectDonut.ProceduralGeneration.World;
using ProjectDonut.Tools;
using ProjectDonut.UI.ScrollDisplay;

namespace ProjectDonut.Core.SceneManagement
{
    public class WorldScene : Scene
    {
        public static WorldScene Instance;

        private WorldChunkManager worldChunks;

        private WorldMapSettings worldMapSettings;
        private FogOfWar _fog;
        private Random random = new Random();

        public Rectangle LastExitLocation;



        public WorldScene(SceneType sceneType)
        {
            if (Instance == null)
            {
                Instance = this;
            }

            base.SceneType = sceneType;
        }

        public override void Initialize()
        {
            base.Initialize();

            worldMapSettings = CreateWorldMapSettings();
            _fog = new FogOfWar(worldMapSettings.Width, worldMapSettings.Height);

            worldChunks = new WorldChunkManager(worldMapSettings);
            _gameObjects.Add("chunkmanager", worldChunks);
            Global.WorldChunkManager = worldChunks;

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var structure = worldChunks.GetCurrentChunk().Structures.FirstOrDefault();
            if (structure != null)
            {
                DebugWindow.Lines[5] = $"Structure: {structure.Bounds}";
            }
            else
            {
                DebugWindow.Lines[5] = "Structure: null";
            }

            DebugWindow.Lines[6] = $"Camera Position: {Global.Camera.Position}";

        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        private WorldMapSettings CreateWorldMapSettings()
        {
            var s = new WorldMapSettings();

            // Dimensions
            s.Width = Global.ChunkSize;
            s.Height = Global.ChunkSize;
            s.TileSize = Global.TileSize;

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

        public override void PrepareForPlayerEntry()
        {
            base.PrepareForPlayerEntry();

            Global.Player.Position = new Vector2(LastExitLocation.X, LastExitLocation.Y);
        }
    }
}
