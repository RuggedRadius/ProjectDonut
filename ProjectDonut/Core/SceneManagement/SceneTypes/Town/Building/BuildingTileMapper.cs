using Microsoft.Xna.Framework;
using ProjectDonut.Core.Sprites;
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
                        Texture = SpriteLib.Town.Terrain["grass-c"],
                        TileType = TileType.Instance,
                        IsExplored = true                        
                    };

                    tile.Initialize();

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
                    tile.Initialize();
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

        public static Tilemap GenerateFloorTileMap(int[,] map, Plot plot, BuildingLevel level)
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
                        Texture = RuleTiler.Town.FloorTile(map, level.WallDataMap, i, j),// SpriteLib.BuildingBlockSprites["floor-c"],
                        TileType = TileType.Instance,
                        IsExplored = true
                    };
                    tile.Initialize();
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
                    tile.Initialize();
                    tm.Map[i, j] = tile;
                }
            }

            return tm;
        }

        public static Tilemap GenerateStairsTileMap(int[,] map, Plot plot)
        {
            var tm = new Tilemap(map.GetLength(0), map.GetLength(1));

            var stairTiles = new string[]
            {
                "stairs-top-01",
                "stairs-bottom-01",
                "stairs-bottom-01",
                "stairs-top-02",
                "stairs-bottom-02",
                "stairs-bottom-02",
                "stairs-top-03",
                "stairs-bottom-03",
                "stairs-bottom-03",
            };

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 0)
                    {
                        continue;
                    }

                    int tileNameCounter = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            tm.Map[i + k, j + l] = new Tile(false)
                            {
                                xIndex = i + k,
                                yIndex = j + l,
                                LocalPosition = new Vector2((i + k) * Global.TileSize, (j + l) * Global.TileSize) + plot.WorldPosition,
                                Size = new Vector2(Global.TileSize, Global.TileSize),
                                Texture = SpriteLib.Town.Stairs[stairTiles[tileNameCounter]],
                                TileType = TileType.Instance,
                                IsExplored = true
                            };
                            tm.Map[i + k, j + l].Initialize();
                            tileNameCounter++;
                        }
                    }

                    return tm;
                }
            }

            return tm;
        }
    }
}
