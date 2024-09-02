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
        private Dictionary<string, IGameObject> _gameObjects;
        private Dictionary<string, IScreenObject> _screenObjects;
    
        private Random random = new Random();


        public Game1()
        {
            Global.GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Global.GraphicsDeviceManager.PreferredBackBufferWidth = 1920;
            Global.GraphicsDeviceManager.PreferredBackBufferHeight = 1080;

            _gameObjects = new Dictionary<string, IGameObject>();
            _screenObjects = new Dictionary<string, IScreenObject>();

            Global.InputManager = new Core.Input.InputManager();
            _gameObjects.Add("inputManager", Global.InputManager);
        }

        protected override void Initialize()
        {
            Global.GraphicsDevice = Global.GraphicsDeviceManager.GraphicsDevice;
            Global.SpriteBatch = new SpriteBatch(Global.GraphicsDevice);
            Global.ContentManager = Content;

            Global.SpriteLibrary = new SpriteLibrary();
            Global.Camera = new Camera();
            Global.DialogueManager = new DialogueManager();
            Global.GameCursor = new GameCursor(this);
            Global.Player = new Player();
            Global.SceneManager = new SceneManager();
            //Global.Pathfinding = new Pathfinding.Astar();

            _gameObjects.Add("camera", Global.Camera);            
            _screenObjects.Add("dialogue", Global.DialogueManager);            
            _screenObjects.Add("cursor", Global.GameCursor);            
            _gameObjects.Add("player", Global.Player);            
            _gameObjects.Add("sceneManager", Global.SceneManager);

            Debugger.Initialize();

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());

            //Task.Run(() =>
            //{
            //    var test = Global.DialogueManager.CreateTestDialogue();
            //    Global.DialogueManager.ExecuteMultipleLines(test);
            //});

            // Position player in middle of the map
            //Global.Player.PositionPlayerInMiddleOfMap(worldMapSettings);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Debugger.LoadContent();

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent(Content));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent());
        }

        protected override void Update(GameTime gameTime)
        {
            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));

            Debugger.Lines[4] = $"Cursor: {Global.GameCursor.Position}";
            
            Debugger.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Global.SceneManager.Draw(gameTime, Global.SpriteBatch);

            // GameObjects
            //Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            _gameObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime, Global.SpriteBatch));
            //Global.SpriteBatch.End();

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
