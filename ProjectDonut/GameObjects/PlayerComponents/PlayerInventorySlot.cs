using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDonut.GameObjects.PlayerComponents
{
    public class PlayerInventorySlot
    {
        public InventoryItem Item { get; set; }
        private Texture2D _emptySlotTexture;

        private PlayerInventory _inventory;
        private ContentManager _content;

        public PlayerInventorySlot(PlayerInventory inventory, ContentManager content)
        {
            _inventory = inventory;
            _content = content;

            _emptySlotTexture = _content.Load<Texture2D>("Sprites/UI/Items/empty-slot");
        }

        public void SetItem(InventoryItem item)
        {
            Item = item;

            if (item != null)
            {
                Item.Icon = _content.Load<Texture2D>($"Sprites/UI/Items/{Item.ItemID}");
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 drawPosition)
        {
            spriteBatch.Draw(_emptySlotTexture, drawPosition, Color.White);

            if (Item != null)
            {
                var itemPos = new Vector2(drawPosition.X + 2, drawPosition.Y + 2);
                spriteBatch.Draw(Item.Icon, drawPosition, Color.White);
            }
        }
    }
}
