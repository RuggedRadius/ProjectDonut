using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ProjectDonut.Core
{
    public class FPSCounter
    {
        private double fps = 0.0;
        private double frameTime = 0.0;

        public void Update(GameTime gameTime)
        {
            // Calculate frame time in seconds
            frameTime = gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate FPS (frames per second)
            if (frameTime > 0.0)
            {
                fps = 1.0 / frameTime;
            }
        }

        public void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Begin();
            Global.SpriteBatch.DrawString(Global.FontDebug, $"FPS: {fps:F2}", new Vector2(10, 10), Color.White);
            Global.SpriteBatch.End();
        }
    }
}
