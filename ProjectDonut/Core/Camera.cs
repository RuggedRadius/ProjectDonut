using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using IGameComponent = ProjectDonut.Interfaces.IGameComponent;
using ProjectDonut.Debugging;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended;

namespace ProjectDonut.Core
{
    public class Camera : IGameComponent
    {
        private Game1 _game;
        public OrthographicCamera OrthoCamera;

        public Camera(Game1 game)
        {
            _game = game;
        }

        public Matrix GetTransformationMatrix()
        {
            return OrthoCamera.GetViewMatrix();
        }

        public void Initialize()
        {
            var viewportAdapter = new BoxingViewportAdapter(_game.Window, Global.GraphicsDevice, 800, 480);
            OrthoCamera = new OrthographicCamera(viewportAdapter);
        }

        public void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            float zoomPerTick = 0.01f;
            if (state.IsKeyDown(Keys.X))
            {
                OrthoCamera.ZoomIn(zoomPerTick);
            }
            if (state.IsKeyDown(Keys.Z))
            {
                OrthoCamera.ZoomOut(zoomPerTick);
            }

            var viewport = OrthoCamera.BoundingRectangle;
            OrthoCamera.LookAt(Global.Player.WorldPosition);

            DebugWindow.Lines[6] = $"Camera Position: {OrthoCamera.Position:N0}";
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var transformMatrix = OrthoCamera.GetViewMatrix();
            Global.SpriteBatch.Begin(transformMatrix: transformMatrix);
            Global.SpriteBatch.DrawRectangle(new RectangleF(250, 250, 50, 50), Color.Black, 1f);
            Global.SpriteBatch.End();
        }
    }
}
