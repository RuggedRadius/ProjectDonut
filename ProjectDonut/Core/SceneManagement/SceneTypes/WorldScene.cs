using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Debugging;
using ProjectDonut.GameObjects;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World;
using ProjectDonut.Tools;
using ProjectDonut.UI.ScrollDisplay;
using IGameComponent = ProjectDonut.Interfaces.IGameComponent;

namespace ProjectDonut.Core.SceneManagement.SceneTypes
{
    public class WorldScene : BaseScene
    {
        public static WorldScene Instance;

        private WorldChunkManager worldChunks;

        public WorldMapSettings worldMapSettings;
        private FogOfWar _fog;
        private Random random = new Random();

        public Rectangle LastExitLocation;
        public Vector2 LastExitChunkCoords;


        public WorldScene(SceneType sceneType)
        {
            if (Instance == null)
            {
                Instance = this;
            }

            SceneType = sceneType;
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

            Global.PlayerObj.WorldPosition = new Vector2(LastExitLocation.X, LastExitLocation.Y);
        }

        public override void PrepareForPlayerExit()
        {
            base.PrepareForPlayerExit();
        }
    }
}
