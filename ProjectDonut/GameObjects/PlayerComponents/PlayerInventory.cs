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
using ProjectDonut.Core;
using ProjectDonut.Core.Input;
using ProjectDonut.Interfaces;

namespace ProjectDonut.GameObjects.PlayerComponents
{
    public enum UIComponentState
    {
        Shown,
        Hidden
    }

    public class PlayerInventory : IGameObject
    {
        public UIComponentState State { get; set; }
        public int ZIndex { get; set; }

        public Texture2D Texture { get; set; }
        private Texture2D _slotTexture;
        private Texture2D _emptySlotTexture;

        public Vector2 Position { get; set; }

        public List<PlayerInventorySlot> Slots { get; set; }
        
        private int _slotsInRow = 8;

        private float _toggleTimeout = 0.2f;
        private float _toggleTimer = 0;

        public bool IsVisible { get; set; }

        public static SpriteFont QuantityFont { get; set; }

        public PlayerInventory(ContentManager content, GameCursor cursor)
        {
            State = UIComponentState.Hidden;
            ZIndex = 100;
            IsVisible = true;
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
            newSlot.LoadContent();

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
                    item.Icon = Global.ContentManager.Load<Texture2D>($"Sprites/UI/Items/{item.ItemID}");
                    item.ItemType = ItemType.Consumable;
                    return item;

                case 1:
                    var gold = new InventoryItem();
                    gold.ItemID = "gold";
                    gold.Name = "Gold";
                    gold.Description = "Currency can be exchanged for goods and services";
                    gold.Icon = Global.ContentManager.Load<Texture2D>($"Sprites/UI/Items/gold-pile-small");
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
                    slot.Item.Position = slot.Position + new Vector2(2, 2);
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
                    return 50;

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

        public void LoadContent()
        {
            Texture = Global.ContentManager.Load<Texture2D>("Sprites/UI/PlayerInventory");
            _slotTexture = Global.ContentManager.Load<Texture2D>("Sprites/UI/PlayerInventorySlot");
            _emptySlotTexture = Global.ContentManager.Load<Texture2D>("Sprites/UI/Items/empty-slot");
            QuantityFont = Global.ContentManager.Load<SpriteFont>("Fonts/Default");

            var x = Global.ScreenWidth - Texture.Width - 50;
            var y = Global.ScreenHeight - Texture.Height - 50;
            Position = new Vector2(x, y);
        }

        public void ToggleInventory()
        {
            if (_toggleTimer < _toggleTimeout)
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

            _toggleTimer = 0f;
        }

        public void Update(GameTime gameTime)
        {
            _toggleTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            var kbState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (kbState.IsKeyDown(Keys.I))
            {
                ToggleInventory();
            }

            CalculateSlotsBounds();

            var currentlyPickedUpOriginalSlot = Slots.Where(x => x.Item?.State == InventoryItemState.PickedUp).FirstOrDefault();
            foreach (var slot in Slots)
            {
                if (slot.Bounds.Contains(mouseState.Position)) // Mouse over has occurred
                {
                    if (mouseState.LeftButton == ButtonState.Pressed) // Mouse button clicked
                    {
                        if (slot.Item != null && currentlyPickedUpOriginalSlot == null)
                        {
                            slot.Item.State = InventoryItemState.PickedUp;
                        }
                    }
                    else if (currentlyPickedUpOriginalSlot != null) // Mouse button released
                    {
                        slot.Item = currentlyPickedUpOriginalSlot.Item;
                        slot.Item.State = InventoryItemState.InInventory;

                        // Get original slot and clear it
                        currentlyPickedUpOriginalSlot.Item = null;
                        currentlyPickedUpOriginalSlot = null;
                    }
                }
            }

            if (mouseState.LeftButton == ButtonState.Released)
            {
                foreach (var slot in Slots)
                {
                    if (slot.Item != null)
                    {
                        slot.Item.State = InventoryItemState.InInventory;
                    }
                }
            }
            else
            {
                foreach (var slot in Slots)
                {
                    if (slot.Item != null && slot.Item.State == InventoryItemState.PickedUp)
                    {
                        slot.Item.Position = Global.GameCursor.Position - new Vector2(slot.Item.Icon.Width, slot.Item.Icon.Height);
                    }
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (State == UIComponentState.Hidden)
            {
                return;
            }

            Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, transformMatrix: Matrix.Identity);
            Global.SpriteBatch.Draw(Texture, Position, Color.White);

            DrawSlots(gameTime);

            Global.SpriteBatch.End();
        }

        private void DrawSlots(GameTime gameTime)
        {
            foreach (var slot in Slots)
            {
                Global.SpriteBatch.Draw(_emptySlotTexture, slot.Bounds, Color.White);
            }

            foreach (var slot in Slots)
            {
                slot.Draw(gameTime);
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

            var maxPosition = Position.X + Texture.Width - outerSpacing - _emptySlotTexture.Width;

            foreach (var slot in Slots)
            {
                var x = Position.X + startWidth + offsetX;
                var y = Position.Y + startHeight + offsetY;

                var drawPos = new Vector2(x, y);
                var rect = new Rectangle((int)drawPos.X, (int)drawPos.Y, _emptySlotTexture.Width, _emptySlotTexture.Height);

                slot.Bounds = rect;

                offsetX += _emptySlotTexture.Width + cellSpacing;

                var endOfNextSlot = x + _emptySlotTexture.Width + cellSpacing;
                

                if (endOfNextSlot > maxPosition)
                {
                    offsetX = 0;
                    offsetY += _emptySlotTexture.Height + cellSpacing;
                }

                if (slot.Item != null)
                {
                    slot.Item.Position = new Vector2(slot.Bounds.X + 2, slot.Bounds.Y + 2);
                }
            }
        }
    }
}
