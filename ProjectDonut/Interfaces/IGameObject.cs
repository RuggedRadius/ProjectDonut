using Microsoft.Xna.Framework;

namespace ProjectDonut.Interfaces
{
    public interface IGameObject
    {
        Vector2 Position { get; set; }
        int ZIndex { get; set; }

        void Initialize();
        void LoadContent();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
