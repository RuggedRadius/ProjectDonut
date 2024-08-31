using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;

namespace ProjectDonut.GameObjects
{
    public class GameCursor : IScreenObject
    {
        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }

        public bool IsClicked { get; set; }

        private Game1 _game;

        private Texture2D cursorDefault;

        private Vector2 hotspotOffset;
        private Vector2 scaleFactor;

        public GameCursor(Game1 game)
        {
            this._game = game;
        }

        public void Initialize()
        {
            _game.IsMouseVisible = false;
        }

        public void LoadContent()
        {
            cursorDefault = Global.SpriteLibrary.GetSprite("cursor");

            hotspotOffset = new Vector2(cursorDefault.Width / 2, cursorDefault.Height / 2);

            var xScale = (float)Global.GraphicsDevice.Viewport.Width / _game.Window.ClientBounds.Width;
            var yScale = (float)Global.GraphicsDevice.Viewport.Height / _game.Window.ClientBounds.Height;
            scaleFactor = new Vector2(xScale, yScale);
        }

        public void Update(GameTime gameTime)
        {
            Position = Vector2.Transform(Mouse.GetState().Position.ToVector2(), Matrix.Invert(Matrix.Identity));
        }

        public void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Begin(transformMatrix: Matrix.Identity);
            Global.SpriteBatch.Draw(cursorDefault, Position - hotspotOffset, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            Global.SpriteBatch.End();
        }
    }
}
