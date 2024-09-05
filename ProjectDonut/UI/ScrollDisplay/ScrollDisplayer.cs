using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using ProjectDonut.GameObjects;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World.Structures;

namespace ProjectDonut.UI.ScrollDisplay
{
    public class ScrollDisplayer : IScreenObject
    {
        private ScrollShowState state;

        private Texture2D scrollTopLeft;
        private Texture2D scrollTopRight;
        private Texture2D scrollBottom;
        private SpriteFont scrollFont;

        public int DisplayX { get; set; }
        public int DisplayY { get; set; }
        public int DisplayWidth { get; set; }
        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }

        private float _scrollDuration = 1f;
        private float _scrollTimer = 0f;

        private Vector2 textDimensions;
        private string curText = string.Empty;
        private int curBottomWidth;
        private int scale = 5;

        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphicsDevice;
        private RasterizerState rasterizerState;

        public StructureData CurrentStructureData { get; set; }

        public ScrollDisplayer(ContentManager content, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            _content = content;
            _spriteBatch = spriteBatch;
            _graphicsDevice = graphicsDevice;
        }

        public void DisplayScroll(StructureData structure)
        {
            CurrentStructureData = structure;
            //DisplayX = structure.Bounds.X + (structure.Bounds.Width / 2);
            //DisplayY = structure.Bounds.Y + 20;
            DisplayX = Global.GraphicsDeviceManager.PreferredBackBufferWidth / 2;
            DisplayY = Global.GraphicsDeviceManager.PreferredBackBufferHeight - 200;
            textDimensions = scrollFont.MeasureString(structure.Name);
            DisplayWidth = (int)textDimensions.X + 7 * scale;
            curText = structure.Name;

            _scrollTimer = 0f;

            state = ScrollShowState.Scrolling;
        }

        public void HideScroll()
        {
            state = ScrollShowState.Hidden;
            _scrollTimer = 0f;
            CurrentStructureData = null;
        }

        public void Initialize()
        {
            state = ScrollShowState.Hidden;

            rasterizerState = new RasterizerState
            {
                ScissorTestEnable = true
            };
        }

        public void LoadContent()
        {
            scrollTopLeft = _content.Load<Texture2D>("Sprites/UI/Scroll-Top-Left");
            scrollTopRight = _content.Load<Texture2D>("Sprites/UI/Scroll-Top-Right");
            scrollBottom = _content.Load<Texture2D>("Sprites/UI/Scroll-Bottom");
            scrollFont = _content.Load<SpriteFont>("Fonts/OldeEnglishDesc");
        }
        public void Update(GameTime gameTime)
        {
            switch (state)
            {
                case ScrollShowState.Hidden:
                    break;

                case ScrollShowState.Scrolling:
                    _scrollTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    curBottomWidth = (int)MathHelper.Lerp(0, DisplayWidth, _scrollTimer / _scrollDuration);

                    if (_scrollTimer >= _scrollDuration)
                    {
                        state = ScrollShowState.Showing;
                    }
                    break;

                case ScrollShowState.Showing:
                    break;
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (state == ScrollShowState.Hidden)
            {
                return;
            }

            // Calculate positions
            var startX = DisplayX - curBottomWidth / 2;
            var startY = DisplayY + 32 * scale / 2 - textDimensions.Y / 2;

            //_spriteBatch.End();

            // Draw scroll background parts
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, transformMatrix: Matrix.Identity);

            // Middle section
            int middleWidth = curBottomWidth - 7 * scale + 5;  // Adjust width of the middle section
            middleWidth = middleWidth < 0 ? 0 : middleWidth;
            for (int i = 0; i < middleWidth; i++)
            {
                _spriteBatch.Draw(scrollBottom, new Rectangle(startX + 7 * scale + i, DisplayY, 1 * scale, 32 * scale), Color.White);
            }

            _spriteBatch.End();

            // Store the original scissor rectangle
            var originalScissorRect = _graphicsDevice.ScissorRectangle;

            // Apply scissor rectangle
            _graphicsDevice.ScissorRectangle = new Rectangle(startX + 7 * scale, DisplayY, middleWidth, 32 * scale);

            // Draw text with scissor test
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, rasterizerState);
            _spriteBatch.DrawString(scrollFont, curText, new Vector2(startX + 7 * scale + 5, startY), Color.Black);
            _spriteBatch.End();

            // Restore original scissor rectangle
            _graphicsDevice.ScissorRectangle = originalScissorRect;

            // Draw scroll caps
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            _spriteBatch.Draw(scrollTopLeft, new Rectangle(startX, DisplayY, 7 * scale, 32 * scale), Color.White);
            _spriteBatch.Draw(scrollTopRight, new Rectangle(startX + 7 * scale + middleWidth, DisplayY, 7 * scale, 32 * scale), Color.White);
            _spriteBatch.End();
        }


    }
}
