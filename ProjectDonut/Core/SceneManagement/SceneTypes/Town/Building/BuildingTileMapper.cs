﻿using Microsoft.Xna.Framework;
using ProjectDonut.ProceduralGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town.Building
{
    public static class BuildingTileMapper
    {
        public static Tilemap GenerateGroundTilemap(int[,] map, Plot plot)
        {
            var tm = new Tilemap(map.GetLength(0), map.GetLength(1));

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 0)
                    {
                        continue;
                    }

                    var tile = new Tile(false)
                    {
                        ChunkX = 0,
                        ChunkY = 0,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize) + plot.WorldPosition,
                        Size = new Vector2(Global.TileSize, Global.TileSize),
                        Texture = Global.SpriteLibrary.TownSprites["grass-c"],
                        TileType = TileType.Instance,
                        IsExplored = true
                    };

                    tm.Map[i, j] = tile;
                }
            }

            return tm;
        }

        public static Tilemap GenerateFenceTilemap(int[,] map, Plot plot)
        {
            var tm = new Tilemap(map.GetLength(0), map.GetLength(1));
            var tilePlaced = false;

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 0)
                    {
                        continue;
                    }

                    var tile = new Tile(false)
                    {
                        ChunkX = 0,
                        ChunkY = 0,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize) + plot.WorldPosition,
                        Size = new Vector2(Global.TileSize, Global.TileSize),
                        Texture = RuleTiler.Town.FenceTexture(map, i, j),
                        TileType = TileType.Instance,
                        IsExplored = true
                    };

                    tm.Map[i, j] = tile;
                    tilePlaced = true;
                }
            }

            if (!tilePlaced)
            {
                ;
            }

            return tm;
        }

        public static Tilemap GenerateFloorTileMap(int[,] map, Plot plot)
        {
            var tm = new Tilemap(map.GetLength(0), map.GetLength(1));

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 0)
                    {
                        continue;
                    }

                    var tile = new Tile(false)
                    {
                        ChunkX = 0,
                        ChunkY = 0,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize) + plot.WorldPosition,
                        Size = new Vector2(Global.TileSize, Global.TileSize),
                        Texture = RuleTiler.Town.FloorTile(map, i, j),// Global.SpriteLibrary.BuildingBlockSprites["floor-c"],
                        TileType = TileType.Instance,
                        IsExplored = true
                    };

                    tm.Map[i, j] = tile;
                }
            }

            return tm;
        }

        public static Tilemap GenerateWallTileMap(int[,] wallMap, int[,] floorMap, Plot plot)
        {
            var tm = new Tilemap(wallMap.GetLength(0), wallMap.GetLength(1));

            for (int i = 0; i < wallMap.GetLength(0); i++)
            {
                for (int j = 0; j < wallMap.GetLength(1); j++)
                {
                    if (wallMap[i, j] == 0)
                    {
                        continue;
                    }

                    var tile = new Tile(false)
                    {
                        ChunkX = 0,
                        ChunkY = 0,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize) + plot.WorldPosition,
                        Size = new Vector2(Global.TileSize, Global.TileSize),
                        Texture = RuleTiler.Town.WallTexture(wallMap, floorMap, i, j),
                        TileType = TileType.Instance,
                        IsExplored = true
                    };

                    tm.Map[i, j] = tile;
                }
            }

            return tm;
        }

        //public static Tilemap GenerateWallTileMapOLD(int[,] wallMap, int[,] floorMap, Plot plot)
        //{
        //    var tm = new Tilemap(wallMap.GetLength(0), wallMap.GetLength(1));

        //    for (int i = 0; i < wallMap.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < wallMap.GetLength(1); j++)
        //        {
        //            if (wallMap[i, j] == 0)
        //            {
        //                continue;
        //            }

        //            var tile = new Tile(false)
        //            {
        //                ChunkX = 0,
        //                ChunkY = 0,
        //                xIndex = i,
        //                yIndex = j,
        //                LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize) + plot.WorldPosition,
        //                Size = new Vector2(Global.TileSize, Global.TileSize),
        //                Texture = TextureDecider.DetermineBuildingWallTexture(wallMap, floorMap, i, j),
        //                TileType = TileType.Instance,
        //                IsExplored = true
        //            };

        //            tm.Map[i, j] = tile;
        //        }
        //    }

        //    return tm;
        //}

        //public static Tilemap GenerateWallCapTileMap(int[,] wallMap, int[,] floorMap, int index, Plot plot)
        //{
        //    var tm = new Tilemap(wallMap.GetLength(0), wallMap.GetLength(1));

        //    for (int i = 0; i < wallMap.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < wallMap.GetLength(1); j++)
        //        {
        //            if (wallMap[i, j] == 0)
        //            {
        //                continue;
        //            }

        //            if (wallMap[i, j] == 1 || wallMap[i, j] == 2)
        //            {
        //                var tile = new Tile(false)
        //                {
        //                    ChunkX = 0,
        //                    ChunkY = 0,
        //                    xIndex = i,
        //                    yIndex = j - 1,
        //                    LocalPosition = new Vector2(i * Global.TileSize, (j - 1) * Global.TileSize) + plot.WorldPosition,
        //                    Size = new Vector2(Global.TileSize, Global.TileSize),
        //                    Texture = RuleTiler.DetermineBuildingWallCapTexture(wallMap, floorMap, tm, i, j),
        //                    TileType = TileType.Instance,
        //                    IsExplored = true
        //                };

        //                tm.Map[i, j] = tile;
        //            }
        //        }
        //    }

        //    int tryCount = 1000;
        //    do
        //    {
        //        for (int i = 0; i < tm.Map.GetLength(0); i++)
        //        {
        //            for (int j = 0; j < tm.Map.GetLength(1); j++)
        //            {
        //                if (tm.Map[i, j] != null && tm.Map[i, j].Texture == null)
        //                {
        //                    tm.Map[i, j].Texture = RuleTiler.DetermineBuildingWallCapTexturePass2(wallMap, floorMap, tm, i, j);
        //                }
        //            }
        //        }

        //        tryCount--;

        //        if (tryCount <= 0)
        //        {
        //            break;
        //        }
        //    }
        //    while (tm.Map.Cast<Tile>().Any(t => t != null && t.Texture == null));



        //    return tm;
        //}

        public static Tilemap GenerateRoofTileMap(int[,] map, Plot plot)
        {
            var tm = new Tilemap(map.GetLength(0), map.GetLength(1));

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 0)
                    {
                        continue;
                    }

                    var tile = new Tile(false)
                    {
                        ChunkX = 0,
                        ChunkY = 0,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize) + plot.WorldPosition,
                        Size = new Vector2(Global.TileSize, Global.TileSize),
                        Texture = Global.SpriteLibrary.BuildingBlockSprites["roof"],
                        TileType = TileType.Instance,
                        IsExplored = true
                    };

                    tm.Map[i, j] = tile;
                }
            }

            return tm;
        }

        public static Tilemap GenerateStairsTileMap(int[,] map, Plot plot)
        {
            var tm = new Tilemap(map.GetLength(0), map.GetLength(1));

            var stairTileNames = new string[,]
            {
                { "stairs-01", "stairs-02" },
                { "stairs-03", "stairs-04" },
                { "stairs-05", "stairs-06" },
                { "stairs-07", "stairs-08" },
                { "stairs-09", "stairs-10" },
            };

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 0)
                    {
                        continue;
                    }

                    for (int x = 0; x < 2; x++)
                    {
                        for (int y = 0; y < 5; y++)
                        {
                            var stairTileName = stairTileNames[y, x];

                            var tile = new Tile(false)
                            {
                                ChunkX = 0,
                                ChunkY = 0,
                                xIndex = i + x,
                                yIndex = j + y,
                                LocalPosition = new Vector2((i + x) * Global.TileSize, (j + y) * Global.TileSize) + plot.WorldPosition,
                                Size = new Vector2(Global.TileSize, Global.TileSize),
                                Texture = Global.SpriteLibrary.BuildingBlockSprites[stairTileName],
                                TileType = TileType.Instance,
                                IsExplored = true
                            };

                            tm.Map[i + x, j + y] = tile;
                        }
                    }

                    return tm;
                }
            }

            return tm;
        }
    }
}
