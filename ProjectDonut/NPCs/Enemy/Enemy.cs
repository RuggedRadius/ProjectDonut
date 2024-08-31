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
        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }

        private Texture2D _sprite;
        private Vector2 _textureOrigin => new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);


        public void Initialize()
        {
        }

        public void LoadContent(ContentManager content)
        {
            _sprite = Global.ContentManager.Load<Texture2D>("Sprites/Enemy/Test_Grunt");
        }

        public void Update(GameTime gameTime)
        {
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //spriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            spriteBatch.Draw(_sprite, Position, null, Color.White, 0, _textureOrigin, 1f, SpriteEffects.None, 0);
            //spriteBatch.End();
        }
    }
}
