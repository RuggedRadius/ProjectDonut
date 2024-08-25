using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Interfaces;

namespace ProjectDonut.GameObjects.PlayerComponents
{
    public enum UIComponentState
    {
        Shown,
        Hidden
    }

    public class PlayerInventory// : IScreenObject
    {
        public UIComponentState State { get; set; }
        public int ZIndex { get; set; }

        private Texture2D _baseTexture;
        private Texture2D _slotTexture;

        private Vector2 _position;

        public List<PlayerInventorySlot> Slots { get; set; }

        private ContentManager _content;

        public PlayerInventory(ContentManager content)
        {
            State = UIComponentState.Shown;
            ZIndex = 100;
            _content = content;
        }

        public void Initialize()
        {
            Slots = new List<PlayerInventorySlot>();

            for (int i = 0; i < 5; i++)
            {
                var newSlot = new PlayerInventorySlot(this, _content);
                var item = new InventoryItem()
                {
                    ItemID = "health-potion-01"
                };
                newSlot.SetItem(item);
                Slots.Add(newSlot);
            }
            for (int i = 0; i < 50; i++)
            {
                var newSlot = new PlayerInventorySlot(this, _content);
                Slots.Add(newSlot);
            }

        }

        public void LoadContent(ContentManager content)
        {
            _baseTexture = content.Load<Texture2D>("Sprites/UI/PlayerInventory");
            _slotTexture = content.Load<Texture2D>("Sprites/UI/PlayerInventorySlot");

            var x = 1920 - _baseTexture.Width - 50;
            var y = 1080 - _baseTexture.Height - 50;
            _position = new Vector2(x, y);
        }

        public void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.I))
            {
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

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (State == UIComponentState.Hidden)
            {
                return;
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, transformMatrix: Matrix.Identity);
            spriteBatch.Draw(_baseTexture, _position, Color.White);

            DrawSlots(spriteBatch, gameTime);

            spriteBatch.End();
        }

        private void DrawSlots(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var startHeight = 400;
            var startWidth = 6;

            var offsetX = 0;
            var offsetY = 0;

            foreach (var slot in Slots)
            {
                var x = _position.X + startWidth + offsetX;
                var y = _position.Y + startHeight + offsetY;

                var drawPos = new Vector2(x, y);
                slot.Draw(spriteBatch, gameTime, drawPos);

  

                offsetX += 32 + 3;

                if ((x + 32) > _position.X + _baseTexture.Width - 8)
                {
                    offsetX = 0;
                    offsetY += 35;
                }
            }
        }
    }
}
