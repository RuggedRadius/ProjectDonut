﻿using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.World;
using ProjectDonut.ProceduralGeneration.World.Generators;
using System;
using static ProjectDonut.ProceduralGeneration.Dungeons.DungeonGenerator;

namespace ProjectDonut.Core.Sprites
{
    public class NeighbourDataTiles
    {
        public NeighbourDataTiles(int[,] map, int x, int y)
        {
            if (y > 0 && x > 0)
                NorthWest = map[x - 1, y - 1] == 1;

            if (y > 0)
                North = map[x, y - 1] == 1;

            if (y > 0 && x < map.GetLength(0) - 1)
                NorthEast = map[x + 1, y - 1] == 1;

            if (x > 0)
                West = map[x - 1, y] == 1;

            if (x < map.GetLength(0) - 1)
                East = map[x + 1, y] == 1;

            if (y < map.GetLength(1) - 1 && x > 0)
                SouthWest = map[x - 1, y + 1] == 1;

            if (y < map.GetLength(1) - 1)
                South = map[x, y + 1] == 1;

            if (y < map.GetLength(1) - 1 && x < map.GetLength(0) - 1)
                SouthEast = map[x + 1, y + 1] == 1;
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

    public class RuleTiler
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

        //public static Texture2D DetermineBuildingWallCapTexture(int[,] wallMap, int[,] floorMap, Tilemap wallTiles, int x, int y)
        //{
        //    var nb = new NeighbourDataTiles(wallMap, x, y);

        //    // Explicit corners
        //    if (nb.North && nb.East && !nb.West)
        //    {
        //        return SpriteLib.Town.Walls["wall-cap-sw"];
        //    }

        //    if (nb.North && nb.West && !nb.East)
        //    {
        //        return SpriteLib.Town.Walls["wall-cap-se"];
        //    }

        //    if (nb.South && nb.East && !nb.West)
        //    {
        //        return SpriteLib.Town.Walls["wall-cap-nw"];
        //    }

        //    if (nb.South && nb.West && !nb.East)
        //    {
        //        return SpriteLib.Town.Walls["wall-cap-ne"];
        //    }

        //    if (nb.North && nb.East && nb.South && nb.West)
        //        return SpriteLib.Town.Walls["wall-cap-se"];



        //    if (nb.East && nb.South)
        //    {
        //        return SpriteLib.Town.Walls["wall-cap-nw"];
        //    }

        //    if (nb.South && nb.West)
        //    {
        //        return SpriteLib.Town.Walls["wall-cap-ne"];
        //    }



        //    if (nb.North && nb.West)
        //    {
        //        return SpriteLib.Town.Walls["wall-cap-se"];
        //    }



        //    if (!nb.North)
        //    {
        //        return SpriteLib.Town.Walls["wall-cap-n"];
        //    }

        //    if (!nb.East)
        //    {
        //        if (floorMap[x - 1, y] == 1)
        //            return SpriteLib.Town.Walls["wall-cap-e"];

        //        if (floorMap[x + 1, y] == 1)
        //            return SpriteLib.Town.Walls["wall-cap-w"];

        //        return Global.MISSING_TEXTURE;
        //    }

        //    if (!nb.South)
        //    {
        //        return SpriteLib.Town.Walls["wall-cap-s"];
        //    }

        //    if (!nb.West)
        //    {
        //        return SpriteLib.Town.Walls["wall-cap-w"];
        //    }

        //    return Global.MISSING_TEXTURE;
        //}

        public static class Town
        {
            public static Texture2D TownBaseTexture(int[,] map, int x, int y)
            {
                switch (map[x, y])
                {
                    case 0:
                        return SpriteLib.Town.Terrain["dirt-c"];

                    case 1:
                        return SpriteLib.Town.Terrain["dirt-c"];

                    case 2:
                        return SpriteLib.Town.Terrain["grass-c"];

                    case 3:
                        return SpriteLib.Town.Terrain["grass-c"];

                    default:
                        return SpriteLib.Town.Terrain["dirt-c"];
                }
            }

            public static Texture2D FenceTexture(int[,] map, int x, int y)
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
                    return SpriteLib.Town.Fences["fence-nw"];
                if (!n && e && !s && w)
                {
                    if (y == 0)
                        return SpriteLib.Town.Fences["fence-n"];
                    //else if (map[x, y - 1] == 1)
                    //    return SpriteLib.TownSprites["fence-s"];
                    else
                        return SpriteLib.Town.Fences["fence-s"];
                }

                if (!n && !e && s && w)
                    return SpriteLib.Town.Fences["fence-ne"];
                if (n && !e && s && !w)
                {
                    if (x == 0)
                        return SpriteLib.Town.Fences["fence-w"];
                    //else if (map[x - 1, y] == 1)
                    //    return SpriteLib.TownSprites["fence-e"];
                    else
                        return SpriteLib.Town.Fences["fence-e"];
                }
                if (n && e && !s && !w)
                    return SpriteLib.Town.Fences["fence-sw"];
                if (n && !e && !s && w)
                    return SpriteLib.Town.Fences["fence-se"];
                else
                    return SpriteLib.Town.Fences["fence-s"];
            }

            public static void ApplyRulesToPlotForFences(ref WorldChunk chunk)
            {
                var tilemap = chunk.Town.Tilemaps["fences"];

                foreach (var plot in chunk.Town.Plots)
                {
                    for (int i = plot.LocalBounds.Left; i < plot.LocalBounds.Right; i += Global.TileSize)
                    {
                        for (int j = plot.LocalBounds.Top; j < plot.LocalBounds.Bottom; j += Global.TileSize)
                        {
                            // NORTH-WEST
                            if (i == plot.LocalBounds.Left && j == plot.LocalBounds.Top)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Fences["fence-nw"];
                                continue;
                            }

                            // NORTH-EAST
                            if (i == plot.LocalBounds.Right - Global.TileSize && j == plot.LocalBounds.Top)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Fences["fence-ne"];
                                continue;
                            }

                            // SOUTH-WEST
                            if (i == plot.LocalBounds.Left && j == plot.LocalBounds.Bottom - Global.TileSize)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Fences["fence-sw"];
                                continue;
                            }

                            // SOUTH-EAST
                            if (i == plot.LocalBounds.Right - Global.TileSize && j == plot.LocalBounds.Bottom - Global.TileSize)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Fences["fence-se"];
                                continue;
                            }

                            // WEST
                            if (i == plot.LocalBounds.Left)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Fences["fence-w"];
                                continue;
                            }

                            // EAST
                            if (i == plot.LocalBounds.Right - Global.TileSize)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Fences["fence-e"];
                                continue;
                            }

                            // NORTH
                            if (j == plot.LocalBounds.Top)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Fences["fence-n"];
                                continue;
                            }

                            // SOUTH
                            if (j == plot.LocalBounds.Bottom - Global.TileSize)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Fences["fence-s"];
                                continue;
                            }
                        }
                    }
                }
            }

            public static void ApplyRulesToBuildingForWalls(ref WorldChunk chunk)
            {
                var tilemap = chunk.Town.Tilemaps["walls"];

                foreach (var plot in chunk.Town.Plots)
                {
                    for (int i = plot.Building.LocalBounds.Left; i < plot.Building.LocalBounds.Right; i += Global.TileSize)
                    {
                        for (int j = plot.Building.LocalBounds.Top; j < plot.Building.LocalBounds.Bottom; j += Global.TileSize)
                        {
                            // NORTH-WEST
                            if (i == plot.Building.LocalBounds.Left && j == plot.Building.LocalBounds.Top)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Walls["wall-nw"];
                                continue;
                            }

                            // NORTH-EAST
                            if (i == plot.Building.LocalBounds.Right - Global.TileSize && j == plot.Building.LocalBounds.Top)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Walls["wall-ne"];
                                continue;
                            }

                            // SOUTH-WEST
                            if (i == plot.Building.LocalBounds.Left && j == plot.Building.LocalBounds.Bottom - Global.TileSize)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Walls["wall-sw"];
                                continue;
                            }

                            // SOUTH-EAST
                            if (i == plot.Building.LocalBounds.Right - Global.TileSize && j == plot.Building.LocalBounds.Bottom - Global.TileSize)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Walls["wall-se"];
                                continue;
                            }

                            // WEST
                            if (i == plot.Building.LocalBounds.Left)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Walls["wall-w"];
                                continue;
                            }

                            // EAST
                            if (i == plot.Building.LocalBounds.Right - Global.TileSize)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Walls["wall-e"];
                                continue;
                            }

                            // NORTH
                            if (j == plot.Building.LocalBounds.Top)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Walls["wall-n"];
                                continue;
                            }

                            // SOUTH
                            if (j == plot.Building.LocalBounds.Bottom - Global.TileSize)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Walls["wall-s"];
                                continue;
                            }
                        }
                    }
                }
            }

            public static void ApplyRulesToBuildingForFloors(ref WorldChunk chunk)
            {
                var tilemap = chunk.Town.Tilemaps["floor"];

                foreach (var plot in chunk.Town.Plots)
                {
                    var left = plot.Building.LocalBounds.Left + Global.TileSize;
                    var right = plot.Building.LocalBounds.Right - Global.TileSize;
                    var top = plot.Building.LocalBounds.Top + Global.TileSize;
                    var bottom = plot.Building.LocalBounds.Bottom - Global.TileSize;

                    for (int i = plot.Building.LocalBounds.Left; i < plot.Building.LocalBounds.Right; i += Global.TileSize)
                    {
                        for (int j = plot.Building.LocalBounds.Top; j < plot.Building.LocalBounds.Bottom; j += Global.TileSize)
                        {
                            // NORTH-WEST
                            if (i == left && j == top)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Floor["floor-nw"];
                                continue;
                            }

                            // NORTH-EAST
                            if (i == right - Global.TileSize && j == top)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Floor["floor-ne"];
                                continue;
                            }

                            // SOUTH-WEST
                            if (i == left && j == bottom - Global.TileSize)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Floor["floor-sw"];
                                continue;
                            }

                            // SOUTH-EAST
                            if (i == right - Global.TileSize && j == bottom - Global.TileSize)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Floor["floor-se"];
                                continue;
                            }

                            // WEST
                            if (i == left)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Floor["floor-w"];
                                continue;
                            }

                            // EAST
                            if (i == right - Global.TileSize)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Floor["floor-e"];
                                continue;
                            }

                            // NORTH
                            if (j == top)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Floor["floor-n"];
                                continue;
                            }

                            // SOUTH
                            if (j == bottom - Global.TileSize)
                            {
                                tilemap.Map[i / Global.TileSize, j / Global.TileSize].Texture = SpriteLib.Town.Floor["floor-s"];
                                continue;
                            }
                        }
                    }
                }
            }

            public static Texture2D WallTexture(int[,] wallMap, int[,] floorMap, int x, int y)
            {
                var wn = new NeighbourDataTiles(wallMap, x, y);
                var fn = new NeighbourDataTiles(floorMap, x, y);
                var lib = SpriteLib.Town.Walls;

                // CORNERS
                if (!wn.North && wn.East && wn.South && !wn.West) // ES
                    return lib["wall-nw"];

                if (!wn.North && !wn.East && wn.South && wn.West) // SW
                    return lib["wall-ne"];

                if (wn.North && !wn.East && !wn.South && wn.West) // NW
                    return lib["wall-se"];

                if (wn.North && wn.East && !wn.South && !wn.West) // NE
                    return lib["wall-sw"];

                // STRAIGHTS
                if (!wn.North && wn.East && !wn.South && wn.West) // EW
                    return lib["wall-n"];

                if (wn.North && !wn.East && wn.South && !wn.West) // NS
                    return lib["wall-e"];

                // JUNCTIONS
                if (wn.North && wn.East && wn.South && wn.West) // NESW
                    return lib["wall-junc-nesw"];

                if (wn.North && wn.East && !wn.South && wn.West) // NEW
                    return lib["wall-junc-new"];

                if (!wn.North && wn.East && wn.South && wn.West) // ESW
                    return lib["wall-junc-esw"];

                if (wn.North && wn.East && wn.South && !wn.West) // NES
                    return lib["wall-junc-nes"];

                if (wn.North && !wn.East && wn.South && wn.West) // NSW
                    return lib["wall-junc-nsw"];

                // CAPS
                if (wn.North && !wn.East && !wn.South && !wn.West)
                    return lib["wall-int-cap-n"];
                if (!wn.North && wn.East && !wn.South && !wn.West)
                    return lib["wall-int-cap-e"];
                if (!wn.North && !wn.East && wn.South && !wn.West)
                    return lib["wall-int-cap-s"];
                if (!wn.North && !wn.East && !wn.South && wn.West)
                    return lib["wall-int-cap-w"];

                // PILLAR
                if (fn.North && fn.NorthEast && fn.East && fn.SouthEast && fn.South && fn.SouthWest && fn.West && fn.NorthWest)
                    return lib["wall-pillar"];

                return Global.MISSING_TEXTURE;
            }


            public static Texture2D FloorTile(int[,] floorMap, int[,] wallMap, int x, int y)
            {
                var fn = new NeighbourDataTiles(floorMap, x, y);
                var wn = new NeighbourDataTiles(wallMap, x, y);
                var lib = SpriteLib.Town.Floor;

                // ODD - SPECIFIC
                if (!fn.North && !fn.NorthEast && fn.East && !fn.SouthEast && !fn.South && !fn.SouthWest && !fn.West && !fn.NorthWest)
                {
                    if (wn.NorthWest && wn.North && wn.NorthEast && wn.SouthWest && wn.West)
                    {
                        return lib["floor-odd-missing-nw,n,ne,se,s,sw,w"];
                    }
                }

                if (!fn.North && !fn.NorthEast && !fn.East && !fn.SouthEast && !fn.South && !fn.SouthWest && fn.West && !fn.NorthWest)
                {
                    if (wn.NorthWest && wn.North && wn.NorthEast && wn.SouthEast && wn.East)
                    {
                        return lib["floor-odd-missing-nw,n,ne,e,se,s,sw"];
                    }
                }

                if (fn.North && !fn.NorthEast && !fn.East && !fn.SouthEast && !fn.South && !fn.SouthWest && !fn.West && !fn.NorthWest)
                {
                    if (wn.SouthEast && wn.South && wn.SouthWest && wn.West && wn.NorthWest)
                    {
                        return lib["floor-odd-missing-ew"];
                    }
                }

                if (fn.North && fn.NorthEast && fn.East && !fn.SouthEast && !fn.South && !fn.SouthWest && !fn.West && !fn.NorthWest)
                {
                    if (wn.NorthWest && wn.West && wn.SouthWest && !wn.South && !wn.SouthEast)
                    {
                        return lib["floor-odd-missing-nw,n,ne,se,s,sw,w2"];
                    }

                    if (wn.NorthWest && wn.West && wn.SouthWest && !wn.SouthEast)
                    {
                        return lib["floor-odd-missing-nw,n,ne,se,s,sw,w"];
                    }
                }

                if (fn.North && !fn.NorthEast && !fn.East && !fn.SouthEast && !fn.South && !fn.SouthWest && fn.West && fn.NorthWest)
                {
                    if (!wn.NorthWest && !wn.West && !wn.SouthWest && !wn.South && wn.SouthEast && wn.East && wn.NorthEast)
                    {
                        return lib["floor-odd-missing-nw,n,ne,se,s,sw,w2"];
                        //return lib["floor-odd-missing-3"];
                    }
                }

                if (fn.NorthWest && fn.North && !fn.NorthEast && !fn.East && !fn.SouthEast && fn.South && !fn.SouthWest && fn.West)
                    return lib["floor-odd-missing-4"];

                if (fn.NorthWest && fn.North && !fn.NorthEast && fn.East && !fn.SouthEast && !fn.South && !fn.SouthWest && fn.West)
                    return lib["floor-ne-ext-stair"];



                // CORNERS
                if (!fn.North && fn.East && fn.South && !fn.West) // ES
                    return lib["floor-nw"];

                if (!fn.North && !fn.East && fn.South && fn.West) // SW
                    return lib["floor-ne"];

                if (fn.North && !fn.East && !fn.South && fn.West) // NW
                    return lib["floor-se"];

                if (fn.North && fn.East && !fn.South && !fn.West) // NE
                {

                    return lib["floor-sw"];
                }

                // CORNERS - INTERNAL
                if (fn.North && fn.East && fn.South && fn.West && !fn.SouthWest && !fn.SouthEast)
                    return lib["floor-doublecorner-bottom"];

                if (fn.North && fn.East && fn.South && fn.West && !fn.NorthWest && !fn.NorthEast)
                    return lib["floor-doublecorner-top"];

                if (fn.North && fn.East && fn.South && fn.West && !fn.NorthEast && !fn.SouthEast)
                    return lib["floor-ne-ext-stair"];

                if (fn.North && fn.East && fn.South && fn.West && !fn.NorthWest && !fn.SouthWest)
                    return lib["floor-nw-ext-stair"];


                // CORNERS - EXTERNAL
                if (fn.North && !fn.NorthEast && fn.East && fn.SouthEast && fn.South && fn.SouthWest && fn.West && fn.NorthWest)
                    return lib["floor-ne-ext-stair"];

                if (fn.North && fn.NorthEast && fn.East && fn.SouthEast && fn.South && fn.SouthWest && fn.West && !fn.NorthWest)
                {
                    //if (wn.NorthWest)
                    //    return lib["floor-ne-ext-stair"];

                    return lib["floor-nw-ext-stair"];
                }

                if (fn.North && fn.NorthEast && fn.East && !fn.SouthEast && fn.South && fn.SouthWest && fn.West && fn.NorthWest)
                {
                    if (wn.SouthEast)
                        return lib["floor-c"];

                    return lib["floor-se-ext-stair"];
                }

                if (fn.North && fn.NorthEast && fn.East && fn.SouthEast && fn.South && !fn.SouthWest && fn.West && fn.NorthWest)
                {
                    if (wn.SouthWest)
                        return lib["floor-c"];

                    return lib["floor-sw-ext-stair"];
                }


                // ODD
                if (fn.North && !fn.NorthEast && !fn.East && !fn.SouthEast && fn.South && fn.SouthWest && fn.West && !fn.NorthWest)
                {
                    return lib["floor-odd-nsw-only"];
                }

                if (fn.North && !fn.NorthEast && fn.East && fn.SouthEast && fn.South && !fn.SouthWest && !fn.West && !fn.NorthWest)
                {
                    return lib["floor-odd-missing-ne,sw,w,nw"];
                }


                if (fn.North && fn.NorthEast && fn.East && fn.SouthEast && fn.South && !fn.West && fn.NorthWest)
                {
                    if (wn.West)
                    {
                        return lib["floor-odd-missing-w-[o]sw"];
                    }
                    else
                    {
                        return lib["floor-w"];
                    }
                }
                //return lib["floor-odd-missing-w-[o]sw"];

                if (fn.North && fn.NorthEast && !fn.East && fn.South && fn.SouthWest && fn.West && fn.NorthWest)
                {
                    if (wn.East)
                    {
                        return lib["floor-odd-missing-e-[o]se"];
                    }
                    else
                    {
                        return lib["floor-e"];
                    }
                }




                if (fn.North && fn.NorthEast && !fn.East && fn.SouthEast && fn.South && !fn.West && fn.NorthWest) // Vertical doorway
                    return lib["floor-doorway-vertical"];

                if (fn.North && !fn.East && fn.South && !fn.West)
                    return lib["floor-odd-missing-ew"];

                // JUNCTIONS
                if (fn.North && fn.East && !fn.South && fn.West) // NEW
                    return lib["floor-s"];

                if (!fn.North && fn.East && fn.South && fn.West) // ESW
                    return lib["floor-n"];

                if (fn.North && fn.East && fn.South && !fn.West) // NES
                    return lib["floor-w"];

                if (fn.North && !fn.East && fn.South && fn.West) // NSW
                {
                    if (!fn.NorthEast && !fn.East && !fn.SouthEast && !fn.SouthWest && !fn.NorthWest)
                    {
                        return lib["floor-odd-nsw-only"];
                    }



                    return lib["floor-e"];
                }

                if (fn.North && !fn.East && fn.South && !fn.West) // NS
                    return lib["floor-junc-ew"];

                if (!fn.North && fn.East && !fn.South && fn.West) // NS
                    return lib["floor-junc-ns"];


                //if (!nb.NorthWest || !nb.North || !nb.NorthEast 
                //    || !nb.West || !nb.East 
                //    || !nb.SouthWest || !nb.South || !nb.SouthEast)
                //    return lib["floor-c"];



                if (fn.North && fn.East && fn.South && fn.West) // NESW
                    return lib["floor-c"];


                // EXTERNAL CORNERS
                // ...




                //return lib["floor-c"];
                return Global.MISSING_TEXTURE;
            }
        }

        public static class World
        {
            public static Texture2D DetermineTerrainTexture(int x, int y, int biomeValue, int[,] heightData, WorldMapSettings settings)
            {
                int heightValue = heightData[x, y];
                var biome = (Biome)biomeValue;

                if (heightValue >= settings.GroundHeightMin)
                {
                    // COAST
                    if (heightValue >= 30 && heightValue < 34)
                    {
                        switch (biome)
                        {
                            case Biome.Desert:
                                return SpriteLib.World.Desert["coast-c"];

                            case Biome.Grasslands:
                                return SpriteLib.World.Grasslands["coast-c"];

                            case Biome.Winterlands:
                                return SpriteLib.World.Winterlands["coast-c"];

                            case Biome.Ashlands:
                                return SpriteLib.World.Ashlands["coast-c"];

                            case Biome.Wetlands:
                                return SpriteLib.World.Wetlands["coast-c"];

                            default:
                                return SpriteLib.World.Grasslands["coast-c"];
                        }
                    }
                    else
                    {
                        switch (biome)
                        {
                            case Biome.Desert:
                                return SpriteLib.World.Desert["desert-c"];

                            case Biome.Grasslands:
                                return SpriteLib.World.Grasslands["grass-c"];

                            case Biome.Winterlands:
                                return SpriteLib.World.Winterlands["winterlands-c"];

                            case Biome.Ashlands:
                                return SpriteLib.World.Ashlands["ashlands-c"];

                            case Biome.Wetlands:
                                return SpriteLib.World.Wetlands["wetlands-c"];

                            default:
                                return SpriteLib.World.Grasslands["grass-c"];
                        }
                    }
                }
                else
                {
                    if (heightValue >= settings.WaterHeightMin)
                    {
                        return SpriteLib.GetSprite("coast-inv-c");
                    }
                    else
                    {
                        return SpriteLib.GetSprite("deepwater-C");
                    }
                }
            }
        }
    }
}
