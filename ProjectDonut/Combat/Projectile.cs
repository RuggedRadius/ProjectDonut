using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Combat.Combatants;
using ProjectDonut.Core;
using ProjectDonut.Core.SceneManagement.SceneTypes;

namespace ProjectDonut.Combat
{
    public class Projectile
    {
        public Texture2D Texture { get; set; }
        public float Speed { get; set; }
        public Vector2 ScreenPosition { get; set; }
        public Vector2 ScreenDestination { get; set; }
        public Vector2 Direction { get; set; }
        public Combatant Target { get; set; }

        private bool IsFlipped { get; set; }


        public Projectile(Texture2D texture, float speed, Vector2 screenPosition, Combatant target)
        {
            Texture = texture;
            Speed = speed;
            ScreenPosition = screenPosition;
            ScreenDestination = target.ScreenPosition;
            Direction = Vector2.Normalize(ScreenDestination - ScreenPosition);
            IsFlipped = target._spriteEffects == SpriteEffects.FlipHorizontally;
            Target = target;
        }

        public void Update(GameTime gameTime)
        {
            ScreenPosition += Direction * Speed;
        }

        public void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Draw(
                texture: Texture,
                position: ScreenPosition,
                sourceRectangle: null,
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: Vector2.One * CombatScene.SceneScale,
                effects: IsFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth: 0f
            );
        }

        public bool HasReachedDestination()
        {
            if (!IsFlipped)
            {
                if (ScreenPosition.X < ScreenDestination.X)
                {
                    return true;
                }
            }

            if (IsFlipped)
            {
                if (ScreenPosition.X > ScreenDestination.X)
                {
                    return true;
                }
            }


            var xDiff = Math.Abs(ScreenPosition.X - ScreenDestination.X);
            var yDiff = Math.Abs(ScreenPosition.Y - ScreenDestination.Y);

            if (xDiff < 1 && yDiff < 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
