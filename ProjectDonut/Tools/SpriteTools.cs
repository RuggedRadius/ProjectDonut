using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Tools
{
    public static class SpriteTools
    {
        public static Texture2D ExtractSprite(Texture2D spriteSheet, int x, int y)
        {
            var width = Global.TileSize;
            var height = Global.TileSize;

            x *= width;
            y *= height;

            Rectangle sourceRectangle = new Rectangle(x, y, width, height);

            // Extract the pixel data from the spritesheet
            Color[] data = new Color[width * height];
            spriteSheet.GetData(0, sourceRectangle, data, 0, data.Length);

            // Create a new texture for the sprite and set the pixel data
            Texture2D sprite = new Texture2D(Global.GraphicsDevice, width, height);
            sprite.SetData(data);

            return sprite;
        }
    }
}
