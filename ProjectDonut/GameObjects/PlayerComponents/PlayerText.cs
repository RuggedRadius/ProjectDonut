using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Core;
using ProjectDonut.Core.Input;
using ProjectDonut.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.GameObjects.PlayerComponents
{
    public class PlayerText
    {
        public bool MoveVertical { get; set; }

        public string Text { get; set; }
        public float RemainingDuration { get; set; }
        public float TotalDuration { get; set; }
        public Color TextColour { get; set; }

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
    }

    public class PlayerTextDisplay : IGameObject
    {
        public Queue<PlayerText> TextQueue { get; set; }
        public List<PlayerText> Texts { get; set; }

        public bool IsVisible => throw new NotImplementedException();

        public Texture2D Texture { get; set; }

        public Vector2 WorldPosition { get; set; }

        public int ZIndex { get; set; }

        private float _baseDuration = 2;
        private int _baseOffsetX = 5;
        private int _baseOffsetY = 5;

        public PlayerTextDisplay()
        {
            Texts = new List<PlayerText>();
            TextQueue = new Queue<PlayerText>();
        }

        public void AddText(string text, int durationMod, Vector2 offsetMod, Color textColour, bool moveVertical = true)
        {
            var playerText = new PlayerText
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
            if (InputManager.KeyboardState.IsKeyDown(Keys.G))
            {
                AddText("Testing 123", 0, Vector2.Zero, Color.White);
            }

            if (TextQueue.Count > 0)
            {
                var text = TextQueue.Dequeue();
                //text.OffsetY -= (20 * Texts.Count);

                foreach (var txt in Texts)
                {
                    txt.OffsetY -= 20;
                }

                Texts.Add(text);
            }

            for (int i = 0; i < Texts.Count; i++)
            {
                var text = Texts[i];
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
            Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());

            for (int i = 0; i < Texts.Count; i++)
            {
                var text = Texts[i];
                Global.SpriteBatch.DrawString(
                    Global.FontDebug, 
                    text.Text, 
                    new Vector2(
                        Global.PlayerObj.WorldPosition.X + text.OffsetX, 
                        Global.PlayerObj.WorldPosition.Y + text.OffsetY), 
                    text.TextColour * (text.RemainingDuration/text.TotalDuration));
            }

            Global.SpriteBatch.End();
        }
    }


}
