using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.GameObjects.Doodads
{
    internal interface IDoodad
    {
        Rectangle Bounds { get; set; }
        Texture2D Texture { get; set; }
        Vector2 WorldPosition { get; }

        void Draw(GameTime gameTime);
    }
}
