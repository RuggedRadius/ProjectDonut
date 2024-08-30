﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.GameObjects;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.Tools;
using ProjectDonut.UI.ScrollDisplay;
using ProjectDonut.ProceduralGeneration.Dungeons.BSP;
using System.Diagnostics;
using ProjectDonut.ProceduralGeneration.Dungeons;
using System.IO;

namespace ProjectDonut.Core.SceneManagement
{
    public class InstanceScene
    {
        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphicsDevice;

        private Dictionary<string, IGameObject> _gameObjects;
        private Dictionary<string, IScreenObject> _screenObjects;

        private SpriteLibrary _spriteLib;
        private Camera _camera;

        private FogOfWar _fog;
        private Random random;

        private Player _player;

        // Instance-related 
        private BSP _bsp;

        public SceneType SceneType { get; private set; }
        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }

        private Tilemap _tilemap;

        private const int Dimension = 100;

        public InstanceScene(SceneType sceneType, Player player, ContentManager content, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Camera camera, SpriteLibrary spriteLibray)
        {
            SceneType = sceneType;
            random = new Random();
            _player = player;

            _content = content;
            _spriteBatch = spriteBatch;
            _graphicsDevice = graphicsDevice;
            _camera = camera;
            _spriteLib = spriteLibray;

            _bsp = new BSP();
        }

        public void Initialize()
        {
            _gameObjects = new Dictionary<string, IGameObject>();
            _screenObjects = new Dictionary<string, IScreenObject>();

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());

            _tilemap = GenerateDungeon(Dimension, Dimension, true, false);
        }

        private int[,] dataMap;
        private Tilemap GenerateDungeon(int width, int height, bool loadLast, bool squashRooms)
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

            Debugging.Debugger.PrintDataMap(dataMap, @"C:\Dungeon.txt");
            Debugging.Debugger.SaveIntArrayToFile(dataMap, path);

            var generator = new DungeonGenerator(_spriteBatch, _content, _graphicsDevice);
            return generator.CreateTileMap(dataMap);
        }

        public void SlowBuild(int width, int height)
        {

        }

        public void LoadContent(ContentManager content)
        {
            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent(content));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent());
        }

        public void Update(GameTime gameTime)
        {
            var kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.F1))
            {
                _tilemap = GenerateDungeon(Dimension, Dimension, false, false);
            }

            if (kbState.IsKeyDown(Keys.F2))
            {
                _tilemap = GenerateDungeon(Dimension, Dimension, false, true);
            }

            if (kbState.IsKeyDown(Keys.F4))
            {
                var path = @"C:\DungeonData.txt";
                dataMap = Debugging.Debugger.LoadIntArrayFromFile(path);
                _tilemap = GenerateDungeon(Dimension, Dimension, true, true);
            }

            if (kbState.IsKeyDown(Keys.F5))
            {
                var path = @"C:\DungeonData.txt";
                Debugging.Debugger.SaveIntArrayToFile(dataMap, path);
            }

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _spriteBatch.Begin(transformMatrix: _camera.GetTransformationMatrix());

            foreach (var tile in _tilemap.Map)
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
            _spriteBatch.End();

            // ScreenObjects
            _screenObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));
        }
    }
}
