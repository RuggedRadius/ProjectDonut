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
        private SpriteLibrary spriteLib;
        private Tilemap tilemap;


        public WorldTileRuler(SpriteLibrary spriteLib, Tilemap tilemap)
        {
            this.spriteLib = spriteLib;
            this.tilemap = tilemap;
        }

        public Tilemap ApplyTileRules()
        {
            tilemap = ApplyCoastLineRules();
            tilemap = ApplyForestRules();
            return tilemap;
        }


        #region Coast-line Rules
        private Tilemap ApplyCoastLineRules()
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
                        counter++;
                        continue;
                    }

                    if (isNorthWestCoast(x, y))
                    {
                        tile.Texture = spriteLib.GetSprite("coast-NW");
                    }

                    if (isNorthEastCoast(x, y))
                    {
                        tile.Texture = spriteLib.GetSprite("coast-NE");
                    }

                    if (isSouthEastCoast(x, y))
                    {
                        tile.Texture = spriteLib.GetSprite("coast-SE");
                    }

                    if (isSouthWestCoast(x, y))
                    {
                        tile.Texture = spriteLib.GetSprite("coast-SW");
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
        #endregion

        #region Forest Rules
        private Tilemap ApplyForestRules()
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
                        counter++;
                        continue;
                    }

                    if (tile.TileType != TileType.Forest)
                    {
                        continue;
                    }

                    var placement = GetForestTilePlacement(x, y);

                    switch (placement)
                    {
                        case TilePlacement.NW:
                            tile.Texture = spriteLib.GetSprite("forest-NW");
                            break;

                        case TilePlacement.N:
                            tile.Texture = spriteLib.GetSprite("forest-N");
                            break;

                        case TilePlacement.NE:
                            tile.Texture = spriteLib.GetSprite("forest-NE");
                            break;

                        //case TilePlacement.E:
                        //    break;

                        //case TilePlacement.SE:
                        //    break;

                        //case TilePlacement.S:
                        //    break;

                        //case TilePlacement.SW:
                        //    break;

                        //case TilePlacement.W:
                        //    break;

                        case TilePlacement.C:
                        default:
                            tile.Texture = spriteLib.GetSprite("forest-C");
                            break;
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
        #endregion

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

        private TilePlacement GetForestTilePlacement(int x, int y)
        {
            var neighbours = GetNeighbours(x, y);

            var currentTile = neighbours[4].TileType;

            var nw = neighbours[0].TileType;
            var w = neighbours[1].TileType;
            var sw = neighbours[2].TileType;
            var n = neighbours[3].TileType;
            
            var s = neighbours[5].TileType;
            var ne = neighbours[6].TileType;
            var e = neighbours[7].TileType;
            var se = neighbours[8].TileType;

            if (nw != TileType.Forest && n != TileType.Forest && w != TileType.Forest)
            {
                return TilePlacement.NW;
            }
            else if (n != TileType.Forest && w == TileType.Forest && e == TileType.Forest)
            {
                return TilePlacement.N;
            }
            else if (e != TileType.Forest && n != TileType.Forest && w == TileType.Forest)
            {
                return TilePlacement.NE;
            }
            else
            {
                return TilePlacement.C;
            }
        }
    }
}
