using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class BiomeMapGenerator
    {
        private FastNoiseLite noise;
        private WorldMapSettings settings;


        public BiomeMapGenerator(WorldMapSettings settings) 
        {
            this.settings = settings;

            noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            noise.SetSeed(new Random().Next(int.MinValue, int.MaxValue));

            noise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
            noise.SetCellularJitter(1.0f);
            noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);

            noise.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            noise.SetDomainWarpAmp(100.0f);
            noise.SetFrequency(0.0075f);

            noise.SetFractalGain(0.5f);
            noise.SetFractalType(FastNoiseLite.FractalType.DomainWarpIndependent);
            noise.SetFractalOctaves(3);
            noise.SetFractalLacunarity(2.0f);
        }

        public int[,] GenerateBiomes(int width, int height, int xOffset, int yOffset)
        {
            // Gather noise data
            float[,] noiseData = new float[height, width];
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var actualOffsetX = (xOffset) * (-settings.Width);
                    var actualOffsetY = (yOffset) * (-settings.Height);

                    //float sampleX = actualOffsetX + x;
                    //float sampleY = actualOffsetY + y;

                    float sampleX = -(xOffset * width + x);
                    float sampleY = -(yOffset * height + y);

                    noiseData[x, y] = noise.GetNoise(sampleX, sampleY);                    

                    if (noiseData[x, y] < minValue)
                    {
                        minValue = noiseData[x, y];
                    }
                    if (noiseData[x, y] > maxValue)
                    {
                        maxValue = noiseData[x, y];
                    }
                }
            }

            // Normalise and convert to integer
            int[,] intData = new int[height, width];
            float range = maxValue - minValue;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Normalise value to the range [0, 1]
                    float normalizedValue = (noiseData[x, y] - minValue) / range;

                    // Scale to integer range (e.g., 0 to 255)
                    intData[x, y] = (int)(normalizedValue * Enum.GetNames(typeof(Biome)).Length);
                }
            }

            return intData;
        }
    }
}
