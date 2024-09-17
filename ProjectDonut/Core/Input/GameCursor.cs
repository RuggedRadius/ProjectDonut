using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using ProjectDonut.Core;
using ProjectDonut.Core.Sprites;
using ProjectDonut.Interfaces;

namespace ProjectDonut.Core.Input
{
    public class GameCursor : IScreenObject
    {
        public Vector2 Position { get; set; }
        public Vector2 CursorWorldPosition { get; set; }
        public int ZIndex { get; set; }

        public bool IsClicked { get; set; }

        private Game1 _game;

        private Texture2D cursorDefault;

        private Vector2 hotspotOffset;
        private Vector2 scaleFactor;

        public GameCursor(Game1 game)
        {
            _game = game;
        }

        public void Initialize()
        {
            _game.IsMouseVisible = false;
        }

        public void LoadContent()
        {
            cursorDefault = SpriteLib.GetSprite("cursor");

            hotspotOffset = new Vector2(cursorDefault.Width / 2, cursorDefault.Height / 2);

            var xScale = (float)Global.GraphicsDevice.Viewport.Width / _game.Window.ClientBounds.Width;
            var yScale = (float)Global.GraphicsDevice.Viewport.Height / _game.Window.ClientBounds.Height;
            scaleFactor = new Vector2(xScale, yScale);
        }

        public void Update(GameTime gameTime)
        {
            Position = Vector2.Transform(Mouse.GetState().Position.ToVector2(), Matrix.Invert(Matrix.Identity));

            CursorWorldPosition = Global.Camera.OrthoCamera.ScreenToWorld(
                new Vector2(
                    InputManager.MouseState.X,
                    InputManager.MouseState.Y
                    ));
        }

        public void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Begin(transformMatrix: Matrix.Identity);
            Global.SpriteBatch.Draw(cursorDefault, Position - hotspotOffset, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            Global.SpriteBatch.End();
        }
    }
}
