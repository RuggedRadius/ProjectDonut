using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDonut.GameObjects
{
    public class Debugger : GameObject
    {
        private SpriteBatch _spriteBatch;
        private ContentManager _content;
        private GraphicsDevice _graphicsDevice;
        private Camera _camera;

        private SpriteFont debugFont;
        private Texture2D debugTexture;
        private Rectangle debugRect;

        public string[] debug;

        public Debugger(SpriteBatch spriteBatch, ContentManager content, GraphicsDevice graphicsDevice, Camera camera)
        {
            _spriteBatch = spriteBatch;
            _content = content;
            _graphicsDevice = graphicsDevice;
            _camera = camera;

            debug = new string[5];
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            debugFont = _content.Load<SpriteFont>("Fonts/Default");
            debugTexture = CreateTexture(_graphicsDevice, 1, 1, Color.Black);
        }

        Texture2D CreateTexture(GraphicsDevice graphicsDevice, int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, width, height);
            Color[] colorData = new Color[width * height];
            for (int i = 0; i < colorData.Length; ++i) colorData[i] = color;
            texture.SetData(colorData);
            return texture;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Calculate the height of the debug panel based on the number of debug lines
            int height = 5;
            for (int i = 0; i < debug.Length; i++)
            {
                if (debug[i] != null)
                {
                    height += 30;
                }
            }

            // Set the position of the debug rectangle to the top-left corner of the screen
            int x = 10;
            int y = 10;

            debugRect = new Rectangle(x, y, 400, height);
        }

        public override void Draw(GameTime gameTime)
        {
            //_spriteBatch.Begin(transformMatrix: Matrix.Identity);

            _spriteBatch.Draw(debugTexture, debugRect, Color.Black);

            var camPos = _camera.Position;

            for (int i = 0; i < debug.Length; i++)
            {
                if (debug[i] == null)
                {
                    continue;
                }

                // Debug Text
                var pos = new Vector2(debugRect.X + 10, debugRect.Y + 5 + (30 * i));
                _spriteBatch.DrawString(debugFont, debug[i], pos, Color.White);
            }

            //_spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void PrintDataMap(int[,] map, string filePath)
        {
            var height = map.GetLength(0);
            var width = map.GetLength(1);

            var lines = new List<string>();

            for (int i = 0; i < width; i++)
            {
                var sb = new StringBuilder();
                for (int j = 0; j < height; j++)
                {
                    sb.Append(map[i, j]);
                    sb.Append(",");
                }
                lines.Add(sb.ToString());
            }

            File.WriteAllLines(filePath, lines);
        }
    }
}
