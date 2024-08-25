using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
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
        private int _slotsInRow = 8;

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


            for (int i = 0; i < 30; i++)
            {
                var newSlot = CreateNewSlot();
                Slots.Add(newSlot);
            }

            for (int i = 0; i < 5; i++)
            {
                var item = CreateTestItem();
                AddItemToInventory(item);
            }
        }

        private PlayerInventorySlot CreateNewSlot(InventoryItem item = null)
        {
            var newSlot = new PlayerInventorySlot(this, item);
            newSlot.Initialize();
            newSlot.LoadContent(_content);

            return newSlot;
        }

        private InventoryItem CreateTestItem()
        {
            var choice = new Random().Next(0, 2);

            switch (choice)
            {   
                case 0:
                    var item = new InventoryItem();
                    item.ItemID = "health-potion-01";
                    item.Name = "Health Potion";
                    item.Description = "Heals 50 HP";
                    item.Icon = _content.Load<Texture2D>($"Sprites/UI/Items/{item.ItemID}");
                    item.ItemType = ItemType.Consumable;
                    return item;

                case 1:
                    var gold = new InventoryItem();
                    gold.ItemID = "gold";
                    gold.Name = "Gold";
                    gold.Description = "Currency can be exchanged for goods and services";
                    gold.Icon = _content.Load<Texture2D>($"Sprites/UI/Items/gold-pile-small");
                    gold.ItemType = ItemType.Currency;
                    return gold;

                default:
                    return null;
            }
        }

        public void AddItemToInventory(InventoryItem item)
        {
            foreach (var slot in Slots)
            {
                if (slot.Item != null && slot.Item.Name == item.Name)
                {
                    var maxStack = GetMaxStackAmount(slot.Item.ItemType);

                    if (slot.Item.Quantity < maxStack)
                    {
                        slot.Item.Quantity++;
                        return;
                    }
                }
            }

            foreach (var slot in Slots)
            {
                if (slot.Item == null)
                {
                    slot.Item = item;
                    return;
                }
            }

            // Inventory is full!
            return;
        }

        private int GetMaxStackAmount(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                    return 1;

                case ItemType.Consumable:
                    return 10;

                case ItemType.Armor:
                    return 1;

                case ItemType.QuestItem:
                    return 1;

                case ItemType.Currency:
                    return 5000;

                default:
                    return 1;
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

            CalculateSlotsBounds();

            Debugging.Debugger.Lines[2] = string.Empty;
            foreach (var slot in Slots)
            {
                if (slot.Bounds.Contains(mouseState.Position))
                {
                    if (slot.Item == null)
                    {
                        Debugging.Debugger.Lines[2] = "Empty slot";
                    }
                    else
                    {
                        Debugging.Debugger.Lines[2] = $"{slot.Item.Name} x {slot.Item.Quantity}";
                    }
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
            foreach (var slot in Slots)
            {
                spriteBatch.Draw(_emptySlotTexture, slot.Bounds, Color.White);
                slot.Draw(gameTime, spriteBatch);
            }
        }

        private void CalculateSlotsBounds()
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
                var rect = new Rectangle((int)drawPos.X, (int)drawPos.Y, _emptySlotTexture.Width, _emptySlotTexture.Height);

                slot.Bounds = rect;

                offsetX += _emptySlotTexture.Width + cellSpacing;

                var endOfNextSlot = x + _emptySlotTexture.Width + cellSpacing;
                var maxPosition = _position.X + _baseTexture.Width - outerSpacing;

                if (endOfNextSlot > maxPosition)
                {
                    offsetX = 0;
                    offsetY += _emptySlotTexture.Height + cellSpacing;
                }
            }
        }
    }
}
