using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class WorldMapSettings
    {
        // General 
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileSize { get; set; }

        // Heights
        public int DeepWaterHeightMin { get; set; }
        public int DeepWaterHeightMax { get; set; }
        public int WaterHeightMin { get; set; }
        public int WaterHeightMax { get; set; }
        public int GroundHeightMin { get; set; }
        public int GroundHeightMax { get; set; }
        public int MountainHeightMin { get; set; }
        public int MountainHeightMax { get; set; }

        // Forest
        public int ForestCount { get; set; }
        public int MinWalk { get; set; }
        public int MaxWalk { get; set; }
        public int WalkRadius { get; set; }

        // Rivers
        //public int RiverCount { get; set; }
        public int RiverCount => 20;// (Width * Height) / 1000;
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public double RiverForkChance { get; set; }
        public int MinForkLength { get; set; }
        public int MinRiverRadius { get; set; }
        public int MaxRiverRadius { get; set; } 
        public float RiverRadiusDegradationChance { get; set; }

        // Erosion
        public int CoastErosionMin { get; set; }
        public int CoastErosionMax { get; set; }
        public int BiomeErosionMin { get; set; }
        public int BiomeErosionMax { get; set; }
        public int DeepWaterErosionMin { get; set; }
        public int DeepWaterErosionMax { get; set; }
        public int DeepWaterErosionWidthMin { get; set; }
        public int DeepWaterErosionWidthMax { get; set; }
    }
}
