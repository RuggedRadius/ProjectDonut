using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Interfaces;

namespace ProjectDonut.NPCs.Enemy
{
    public class Enemy : IGameObject
    {
        public EnemyState State { get; set; }
        public Vector2 WorldPosition { get; set; }
        public Rectangle Bounds { get; set; }
        public int ZIndex { get; set; }
        public bool IsVisible { get; set; }

        public Texture2D Texture { get; set; }
        public Vector2 _textureOrigin => new Vector2(Texture.Width / 2f, Texture.Height / 2f);


        public virtual void Initialize()
        {
        }

        public virtual void LoadContent()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            Bounds = new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, Texture.Width, Texture.Height);
            UpdateObjectVisibility();
        }

        public virtual void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Draw(Texture, WorldPosition, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        public void UpdateObjectVisibility()
        {
            float distance = Vector2.Distance(Global.PlayerObj.WorldPosition, WorldPosition);
            IsVisible = (distance <= Global.FOG_OF_WAR_RADIUS) ? true : false;
        }
    }
}
