using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Interfaces;
using IGameComponent = ProjectDonut.Interfaces.IGameComponent;

namespace ProjectDonut.GameObjects.PlayerComponents
{
    public class PlayerInventorySlot : IGameComponent
    {
        public Rectangle Bounds { get; set; }
        public InventoryItem Item { get; set; }
        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }

        private Texture2D _emptySlotTexture;

        private PlayerInventory _inventory;


        public PlayerInventorySlot(PlayerInventory inventory, InventoryItem item)
        {
            _inventory = inventory;
            Item = item;
        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
            _emptySlotTexture = Global.ContentManager.Load<Texture2D>("Sprites/UI/Items/empty-slot");
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime)
        {
            if (Item != null)
            {
                Global.SpriteBatch.Draw(Item.Icon, Item.Position, Color.White);

                if (Item.Quantity > 1)
                {
                    Global.SpriteBatch.DrawString(PlayerInventory.QuantityFont, Item.Quantity.ToString(), new Vector2(Item.Position.X + 20, Item.Position.Y + 20), Color.White);
                }
            }
        }
    }
}
