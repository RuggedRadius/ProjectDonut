using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsepriteDotNet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Interfaces;

namespace ProjectDonut.NPCs.Enemy
{
    public class Enemy : IGameObject
    {
        public EnemyState State { get; set; }
        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }

        public Texture2D _sprite;
        public Vector2 _textureOrigin => new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);


        public virtual void Initialize()
        {
        }

        public virtual void LoadContent(ContentManager content)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //spriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            spriteBatch.Draw(_sprite, Position, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            //spriteBatch.End();
        }
    }
}
