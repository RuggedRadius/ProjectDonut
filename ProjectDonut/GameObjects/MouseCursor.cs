using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.ProceduralGeneration;

namespace ProjectDonut.GameObjects
{
    public class MouseCursor : GameObject
    {
        public Vector2 Position { get; set; }

        private Game1 _game;
        private SpriteBatch _spriteBatch;
        private SpriteLibrary _spriteLib;
        private GraphicsDevice _graphicsDevice;
        private Camera _camera;

        private Texture2D cursorDefault;

        private Vector2 hotspotOffset;
        private Vector2 scaleFactor;

        public MouseCursor(Game1 game, SpriteLibrary spriteLib, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Camera camera)
        {
            this._spriteLib = spriteLib;
            this._spriteBatch = spriteBatch;
            this._game = game;
            this._graphicsDevice = graphicsDevice;
            this._camera = camera;
        }

        public override void Initialize()
        {
            _game.IsMouseVisible = false;
        }

        public override void LoadContent()
        {
            cursorDefault = _spriteLib.GetSprite("cursor");

            hotspotOffset = new Vector2(cursorDefault.Width / 2, cursorDefault.Height / 2);

            var xScale = (float)_graphicsDevice.Viewport.Width / _game.Window.ClientBounds.Width;
            var yScale = (float)_graphicsDevice.Viewport.Height / _game.Window.ClientBounds.Height;
            scaleFactor = new Vector2(xScale, yScale);
        }

        public override void Update(GameTime gameTime)
        {
            //Position = Mouse.GetState().Position.ToVector2() * scaleFactor;
            Position = Vector2.Transform(Mouse.GetState().Position.ToVector2(), Matrix.Invert(_camera.GetTransformationMatrix(_graphicsDevice, _graphicsDevice.Viewport)));
        }

        public override void Draw(GameTime gameTime)
        {
            //_spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Matrix.Identity);

            //// Draw the cursor at the mouse position, unaffected by camera transformations
            //_spriteBatch.Draw(cursorDefault, Position - hotspotOffset, Color.White);

            //_spriteBatch.End();

            _spriteBatch.Draw(cursorDefault, Position - hotspotOffset, Color.White);
        }
    }
}
