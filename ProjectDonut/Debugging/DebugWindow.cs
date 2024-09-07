using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using ProjectDonut.GameObjects;
using ProjectDonut.Interfaces;

namespace ProjectDonut.Debugging
{
    public class DebugWindow : IScreenObject
    {
        public bool IsShown { get; set; }

        
        private Texture2D debugTexture;
        private Rectangle debugRect;
        private int maxWindowWidth;

        public static string[] Lines = new string[15];
        public int ZIndex { get; set; }

        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
            Global.FontDebug = Global.ContentManager.Load<SpriteFont>("Fonts/Default");
            debugTexture = CreateTexture(Global.GraphicsDevice, 1, 1, Color.Black);
        }

        private Texture2D CreateTexture(GraphicsDevice graphicsDevice, int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, width, height);
            Color[] colorData = new Color[width * height];
            for (int i = 0; i < colorData.Length; ++i) colorData[i] = color;
            texture.SetData(colorData);
            return texture;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsShown) return;

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

                var length = (int)Global.FontDebug.MeasureString(line).X + 10;
                if (length > maxWindowWidth)
                {
                    maxWindowWidth = length;
                }
            }

            debugRect = new Rectangle(x, y, maxWindowWidth, height);
        }
        
        public void Draw(GameTime gameTime)
        {
            if (!IsShown) return;

            Global.SpriteBatch.Begin(transformMatrix: Matrix.Identity);
            Global.SpriteBatch.Draw(debugTexture, debugRect, Color.Black * 0.5f);

            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i] == null)
                {
                    continue;
                }

                // Debug Text
                var pos = new Vector2(debugRect.X + 10, debugRect.Y + 5 + 30 * i);
                Global.SpriteBatch.DrawString(Global.FontDebug, Lines[i], pos, Color.White);
            }

            Global.SpriteBatch.End();
        }

        public static void PrintDataMap(int[,] map, string filePath)
        {
            var height = map.GetLength(1);
            var width = map.GetLength(0);

            var lines = new List<string>();

            for (int i = 0; i < width; i++)
            {
                var sb = new StringBuilder();
                for (int j = 0; j < height; j++)
                {
                    if (map[j, i] == 0)
                        sb.Append("  ");

                    if (map[j, i] == 1)
                        sb.Append("▓▓");

                    if (map[j, i] == 2)
                        sb.Append("░░");
                }
                lines.Add(sb.ToString());
            }

            File.WriteAllLines(filePath, lines);
        }

        public static void SaveIntArrayToFile(int[,] array, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                int rows = array.GetLength(0);
                int cols = array.GetLength(1);

                writer.WriteLine(rows);
                writer.WriteLine(cols);

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        writer.Write(array[i, j]);
                        if (j < cols - 1)
                            writer.Write(",");  // Separate elements with a comma
                    }
                    writer.WriteLine();  // New line for each row
                }
            }
        }

        public static int[,] LoadIntArrayFromFile(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                int rows = int.Parse(reader.ReadLine());
                int cols = int.Parse(reader.ReadLine());

                int[,] array = new int[rows, cols];

                for (int i = 0; i < rows; i++)
                {
                    string[] line = reader.ReadLine().Split(',');

                    for (int j = 0; j < cols; j++)
                    {
                        array[i, j] = int.Parse(line[j]);
                    }
                }

                return array;
            }
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
