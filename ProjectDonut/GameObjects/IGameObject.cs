using Microsoft.Xna.Framework;

namespace ProjectDonut.GameObjects
{
    public interface IGameObject
    {
        void Initialize();
        void LoadContent();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
