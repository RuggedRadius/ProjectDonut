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
            var viewportRectangle = new Rectangle(
                (int)camera.Position.X - (graphicsDevice.Viewport.Width / 2) - 32,
                (int)camera.Position.Y - (graphicsDevice.Viewport.Height / 2) - 32,
                graphicsDevice.Viewport.Width + 32,
                graphicsDevice.Viewport.Height + 32
            );

            foreach (var tile in map.Map)
            {
                if (viewportRectangle.Contains(tile.Position.X, tile.Position.Y))
                {
                    spriteBatch.Draw(tile.Texture, tile.Position, null, Color.White);
                }
            }
        }

        bool IsTileVisible(Rectangle tileBounds, Rectangle cameraBounds)
        {
            return tileBounds.Intersects(cameraBounds);
        }

        private Rectangle DetermineSourceRect(int dataValue)
        {
            if (dataValue >= 5)
            {
                return new Rectangle(tileSize * 1, tileSize * 1, tileSize, tileSize);
            }
            else
            {
                return new Rectangle(tileSize * 4, tileSize * 1, tileSize, tileSize);
            }


            switch (dataValue)
            {
                case 0: return new Rectangle(tileSize * 0, tileSize * 1, tileSize, tileSize);
                case 1: return new Rectangle(tileSize * 1, tileSize * 1, tileSize, tileSize);
                case 2: return new Rectangle(tileSize * 2, tileSize * 1, tileSize, tileSize);

                case 3: return new Rectangle(tileSize * 0, tileSize * 2, tileSize, tileSize);
                case 4: return new Rectangle(tileSize * 1, tileSize * 2, tileSize, tileSize);
                case 5: return new Rectangle(tileSize * 2, tileSize * 2, tileSize, tileSize);

                case 6: return new Rectangle(tileSize * 2, tileSize * 3, tileSize, tileSize);
                case 7: return new Rectangle(tileSize * 2, tileSize * 3, tileSize, tileSize);
                case 8: return new Rectangle(tileSize * 2, tileSize * 3, tileSize, tileSize);

                default: return new Rectangle(tileSize * 1, tileSize * 2, tileSize, tileSize);
            }
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
