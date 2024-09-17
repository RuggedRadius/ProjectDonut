using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using ProjectDonut.Interfaces;
using ProjectDonut.UI.DialogueSystem;
using ProjectDonut.Debugging;
using System;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.Core;
using IGameComponent = ProjectDonut.Interfaces.IGameComponent;
using IScreenObject = ProjectDonut.Interfaces.IScreenObject;
using ProjectDonut.Debugging.Console;
using ProjectDonut.UI.ScrollDisplay;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Core.Input;
using Penumbra;
using ProjectDonut.Environment;

namespace ProjectDonut
{
    public class Game1 : Game
    {
        private SpriteFont _font;

        private Dictionary<string, IGameComponent> _gameComponents;
        private Dictionary<string, IGameObject> _gameObjects;
        private Dictionary<string, IScreenObject> _screenObjects;

        private DialogueManager dialogue;        
        private Random random = new Random();

        private List<IGameObject> _objectsToDraw;

        

        public Game1()
        {
            Global.GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Global.GraphicsDeviceManager.PreferredBackBufferWidth = Global.ScreenWidth;
            Global.GraphicsDeviceManager.PreferredBackBufferHeight = Global.ScreenHeight;
            Global.GraphicsDeviceManager.SupportedOrientations = DisplayOrientation.Portrait;

            Global.Penumbra = new PenumbraComponent(this);
            Components.Add(Global.Penumbra);

            DevConsole.InitialiseConsole(this);


            // ****************** TEMP DEBUG ***********************
            if (System.IO.Directory.Exists($@"C:\Users\benro\Documents\DEBUG"))
            {
                System.IO.Directory.Delete($@"C:\Users\benro\Documents\DEBUG", true);
            }
            System.IO.Directory.CreateDirectory($@"C:\Users\benro\Documents\DEBUG");
            // ****************** TEMP DEBUG ***********************
        }

        protected override void Initialize()
        {
            Global.GraphicsDevice = Global.GraphicsDeviceManager.GraphicsDevice;
            Global.SpriteBatch = new SpriteBatch(GraphicsDevice);
            Global.ContentManager = Content;

            Global.DEBUG_TEXTURE = new Texture2D(Global.GraphicsDevice, 1, 1);
            Global.DEBUG_TEXTURE.SetData(new[] { Color.Magenta });
            Global.MISSING_TEXTURE = new Texture2D(Global.GraphicsDevice, 64, 64);
            var missingColours = new Color[64 * 64];
            for (int i = 0; i < missingColours.Length; i++)
            {
                int row = i / 64;
                int col = i % 64;
                int squareSize = 16; // Change the square size here
                if ((row / squareSize + col / squareSize) % 2 == 0)
                    missingColours[i] = new Color(0, 0, 0, 0); // Color.Blue;
                else
                    missingColours[i] = Color.Red;
            }
            Global.MISSING_TEXTURE.SetData(missingColours);

            Global.SpriteLib = new Core.SpriteLibrary.SpriteLib();
            Global.SpriteLib.LoadSpriteLibrary();

            Global.PlayerObj = new PlayerObj();
            Global.PlayerObj.Initialize();

            CreateGameComponents();
            CreateGameObjects();
            CreateScreenObjects();

            _gameComponents.Select(x => x.Value).ToList().ForEach(x => x.Initialize());
            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());

            //Task.Run(() =>
            //{
            //    var test = dialogue.CreateTestDialogue();
            //    dialogue.ExecuteMultipleLines(test);
            //});

            base.Initialize();
        }

        private void CreateGameComponents()
        {
            _gameComponents = new Dictionary<string, IGameComponent>();

            // World map
            Global.SceneManager = new SceneManager();
            Global.SceneManager.CurrentSceneType = SceneType.World;

            // Camera
            Global.Camera = new Camera(this);

            Global.InputManager = new InputManager();



            _gameComponents.Add("sceneManager", Global.SceneManager);
            _gameComponents.Add("camera", Global.Camera);
            _gameComponents.Add("input", Global.InputManager);
        }

        private void CreateScreenObjects()
        {
            _screenObjects = new Dictionary<string, IScreenObject>();

            // Dialogue manager
            dialogue = new DialogueManager();
            _screenObjects.Add("dialogue", dialogue);

            // Game cursor
            Global.GameCursor = new GameCursor(this);
            _screenObjects.Add("cursor", Global.GameCursor);

            Global.DebugWindow = new DebugWindow();
            Global.DebugWindow.Initialize();
            _screenObjects.Add("debugWindow", Global.DebugWindow);

            Global.ScrollDisplay = new ScrollDisplayer();
            _screenObjects.Add("scrollDisplay", Global.ScrollDisplay);

            Global.Player.Inventory = new PlayerInventory();
            Global.Player.Inventory.Initialize();
            _screenObjects.Add("inventory", Global.Player.Inventory);


        }

        private void CreateGameObjects()
        {
            _gameObjects = new Dictionary<string, IGameObject>();

            Global.DayNightCycle = new DayNightCycle();
            _gameObjects.Add("day-night", Global.DayNightCycle);

            Global.Player.TextDisplay = new PlayerTextDisplay();
            Global.Player.TextDisplay.Initialize();
            _gameObjects.Add("playerTextDisplay", Global.Player.TextDisplay);
        }

        protected override void LoadContent()
        {
            Global.SceneManager.LoadContent();

            Global.PlayerObj.LoadContent();
            _gameComponents.Select(x => x.Value).ToList().ForEach(x => x.LoadContent());
            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent());
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent());

            _font = Content.Load<SpriteFont>("Fonts/Default");
        }

        protected override void Update(GameTime gameTime)
        {
            Global.InputManager.Update(gameTime);

            var kbState = Keyboard.GetState();

            if (InputManager.LastKeyboardState.IsKeyUp(Keys.OemTilde) &&
                InputManager.KeyboardState.IsKeyDown(Keys.OemTilde))
            {
                Global.Debug.Console.ToggleOpenClose();
                DebugWindow.IsShown = !DebugWindow.IsShown;
            }

            //if (kbState.IsKeyDown(Keys.F8))
            //{
            //    Global.SceneManager.SetCurrentScene(Global.SceneManager.Scenes["world"], SceneType.World);
            //    Global.SceneManager.CurrentScene.PrepareForPlayerEntry();
            //}

            //if (kbState.IsKeyDown(Keys.F9))
            //{
            //    var worldScene = (WorldScene)Global.SceneManager.CurrentScene;
            //    worldScene.LastExitLocation = new Rectangle((int)Global.Player.WorldPosition.X, (int)Global.Player.WorldPosition.Y, Global.TileSize, Global.TileSize);
            //    Global.SceneManager.SetCurrentScene(Global.SceneManager.Scenes["instance"], SceneType.Instance);
            //    Global.SceneManager.CurrentScene.PrepareForPlayerEntry();
            //}

            //if (kbState.IsKeyDown(Keys.O))
            //{
            //    //testScroll.DisplayScroll(500, 300, "Flandaria");                
            //    Global.ScrollDisplay.DisplayScroll();
            //}

            //if (kbState.IsKeyDown(Keys.P))
            //{
            //    Global.ScrollDisplay.HideScroll();
            //}zz

            Global.SceneManager.Update(gameTime);
            Global.PlayerObj.Update(gameTime);

            _gameComponents.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));
            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));

            DebugWindow.Lines[4] = $"Cursor: {Global.GameCursor.Position}";
            
            base.Update(gameTime);
        }

        //private List<IDrawable> _gameObjectsToDraw = new List<IDrawable>();
        //private void GetAllDrawableObjects()
        //{
        //    _gameObjectsToDraw.Clear();

        //    foreach (var go in _gameObjects)
        //    {
        //        _gameObjectsToDraw.Add(go.Value);

        //    }
        //}

        protected override void Draw(GameTime gameTime)
        {
            if (Global.LIGHTING_ENABLED)
            {
                Global.Penumbra.BeginDraw();
            }

            GraphicsDevice.Clear(Color.Black);

            Global.SceneManager.Draw(gameTime);

            _gameObjects
                .Select(x => x.Value)
                .OrderBy(x => x.WorldPosition.Y)
                .ThenBy(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));

            //if (Global.SceneManager.CurrentSceneType == SceneType.World)
            //{
            //    foreach (var chunk in Global.WorldChunkManager.CurrentChunks)
            //    {
            //        chunk.DrawMineableObjectsBelowPlayer(gameTime);    
            //        chunk.DrawSceneObjectsBelowPlayer(gameTime);
            //    }
            //}

            //Global.PlayerObj.Draw(gameTime);

            //if (Global.SceneManager.CurrentSceneType == SceneType.World)
            //{
            //    foreach (var chunk in Global.WorldChunkManager.CurrentChunks)
            //    {
            //        chunk.DrawMineableObjectsAbovePlayer(gameTime);
            //        chunk.DrawSceneObjectsAbovePlayer(gameTime);
            //    }
            //}

            //if (Global.LIGHTING_ENABLED == false)
            //{
            //    //Global.SpriteBatch.Begin();
            //    Global.Penumbra.BeginDraw();
            //}
            base.Draw(gameTime);
            //if (Global.LIGHTING_ENABLED == false)
            //{
            //    //Global.Penumbra.Draw(gameTime);
            //    //Global.SpriteBatch.End();
            //}
            //Global.Penumbra.Draw(gameTime);

            _screenObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));

            //Global.SpriteBatch.Begin();
            //base.Draw(gameTime);
            //Global.SpriteBatch.End();

        }
    }
}
