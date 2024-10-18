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
using ProjectDonut.Core.Sprites;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using ProjectDonut.Tools;
using ProjectDonut.Combat.Combatants;
using ProjectDonut.Combat.Combatants.Base;

namespace ProjectDonut
{
    public class Game1 : Game
    {
        private SpriteFont _font;

        private Dictionary<string, IGameComponent> _gameComponents;
        private Dictionary<string, IGameObject> _gameObjects;
        private Dictionary<string, IScreenObject> _screenObjects;

        //private DialogueManager dialogue;        
        private Random random = new Random();

        private List<IGameObject> _objectsToDraw;
        private FPSCounter _fpsCounter;


        public Game1()
        {
            Global.GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Global.GraphicsDeviceManager.PreferredBackBufferWidth = Global.ScreenWidth;
            Global.GraphicsDeviceManager.PreferredBackBufferHeight = Global.ScreenHeight;
            //Global.GraphicsDeviceManager.IsFullScreen = true;

            Global.Penumbra = new PenumbraComponent(this);
            if (Global.LIGHTING_ENABLED)
                Components.Add(Global.Penumbra);

            DevConsole.InitialiseConsole(this);


            // ****************** TEMP DEBUG ***********************
            if (System.IO.Directory.Exists($@"C:\Users\benro\Documents\DEBUG"))
            {
                System.IO.Directory.Delete($@"C:\Users\benro\Documents\DEBUG", true);
            }
            System.IO.Directory.CreateDirectory($@"C:\Users\benro\Documents\DEBUG");
            System.IO.Directory.CreateDirectory($@"C:\Users\benro\Documents\DEBUG\MapThumbnails");
            // ****************** TEMP DEBUG ***********************

            _fpsCounter = new FPSCounter();
            IsFixedTimeStep = false; // Allow the game to run at variable frame rates
            Global.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false; // Disable VSync
            Global.GraphicsDeviceManager.ApplyChanges(); // Apply the changes to the graphics settings

            //Global.GraphicsDeviceManager.IsFullScreen = true;
        }

        protected override void Initialize()
        {
            Global.GraphicsDevice = Global.GraphicsDeviceManager.GraphicsDevice;
            Global.SpriteBatch = new SpriteBatch(GraphicsDevice);
            Global.ContentManager = Content;

            Global.BLANK_TEXTURE = new Texture2D(Global.GraphicsDevice, 1, 1);
            Global.BLANK_TEXTURE.SetData(new[] { Color.White });

            Global.DEBUG_TEXTURE = new Texture2D(Global.GraphicsDevice, 1, 1);
            Global.DEBUG_TEXTURE.SetData(new[] { Color.Magenta });
            Global.MISSING_TEXTURE = new Texture2D(Global.GraphicsDevice, Global.TileSize, Global.TileSize);
            var missingColours = new Color[Global.TileSize * Global.TileSize];
            for (int i = 0; i < missingColours.Length; i++)
            {
                int row = i / Global.TileSize;
                int col = i % Global.TileSize;
                int squareSize = 8; // Change the square size here
                if ((row / squareSize + col / squareSize) % 2 == 0)
                    missingColours[i] = new Color(0, 0, 0, 0); // Color.Blue;
                else
                    missingColours[i] = Color.Red;
            }
            Global.MISSING_TEXTURE.SetData(missingColours);

            Global.SpriteLibrary = new SpriteLib();
            Global.SpriteLibrary.LoadSpriteLibrary();

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


            TEST_COMBAT_SCENE();
        }

        private void TEST_COMBAT_SCENE()
        {
            var scene = new CombatScene(Global.SceneManager.CurrentScene);

            var playerTeam = new List<Combatant>();
            var enemyTeam = new List<Combatant>();

            for (int i = 0; i < 4; i++)
            {
                var randomIndex = random.Next(0, 4);

                switch (randomIndex)
                {
                    case 0: 
                        playerTeam.Add(new Goblin(TeamType.Player, scene.Manager) { Details = new CombatantDetails() { Name = NameGenerator.GenerateRandomName(2) } }); 
                        break;

                    case 1:
                        playerTeam.Add(new Skeleton(TeamType.Player, scene.Manager) { Details = new CombatantDetails() { Name = NameGenerator.GenerateRandomName(2) } });
                        break;

                    case 2:
                        playerTeam.Add(new Slime(TeamType.Player, scene.Manager) { Details = new CombatantDetails() { Name = NameGenerator.GenerateRandomName(2) } });
                        break;

                    case 3:
                        playerTeam.Add(new Rat(TeamType.Player, scene.Manager) { Details = new CombatantDetails() { Name = NameGenerator.GenerateRandomName(2) } });
                        break;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                var randomIndex = random.Next(0, 4);

                switch (randomIndex)
                {
                    case 0:
                        enemyTeam.Add(new Goblin(TeamType.Enemy, scene.Manager) { Details = new CombatantDetails() { Name = NameGenerator.GenerateRandomName(2) } });
                        break;

                    case 1:
                        enemyTeam.Add(new Skeleton(TeamType.Enemy, scene.Manager) { Details = new CombatantDetails() { Name = NameGenerator.GenerateRandomName(2) } });
                        break;

                    case 2:
                        enemyTeam.Add(new Slime(TeamType.Enemy, scene.Manager) { Details = new CombatantDetails() { Name = NameGenerator.GenerateRandomName(2) } });
                        break;

                    case 3:
                        enemyTeam.Add(new Rat(TeamType.Enemy, scene.Manager) { Details = new CombatantDetails() { Name = NameGenerator.GenerateRandomName(2) } });
                        break;
                }
            }

            scene.Manager.AddTeam(playerTeam, true);
            scene.Manager.AddTeam(enemyTeam, false);
            
            scene.Initialize();
            scene.LoadContent();
            Global.SceneManager.SetCurrentScene(scene);
        }

        private void CreateGameComponents()
        {
            _gameComponents = new Dictionary<string, IGameComponent>();

            // World map
            Global.SceneManager = new SceneManager();
            Global.SceneManager.CurrentSceneType = SceneType.World;

            // Camera
            Global.Camera = new Camera(this, false);
            

            Global.InputManager = new InputManager();
            Global.InputManager.Initialize();



            //_gameComponents.Add("sceneManager", Global.SceneManager);
            _gameComponents.Add("camera", Global.Camera);
            
            //_gameComponents.Add("input", Global.InputManager);
        }

        private void CreateScreenObjects()
        {
            _screenObjects = new Dictionary<string, IScreenObject>();

            // Dialogue manager
            Global.DialogueManager = new DialogueManager();
            _screenObjects.Add("dialogue", Global.DialogueManager);

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

            Global.Player.Equipment = new PlayerEquipment();
            Global.Player.Equipment.Initialize();
            _screenObjects.Add("equipment", Global.Player.Equipment);

            Global.CameraMinimap = new CameraMinimap(this, true);
            Global.CameraMinimap.Initialize();
            //_screenObjects.Add("camera-minimap", Global.CameraMinimap);
        }

        private void CreateGameObjects()
        {
            _gameObjects = new Dictionary<string, IGameObject>();

            Global.DayNightCycle = new DayNightCycle();
            Global.DayNightCycle.Initialize();
            //_gameObjects.Add("day-night", Global.DayNightCycle);

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

            Global.DayNightCycle.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            Global.InputManager.Update(gameTime);

            if (InputManager.IsKeyPressed(Keys.OemTilde))
            {
                Global.Debug.Console.ToggleOpenClose();
                DebugWindow.IsShown = !DebugWindow.IsShown;
            }


            Global.SceneManager.Update(gameTime);
            Global.PlayerObj.Update(gameTime);

            _gameComponents.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));
            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));

            DebugWindow.Lines[4] = $"Cursor: {Global.GameCursor.Position}";

            Global.CameraMinimap.Update(gameTime);
            Global.DayNightCycle.Update(gameTime);

            _fpsCounter.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (Global.LIGHTING_ENABLED)
            {
                Global.Penumbra.BeginDraw();
            }

            GraphicsDevice.Clear(Color.Black);

            // Minimap
            if (Global.SceneManager.CurrentScene is not CombatScene)
            {
                Global.GraphicsDevice.SetRenderTarget(Global.CameraMinimap.RenderTarget);
                Global.GraphicsDevice.Clear(Color.Black);
                Global.SpriteBatch.Begin(transformMatrix: Global.CameraMinimap.GetTransformationMatrix());
                Global.SceneManager.DrawMinimap(gameTime);
                Global.SpriteBatch.End();
                Global.GraphicsDevice.SetRenderTarget(null);
            }



            Global.SceneManager.Draw(gameTime);


            _gameObjects
                .Select(x => x.Value)
                .OrderBy(x => x.WorldPosition.Y)
                .ThenBy(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));

            _fpsCounter.Draw(gameTime);

            base.Draw(gameTime);

            _screenObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));

            if (Global.SceneManager.CurrentScene is not CombatScene)
            {
                Global.CameraMinimap.Draw(gameTime);
                Global.DayNightCycle.Draw(gameTime);
            }
        }
    }
}
