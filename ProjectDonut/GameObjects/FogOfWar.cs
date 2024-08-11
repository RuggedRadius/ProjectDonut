using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.GameObjects
{
    public class FogOfWar
    {
        private bool TEMP_EXPLORE_ALL = true;

        private int sightRadius = 10;
        public bool[,] exploredTiles;

        public FogOfWar(int width, int height)
        {
            exploredTiles = new bool[width, height];

            if (TEMP_EXPLORE_ALL)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        exploredTiles[i, j] = true;
                    }
                }
            }
        }

        public void UpdateFogOfWar(int playerX, int playerY)
        {
            playerX /= 32;
            playerY /= 32;

            for (int i = playerX - sightRadius; i < playerX + sightRadius; i++)
            {
                for (int j = playerY - sightRadius; j <+ playerY + sightRadius; j++)
                {
                    if (i < 0 || i >= exploredTiles.GetLength(0) || j < 0 || j >= exploredTiles.GetLength(1))
                    {
                        continue;
                    }

                    if (Math.Sqrt(Math.Pow(i - playerX, 2) + Math.Pow(j - playerY, 2)) <= sightRadius)
                    {
                        exploredTiles[i, j] = true;
                    }
                }
            }
        }

        public bool IsTileInSightRadius(int x, int y, int playerX, int playerY)
        {
            playerX /= 32;
            playerY /= 32;

            var calcX = Math.Pow(x - playerX, 2);
            var calcY = Math.Pow(y - playerY, 2);

            return Math.Sqrt(calcX + calcY) <= sightRadius;
        }

        public bool IsTileExplored(int x, int y)
        {
            return exploredTiles[x, y];
        }
    }
}
