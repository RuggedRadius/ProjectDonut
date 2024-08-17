using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class BiomeGenerator
    {
        private FastNoiseLite _noise;
        private WorldMapSettings settings;


        public BiomeGenerator(WorldMapSettings settings) 
        {
            this.settings = settings;

            _noise = new FastNoiseLite();
            _noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            _noise.SetSeed(new Random().Next(int.MinValue, int.MaxValue));

            _noise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
            _noise.SetCellularJitter(1.0f);
            _noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);

            _noise.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            _noise.SetDomainWarpAmp(100.0f);
            _noise.SetFrequency(0.0075f);

            _noise.SetFractalGain(0.5f);
            _noise.SetFractalType(FastNoiseLite.FractalType.DomainWarpIndependent);
            _noise.SetFractalOctaves(3);
            _noise.SetFractalLacunarity(2.0f);
        }

        public int[,] GenerateBiomes(int width, int height, int xOffset, int yOffset)
        {
            int biomeCount = Enum.GetNames(typeof(Biome)).Length;
            int[,] heightData = new int[height, width];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    heightData[i, j] = (int)(_noise.GetNoise((xOffset * settings.Width) + i, (yOffset * settings.Height) + j) * biomeCount);
                }
            }

            return heightData;
        }
    }
}
