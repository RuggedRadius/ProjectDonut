using Microsoft.Xna.Framework;
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
                        Texture = TextureDecider.DetermineTownFenceTexture(map, i, j),
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

        public static Tilemap GenerateHouseFloorMap(int[,] map, Plot plot)
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
                        Texture = Global.SpriteLibrary.BuildingBlockSprites["building-floor"],
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

        public static Tilemap GenerateHouseWallMap(int[,] wallMap, int[,] floorMap, Plot plot)
        {
            var tm = new Tilemap(wallMap.GetLength(0), wallMap.GetLength(1));
            var tilePlaced = false;

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
                        Texture = TextureDecider.DetermineBuildingWallTexture(wallMap, floorMap, i, j),
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

        public static Tilemap GenerateHouseRoofMap(int[,] map, Plot plot)
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
                        Texture = Global.SpriteLibrary.BuildingBlockSprites["building-roof-thatching"],
                        TileType = TileType.Instance,
                        IsExplored = true
                    };

                    tm.Map[i, j] = tile;
                }
            }

            return tm;
        }

        public static Tilemap GenerateStairsTileMap(int[,] map)
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
                        LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize),
                        Size = new Vector2(Global.TileSize, Global.TileSize),
                        Texture = Global.SpriteLibrary.BuildingBlockSprites["building-stairs"],
                        TileType = TileType.Instance,
                        IsExplored = true
                    };

                    tm.Map[i, j] = tile;
                }
            }

            return tm;
        }
    }
}
