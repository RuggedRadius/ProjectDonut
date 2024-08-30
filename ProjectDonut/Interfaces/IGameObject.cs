using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDonut.Interfaces
{
    public interface IGameObject
    {
        Vector2 Position { get; set; }
        int ZIndex { get; set; }

        void Initialize();
        void LoadContent(ContentManager content);
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
