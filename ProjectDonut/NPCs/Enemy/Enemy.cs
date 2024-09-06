using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsepriteDotNet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Interfaces;
using ProjectDonut.Pathfinding;

namespace ProjectDonut.NPCs.Enemy
{
    public class Enemy : IGameObject
    {
        public EnemyState State { get; set; }
        public Vector2 Position { get; set; }
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
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            UpdateObjectVisibility();
        }

        public virtual void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        public void UpdateObjectVisibility()
        {
            float distance = Vector2.Distance(Global.Player.Position, Position);
            IsVisible = (distance <= Global.FOG_OF_WAR_RADIUS) ? true : false;
        }
    }
}
