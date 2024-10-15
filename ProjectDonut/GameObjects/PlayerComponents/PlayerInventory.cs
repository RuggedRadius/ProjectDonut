using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Core.Input;
using ProjectDonut.Interfaces;

namespace ProjectDonut.GameObjects.PlayerComponents
{
    public enum UIComponentState
    {
        Shown,
        Hidden
    }

    public class PlayerInventory : IUIComponent
    {
        public UIComponentState State { get; set; }
        public int ZIndex { get; set; }

        public Texture2D Texture { get; set; }
        private Texture2D _slotTexture;
        private Texture2D _emptySlotTexture;

        public Vector2 ScreenPosition { get; set; }

        public List<PlayerInventorySlot> Slots { get; set; }
        
        private int _slotsInRow = 8;

        public static SpriteFont QuantityFont { get; set; }

        public PlayerInventory()
        {
            State = UIComponentState.Hidden;
            ZIndex = 100;
        }

        public void Initialize()
        {
            Slots = new List<PlayerInventorySlot>();

            for (int i = 0; i < 30; i++)
            {
                var newSlot = CreateNewSlot();
                Slots.Add(newSlot);
            }
        }

        private PlayerInventorySlot CreateNewSlot(InventoryItem item = null)
        {
            var newSlot = new PlayerInventorySlot(this, item);
            newSlot.Initialize();
            newSlot.LoadContent();

            return newSlot;
        }

        //private InventoryItem CreateTestItem()
        //{
        //    var choice = new Random().Next(0, 2);

        //    switch (choice)
        //    {
        //        case 0:
        //            var item = new InventoryItem();
        //            item.ItemID = "health-potion-01";
        //            item.Name = "Health Potion";
        //            item.Description = "Heals 50 HP";
        //            item.Icon = Global.ContentManager.Load<Texture2D>($"Sprites/UI/Items/{item.ItemID}");
        //            item.ItemType = ItemType.Consumable;
        //            return item;

        //        case 1:
        //            var gold = new InventoryItem();
        //            gold.ItemID = "gold";
        //            gold.Name = "Gold";
        //            gold.Description = "Currency can be exchanged for goods and services";
        //            gold.Icon = Global.ContentManager.Load<Texture2D>($"Sprites/UI/Items/gold-pile-small");
        //            gold.ItemType = ItemType.Currency;
        //            return gold;

        //        default:
        //            return null;
        //    }
        //}

        public bool HasRoomForItem(InventoryItem item)
        {
            var emptySlots = Slots.Where(x => x.Item == null).ToList();
            var existingUnmaxxedStacks = Slots
                .Where(x => x.Item != null && x.Item.Name == item.Name && x.Item.Quantity < GetMaxStackAmount(x.Item.ItemType))
                .ToList();

            if (emptySlots.Count > 0 || existingUnmaxxedStacks.Count > 0)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Adds the quantity of the item to the inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>An integer count of the item NOT added to the inventory if it is full</returns>
        public int AddItemToInventory(InventoryItem item)
        {
            if (item.Quantity <= 0)
            {
                throw new Exception("Item quantity must be greater than 0");
            }
            
            int quantity = item.Quantity;

            // Check for full inventory
            var emptySlots = Slots.Where(x => x.Item == null).ToList();
            var existingUnmaxxedStacks = Slots
                .Where(x => x.Item != null && x.Item.Name == item.Name && x.Item.Quantity < GetMaxStackAmount(x.Item.ItemType))
                .ToList();

            if (emptySlots.Count == 0 && existingUnmaxxedStacks.Count == 0)
            {
                return quantity;
            }

            var maxStackQuantity = GetMaxStackAmount(item.ItemType);
            var itemsPlaced = 0;
            //var attemptCounter = 10;
            while (itemsPlaced < quantity)
            {
                existingUnmaxxedStacks = Slots
                .Where(x => x.Item != null && x.Item.Name == item.Name && x.Item.Quantity < GetMaxStackAmount(x.Item.ItemType))
                .ToList();

                if (existingUnmaxxedStacks.Count == 0)
                {
                    emptySlots = Slots.Where(x => x.Item == null).ToList();

                    if (emptySlots.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        emptySlots[0].Item = new InventoryItem()
                        {
                            Name = item.Name,
                            Description = item.Description,
                            Icon = item.Icon,
                            ItemType = item.ItemType,
                            Quantity = 1,
                            Position = emptySlots[0].Position + new Vector2(2, 2)
                        };
                        itemsPlaced++;
                    }
                }
                else
                {
                    existingUnmaxxedStacks[0].Item.Quantity++;
                    itemsPlaced++;
                }
            }

            Global.Player.TextDisplay.AddText($"+{itemsPlaced} " + item.Name, 0, Vector2.Zero, Color.White);

            return quantity - itemsPlaced;
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

            var x = 100 + Texture.Width;
            var y = 50;
            ScreenPosition = new Vector2(x, y);
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

        public void Update(GameTime gameTime)
        {
            if (InputManager.IsKeyPressed(Keys.I))
            {
                ToggleVisibility();
            }

            if (State == UIComponentState.Hidden)
            {
                return;
            }

            CalculateSlotsBounds();

            var currentlyPickedUpOriginalSlot = Slots.Where(x => x.Item?.State == InventoryItemState.PickedUp).FirstOrDefault();
            foreach (var slot in Slots)
            {
                if (slot.Bounds.Contains(InputManager.MouseState.Position)) // Mouse over has occurred
                {
                    if (InputManager.MouseState.LeftButton == ButtonState.Pressed) // Mouse button clicked
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

            if (InputManager.MouseState.LeftButton == ButtonState.Released)
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
            Global.SpriteBatch.Draw(Texture, ScreenPosition, Color.White);

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
            var startHeight = 50;
            var startWidth = 6;

            var offsetX = 0;
            var offsetY = 0;

            var cellSpacing = 0;
            var outerSpacing = startWidth;

            var maxPosition = ScreenPosition.X + Texture.Width - outerSpacing - _emptySlotTexture.Width;

            foreach (var slot in Slots)
            {
                var x = ScreenPosition.X + startWidth + offsetX;
                var y = ScreenPosition.Y + startHeight + offsetY;

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
