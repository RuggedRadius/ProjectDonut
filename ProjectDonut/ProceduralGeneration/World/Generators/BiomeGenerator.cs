using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.World.Generators
{
    public class BiomeGenerator
    {
        private FastNoiseLite[] _noise;
        private WorldMapSettings settings;
        private float OctaveBlendAmount = 0.125f;

        public BiomeGenerator(WorldMapSettings settings)
        {
            this.settings = settings;

            var random = new Random();
            _noise = new FastNoiseLite[2];

            _noise[0] = new FastNoiseLite();
            _noise[0].SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            _noise[0].SetSeed(random.Next(int.MinValue, int.MaxValue));
            _noise[0].SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
            _noise[0].SetCellularJitter(1.0f);
            _noise[0].SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            _noise[0].SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            _noise[0].SetDomainWarpAmp(100.0f);
            _noise[0].SetFrequency(0.0075f);
            _noise[0].SetFractalGain(0.5f);
            _noise[0].SetFractalType(FastNoiseLite.FractalType.DomainWarpIndependent);
            _noise[0].SetFractalOctaves(8);
            _noise[0].SetFractalLacunarity(2.0f);

            _noise[1] = new FastNoiseLite();
            _noise[1].SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            _noise[1].SetSeed(random.Next(int.MinValue, int.MaxValue));
            _noise[1].SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
            _noise[1].SetCellularJitter(1.0f);
            _noise[1].SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            _noise[1].SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            _noise[1].SetDomainWarpAmp(100.0f);
            _noise[1].SetFrequency(0.0075f);
            _noise[1].SetFractalGain(3.5f);
            _noise[1].SetFractalType(FastNoiseLite.FractalType.FBm);
            _noise[1].SetFractalOctaves(3);
            _noise[1].SetFractalLacunarity(4.0f);
            _noise[1].SetFractalWeightedStrength(0);
        }

        public int[,] GenerateBiomes(int width, int height, int xOffset, int yOffset)
        {
            int biomeCount = Enum.GetNames(typeof(Biome)).Length;

            var data = new List<int[,]>();

            for (int z = 0; z < 2; z++)
            {
                int[,] heightData = new int[height, width];

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        heightData[i, j] = (int)(_noise[z].GetNoise(xOffset * settings.Width + i, yOffset * settings.Height + j) * biomeCount * 100);
                    }
                }

                data.Add(heightData);
            }

            var result = new int[height, width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    result[i, j] = Blend(data[0][i, j], data[1][i, j], OctaveBlendAmount) / 100;
                }
            }

            return result;
        }

        int Blend(int a, int b, float t)
        {
            return (int)(a + t * (b - a));
        }
    }
}
