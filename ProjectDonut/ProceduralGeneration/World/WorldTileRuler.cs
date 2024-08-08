using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class WorldTileRuler
    {
        private Dictionary<string, Texture2D> spriteLib;
        private Tilemap tilemap;


        public WorldTileRuler(Dictionary<string, Texture2D> spriteLib, Tilemap tilemap)
        {
            this.spriteLib = spriteLib;
            this.tilemap = tilemap;
        }

        public Tilemap ApplyTileRules()
        {
            int counter = 0;
            foreach (var tile in tilemap.Map)
            {
                int x = tile.xIndex;
                int y = tile.yIndex;

                try
                {
                    if (x == 0 || y == 0 || x == tilemap.Map.GetLength(0) - 1 || y == tilemap.Map.GetLength(1) - 1)
                    {
                        //tile.Texture = spriteLib["coast-inv"];
                        counter++;
                        continue;
                    }

                    if (isNorthWestCoast(x, y))
                    {
                        tile.Texture = spriteLib["coast-NW"];
                    }

                    if (isNorthEastCoast(x, y))
                    {
                        tile.Texture = spriteLib["coast-NE"];
                    }

                    if (isSouthEastCoast(x, y))
                    {
                        tile.Texture = spriteLib["coast-SE"];
                    }

                    if (isSouthWestCoast(x, y))
                    {
                        tile.Texture = spriteLib["coast-SW"];
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                counter++;
            }

            return tilemap;
        }

        private bool isNorthWestCoast(int x, int y)
        {
            var neighbours = GetNeighbours(x, y);

            if (neighbours.Where(x => x == null).Any())
            {
                return false;
            }

            var northWestTile = neighbours[0].TileType;
            var westTile = neighbours[1].TileType;
            var southWestTile = neighbours[2].TileType;
            var northTile = neighbours[3].TileType;
            var currentTile = neighbours[4].TileType;
            var southTile = neighbours[5].TileType;
            var northEastTile = neighbours[6].TileType;
            var eastTile = neighbours[7].TileType;
            var southEastTile = neighbours[8].TileType;

            if (northWestTile == TileType.Water &&
                northTile == TileType.Water &&
                westTile == TileType.Water &&
                southWestTile == TileType.Coast &&
                northEastTile == TileType.Coast &&
                eastTile == TileType.Coast &&
                southEastTile == TileType.Coast &&
                southTile == TileType.Coast)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isSouthWestCoast(int x, int y)
        {
            var neighbours = GetNeighbours(x, y);

            if (neighbours.Where(x => x == null).Any())
            {
                return false;
            }

            var northWestTile = neighbours[0].TileType;
            var westTile = neighbours[1].TileType;
            var southWestTile = neighbours[2].TileType;
            var northTile = neighbours[3].TileType;
            var currentTile = neighbours[4].TileType;
            var southTile = neighbours[5].TileType;
            var northEastTile = neighbours[6].TileType;
            var eastTile = neighbours[7].TileType;
            var southEastTile = neighbours[8].TileType;

            if (northWestTile == TileType.Coast &&
                northTile == TileType.Coast &&
                westTile == TileType.Water &&
                southWestTile == TileType.Water &&
                northEastTile == TileType.Coast &&
                eastTile == TileType.Coast &&
                southEastTile == TileType.Coast &&
                southTile == TileType.Water)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isNorthEastCoast(int x, int y)
        {
            var neighbours = GetNeighbours(x, y);

            if (neighbours.Where(x => x == null).Any())
            {
                return false;
            }

            var northWestTile = neighbours[0].TileType;
            var westTile = neighbours[1].TileType;
            var southWestTile = neighbours[2].TileType;
            var northTile = neighbours[3].TileType;
            var currentTile = neighbours[4].TileType;
            var southTile = neighbours[5].TileType;
            var northEastTile = neighbours[6].TileType;
            var eastTile = neighbours[7].TileType;
            var southEastTile = neighbours[8].TileType;

            if (northWestTile == TileType.Coast &&
                northTile == TileType.Water &&
                westTile == TileType.Coast &&
                southWestTile == TileType.Coast &&
                northEastTile == TileType.Water &&
                eastTile == TileType.Water &&
                southEastTile == TileType.Coast &&
                southTile == TileType.Coast)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isSouthEastCoast(int x, int y)
        {
            var neighbours = GetNeighbours(x, y);

            if (neighbours.Where(x => x == null).Any())
            {
                return false;
            }

            var northWestTile = neighbours[0].TileType;
            var westTile = neighbours[1].TileType;
            var southWestTile = neighbours[2].TileType;
            var northTile = neighbours[3].TileType;
            var currentTile = neighbours[4].TileType;
            var southTile = neighbours[5].TileType;
            var northEastTile = neighbours[6].TileType;
            var eastTile = neighbours[7].TileType;
            var southEastTile = neighbours[8].TileType;

            if (northWestTile == TileType.Coast &&
                northTile == TileType.Coast &&
                westTile == TileType.Coast &&
                southWestTile == TileType.Coast &&
                northEastTile == TileType.Coast &&
                eastTile == TileType.Water &&
                southEastTile == TileType.Water &&
                southTile == TileType.Water)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<Tile> GetNeighbours(int x, int y)
        {
            var neighbours = new List<Tile>();

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    neighbours.Add(tilemap.Map[x + i, y + j]);
                }
            }

            return neighbours;
        }
    }
}
