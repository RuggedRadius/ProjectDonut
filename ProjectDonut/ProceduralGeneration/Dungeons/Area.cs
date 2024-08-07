using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.Dungeons
{
    public class Area
    {
        public int xTop { get; set; }
        public int yTop { get; set; }
        public int xBottom { get; set; }
        public int yBottom { get; set; }

        public Area brother { get; set; }

        public int minWidth = 8;
        public int minHeight = 8;

        public int Width { get { return xTop - xBottom; } }
        public int Height { get { return yTop - yBottom; } }
        public bool IsPartitionable
        {
            get
            {
                return Width > 2 * minWidth && Height > 2 * minHeight;
            }
        }

        public Area(int xBottom, int yBottom, int xTop, int yTop, Area brother, int minWidth, int minHeight)
        {
            this.xTop = xTop;
            this.yTop = yTop;
            this.xBottom = xBottom;
            this.yBottom = yBottom;
            this.brother = brother;
            this.minWidth = minWidth;
            this.minHeight = minHeight;
        }
    }
}
