using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDonut.Interfaces
{
    public interface IDrawable
    {
        Texture2D Texture { get; set; }
        Vector2 WorldPosition { get; } //set;
        int ZIndex { get; set; }

        void Draw(GameTime gameTime);
    }
}
