using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Interfaces;

namespace ProjectDonut.GameObjects.PlayerComponents
{
    public class PlayerInventorySlot : IGameObject
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

        public void LoadContent(ContentManager content)
        {
            _emptySlotTexture = content.Load<Texture2D>("Sprites/UI/Items/empty-slot");
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Item != null)
            {
                spriteBatch.Draw(Item.Icon, Item.Position, Color.White);
            }
        }
    }
}
