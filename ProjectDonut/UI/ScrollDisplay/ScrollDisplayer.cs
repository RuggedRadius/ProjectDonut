using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Core.Sprites;
using ProjectDonut.Interfaces;

namespace ProjectDonut.UI.ScrollDisplay
{
    public enum ScrollShowState
    {
        Scrolling,
        Showing,
        Completed
    }

    public class ScrollDisplay
    {
        public string Text { get; set; }
        public string SubText { get; set; }
        public ScrollShowState State { get; set; }

        public bool IsTimed { get; set; }
        public float ShowDuration { get; set; }
        public float ScrollOutDuration { get; set; }

        public float ShowTimer { get; set; }
        public float ScrollOutTimer { get; set; }

        public Vector2? ScreenPosition { get; set; }
        public Vector2 TextDimensions { get; set; }
        public Vector2 SubTextDimensions { get; set; }
        public int DisplayWidth { get; set; }
    }

    public class ScrollDisplayer : IScreenObject
    {
        public List<ScrollDisplay> Scrolls = new List<ScrollDisplay>();

        private Texture2D scrollTopLeft;
        private Texture2D scrollTopRight;
        private Texture2D scrollBottom;

        private SpriteFont _fontText;
        private SpriteFont _fontSubText;

        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }
                
        private int curBottomWidth;
        private int scale = 5;

        private RasterizerState rasterizerState;

        public void DisplayScroll(ScrollDisplay scroll)
        {
            if (scroll.ShowDuration <= 0f)
                throw new ArgumentOutOfRangeException("Show duration for scroll cannot be 0 or less");

            Scrolls.Clear();

            if (scroll.SubText == null)
                scroll.SubText = string.Empty;

            if (scroll.ScreenPosition == null)
            {
                scroll.ScreenPosition = new Vector2(
                    Global.GraphicsDeviceManager.PreferredBackBufferWidth / 2,
                    Global.GraphicsDeviceManager.PreferredBackBufferHeight - 200);
            }

            scroll.TextDimensions = _fontText.MeasureString(scroll.Text);
            scroll.SubTextDimensions = _fontText.MeasureString(scroll.SubText);

            if (scroll.TextDimensions.X > scroll.SubTextDimensions.X)
                scroll.DisplayWidth = (int)scroll.TextDimensions.X + (scrollTopLeft.Width * scale * 2);
            else
                scroll.DisplayWidth = (int)scroll.SubTextDimensions.X + (scrollTopLeft.Width * scale * 2);

            scroll.ShowTimer = 0f;
            scroll.State = ScrollShowState.Scrolling;

            Scrolls.Add(scroll);
        }

        public void HideScroll(ScrollDisplay scroll)
        {
            Scrolls.Remove(scroll);
        }

        public void ClearAllScrolls()
        {
            Scrolls.Clear();
        }

        public void Initialize()
        {
            rasterizerState = new RasterizerState
            {
                ScissorTestEnable = true
            };
        }

        public void LoadContent()
        {
            scrollTopLeft = SpriteLib.UI.Scroll["scroll-left"];
            scrollTopRight = SpriteLib.UI.Scroll["scroll-right"];
            scrollBottom = SpriteLib.UI.Scroll["scroll-middle"];

            _fontText = Global.ContentManager.Load<SpriteFont>("Fonts/OldeEnglishDesc");
            _fontSubText = Global.ContentManager.Load<SpriteFont>("Fonts/OldeEnglishDescSubText");
        }
        public void Update(GameTime gameTime)
        {
            foreach (var scroll in Scrolls)
            {
                switch (scroll.State)
                {
                    case ScrollShowState.Scrolling:
                        scroll.ScrollOutTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                        curBottomWidth = (int)MathHelper.Lerp(0, scroll.DisplayWidth, scroll.ScrollOutTimer / scroll.ScrollOutDuration);

                        if (scroll.ScrollOutTimer >= scroll.ScrollOutDuration)
                        {
                            scroll.State = ScrollShowState.Showing;
                        }
                        break;

                    case ScrollShowState.Showing:
                        if (scroll.IsTimed == false)
                            break;

                        scroll.ShowTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (scroll.ShowTimer >= scroll.ShowDuration)
                            scroll.State = ScrollShowState.Completed;

                        break;
                }
            }

            var completedScrolls = Scrolls.Where(s => s.State == ScrollShowState.Completed).ToList();
            foreach (var scroll in completedScrolls)
            {
                Scrolls.Remove(scroll);
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var scroll in Scrolls)
            {
                // Calculate positions
                var startX = (int)(scroll.ScreenPosition.Value.X - curBottomWidth / 2);
                var startY = (int)(scroll.ScreenPosition.Value.Y + Global.TileSize * scale / 2 - scroll.TextDimensions.Y / 2);

                // Draw scroll background parts
                Global.SpriteBatch.Begin(transformMatrix: Matrix.Identity);

                // Middle section
                int middleWidth = curBottomWidth - 7 * scale + 5;  // Adjust width of the middle section
                middleWidth = middleWidth < 0 ? 0 : middleWidth;
                for (int i = 0; i < middleWidth; i++)
                {
                    Global.SpriteBatch.Draw(scrollBottom, new Rectangle(startX + 7 * scale + i, (int)scroll.ScreenPosition.Value.Y, 1 * scale, Global.TileSize * scale), Color.White);
                }

                Global.SpriteBatch.End();

                // Store the original scissor rectangle
                var originalScissorRect = Global.GraphicsDevice.ScissorRectangle;

                // Apply scissor rectangle
                Global.GraphicsDevice.ScissorRectangle = new Rectangle(startX + 7 * scale, (int)scroll.ScreenPosition.Value.Y, middleWidth, Global.TileSize * scale);

                // Draw text with scissor test
                var totalWidth = (2 * scrollTopLeft.Width * scale) + scroll.DisplayWidth;
                Global.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, rasterizerState);

                var textStartX = (int)(scroll.ScreenPosition.Value.X - (scroll.TextDimensions.X / 2) + (scrollTopLeft.Width * scale / 2)) + 5;
                var textStartY = startY - 5;
                Global.SpriteBatch.DrawString(_fontText, scroll.Text, new Vector2(textStartX, textStartY), Color.Black);

                var subTextStartY = startY + 55;
                var subTextStartX = (int)(scroll.ScreenPosition.Value.X - (scroll.SubTextDimensions.X / 4) + (scrollTopLeft.Width * scale / 2));
                Global.SpriteBatch.DrawString(_fontSubText, scroll.SubText, new Vector2(subTextStartX, subTextStartY), Color.Black);

                Global.SpriteBatch.End();

                // Restore original scissor rectangle
                Global.GraphicsDevice.ScissorRectangle = originalScissorRect;

                // Draw scroll caps
                Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                Global.SpriteBatch.Draw(scrollTopLeft, new Rectangle(startX, (int)scroll.ScreenPosition.Value.Y, scrollTopLeft.Width * scale, Global.TileSize * scale), Color.White);
                Global.SpriteBatch.Draw(scrollTopRight, new Rectangle(startX + 7 * scale + middleWidth, (int)scroll.ScreenPosition.Value.Y, scrollTopLeft.Width * scale, Global.TileSize * scale), Color.White);
                Global.SpriteBatch.End();
            }

            
        }


    }
}
