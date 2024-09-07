using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.World.Generators
{
    public class HeightGenerator
    {
        private WorldMapSettings settings;
        private FastNoiseLite[] _noise;

        private float OctaveBlend = 0.0525f;

        public HeightGenerator(WorldMapSettings settings)
        {
            this.settings = settings;

            _noise = new FastNoiseLite[2];
            _noise[0] = new FastNoiseLite();
            //_noise[0].SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            _noise[0].SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            _noise[0].SetSeed(new Random().Next(int.MinValue, int.MaxValue));

            _noise[1] = new FastNoiseLite();
            _noise[1].SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            //_noise[1].SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            _noise[1].SetSeed(new Random().Next(int.MinValue, int.MaxValue));
            _noise[1].SetFrequency(0.5f);
            _noise[1].SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Manhattan);
        }

        public int[,] GenerateHeightMap(int width, int height, int xOffset, int yOffset)
        {
            var datas = new List<int[,]>();

            for (int z = 0; z < 2; z++)
            {
                int[,] heightData = new int[height, width];

                int min = 0;
                int max = 0;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        var x = xOffset * settings.Width + i;
                        var y = yOffset * settings.Height + j;

                        var heightValue = (int)(_noise[z].GetNoise(x, y) * Global.ChunkSize) + 35;

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

                datas.Add(heightData);
            }

            int[,] result = new int[height, width];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    result[i, j] = Blend(datas[0][i, j], datas[1][i, j], OctaveBlend);
                }
            }

            return result;
        }

        int Blend(int a, int b, float t)
        {
            return (int)(a + t * (b - a));
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


        public Tilemap CreateBaseTilemap(WorldChunk chunk)
        {
            var tmBase = new Tilemap(chunk.Width, chunk.Height);

            var waterTileCount = 0;
            var groundTileCount = 0;
            var mountainTileCount = 0;

            for (int i = 0; i < chunk.Width; i++)
            {
                for (int j = 0; j < chunk.Height; j++)
                {
                    var biomeValue = chunk.BiomeData[i, j];
                    var heightValue = chunk.HeightData[i, j];

                    var tile = new Tile(false)
                    {
                        ChunkX = chunk.ChunkCoordX,
                        ChunkY = chunk.ChunkCoordY,
                        xIndex = i,
                        yIndex = j,
                        //Position = new Vector2(i * settings.TileSize, j * settings.TileSize) + chunk.Position,
                        LocalPosition = new Vector2(i * settings.TileSize, j * settings.TileSize),
                        Size = new Vector2(settings.TileSize, settings.TileSize),
                        Texture = DetermineTexture(i, j, biomeValue, heightValue),
                        TileType = TileType.World,
                        WorldTileType = DetermineTileType(i, j, heightValue),
                        Biome = (Biome)chunk.BiomeData[i, j]
                    };

                    if (tile.WorldTileType == WorldTileType.Water)
                        waterTileCount++;
                    else if (tile.WorldTileType == WorldTileType.Ground)
                        groundTileCount++;
                    else
                        mountainTileCount++;

                    tmBase.Map[i, j] = tile;
                }
            }

            return tmBase;
        }

        private Texture2D DetermineTexture(int x, int y, int biomeValue, int heightValue)
        {
            var biome = (Biome)biomeValue;

            if (heightValue >= settings.GroundHeightMin)
            {
                switch (biome)
                {
                    case Biome.Desert:
                        return Global.SpriteLibrary.GetSprite("desert");

                    case Biome.Grasslands:
                        return Global.SpriteLibrary.GetSprite("grasslands");

                    case Biome.Winterlands:
                        return Global.SpriteLibrary.GetSprite("winterlands");

                    default:
                        return Global.SpriteLibrary.GetSprite("grasslands");
                }
            }
            else
            {
                if (heightValue >= settings.WaterHeightMin)
                {
                    return Global.SpriteLibrary.GetSprite("coast-inv");
                }
                else
                {
                    return Global.SpriteLibrary.GetSprite("deepwater-C");
                }
            }
        }

        private WorldTileType DetermineTileType(int x, int y, int heightValue)
        {
            if (heightValue >= settings.MountainHeightMin)
            {
                return WorldTileType.Mountain;
            }
            else if (heightValue >= settings.GroundHeightMin)
            {
                return WorldTileType.Ground;
            }
            else
            {
                return WorldTileType.Water;
            }
        }
    }
}
