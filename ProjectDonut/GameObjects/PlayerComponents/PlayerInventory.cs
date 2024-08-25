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
        private Texture2D _emptySlotTexture;

        private Vector2 _position;

        public List<PlayerInventorySlot> Slots { get; set; }

        private ContentManager _content;
        private GameCursor _cursor;

        public PlayerInventory(ContentManager content, GameCursor cursor)
        {
            State = UIComponentState.Shown;
            ZIndex = 100;
            _content = content;
            _cursor = cursor;
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
            _emptySlotTexture = _content.Load<Texture2D>("Sprites/UI/Items/empty-slot");

            var x = 1920 - _baseTexture.Width - 50;
            var y = 1080 - _baseTexture.Height - 50;
            _position = new Vector2(x, y);
        }

        public void Update(GameTime gameTime)
        {
            var kbState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (kbState.IsKeyDown(Keys.I))
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

            foreach (var slot in Slots)
            {
                if (slot.Bounds.Contains(mouseState.Position))
                {

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

            var cellSpacing = 0;
            var outerSpacing = startWidth;

            foreach (var slot in Slots)
            {
                var x = _position.X + startWidth + offsetX;
                var y = _position.Y + startHeight + offsetY;

                var drawPos = new Vector2(x, y);

                spriteBatch.Draw(_emptySlotTexture, drawPos, Color.White);
                slot.Draw(spriteBatch, gameTime, drawPos);

                offsetX += _emptySlotTexture.Width + cellSpacing;

                if ((x + _emptySlotTexture.Width) > _position.X + _baseTexture.Width - outerSpacing)
                {
                    offsetX = 0;
                    offsetY += _emptySlotTexture.Height + cellSpacing;
                }
            }
        }
    }
}
