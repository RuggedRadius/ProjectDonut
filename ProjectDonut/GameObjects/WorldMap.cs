using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.World;
using ProjectGorilla.GameObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.GameObjects
{
    internal class WorldMap : GameObject
    {
        public Dictionary<string, Tilemap> tilemaps;

        private WorldGenerator worldGen;
        private WorldMapSettings settings;

        private ContentManager content;
        private GraphicsDeviceManager graphics;
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;
        private Camera camera;

        private SpriteFont debugFont;

        private int width;
        private int height;

        private Player player;
        public FogOfWar fog;
        Color halfOpacity = new Color(255, 255, 255, 128);

        public WorldMap(int width, int height, List<object> dependencies, WorldMapSettings settings)
        {
            this.width = width;
            this.height = height;
            this.settings = settings;

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

                    default:
                        throw new ArgumentException("Unknown dependency type");
                }
            }

            if (content == null || graphics == null || spriteBatch == null || camera == null)
            {
                throw new ArgumentException("WorldMap: Missing dependencies");
            }

            worldGen = new WorldGenerator(content, graphicsDevice, settings);
        }

        public override void Initialize()
        {
            ZIndex = 11;
        }

        public override void LoadContent()
        {
            tilemaps = new Dictionary<string, Tilemap>();
            tilemaps.Add("base", worldGen.GenerateBaseMap(width, height));
            tilemaps.Add("forest", worldGen.GenerateForestMap(width, height));

            //debugFont = content.Load<SpriteFont>("Fonts/Default");
            //SaveTilemapToFile($@"C:\DebugMapData_{DateTime.Now.ToString("hh-mm-sstt")}.png");
        }

        public override void Draw(GameTime gameTime)
        {
            var viewportRectangle = GetViewportRect();

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

                        if (viewportRectangle.Contains(tile.Position.X, tile.Position.Y))
                        {
                            var isExplored = fog.IsTileExplored(x, y);

                            if (!isExplored)
                            {
                                spriteBatch.Draw(tile.Texture, tile.Position, null, Color.Black);
                            }
                            else
                            {
                                if (fog.IsTileInSightRadius(x, y, (int)player.position.X, (int)player.position.Y))
                                {
                                    spriteBatch.Draw(tile.Texture, tile.Position, null, Color.White);
                                }
                                else
                                {
                                    spriteBatch.Draw(tile.Texture, tile.Position, null, Color.Gray);
                                }
                            }
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

        bool IsTileVisible(Rectangle tileBounds, Rectangle cameraBounds)
        {
            return tileBounds.Intersects(cameraBounds);
        }

        public override void Update(GameTime gameTime)
        {            
        }

        public void SaveTilemapToFile(string filePath)
        {
            int width = settings.Width * settings.TileSize;
            int height = settings.Height * settings.TileSize;

            // Create a RenderTarget2D with the size of the entire tilemap
            using (RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, width, height))
            {
                // Set the RenderTarget
                graphicsDevice.SetRenderTarget(renderTarget);

                // Clear the RenderTarget (optional)
                graphicsDevice.Clear(Color.Transparent);

                // Create a SpriteBatch to draw the textures
                SpriteBatch spriteBatch = new SpriteBatch(graphicsDevice);

                spriteBatch.Begin();

                // Loop through each texture and draw it on the RenderTarget
                for (int x = 0; x < settings.Width; x++)
                {
                    for (int y = 0; y < settings.Height; y++)
                    {
                        // Draw tiles
                        foreach (var tilemap in tilemaps)
                        {
                            var mapData = tilemap.Value.Map;
                            Tile tile = mapData[x, y];

                            if (tile == null)
                            {
                                continue;
                            }

                            Texture2D texture = tile.Texture;
                            Vector2 position = new Vector2(x * settings.TileSize, y * settings.TileSize);
                            spriteBatch.Draw(texture, position, Color.White);
                        }

                        // Draw height value
                        var heightValue = $"{worldGen.heightData[x, y]}";
                        var textPosition = new Vector2(x * settings.TileSize, y * settings.TileSize);
                        spriteBatch.DrawString(debugFont, heightValue, textPosition, Color.Black);
                    }
                }

                spriteBatch.End();

                // Reset the RenderTarget to null
                graphicsDevice.SetRenderTarget(null);

                // Save the RenderTarget as a PNG file
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    renderTarget.SaveAsPng(stream, renderTarget.Width, renderTarget.Height);
                }
            }
        }
    }
}
