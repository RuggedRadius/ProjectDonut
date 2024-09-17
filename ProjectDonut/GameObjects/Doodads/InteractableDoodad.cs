using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Core.Input;
using ProjectDonut.GameObjects.PlayerComponents;
using System;

namespace ProjectDonut.GameObjects.Doodads
{
    public class InteractableDoodad : IInteractable, IDoodad
    {
        public Rectangle Bounds { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 WorldPosition => new Vector2(Bounds.X, Bounds.Y);

        public InteractableState State { get; set; }
        public Rectangle InteractBounds { get; set; }

        internal SpriteSheet _spriteSheet;
        internal AnimationController _animControllerHit;
        internal AnimatedSprite _sprite;

        public InteractableDoodad(Rectangle bounds)
        {
            Bounds = bounds;
        }        

        public virtual void Interact()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        
        }

        public virtual void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Draw(_sprite, WorldPosition, 0.0f, Vector2.One);
            Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, InteractBounds, Color.Blue * 0.1f);
        }
    }
}
