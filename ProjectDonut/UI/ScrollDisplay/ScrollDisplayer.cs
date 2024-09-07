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
        public ScrollShowState ScrollState;

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
        public static string ScrollText = string.Empty;
        private int curBottomWidth;
        private int scale = 5;

        private RasterizerState rasterizerState;

        public static WorldStructure CurrentStructure { get; set; }

        public void DisplayScroll()
        {
            DisplayX = Global.GraphicsDeviceManager.PreferredBackBufferWidth / 2;
            DisplayY = Global.GraphicsDeviceManager.PreferredBackBufferHeight - 200;            
            
            ScrollText = CurrentStructure?.StructureName;
            textDimensions = scrollFont.MeasureString(ScrollText);
            DisplayWidth = (int)textDimensions.X + 7 * scale;

            _scrollTimer = 0f;

            ScrollState = ScrollShowState.Scrolling;
        }

        public void HideScroll()
        {
            ScrollState = ScrollShowState.Hidden;
            _scrollTimer = 0f;
            CurrentStructure = null;
        }

        public void Initialize()
        {
            ScrollState = ScrollShowState.Hidden;

            rasterizerState = new RasterizerState
            {
                ScissorTestEnable = true
            };
        }

        public void LoadContent()
        {
            scrollTopLeft = Global.ContentManager.Load<Texture2D>("Sprites/UI/Scroll-Top-Left");
            scrollTopRight = Global.ContentManager.Load<Texture2D>("Sprites/UI/Scroll-Top-Right");
            scrollBottom = Global.ContentManager.Load<Texture2D>("Sprites/UI/Scroll-Bottom");
            scrollFont = Global.ContentManager.Load<SpriteFont>("Fonts/OldeEnglishDesc");
        }
        public void Update(GameTime gameTime)
        {
            if (CurrentStructure == null)
            {
                HideScroll();
                return;
            }

            switch (ScrollState)
            {
                case ScrollShowState.Hidden:
                    if (CurrentStructure != null)
                    {
                        ScrollText = CurrentStructure.StructureName;
                        DisplayScroll();
                    }
                    break;

                case ScrollShowState.Scrolling:
                    _scrollTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    curBottomWidth = (int)MathHelper.Lerp(0, DisplayWidth, _scrollTimer / _scrollDuration);

                    if (_scrollTimer >= _scrollDuration)
                    {
                        ScrollState = ScrollShowState.Showing;
                    }
                    break;

                case ScrollShowState.Showing:
                    _scrollTimer = 0f;
                    break;
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (ScrollState == ScrollShowState.Hidden)
            {
                return;
            }

            // Calculate positions
            var startX = DisplayX - curBottomWidth / 2;
            var startY = DisplayY + 32 * scale / 2 - textDimensions.Y / 2;

            //Global.SpriteBatch.End();

            // Draw scroll background parts
            //Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, transformMatrix: Matrix.Identity);
            Global.SpriteBatch.Begin(transformMatrix: Matrix.Identity);

            // Middle section
            int middleWidth = curBottomWidth - 7 * scale + 5;  // Adjust width of the middle section
            middleWidth = middleWidth < 0 ? 0 : middleWidth;
            for (int i = 0; i < middleWidth; i++)
            {
                Global.SpriteBatch.Draw(scrollBottom, new Rectangle(startX + 7 * scale + i, DisplayY, 1 * scale, 32 * scale), Color.White);
            }

            Global.SpriteBatch.End();

            // Store the original scissor rectangle
            var originalScissorRect = Global.GraphicsDevice.ScissorRectangle;

            // Apply scissor rectangle
            Global.GraphicsDevice.ScissorRectangle = new Rectangle(startX + 7 * scale, DisplayY, middleWidth, 32 * scale);

            // Draw text with scissor test
            //Global.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, rasterizerState);
            Global.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, rasterizerState);
            Global.SpriteBatch.DrawString(scrollFont, ScrollText, new Vector2(startX + 7 * scale + 5, startY), Color.Black);
            Global.SpriteBatch.End();

            // Restore original scissor rectangle
            Global.GraphicsDevice.ScissorRectangle = originalScissorRect;

            // Draw scroll caps
            Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            Global.SpriteBatch.Draw(scrollTopLeft, new Rectangle(startX, DisplayY, 7 * scale, 32 * scale), Color.White);
            Global.SpriteBatch.Draw(scrollTopRight, new Rectangle(startX + 7 * scale + middleWidth, DisplayY, 7 * scale, 32 * scale), Color.White);
            Global.SpriteBatch.End();
        }


    }
}
