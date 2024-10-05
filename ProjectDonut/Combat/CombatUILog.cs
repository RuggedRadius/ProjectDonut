using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectDonut.Core;
using static System.Net.Mime.MediaTypeNames;

namespace ProjectDonut.Combat
{
    public class CombatUILog
    {
        private List<string> _logEntries;

        private Rectangle Bounds { get; set; }

        private int linesToShowCount = 10;
        private int lineHeight = 20;
        private int padding = 10;
        private int Width = 800;
        private int Height = 250;
        private int margin = 10;

        public CombatUILog()
        {
            _logEntries = new List<string>();

            Height = linesToShowCount * lineHeight + (2 * padding);

            Bounds = new Rectangle(
                Global.GraphicsDeviceManager.PreferredBackBufferWidth - Width - margin,
                Global.GraphicsDeviceManager.PreferredBackBufferHeight - Height - margin,
                Width,
                Height);
        }

        public void AddLogEntry(string entry)
        {
            _logEntries.Add(entry);
        }

        public void Draw(GameTime gameTime)
        {
            // Draw background
            Global.SpriteBatch.Draw(Global.BLANK_TEXTURE, Bounds, Color.Black * 0.5f);

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

                var position = new Vector2(Bounds.X + padding, Bounds.Y + padding + (counter * lineHeight));
                //Global.SpriteBatch.DrawString(Global.FontDebug, _logEntries[i], position, Color.White);
                var components = GetLogComponents(_logEntries[i]);
                DrawLog(components, position);

                counter++;
            }
        }

        private void DrawLog(List<(string log, Color colour)> components, Vector2 position)
        {
            var curPosition = position;
            foreach (var c in components)
            {
                var stringSize = Global.FontDebug.MeasureString(c.log);
                Global.SpriteBatch.DrawString(Global.FontDebug, c.log, curPosition, c.colour);
                curPosition.X += stringSize.X;
            }
        }

        private List<(string, Color)> GetLogComponents(string log)
        {
            var components = new List<(string, Color)>();

            var curComponent = "";
            var curColour = Color.White;

            for (int i = 0; i < log.Length; i++)
            {
                if (i < log.Length - 3 &&
                    log[i] == '[' &&  
                    log[i + 1] == '#')
                {
                    // Begin colour
                    components.Add((curComponent, curColour));
                    curComponent = "";

                    var colourString = "";
                    int counter = i + 2;
                    while (log[counter] != ']')
                    {
                        colourString += log[counter];
                        counter++;
                    }

                    curColour = GetColor(colourString);
                    i += colourString.Length + 2;
                }
                else if (i < log.Length - 2 &&
                    log[i] == '[' && 
                    log[i + 1] == '/' &&
                    log[i + 2] == ']')
                {
                    // End colour
                    components.Add((curComponent, curColour));
                    curComponent = "";
                    curColour = Color.White;

                    i += 2;
                }
                else
                {
                    // Normal character
                    curComponent += log[i];
                }
            }

            components.Add((curComponent, curColour));
            return components;
        }

        private Color GetColor(string name) 
        {
            switch (name.ToLower()) 
            { 
                case "red": return Color.Red;
                case "green": return Color.Green;
                case "blue": return Color.Blue;
                case "yellow": return Color.Yellow;
                case "orange": return Color.Orange;
                case "purple": return Color.Purple;
                case "white": return Color.White;
                case "black": return Color.Black;
                case "cyan": return Color.Cyan;
                case "magenta": return Color.Magenta;
                case "gray": return Color.Gray;
                default: return Color.White;
            }
        }
    }
}
