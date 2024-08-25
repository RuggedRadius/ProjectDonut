using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDonut.GameObjects.PlayerComponents
{
    public class InventoryItem
    {
        public ItemType ItemType { get; set; }
        public string ItemID { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public Texture2D Icon { get; set; }

        public InventoryItem()
        {
           Quantity = 1;
        }
    }
}
