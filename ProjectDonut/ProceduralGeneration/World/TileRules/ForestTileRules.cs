using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            int counter = 0;
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
                    if (x == 0 || y == 0 || x == tilemap.Map.GetLength(0) - 1 || y == tilemap.Map.GetLength(1) - 1)
                    {
                        counter++;
                        continue;
                    }

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
                        tile.Texture = spriteLib.GetSprite("forest-NW");
                    }
                    else if (!n && e && s && w)
                    {
                        tile.Texture = spriteLib.GetSprite("forest-N");
                    }
                    else if (!n && !e && s && w)
                    {
                        tile.Texture = spriteLib.GetSprite("forest-NE");
                    }
                    else if (n && e && s && !w)
                    {
                        tile.Texture = spriteLib.GetSprite("forest-W");
                    }
                    else if (n && e && s && w)
                    {
                        tile.Texture = spriteLib.GetSprite("forest-C");
                    }
                    else if (n && !e && s && w)
                    {
                        tile.Texture = spriteLib.GetSprite("forest-E");
                    }
                    else if (n && e && !s && !w)
                    {
                        tile.Texture = spriteLib.GetSprite("forest-SW");
                    }
                    else if (n && e && !s && w)
                    {
                        tile.Texture = spriteLib.GetSprite("forest-S");
                    }
                    else if (n && !e && !s && w)
                    {
                        tile.Texture = spriteLib.GetSprite("forest-SE");
                    }
                    
                    if (n && e && s && w && !se && nw && ne && sw)
                    {
                        tile.Texture = spriteLib.GetSprite("forest-inv-NW");
                    }
                    else if (n && e && s && w && !sw && ne && nw && se)
                    {
                        tile.Texture = spriteLib.GetSprite("forest-inv-NE");
                    }
                    else if (n && e && s && w && sw && ne && !nw && se)
                    {
                        tile.Texture = spriteLib.GetSprite("forest-inv-SE");
                    }
                    else if (n && e && s && w && sw && !ne && nw && se)
                    {
                        tile.Texture = spriteLib.GetSprite("forest-inv-SW");
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
    }
}
