using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectDonut.Core;

namespace ProjectDonut.ProceduralGeneration.World.TileRules
{
    public class GrasslandsRules
    {
        public GrasslandsRules()
        {
        }

        public Tilemap ApplyRules(Tilemap tilemap)
        {
            foreach (var tile in tilemap.Map)
            {
                if (tile == null)
                {
                    continue;
                }

                if (tile.Texture.Name == null)
                {
                    continue;
                }

                //if (tile.Texture.Name?.Contains("grasslands") == false)
                //{
                //    continue;
                //}

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
                    var tileString = $"grasslands-{directionString}";

                    tile.Texture = Global.SpriteLibrary.GetSprite(tileString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return tilemap;
        }

        private string GetDirectionString(Tilemap tilemap, int x, int y)
        {
            var n = tilemap.GetTile(x, y - 1)?.Biome == Biome.Grasslands;
            var nw = tilemap.GetTile(x - 1, y - 1)?.Biome == Biome.Grasslands;
            var ne = tilemap.GetTile(x + 1, y - 1)?.Biome == Biome.Grasslands;
            var e = tilemap.GetTile(x + 1, y)?.Biome == Biome.Grasslands;
            var w = tilemap.GetTile(x - 1, y)?.Biome == Biome.Grasslands;
            var s = tilemap.GetTile(x, y + 1)?.Biome == Biome.Grasslands;
            var se = tilemap.GetTile(x + 1, y + 1)?.Biome == Biome.Grasslands;
            var sw = tilemap.GetTile(x - 1, y + 1)?.Biome == Biome.Grasslands;

            if (!n && e && s && !w)
            {
                return "ext-NW";
            }
            else if (!n && e && s && w)
            {
                return "ext-N";
            }
            else if (!n && !e && s && w)
            {
                return "ext-NE";
            }
            else if (n && e && s && !w)
            {
                return "ext-W";
            }
            else if (n && e && s && w)
            {
                return "ext-C";
            }
            else if (n && !e && s && w)
            {
                return "ext-E";
            }
            else if (n && e && !s && !w)
            {
                return "ext-SW";
            }
            else if (n && e && !s && w)
            {
                return "ext-S";
            }
            else if (n && !e && !s && w)
            {
                return "ext-SE";
            }

            if (n && e && s && w && !se && nw && ne && sw)
            {
                return "int-NW";
            }
            else if (n && e && s && w && !sw && ne && nw && se)
            {
                return "int-NE";
            }
            else if (n && e && s && w && sw && ne && !nw && se)
            {
                return "int-SE";
            }
            else if (n && e && s && w && sw && !ne && nw && se)
            {
                return "int-SW";
            }

            return "int-C";
        }
    }
}
