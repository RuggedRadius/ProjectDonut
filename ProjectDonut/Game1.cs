using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectGorilla.GameObjects;
using System.Collections.Generic;
using System.Linq;
using ProjectDonut.ProceduralGeneration.World;
using ProjectDonut.GameObjects;
using System;
using ProjectDonut.ProceduralGeneration;

namespace ProjectDonut
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Dictionary<string, GameObject> _gameObjects;

        private SpriteFont _font;

        // World Map
        private Vector2 mapSizeWorld = new Vector2(1000, 1000);
        private WorldGenerator worldGenerator;
        private Tilemap map;
        private string[] mapStrings;

        private Camera camera = new Camera();

        

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
        }

        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _gameObjects = new Dictionary<string, GameObject>();

            _gameObjects.Add("player", new Player(_graphics, GraphicsDevice, Content, _spriteBatch));
            _gameObjects["player"].position = new Vector2(250, 250);

            worldGenerator = new WorldGenerator(Content, GraphicsDevice);
            map = worldGenerator.Generate((int)mapSizeWorld.X, (int)mapSizeWorld.Y);
            
            _gameObjects.Add("MapDrawer", new MapDrawer(map, Content, _graphics, _spriteBatch, camera, GraphicsDevice));

            _gameObjects.Add("camera", camera);

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent());

            _font = Content.Load<SpriteFont>("Fonts/Default");
        }

        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();



            camera.Position = _gameObjects["player"].position;

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));

            base.Update(gameTime);
        }



        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(transformMatrix: camera.GetTransformationMatrix(GraphicsDevice, GraphicsDevice.Viewport));


            _gameObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));

            //DrawMap();

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawMap()
        {
            var x = 0;
            var y = 0;

            foreach (var line in mapStrings)
            {
                _spriteBatch.DrawString(_font, $"{line}", new Vector2(x, y), Color.Green);

                y += 20;
            }
        }
    }
}
