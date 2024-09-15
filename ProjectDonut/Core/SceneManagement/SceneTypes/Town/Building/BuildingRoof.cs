using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Interfaces;

namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town.Building
{
    public class BuildingRoof : IGameObject
    {
        public Dictionary<Rectangle, Texture2D> RoofSprites { get; set; }

        public bool IsVisible => throw new NotImplementedException();
        public Texture2D Texture { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Vector2 WorldPosition { get; set; }
        public int ZIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void BuildRoof(BuildingLevel topLevel)
        {
            RoofSprites = new Dictionary<Rectangle, Texture2D>();

            BuildLeft(topLevel);
            BuildRight(topLevel);
            BuildMiddle(topLevel);
        }

        private void BuildMiddle(BuildingLevel topLevel)
        {
            for (int i = 0; i < topLevel.ParentBuilding.BuildingWorldBounds.Width - (Global.TileSize * 4); i += (2 * Global.TileSize))
            {
                RoofSprites.Add(
                    new Rectangle(
                        (int)topLevel.ParentBuilding.BuildingWorldBounds.X + i + (2 * Global.TileSize),
                        (int)topLevel.ParentBuilding.BuildingWorldBounds.Bottom - (2 * Global.TileSize),
                        Global.TileSize,
                        2 * Global.TileSize),
                    Global.SpriteLibrary.BuildingBlockSprites["roof-top-front"]);

                RoofSprites.Add(
                    new Rectangle(
                        (int)topLevel.ParentBuilding.BuildingWorldBounds.X + i + (3 * Global.TileSize),
                        (int)topLevel.ParentBuilding.BuildingWorldBounds.Bottom - (2 * Global.TileSize),
                        Global.TileSize,
                        2 * Global.TileSize),
                    Global.SpriteLibrary.BuildingBlockSprites["roof-top-front2"]);
            }

            //// Place front right
            //var startX = topLevel.ParentBuilding.BuildingWorldBounds.Right - (1 * Global.TileSize);
            //var startY = (int)topLevel.ParentBuilding.BuildingWorldBounds.Bottom - (2 * Global.TileSize);
            //var width = 2 * Global.TileSize;
            //var height = 2 * Global.TileSize;
            //RoofSprites.Add(
            //    new Rectangle(startX, startY, width, height),
            //    Global.SpriteLibrary.BuildingBlockSprites["roof-front-right"]);
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
                Global.SpriteLibrary.BuildingBlockSprites["roof-back-right"]);

            // Place middle right
            for (int i = 0; i < topLevel.ParentBuilding.BuildingWorldBounds.Height - (Global.TileSize * 2); i += Global.TileSize)
            {
                RoofSprites.Add(
                    new Rectangle(
                        (int)topLevel.ParentBuilding.BuildingWorldBounds.Right - (1 * Global.TileSize),
                        (int)topLevel.WorldPosition.Y + i,
                        2 * Global.TileSize,
                        Global.TileSize),
                    Global.SpriteLibrary.BuildingBlockSprites["roof-side-right"]);
            }

            // Place front right
            var startX = topLevel.ParentBuilding.BuildingWorldBounds.Right - (1 * Global.TileSize);
            var startY = (int)topLevel.ParentBuilding.BuildingWorldBounds.Bottom - (2 * Global.TileSize);
            var width = 2 * Global.TileSize;
            var height = 2 * Global.TileSize;
            RoofSprites.Add(
                new Rectangle(startX, startY, width, height),
                Global.SpriteLibrary.BuildingBlockSprites["roof-front-right"]);
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
                Global.SpriteLibrary.BuildingBlockSprites["roof-back-left"]);

            // Place middle left
            for (int i = 0; i < topLevel.ParentBuilding.BuildingWorldBounds.Height - (Global.TileSize * 2); i += Global.TileSize)
            {
                RoofSprites.Add(
                    new Rectangle(
                        (int)topLevel.WorldPosition.X,
                        (int)topLevel.WorldPosition.Y + i,
                        2 * Global.TileSize,
                        Global.TileSize),
                    Global.SpriteLibrary.BuildingBlockSprites["roof-side-left"]);
            }

            // Place front left
            var startX = (int)topLevel.ParentBuilding.BuildingWorldBounds.X;
            var startY = (int)topLevel.ParentBuilding.BuildingWorldBounds.Bottom - (2 * Global.TileSize);
            var width = 2 * Global.TileSize;
            var height = 2 * Global.TileSize;
            RoofSprites.Add(
                new Rectangle(startX, startY, width, height),
                Global.SpriteLibrary.BuildingBlockSprites["roof-front-left"]);
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
