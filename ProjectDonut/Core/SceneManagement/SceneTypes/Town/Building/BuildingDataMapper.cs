using Microsoft.Xna.Framework;
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

        public static int[,] GenerateWallCapDataMap(Plot plot, List<Rectangle> roomBounds)
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
                        if (i == room.Left || i == room.Right)
                        {
                            map[i - plot.PlotBounds.X, j - plot.PlotBounds.Y] = 1;
                        }

                        if (j == room.Top || j == room.Bottom)
                        {
                            map[i - plot.PlotBounds.X, j - plot.PlotBounds.Y] = 1;
                        }
                    }
                }
            }

            //DebugMapData.WriteMapData(map, $"{plot.WorldPosition.X}-{plot.WorldPosition.Y}_wallCapDataMap");

            return map;
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

            for (int i = 0; i < plot.PlotBounds.Width; i++)
            {
                for (int j = 0; j < plot.PlotBounds.Height; j++)
                {
                    foreach (var room in roomBounds)
                    {
                        if (room.Contains(plot.PlotBounds.X + i, plot.PlotBounds.Y + j))
                        {
                            map[i, j] = 1;
                        }
                    }
                }
            }

            return map;
        }
    }
}
