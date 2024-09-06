using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Interfaces;
using Microsoft.Xna.Framework.Content;
using IGameComponent = ProjectDonut.Interfaces.IGameComponent;
using ProjectDonut.Debugging;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended;
using SadConsole.UI;

namespace ProjectDonut.Core
{
    public class Camera : IGameComponent
    {
        private Game1 _game;
        private OrthographicCamera _camera;

        public Camera(Game1 game)
        {
            _game = game;
        }

        public Matrix GetTransformationMatrix()
        {
            return _camera.GetViewMatrix();
        }

        public void Initialize()
        {
            var viewportAdapter = new BoxingViewportAdapter(_game.Window, Global.GraphicsDevice, 800, 480);
            _camera = new OrthographicCamera(viewportAdapter);
        }

        public void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            float zoomPerTick = 0.01f;
            if (state.IsKeyDown(Keys.Z))
            {
                _camera.ZoomIn(zoomPerTick);
            }
            if (state.IsKeyDown(Keys.X))
            {
                _camera.ZoomOut(zoomPerTick);
            }

            var viewport = _camera.BoundingRectangle;
            _camera.LookAt(Global.Player.Position);

            DebugWindow.Lines[6] = $"Camera Position: {_camera.Position:N0}";
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var transformMatrix = _camera.GetViewMatrix();
            Global.SpriteBatch.Begin(transformMatrix: transformMatrix);
            Global.SpriteBatch.DrawRectangle(new RectangleF(250, 250, 50, 50), Color.Black, 1f);
            Global.SpriteBatch.End();
        }
    }
}
