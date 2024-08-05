using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BSPDungeon.BSP2;

namespace BSPDungeon
{


    public class BSP2
    {
        private static Random randy = new Random();

        private const int minRoomWidth = 8;
        private const int minRoomHeight = 8;

        public static char[,] Generate(int width, int height, int minAreaSize)
        {
            var startingArea = new Area(0, 0, width, height, null);
            var areas = new List<Area> { startingArea };

            while (areas.Any(a => a.IsPartitionable))
            {
                var areaToPartition = areas.FirstOrDefault(a => a.IsPartitionable);
                areas.Remove(areaToPartition);
                areas.AddRange(PartitionArea(areaToPartition));
            }

            var rooms = ReplaceAreasWithRooms(areas);
            var map = CompileTo2DArray(rooms, width, height);
            map = LinkBrotherRooms(rooms, map);
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


        private static List<Area> ReplaceAreasWithRooms(List<Area> areas)
        {
            var rooms = new List<Area>();

            foreach (var area in areas)
            {
                if (area.Width > minRoomWidth && area.Height > minRoomHeight)
                {
                    rooms.Add(CreateRoomInArea(area));
                }
            }

            return rooms;
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

            return new Area(startX, startY, endX, endY, area.brother);
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

                var leftArea = new Area(area.xBottom, area.yBottom, partitionPoint, area.yTop, null);
                var rightArea = new Area(partitionPoint, area.yBottom, area.xTop, area.yTop, leftArea);
                leftArea.brother = rightArea;

                results.Add(leftArea);
                results.Add(rightArea);
            }
            else
            {
                var partitionPoint = randy.Next(area.yBottom + minRoomHeight, area.yTop - minRoomHeight);

                var topArea = new Area(area.xBottom, partitionPoint, area.xTop, area.yTop, null);
                var bottomArea = new Area(area.xBottom, area.yBottom, area.xTop, partitionPoint, topArea);
                topArea.brother = bottomArea;

                results.Add(topArea);
                results.Add(bottomArea);
            }

            return results;
        }

        public class Area
        {
            public int xTop { get; set; }
            public int yTop { get; set; }
            public int xBottom { get; set; }
            public int yBottom { get; set; }

            public Area brother { get; set; }

            public int Width { get { return xTop - xBottom; } }
            public int Height { get { return yTop - yBottom; } }
            public bool IsPartitionable
            {
                get
                {
                    return Width > (2 * minRoomWidth) && (Height > 2 * minRoomHeight);
                }
            }

            public Area(int xBottom, int yBottom, int xTop, int yTop, Area brother)
            {
                this.xTop = xTop;
                this.yTop = yTop;
                this.xBottom = xBottom;
                this.yBottom = yBottom;
                this.brother = brother;
            }
        }
    }
}
