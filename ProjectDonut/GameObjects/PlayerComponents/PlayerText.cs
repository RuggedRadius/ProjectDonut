using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
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
        public Color TextColour { get; set; }

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
    }

    public class PlayerTextDisplay : IGameObject
    {
        public List<PlayerText> Texts { get; set; }

        public bool IsVisible => throw new NotImplementedException();

        public Texture2D Texture { get; set; }

        public Vector2 WorldPosition { get; set; }

        public int ZIndex { get; set; }

        public PlayerTextDisplay()
        {
            Texts = new List<PlayerText>();
        }

        public void AddText(string text, int duration, int offsetX, int offsetY, bool moveVertical, Color textColour)
        {
            var playerText = new PlayerText
            {
                Text = text,
                RemainingDuration = duration,
                OffsetX = offsetX,
                OffsetY = offsetY,
                MoveVertical = moveVertical,
                TextColour = textColour
            };

            Texts.Add(playerText);
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
                    text.TextColour);
            }

            Global.SpriteBatch.End();
        }
    }


}
