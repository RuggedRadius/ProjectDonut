using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.World;
using ProjectGorilla.GameObjects;
using System;
using System.Collections.Generic;
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

        private int width;
        private int height;

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
                            spriteBatch.Draw(tile.Texture, tile.Position, null, Color.White);
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
    }
}
