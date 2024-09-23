using Microsoft.Xna.Framework;
using Penumbra;
using ProjectDonut.Tools;
using System;
using System.Collections.Generic;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Room = ProjectDonut.ProceduralGeneration.BSP.Room;

namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town
{
    public static class PlotDataMapper
    {


        public static int[,] GeneratePlotMap(Rectangle bounds)
        {
            var map = new int[bounds.Width, bounds.Height];

            for (int i = 0; i < bounds.Width; i++)
            {
                for (int j = 0; j < bounds.Height; j++)
                {
                    if (TownMapHelper.IsNeighbourCellRoad(map, i, j))
                    {
                        map[i, j] = 3;
                    }
                    else
                    {
                        map[i, j] = 2;
                    }
                }
            }

            return map;
        }

        public static int[,] GenerateFenceMap(Rectangle bounds)
        {
            var map = new int[bounds.Width, bounds.Height];

            for (int i = 0; i < bounds.Width; i++)
            {
                for (int j = 0; j < bounds.Height; j++)
                {
                    if (i == 0 || i == bounds.Width - 1 || j == 0 || j == bounds.Height - 1)
                    {
                        map[i, j] = 1;
                    }
                    else
                    {
                        map[i, j] = 0;
                    }
                }
            }

            return map;
        }

        //public static List<int[,]> GenerateHouseMap(Plot plot)
        //{
        //    var width = plot.PlotBounds.Width;
        //    var height = plot.PlotBounds.Height;
        //    var houseMap = new List<int[,]>();

        //    var houseWidth = plot.HouseBounds.Width / Global.TileSize;
        //    var houseHeight = plot.HouseBounds.Height / Global.TileSize;

        //    var floorMap = new int[houseWidth, height];
        //    for (int i = 0; i < houseWidth; i++)
        //    {
        //        for (int j = 0; j < houseHeight; j++)
        //        {
        //            floorMap[i, j] = 1;
        //        }
        //    }

        //    var wall1Map = new int[width, height];
        //    for (int i = 0; i < houseWidth; i++)
        //    {
        //        for (int j = 0; j < houseHeight; j++)
        //        {
        //            if (i == 0 || i == houseWidth - 1 || j == 0 || j == houseHeight - 1)
        //                wall1Map[i, j] = 1;
        //            else
        //                wall1Map[i, j] = 0;
        //        }
        //    }

        //    var roofMap = new int[width, height];
        //    for (int i = 0; i < houseWidth; i++)
        //    {
        //        for (int j = 0; j < houseHeight; j++)
        //        {
        //            roofMap[i, j] = 1;
        //        }
        //    }

        //    houseMap.Add(floorMap);
        //    houseMap.Add(wall1Map);
        //    houseMap.Add(roofMap);

        //    return houseMap;
        //}

        //public static Vector2 CalculateHouseOffset(Plot plot)
        //{
        //    var random = new Random();

        //    return new Vector2(
        //        random.Next(1, plot.PlotBounds.Width - plot.HouseMap[0].GetLength(0) - 1),
        //        random.Next(1, plot.PlotBounds.Height - plot.HouseMap[0].GetLength(1) - 1)
        //        );
        //}


    }
}
