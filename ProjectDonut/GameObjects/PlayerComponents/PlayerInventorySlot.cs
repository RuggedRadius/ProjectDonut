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
        private ContentManager _content;


        public PlayerInventorySlot(PlayerInventory inventory, ContentManager content)
        {
            _inventory = inventory;
            _content = content;

            
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
            if (Item != null)
            {
                var itemPos = new Vector2(drawPosition.X + 2, drawPosition.Y + 2);
                spriteBatch.Draw(Item.Icon, drawPosition, Color.White);
            }
        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
            _emptySlotTexture = _content.Load<Texture2D>("Sprites/UI/Items/empty-slot");
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Item != null)
            {
                var itemPos = new Vector2(Bounds.X + 2, Bounds.Y + 2);
                spriteBatch.Draw(Item.Icon, itemPos, Color.White);
            }
        }
    }
}
