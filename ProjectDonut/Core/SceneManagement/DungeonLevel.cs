﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.Dungeons.BSP;
using ProjectDonut.ProceduralGeneration.Dungeons.DungeonPopulation;
using ProjectDonut.ProceduralGeneration.Dungeons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectDonut.ProceduralGeneration;
using System.IO;

namespace ProjectDonut.Core.SceneManagement
{
    public class DungeonLevel : Scene
    {
        //private FogOfWar _fog;
        private Random random = new Random();

        // Instance-related 
        public int[,] DataMap;
        private BSP _bsp;
        private Tilemap _tilemap;
        private Tilemap _populationMap;

        private const int Dimension = 100;

        private Rectangle EntryLocation;

        private Dictionary<string, Rectangle> ExitLocations;
        public List<IGameObject> Enemies { get; set; }

        public DungeonLevel()
        {
            _bsp = new BSP();
        }

        public override void Initialize()
        {
            base.Initialize();

            GenerateDungeon(true, false);
        }

        private void GenerateDungeon(bool loadLast, bool SquashRooms)
        {
            _tilemap = GenerateDungeonTileMap(Dimension, Dimension, loadLast, SquashRooms);

            var popSettings = new DungeonLevelSettings()
            {
                EnemyCount = 50,
                PossibleEnemies = new Dictionary<string, int>()
                {
                    { "goblin", 10 },
                    { "orc", 5 }
                },
                HasSubsequentLevel = true
            };

            var dungeonPopulater = new DungeonPopulater(DataMap);
            dungeonPopulater.PopulateDungeon(popSettings);
            _populationMap = dungeonPopulater.CreateTileMap();

            Enemies = new List<IGameObject>();
            var enemies = dungeonPopulater.CreateEnemies(popSettings);
            for (int i = 0; i < enemies.Count; i++)
            {
                _gameObjects.Add($"enemy-{i + 1}", enemies[i]);
                Enemies.Add(enemies[i]);
            }

            var stairsLocation = dungeonPopulater.GetStairsLocation();
            EntryLocation = new Rectangle(stairsLocation.Item1, stairsLocation.Item2, 32, 32);
            //Global.Player.Position = new Vector2(EntryLocation.X, EntryLocation.Y);

            ExitLocations = new Dictionary<string, Rectangle>();
            var exitPoints = dungeonPopulater.GetExitLocations();
            foreach (var point in exitPoints)
            {
                ExitLocations.Add("world-exit", new Rectangle(point.Item1, point.Item2, 32, 32));
            }
        }


        private Tilemap GenerateDungeonTileMap(int width, int height, bool loadLast, bool squashRooms)
        {
            var path = @"C:\DungeonData.txt";
            var dataMap = new int[width, height];
            if (File.Exists(path) && loadLast)
            {
                dataMap = Debugging.Debugger.LoadIntArrayFromFile(path);
            }
            else
            {
                // Generate rooms
                var rooms = _bsp.GenerateRooms(width, height);
                rooms[rooms.Count - 1] = _bsp.CreateRoomsWithinAreas(rooms[rooms.Count - 1]);

                // Squash rooms in
                if (squashRooms)
                {
                    rooms[rooms.Count - 1] = _bsp.SquashRooms(rooms[rooms.Count - 1], width, height);
                }

                // Generate data map
                dataMap = _bsp.CreateDataMap(rooms[rooms.Count - 1], width, height);

                // Link rooms
                var rects = rooms[rooms.Count - 1].Select(x => x.Bounds).ToList();
                var rectLinker = new RectangleLinker();
                var links = rectLinker.LinkRectangles(rects);
                var linkages = _bsp.LinkAllRooms(links, dataMap);
                dataMap = BSP.MergeArrays(dataMap, linkages);
            }

            DataMap = dataMap;

            //Debugging.Debugger.PrintDataMap(dataMap, @"C:\Dungeon.txt");
            //Debugging.Debugger.SaveIntArrayToFile(dataMap, path);

            var generator = new DungeonGenerator();
            return generator.CreateTileMap(dataMap);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.F1))
            {
                GenerateDungeon(false, false);
            }

            if (kbState.IsKeyDown(Keys.F2))
            {
                GenerateDungeon(false, true);
            }

            if (kbState.IsKeyDown(Keys.F4))
            {
                var path = @"C:\DungeonData.txt";
                DataMap = Debugging.Debugger.LoadIntArrayFromFile(path);
                _tilemap = GenerateDungeonTileMap(Dimension, Dimension, true, true);
            }

            if (kbState.IsKeyDown(Keys.F5))
            {
                var path = @"C:\DungeonData.txt";
                Debugging.Debugger.SaveIntArrayToFile(DataMap, path);
            }

            foreach (var exitPoint in ExitLocations)
            {
                if (exitPoint.Value.Contains(Global.Player.ChunkPosition))
                {

                    Global.SceneManager.SetCurrentScene(Global.SceneManager.Scenes["world"]);
                    ((WorldScene)Global.SceneManager.CurrentScene).PrepareForPlayerEntry();
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());

            foreach (var tile in _tilemap.Map)
            {
                if (tile == null)
                    continue;

                tile.Draw(gameTime, spriteBatch);
            }

            foreach (var tile in _populationMap.Map)
            {
                if (tile == null)
                    continue;

                tile.Draw(gameTime, spriteBatch);
            }

            _gameObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime, spriteBatch));
            spriteBatch.End();

            // ScreenObjects
            _screenObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));
        }

        public override void PrepareForPlayerEntry()
        {
            base.PrepareForPlayerEntry();

            Global.Player.Position = new Vector2(EntryLocation.X, EntryLocation.Y);
        }
    }
}
