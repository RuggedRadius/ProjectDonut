using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class WaterGenerator
    {
        private WorldMapSettings settings;

        public WaterGenerator(WorldMapSettings settings)
        {
            this.settings = settings;
        }

        public int[,] CarveRivers(int[,] heightData)
        {
            var width = heightData.GetLength(0);
            var height = heightData.GetLength(1);

            var randy = new Random();

            // Find coast
            //var coastCoords = FindCoastCoords(heightData);
            var coastCoords = FindCoastCoords(heightData);

            if (coastCoords.Count == 0)
            {
                return heightData;
            }

            // Random walk + splinter
            for (int i = 0; i < settings.RiverCount; i++)
            {
                var startindex = randy.Next(0, coastCoords.Count);
                var start = coastCoords[startindex];
                var length = randy.Next(settings.MinLength, settings.MaxLength);

                heightData = CarveRiver(length, start.Item3, start.Item1, start.Item2, heightData);
            }

            //var debug = new DebugMapData(settings);
            //debug.WriteMapData(heightData, "heightData");

            return heightData;
        }

        private int[,] CarveRiver(int length, int startDirection, int startX, int startY, int[,] heightData)
        {
            var width = heightData.GetLength(0);
            var height = heightData.GetLength(1);

            var randy = new Random();
            var bannedDirection = CalculateBannedDirection(startDirection);
            var riverRadius = randy.Next(settings.MinRiverRadius, settings.MaxRiverRadius);

            var originalHeightData = (int[,])heightData.Clone();

            for (int j = 0; j < length; j++)
            {
                if (riverRadius > 1)
                {
                    if (randy.NextDouble() <= settings.RiverRadiusDegradationChance)
                    {
                        riverRadius -= 1;
                    }
                }

                // Fork river by chance
                ForkRiverChance(length, j, startDirection, startX, startY, bannedDirection, heightData, riverRadius);

                // Get new direction
                var direction = GetRandomDirection(bannedDirection);

                // Update new positions
                var newDirections = CalculateNewPositions(direction, startX, startY, width, height);
                startX = newDirections.Item1;
                startY = newDirections.Item2;

                if (originalHeightData[startX, startY] >= settings.MountainHeightMin)
                {
                    return heightData;
                }

                if (originalHeightData[startX, startY] <= settings.DeepWaterHeightMax)
                {
                    return heightData;
                }

                for (int x = -riverRadius; x <= riverRadius; x++)
                {
                    for (int y = -riverRadius; y <= riverRadius; y++)
                    {
                        var xCoord = startX + x;
                        var yCoord = startY + y;

                        if (xCoord < 0 || xCoord >= width || yCoord < 0 || yCoord >= height)
                        {
                            continue;
                        }

                        heightData[startX + x, startY + y] = settings.WaterHeightMin;
                    }
                }
            }

            return heightData;
        }

        private int CalculateBannedDirection(int startDirection)
        {
            var bannedDirection = 0;

            if (startDirection == 0)
                return 2;
            if (startDirection == 1)
                return 3;
            if (startDirection == 2)
                return 0;
            if (startDirection == 3)
                return 1;

            return bannedDirection;
        }

        private (int, int) CalculateNewPositions(int direction, int startX, int startY, int width, int height)
        {
            switch (direction)
            {
                case 0:
                    if (startX + 1 < width)
                    {
                        startX += 1;
                    }
                    break;
                case 1:
                    if (startX - 1 >= 0)
                    {
                        startX -= 1;
                    }
                    break;
                case 2:
                    if (startY + 1 < height)
                    {
                        startY += 1;
                    }
                    break;
                case 3:
                    if (startY - 1 >= 0)
                    {
                        startY -= 1;
                    }
                    break;
            }

            return (startX, startY);
        }

        private int GetRandomDirection(int bannedDirection)
        {
            var randy = new Random();

            var direction = randy.Next(0, 4);
            int counter = 0;
            var suitableDirectionFound = false;
            while (suitableDirectionFound == false)
            {
                direction = randy.Next(0, 4);

                if (direction != bannedDirection)
                {
                    suitableDirectionFound = true;
                }

                counter++;

                if (counter > 1000)
                {
                    continue;
                }
            }

            return direction;
        }

        private void ForkRiverChance(
            int length,
            int currentLength,
            int startDirection,
            int startX,
            int startY,
            int bannedDirection,
            int[,] heightData,
            int riverRadius)
        {
            var width = heightData.GetLength(0);
            var height = heightData.GetLength(1);

            var randy = new Random();

            if (randy.NextDouble() <= settings.RiverForkChance && (length - currentLength) > settings.MinForkLength)
            {
                var forkLength = randy.Next(settings.MinForkLength, length - currentLength);
                var forkDirection = randy.Next(0, 4);
                int forkCounter = 0;
                var suitableForkDirectionFound = false;

                while (suitableForkDirectionFound == false)
                {
                    forkDirection = randy.Next(0, 4);

                    if (forkDirection != bannedDirection)
                    {
                        suitableForkDirectionFound = true;
                    }

                    forkCounter++;

                    if (forkCounter > 1000)
                    {
                        continue;
                    }
                }

                heightData = CarveRiver(forkLength, forkDirection, startX, startY, heightData);
            }
        }

        private List<(int, int, int)> FindCoastCoords(int[,] heightData)
        {
            var width = heightData.GetLength(0);
            var height = heightData.GetLength(1);

            var coords = new List<(int, int, int)>();

            var waterCoords = GetAllWaterCoords(heightData);

            foreach (var waterCoord in waterCoords)
            {
                var x = waterCoord.Item1;
                var y = waterCoord.Item2;

                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    continue;
                }

                var n = heightData[x + 0, y - 1] >= settings.GroundHeightMin;
                var w = heightData[x + 0, y + 1] >= settings.GroundHeightMin;
                var e = heightData[x + 1, y + 0] >= settings.GroundHeightMin;
                var s = heightData[x - 1, y + 0] >= settings.GroundHeightMin;

                if (n)
                {
                    coords.Add((x, y, 0));
                    continue;
                }
                if (e)
                {
                    coords.Add((x, y, 1));
                    continue;
                }
                if (s)
                {
                    coords.Add((x, y, 2));
                    continue;
                }
                if (w)
                {
                    coords.Add((x, y, 3));
                    continue;
                }
            }

            return coords;
        }

        private List<(int, int)> GetAllDeepWaterCoords(int[,] heightData)
        {
            var coords = new List<(int, int)>();
            var width = heightData.GetLength(0);
            var height = heightData.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (heightData[x, y] <= settings.DeepWaterHeightMax && heightData[x, y] >= settings.DeepWaterHeightMin)
                    {
                        coords.Add((x, y));
                    }
                }
            }

            return coords;
        }

        private List<(int, int)> GetAllWaterCoords(int[,] heightData)
        {
            var coords = new List<(int, int)>();
            var width = heightData.GetLength(0);
            var height = heightData.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (heightData[x, y] <= settings.WaterHeightMax && heightData[x, y] >= settings.WaterHeightMin)
                    {
                        coords.Add((x, y));
                    }
                }
            }

            return coords;
        }

        private List<(int, int)> GetAllMountainCoords(int[,] heightData)
        {
            var coords = new List<(int, int)>();
            var width = heightData.GetLength(0);
            var height = heightData.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (heightData[x, y] <= settings.MountainHeightMax && heightData[x, y] >= settings.MountainHeightMin)
                    {
                        coords.Add((x, y));
                    }
                }
            }

            return coords;
        }

        private int IsCoastCoord(int x, int y, int[,] heightData)
        {
            if (x == 0 || x == heightData.GetLength(0) - 1 || y == 0 || y == heightData.GetLength(1) - 1)
                return -1;

            var isCoastNorth = heightData[x, y - 1] <= settings.GroundHeightMin;
            var isCoastEast = heightData[x + 1, y] <= settings.GroundHeightMin;
            var isCoastSouth = heightData[x - 1, y] <= settings.GroundHeightMin;
            var isCoastWest = heightData[x, y + 1] <= settings.GroundHeightMin;

            if (isCoastNorth)
                return 0;
            if (isCoastEast)
                return 1;
            if (isCoastSouth)
                return 2;
            if (isCoastWest)
                return 3;
            else
                return -1;
        }

        public int[,] ErodeCoast(int[,] heightData)
        {
            var width = heightData.GetLength(0);
            var height = heightData.GetLength(1);

            var coastCoords = FindCoastCoords(heightData);

            var randy = new Random();

            foreach (var coord in coastCoords)
            {
                var x = coord.Item1;
                var y = coord.Item2;

                var erodeCount = randy.Next(settings.CoastErosionMin, settings.CoastErosionMax);
                for (int i = 0; i < erodeCount; i++)
                {
                    if (x < 1 || x >= width - 1 || y < 1 || y >= height - 1)
                    {
                        continue;
                    }

                    var erodeDirection = randy.Next(0, 4);
                    var erodeFactor = randy.Next(settings.DeepWaterErosionWidthMin, settings.DeepWaterErosionWidthMax);
                    switch (erodeDirection)
                    {
                        case 0:
                            //ErodeTile(x, y - 1, erodeFactor, settings.WaterHeightMin, heightData);
                            heightData[x + 0, y - 1] = settings.WaterHeightMin;
                            y--;
                            break;

                        case 1:
                            //ErodeTile(x + 1, y, erodeFactor, settings.WaterHeightMin, heightData);
                            heightData[x + 1, y + 0] = settings.WaterHeightMin;
                            x++;
                            break;

                        case 2:
                            //ErodeTile(x, y + 1, erodeFactor, settings.WaterHeightMin, heightData);
                            heightData[x + 0, y + 1] = settings.WaterHeightMin;
                            y++;
                            break;

                        case 3:
                            //ErodeTile(x - 1, y, erodeFactor, settings.WaterHeightMin, heightData);
                            heightData[x - 1, y + 0] = settings.WaterHeightMin;
                            x--;
                            break;

                        default:
                            break;
                    }
                }
            }

            return heightData;
        }

        private void ErodeTile(int x, int y, int radius, int targetValue, int[,] heightData)
        {
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    try
                    {
                        heightData[x + i, y + j] = targetValue;
                    }
                    catch (Exception ex) {}
                }
            }
        }

        public int[,] ErodeDeepWater(int[,] heightData)
        {
            var width = heightData.GetLength(0);
            var height = heightData.GetLength(1);

            var coastCoords = GetAllDeepWaterCoords(heightData);

            var randy = new Random();

            foreach (var coord in coastCoords)
            {
                var x = coord.Item1;
                var y = coord.Item2;

                var erodeCount = randy.Next(settings.DeepWaterErosionMin, settings.DeepWaterErosionMax);
                for (int i = 0; i < erodeCount; i++)
                {
                    if (x < 1 || x >= width - 1 || y < 1 || y >= height - 1)
                    {
                        continue;
                    }

                    var erodeDirection = new Random().Next(0, 4);
                    switch (erodeDirection)
                    {
                        case 0:
                            heightData[x + 0, y - 1] = settings.DeepWaterHeightMin;
                            y--;
                            break;

                        case 1:
                            heightData[x + 1, y + 0] = settings.DeepWaterHeightMin;
                            x++;
                            break;

                        case 2:
                            heightData[x + 0, y + 1] = settings.DeepWaterHeightMin;
                            y++;
                            break;

                        case 3:
                            heightData[x - 1, y + 0] = settings.DeepWaterHeightMin;
                            x--;
                            break;

                        default:
                            break;
                    }
                }
            }

            return heightData;
        }

        public int[,] ErodeMountains(int[,] heightData)
        {
            var width = heightData.GetLength(0);
            var height = heightData.GetLength(1);

            var coastCoords = GetAllMountainCoords(heightData);

            var randy = new Random();

            foreach (var coord in coastCoords)
            {
                var x = coord.Item1;
                var y = coord.Item2;

                var erodeCount = randy.Next(settings.CoastErosionMin, settings.CoastErosionMax);
                for (int i = 0; i < erodeCount; i++)
                {
                    if (x < 1 || x >= width - 1 || y < 1 || y >= height - 1)
                    {
                        continue;
                    }

                    var erodeDirection = new Random().Next(0, 4);
                    switch (erodeDirection)
                    {
                        case 0:
                            heightData[x + 0, y - 1] = settings.MountainHeightMax;
                            y--;
                            break;

                        case 1:
                            heightData[x + 1, y + 0] = settings.MountainHeightMax;
                            x++;
                            break;

                        case 2:
                            heightData[x + 0, y + 1] = settings.MountainHeightMax;
                            y++;
                            break;

                        case 3:
                            heightData[x - 1, y + 0] = settings.MountainHeightMax;
                            x--;
                            break;

                        default:
                            break;
                    }
                }
            }

            return heightData;
        }

        public int[,] ErodeBiomeBorder(Biome biome, int[,] biomeData)
        {
            var width = biomeData.GetLength(0);
            var height = biomeData.GetLength(1);

            var coords = GetBiomeCoordsList(biome, biomeData);
            int targetValue = (int)biome;

            var randy = new Random();

            foreach (var coord in coords)
            {
                var x = coord.Item1;
                var y = coord.Item2;

                var erodeCount = randy.Next(settings.BiomeErosionMin, settings.BiomeErosionMax);
                for (int i = 0; i < erodeCount; i++)
                {
                    if (x < 1 || x >= width - 1 || y < 1 || y >= height - 1)
                    {
                        continue;
                    }

                    var erodeDirection = new Random().Next(0, 4);
                    switch (erodeDirection)
                    {
                        case 0:
                            biomeData[x + 0, y - 1] = targetValue;
                            y--;
                            break;

                        case 1:
                            biomeData[x + 1, y + 0] = targetValue;
                            x++;
                            break;

                        case 2:
                            biomeData[x + 0, y + 1] = targetValue;
                            y++;
                            break;

                        case 3:
                            biomeData[x - 1, y + 0] = targetValue;
                            x--;
                            break;

                        default:
                            break;
                    }
                }
            }

            return biomeData;
        }

        public List<(int, int)> GetBiomeCoordsList(Biome biome, int[,] biomeData)
        {
            int targetValue = (int)biome;
            var width = biomeData.GetLength(0);
            var height = biomeData.GetLength(1);

            var coords = new List<(int, int)>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (biomeData[x, y] == targetValue)
                    {
                        coords.Add((x, y));
                    }
                }
            }

            return coords;
        }
    }
}
