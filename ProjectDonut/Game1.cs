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
using ProjectDonut.Tools;
using System;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.GameObjects.PlayerComponents;

namespace ProjectDonut
{
    public class Game1 : Game
    {
        // Naughty naughty
        public static GraphicsDeviceManager MyGraphicsDeviceManager;
        public static GraphicsDevice MyGraphicsDevice;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SceneManager _sceneManager;


        private SpriteFont _font;

        private Dictionary<string, IGameObject> _gameObjects;
        private Dictionary<string, IScreenObject> _screenObjects;

        private SpriteLibrary spriteLib;

        private Camera _camera;
        private Player player;
        private DialogueManager dialogue;
        private GameCursor _cursor;


        
        private Random random;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferredBackBufferHeight = 1440;

            random = new Random();

            MyGraphicsDeviceManager = _graphics;
            MyGraphicsDevice = _graphics.GraphicsDevice;
        }

        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _gameObjects = new Dictionary<string, IGameObject>();
            _screenObjects = new Dictionary<string, IScreenObject>();



            //worldChunks = new WorldChunk[3,3];

            spriteLib = new SpriteLibrary(Content, GraphicsDevice);

            // Fog of ware

            // Camera
            _camera = new Camera(GraphicsDevice);
            _gameObjects.Add("camera", _camera);

            // Player


            dialogue = new DialogueManager(spriteLib, _spriteBatch, _camera, Content);
            _screenObjects.Add("dialogue", dialogue);

            _cursor = new GameCursor(this, spriteLib, _spriteBatch, GraphicsDevice, _camera);
            _screenObjects.Add("cursor", _cursor);

            player = new Player(_graphics, GraphicsDevice, Content, _camera, _cursor);
            _gameObjects.Add("player", player);

            // World map
            _sceneManager = new SceneManager(Content, _spriteBatch, GraphicsDevice, player, spriteLib, _camera);
            _sceneManager.CreateWorldScene();
            _sceneManager.Initialize();
            _gameObjects.Add("sceneManager", _sceneManager);

            Debugger._spriteBatch = _spriteBatch;
            Debugger._content = Content;
            Debugger._graphicsDevice = GraphicsDevice;
            Debugger._camera = _camera;
            Debugger.Initialize();

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

        

        protected override void LoadContent()
        {
            Debugger.LoadContent();

            _sceneManager.LoadContent(Content);

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent(Content));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent());

            _font = Content.Load<SpriteFont>("Fonts/Default");
        }

        protected override void Update(GameTime gameTime)
        {
            _sceneManager.Update(gameTime);
            _camera.Position = _gameObjects["player"].Position;

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));

            Debugger.Lines[4] = $"Cursor: {_cursor.Position}";
            
            Debugger.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _sceneManager.Draw(gameTime, _spriteBatch);

            // GameObjects
            //_spriteBatch.Begin(transformMatrix: _camera.GetTransformationMatrix());
            _gameObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime, _spriteBatch));
            //_spriteBatch.End();

            // ScreenObjects
            _screenObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));

            Debugger.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
