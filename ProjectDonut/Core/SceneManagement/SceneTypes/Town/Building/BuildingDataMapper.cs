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
        public static int[,] GenerateFloorDataMap(Rectangle buildingBounds, List<Rectangle> roomBounds)
        {
            var map = new int[buildingBounds.Width, buildingBounds.Height];

            foreach (var room in roomBounds)
            {
                var offsetX = room.X - buildingBounds.X;
                var offsetY = room.Y - buildingBounds.Y;

                for (int i = room.X; i < room.Width; i++)
                {
                    for (int j = room.Y; j < room.Height; j++)
                    {
                        map[i + offsetX, j + offsetY] = 1;
                    }
                }
            }

            return map;
        }

        public static int[,] GenerateWallDataMap(Rectangle buildingBounds, List<Rectangle> roomBounds)
        {
            var map = new int[buildingBounds.Width, buildingBounds.Height];

            foreach (var room in roomBounds)
            {
                var offsetX = room.X - buildingBounds.X;
                var offsetY = room.Y - buildingBounds.Y;

                for (int i = room.X; i < room.Width; i++)
                {
                    for (int j = room.Y; j < room.Height; j++)
                    {
                        if (i == room.X || i == room.Width - 1 || j == room.Y || j == room.Height - 1)
                        {
                            map[i + offsetX, j + offsetY] = 1;
                        }
                        else
                        {
                            map[i + offsetX, j + offsetY] = 0;
                        }
                    }
                }
            }

            return map;
        }

        //public static int[,] GenerateWallDataMap(Rectangle buildingBounds, List<Rectangle> roomBounds)
        //{
        //    var map = new int[buildingBounds.Width, buildingBounds.Height];

        //    foreach (var room in roomBounds)
        //    {
        //        var startXPos = room.X;
        //        var startYPos = room.Y;

        //        var endXPos = room.X + room.Width;
        //        var endYPos = room.Y + room.Height;

        //        var offsetX = room.X - buildingBounds.X;
        //        var offsetY = room.Y - buildingBounds.Y;

        //        for (int i = startXPos; i < endXPos; i++)
        //        {
        //            for (int j = startYPos; j < endYPos; j++)
        //            {
        //                if (i == startXPos || i == endXPos - 1 || j == startYPos || j == endYPos - 1)
        //                {
        //                    map[i + offsetX, j + offsetY] = 1;
        //                }
        //                else
        //                {
        //                    map[i + offsetX, j + offsetY] = 0;
        //                }
        //            }
        //        }
        //    }

        //    return map;
        //}

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

        public static int[,] GenerateRoofDataMap(Rectangle plotBounds, List<Rectangle> roomBounds)
        {
            var map = new int[plotBounds.Width, plotBounds.Height];

            foreach (var room in roomBounds)
            {
                var startXPos = room.X / Global.TileSize;
                var startYPos = room.Y / Global.TileSize;

                var endXPos = (room.X + room.Width - 1) / Global.TileSize;
                var endYPos = (room.Y + room.Height - 1) / Global.TileSize;

                for (int i = startXPos; i < endXPos; i++)
                {
                    for (int j = startYPos; j < endYPos; j++)
                    {
                        map[i, j] = 1;
                    }
                }
            }

            return map;
        }
    }
}
