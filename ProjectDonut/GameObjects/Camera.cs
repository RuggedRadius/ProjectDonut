using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using ProjectDonut.GameObjects;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Interfaces;
using Microsoft.Xna.Framework.Content;
using ProjectDonut.Core;

namespace ProjectDonut.GameObjects
{
    public class Camera : IGameObject
    {
        public Vector2 Position { get; set; }
        public float Zoom { get; set; }
        public float ZoomMax = 0.15f;
        public float ZoomMin = 6f;
        public float Rotation { get; set; }
        public int ZIndex { get; set; }

        // States
        private MouseState _previousMouseState;


        public Camera()
        {
            Position = Vector2.Zero;
            Zoom = 1f;
            Rotation = 0f;
        }

        public Matrix GetTransformationMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-Position, 0)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                   Matrix.CreateTranslation(new Vector3(Global.GraphicsDevice.Viewport.Width * 0.5f, Global.GraphicsDevice.Viewport.Height * 0.5f, 0));
        }

        public Matrix GetViewMatrix()
        {
            // Translation moves the camera to its position
            Matrix translationMatrix = Matrix.CreateTranslation(-Position.X, -Position.Y, 0);

            // Rotation applies any rotation to the camera view
            Matrix rotationMatrix = Matrix.CreateRotationZ(Rotation);

            // Scale applies zoom to the camera view
            Matrix scaleMatrix = Matrix.CreateScale(Zoom, Zoom, 1);

            // Combine the transformations
            Matrix transform = translationMatrix * rotationMatrix * scaleMatrix;

            // Translate back by half the viewport to keep the camera centered
            Matrix centerMatrix = Matrix.CreateTranslation(Global.GraphicsDevice.Viewport.Width / 2f, Global.GraphicsDevice.Viewport.Height / 2f, 0);

            return transform * centerMatrix;
        }


        public void Update(GameTime gameTime)
        {
            Position = Global.Player.Position;

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

        public void Initialize()
        {
            //throw new System.NotImplementedException();
        }

        public void LoadContent(ContentManager content)
        {
            //throw new System.NotImplementedException();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //throw new System.NotImplementedException();
        }
    }
}
