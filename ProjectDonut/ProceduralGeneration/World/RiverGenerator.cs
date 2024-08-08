using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class RiverGenerator
    {
        private int[,] heightData;
        private WorldMapSettings settings;

        public RiverGenerator(int[,] heightData, WorldMapSettings settings)
        {
            this.heightData = heightData;
            this.settings = settings;
        }

        public int[,] CarveRivers(int width, int height)
        {
            int riverCount = 50;
            int minLength = 50;
            int maxLength = 500;

            var randy = new Random();

            // Find coast
            var coastCoords = FindCoastCoords(width, height);

            // Random walk + splinter
            for (int i = 0; i < riverCount; i++)
            {
                var startindex = randy.Next(0, coastCoords.Count);
                var start = coastCoords[startindex];
                var length = randy.Next(minLength, maxLength);

                CarveRiver(width, height, length, start.Item3, start.Item1, start.Item2);
            }

            return heightData;
        }

        private void CarveRiver(int width, int height, int length, int startDirection, int startX, int startY)
        {
            double forkChance = 0.0025f;
            int minForkLength = 5;

            var randy = new Random();

            var bannedDirection = 0;
            if (startDirection == 0)
                bannedDirection = 2;
            if (startDirection == 1)
                bannedDirection = 3;
            if (startDirection == 2)
                bannedDirection = 0;
            if (startDirection == 3)
                bannedDirection = 1;

            for (int j = 0; j < length; j++)
            {
                if (randy.NextDouble() <= forkChance && (length - j) > minForkLength)
                {
                    var forkLength = randy.Next(minForkLength, length - j);
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

                    CarveRiver(width, height, forkLength, forkDirection, startX, startY);
                }

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

                if (heightData[startX, startY] > settings.WaterHeightMax)
                {
                    if (heightData[startX, startY] >= settings.MountainHeightMin)
                    {
                        return;
                    }

                    heightData[startX, startY] = settings.WaterHeightMax;
                }
            }
        }

        private List<(int, int, int)> FindCoastCoords(int width, int height)
        {
            var coords = new List<(int, int, int)>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (heightData[x, y] <= settings.WaterHeightMax)
                    {
                        var direction = IsCoastCoord(x, y);

                        if (direction >= 0)
                        {
                            coords.Add((x, y, direction));
                        }
                    }
                }
            }

            return coords;
        }

        private int IsCoastCoord(int x, int y)
        {
            if (x == 0 || x == heightData.GetLength(0) - 1 || y == 0 || y == heightData.GetLength(1) - 1)
                return -1;

            var isCoastNorth = heightData[x, y - 1] > settings.WaterHeightMax;
            var isCoastEast = heightData[x + 1, y] > settings.WaterHeightMax;
            var isCoastSouth = heightData[x - 1, y] > settings.WaterHeightMax;
            var isCoastWest = heightData[x, y + 1] > settings.WaterHeightMax;

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
    }
}
