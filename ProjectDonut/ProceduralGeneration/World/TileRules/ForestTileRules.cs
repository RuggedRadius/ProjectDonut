using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsepriteDotNet;
using MonoGame.Aseprite;

namespace ProjectDonut.ProceduralGeneration.World.TileRules
{
    public class ForestTileRules
    {
        private SpriteLibrary spriteLib;

        public ForestTileRules(SpriteLibrary spriteLib)
        {
            this.spriteLib = spriteLib;
        }

        public Tilemap ApplyForestRules(Tilemap tilemap)
        {
            foreach (var tile in tilemap.Map)
            {
                if (tile == null)
                {
                    continue;
                }

                int x = tile.xIndex;
                int y = tile.yIndex;

                try
                {
                    if (x == 0 || 
                        y == 0 || 
                        x == tilemap.Map.GetLength(0) - 1 || 
                        y == tilemap.Map.GetLength(1) - 1)
                    {
                        continue;
                    }

                    var directionString = GetDirectionString(tilemap, x, y);
                    var biomeString = GetBiomeString(tile);
                    var tileString = $"{biomeString}-{directionString}";

                    tile.Texture = spriteLib.GetSprite(tileString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return tilemap;
        }

        private string GetBiomeString(Tile tile)
        {
            switch (tile.Biome)
            {
                case Biome.Winterlands:
                    return "forest-frost";

                case Biome.Grasslands:
                default:
                    return "forest";
            }
        }

        private string GetDirectionString(Tilemap tilemap, int x, int y)
        {
            var n = tilemap.GetTile(x, y - 1) != null;
            var nw = tilemap.GetTile(x - 1, y - 1) != null;
            var ne = tilemap.GetTile(x + 1, y - 1) != null;
            var e = tilemap.GetTile(x + 1, y) != null;
            var w = tilemap.GetTile(x - 1, y) != null;
            var s = tilemap.GetTile(x, y + 1) != null;
            var se = tilemap.GetTile(x + 1, y + 1) != null;
            var sw = tilemap.GetTile(x - 1, y + 1) != null;

            if (!n && e && s && !w)
            {
                return "NW";
            }
            else if (!n && e && s && w)
            {
                return "N";
            }
            else if (!n && !e && s && w)
            {
                return "NE";
            }
            else if (n && e && s && !w)
            {
                return "W";
            }
            else if (n && e && s && w)
            {
                return "C";
            }
            else if (n && !e && s && w)
            {
                return "E";
            }
            else if (n && e && !s && !w)
            {
                return "SW";
            }
            else if (n && e && !s && w)
            {
                return "S";
            }
            else if (n && !e && !s && w)
            {
                return "SE";
            }

            if (n && e && s && w && !se && nw && ne && sw)
            {
                return "inv-NW";
            }
            else if (n && e && s && w && !sw && ne && nw && se)
            {
                return "inv-NE";
            }
            else if (n && e && s && w && sw && ne && !nw && se)
            {
                return "inv-SE";
            }
            else if (n && e && s && w && sw && !ne && nw && se)
            {
                return "inv-SW";
            }

            return "C";
        }
    }
}
