using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.World;
using ProjectDonut.GameObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.GameObjects
{
    public class WorldChunk : IGameObject
    {
        public int ChunkCoordX { get; private set; }
        public int ChunkCoordY { get; private set; }

        public int WorldCoordX;
        public int WorldCoordY;
        
        public int[,] HeightData;
        public int[,] BiomeData;
        public int[,] ForestData;
        public int[,] RiverData;

        private int TileSize = 32;

        public Dictionary<string, Tilemap> Tilemaps;

        public int Width 
        { 
            get
            {
                if (HeightData == null)
                    return 0;

                return HeightData.GetLength(0);
            }
            set
            {
                Width = value;
            }
        }
        public int Height
        {
            get
            {
                if (HeightData == null)
                    return 0;

                return HeightData.GetLength(1);
            }
            set
            {
                Height = value;
            }
        }

        private Texture2D tempTexture;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;

        public WorldChunk(int chunkXPos, int chunkYPos, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            ChunkCoordX = chunkXPos;
            ChunkCoordY = chunkYPos;

            WorldCoordX = chunkXPos * 100 * 32;
            WorldCoordY = chunkYPos * 100 * 32;

            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;

            Tilemaps = new Dictionary<string, Tilemap>();

            // Create a new Texture2D object with the dimensions 32x32
            tempTexture = new Texture2D(_graphicsDevice, 32, 32);

            // Create an array to hold the color data
            Color[] colorData = new Color[32 * 32];

            // Fill the array with Color.White
            for (int i = 0; i < colorData.Length; i++)
            {
                colorData[i] = Color.White;
            }

            // Set the texture data to the array of colors
            tempTexture.SetData(colorData);

        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var tilemap in Tilemaps)
            {
                foreach (var tile in tilemap.Value.Map)
                {
                    if (tile == null)
                        continue;

                    var x = WorldCoordX + (tile.LocalPosition.X);
                    var y = WorldCoordY + (tile.LocalPosition.Y);
                    var position = new Vector2(x, y);
                    _spriteBatch.Draw(tile.Texture, position, null, Color.White);
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var position = new Vector2(WorldCoordX + (x * 32), WorldCoordY + (y * 32));

                    if (x == 0 || y == 0)
                    {
                        _spriteBatch.Draw(tempTexture, position, null, Color.Magenta);
                    }
                    else if (x == Width - 1 || y == Height - 1)
                    {
                        _spriteBatch.Draw(tempTexture, position, null, Color.Magenta);
                    }
                }
            }

            return;
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
