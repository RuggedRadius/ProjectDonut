using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Content.Tiled;
using ProjectDonut.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectDonut.Core.Global;

namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town.Building
{
    public class BuildingDataMapper
    {
        public static Rectangle CalculateHouseBounds(Plot plot, Vector2 minHouseSize)
        {
            var random = new Random();

            var width = random.Next((int)minHouseSize.X, plot.PlotBounds.Width - 2);
            var height = random.Next((int)minHouseSize.Y, plot.PlotBounds.Height - 2);

            var offsetX = random.Next(1, plot.PlotBounds.Width - width - 1);
            var offsetY = random.Next(1, plot.PlotBounds.Height - height - 1);

            return new Rectangle(
                (int)((plot.WorldPosition.X / Global.TileSize) + offsetX),
                (int)((plot.WorldPosition.Y / Global.TileSize) + offsetY),
                width,
                height);
        }

        // TODO: Need to factor in the presence of stairs?
        public static int[,] GenerateFloorDataMap(Plot plot, List<Rectangle> roomBounds)
        {
            var map = new int[plot.PlotBounds.Width, plot.PlotBounds.Height];

            for (int i = 0; i < plot.PlotBounds.Width; i++)
            {
                for (int j = 0; j < plot.PlotBounds.Height; j++)
                {
                    foreach (var room in roomBounds)
                    {
                        //var adjustedRoomBounds = new Rectangle(
                        //    room.X, room.Y, room.Width + 1, room.Height);

                        var adjustedRoomBounds = new Rectangle(
                            room.X + 1, 
                            room.Y + 1, 
                            room.Width - 1, 
                            room.Height - 1);

                        if (adjustedRoomBounds.Contains(plot.PlotBounds.X + i, plot.PlotBounds.Y + j))
                        {
                            map[i, j] = 1;
                        }
                    }
                }
            }

            //DebugMapData.WriteMapData(map, $"{plot.WorldPosition.X}-{plot.WorldPosition.Y}_floorDataMap");

            return map;
        }

        public static int[,] GenerateRoomDataMap(Plot plot, Rectangle roomBounds)
        {
            var map = new int[plot.PlotBounds.Width, plot.PlotBounds.Height];


            for (int i = 0; i < plot.PlotBounds.Width; i++)
            {
                for (int j = 0; j < plot.PlotBounds.Height; j++)
                {
                    var adjustedRoomBounds = new Rectangle(roomBounds.X, roomBounds.Y, roomBounds.Width + 1, roomBounds.Height);

                    if (adjustedRoomBounds.Contains(plot.PlotBounds.X + i, plot.PlotBounds.Y + j))
                    {
                        map[i, j] = 1;
                    }
                }
            }

            //DebugMapData.WriteMapData(map, $"{plot.WorldPosition.X}-{plot.WorldPosition.Y}_floorDataMap");

            return map;
        }

        public static int[,] GenerateRoomDataMap2(Plot plot, Rectangle roomBounds)
        {
            var map = new int[
                (int)plot.Town.MapSize.X,
                (int)plot.Town.MapSize.Y];


            for (int i = 0; i < plot.PlotBounds.Width; i++)
            {
                for (int j = 0; j < plot.PlotBounds.Height; j++)
                {
                    var adjustedRoomBounds = new Rectangle(roomBounds.X, roomBounds.Y, roomBounds.Width + 1, roomBounds.Height);

                    if (adjustedRoomBounds.Contains(plot.PlotBounds.X + i, plot.PlotBounds.Y + j))
                    {
                        map[
                            (int)(plot.WorldPosition.X / Global.TileSize / Global.TileSize) + i,
                            (int)(plot.WorldPosition.Y / Global.TileSize / Global.TileSize) + j] = 1;
                    }
                }
            }

            //DebugMapData.WriteMapData(map, $"{plot.WorldPosition.X}-{plot.WorldPosition.Y}_floorDataMap");

            return map;
        }


        public static int[,] GenerateWallDataMap(Plot plot, List<Rectangle> roomBounds)
        {
            var map = new int[
                plot.PlotBounds.Width,
                plot.PlotBounds.Height];

            foreach (var room in roomBounds)
            {
                for (int i = room.Left; i <= room.Right; i++)
                {
                    for (int j = room.Top; j <= room.Bottom; j++)
                    {
                        if (i == room.Left || i == room.Right || j == room.Top || j == room.Bottom)
                        {
                            map[i - plot.PlotBounds.X, j - plot.PlotBounds.Y] = 1;
                        }
                    }
                }
            }

            //DebugMapData.WriteMapData(map, $"{plot.WorldPosition.X}-{plot.WorldPosition.Y}_wallDataMap");

            return map;
        }

        public static int[,] GenerateRoomWallDataMap(Plot plot, Rectangle roomBounds)
        {
            var map = new int[
                plot.PlotBounds.Width,
                plot.PlotBounds.Height];

            for (int i = roomBounds.Left; i <= roomBounds.Right; i++)
            {
                for (int j = roomBounds.Top; j <= roomBounds.Bottom; j++)
                {
                    if (i == roomBounds.Left || i == roomBounds.Right || j == roomBounds.Top || j == roomBounds.Bottom)
                    {
                        map[i - plot.PlotBounds.X, j - plot.PlotBounds.Y] = 1;
                    }
                }
            }

            //DebugMapData.WriteMapData(map, $"{plot.WorldPosition.X}-{plot.WorldPosition.Y}_wallDataMap");

            return map;
        }

        //public static int[,] GenerateRoomWallDataMap2(Plot plot, Rectangle roomBounds)
        //{
        //    var map = new int[
        //        (int)plot.Town.MapSize.X,
        //        (int)plot.Town.MapSize.Y];

        //    for (int i = roomBounds.Left; i <= roomBounds.Right; i++)
        //    {
        //        for (int j = roomBounds.Top; j <= roomBounds.Bottom; j++)
        //        {
        //            if (i == roomBounds.Left || i == roomBounds.Right || j == roomBounds.Top || j == roomBounds.Bottom)
        //            {
        //                map[
        //                    (int)(plot.WorldPosition.X / Global.TileSize / Global.TileSize) + i - plot.PlotBounds.X,
        //                    (int)(plot.WorldPosition.Y / Global.TileSize / Global.TileSize) + j - plot.PlotBounds.Y] 
        //                    = 1;
        //            }
        //        }
        //    }

        //    //DebugMapData.WriteMapData(map, $"{plot.WorldPosition.X}-{plot.WorldPosition.Y}_wallDataMap");

        //    return map;
        //}


        //public static int[,] GenerateWallCapDataMap(Plot plot, List<Rectangle> roomBounds)
        //{
        //    var map = new int[
        //        plot.PlotBounds.Width,
        //        plot.PlotBounds.Height];

        //    foreach (var room in roomBounds)
        //    {
        //        for (int i = room.Left; i <= room.Right; i++)
        //        {
        //            for (int j = room.Top; j <= room.Bottom; j++)
        //            {
        //                if (i == room.Left || i == room.Right)
        //                {
        //                    map[i - plot.PlotBounds.X, j - plot.PlotBounds.Y] = 1;
        //                }

        //                if (j == room.Top || j == room.Bottom)
        //                {
        //                    map[i - plot.PlotBounds.X, j - plot.PlotBounds.Y] = 1;
        //                }
        //            }
        //        }
        //    }

        //    //DebugMapData.WriteMapData(map, $"{plot.WorldPosition.X}-{plot.WorldPosition.Y}_wallCapDataMap");

        //    return map;
        //}



        public static int[,] GenerateStairsDataMap(Plot plot, List<Rectangle> roomBounds)
        {
            var map = new int[plot.PlotBounds.Width, plot.PlotBounds.Height];
            var randomRoomRect = roomBounds[new Random().Next(roomBounds.Count)];

            map[
                randomRoomRect.Left - plot.PlotBounds.X + 1, 
                randomRoomRect.Top - plot.PlotBounds.Y]
                = 1;

            return map;
        }

        public static int[,] GenerateRoofDataMap(Plot plot, List<Rectangle> roomBounds)
        {
            var map = new int[plot.PlotBounds.Width, plot.PlotBounds.Height];

            for (int i = 0; i < plot.PlotBounds.Width; i++)
            {
                for (int j = 0; j < plot.PlotBounds.Height; j++)
                {
                    foreach (var room in roomBounds)
                    {
                        var adjustedRoomBounds = new Rectangle(
                            room.X, room.Y, room.Width + 1, room.Height);

                        if (adjustedRoomBounds.Contains(plot.PlotBounds.X + i, plot.PlotBounds.Y + j))
                        {
                            map[i, j] = 1;
                        }
                    }
                }
            }

            //DebugMapData.WriteMapData(map, $"{plot.WorldPosition.X}-{plot.WorldPosition.Y}_floorDataMap");

            return map;
            //var map = new int[plot.PlotBounds.Width, plot.PlotBounds.Height];

            //for (int i = 0; i < plot.PlotBounds.Width; i++)
            //{
            //    for (int j = 0; j < plot.PlotBounds.Height; j++)
            //    {
            //        foreach (var room in roomBounds)
            //        {
            //            if (room.Contains(plot.PlotBounds.X + i, plot.PlotBounds.Y + j))
            //            {
            //                map[i + 1, j] = 1;
            //            }

            //            if (room.Contains(plot.PlotBounds.X + i + 1, plot.PlotBounds.Y + j))
            //            {
            //                map[i + 1, j] = 1;
            //            }
            //        }
            //    }
            //}

            //return map;
        }


        #region Room Linkage
        private static List<List<(int, int)>> _paths;
        public static int[,] LinkAllRooms(List<(Rectangle, Rectangle)> rooms, int[,] datamap)
        {
            _paths = new List<List<(int, int)>>();
            int width = datamap.GetLength(0);
            int height = datamap.GetLength(1);
            var canvas = new int[width, height];

            foreach (var roomPair in rooms)
            {
                var curRoomLink = LinkRooms(roomPair, datamap);
                canvas = MergeArrays(canvas, curRoomLink);
            }

            // Carve all paths
            foreach (var path in _paths)
            {
                foreach (var step in path)
                {
                    canvas[step.Item1, step.Item2] = 2;
                }
            }

            return canvas;
        }

        public static int[,] LinkRooms((Rectangle, Rectangle) rooms, int[,] datamap)
        {
            var random = new Random();
            int width = datamap.GetLength(0);
            int height = datamap.GetLength(1);
            var canvas = new int[width, height];
            var path = new List<(int, int)>();
            var attemptCounter = 0;

            do
            {
                var aX = random.Next(rooms.Item1.X + 1, rooms.Item1.X + rooms.Item1.Width - 1);
                var aY = random.Next(rooms.Item1.Y + 1, rooms.Item1.Y + rooms.Item1.Height - 1);
                var bX = random.Next(rooms.Item2.X + 1, rooms.Item2.X + rooms.Item2.Width - 1);
                var bY = random.Next(rooms.Item2.Y + 1, rooms.Item2.Y + rooms.Item2.Height - 1);

                path = CalculatePath(datamap, aX, aY, bX, bY);
                attemptCounter++;

                if (attemptCounter > 100)
                {
                    break;
                }
            }
            while (!TestPathDoesntCollide(datamap, path));

            canvas = WallAroundPath(datamap, canvas, path);

            _paths.Add(path);
            return canvas;
        }

        public static bool TestPathDoesntCollide(int[,] datamap, List<(int, int)> path)
        {
            foreach (var step in path)
            {
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        if (datamap[step.Item1 + x, step.Item2 + y] != 0)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static List<(int, int)> CalculatePath(int[,] datamap, int aX, int aY, int bX, int bY)
        {
            var deltaX = bX - aX;
            var deltaY = bY - aY;

            var path = new List<(int, int)>();

            // Gather path horizontal corridor
            if (deltaX != 0)
            {
                int stepX = deltaX > 0 ? 1 : -1;
                for (int x = 0; x != deltaX; x += stepX)
                {
                    if (datamap[aX + x, aY] == 2)
                        continue;

                    path.Add((aX + x, aY));
                }
            }

            // Gather path vertical corridor
            if (deltaY != 0)
            {
                int stepY = deltaY > 0 ? 1 : -1;
                for (int y = 0; y != deltaY; y += stepY)
                {
                    if (datamap[aX + deltaX, aY + y] == 2)
                        continue;

                    path.Add((aX + deltaX, aY + y));
                }
            }

            return path;
        }

        public static int[,] WallAroundPath(int[,] datamap, int[,] canvas, List<(int, int)> path)
        {
            foreach (var step in path)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        var xCoord = step.Item1 + x;
                        var yCoord = step.Item2 + y;

                        if (datamap[xCoord, yCoord] == 0)
                        {
                            canvas[xCoord, yCoord] = 1;
                        }
                    }
                }
            }

            return canvas;
        }

        public static int[,] MergeArrays(int[,] array1, int[,] array2)
        {
            int rows = array1.GetLength(0);
            int cols = array1.GetLength(1);

            if (rows != array2.GetLength(0) || cols != array2.GetLength(1))
            {
                throw new ArgumentException("Both arrays must have the same dimensions.");
            }

            int[,] result = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (array2[i, j] == 0)
                    {
                        result[i, j] = array1[i, j];
                    }
                    else
                    {
                        result[i, j] = array2[i, j];
                    }
                }
            }

            return result;
        }
        #endregion
    }
}
