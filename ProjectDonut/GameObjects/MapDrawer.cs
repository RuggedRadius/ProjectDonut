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
        public Tilemap mapBase;
        public Tilemap mapForest;

        private Texture2D spriteSheet;

        private ContentManager content;
        private GraphicsDeviceManager graphics;
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;
        private Camera camera;
        private SpriteLibrary spriteLib;

        private int tileSize = 32;

        public MapDrawer(Tilemap mapBase, Tilemap mapForest, ContentManager content, GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Camera camera, GraphicsDevice graphicsDevice, SpriteLibrary spriteLib)
        {
            this.mapBase = mapBase;
            this.mapForest = mapForest;
            this.content = content;
            this.graphics = graphics;
            this.spriteBatch = spriteBatch;
            this.camera = camera;
            this.graphicsDevice = graphicsDevice;
            this.spriteLib = spriteLib;
        }

        public override void Draw(GameTime gameTime)
        {
            var viewportRectangle = GetViewportRect();

            var width = mapBase.Map.GetLength(0);
            var height = mapBase.Map.GetLength(1);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var tile = mapBase.Map[i, j];
                    var position = tile.Position;

                    if (viewportRectangle.Contains(position.X, position.Y))
                    {
                        // Base
                        spriteBatch.Draw(tile.Texture, tile.Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

                        // Forest
                        var forestTile = mapForest.Map[i, j];
                        if (forestTile != null)
                        {
                            spriteBatch.Draw(forestTile.Texture, forestTile.Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1);
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
