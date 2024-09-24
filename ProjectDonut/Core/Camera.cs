using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using IGameComponent = ProjectDonut.Interfaces.IGameComponent;
using ProjectDonut.Debugging;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended;
using ProjectDonut.Core.Input;

namespace ProjectDonut.Core
{
    public class Camera : IGameComponent
    {
        private Game1 _game;
        public OrthographicCamera OrthoCamera;
        public RenderTarget2D RenderTarget;
        public Texture2D MinimapTexture;

        public bool IsMinimap = false;

        public Camera(Game1 game, bool isMinimap)
        {
            _game = game;
            IsMinimap = isMinimap;
        }

        public Matrix GetTransformationMatrix()
        {
            return OrthoCamera.GetViewMatrix();
        }

        public void Initialize()
        {
            var viewportAdapter = new BoxingViewportAdapter(_game.Window, Global.GraphicsDevice, 800, 480);
            OrthoCamera = new OrthographicCamera(viewportAdapter);
            OrthoCamera.MaximumZoom = 4;
            OrthoCamera.MinimumZoom = 0.1f;

            if (IsMinimap)
            {
                OrthoCamera.Zoom = 0.5f;
                RenderTarget = new RenderTarget2D(Global.GraphicsDevice, 200, 200);
            }
        }

        public void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            if (IsMinimap)
            {
                OrthoCamera.LookAt(Global.PlayerObj.WorldPosition);
            }
            else
            {
                float zoomPerTick = 0.01f;
                if (InputManager.KeyboardState.IsKeyDown(Keys.X))
                {
                    OrthoCamera.ZoomIn(zoomPerTick);
                }
                if (InputManager.KeyboardState.IsKeyDown(Keys.Z))
                {
                    OrthoCamera.ZoomOut(zoomPerTick);
                }

                if (InputManager.ScrollWheelDelta > 0)
                {
                    OrthoCamera.ZoomIn(zoomPerTick * 10);
                }
                if (InputManager.ScrollWheelDelta < 0)
                {
                    OrthoCamera.ZoomOut(zoomPerTick * 10);
                }

                var viewport = OrthoCamera.BoundingRectangle;
                OrthoCamera.LookAt(Global.PlayerObj.WorldPosition);

                DebugWindow.Lines[6] = $"Camera Position: {OrthoCamera.Position:N0}";
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Global.SpriteBatch.Begin(transformMatrix: OrthoCamera.GetViewMatrix());
            Global.SpriteBatch.DrawRectangle(new RectangleF(250, 250, 50, 50), Color.Black, 1f);
            Global.SpriteBatch.End();
        }
    }
}
