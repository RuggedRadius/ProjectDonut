using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.GameObjects;
using System.Collections.Generic;
using System.Linq;
using ProjectDonut.ProceduralGeneration.World;
using System.Threading.Tasks;
using ProjectDonut.Interfaces;
using ProjectDonut.UI.DialogueSystem;
using ProjectDonut.UI.ScrollDisplay;
using ProjectDonut.Debugging;
using ProjectDonut.Tools;
using System;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.Core;

namespace ProjectDonut
{
    public class Game1 : Game
    {
        private SpriteFont _font;

        private Dictionary<string, IGameObject> _gameObjects;
        private Dictionary<string, IScreenObject> _screenObjects;

        private DialogueManager dialogue;        
        private Random random = new Random();

        private List<IGameObject> _objectsToDraw();


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

            Global.DEBUG_TEXTURE = new Texture2D(Global.GraphicsDevice, 1, 1);
            Global.DEBUG_TEXTURE.SetData(new[] { Color.Magenta });

            //worldChunks = new WorldChunk[3,3];

            Global.SpriteLibrary = new SpriteLibrary();
            Global.SpriteLibrary.LoadSpriteLibrary();

            // Fog of ware

            // Camera
            Global.Camera = new Camera();
            _gameObjects.Add("camera", Global.Camera);

            // Player


            dialogue = new DialogueManager();
            _screenObjects.Add("dialogue", dialogue);

            Global.GameCursor = new GameCursor(this);
            _screenObjects.Add("cursor", Global.GameCursor);

            Global.Player = new Player();
            //_gameObjects.Add("player", Global.Player);

            // World map
            Global.SceneManager = new SceneManager();
            Global.SceneManager.CurrentSceneType = SceneType.World;
            _gameObjects.Add("sceneManager", Global.SceneManager);

            Debugger.Initialize();

            Global.Player.Initialize();
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

            Global.Player.LoadContent(Content);
            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent(Content));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent());

            _font = Content.Load<SpriteFont>("Fonts/Default");
        }

        protected override void Update(GameTime gameTime)
        {
            var kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.F8))
            {
                Global.SceneManager.SetCurrentScene(Global.SceneManager.Scenes["world"], SceneType.World);
                Global.SceneManager.CurrentScene.PrepareForPlayerEntry();
            }

            if (kbState.IsKeyDown(Keys.F9))
            {
                var worldScene = (WorldScene)Global.SceneManager.CurrentScene;
                worldScene.LastExitLocation = new Rectangle((int)Global.Player.Position.X, (int)Global.Player.Position.Y, Global.TileSize, Global.TileSize);
                Global.SceneManager.SetCurrentScene(Global.SceneManager.Scenes["instance"], SceneType.Instance);
                Global.SceneManager.CurrentScene.PrepareForPlayerEntry();
            }

            Global.SceneManager.Update(gameTime);
            Global.Camera.Position = Global.Player.Position;

            Global.Player.Update(gameTime);
            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));

            Debugger.Lines[4] = $"Cursor: {Global.GameCursor.Position}";
            
            Debugger.Update(gameTime);
            base.Update(gameTime);
        }

        //private Dictionary<object, int> _gameObjectsToDraw = new Dictionary<object, int>();
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Global.SceneManager.Draw(gameTime, Global.SpriteBatch);

            _gameObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime, Global.SpriteBatch));

            if (Global.SceneManager.CurrentSceneType == SceneType.World)
            {
                foreach (var chunk in Global.WorldChunkManager.CurrentChunks)
                {
                    chunk.DrawSceneObjectsBelowPlayer();
                }
            }

            Global.Player.Draw(gameTime, Global.SpriteBatch);

            if (Global.SceneManager.CurrentSceneType == SceneType.World)
            {
                foreach (var chunk in Global.WorldChunkManager.CurrentChunks)
                {
                    chunk.DrawSceneObjectsAbovePlayer();
                }
            }

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
