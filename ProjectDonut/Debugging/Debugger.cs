using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.GameObjects;
using ProjectDonut.Interfaces;

namespace ProjectDonut.Debugging
{
    public static class Debugger
    {
        public static SpriteBatch _spriteBatch;
        public static ContentManager _content;
        public static GraphicsDevice _graphicsDevice;
        public static Camera _camera;

        private static SpriteFont debugFont;
        private static Texture2D debugTexture;
        private static Rectangle debugRect;
        private static int maxWindowWidth;

        public static string[] Lines = new string[10];

        public static int ZIndex { get; set; }

        //public Debugger(SpriteBatch spriteBatch, ContentManager content, GraphicsDevice graphicsDevice, Camera camera)
        //{
        //    _spriteBatch = spriteBatch;
        //    _content = content;
        //    _graphicsDevice = graphicsDevice;
        //    _camera = camera;

        //    Lines = new string[10];
        //}

        public static void Initialize()
        {
        }

        public static void LoadContent()
        {
            debugFont = _content.Load<SpriteFont>("Fonts/Default");
            debugTexture = CreateTexture(_graphicsDevice, 1, 1, Color.Black);
        }

        private static Texture2D CreateTexture(GraphicsDevice graphicsDevice, int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, width, height);
            Color[] colorData = new Color[width * height];
            for (int i = 0; i < colorData.Length; ++i) colorData[i] = color;
            texture.SetData(colorData);
            return texture;
        }

        public static void Update(GameTime gameTime)
        {
            // Calculate the height of the debug panel based on the number of debug lines
            int height = 5;
            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i] != null)
                {
                    height += 30;
                }
            }

            // Set the position of the debug rectangle to the top-left corner of the screen
            int x = 10;
            int y = 10;

            foreach (var line in Lines)
            {
                if (line == null)
                {
                    continue;
                }

                var length = (int)debugFont.MeasureString(line).X + 10;
                if (length > maxWindowWidth)
                {
                    maxWindowWidth = length;
                }
            }

            debugRect = new Rectangle(x, y, maxWindowWidth, height);
        }
        
        public static void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(transformMatrix: Matrix.Identity);
            _spriteBatch.Draw(debugTexture, debugRect, Color.Black);

            var camPos = _camera.Position;

            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i] == null)
                {
                    continue;
                }

                // Debug Text
                var pos = new Vector2(debugRect.X + 10, debugRect.Y + 5 + 30 * i);
                _spriteBatch.DrawString(debugFont, Lines[i], pos, Color.White);
            }

            _spriteBatch.End();
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

        //public void SaveTilemapToFile(string filePath)
        //{
        //    int width = settings.Width * settings.TileSize;
        //    int height = settings.Height * settings.TileSize;

        //    // Create a RenderTarget2D with the size of the entire tilemap
        //    using (RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, width, height))
        //    {
        //        // Set the RenderTarget
        //        graphicsDevice.SetRenderTarget(renderTarget);

        //        // Clear the RenderTarget (optional)
        //        graphicsDevice.Clear(Color.Transparent);

        //        // Create a SpriteBatch to draw the textures
        //        SpriteBatch spriteBatch = new SpriteBatch(graphicsDevice);

        //        spriteBatch.Begin();

        //        // Loop through each texture and draw it on the RenderTarget
        //        for (int x = 0; x < settings.Width; x++)
        //        {
        //            for (int y = 0; y < settings.Height; y++)
        //            {
        //                // Draw tiles
        //                foreach (var tilemap in tilemaps)
        //                {
        //                    var mapData = tilemap.Value.Map;
        //                    Tile tile = mapData[x, y];

        //                    if (tile == null)
        //                    {
        //                        continue;
        //                    }

        //                    Texture2D texture = tile.Texture;
        //                    Vector2 position = new Vector2(x * settings.TileSize, y * settings.TileSize);
        //                    spriteBatch.Draw(texture, position, Color.White);
        //                }

        //                // Draw height value
        //                var heightValue = $"{heightData[x, y]}";
        //                var textPosition = new Vector2(x * settings.TileSize, y * settings.TileSize);
        //                spriteBatch.DrawString(debugFont, heightValue, textPosition, Color.Black);
        //            }
        //        }

        //        spriteBatch.End();

        //        // Reset the RenderTarget to null
        //        graphicsDevice.SetRenderTarget(null);

        //        // Save the RenderTarget as a PNG file
        //        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            renderTarget.SaveAsPng(stream, renderTarget.Width, renderTarget.Height);
        //        }
        //    }
        //}
    }
}
