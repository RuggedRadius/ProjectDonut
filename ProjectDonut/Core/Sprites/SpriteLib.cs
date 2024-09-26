using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Tools;
using System;
using System.Collections.Generic;

namespace ProjectDonut.Core.Sprites
{
    public class SpriteLib
    {
        // TODO: Remove these, create smaller independent dictionaries in relevant subclass
        // NO NEED TO STORE SPRITESHEETS
        public static Dictionary<string, Texture2D> lib;
        private static Dictionary<string, Texture2D> sheets;
        private static Texture2D spriteSheetTiles;
        private static Texture2D spriteSheetBiomes;
        private static Texture2D spriteSheetForest;
        private static Texture2D spriteSheetWinter;
        private static Texture2D spriteSheetMountain;
        private static Texture2D spriteSheetCastle;
        private static Texture2D spriteSheetTown;
        

        public SpriteLib()
        {
            LoadSpriteLibrary();
        }

        public static class Mineables
        {
            public static Dictionary<string, List<Texture2D>> Sprites;
            public static Dictionary<string, Texture2D> UIIcons;

            public static void Load()
            {
                LoadMineables();
            }

            private static void LoadMineables()
            {
                Sprites = new Dictionary<string, List<Texture2D>>();

                var trees = new List<Texture2D>();
                trees.Add(Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Tree2"));
                Sprites.Add("tree-02", trees);

                var treeStumps = new List<Texture2D>();
                treeStumps.Add(Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Tree-stump-export"));
                Sprites.Add("tree-stump", treeStumps);

                var treesWinter = new List<Texture2D>();
                treesWinter.Add(Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Tree2-winter"));
                Sprites.Add("tree-02-winter", treesWinter);

                var rocks = new List<Texture2D>();
                rocks.Add(Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Rock01"));
                Sprites.Add("rock", rocks);

                var cactus = new List<Texture2D>();
                cactus.Add(Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Cactus01"));
                Sprites.Add("cactus-01", cactus);

                var rockSmashed = new List<Texture2D>();
                var rockSheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Rock01");
                rockSmashed.Add(SpriteLib.ExtractSprite(rockSheet, 4 * Global.TileSize, 0, Global.TileSize, Global.TileSize));
                Sprites.Add("rock-smashed", rockSmashed);
            }          
        }

        public static class Town
        {
            public static Dictionary<string, Texture2D> Terrain;
            public static Dictionary<string, Texture2D> Doodads;
            public static Dictionary<string, Texture2D> Fences;
            public static Dictionary<string, Texture2D> Floor;
            public static Dictionary<string, Texture2D> Walls;
            public static Dictionary<string, Texture2D> Stairs;
            public static Dictionary<string, Texture2D> Doors;
            public static Dictionary<string, Texture2D> Roof;

            public static void Load()
            {
                Terrain = new Dictionary<string, Texture2D>();
                Doodads = new Dictionary<string, Texture2D>();
                Fences = new Dictionary<string, Texture2D>();
                Floor = new Dictionary<string, Texture2D>();
                Walls = new Dictionary<string, Texture2D>();
                Stairs = new Dictionary<string, Texture2D>();
                Doors = new Dictionary<string, Texture2D>();
                Roof = new Dictionary<string, Texture2D>();

                var townTerrainSheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/Town/Town01");
                var buildingBlocksSheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/Town/house_sprites3");
                var roofSheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/Town/RoofTiles");

                LoadGrass(townTerrainSheet);
                LoadDirt(townTerrainSheet);
                LoadFence(townTerrainSheet);

                LoadWalls(buildingBlocksSheet);
                LoadFloor(buildingBlocksSheet);
                LoadStairs(buildingBlocksSheet);
                LoadDoors(buildingBlocksSheet);

                LoadRoof(roofSheet);

                LoadDoodads();
            }

            private static void LoadGrass(Texture2D sheet)
            {
                Terrain.Add("grass-nw", ExtractSprite(sheet, 0, 0));
                Terrain.Add("grass-n", ExtractSprite(sheet, 1, 0));
                Terrain.Add("grass-ne", ExtractSprite(sheet, 2, 0));
                Terrain.Add("grass-w", ExtractSprite(sheet, 0, 1));
                Terrain.Add("grass-c", ExtractSprite(sheet, 1, 1));
                Terrain.Add("grass-e", ExtractSprite(sheet, 2, 1));
                Terrain.Add("grass-sw", ExtractSprite(sheet, 0, 2));
                Terrain.Add("grass-s", ExtractSprite(sheet, 1, 2));
                Terrain.Add("grass-se", ExtractSprite(sheet, 2, 2));
            }

            private static void LoadDirt(Texture2D sheet)
            {
                Terrain.Add("dirt-nw", ExtractSprite(sheet, 3, 0));
                Terrain.Add("dirt-n", ExtractSprite(sheet, 4, 0));
                Terrain.Add("dirt-ne", ExtractSprite(sheet, 5, 0));
                Terrain.Add("dirt-w", ExtractSprite(sheet, 3, 1));
                Terrain.Add("dirt-c", ExtractSprite(sheet, 4, 1));
                Terrain.Add("dirt-e", ExtractSprite(sheet, 5, 1));
                Terrain.Add("dirt-sw", ExtractSprite(sheet, 3, 2));
                Terrain.Add("dirt-s", ExtractSprite(sheet, 4, 2));
                Terrain.Add("dirt-se", ExtractSprite(sheet, 5, 2));
            }

            private static void LoadFence(Texture2D sheet)
            {
                Fences.Add("fence-nw", ExtractSprite(sheet, 6, 0));
                Fences.Add("fence-n", ExtractSprite(sheet, 7, 0));
                Fences.Add("fence-ne", ExtractSprite(sheet, 8, 0));
                Fences.Add("fence-w", ExtractSprite(sheet, 6, 1));
                Fences.Add("fence-c", ExtractSprite(sheet, 7, 1));
                Fences.Add("fence-e", ExtractSprite(sheet, 8, 1));
                Fences.Add("fence-sw", ExtractSprite(sheet, 6, 2));
                Fences.Add("fence-s", ExtractSprite(sheet, 7, 2));
                Fences.Add("fence-se", ExtractSprite(sheet, 8, 2));
            }

            private static void LoadWalls(Texture2D sheet)
            {
                Walls.Add("wall-nw", ExtractSprite(sheet, 4, 1));
                Walls.Add("wall-n", ExtractSprite(sheet, 5, 1));
                Walls.Add("wall-ne", ExtractSprite(sheet, 6, 3));
                Walls.Add("wall-w", ExtractSprite(sheet, 4, 2));
                Walls.Add("wall-e", ExtractSprite(sheet, 6, 2));
                Walls.Add("wall-sw", ExtractSprite(sheet, 4, 3));
                Walls.Add("wall-s", ExtractSprite(sheet, 5, 3));
                Walls.Add("wall-se", ExtractSprite(sheet, 6, 3));
                Walls.Add("wall-junc-esw", ExtractSprite(sheet, 9, 2));
                Walls.Add("wall-junc-new", ExtractSprite(sheet, 4, 4));
                Walls.Add("wall-junc-nes", ExtractSprite(sheet, 8, 1));
                Walls.Add("wall-junc-nsw", ExtractSprite(sheet, 10, 2));
                Walls.Add("wall-junc-nesw", ExtractSprite(sheet, 8, 2));
                Walls.Add("wall-int-cap-n", ExtractSprite(sheet, 8, 3));
                Walls.Add("wall-int-cap-e", ExtractSprite(sheet, 5, 4));
                Walls.Add("wall-int-cap-s", ExtractSprite(sheet, 7, 1));
                Walls.Add("wall-int-cap-w", ExtractSprite(sheet, 6, 4));
                Walls.Add("wall-pillar", ExtractSprite(sheet, 5, 2));
            }

            private static void LoadFloor(Texture2D blockSheet)
            {
                Floor.Add("floor-nw", SpriteLib.ExtractSprite(blockSheet, 1, 1));
                Floor.Add("floor-n", SpriteLib.ExtractSprite(blockSheet, 2, 1));
                Floor.Add("floor-ne", SpriteLib.ExtractSprite(blockSheet, 3, 1));
                Floor.Add("floor-w", SpriteLib.ExtractSprite(blockSheet, 1, 2));
                Floor.Add("floor-c", SpriteLib.ExtractSprite(blockSheet, 2, 2));
                Floor.Add("floor-e", SpriteLib.ExtractSprite(blockSheet, 3, 2));
                Floor.Add("floor-sw", SpriteLib.ExtractSprite(blockSheet, 1, 3));
                Floor.Add("floor-s", SpriteLib.ExtractSprite(blockSheet, 2, 3));
                Floor.Add("floor-se", SpriteLib.ExtractSprite(blockSheet, 3, 3));
                Floor.Add("floor-se-ext", SpriteLib.ExtractSprite(blockSheet, 1, 5));
                Floor.Add("floor-sw-ext", SpriteLib.ExtractSprite(blockSheet, 4, 5));
                Floor.Add("floor-ne-ext", SpriteLib.ExtractSprite(blockSheet, 3, 5));
                Floor.Add("floor-nw-ext", SpriteLib.ExtractSprite(blockSheet, 2, 5));
                Floor.Add("floor-junc-ns", SpriteLib.ExtractSprite(blockSheet, 2, 1));
                Floor.Add("floor-junc-ew", SpriteLib.ExtractSprite(blockSheet, 1, 8));
                Floor.Add("floor-doublecorner-bottom", SpriteLib.ExtractSprite(blockSheet, 2, 2));
                Floor.Add("floor-doublecorner-top", SpriteLib.ExtractSprite(blockSheet, 2, 6));
                Floor.Add("floor-doublecorner-right", SpriteLib.ExtractSprite(blockSheet, 3, 6));
                Floor.Add("floor-doublecorner-left", SpriteLib.ExtractSprite(blockSheet, 4, 6));
                Floor.Add("floor-odd-nsw-only", SpriteLib.ExtractSprite(blockSheet, 5, 6));
                Floor.Add("floor-odd-missing-ne,sw,w,nw", SpriteLib.ExtractSprite(blockSheet, 2, 2));
                Floor.Add("floor-odd-missing-ew", SpriteLib.ExtractSprite(blockSheet, 1, 8));
                Floor.Add("floor-odd-missing-w-[o]sw", SpriteLib.ExtractSprite(blockSheet, 2, 8));
                Floor.Add("floor-odd-missing-e-[o]se", SpriteLib.ExtractSprite(blockSheet, 3, 8));
                Floor.Add("floor-odd-missing-nw,n,ne,se,s,sw,w", SpriteLib.ExtractSprite(blockSheet, 5, 8));
                Floor.Add("floor-odd-missing-nw,n,ne,se,s,sw,w2", SpriteLib.ExtractSprite(blockSheet, 8, 8));
                Floor.Add("floor-odd-missing-4", SpriteLib.ExtractSprite(blockSheet, 7, 8));
                Floor.Add("floor-odd-missing-nw,n,ne,e,se,s,sw", SpriteLib.ExtractSprite(blockSheet, 6, 8));
                Floor.Add("floor-doorway-vertical", SpriteLib.ExtractSprite(blockSheet, 4, 8));
                Floor.Add("floor-nw-ext-stair", SpriteLib.ExtractSprite(blockSheet, 1, 7));
                Floor.Add("floor-ne-ext-stair", SpriteLib.ExtractSprite(blockSheet, 2, 7));
                Floor.Add("floor-sw-ext-stair", SpriteLib.ExtractSprite(blockSheet, 3, 7));
                Floor.Add("floor-se-ext-stair", SpriteLib.ExtractSprite(blockSheet, 4, 7));
            }

            private static void LoadStairs(Texture2D blockSheet)
            {
                Stairs.Add("stairs-top-01", ExtractSprite(blockSheet, 8, 5));
                Stairs.Add("stairs-top-02", ExtractSprite(blockSheet, 9, 5));
                Stairs.Add("stairs-top-03", ExtractSprite(blockSheet, 10, 5));
                Stairs.Add("stairs-bottom-01", ExtractSprite(blockSheet, 8, 6));
                Stairs.Add("stairs-bottom-02", ExtractSprite(blockSheet, 9, 6));
                Stairs.Add("stairs-bottom-03", ExtractSprite(blockSheet, 10, 6));
            }

            private static void LoadDoors(Texture2D blockSheet)
            {
                Doors.Add("door-int", SpriteLib.ExtractSprite(blockSheet, 1, 4));
            }

            private static void LoadRoof(Texture2D roofSheet)
            {
                Roof.Add("roof-back-left", SpriteLib.ExtractSprite(roofSheet, 0, 0, 2 * Global.TileSize, 2 * Global.TileSize));
                Roof.Add("roof-back-right", SpriteLib.ExtractSprite(roofSheet, 4 * Global.TileSize, 0, 2 * Global.TileSize, 2 * Global.TileSize));
                Roof.Add("roof-front-left", SpriteLib.ExtractSprite(roofSheet, 0, 3 * Global.TileSize, 2 * Global.TileSize, 2 * Global.TileSize));
                Roof.Add("roof-front-right", SpriteLib.ExtractSprite(roofSheet, 4 * Global.TileSize, 3 * Global.TileSize, 2 * Global.TileSize, 2 * Global.TileSize));
                Roof.Add("roof-side-left", SpriteLib.ExtractSprite(roofSheet, 0, 2 * Global.TileSize, 2 * Global.TileSize, Global.TileSize));
                Roof.Add("roof-side-right", SpriteLib.ExtractSprite(roofSheet, 4 * Global.TileSize, 2 * Global.TileSize, 2 * Global.TileSize, 1 * Global.TileSize));
                Roof.Add("roof-top-back", SpriteLib.ExtractSprite(roofSheet, 2 * Global.TileSize, 0 * Global.TileSize, 1 * Global.TileSize, 2 * Global.TileSize));
                Roof.Add("roof-top-middle", SpriteLib.ExtractSprite(roofSheet, 2 * Global.TileSize, 2 * Global.TileSize, 1 * Global.TileSize, 1 * Global.TileSize));
                Roof.Add("roof-top-front", SpriteLib.ExtractSprite(roofSheet, 2 * Global.TileSize, 3 * Global.TileSize, 1 * Global.TileSize, 2 * Global.TileSize));
                Roof.Add("roof-top-front2", SpriteLib.ExtractSprite(roofSheet, 3 * Global.TileSize, 3 * Global.TileSize, 1 * Global.TileSize, 2 * Global.TileSize));
            }

            private static void LoadDoodads()
            {
                // TODO: Consolidate this with a sprite sheet?
                Doodads.Add("sign-forsale", Global.ContentManager.Load<Texture2D>("Sprites/Map/Town/Sign_ForSale"));
            }
        }

        public static class World
        {
            public static Dictionary<string, Texture2D> Terrain;

            public static Dictionary<string, Texture2D> Grasslands;
            public static Dictionary<string, Texture2D> Winterlands;
            public static Dictionary<string, Texture2D> Desert;
            public static Dictionary<string, Texture2D> Wetlands;
            public static Dictionary<string, Texture2D> Ashlands;

            public static Dictionary<string, Texture2D> Structures;



            public static void Load()
            {
                Terrain = new Dictionary<string, Texture2D>();

                Grasslands = new Dictionary<string, Texture2D>();
                Winterlands = new Dictionary<string, Texture2D>();
                Desert = new Dictionary<string, Texture2D>();
                Wetlands = new Dictionary<string, Texture2D>();
                Ashlands = new Dictionary<string, Texture2D>();
                Structures = new Dictionary<string, Texture2D>();

                spriteSheetBiomes = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Biomes");
                var terrainSheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/WorldTerrain01"); // TODO: NOT USED YET

                LoadGrasslands();
                LoadWinterlands();
                LoadDesert();
                LoadWetlands();
                LoadAshlands();

                LoadMountain();

                LoadCoast();
                LoadWater();

                LoadStructures();
            }

            private static void LoadGrasslands()
            {
                Grasslands.Add("grass-c", ExtractBiomeSprite(0, 0));
                Grasslands.Add("coast-c", ExtractBiomeSprite(1, 0));
            }

            private static void LoadWinterlands()
            {
                Winterlands.Add("winterlands-c", ExtractBiomeSprite(2, 0));
                Winterlands.Add("coast-c", ExtractBiomeSprite(6, 0));
            }

            private static void LoadDesert()
            {
                Desert.Add("desert-c", ExtractBiomeSprite(3, 0));
                Desert.Add("coast-c", ExtractBiomeSprite(1, 0));
            }

            private static void LoadWetlands()
            {
                Wetlands.Add("wetlands-c", ExtractBiomeSprite(0, 1));
                Wetlands.Add("coast-c", ExtractBiomeSprite(1, 1));
            }

            private static void LoadAshlands()
            {
                Ashlands.Add("ashlands-c", ExtractBiomeSprite(2, 1));
                Ashlands.Add("coast-c", ExtractBiomeSprite(3, 1));
            }



            //// NOT USED
            //public static void LoadGrass(Texture2D sheet)
            //{
            //    Terrain.Add("grass-NW", ExtractSprite(sheet, 6, 0));
            //    Terrain.Add("grass-N", ExtractSprite(sheet, 7, 0));
            //    Terrain.Add("grass-NE", ExtractSprite(sheet, 8, 0));
            //    Terrain.Add("grass-W", ExtractSprite(sheet, 6, 1));
            //    Terrain.Add("grass", ExtractSprite(sheet, 7, 1));
            //    Terrain.Add("grass-E", ExtractSprite(sheet, 8, 1));
            //    Terrain.Add("grass-SW", ExtractSprite(sheet, 6, 2));
            //    Terrain.Add("grass-S", ExtractSprite(sheet, 7, 2));
            //    Terrain.Add("grass-SE", ExtractSprite(sheet, 8, 2));

            //    Terrain.Add("grass-inv-NW", ExtractSprite(sheet, 9, 0));
            //    Terrain.Add("grass-inv-N", ExtractSprite(sheet, 10, 0));
            //    Terrain.Add("grass-inv-NE", ExtractSprite(sheet, 11, 0));
            //    Terrain.Add("grass-inv-W", ExtractSprite(sheet, 9, 1));
            //    Terrain.Add("grass-inv", ExtractSprite(sheet, 10, 1));
            //    Terrain.Add("grass-inv-E", ExtractSprite(sheet, 11, 1));
            //    Terrain.Add("grass-inv-SW", ExtractSprite(sheet, 9, 2));
            //    Terrain.Add("grass-inv-S", ExtractSprite(sheet, 10, 2));
            //    Terrain.Add("grass-inv-SE", ExtractSprite(sheet, 11, 2));
            //}

            //// NOT USED
            //public static void LoadBiomeGrasslands()
            //{
            //    var sheet = sheets["biome-grasslands"];

            //    lib.Add("grasslands-ext-NW", ExtractSprite(sheet, 0, 0));
            //    lib.Add("grasslands-ext-N", ExtractSprite(sheet, 1, 0));
            //    lib.Add("grasslands-ext-NE", ExtractSprite(sheet, 2, 0));
            //    lib.Add("grasslands-ext-W", ExtractSprite(sheet, 0, 1));
            //    lib.Add("grasslands-ext-C", ExtractSprite(sheet, 1, 1));
            //    lib.Add("grasslands-ext-E", ExtractSprite(sheet, 2, 1));
            //    lib.Add("grasslands-ext-SW", ExtractSprite(sheet, 0, 2));
            //    lib.Add("grasslands-ext-S", ExtractSprite(sheet, 1, 2));
            //    lib.Add("grasslands-ext-SE", ExtractSprite(sheet, 2, 2));

            //    lib.Add("grasslands-int-NW", ExtractSprite(sheet, 3, 0));
            //    lib.Add("grasslands-int-N", ExtractSprite(sheet, 4, 0));
            //    lib.Add("grasslands-int-NE", ExtractSprite(sheet, 5, 0));
            //    lib.Add("grasslands-int-W", ExtractSprite(sheet, 3, 1));
            //    lib.Add("grasslands-int-C", ExtractSprite(sheet, 4, 1));
            //    lib.Add("grasslands-int-E", ExtractSprite(sheet, 5, 1));
            //    lib.Add("grasslands-int-SW", ExtractSprite(sheet, 3, 2));
            //    lib.Add("grasslands-int-S", ExtractSprite(sheet, 4, 2));
            //    lib.Add("grasslands-int-SE", ExtractSprite(sheet, 5, 2));

            //    lib["grasslands-ext-NW"].Name = "grasslands-ext-NW";
            //    lib["grasslands-ext-N"].Name = "grasslands-ext-N";
            //    lib["grasslands-ext-NE"].Name = "grasslands-ext-NE";
            //    lib["grasslands-ext-W"].Name = "grasslands-ext-W";
            //    lib["grasslands-ext-C"].Name = "grasslands-ext-C";
            //    lib["grasslands-ext-E"].Name = "grasslands-ext-E";
            //    lib["grasslands-ext-SW"].Name = "grasslands-ext-SW";
            //    lib["grasslands-ext-S"].Name = "grasslands-ext-S";
            //    lib["grasslands-ext-SE"].Name = "grasslands-ext-SE";

            //    lib["grasslands-int-NW"].Name = "grasslands-int-NW";
            //    lib["grasslands-int-N"].Name = "grasslands-int-N";
            //    lib["grasslands-int-NE"].Name = "grasslands-int-NE";
            //    lib["grasslands-int-W"].Name = "grasslands-int-W";
            //    lib["grasslands-int-C"].Name = "grasslands-int-C";
            //    lib["grasslands-int-E"].Name = "grasslands-int-E";
            //    lib["grasslands-int-SW"].Name = "grasslands-int-SW";
            //    lib["grasslands-int-S"].Name = "grasslands-int-S";
            //    lib["grasslands-int-SE"].Name = "grasslands-int-SE";
            //}

            //// NOT USED
            public static void LoadMountain()
            {
                // Mountain
                lib.Add("mountain-NW", ExtractTileSprite(12, 0));
                lib.Add("mountain-N", ExtractTileSprite(13, 0));
                lib.Add("mountain-NE", ExtractTileSprite(14, 0));
                lib.Add("mountain-W", ExtractTileSprite(12, 1));

                //spriteLib.Add("mountain", ExtractTileSprite(13, 1));
                lib.Add("mountain", ExtractSprite(spriteSheetMountain, 0, 0));

                lib.Add("mountain-E", ExtractTileSprite(14, 1));
                lib.Add("mountain-SW", ExtractTileSprite(12, 2));
                lib.Add("mountain-S", ExtractTileSprite(13, 2));
                lib.Add("mountain-SE", ExtractTileSprite(14, 2));

                // Inverted mountain
                lib.Add("mountain-inv-NW", ExtractTileSprite(15, 0));
                lib.Add("mountain-inv-N", ExtractTileSprite(16, 0));
                lib.Add("mountain-inv-NE", ExtractTileSprite(17, 0));
                lib.Add("mountain-inv-W", ExtractTileSprite(15, 1));
                lib.Add("mountain-inv", ExtractTileSprite(16, 1));
                lib.Add("mountain-inv-E", ExtractTileSprite(17, 1));
                lib.Add("mountain-inv-SW", ExtractTileSprite(15, 2));
                lib.Add("mountain-inv-S", ExtractTileSprite(16, 2));
                lib.Add("mountain-inv-SE", ExtractTileSprite(17, 2));
            }

            //// NOT USED
            //public static void LoadForest()
            //{
            //    // Forest
            //    ExtractSprites("forest", spriteSheetForest, 3, 3);
            //    lib.Add("forest-inv-NW", ExtractSprite(spriteSheetForest, 3, 0));
            //    lib.Add("forest-inv-N", ExtractSprite(spriteSheetForest, 4, 0));
            //    lib.Add("forest-inv-NE", ExtractSprite(spriteSheetForest, 5, 0));
            //    lib.Add("forest-inv-W", ExtractSprite(spriteSheetForest, 3, 1));
            //    lib.Add("forest-inv-E", ExtractSprite(spriteSheetForest, 5, 1));
            //    lib.Add("forest-inv-SW", ExtractSprite(spriteSheetForest, 3, 2));
            //    lib.Add("forest-inv-S", ExtractSprite(spriteSheetForest, 4, 2));
            //    lib.Add("forest-inv-SE", ExtractSprite(spriteSheetForest, 5, 2));

            //    lib.Add("forest-frost-NW", ExtractSprite(spriteSheetForest, 0, 3));
            //    lib.Add("forest-frost-N", ExtractSprite(spriteSheetForest, 1, 3));
            //    lib.Add("forest-frost-NE", ExtractSprite(spriteSheetForest, 2, 3));
            //    lib.Add("forest-frost-W", ExtractSprite(spriteSheetForest, 0, 4));
            //    lib.Add("forest-frost-C", ExtractSprite(spriteSheetForest, 1, 4));
            //    lib.Add("forest-frost-E", ExtractSprite(spriteSheetForest, 2, 4));
            //    lib.Add("forest-frost-SW", ExtractSprite(spriteSheetForest, 0, 5));
            //    lib.Add("forest-frost-S", ExtractSprite(spriteSheetForest, 1, 5));
            //    lib.Add("forest-frost-SE", ExtractSprite(spriteSheetForest, 2, 5));

            //    lib.Add("forest-frost-inv-NW", ExtractSprite(spriteSheetForest, 3, 3));
            //    lib.Add("forest-frost-inv-N", ExtractSprite(spriteSheetForest, 4, 3));
            //    lib.Add("forest-frost-inv-NE", ExtractSprite(spriteSheetForest, 5, 3));
            //    lib.Add("forest-frost-inv-W", ExtractSprite(spriteSheetForest, 3, 4));
            //    lib.Add("forest-frost-inv-C", ExtractSprite(spriteSheetForest, 4, 4));
            //    lib.Add("forest-frost-inv-E", ExtractSprite(spriteSheetForest, 5, 4));
            //    lib.Add("forest-frost-inv-SW", ExtractSprite(spriteSheetForest, 3, 5));
            //    lib.Add("forest-frost-inv-S", ExtractSprite(spriteSheetForest, 4, 5));
            //    lib.Add("forest-frost-inv-SE", ExtractSprite(spriteSheetForest, 5, 5));
            //}

            public static void LoadCoast()
            {
                // Coast
                lib.Add("coast-NW", ExtractTileSprite(0, 0));
                lib.Add("coast-N", ExtractTileSprite(1, 0));
                lib.Add("coast-NE", ExtractTileSprite(2, 0));
                lib.Add("coast-W", ExtractTileSprite(0, 1));
                lib.Add("coast-c", ExtractTileSprite(1, 1));
                lib.Add("coast-E", ExtractTileSprite(2, 1));
                lib.Add("coast-SW", ExtractTileSprite(0, 2));
                lib.Add("coast-S", ExtractTileSprite(1, 2));
                lib.Add("coast-SE", ExtractTileSprite(2, 2));

                // Inverted coast
                lib.Add("coast-inv-NW", ExtractTileSprite(3, 0));
                lib.Add("coast-inv-N", ExtractTileSprite(4, 0));
                lib.Add("coast-inv-NE", ExtractTileSprite(5, 0));
                lib.Add("coast-inv-W", ExtractTileSprite(3, 1));
                lib.Add("coast-inv-c", ExtractTileSprite(4, 1));
                lib.Add("coast-inv-E", ExtractTileSprite(5, 1));
                lib.Add("coast-inv-SW", ExtractTileSprite(3, 2));
                lib.Add("coast-inv-S", ExtractTileSprite(4, 2));
                lib.Add("coast-inv-SE", ExtractTileSprite(5, 2));
            }

            
            public static void LoadWater()
            {
                lib.Add("deepwater-NW", ExtractSprite(spriteSheetTiles, 18, 0));
                lib.Add("deepwater-N", ExtractSprite(spriteSheetTiles, 19, 0));
                lib.Add("deepwater-NE", ExtractSprite(spriteSheetTiles, 20, 0));
                lib.Add("deepwater-W", ExtractSprite(spriteSheetTiles, 18, 1));
                lib.Add("deepwater-C", ExtractSprite(spriteSheetTiles, 19, 1));
                lib.Add("deepwater-E", ExtractSprite(spriteSheetTiles, 20, 1));
                lib.Add("deepwater-SW", ExtractSprite(spriteSheetTiles, 18, 2));
                lib.Add("deepwater-S", ExtractSprite(spriteSheetTiles, 19, 2));
                lib.Add("deepwater-SE", ExtractSprite(spriteSheetTiles, 20, 2));
            }

            public static void LoadStructures()
            {
                Structures.Add("castle", Global.ContentManager.Load<Texture2D>("Sprites/WorldStructures/Castle"));
                Structures.Add("town", Global.ContentManager.Load<Texture2D>("Sprites/WorldStructures/Town01"));
            }
        }

        public static class Dungeon
        {
            public static Dictionary<string, List<Texture2D>> DungeonSprites;
            public static Dictionary<string, Texture2D> Walls;
            public static Dictionary<string, Texture2D> Floor;
            public static Dictionary<string, Texture2D> Doors;
            public static Dictionary<string, Texture2D> Stairs;


            public static void Load()
            {
                Walls = new Dictionary<string, Texture2D>();
                Floor = new Dictionary<string, Texture2D>();
                Doors = new Dictionary<string, Texture2D>();
                Stairs = new Dictionary<string, Texture2D>();

                var sheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/Dungeon/dungeon_sprites3");

                //LoadWallsAndFloor();
                //LoadDungeonSprites();
                LoadWalls(sheet);
                LoadDoors(sheet);
                LoadFloor(sheet);
                LoadStairs(sheet);
                //LoadDungeonPopulationSprites();
            }

            //private static void LoadDungeonSprites()
            //{
            //    var sheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/Dungeon/sheet");
            //    sheets.Add("dungeon-layout", sheet);
            //    //var sheet = _content.Load<Texture2D>("Sprites/Map/Dungeon/Tileset_Dungeon02");

            //    DungeonSprites = new Dictionary<string, List<Texture2D>>
            //{
            //    { "wall-nw", new List<Texture2D> { ExtractSprite(sheet, 0, 0) } },
            //    { "wall-n", new List<Texture2D> { ExtractSprite(sheet, 1, 0) } },
            //    { "wall-ne", new List<Texture2D> { ExtractSprite(sheet, 2, 0) } },
            //    { "wall-w", new List<Texture2D> { ExtractSprite(sheet, 0, 1) } },
            //    { "wall-e", new List<Texture2D> { ExtractSprite(sheet, 2, 1) } },
            //    { "wall-sw", new List<Texture2D> { ExtractSprite(sheet, 0, 2) } },
            //    { "wall-s", new List<Texture2D> { ExtractSprite(sheet, 1, 2) } },
            //    { "wall-se", new List<Texture2D> { ExtractSprite(sheet, 2, 2) } },
            //    { "wall-ext-nw", new List<Texture2D> { ExtractSprite(sheet, 3, 0) } },
            //    { "wall-ext-ne", new List<Texture2D> { ExtractSprite(sheet, 4, 0) } },
            //    { "wall-ext-sw", new List<Texture2D> { ExtractSprite(sheet, 3, 1) } },
            //    { "wall-ext-se", new List<Texture2D> { ExtractSprite(sheet, 4, 1) } },
            //    { "floor-01", new List<Texture2D>() }
            //};

            //    for (int i = 0; i < 7; i++)
            //    {
            //        for (int j = 0; j < 4; j++)
            //        {
            //            DungeonSprites["floor-01"].Add(ExtractSprite(sheet, i + 6, j));
            //        }
            //    }
            //}

            private static void LoadWalls(Texture2D sheet)
            {
                Walls.Add("wall-nw", ExtractSprite(sheet, 4, 1));
                Walls.Add("wall-n", ExtractSprite(sheet, 5, 1));
                Walls.Add("wall-ne", ExtractSprite(sheet, 6, 3));
                Walls.Add("wall-w", ExtractSprite(sheet, 4, 2));
                Walls.Add("wall-e", ExtractSprite(sheet, 6, 2));
                Walls.Add("wall-sw", ExtractSprite(sheet, 4, 3));
                Walls.Add("wall-s", ExtractSprite(sheet, 5, 3));
                Walls.Add("wall-se", ExtractSprite(sheet, 6, 3));
                Walls.Add("wall-junc-esw", ExtractSprite(sheet, 9, 2));
                Walls.Add("wall-junc-new", ExtractSprite(sheet, 4, 4));
                Walls.Add("wall-junc-nes", ExtractSprite(sheet, 8, 1));
                Walls.Add("wall-junc-nsw", ExtractSprite(sheet, 10, 2));
                Walls.Add("wall-junc-nesw", ExtractSprite(sheet, 8, 2));
                Walls.Add("wall-int-cap-n", ExtractSprite(sheet, 8, 3));
                Walls.Add("wall-int-cap-e", ExtractSprite(sheet, 5, 4));
                Walls.Add("wall-int-cap-s", ExtractSprite(sheet, 7, 1));
                Walls.Add("wall-int-cap-w", ExtractSprite(sheet, 6, 4));
                Walls.Add("wall-pillar", ExtractSprite(sheet, 5, 2));
            }

            private static void LoadFloor(Texture2D blockSheet)
            {
                Floor.Add("floor-nw", SpriteLib.ExtractSprite(blockSheet, 1, 1));
                Floor.Add("floor-n", SpriteLib.ExtractSprite(blockSheet, 2, 1));
                Floor.Add("floor-ne", SpriteLib.ExtractSprite(blockSheet, 3, 1));
                Floor.Add("floor-w", SpriteLib.ExtractSprite(blockSheet, 1, 2));
                Floor.Add("floor-c", SpriteLib.ExtractSprite(blockSheet, 2, 2));
                Floor.Add("floor-e", SpriteLib.ExtractSprite(blockSheet, 3, 2));
                Floor.Add("floor-sw", SpriteLib.ExtractSprite(blockSheet, 1, 3));
                Floor.Add("floor-s", SpriteLib.ExtractSprite(blockSheet, 2, 3));
                Floor.Add("floor-se", SpriteLib.ExtractSprite(blockSheet, 3, 3));
                Floor.Add("floor-se-ext", SpriteLib.ExtractSprite(blockSheet, 1, 5));
                Floor.Add("floor-sw-ext", SpriteLib.ExtractSprite(blockSheet, 4, 5));
                Floor.Add("floor-ne-ext", SpriteLib.ExtractSprite(blockSheet, 3, 5));
                Floor.Add("floor-nw-ext", SpriteLib.ExtractSprite(blockSheet, 2, 5));
                Floor.Add("floor-junc-ns", SpriteLib.ExtractSprite(blockSheet, 2, 1));
                Floor.Add("floor-junc-ew", SpriteLib.ExtractSprite(blockSheet, 1, 8));
                Floor.Add("floor-doublecorner-bottom", SpriteLib.ExtractSprite(blockSheet, 2, 2));
                Floor.Add("floor-doublecorner-top", SpriteLib.ExtractSprite(blockSheet, 2, 6));
                Floor.Add("floor-doublecorner-right", SpriteLib.ExtractSprite(blockSheet, 3, 6));
                Floor.Add("floor-doublecorner-left", SpriteLib.ExtractSprite(blockSheet, 4, 6));
                Floor.Add("floor-odd-nsw-only", SpriteLib.ExtractSprite(blockSheet, 5, 6));
                Floor.Add("floor-odd-missing-ne,sw,w,nw", SpriteLib.ExtractSprite(blockSheet, 2, 2));
                Floor.Add("floor-odd-missing-ew", SpriteLib.ExtractSprite(blockSheet, 1, 8));
                Floor.Add("floor-odd-missing-w-[o]sw", SpriteLib.ExtractSprite(blockSheet, 2, 8));
                Floor.Add("floor-odd-missing-e-[o]se", SpriteLib.ExtractSprite(blockSheet, 3, 8));
                Floor.Add("floor-odd-missing-nw,n,ne,se,s,sw,w", SpriteLib.ExtractSprite(blockSheet, 5, 8));
                Floor.Add("floor-odd-missing-nw,n,ne,se,s,sw,w2", SpriteLib.ExtractSprite(blockSheet, 8, 8));
                Floor.Add("floor-odd-missing-4", SpriteLib.ExtractSprite(blockSheet, 7, 8));
                Floor.Add("floor-odd-missing-nw,n,ne,e,se,s,sw", SpriteLib.ExtractSprite(blockSheet, 6, 8));
                Floor.Add("floor-doorway-vertical", SpriteLib.ExtractSprite(blockSheet, 4, 8));
                Floor.Add("floor-nw-ext-stair", SpriteLib.ExtractSprite(blockSheet, 1, 7));
                Floor.Add("floor-ne-ext-stair", SpriteLib.ExtractSprite(blockSheet, 2, 7));
                Floor.Add("floor-sw-ext-stair", SpriteLib.ExtractSprite(blockSheet, 3, 7));
                Floor.Add("floor-se-ext-stair", SpriteLib.ExtractSprite(blockSheet, 4, 7));
            }

            private static void LoadStairs(Texture2D blockSheet)
            {
                Stairs.Add("stairs-top-01", ExtractSprite(blockSheet, 8, 5));
                Stairs.Add("stairs-top-02", ExtractSprite(blockSheet, 9, 5));
                Stairs.Add("stairs-top-03", ExtractSprite(blockSheet, 10, 5));
                Stairs.Add("stairs-bottom-01", ExtractSprite(blockSheet, 8, 6));
                Stairs.Add("stairs-bottom-02", ExtractSprite(blockSheet, 9, 6));
                Stairs.Add("stairs-bottom-03", ExtractSprite(blockSheet, 10, 6));
            }

            private static void LoadDoors(Texture2D blockSheet)
            {
                Doors.Add("door-int", SpriteLib.ExtractSprite(blockSheet, 1, 4));
            }


            private static void LoadDungeonPopulationSprites()
            {
                var sheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/Dungeon/stairs");

                DungeonSprites.Add("stairs-nw", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 0, 0) });
                DungeonSprites.Add("stairs-n", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 1, 0) });
                DungeonSprites.Add("stairs-ne", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 2, 0) });
                DungeonSprites.Add("stairs-w", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 0, 1) });
                DungeonSprites.Add("stairs-c", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 1, 1) });
                DungeonSprites.Add("stairs-e", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 2, 1) });
                DungeonSprites.Add("stairs-sw", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 0, 2) });
                DungeonSprites.Add("stairs-s", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 1, 2) });
                DungeonSprites.Add("stairs-se", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 2, 2) });
            }
        
            
        }

        public static class UI
        {
            public static Dictionary<string, Texture2D> Items;
            public static Dictionary<string, Texture2D> Cursor;
            public static Dictionary<string, Texture2D> Dialogue;
            public static Dictionary<string, Texture2D> Scroll;
            public static Dictionary<string, Texture2D> Minimap;

            public static void Load()
            {
                LoadItems();
                LoadMouseCursor();
                LoadDialogueSystem();
                LoadScroll();
                LoadMinimapFrame();
            }

            public static void LoadItems()
            {
                Items = new Dictionary<string, Texture2D>();

                Items.Add("wood-log", Global.ContentManager.Load<Texture2D>("Sprites/UI/Items/wood-log-01"));
                Items.Add("rock", ExtractSprite(Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Rock01"), 0, 0, Global.TileSize, Global.TileSize));
            }

            private static void LoadMouseCursor()
            {
                Cursor = new Dictionary<string, Texture2D>();

                var sheet = Global.ContentManager.Load<Texture2D>("Sprites/UI/MouseCursor");

                Cursor.Add("cursor", ExtractSprite(sheet, 0, 0));
            }

            private static void LoadDialogueSystem()
            {
                Dialogue = new Dictionary<string, Texture2D>();

                var sheet = Global.ContentManager.Load<Texture2D>("Sprites/UI/Dialogue");

                Dialogue.Add("dialogue-NW", ExtractSprite(sheet, 0, 0));
                Dialogue.Add("dialogue-N", ExtractSprite(sheet, 1, 0));
                Dialogue.Add("dialogue-NE", ExtractSprite(sheet, 2, 0));

                Dialogue.Add("dialogue-W", ExtractSprite(sheet, 0, 1));
                Dialogue.Add("dialogue-C", ExtractSprite(sheet, 1, 1));
                Dialogue.Add("dialogue-E", ExtractSprite(sheet, 2, 1));

                Dialogue.Add("dialogue-SW", ExtractSprite(sheet, 0, 2));
                Dialogue.Add("dialogue-S", ExtractSprite(sheet, 1, 2));
                Dialogue.Add("dialogue-SE", ExtractSprite(sheet, 2, 2));
            }

            private static void LoadScroll()
            {
                Scroll = new Dictionary<string, Texture2D>();

                var sheet = Global.ContentManager.Load<Texture2D>("Sprites/UI/Scroll");

                // TODO: Change this to use one input sprite instead of 3 files
                Scroll.Add("scroll-left", Global.ContentManager.Load<Texture2D>("Sprites/UI/Scroll-Top-Left"));
                Scroll.Add("scroll-right", Global.ContentManager.Load<Texture2D>("Sprites/UI/Scroll-Top-Right"));
                Scroll.Add("scroll-middle", Global.ContentManager.Load<Texture2D>("Sprites/UI/Scroll-Bottom"));
            }

            private static void LoadMinimapFrame()
            {
                var sheet = Global.ContentManager.Load<Texture2D>("Sprites/UI/MinimapFrameBACKUP");
                Minimap = new Dictionary<string, Texture2D>()
                {
                    { "minimap-frame", sheet }
                    //{ "nw", ExtractSprite(sheet, 0, 0) },
                    //{ "n", ExtractSprite(sheet, 1, 0) },
                    //{ "ne", ExtractSprite(sheet, 2, 0) },
                    //{ "w", ExtractSprite(sheet, 0, 1) },
                    //{ "c", ExtractSprite(sheet, 1, 1) },
                    //{ "e", ExtractSprite(sheet, 2, 1) },
                    //{ "sw", ExtractSprite(sheet, 0, 2) },
                    //{ "s", ExtractSprite(sheet, 1, 2) },
                    //{ "se", ExtractSprite(sheet, 2, 2) }
                };
            }
        }

        public static class Player
        {
            public static Dictionary<string, Texture2D> Idle;

            public static void Load()
            {
                LoadPlayerIdle();
            }

            private static void LoadPlayerIdle()
            {
                Idle = new Dictionary<string, Texture2D>();

                var sheet = sheets["player"];

                Idle.Add("walk-north-01", Global.ContentManager.Load<Texture2D>("Sprites/Player/Player-Walk-N-01"));
                Idle.Add("walk-east-01", Global.ContentManager.Load<Texture2D>("Sprites/Player/Player-Walk-E-01"));
                Idle.Add("walk-south-01", Global.ContentManager.Load<Texture2D>("Sprites/Player/Player-Walk-S-01"));
                Idle.Add("walk-west-01", Global.ContentManager.Load<Texture2D>("Sprites/Player/Player-Walk-W-01"));
            }
        }
        
        // TODO: MAYBE UNUSED CODE HERE
        public static class Doodads
        {
            public static Dictionary<string, Texture2D> Chests;
            public static Dictionary<string, Texture2D> Barrels;

            public static void Load()
            {
                LoadChests();
                LoadBarrels();
            }

            // DONT NEED TO DO THIS? LOADED IN ITS OWN CLASS?
            public static void LoadChests()
            {
                Chests = new Dictionary<string, Texture2D>();

                var sheet = Global.ContentManager.Load<Texture2D>("Sprites/Doodads/Chests/Chest01");

                Chests.Add("chest-01", ExtractSprite(sheet, 0, 0));
            }

            // DONT NEED TO DO THIS? LOADED IN ITS OWN CLASS?
            public static void LoadBarrels()
            {
                Barrels = new Dictionary<string, Texture2D>();
                var sheet = Global.ContentManager.Load<Texture2D>("Sprites/Doodads/Barrels/Barrel01");
                Barrels.Add("barrel-01", ExtractSprite(sheet, 0, 0));
            }
        }

        public void LoadSpriteLibrary()
        {
            sheets = new Dictionary<string, Texture2D>();
            lib = new Dictionary<string, Texture2D>();

            // TODO: Remove usage of these as class-wide variable
            spriteSheetTiles = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/WorldTerrain01");
            spriteSheetForest = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Forest");
            spriteSheetMountain = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Mountain");
            spriteSheetCastle = Global.ContentManager.Load<Texture2D>("Sprites/WorldStructures/Castle");
            sheets.Add("biome-grasslands", Global.ContentManager.Load<Texture2D>("Sprites/Map/World/BiomeGrasslands"));
            sheets.Add("player", Global.ContentManager.Load<Texture2D>("Sprites/Player/Player"));

            World.Load();
            Town.Load();
            UI.Load();
            Mineables.Load();
            Dungeon.Load();
            Player.Load();
            Doodads.Load();
        }
        
        public static Texture2D ExtractSprite(Texture2D spriteSheet, int x, int y)
        {
            var width = Global.TileSize;
            var height = Global.TileSize;

            x *= width;
            y *= height;

            Rectangle sourceRectangle = new Rectangle(x, y, width, height);

            // Extract the pixel data from the spritesheet
            Color[] data = new Color[width * height];
            spriteSheet.GetData(0, sourceRectangle, data, 0, data.Length);

            // Create a new texture for the sprite and set the pixel data
            Texture2D sprite = new Texture2D(Global.GraphicsDevice, width, height);
            sprite.SetData(data);

            return sprite;
        }

        public static Texture2D ExtractSprite(Texture2D spriteSheet, int x, int y, int width, int height)
        {
            Rectangle sourceRectangle = new Rectangle(x, y, width, height);

            // Extract the pixel data from the spritesheet
            Color[] data = new Color[width * height];
            spriteSheet.GetData(0, sourceRectangle, data, 0, data.Length);

            // Create a new texture for the sprite and set the pixel data
            Texture2D sprite = new Texture2D(Global.GraphicsDevice, width, height);
            sprite.SetData(data);

            return sprite;
        }

        public static Texture2D GetSprite(string key)
        {
            return lib[key];
        }

        [Obsolete("Refactor this out to more granular methods")]
        private static Texture2D ExtractBiomeSprite(int x, int y)
        {
            var width = Global.TileSize;
            var height = Global.TileSize;

            x *= width;
            y *= height;

            Rectangle sourceRectangle = new Rectangle(x, y, width, height);

            // Extract the pixel data from the spritesheet
            Color[] data = new Color[width * height];
            spriteSheetBiomes.GetData(0, sourceRectangle, data, 0, data.Length);

            // Create a new texture for the sprite and set the pixel data
            Texture2D sprite = new Texture2D(Global.GraphicsDevice, width, height);
            sprite.SetData(data);

            // Store the new texture in the array
            return sprite;
        }

        [Obsolete ("Refactor this out to more granular methods")]
        public static Texture2D ExtractTileSprite(int x, int y)
        {
            var width = Global.TileSize;
            var height = Global.TileSize;

            x *= width;
            y *= height;

            Rectangle sourceRectangle = new Rectangle(x, y, width, height);

            // Extract the pixel data from the spritesheet
            Color[] data = new Color[width * height];
            spriteSheetTiles.GetData(0, sourceRectangle, data, 0, data.Length);

            // Create a new texture for the sprite and set the pixel data
            Texture2D sprite = new Texture2D(Global.GraphicsDevice, width, height);
            sprite.SetData(data);

            // Store the new texture in the array
            return sprite;
        }
    }
}
