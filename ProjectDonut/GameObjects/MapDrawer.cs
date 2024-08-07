using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.ProceduralGeneration;
using ProjectGorilla.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.GameObjects
{
    internal class MapDrawer : GameObject
    {
        private Tilemap map;

        private Texture2D spriteSheet;

        private ContentManager content;
        private GraphicsDeviceManager graphics;
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;
        private Camera camera;

        private int tileSize = 32;

        public MapDrawer(Tilemap mapData, ContentManager content, GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Camera camera, GraphicsDevice graphicsDevice)
        {
            this.map = mapData;
            this.content = content;
            this.graphics = graphics;
            this.spriteBatch = spriteBatch;
            this.camera = camera;
            this.graphicsDevice = graphicsDevice;
        }

        public override void Draw(GameTime gameTime)
        {
            var viewportRectangle = GetViewportRect();

            foreach (var tile in map.Map)
            {
                if (viewportRectangle.Contains(tile.Position.X, tile.Position.Y))
                {
                    spriteBatch.Draw(tile.Texture, tile.Position, null, Color.White);
                }
            }
        }

        private Rectangle GetViewportRect() 
        {
            var tileSize = 32;

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

        public override void Initialize()
        {
            ZIndex = 11;
        }

        public override void LoadContent()
        {
            spriteSheet = content.Load<Texture2D>("Sprites/Map/World/WorldTerrain01");
        }

        public override void Update(GameTime gameTime)
        {            
        }
    }
}
