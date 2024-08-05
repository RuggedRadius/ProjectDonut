using BSPDungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectGorilla.GameObjects;
using System.Collections.Generic;
using System.Linq;

namespace ProjectDonut
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Dictionary<string, GameObject> _gameObjects;

        private char[,] map;
        private string[] mapStrings;

        private SpriteFont _font;

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

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Initialize());

            map = BSP2.Generate(50, 50, 5);
            mapStrings = MapToStrings(map);
            base.Initialize();
        }

        private string[] MapToStrings(char[,] map)
        {
            var result = new List<string>();

            for (int i = 0; i < map.GetLength(0); i++)
            {
                var line = string.Empty;

                for (int j = 0; j < map.GetLength(1); j++)
                {
                    line += map[i, j];
                }

                result.Add(line);
            }

            return result.ToArray();
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

            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            _gameObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));

            DrawMap();

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
