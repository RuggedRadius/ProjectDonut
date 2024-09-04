using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDonut.Interfaces
{
    public interface IGameObject : IDrawable
    {
        void Initialize();
        void LoadContent();
        void Update(GameTime gameTime);
        new void Draw(GameTime gameTime);
    }
}
