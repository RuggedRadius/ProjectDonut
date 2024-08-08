using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class WorldMapSettings
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int TileSize { get; set; }

        // Heights
        public int WaterHeightMin { get; set; }
        public int WaterHeightMax { get; set; }
        public int GroundHeightMin { get; set; }
        public int GroundHeightMax { get; set; }
        public int MountainHeightMin { get; set; }
        public int MountainHeightMax { get; set; }     

        // Forest


        // Rivers
    }
}
