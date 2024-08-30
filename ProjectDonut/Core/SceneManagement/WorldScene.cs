﻿using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Debugging;
using ProjectDonut.GameObjects;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.World;
using ProjectDonut.Tools;
using ProjectDonut.UI.ScrollDisplay;

namespace ProjectDonut.Core.SceneManagement
{
    public class WorldScene : Scene
    {
        public static WorldScene Instance;

        private SpriteLibrary _spriteLib;
        private ScrollDisplayer _scrollDisplay;
        private WorldChunkManager worldChunks;
        private const int ChunkSize = 100;

        private WorldMapSettings worldMapSettings;
        private FogOfWar _fog;
        private Random random = new Random();
        

        public WorldScene(SceneType sceneType, SpriteLibrary spriteLibray)
        {
            if (Instance == null)
            {
                Instance = this;
            }

            base.SceneType = sceneType;

            _spriteLib = spriteLibray;
        }

        public override void Initialize()
        {
            base.Initialize();

            worldMapSettings = CreateWorldMapSettings();
            _fog = new FogOfWar(worldMapSettings.Width, worldMapSettings.Height, Global.Player);

            _scrollDisplay = new ScrollDisplayer(Global.ContentManager, Global.SpriteBatch, Global.GraphicsDevice);
            _screenObjects.Add("scrollDisplay", _scrollDisplay);

            worldChunks = new WorldChunkManager(_spriteLib, _scrollDisplay, worldMapSettings);
            _gameObjects.Add("chunkmanager", worldChunks);

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


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
                Debugger.Lines[5] = $"Structure: {structure.Bounds}";
            }
            else
            {
                Debugger.Lines[5] = "Structure: null";
            }

            Debugger.Lines[6] = $"Camera Position: {Global.Camera.Position}";

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
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
