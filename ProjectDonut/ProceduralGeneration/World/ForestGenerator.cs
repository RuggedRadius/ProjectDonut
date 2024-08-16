using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                        LocalPosition = new Vector2(i * settings.TileSize, j * settings.TileSize),
                        Size = new Vector2(settings.TileSize, settings.TileSize),
                        Texture = DetermineTexture(i, j, biomeData),
                        TileType = TileType.Forest,
                        Biome = (Biome)biomeData[i, j]
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
            var randy = new Random();
            var width = heightData.GetLength(0);
            var height = heightData.GetLength(1);
            var possibleCoords = GetPossibleStartingCoordinates(heightData, biomeData);
            var forestData = new int[width, height];

            if (possibleCoords.Count == 0)
            {
                return forestData;
            }

            for (int x = 0; x < settings.ForestCount; x++)
            {
                var randomIndex = randy.Next(0, possibleCoords.Count);

                if (possibleCoords.Count == 0)
                {
                    return forestData;
                }

                var coords = possibleCoords[randomIndex];
                var walkLength = randy.Next(settings.MinWalk, settings.MaxWalk);

                for (int y = 0; y < walkLength; y++)
                {
                    coords = UpdateCoordinates(coords, width, height);
                    
                    for (int i = -settings.WalkRadius; i < settings.WalkRadius; i++)
                    {
                        for (int j = -settings.WalkRadius; j < settings.WalkRadius; j++)
                        {
                            var xCoord = coords.Item1 + i;
                            var yCoord = coords.Item2 + j;

                            if (IsCoordsWithinMapBounds(xCoord, yCoord, width, height) == false)
                            {
                                continue;
                            }

                            if (IsCoordsInSuitableBiome(biomeData, xCoord, yCoord) == false)
                            {
                                continue;
                            }

                            if (IsCoordsAtSuitableHeight(heightData, xCoord, yCoord) == false)
                            {
                                continue;
                            }

                            forestData[xCoord, yCoord] = 1;
                        }
                    }
                }

                possibleCoords.Remove(coords);
            }

            return forestData;
        }

        private bool IsCoordsAtSuitableHeight(int[,] heightData, int x, int y)
        {
            if (heightData[x, y] >= settings.GroundHeightMin && heightData[x, y] <= settings.GroundHeightMax)
            {
                return true;
            }

            return false;
        }

        private bool IsCoordsWithinMapBounds(int x, int y, int width, int height)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
            {
                return false;
            }

            return true;
        }

        private bool IsCoordsInSuitableBiome(int[,] biomeData, int x, int y)
        {
            var suitableBiomes = new List<int>
            {
                (int)Biome.Grasslands,
                (int)Biome.Winterlands
            };

            if (suitableBiomes.Contains(biomeData[x, y]))
            {
                return true;
            }

            return false;
        }

        private (int, int) UpdateCoordinates((int, int) coords, int width, int height)
        {
            var randy = new Random();
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

            return coords;
        }

        private List<(int x, int y)> GetPossibleStartingCoordinates(int[,] heightData, int[,] biomeData)
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

            return possibleCoords;
        }
    }
}
