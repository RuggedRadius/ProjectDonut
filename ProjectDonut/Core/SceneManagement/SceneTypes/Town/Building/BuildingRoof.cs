using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core.Sprites;
using ProjectDonut.Interfaces;

namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town.Building
{
    public class BuildingRoof : IGameObject
    {
        public Dictionary<Rectangle, Texture2D> RoofSprites { get; set; }
        public BuildingObj ParentBuilding { get; set; }

        public bool IsVisible => throw new NotImplementedException();
        public Texture2D Texture { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Vector2 WorldPosition { get; set; }
        public int ZIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public BuildingRoof(BuildingObj parentBuilding)
        {
            ParentBuilding = parentBuilding;
            WorldPosition = parentBuilding.BuildingWorldBounds.Location.ToVector2();
        }

        public void BuildRoof(BuildingLevel topLevel)
        {
            RoofSprites = new Dictionary<Rectangle, Texture2D>();

            BuildLeft(topLevel);
            BuildRight(topLevel);
            BuildMiddle(topLevel);

            // NOT WORKING
            //var combinedTexture = CombineTextures(RoofSprites);
            //RoofSprites.Clear();
            //RoofSprites.Add(ParentBuilding.BuildingBounds, combinedTexture);
        }

        private void BuildMiddle(BuildingLevel topLevel)
        {
            var bounds = topLevel.ParentBuilding.BuildingWorldBounds;

            // Alternating front panels
            var alternatingSprites = new Texture2D[2] {
                SpriteLib.Town.Roof["roof-top-front"],
                SpriteLib.Town.Roof["roof-top-front2"]
            };
            var width = Global.TileSize;
            var height = Global.TileSize * 2;
            for (int i = 0, c = 0; i < bounds.Width - (Global.TileSize * 3); i += Global.TileSize, c++)
            {
                var x = (int)bounds.X + i + (2 * Global.TileSize);
                var y = (int)bounds.Bottom - (2 * Global.TileSize);
                var rect = new Rectangle(x, y, width, height);
                if (c % 2 == 0)
                {
                    RoofSprites.Add(rect, alternatingSprites[0]);
                }
                else
                {
                    RoofSprites.Add(rect, alternatingSprites[1]);
                }
            }

            // Back row
            for (int i = 0; i < bounds.Width - (Global.TileSize * 3); i += Global.TileSize)
            {
                var x = (int)bounds.X + i + (2 * Global.TileSize);
                var y = (int)bounds.Top - (2 * Global.TileSize);
                RoofSprites.Add(
                    new Rectangle(x, y, Global.TileSize, Global.TileSize),
                    SpriteLib.Town.Roof["roof-top-back"]);
            }

            // Fill
            
            width = bounds.Width - (Global.TileSize * 3);
            height = bounds.Height - (Global.TileSize * 1);

            for (int i = 0; i < width; i += Global.TileSize)
            {
                for (int j = 0; j < height; j += Global.TileSize)
                {
                    RoofSprites.Add(
                        new Rectangle(
                            (int)bounds.X + i + (2 * Global.TileSize),
                            (int)bounds.Y + j - Global.TileSize,
                            Global.TileSize,
                            Global.TileSize),
                        SpriteLib.Town.Roof["roof-top-middle"]);
                }
            }
        }

        private void BuildRight(BuildingLevel topLevel)
        {
            // Place back right
            RoofSprites.Add(
                new Rectangle(
                    (int)topLevel.ParentBuilding.BuildingWorldBounds.Right - (1 * Global.TileSize),
                    (int)topLevel.WorldPosition.Y - (2 * Global.TileSize),
                    2 * Global.TileSize,
                    2 * Global.TileSize),
                SpriteLib.Town.Roof["roof-back-right"]);

            // Place middle right
            for (int i = 0; i < topLevel.ParentBuilding.BuildingWorldBounds.Height - (Global.TileSize * 2); i += Global.TileSize)
            {
                RoofSprites.Add(
                    new Rectangle(
                        (int)topLevel.ParentBuilding.BuildingWorldBounds.Right - (1 * Global.TileSize),
                        (int)topLevel.WorldPosition.Y + i,
                        2 * Global.TileSize,
                        Global.TileSize),
                    SpriteLib.Town.Roof["roof-side-right"]);
            }

            // Place front right
            var startX = topLevel.ParentBuilding.BuildingWorldBounds.Right - (1 * Global.TileSize);
            var startY = (int)topLevel.ParentBuilding.BuildingWorldBounds.Bottom - (2 * Global.TileSize);
            var width = 2 * Global.TileSize;
            var height = 2 * Global.TileSize;
            RoofSprites.Add(
                new Rectangle(startX, startY, width, height),
                SpriteLib.Town.Roof["roof-front-right"]);
        }

        private void BuildLeft(BuildingLevel topLevel)
        {
            // Place back left
            RoofSprites.Add(
                new Rectangle(
                    (int)topLevel.WorldPosition.X,
                    (int)topLevel.WorldPosition.Y - (2 * Global.TileSize),
                    2 * Global.TileSize,
                    2 * Global.TileSize),
                SpriteLib.Town.Roof["roof-back-left"]);

            // Place middle left
            for (int i = 0; i < topLevel.ParentBuilding.BuildingWorldBounds.Height - (Global.TileSize * 2); i += Global.TileSize)
            {
                RoofSprites.Add(
                    new Rectangle(
                        (int)topLevel.WorldPosition.X,
                        (int)topLevel.WorldPosition.Y + i,
                        2 * Global.TileSize,
                        Global.TileSize),
                    SpriteLib.Town.Roof["roof-side-left"]);
            }

            // Place front left
            var startX = (int)topLevel.ParentBuilding.BuildingWorldBounds.X;
            var startY = (int)topLevel.ParentBuilding.BuildingWorldBounds.Bottom - (2 * Global.TileSize);
            var width = 2 * Global.TileSize;
            var height = 2 * Global.TileSize;
            RoofSprites.Add(
                new Rectangle(startX, startY, width, height),
                SpriteLib.Town.Roof["roof-front-left"]);
        }

        public Texture2D CombineTextures(Dictionary<Rectangle, Texture2D> roofSprites)
        {            
            // Calculate width
            var width = 0;
            foreach (var sprite in roofSprites.Keys)
            {
                width += sprite.Width;
            }

            // Calculate height
            var height = 0;
            foreach (var sprite in roofSprites.Keys)
            {
                height += sprite.Height;
            }

            if (Global.GraphicsDevice == null)
            {
                throw new InvalidOperationException("GraphicsDevice is not initialized.");
            }

            RenderTarget2D renderTarget = new RenderTarget2D(Global.GraphicsDevice, width, height);

            if (renderTarget.IsDisposed)
            {
                throw new InvalidOperationException("RenderTarget2D is disposed.");
            }

            Global.GraphicsDevice.SetRenderTarget(renderTarget);
            Global.GraphicsDevice.Clear(Color.Transparent);

            SpriteBatch spriteBatch = new SpriteBatch(Global.GraphicsDevice);
            spriteBatch.Begin();

            foreach (var obj in roofSprites)
            {
                if (obj.Value.IsDisposed)
                {
                    throw new InvalidOperationException("Texture2D in roofSprites is disposed.");
                }

                if (obj.Value == null)
                    continue;

                spriteBatch.Draw(obj.Value, new Vector2(obj.Key.X, obj.Key.Y), Color.White);
            }

            spriteBatch.End();
            Global.GraphicsDevice.SetRenderTarget(null);

            Texture2D finalTexture = renderTarget;
            renderTarget.Dispose();
            return finalTexture;
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
            foreach (var piece in RoofSprites)
            {
                Global.SpriteBatch.Draw(piece.Value, piece.Key, Color.White);
            }
        }
    }
}
