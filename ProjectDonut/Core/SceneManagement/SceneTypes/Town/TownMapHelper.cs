using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town
{
    public static class TownMapHelper
    {
        public static bool IsNeighbourCellRoad(int[,] map, int x, int y)
        {
            var n = 0;
            var e = 0;
            var s = 0;
            var w = 0;

            if (y > 0)
                n = map[x, y - 1];
            if (x > 0)
                w = map[x - 1, y];
            if (x < map.GetLength(0) - 1)
                e = map[x + 1, y];
            if (y < map.GetLength(1) - 1)
                s = map[x, y + 1];

            return n == 1 || e == 1 || s == 1 || w == 1;
        }
    }
}
