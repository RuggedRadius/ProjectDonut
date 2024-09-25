using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Core;
using ProjectDonut.Core.Input;
using ProjectDonut.Interfaces;
using System;

namespace ProjectDonut.GameObjects.PlayerComponents
{
    public class PlayerEquipment : IUIComponent
    {
        public UIComponentState State { get; set; }

        public int ZIndex { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 ScreenPosition { get; set; }

        public PlayerEquipment()
        {
            State = UIComponentState.Hidden;
            ZIndex = 100;
        }


        public void Initialize()
        {
        }

        public void LoadContent()
        {
            Texture = Global.ContentManager.Load<Texture2D>("Sprites/UI/PlayerEquipment");

            var x = 50;
            var y = 50;
            ScreenPosition = new Vector2(x, y);
        }

        public void Update(GameTime gameTime)
        {
            if (InputManager.IsKeyPressed(Keys.U))
            {
                ToggleVisibility();
            }

            if (State == UIComponentState.Hidden)
            {
                return;
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (State == UIComponentState.Hidden)
            {
                return;
            }

            Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, transformMatrix: Matrix.Identity);
            Global.SpriteBatch.Draw(Texture, ScreenPosition, Color.White);

            Global.SpriteBatch.End();
        }

        public void ToggleVisibility()
        {
            if (Global.Debug.Console.IsVisible)
            {
                return;
            }

            if (State == UIComponentState.Hidden)
            {
                State = UIComponentState.Shown;
            }
            else
            {
                State = UIComponentState.Hidden;
            }
        }
    }
}
