using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using ProjectDonut.Core;

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

        private ConcurrentDictionary<(int, int), int> biomeSeeds = new ConcurrentDictionary<(int, int), int>();

        private Random _random = new Random();

        public BiomeGenerator(WorldMapSettings settings)
        {
            this.settings = settings;

            var random = new Random();
            _noise = new FastNoiseLite[3];

            _noise[0] = new FastNoiseLite();
            _noise[0].SetNoiseType(FastNoiseLite.NoiseType.Cellular);
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

            //PlantBiomeSeeds();

            //var biomeCount = Enum.GetValues(typeof(Biome)).Length;
            //for (int i = 0; i < biomeCount; i++)
            //{
            //    var x = _random.Next(0, settings.Width) + _random.Next(-1, 2) * settings.Width;
            //    var y = _random.Next(0, settings.Height) + _random.Next(-1, 2) * settings.Height;

            //    var coords = (x, y);
            //    biomeSeeds.Add(coords, i);
            //}
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
                    var noiseValue = _noise[0].GetNoise(xOffset * settings.Width + i, yOffset * settings.Height + j);
                    result[i, j] = NormalizeToRange(noiseValue);
                }
            }

            return result;
        }

        public static int NormalizeToRange(double value)
        {
            double normalizedValue = (value + 1) / 2 * (Enum.GetValues(typeof (Biome)).Length);
            return (int)Math.Round(normalizedValue, 0);
        }


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

        private void PlantBiomeSeeds()
        {
            int counter = 0;

            var testDistance = 100 * Global.ChunkSize;

            for (int i = -testDistance; i <= testDistance; i += Global.ChunkSize * 5)
            //for (int i = int.MinValue; i <= int.MaxValue; i += Global.ChunkSize)
            {
                for (int j = -testDistance; j <= testDistance; j += Global.ChunkSize * 5)
                //for (int j = int.MinValue; j <= int.MaxValue; j += Global.ChunkSize)
                {
                    //if (_random.Next(0, 100) < 50)
                    //{
                        var biomeCount = Enum.GetNames(typeof(Biome)).Length;
                        var biomeValue = _random.Next(0, biomeCount);

                        var x = i + (_random.Next(0, settings.Width) + _random.Next(-1, 2));
                        var y = j + (_random.Next(0, settings.Height) + _random.Next(-1, 2));

                        var coords = (x, y);

                        biomeSeeds[coords] = biomeValue;

                        counter++;
                    //}
                }
            }

            Debug.WriteLine($"Planted {counter} biome seeds");
        }

        public int[,] GenerateBiomes(int width, int height, int xOffset, int yOffset)
        {
            //var x = _random.Next(0, width) + (xOffset * Global.ChunkSize);
            //var y = _random.Next(0, height) + (yOffset * Global.ChunkSize);

            //var coords = (x, y);
            //var biomeValue = _random.Next(0, Enum.GetValues(typeof(Biome)).Length);

            //if (biomeSeeds.ContainsKey(coords) == false)
            //{
            //    biomeSeeds[coords] = biomeValue;                          
            //}

            int biomeCount = Enum.GetNames(typeof(Biome)).Length;

            var result = new int[height, width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    result[i, j] = GetClosestBiome(i, j, xOffset, yOffset);
                }
            }

            return result;
        }

        int Blend(int a, int b, float t)
        {
            return (int)(a + t * (b - a));
        }

        private int GetClosestBiome(int x, int y, int xOffset, int yOffset)
        {
            int closestBiome = 0;
            double closestDistance = double.MaxValue;

            var biomeClone = new ConcurrentDictionary<(int, int), int>(biomeSeeds);

            var adjustedX = x + (xOffset * Global.ChunkSize);
            var adjustedY = y + (yOffset * Global.ChunkSize);

            foreach (var seed in biomeClone)
            {
                double distance = CalculateEuclideanDistance(adjustedX, adjustedY, seed.Key.Item1, seed.Key.Item2);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBiome = seed.Value;
                }
            }

            return closestBiome;
        }

        public static double CalculateEuclideanDistance(int x1, int y1, int x2, int y2)
        {
            // Calculate the difference in x and y coordinates
            int dx = x2 - x1;
            int dy = y2 - y1;

            // Apply the Euclidean distance formula
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
