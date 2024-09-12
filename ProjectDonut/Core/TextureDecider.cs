using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Core
{
    public class NeighbourTiles
    {
        public NeighbourTiles(int[,] map, int x, int y)
        {
            if (y > 0 && x > 0)
                NorthWest = map[x, y - 1] == 1;

            if (y > 0)
                North = map[x, y - 1] == 1;

            if (y > 0 && x < map.GetLength(0) - 1)
                NorthEast = map[x, y - 1] == 1;

            if (x > 0)
                West = map[x - 1, y] == 1;

            if (x < map.GetLength(0) - 1)
                East = map[x + 1, y] == 1;

            if (y < map.GetLength(1) - 1 && x > 0)
                SouthWest = map[x, y + 1] == 1;

            if (y < map.GetLength(1) - 1)
                South = map[x, y + 1] == 1;

            if (y < map.GetLength(1) - 1 && x < map.GetLength(0) - 1)
                SouthEast = map[x, y + 1] == 1;
        }

        public bool NorthWest { get; set; }
        public bool North { get; set; }
        public bool NorthEast { get; set; }

        public bool West { get; set; }
        public bool East { get; set; }

        public bool SouthWest { get; set; }
        public bool South { get; set; }
        public bool SouthEast { get; set; }
    }

    public class TextureDecider
    {
        public static Texture2D DetermineBuildingWallCapTexture(int[,] wallMap, int[,] floorMap, int x, int y)
        {
            var nb = new NeighbourTiles(wallMap, x, y);

            if (nb.North && nb.South)
            {
                if (x == 0)
                    return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-w"];
                if (floorMap[x - 1, y] == 1)
                    return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-e"];
                else
                    return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-w"];
            }

            if (nb.East && nb.West)
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-n"];

            if (nb.North && nb.East)
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-sw"];

            if (nb.North && nb.West)
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-se"];

            if (nb.South && nb.East)
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-nw"];

            if (nb.South && nb.West)
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-ne"];

            return Global.DEBUG_TEXTURE;



            if (!nb.North && nb.East && nb.South && !nb.West)
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-nw"];

            if (!nb.North && nb.East && !nb.South && nb.West)
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-n"];

            if (!nb.North && !nb.East && nb.South && nb.West)
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-ne"];

            if (nb.North && !nb.East && nb.South && !nb.West)
            {
                if (x == 0)
                    return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-w"];
                if (floorMap[x - 1, y] == 1)
                    return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-e"];
                else
                    return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-w"];
            }

            if (nb.North && nb.East && !nb.South && !nb.West)
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-sw"];

            if (nb.North && !nb.East && !nb.South && nb.West)
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-se"];
            else
            {
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-n"];
            }
        }

        public static Texture2D DetermineBuildingWallTexture(int[,] wallMap, int[,] floorMap, int x, int y)
        {
            var nb = new NeighbourTiles(wallMap, x, y);

            if (!nb.North)
            {
                return Global.SpriteLibrary.BuildingBlockSprites["wall-face"];
            }

            if (!nb.South)
            {
                return Global.SpriteLibrary.BuildingBlockSprites["wall-face"];
            }

            if (nb.North || nb.South)
            {
                if (nb.East || nb.West)
                    return Global.SpriteLibrary.BuildingBlockSprites["wall-face"];
            }

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
