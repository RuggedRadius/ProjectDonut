using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.World;
using static ProjectDonut.ProceduralGeneration.Dungeons.DungeonGenerator;

namespace ProjectDonut.Core
{
    public class NeighbourDataTiles
    {
        public NeighbourDataTiles(int[,] map, int x, int y)
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
        /*
         * NOTE TO FUTURE SELF:
         * 
         * I THINK THE WAY TO DO THIS CLEANLY IS TO HAVE A TILEMAP FOR EACH ROOM, AND THEN
         * CONSOLIDATE DATE THEM AT THE END. THIS WAY WE CAN DETERMINE THE TEXTURE FOR EACH
         * WALL NICELY WITHOUT COMPLEX RULES FOR BACK-TO-BACK WALLS ETC.
         * 
         * 
         */

        public static Texture2D DetermineBuildingWallCapTexture(int[,] wallMap, int[,] floorMap, Tilemap wallTiles, int x, int y)
        {
            var nb = new NeighbourDataTiles(wallMap, x, y);

            // Explicit corners
            if (nb.North && nb.East && !nb.West)
            {
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-sw"];
            }

            if (nb.North && nb.West && !nb.East)
            {
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-se"];
            }

            if (nb.South && nb.East && !nb.West)
            {
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-nw"];
            }

            if (nb.South && nb.West && !nb.East)
            {
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-ne"];
            }

            if (nb.North && nb.East && nb.South && nb.West)
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-se"];



            if (nb.East && nb.South)
            {
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-nw"];
            }

            if (nb.South && nb.West)
            {
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-ne"];
            }



            if (nb.North && nb.West)
            {
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-se"];
            }



            if (!nb.North)
            {
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-n"];
            }

            if (!nb.East)
            {
                if (floorMap[x - 1, y] == 1)
                    return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-e"];

                if (floorMap[x + 1, y] == 1)
                    return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-w"];

                return Global.MISSING_TEXTURE;
            }

            if (!nb.South)
            {
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-s"];
            }

            if (!nb.West)
            {
                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-w"];
            }

            return Global.MISSING_TEXTURE;
        }

        public static Texture2D DetermineBuildingWallCapTexturePass2(int[,] wallMap, int[,] floorMap, Tilemap wallTiles, int x, int y)
        {
            //Global.SpriteLibrary.BuildingBlockSprites["wall-cap-n"]

            var nw = wallTiles.Map[x-1, y-1]?.Texture;
            var n = wallTiles.Map[x, y-1]?.Texture;
            var ne = wallTiles.Map[x+1, y-1]?.Texture;
            var w = wallTiles.Map[x-1, y]?.Texture;
            var e = wallTiles.Map[x+1, y]?.Texture;
            var sw = wallTiles.Map[x-1, y+1]?.Texture;
            var s = wallTiles.Map[x, y+1]?.Texture;
            var se = wallTiles.Map[x+1, y+1]?.Texture;

            if (s == Global.SpriteLibrary.BuildingBlockSprites["wall-face"])
            {
                if (e == Global.SpriteLibrary.BuildingBlockSprites["wall-face"])
                    return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-ne"];

                if (w == Global.SpriteLibrary.BuildingBlockSprites["wall-face"])
                    return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-nw"];

                return Global.SpriteLibrary.BuildingBlockSprites["wall-cap-n"];
            }

            return null;
        }

        public static Texture2D DetermineBuildingWallTexture(int[,] wallMap, int[,] floorMap, int x, int y)
        {
            var nb = new NeighbourDataTiles(wallMap, x, y);

            // Wall internal
            if (nb.South && nb.West)
                return Global.SpriteLibrary.BuildingBlockSprites["wall-ne"];

            if (nb.North && nb.East)
            {
                if (floorMap[x + 1, y] == 1 && floorMap[x - 1, y] == 0)
                    return Global.SpriteLibrary.BuildingBlockSprites["wall-e"];
                else
                    return Global.SpriteLibrary.BuildingBlockSprites["wall-sw"];
                //return Global.SpriteLibrary.BuildingBlockSprites["wall-sw"];
            }
                

            if (nb.North && nb.West)
                return Global.SpriteLibrary.BuildingBlockSprites["wall-se"];

            if (nb.South && nb.East)
                return Global.SpriteLibrary.BuildingBlockSprites["wall-nw"];

            if (nb.North && nb.South)
            {
                // Determine east or west...
                if (floorMap[x - 1, y] == 1)
                    return Global.SpriteLibrary.BuildingBlockSprites["wall-e"];
                else
                    return Global.SpriteLibrary.BuildingBlockSprites["wall-w"];
            }
            if (nb.East && nb.West)
            {
                // Determine north or south...
                if (floorMap[x, y - 1] == 1)
                    return Global.SpriteLibrary.BuildingBlockSprites["wall-s"];
                else
                    return Global.SpriteLibrary.BuildingBlockSprites["wall-n"];
            }

            return Global.MISSING_TEXTURE;


            //if (nb.North != DungeonInteriorTileType.Floor && e == DungeonInteriorTileType.Wall && s == DungeonInteriorTileType.Wall && w != DungeonInteriorTileType.Floor && se == DungeonInteriorTileType.Floor)
            //    return GetRandomTextureFor("wall-nw");

            //if (n != DungeonInteriorTileType.Floor && e == DungeonInteriorTileType.Wall && s == DungeonInteriorTileType.Floor && w == DungeonInteriorTileType.Wall)
            //    return GetRandomTextureFor("wall-n");

            //if (n != DungeonInteriorTileType.Floor && e != DungeonInteriorTileType.Floor && s == DungeonInteriorTileType.Wall && w == DungeonInteriorTileType.Wall && sw == DungeonInteriorTileType.Floor)
            //    return GetRandomTextureFor("wall-ne");

            //if (n == DungeonInteriorTileType.Wall && e == DungeonInteriorTileType.Floor && s == DungeonInteriorTileType.Wall && w != DungeonInteriorTileType.Floor)
            //    return GetRandomTextureFor("wall-w");

            //if (n == DungeonInteriorTileType.Wall && e != DungeonInteriorTileType.Floor && s == DungeonInteriorTileType.Wall && w == DungeonInteriorTileType.Floor)
            //    return GetRandomTextureFor("wall-e");

            //if (n == DungeonInteriorTileType.Wall && e == DungeonInteriorTileType.Wall && s != DungeonInteriorTileType.Floor && w != DungeonInteriorTileType.Floor && ne == DungeonInteriorTileType.Floor)
            //    return GetRandomTextureFor("wall-sw");

            //if (n == DungeonInteriorTileType.Floor && e == DungeonInteriorTileType.Wall && s != DungeonInteriorTileType.Floor && w == DungeonInteriorTileType.Wall)
            //    return GetRandomTextureFor("wall-s");

            //if (n == DungeonInteriorTileType.Wall && e != DungeonInteriorTileType.Floor && s != DungeonInteriorTileType.Floor && w == DungeonInteriorTileType.Wall && nw == DungeonInteriorTileType.Floor)
            //    return GetRandomTextureFor("wall-se");

            //// Walls external
            //if (n == DungeonInteriorTileType.Floor && e == DungeonInteriorTileType.Wall && s == DungeonInteriorTileType.Wall && w == DungeonInteriorTileType.Floor)
            //    return GetRandomTextureFor("wall-ext-nw");

            //if (n == DungeonInteriorTileType.Floor && e == DungeonInteriorTileType.Floor && s == DungeonInteriorTileType.Wall && w == DungeonInteriorTileType.Wall)
            //    return GetRandomTextureFor("wall-ext-ne");

            //if (n == DungeonInteriorTileType.Wall && e == DungeonInteriorTileType.Wall && s == DungeonInteriorTileType.Floor && w == DungeonInteriorTileType.Floor)
            //    return GetRandomTextureFor("wall-ext-sw");

            //if (n == DungeonInteriorTileType.Wall && e == DungeonInteriorTileType.Floor && s == DungeonInteriorTileType.Floor && w == DungeonInteriorTileType.Wall)
            //    return GetRandomTextureFor("wall-ext-se");
        }


        //public static Texture2D DetermineBuildingWallTextureOLD(int[,] wallMap, int[,] floorMap, int x, int y)
        //{
        //    var nb = new NeighbourDataTiles(wallMap, x, y);

        //    if (!nb.North)
        //    {
        //        return Global.SpriteLibrary.BuildingBlockSprites["wall-face"];
        //    }

        //    if (!nb.South)
        //    {
        //        return Global.SpriteLibrary.BuildingBlockSprites["wall-face"];
        //    }

        //    if (nb.North || nb.South)
        //    {
        //        if (nb.North && nb.South && nb.East && !nb.West)
        //            return Global.DEBUG_TEXTURE;

        //        if (nb.East || nb.West)
        //            return Global.SpriteLibrary.BuildingBlockSprites["wall-face"];
        //    }

        //    return Global.DEBUG_TEXTURE;
        //}


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
    
    
        public static class World
        {
            public static Texture2D DetermineTerrainTexture(int x, int y, int biomeValue, int[,] heightData, WorldMapSettings settings)
            {
                int heightValue = heightData[x, y];
                var biome = (Biome)biomeValue;

                if (heightValue >= settings.GroundHeightMin)
                {
                    if (heightValue >= 30 && heightValue < 34)
                    {
                        return Global.SpriteLibrary.GetSprite("beach");
                    }

                    //if (heightValue > 30 && heightValue % 10 == 0)
                    //{
                    //    return DetermineCliffTexture(x, y, heightData);
                    //}
                    else
                    {
                        switch (biome)
                        {
                            case Biome.Desert:
                                return Global.SpriteLibrary.GetSprite("desert");

                            case Biome.Grasslands:
                                return Global.SpriteLibrary.GetSprite("grasslands");

                            case Biome.Winterlands:
                                return Global.SpriteLibrary.GetSprite("winterlands");

                            default:
                                return Global.SpriteLibrary.GetSprite("grasslands");
                        }
                    }
                }
                else
                {
                    if (heightValue >= settings.WaterHeightMin)
                    {
                        return Global.SpriteLibrary.GetSprite("coast-inv");
                    }
                    else
                    {
                        return Global.SpriteLibrary.GetSprite("deepwater-C");
                    }
                }
            }
        }
    }
}
