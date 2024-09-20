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

        public BiomeGenerator(WorldMapSettings settings)
        {
            this.settings = settings;

            var random = new Random();
            _noise = new FastNoiseLite[3];

            _noise[0] = new FastNoiseLite();
            _noise[0].SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            _noise[0].SetSeed(random.Next(int.MinValue, int.MaxValue));
            _noise[0].SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
            _noise[0].SetCellularJitter(1.01f);
            _noise[0].SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            _noise[0].SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            _noise[0].SetDomainWarpAmp(100.0f);
            _noise[0].SetFrequency(0.003f);
            _noise[0].SetFractalGain(0.75f);
            _noise[0].SetFractalType(FastNoiseLite.FractalType.None);
            _noise[0].SetFractalOctaves(8);
            _noise[0].SetFractalLacunarity(2.0f);

            _noise[1] = new FastNoiseLite();
            _noise[1].SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            _noise[1].SetSeed(random.Next(int.MinValue, int.MaxValue));
            _noise[1].SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Manhattan);
            _noise[1].SetCellularJitter(1.02f);
            _noise[1].SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            _noise[1].SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            _noise[1].SetDomainWarpAmp(100.0f);
            _noise[1].SetFrequency(0.014f);
            _noise[1].SetFractalGain(3.5f);
            _noise[1].SetFractalType(FastNoiseLite.FractalType.None);
            _noise[1].SetFractalOctaves(3);
            _noise[1].SetFractalLacunarity(4.0f);
            _noise[1].SetFractalWeightedStrength(0);

            _noise[2] = new FastNoiseLite();
            _noise[2].SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            _noise[2].SetSeed(random.Next(int.MinValue, int.MaxValue));
            _noise[2].SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
            _noise[2].SetCellularJitter(1.01f);
            _noise[2].SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            _noise[2].SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            _noise[2].SetDomainWarpAmp(100.0f);
            _noise[2].SetFrequency(0.003f);
            _noise[2].SetFractalGain(0.75f);
            _noise[2].SetFractalType(FastNoiseLite.FractalType.None);
            _noise[2].SetFractalOctaves(8);
            _noise[2].SetFractalLacunarity(2.0f);
        }

        public int[,] GenBiomes(int width, int height, int xOffset, int yOffset)
        {
            int biomeCount = Enum.GetNames(typeof(Biome)).Length;

            var heightData = new int[height, width];
            var tempData = new int[height, width];
            var moistureData = new int[height, width];
            var result = new int[height, width];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    heightData[i, j] = (int)(_noise[0].GetNoise(xOffset * settings.Width + i, yOffset * settings.Height + j) * biomeCount);// * Global.ChunkSize);
                    tempData[i, j] = (int)(_noise[1].GetNoise(xOffset * settings.Width + i, yOffset * settings.Height + j) * biomeCount);// * Global.ChunkSize);
                    moistureData[i, j] = (int)(_noise[2].GetNoise(xOffset * settings.Width + i, yOffset * settings.Height + j) * biomeCount);// * Global.ChunkSize);
                }
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    result[i, j] = DetermineBiome(heightData[i, j], tempData[i, j], moistureData[i, j]);
                }
            }

            return result;
        }

        // CHATGPT:
        //private int DetermineBiome(int height, int temp, int humidity)
        //{
        //    if (temp > 70) // Hot regions
        //    {
        //        if (humidity < 30)
        //        {
        //            return (int)Biome.Desert; // Hot and dry
        //        }
        //        else if (humidity > 70)
        //        {
        //            return (int)Biome.Wetlands; // Hot and humid
        //        }
        //        else
        //        {
        //            return (int)Biome.Ashlands; // Hot but moderate humidity, maybe volcanic terrain
        //        }
        //    }
        //    else if (temp < 30) // Cold regions
        //    {
        //        if (humidity < 30)
        //        {
        //            return (int)Biome.Winterlands; // Cold and dry
        //        }
        //        else if (humidity > 70)
        //        {
        //            return (int)Biome.Wetlands; // Cold and wet (perhaps snowy wetlands)
        //        }
        //        else
        //        {
        //            return (int)Biome.Plains; // Cold with moderate humidity
        //        }
        //    }
        //    else // Temperate regions
        //    {
        //        if (humidity < 30)
        //        {
        //            return (int)Biome.Plains; // Temperate but dry
        //        }
        //        else if (humidity > 70)
        //        {
        //            return (int)Biome.Wetlands; // Temperate and humid
        //        }
        //        else
        //        {
        //            if (height > 50) // Higher regions might be grasslands
        //            {
        //                return (int)Biome.Grasslands;
        //            }
        //            else
        //            {
        //                return (int)Biome.Plains; // Lower temperate regions
        //            }
        //        }
        //    }
        //}

        private int DetermineBiome(int height, int temp, int humidity)
        {
            if (height < 20)
            {
                if (temp < 20)
                {
                    return (int)Biome.Winterlands;
                }
                else if (humidity > 50)
                {
                    return (int)Biome.Wetlands;
                }
                else
                {
                    return (int)Biome.Grasslands;
                }
            }
            else if (height < 50)
            {
                if (temp > 70)
                {
                    return (int)Biome.Desert;
                }
                else if (humidity < 30)
                {
                    return (int)Biome.Plains;
                }
                else
                {
                    return (int)Biome.Grasslands;
                }
            }
            else
            {
                if (temp < 30)
                {
                    return (int)Biome.Winterlands;
                }
                else if (humidity < 20)
                {
                    return (int)Biome.Ashlands;
                }
                else
                {
                    return (int)Biome.Plains;
                }
            }
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
