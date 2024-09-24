using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using ProjectDonut.Interfaces;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace ProjectDonut.Core
{
    public class CameraMinimap : IScreenObject
    {
        private Game1 _game;
        public OrthographicCamera OrthoCamera;
        public RenderTarget2D RenderTarget;
        public Texture2D MinimapTexture;

        public bool IsMinimap = false;
        public int ZIndex { get; set; }

        private Rectangle DisplayRect { get; set; }

        private Random _random = new Random();

        public CameraMinimap(Game1 game, bool isMinimap)
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

            OrthoCamera.Zoom = 0.1f;

            var sizeX = 400;
            var sizeY = 300;
            var paddingFromEdgeOfScreen = 10;
            RenderTarget = new RenderTarget2D(Global.GraphicsDevice, sizeX, sizeY);            
            DisplayRect = new Rectangle(
                Global.GraphicsDeviceManager.PreferredBackBufferWidth - sizeX - paddingFromEdgeOfScreen,
                paddingFromEdgeOfScreen,
                sizeX,
                sizeY);
        }

        public void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            OrthoCamera.LookAt(Global.PlayerObj.WorldPosition);
        }

        public void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Begin();
            Global.SpriteBatch.Draw(RenderTarget, DisplayRect, Color.White);
            Global.SpriteBatch.End();
        }
    }
}
