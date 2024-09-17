using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectDonut.Core;
using ProjectDonut.Core.SpriteLibrary;

namespace ProjectDonut.ProceduralGeneration.World.TileRules
{
    public class CoastTileRules
    {
        private Tilemap tilemap;

        public Tilemap ApplyCoastLineRules(Tilemap tilemap)
        {
            this.tilemap = tilemap;

            int counter = 0;
            foreach (var tile in tilemap.Map)
            {
                int x = tile.xIndex;
                int y = tile.yIndex;

                try
                {
                    if (x == 0 || y == 0 || x == tilemap.Map.GetLength(0) - 1 || y == tilemap.Map.GetLength(1) - 1)
                    {
                        counter++;
                        continue;
                    }

                    if (isNorthWestCoast(x, y))
                    {
                        tile.Texture = SpriteLib.GetSprite("coast-NW");
                    }

                    if (isNorthEastCoast(x, y))
                    {
                        tile.Texture = SpriteLib.GetSprite("coast-NE");
                    }

                    if (isSouthEastCoast(x, y))
                    {
                        tile.Texture = SpriteLib.GetSprite("coast-SE");
                    }

                    if (isSouthWestCoast(x, y))
                    {
                        tile.Texture = SpriteLib.GetSprite("coast-SW");
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

            var northWestTile = neighbours[0].WorldTileType;
            var westTile = neighbours[1].WorldTileType;
            var southWestTile = neighbours[2].WorldTileType;
            var northTile = neighbours[3].WorldTileType;
            var currentTile = neighbours[4].WorldTileType;
            var southTile = neighbours[5].WorldTileType;
            var northEastTile = neighbours[6].WorldTileType;
            var eastTile = neighbours[7].WorldTileType;
            var southEastTile = neighbours[8].WorldTileType;

            if (northWestTile == WorldTileType.Water &&
                northTile == WorldTileType.Water &&
                westTile == WorldTileType.Water &&
                southWestTile == WorldTileType.Coast &&
                northEastTile == WorldTileType.Coast &&
                eastTile == WorldTileType.Coast &&
                southEastTile == WorldTileType.Coast &&
                southTile == WorldTileType.Coast)
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

            var northWestTile = neighbours[0].WorldTileType;
            var westTile = neighbours[1].WorldTileType;
            var southWestTile = neighbours[2].WorldTileType;
            var northTile = neighbours[3].WorldTileType;
            var currentTile = neighbours[4].WorldTileType;
            var southTile = neighbours[5].WorldTileType;
            var northEastTile = neighbours[6].WorldTileType;
            var eastTile = neighbours[7].WorldTileType;
            var southEastTile = neighbours[8].WorldTileType;

            if (northWestTile == WorldTileType.Coast &&
                northTile == WorldTileType.Coast &&
                westTile == WorldTileType.Water &&
                southWestTile == WorldTileType.Water &&
                northEastTile == WorldTileType.Coast &&
                eastTile == WorldTileType.Coast &&
                southEastTile == WorldTileType.Coast &&
                southTile == WorldTileType.Water)
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

            var northWestTile = neighbours[0].WorldTileType;
            var westTile = neighbours[1].WorldTileType;
            var southWestTile = neighbours[2].WorldTileType;
            var northTile = neighbours[3].WorldTileType;
            var currentTile = neighbours[4].WorldTileType;
            var southTile = neighbours[5].WorldTileType;
            var northEastTile = neighbours[6].WorldTileType;
            var eastTile = neighbours[7].WorldTileType;
            var southEastTile = neighbours[8].WorldTileType;

            if (northWestTile == WorldTileType.Coast &&
                northTile == WorldTileType.Water &&
                westTile == WorldTileType.Coast &&
                southWestTile == WorldTileType.Coast &&
                northEastTile == WorldTileType.Water &&
                eastTile == WorldTileType.Water &&
                southEastTile == WorldTileType.Coast &&
                southTile == WorldTileType.Coast)
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

            var northWestTile = neighbours[0].WorldTileType;
            var westTile = neighbours[1].WorldTileType;
            var southWestTile = neighbours[2].WorldTileType;
            var northTile = neighbours[3].WorldTileType;
            var currentTile = neighbours[4].WorldTileType;
            var southTile = neighbours[5].WorldTileType;
            var northEastTile = neighbours[6].WorldTileType;
            var eastTile = neighbours[7].WorldTileType;
            var southEastTile = neighbours[8].WorldTileType;

            if (northWestTile == WorldTileType.Coast &&
                northTile == WorldTileType.Coast &&
                westTile == WorldTileType.Coast &&
                southWestTile == WorldTileType.Coast &&
                northEastTile == WorldTileType.Coast &&
                eastTile == WorldTileType.Water &&
                southEastTile == WorldTileType.Water &&
                southTile == WorldTileType.Water)
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
