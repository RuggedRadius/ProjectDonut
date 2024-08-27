using System;
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

            var width = 50;
            var height = 50;
            var rooms = _bsp.GenerateRooms(width, height);
            rooms[rooms.Count - 1] = _bsp.CreateRoomsWithinAreas(rooms[rooms.Count - 1]);
            var dataMap = _bsp.CreateDataMap(rooms[rooms.Count - 1], width, height);
            var linkages = _bsp.LinkAllRooms(rooms[rooms.Count - 1], width, height);
            var dungeon = BSP.MergeArrays(dataMap, linkages);
            Debugging.Debugger.PrintDataMap(dungeon, @"C:\Dungeon.txt");
        }

        public void LoadContent(ContentManager content)
        {
            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent(content));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent());
        }

        public void Update(GameTime gameTime)
        {
            var kbState = Keyboard.GetState();

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));
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
    }
}
