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
        public Tilemap mapBase;
        public Tilemap mapForest;

        private WorldGenerator worldGen;

        private ContentManager content;
        private GraphicsDeviceManager graphics;
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;
        private Camera camera;

        private int tileSize = 32;

        public WorldMap(ContentManager content, GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Camera camera, GraphicsDevice graphicsDevice)
        {
            
            this.content = content;
            this.graphics = graphics;
            this.spriteBatch = spriteBatch;
            this.camera = camera;
            this.graphicsDevice = graphicsDevice;

            worldGen = new WorldGenerator(content, graphicsDevice);
        }

        public override void Initialize()
        {
            ZIndex = 11;
        }

        public override void LoadContent()
        {
            mapBase = worldGen.Generate(1000, 1000);
        }

        public override void Draw(GameTime gameTime)
        {
            var viewportRectangle = GetViewportRect();

            foreach (var tile in mapBase.Map)
            {
                if (viewportRectangle.Contains(tile.Position.X, tile.Position.Y))
                {
                    spriteBatch.Draw(tile.Texture, tile.Position, null, Color.White);
                }
            }
        }

        private Rectangle GetViewportRect() 
        {
            // Adjust position based on the camera's zoom
            var halfViewportWidth = (int)((graphicsDevice.Viewport.Width / 2) / camera.Zoom);
            var halfViewportHeight = (int)((graphicsDevice.Viewport.Height / 2) / camera.Zoom);

            var xPosition = (int)camera.Position.X - halfViewportWidth - tileSize;
            var yPosition = (int)camera.Position.Y - halfViewportHeight - tileSize;

            // Adjust size based on the camera's zoom
            var width = (int)((graphicsDevice.Viewport.Width + tileSize * 2) / camera.Zoom);
            var height = (int)((graphicsDevice.Viewport.Height + tileSize * 2) / camera.Zoom);

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
