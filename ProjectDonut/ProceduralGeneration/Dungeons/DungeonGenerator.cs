using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectDonut.ProceduralGeneration.Dungeons.DungeonGenerator;

namespace ProjectDonut.ProceduralGeneration.Dungeons
{
    public class DungeonGenerator
    {
        private static Random randy = new Random();

        private const int minRoomWidth = 8;
        private const int minRoomHeight = 8;

        private static List<Area> areas;
        private static List<Room> rooms2;

        public static char[,] Generate(int width, int height, int minAreaSize)
        {
            var startingArea = new Area(0, 0, width, height, null, minRoomWidth, minRoomHeight);
            areas = new List<Area> { startingArea };

            while (areas.Any(a => a.IsPartitionable))
            {
                var areaToPartition = areas.FirstOrDefault(a => a.IsPartitionable);
                areas.Remove(areaToPartition);
                areas.AddRange(PartitionArea(areaToPartition));
            }

            ReplaceAreasWithRooms(areas);
            var map = CompileTo2DArray(areas, width, height);
            map = LinkBrotherRooms(areas, map);
            return map;
        }

        private static char[,] LinkBrotherRooms(List<Area> areas, char[,] map)
        {
            while (areas.Count > 0)
            {
                var area = areas.First();
                areas.Remove(area);


                var brother = area.brother;

                if (brother != null)
                {
                    areas.Remove(area.brother);

                    var x1 = randy.Next(area.xBottom, area.xTop);
                    var y1 = randy.Next(area.yBottom, area.yTop);

                    var x2 = randy.Next(brother.xBottom, brother.xTop);
                    var y2 = randy.Next(brother.yBottom, brother.yTop);

                    map = DrawCorridor(x1, y1, x2, y2, map);
                }


            }

            return map;
        }

        private static char[,] DrawCorridor(int x1, int y1, int x2, int y2, char[,] map)
        {
            // Check if the coordinates are within the bounds of the map
            if (x1 < 0 || x1 >= map.GetLength(0) || y1 < 0 || y1 >= map.GetLength(1) ||
                x2 < 0 || x2 >= map.GetLength(0) || y2 < 0 || y2 >= map.GetLength(1))
            {
                throw new ArgumentException("Invalid coordinates");
            }

            // Draw a corridor between the two points using the ASCII character '|'
            if (x1 == x2)
            {
                // Vertical corridor
                int minY = Math.Min(y1, y2);
                int maxY = Math.Max(y1, y2);

                for (int y = minY; y <= maxY; y++)
                {
                    map[x1, y] = '#';
                }
            }
            else if (y1 == y2)
            {
                // Horizontal corridor
                int minX = Math.Min(x1, x2);
                int maxX = Math.Max(x1, x2);

                for (int x = minX; x <= maxX; x++)
                {
                    map[x, y1] = '#';
                }
            }
            else
            {
                int minY = Math.Min(y1, y2);
                int maxY = Math.Max(y1, y2);
                int minX = Math.Min(x1, x2);
                int maxX = Math.Max(x1, x2);

                for (int x = minX; x <= maxX; x++)
                {
                    map[x, y1] = '#';
                }

                for (int y = minY; y <= maxY; y++)
                {
                    map[x1, y] = '#';
                }
            }

            return map;
        }


        private static void ReplaceAreasWithRooms(List<Area> areas)
        {
            DungeonGenerator.areas = new List<Area>();

            foreach (var area in areas)
            {
                if (area.Width > minRoomWidth && area.Height > minRoomHeight)
                {
                    DungeonGenerator.areas.Add(CreateRoomInArea(area));
                }
            }
        }

        private static Area CreateRoomInArea(Area area)
        {
            var startX = randy.Next(area.xBottom, area.xTop - minRoomWidth);
            var startY = randy.Next(area.yBottom, area.yTop - minRoomHeight);

            if (startX <= 0 || startY <= 0)
            {
                return area;
            }

            var endX = randy.Next(startX + minRoomWidth, area.xTop);
            var endY = randy.Next(startY + minRoomHeight, area.yTop);

            return new Area(startX, startY, endX, endY, area.brother, minRoomWidth, minRoomHeight);
        }

        private static char[,] CompileTo2DArray(List<Area> areas, int initialWidth, int initialHeight)
        {
            var map = new char[initialWidth, initialHeight];

            for (int i = 0; i < initialWidth; i++)
            {
                for (int j = 0; j < initialHeight; j++)
                {
                    map[i, j] = '_';
                }
            }

            int charCount = 65;

            foreach (var area in areas)
            {
                for (int i = area.xBottom; i < area.xTop; i++)
                {
                    for (int j = area.yBottom; j < area.yTop; j++)
                    {
                        map[i, j] = (char)charCount;
                    }
                }

                charCount++;
            }

            return map;
        }

        private static List<Area> PartitionArea(Area area)
        {
            var results = new List<Area>();

            if (area.Width > area.Height)
            {
                var partitionPoint = randy.Next(area.xBottom + minRoomWidth, area.xTop - minRoomWidth);

                var leftArea = new Area(area.xBottom, area.yBottom, partitionPoint, area.yTop, null, minRoomWidth, minRoomHeight);
                var rightArea = new Area(partitionPoint, area.yBottom, area.xTop, area.yTop, leftArea, minRoomWidth, minRoomHeight);
                leftArea.brother = rightArea;

                results.Add(leftArea);
                results.Add(rightArea);
            }
            else
            {
                var partitionPoint = randy.Next(area.yBottom + minRoomHeight, area.yTop - minRoomHeight);

                var topArea = new Area(area.xBottom, partitionPoint, area.xTop, area.yTop, null, minRoomWidth, minRoomHeight);
                var bottomArea = new Area(area.xBottom, area.yBottom, area.xTop, partitionPoint, topArea, minRoomWidth, minRoomHeight);
                topArea.brother = bottomArea;

                results.Add(topArea);
                results.Add(bottomArea);
            }

            return results;
        }
    }
}
