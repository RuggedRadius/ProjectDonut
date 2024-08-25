using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDonut.GameObjects.PlayerComponents
{
    public enum InventoryItemState
    {
        Equipped,
        InInventory,
        PickedUp
    }

    public class InventoryItem
    {
        public ItemType ItemType { get; set; }
        public InventoryItemState State { get; set; }
        public string ItemID { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public Texture2D Icon { get; set; }
        public Vector2 Position { get; set; }

        public InventoryItem()
        {
            Quantity = 1;
            State = InventoryItemState.InInventory;
        }
    }
}
