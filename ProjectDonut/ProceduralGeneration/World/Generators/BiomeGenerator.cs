using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectDonut.Core;
using ProjectDonut.Core.SceneManagement.SceneTypes;

namespace ProjectDonut.ProceduralGeneration.World.Generators
{
    public class BiomeGenerator
    {
        private FastNoiseLite[] _noise;
        private WorldMapSettings settings;
        private float OctaveBlendAmount = 0.05f;

        private int[,] heightData;
        private int[,] tempData;
        private int[,] moistureData;

        private Random _random;

        public BiomeGenerator(WorldMapSettings settings)
        {
            this.settings = settings;
            var biomeSize = 0.005f;

            _random = new Random();
            _noise = new FastNoiseLite[4];

            _noise[0] = new FastNoiseLite();
            _noise[0].SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            _noise[0].SetSeed(_random.Next(int.MinValue, int.MaxValue));
            _noise[0].SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
            _noise[0].SetCellularJitter(1.01f);
            _noise[0].SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            _noise[0].SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            _noise[0].SetDomainWarpAmp(100.0f);
            _noise[0].SetFrequency(biomeSize);
            _noise[0].SetFractalGain(0.75f);
            _noise[0].SetFractalType(FastNoiseLite.FractalType.None);
            _noise[0].SetFractalOctaves(8);
            _noise[0].SetFractalLacunarity(2.0f);

            _noise[1] = new FastNoiseLite();
            _noise[1].SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            _noise[1].SetSeed(_random.Next(int.MinValue, int.MaxValue));
            _noise[1].SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Manhattan);
            _noise[1].SetCellularJitter(1.02f);
            _noise[1].SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            _noise[1].SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            _noise[1].SetDomainWarpAmp(100.0f);
            _noise[1].SetFrequency(biomeSize);
            _noise[1].SetFractalGain(3.5f);
            _noise[1].SetFractalType(FastNoiseLite.FractalType.None);
            _noise[1].SetFractalOctaves(3);
            _noise[1].SetFractalLacunarity(4.0f);
            _noise[1].SetFractalWeightedStrength(0);

            _noise[2] = new FastNoiseLite();
            _noise[2].SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            _noise[2].SetSeed(_random.Next(int.MinValue, int.MaxValue));
            _noise[2].SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
            _noise[2].SetCellularJitter(1.01f);
            _noise[2].SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            _noise[2].SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            _noise[2].SetDomainWarpAmp(100.0f);
            _noise[2].SetFrequency(biomeSize);
            _noise[2].SetFractalGain(0.75f);
            _noise[2].SetFractalType(FastNoiseLite.FractalType.None);
            _noise[2].SetFractalOctaves(8);
            _noise[2].SetFractalLacunarity(2.0f);

            _noise[3] = new FastNoiseLite();
            _noise[3].SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            _noise[3].SetSeed(_random.Next(int.MinValue, int.MaxValue));
            //_noise[3].SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
            //_noise[3].SetCellularJitter(1.01f);
            //_noise[3].SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            //_noise[3].SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            //_noise[3].SetDomainWarpAmp(100.0f);
            _noise[3].SetFrequency(0.02f);
            //_noise[3].SetFractalGain(0.75f);
            //_noise[3].SetFractalType(FastNoiseLite.FractalType.None);
            //_noise[3].SetFractalOctaves(8);
            //_noise[3].SetFractalLacunarity(2.0f);
        }

        public int[,] GenBiomes(int width, int height, int xOffset, int yOffset, WorldChunk chunk)
        {
            var tempData = new float[height, width];
            var moistureData = new float[height, width];
            var randomNoise = new float[height, width];
            var result = new float[height, width];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tempData[i, j] = _noise[1].GetNoise(xOffset * settings.Width + i, yOffset * settings.Height + j);
                    moistureData[i, j] = _noise[2].GetNoise(xOffset * settings.Width + i, yOffset * settings.Height + j);
                    randomNoise[i, j] = _noise[3].GetNoise(xOffset * settings.Width + i, yOffset * settings.Height + j);
                }
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    result[i, j] = DetermineBiome(chunk.HeightData[i, j], tempData[i, j], moistureData[i, j]);
                }
            }

            var distanceToOtherBiome = float.MaxValue;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    // foreach tile, iterate over every other tile to get distance
                    // if distance if below threshold, start randomised values
                }
            }



            // Blur with ...something
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    result[i, j] += tempData[i, j];
                    result[i, j] += moistureData[i, j];
                    //result[i, j] += randomNoise[i, j] * _random.Next(-1, 2);
                }
            }

            var biomes = new int[height, width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    biomes[i, j] = (int)Math.Round(result[i, j], 0);
                }
            }

            return biomes;
        }

        private int DetermineBiome(float height, float temp, float humidity)
        {
            var points = new int[Enum.GetValues(typeof(Biome)).Length];

            // TEMPERATURE
            if (temp < -0.75f)
            {
                points[(int)Biome.Winterlands] += 1;
            }
            else if (temp < 0f)
            {
                points[(int)Biome.Wetlands] += 1;
                points[(int)Biome.Grasslands] += 1;
            }
            else if (temp < 0.75f)
            {
                points[(int)Biome.Jungle] += 1;
                points[(int)Biome.Desert] += 1;
            }
            else
            {
                points[(int)Biome.Ashlands] += 1;
            }

            // HUMIDITY
            if (humidity < -0.75f)
            {
                points[(int)Biome.Ashlands] += 1;
                points[(int)Biome.Desert] += 1;
            }
            else if (humidity < 0f)
            {
                points[(int)Biome.Grasslands] += 1;
                points[(int)Biome.Desert] += 1; // ?? MAYBE
            }
            else if (humidity < 0.75f)
            {
                points[(int)Biome.Wetlands] += 1;
                points[(int)Biome.Jungle] += 1;
            }
            else
            {
                points[(int)Biome.Winterlands] += 1;
            }

            int largestPoints = points.Max();
            return points.ToList().IndexOf(largestPoints);

            //if (temp < -0.75f)
            //{
            //    return (int)Biome.Winterlands;
            //}
            //else if (temp < 0f)
            //{
            //    if (humidity > 0.5f)
            //    {
            //        return (int)Biome.Wetlands;
            //    }
            //    else
            //    {
            //        return (int)Biome.Grasslands;
            //    }
            //}
            //else if (temp < 0.75f)
            //{
            //    if (humidity > 0.5f)
            //    {
            //        return (int)Biome.Jungle;
            //    }
            //    else
            //    {
            //        return (int)Biome.Desert;
            //    }
            //}
            //else
            //{
            //    return (int)Biome.Ashlands;
            //}
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
                        heightData[i, j] = (int)(_noise[z].GetNoise(xOffset * settings.Width + i, yOffset * settings.Height + j) * biomeCount * Global.ChunkSize);
                    }
                }

                data.Add(heightData);
            }

            var result = new int[height, width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    result[i, j] = Blend(data[0][i, j], data[1][i, j], OctaveBlendAmount) / Global.ChunkSize;
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
