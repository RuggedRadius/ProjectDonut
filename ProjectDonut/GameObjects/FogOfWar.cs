using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectDonut.Core;
using ProjectDonut.GameObjects.PlayerComponents;

namespace ProjectDonut.GameObjects
{
    public class FogOfWar
    {
        private bool TEMP_EXPLORE_ALL = true;

        private int sightRadius = 20;
        public bool[,] exploredTiles;

        private Player _player;

        public FogOfWar(int width, int height, Player player)
        {
            exploredTiles = new bool[width, height];
            _player = player;

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

        public void UpdateFogOfWar()
        {
            var playerX = (int)_player.Position.X / Global.TileSize;
            var playerY = (int)_player.Position.X / Global.TileSize;

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
            playerX /= Global.TileSize;
            playerY /= Global.TileSize;

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
