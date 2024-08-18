using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class HeightGenerator
    {
        private WorldMapSettings settings;
        private SpriteLibrary spriteLib;
        private FastNoiseLite _noise;

        public HeightGenerator(WorldMapSettings settings, SpriteLibrary spriteLib)
        {
            this.settings = settings;
            this.spriteLib = spriteLib;

            this._noise = new FastNoiseLite();
            //noise.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            _noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

            _noise.SetSeed(new Random().Next(int.MinValue, int.MaxValue));
            _noise.SetSeed(1337);
        }

        public int[,] GenerateHeightMap(int width, int height, int xOffset, int yOffset)
        {
            int[,] heightData = new int[height, width];

            int min = 0;
            int max = 0;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var x = (xOffset * settings.Width) + i;
                    var y = (yOffset * settings.Height) + j;

                    var heightValue = (int)(_noise.GetNoise(x, y) * 100) + 35;

                    heightData[i, j] = heightValue;

                    if (heightValue < min)
                    {
                        min = heightValue;
                    }
                    if (heightValue > max)
                    {
                        max = heightValue;
                    }
                }
            }

            return heightData;
        }

        public void CarveRiver(int[,] heightData, int startX, int startY)
        {
            int width = heightData.GetLength(1);
            int height = heightData.GetLength(0);

            int x = startX;
            int y = startY;

            while (true)
            {
                // Lower the height to carve the river
                heightData[y, x] = Math.Max(0, heightData[y, x] - 10); // Adjust the depth as needed

                // Determine the lowest neighboring cell
                int lowestHeight = heightData[y, x];
                int nextX = x, nextY = y;

                // Check all 4 neighboring cells (you can include diagonals if desired)
                if (x > 0 && heightData[y, x - 1] < lowestHeight)
                {
                    lowestHeight = heightData[y, x - 1];
                    nextX = x - 1;
                    nextY = y;
                }
                if (x < width - 1 && heightData[y, x + 1] < lowestHeight)
                {
                    lowestHeight = heightData[y, x + 1];
                    nextX = x + 1;
                    nextY = y;
                }
                if (y > 0 && heightData[y - 1, x] < lowestHeight)
                {
                    lowestHeight = heightData[y - 1, x];
                    nextX = x;
                    nextY = y - 1;
                }
                if (y < height - 1 && heightData[y + 1, x] < lowestHeight)
                {
                    lowestHeight = heightData[y + 1, x];
                    nextX = x;
                    nextY = y + 1;
                }

                // If the river can't flow lower, end the loop
                if (nextX == x && nextY == y)
                {
                    break;
                }

                x = nextX;
                y = nextY;

                // Handle chunk boundaries
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    // Record exit point to continue river in the neighboring chunk
                    // ...
                    break;
                }
            }
        }


        public Tilemap CreateBaseTilemap(int[,] heightData, int[,] biomeData)
        {
            var tmBase = new Tilemap(heightData.GetLength(0), heightData.GetLength(1));

            for (int i = 0; i < heightData.GetLength(0); i++)
            {
                for (int j = 0; j < heightData.GetLength(1); j++)
                {
                    var biomeValue = biomeData[i, j];
                    var heightValue = heightData[i, j];

                    var tile = new Tile
                    {
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * settings.TileSize, j * settings.TileSize),
                        Size = new Vector2(settings.TileSize, settings.TileSize),
                        Texture = DetermineTexture(i, j, biomeValue, heightValue),
                        TileType = DetermineTileType(i, j, heightValue),
                        Biome = (Biome)biomeData[i, j]
                    };

                    tmBase.Map[i, j] = tile;
                }
            }

            return tmBase;
        }

        private Texture2D DetermineTexture(int x, int y, int biomeValue, int heightValue)
        {
            var biome = (Biome)biomeValue;

            if (heightValue >= settings.MountainHeightMin)
            {
                return spriteLib.GetSprite("mountain");
            }
            else if (heightValue >= settings.GroundHeightMin)
            {
                switch (biome)
                {
                    case Biome.Desert:
                        return spriteLib.GetSprite("desert");

                    case Biome.Grasslands:
                        return spriteLib.GetSprite("grasslands");

                    case Biome.Winterlands:
                        return spriteLib.GetSprite("winterlands");

                    default:
                        return spriteLib.GetSprite("grasslands");
                }
            }
            else
            {
                if (heightValue >= settings.WaterHeightMin)
                {
                    return spriteLib.GetSprite("coast-inv");
                }
                else
                {
                    return spriteLib.GetSprite("deepwater-C");
                }
            }
        }

        private TileType DetermineTileType(int x, int y, int heightValue)
        {
            if (heightValue >= settings.MountainHeightMin)
            {
                return TileType.Mountain;
            }
            else if (heightValue >= settings.GroundHeightMin)
            {
                return TileType.Ground;
            }
            else
            {
                return TileType.Water;
            }
        }
    }
}
