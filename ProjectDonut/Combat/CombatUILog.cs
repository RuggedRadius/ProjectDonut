using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectDonut.Core;

namespace ProjectDonut.Combat
{
    public class CombatUILog
    {
        private List<string> _logEntries;

        private Rectangle Bounds { get; set; }

        private int Width = 800;
        private int Height = 250;

        public CombatUILog()
        {
            _logEntries = new List<string>();

            Bounds = new Rectangle(
                Global.GraphicsDeviceManager.PreferredBackBufferWidth - Width - 10,
                Global.GraphicsDeviceManager.PreferredBackBufferHeight - Height - 10,
                Width,
                Height);
        }

        public void AddLogEntry(string entry)
        {
            _logEntries.Add(entry);
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime)
        {
            // Draw background
            Global.SpriteBatch.Draw(Global.BLANK_TEXTURE, Bounds, Color.Black * 0.75f);

            // Draw log entries
            int counter = 0;
            for (int i = _logEntries.Count - 1; i >= 0; i--)
            {
                if (i >= _logEntries.Count)
                {
                    break;
                }

                if (counter >= 10)
                {
                    break;
                }

                var position = new Vector2(Bounds.X + 10, Bounds.Y + 10 + (counter * 20));
                Global.SpriteBatch.DrawString(Global.FontDebug, _logEntries[i], position, Color.White);

                counter++;
            }
        }
    }
}
