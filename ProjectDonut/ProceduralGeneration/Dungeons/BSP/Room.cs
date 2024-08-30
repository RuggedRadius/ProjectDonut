using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectDonut.ProceduralGeneration.Dungeons.BSP
{
    public class Room
    {
        public int RoomID { get; set; }
        public Rectangle Bounds { get; set; }
        public Room Sibling { get; set; }

        public bool CanSplitHorizontal { get; set; }
        public bool CanSplitVertical { get; set; }
        public bool CorridorDrawn { get; set; }
    }
}
