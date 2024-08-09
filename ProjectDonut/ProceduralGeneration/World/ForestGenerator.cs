using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class ForestGenerator
    {
        private SpriteLibrary spriteLib;
        private WorldMapSettings settings;

        public ForestGenerator(SpriteLibrary spriteLib, WorldMapSettings mapSettings)
        {
            this.spriteLib = spriteLib;
            this.settings = mapSettings;
        }

        public Tilemap CreateForestTilemap(int[,] forestData, int[,] biomeData)
        {
            var width = forestData.GetLength(0);
            var height = forestData.GetLength(1);

            var tmForest = new Tilemap(forestData.GetLength(0), forestData.GetLength(1));

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (forestData[i, j] == 0)
                    {
                        continue;
                    }

                    var tile = new Tile
                    {
                        xIndex = i,
                        yIndex = j,
                        Position = new Vector2(i * settings.TileSize, j * settings.TileSize),
                        Size = new Vector2(settings.TileSize, settings.TileSize),
                        Texture = DetermineTexture(i, j, biomeData),
                        TileType = TileType.Forest
                    };

                    tmForest.Map[i, j] = tile;
                }
            }

            return tmForest;
        }

        private Texture2D DetermineTexture(int x, int y, int[,] biomeData)
        {
            var tileType = (Biome)biomeData[x, y];

            switch (tileType)
            {
                case Biome.Grasslands:
                    return spriteLib.GetSprite("forest-C");

                case Biome.Desert:
                    return spriteLib.GetSprite("forest-C"); // Change this later?

                case Biome.Winterlands:
                    return spriteLib.GetSprite("forest-frost-C"); 

                default:
                    return spriteLib.GetSprite("forest-C");
            }
        }

        public int[,] GenerateForestData(int[,] heightData, int[,] biomeData)
        {
            var width = heightData.GetLength(0);
            var height = heightData.GetLength(1);

            var possibleCoords = new List<(int, int)>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (biomeData[x, y] == (int)Biome.Grasslands)
                    {
                        possibleCoords.Add((x, y));
                    }
                    else if (biomeData[x, y] == (int)Biome.Winterlands)
                    {
                        possibleCoords.Add((x, y));
                    }
                }
            }

            var forestData = new int[width, height];
            var randy = new Random();

            for (int x = 0; x < settings.ForestCount; x++)
            {
                var randomIndex = randy.Next(0, possibleCoords.Count);
                var coords = possibleCoords[randomIndex];

                //forestData[coords.Item1, coords.Item2] = 1;
                var walkLength = randy.Next(settings.MinWalk, settings.MaxWalk);

                for (int y = 0; y < walkLength; y++)
                {
                    var direction = randy.Next(0, 4);
                    switch (direction)
                    {
                        case 0:
                            if (coords.Item1 + 1 < width)
                            {
                                coords.Item1 += 1;
                            }
                            break;
                        case 1:
                            if (coords.Item1 - 1 >= 0)
                            {
                                coords.Item1 -= 1;
                            }
                            break;
                        case 2:
                            if (coords.Item2 + 1 < height)
                            {
                                coords.Item2 += 1;
                            }
                            break;
                        case 3:
                            if (coords.Item2 - 1 >= 0)
                            {
                                coords.Item2 -= 1;
                            }
                            break;
                    }

                    for (int i = -settings.WalkRadius; i < settings.WalkRadius; i++)
                    {
                        for (int j = -settings.WalkRadius; j < settings.WalkRadius; j++)
                        {
                            var xCoord = coords.Item1 + i;
                            var yCoord = coords.Item2 + j;

                            if (xCoord < 0 || xCoord >= width || yCoord < 0 || yCoord >= height)
                            {
                                continue;
                            }

                            if (biomeData[xCoord, yCoord] == (int)Biome.Grasslands ||
                                biomeData[xCoord, yCoord] == (int)Biome.Winterlands)
                            {
                                continue;
                            }

                            if ( heightData[xCoord, yCoord] >= settings.GroundHeightMin
                                && heightData[xCoord, yCoord] <= settings.GroundHeightMax)
                            {
                                forestData[xCoord, yCoord] = 1;
                            }
                        }
                    }
                }

                possibleCoords.Remove(coords);
            }

            return forestData;
        }
    }
}
