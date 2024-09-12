using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Core
{
    public class TextureDecider
    {
        public static Texture2D DetermineBuildingWallTexture(int[,] wallMap, int[,] floorMap, int x, int y)
        {
            var n = false;
            var e = false;
            var s = false;
            var w = false;

            if (y > 0)
                n = wallMap[x, y - 1] == 1;
            if (x < wallMap.GetLength(0) - 1)
                e = wallMap[x + 1, y] == 1;
            if (y < wallMap.GetLength(1) - 1)
                s = wallMap[x, y + 1] == 1;
            if (x > 0)
                w = wallMap[x - 1, y] == 1;

            if (!n && e && s && !w)
                return Global.SpriteLibrary.BuildingBlockSprites["building-wall-nw"];

            if (!n && e && !s && w)
            {
                if (y == 0)
                    return Global.SpriteLibrary.BuildingBlockSprites["building-wall-n"];
                //if (floorMap[x, y - 1] == 1) TEMP DISABLE
                //    return Global.SpriteLibrary.BuildingBlockSprites["building-wall-s"];
                else
                    return Global.SpriteLibrary.BuildingBlockSprites["building-wall-n"];
            }

            if (!n && !e && s && w)
                return Global.SpriteLibrary.BuildingBlockSprites["building-wall-ne"];

            if (n && !e && s && !w)
            {
                if (x == 0)
                    return Global.SpriteLibrary.BuildingBlockSprites["building-wall-w"];
                //if (floorMap[x - 1, y] == 1) // TEMP DISABLE
                //    return Global.SpriteLibrary.BuildingBlockSprites["building-wall-e"];
                else
                    return Global.SpriteLibrary.BuildingBlockSprites["building-wall-w"];
            }
            if (n && e && !s && !w)
                return Global.SpriteLibrary.BuildingBlockSprites["building-wall-sw"];

            if (n && !e && !s && w)
                return Global.SpriteLibrary.BuildingBlockSprites["building-wall-se"];
            else
                return Global.DEBUG_TEXTURE;
        }

        public static Texture2D DetermineTownBaseTexture(int[,] map, int x, int y)
        {
            switch (map[x, y])
            {
                case 0:
                    return Global.SpriteLibrary.TownSprites["dirt-c"];

                case 1:
                    return Global.SpriteLibrary.TownSprites["dirt-c"];

                case 2:
                    return Global.SpriteLibrary.TownSprites["grass-c"];

                case 3:
                    return Global.SpriteLibrary.TownSprites["grass-c"];

                default:
                    return Global.SpriteLibrary.TownSprites["dirt-c"];
            }
        }

        public static Texture2D DetermineTownFenceTexture(int[,] map, int x, int y)
        {
            var n = false;
            var e = false;
            var s = false;
            var w = false;

            if (y > 0)
                n = map[x, y - 1] == 1;
            if (x > 0)
                w = map[x - 1, y] == 1;
            if (x < map.GetLength(0) - 1)
                e = map[x + 1, y] == 1;
            if (y < map.GetLength(1) - 1)
                s = map[x, y + 1] == 1;

            if (!n && e && s && !w)
                return Global.SpriteLibrary.TownSprites["fence-nw"];
            if (!n && e && !s && w)
            {
                if (y == 0)
                    return Global.SpriteLibrary.TownSprites["fence-n"];
                //else if (map[x, y - 1] == 1)
                //    return Global.SpriteLibrary.TownSprites["fence-s"];
                else
                    return Global.SpriteLibrary.TownSprites["fence-s"];
            }

            if (!n && !e && s && w)
                return Global.SpriteLibrary.TownSprites["fence-ne"];
            if (n && !e && s && !w)
            {
                if (x == 0)
                    return Global.SpriteLibrary.TownSprites["fence-w"];
                //else if (map[x - 1, y] == 1)
                //    return Global.SpriteLibrary.TownSprites["fence-e"];
                else
                    return Global.SpriteLibrary.TownSprites["fence-e"];
            }
            if (n && e && !s && !w)
                return Global.SpriteLibrary.TownSprites["fence-sw"];
            if (n && !e && !s && w)
                return Global.SpriteLibrary.TownSprites["fence-se"];
            else
                return Global.SpriteLibrary.TownSprites["fence-s"];
        }
    }
}
