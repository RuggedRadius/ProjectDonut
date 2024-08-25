using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.GameObjects.PlayerComponents
{
    public enum ItemType
    {
        None,
        Weapon,
        Consumable,
        Armor,
        QuestItem,
        Currency
    }

    public static class InventorySettings
    {
        public static int SlotsInRow = 8;
        public static int SlotSize = 32;

        //public static int SlotPadding = 2;

        public static class MaxStackAmounts
        {

        }
    }
}
