using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using ProjectDonut.Core.Input;
using ProjectDonut.Core;
using ProjectDonut.Combat.Combatants;

namespace ProjectDonut.Combat.UI
{
    public class FloatingText
    {
        public bool MoveVertical { get; set; }
        public string Text { get; set; }
        public float RemainingDuration { get; set; }
        public float TotalDuration { get; set; }
        public Color TextColour { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
    }
    public class FloatingTextDisplay
    {
        private Queue<FloatingText> TextQueue { get; set; }
        private List<FloatingText> Texts { get; set; }
        private ICombatant Combatant { get; set; }


        private float _baseDuration = 2;
        private int _baseOffsetX = 5;
        private int _baseOffsetY = 5;

        public FloatingTextDisplay(ICombatant combatant)
        {
            Texts = new List<FloatingText>();
            TextQueue = new Queue<FloatingText>();
            Combatant = combatant;
        }

        public void AddText(string text, int durationMod, Vector2 offsetMod, Color textColour, bool moveVertical = true)
        {
            var playerText = new FloatingText
            {
                Text = text,
                TextColour = textColour,
                RemainingDuration = _baseDuration + durationMod,
                TotalDuration = _baseDuration + durationMod,
                MoveVertical = moveVertical,

                OffsetX = (int)(_baseOffsetX + offsetMod.X),
                OffsetY = (int)(_baseOffsetY + offsetMod.Y),
            };

            //Texts.Add(playerText);
            TextQueue.Enqueue(playerText);
        }

        public void Initialize()
        {

        }

        public void LoadContent()
        {
            // TODO: Load custom here...
        }

        public void Update(GameTime gameTime)
        {
            if (TextQueue.Count > 0)
            {
                var text = TextQueue.Dequeue();
                //text.OffsetY -= (20 * Texts.Count);

                foreach (var txt in Texts)
                {
                    if (txt == null)
                        continue;

                    txt.OffsetY -= 20;
                }

                Texts.Add(text);
            }

            for (int i = 0; i < Texts.Count; i++)
            {
                var text = Texts[i];
                if (text == null)
                    continue;

                text.RemainingDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (text.MoveVertical)
                {
                    text.OffsetY -= 1;
                }

                if (text.RemainingDuration <= 0)
                {
                    Texts.RemoveAt(i);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < Texts.Count; i++)
            {
                var text = Texts[i];

                if (text == null)
                    continue;

                Global.SpriteBatch.DrawString(
                    Global.FontDebug,
                    text.Text,
                    new Vector2(
                        Combatant.ScreenPosition.X + text.OffsetX,
                        Combatant.ScreenPosition.Y + text.OffsetY),
                    text.TextColour * (text.RemainingDuration / text.TotalDuration));
            }
        }
    }
}