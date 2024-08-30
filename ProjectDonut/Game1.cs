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
        private SpriteFont _font;

        private Dictionary<string, IGameObject> _gameObjects;
        private Dictionary<string, IScreenObject> _screenObjects;

        private SpriteLibrary spriteLib;
        private DialogueManager dialogue;        
        private Random random = new Random();


        public Game1()
        {
            Global.GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Global.GraphicsDeviceManager.PreferredBackBufferWidth = 1920;
            Global.GraphicsDeviceManager.PreferredBackBufferHeight = 1080;
            //Global.GraphicsDeviceManager.PreferredBackBufferHeight = 1440;
        }

        protected override void Initialize()
        {
            Global.GraphicsDevice = Global.GraphicsDeviceManager.GraphicsDevice;
            Global.SpriteBatch = new SpriteBatch(GraphicsDevice);
            Global.ContentManager = Content;

            _gameObjects = new Dictionary<string, IGameObject>();
            _screenObjects = new Dictionary<string, IScreenObject>();



            //worldChunks = new WorldChunk[3,3];

            spriteLib = new SpriteLibrary(Content, GraphicsDevice);
            spriteLib.LoadSpriteLibrary();

            // Fog of ware

            // Camera
            Global.Camera = new Camera(GraphicsDevice);
            _gameObjects.Add("camera", Global.Camera);

            // Player


            dialogue = new DialogueManager(spriteLib);
            _screenObjects.Add("dialogue", dialogue);

            Global.GameCursor = new GameCursor(this, spriteLib);
            _screenObjects.Add("cursor", Global.GameCursor);

            Global.Player = new Player();
            _gameObjects.Add("player", Global.Player);

            // World map
            Global.SceneManager = new SceneManager(spriteLib);
            _gameObjects.Add("sceneManager", Global.SceneManager);

            Debugger.Initialize();

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());

            //Task.Run(() =>
            //{
            //    var test = dialogue.CreateTestDialogue();
            //    dialogue.ExecuteMultipleLines(test);
            //});

            // Position player in middle of the map
            //player.PositionPlayerInMiddleOfMap(worldMapSettings);

            base.Initialize();
        }

        

        protected override void LoadContent()
        {
            Debugger.LoadContent();

            Global.SceneManager.LoadContent(Content);

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent(Content));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent());

            _font = Content.Load<SpriteFont>("Fonts/Default");
        }

        protected override void Update(GameTime gameTime)
        {
            var kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.F8))
            {
                Global.SceneManager.SetCurrentScene(Global.SceneManager.Scenes["world"]);
            }

            if (kbState.IsKeyDown(Keys.F9))
            {
                Global.SceneManager.SetCurrentScene(Global.SceneManager.Scenes["instance"]);
            }

            Global.SceneManager.Update(gameTime);
            Global.Camera.Position = _gameObjects["player"].Position;

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
