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

                    //if (isNorthWestCoast(x, y))
                    //{
                    //    tile.Texture = spriteLib.GetSprite("coast-NW");
                    //}

                    //if (isNorthEastCoast(x, y))
                    //{
                    //    tile.Texture = spriteLib.GetSprite("coast-NE");
                    //}

                    //if (isSouthEastCoast(x, y))
                    //{
                    //    tile.Texture = spriteLib.GetSprite("coast-SE");
                    //}

                    //if (isSouthWestCoast(x, y))
                    //{
                    //    tile.Texture = spriteLib.GetSprite("coast-SW");
                    //}
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
