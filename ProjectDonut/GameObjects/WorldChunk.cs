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
    public class WorldChunk : GameObject
    {
        private int[,] heightData;
        private int[,] biomeData; // Switch to these later

        public Dictionary<string, Tilemap> tilemaps;

        //private WorldGenerator worldGen;
        private WorldMapSettings settings;

        private ContentManager content;
        private GraphicsDeviceManager graphics;
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;
        private Camera camera;
        private SpriteLibrary spriteLib;

        private SpriteFont debugFont;

        private int width;
        private int height;

        private Player player;
        public FogOfWar fog;
        Color halfOpacity = new Color(255, 255, 255, 128);

        public int ChunkXPos;
        public int ChunkYPos;

        public WorldChunk(List<object> dependencies, WorldMapSettings settings, int chunkXPos, int chunkYPos)
        {
            this.width = settings.Width;
            this.height = settings.Height;
            this.settings = settings;
            this.ChunkXPos = chunkXPos;
            this.ChunkYPos = chunkYPos;

            foreach (var dependency in dependencies)
            {
                switch (dependency)
                {
                    case ContentManager content:
                        this.content = content;
                        break;

                    case GraphicsDeviceManager graphicsDeviceManager:
                        this.graphics = graphicsDeviceManager;
                        break;

                    case GraphicsDevice graphicsDevice:
                        this.graphicsDevice = graphicsDevice;
                        break;

                    case SpriteBatch spriteBatch:
                        this.spriteBatch = spriteBatch;
                        break;

                    case Camera camera:
                        this.camera = camera;
                        break;

                    case Player player:
                        this.player = player;
                        break;

                    case FogOfWar fog:
                        this.fog = fog;
                        break;

                    case SpriteLibrary spriteLib:
                        this.spriteLib = spriteLib;
                        break;

                    default:
                        throw new ArgumentException("Unknown dependency type");
                }
            }

            if (content == null || graphics == null || spriteBatch == null || camera == null)
            {
                throw new ArgumentException("WorldMap: Missing dependencies");
            }

            tilemaps = new Dictionary<string, Tilemap>();

            //worldGen = new WorldGenerator(content, graphicsDevice, settings, spriteLib);
        }

        public override void Initialize()
        {
            ZIndex = 11;
        }

        public override void LoadContent()
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            var viewportRectangle = GetViewportRect();

            var playerChunkCoords = player.GetWorldChunkCoords();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    foreach (var tilemap in tilemaps)
                    {
                        var tile = tilemap.Value.Map[x, y];
                        if (tile == null)
                        {
                            continue;
                        }

                        // TEMP
                        var newX = tile.LocalPosition.X - (settings.Width * settings.TileSize * ChunkXPos);
                        var newY = tile.LocalPosition.Y - (settings.Height * settings.TileSize * ChunkYPos);
                        //tile.LocalPosition = new Vector2(newX, newY);

                        if (viewportRectangle.Contains(new Vector2(newX, newY).X, new Vector2(newX, newY).Y))
                        {
                            var isExplored = fog.IsTileExplored(x, y);

                            if (!isExplored)
                            {
                                spriteBatch.Draw(tile.Texture, new Vector2(newX, newY), null, Color.Black);
                            }
                            else
                            {
                                if (fog.IsTileInSightRadius(x, y, (int)player.position.X, (int)player.position.Y, ChunkXPos, ChunkYPos))
                                {
                                    spriteBatch.Draw(tile.Texture, new Vector2(newX, newY), null, Color.White);
                                }
                                else
                                {
                                    spriteBatch.Draw(tile.Texture, new Vector2(newX, newY), null, Color.Gray);
                                }
                            }
                            //if (player.ChunkPosX == ChunkXPos && player.ChunkPosY == ChunkYPos)
                            //{

                            //}
                            //else
                            //{
                            //    spriteBatch.Draw(tile.Texture, new Vector2(newX, newY), null, Color.Gray);
                            //}
                        }
                    }
                }
            }
        }

        private Rectangle GetViewportRect() 
        {
            // Adjust position based on the camera's zoom
            var halfViewportWidth = (int)((graphicsDevice.Viewport.Width / 2) / camera.Zoom);
            var halfViewportHeight = (int)((graphicsDevice.Viewport.Height / 2) / camera.Zoom);

            var xPosition = (int)camera.Position.X - halfViewportWidth - settings.TileSize;
            var yPosition = (int)camera.Position.Y - halfViewportHeight - settings.TileSize;

            // Adjust size based on the camera's zoom
            var width = (int)((graphicsDevice.Viewport.Width + settings.TileSize * 2) / camera.Zoom);
            var height = (int)((graphicsDevice.Viewport.Height + settings.TileSize * 2) / camera.Zoom);

            return new Rectangle(xPosition, yPosition, width, height);
        }

        public override void Update(GameTime gameTime)
        {            
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
