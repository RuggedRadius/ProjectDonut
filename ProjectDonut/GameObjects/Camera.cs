using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using ProjectGorilla.GameObjects;
using Microsoft.Xna.Framework.Input;

namespace ProjectDonut.GameObjects
{
    public class Camera : GameObject
    {
        public Vector2 Position { get; set; }
        public float Zoom { get; set; }
        public float ZoomMax = 0.025f;
        public float ZoomMin = 4f;
        public float Rotation { get; set; }

        // States
        private MouseState _previousMouseState;

        public Camera()
        {
            Position = Vector2.Zero;
            Zoom = 1f;
            Rotation = 0f;
        }

        public Matrix GetTransformationMatrix(GraphicsDevice graphicsDevice, Viewport viewport)
        {
            return Matrix.CreateTranslation(new Vector3(-Position, 0)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                   Matrix.CreateTranslation(new Vector3(viewport.Width * 0.5f, viewport.Height * 0.5f, 0));
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Z))
            {
                Zoom -= 0.025f;
            }
            if (keyboardState.IsKeyDown(Keys.X))
            {
                Zoom += 0.025f;
            }

            Zoom = MathHelper.Clamp(Zoom, ZoomMax, ZoomMin);

            base.Update(gameTime);
        }

        private void HandleMouseZoom()
        {
            MouseState mouseState = Mouse.GetState();

            int scrollDelta = mouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue;

            if (scrollDelta != 0)
            {
                if (scrollDelta > 0)
                {
                    Zoom++;
                }
                else
                {
                    Zoom--;
                }

                scrollDelta = (int)MathHelper.Clamp(scrollDelta, -1f, 1f);
                Zoom += scrollDelta;

                if (Zoom < 2)
                {
                    Zoom = 2;
                }
                else if (Zoom > 10)
                {
                    Zoom = 50;
                }
            }

            _previousMouseState = mouseState;
        }
    }
}
