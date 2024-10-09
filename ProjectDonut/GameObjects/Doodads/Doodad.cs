using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDonut.GameObjects.Doodads
{
    public class Doodad : IDoodad
    {
        public Rectangle Bounds { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 WorldPosition => new Vector2(Bounds.X, Bounds.Y);

        public Doodad(Rectangle bounds, Texture2D texture)
        {
            Bounds = bounds;
            Texture = texture;
        }

        public void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Draw(Texture, Bounds, Color.White);
        }
    }
}
