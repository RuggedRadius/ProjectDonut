using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace ProjectDonut.GameObjects.Doodads
{
    public class InteractableDoodad : IInteractable, IDoodad
    {
        public Rectangle Bounds { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 WorldPosition => new Vector2(Bounds.X, Bounds.Y);

        public InteractableState State { get; set; }
        public Rectangle InteractBounds { get; set; }

        public bool PlayerInRange { get; set; }

        internal SpriteSheet _spriteSheet;
        public AnimatedSprite _sprite;

        internal float _interactTimer = 0f;
        internal float _interactCooldown = 0.5f;

        public InteractableDoodad(Rectangle bounds)
        {
            Bounds = bounds;
        }        

        public virtual void Interact()
        {
            _interactTimer = 0f;
        }

        public virtual void Update(GameTime gameTime)
        {
            _interactTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            PlayerInRange = InteractBounds.Contains(Global.PlayerObj.WorldPosition);
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (!IsInCameraView())
            {
                return;
            }

            Global.SpriteBatch.Draw(_sprite, WorldPosition, 0.0f, Vector2.One);
            //Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, InteractBounds, Color.Blue * 0.1f);
        }

        private bool IsInCameraView()
        {
            return Global.Camera.OrthoCamera.BoundingRectangle.Intersects(Bounds);
        }
    }
}
