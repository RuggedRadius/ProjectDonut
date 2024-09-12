using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town.Building
{
    public class BuildingDataMapper
    {
        private static int _minHouseWidth = 12;
        private static int _minHouseHeight = 8;

        public static Rectangle CalculateHouseBounds(Plot plot)
        {
            var random = new Random();

            var width = random.Next((_minHouseWidth), plot.PlotBounds.Width - 2);
            var height = random.Next(_minHouseHeight, plot.PlotBounds.Height - 2);

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
            var dataMapped = false;

            for (int i = 0; i < plot.PlotBounds.Width; i++)
            {
                for (int j = 0; j < plot.PlotBounds.Height; j++)
                {
                    foreach (var room in roomBounds)
                    {
                        if (room.Contains(plot.PlotBounds.X + i, plot.PlotBounds.Y + j))
                        {
                            map[i, j] = 1;
                            dataMapped = true;
                        }
                    }
                }
            }

            
            //foreach (var room in roomBounds)
            //{
            //    //var offsetX = room.X - buildingBounds.X - 1;
            //    //var offsetY = room.Y - buildingBounds.Y - 1;

            //    for (int i = room.X; i < room.X + room.Width - 1; i++)
            //    {
            //        for (int j = room.Y; j < room.Y + room.Height - 1; j++)
            //        {
            //            map[i, j] = 1;
            //            //map[i + offsetX, j + offsetY] = 1;
            //            dataMapped = true;
            //        }
            //    }
            //}

            if (!dataMapped)
            {
                ;
            }

            return map;
        }

        public static int[,] GenerateWallDataMap(Rectangle b, List<Rectangle> roomBounds)
        {
            // Create an int[,] the same size as rectangle b
            var tileMap = new int[b.X + b.Width, b.Y + b.Height];

            // Loop over each room's bounds
            foreach (var a in roomBounds)
            {
                for (int i = 0; i < a.X + a.Width; i++)
                {
                    for (int j = 0; j < a.Y + a.Height; j++)
                    {
                        if (i == a.Left || i == a.Right || j == a.Top || j == a.Bottom)
                        {
                            tileMap[i, j] = 1;
                        }
                    }
                }
            }


            //    // Find the bounds of rectangle a within rectangle b
            //    int startX = Math.Max(0, a.X - b.X);
            //    int startY = Math.Max(0, a.Y - b.Y);
            //    int endX = Math.Min(b.Width - 1, startX + a.Width - 1);
            //    int endY = Math.Min(b.Height - 1, startY + a.Height - 1);

            //    // Mark the boundaries of rectangle a with 1s, ensuring that we don't go out of bounds
            //    for (int x = startX; x <= endX && x < b.Width; x++)
            //    {
            //        if (startY >= 0 && startY < b.Height) tileMap[startY, x] = 1; // Top boundary
            //        if (endY >= 0 && endY < b.Height) tileMap[endY, x] = 1;       // Bottom boundary
            //    }

            //    for (int y = startY; y <= endY && y < b.Height; y++)
            //    {
            //        if (startX >= 0 && startX < b.Width) tileMap[y, startX] = 1;  // Left boundary
            //        if (endX >= 0 && endX < b.Width) tileMap[y, endX] = 1;        // Right boundary
            //    }
            //}

            return tileMap;
        }



        public static int[,] GenerateStairDataMap(Rectangle plotBounds, List<Rectangle> roomBounds)
        {
            var map = new int[plotBounds.Width, plotBounds.Height];

            //foreach (var room in roomBounds)
            //{
            //    for (int i = room.X; i < room.X + room.Width; i++)
            //    {
            //        for (int j = room.Y; j < room.Y + room.Height; j++)
            //        {
            //            map[i, j] = 1;
            //        }
            //    }
            //}

            return map;
        }

        public static int[,] GenerateRoofDataMap(Plot plot, List<Rectangle> roomBounds)
        {
            var map = new int[plot.PlotBounds.Width, plot.PlotBounds.Height];
            var dataMapped = false;

            for (int i = 0; i < plot.PlotBounds.Width; i++)
            {
                for (int j = 0; j < plot.PlotBounds.Height; j++)
                {
                    foreach (var room in roomBounds)
                    {
                        if (room.Contains(plot.PlotBounds.X + i, plot.PlotBounds.Y + j))
                        {
                            map[i, j] = 1;
                            dataMapped = true;
                        }
                    }
                }
            }

            if (!dataMapped)
            {
                ;
            }

            return map;
        }
    }
}
