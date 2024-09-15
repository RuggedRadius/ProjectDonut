using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Tools;
using System.Collections.Generic;

namespace ProjectDonut.Core
{
    public class SpriteLibrary
    {
        private Texture2D spriteSheetTiles;
        private Texture2D spriteSheetBiomes;
        private Texture2D spriteSheetForest;
        private Texture2D spriteSheetWinter;
        private Texture2D spriteSheetMountain;
        private Texture2D spriteSheetCastle;
        private Texture2D spriteSheetTown;


        private Dictionary<string, Texture2D> sheets;
        public Dictionary<string, List<Texture2D>> DungeonSprites;
        public Dictionary<string, List<Texture2D>> WorldMapSprites;
        public Dictionary<string, Texture2D> ItemsSprites;
        public Dictionary<string, Texture2D> TownSprites;
        public Dictionary<string, Texture2D> BuildingBlockSprites;


        private Dictionary<string, Texture2D> lib;


        public SpriteLibrary()
        {
            LoadSpriteLibrary();
        }

        public void LoadSpriteLibrary()
        {
            sheets = new Dictionary<string, Texture2D>();
            lib = new Dictionary<string, Texture2D>();

            WorldMapSprites = new Dictionary<string, List<Texture2D>>();

            LoadWorldMapSprites();
            LoadSpriteSheets();

            LoadStructureCastle();
            LoadDialogueSystem();
            LoadStructureTown();

            LoadWater();
            LoadCoast();
            LoadForest();
            LoadMountain();
            LoadGrass();

            LoadBiomeGrasslands();

            LoadPlayerNEW();

            LoadMouseCursor();

            // Biomes
            lib.Add("grasslands", ExtractBiomeSprite(0, 0));
            lib.Add("beach", ExtractBiomeSprite(1, 0));
            lib.Add("winterlands", ExtractBiomeSprite(2, 0));
            lib.Add("desert", ExtractBiomeSprite(3, 0));


            LoadDungeonSpritesORIGINALWORKING();
            //LoadDungeonSprites();
            LoadDungeonPopulationSprites();

            LoadItems();

            LoadTownSprites();
        }

        private void LoadTownSprites()
        {
            TownSprites = new Dictionary<string, Texture2D>();

            var sheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/Town/Town01");

            // Grass
            TownSprites.Add("grass-nw", ExtractSprite(sheet, 0, 0));
            TownSprites.Add("grass-n", ExtractSprite(sheet, 1, 0));
            TownSprites.Add("grass-ne", ExtractSprite(sheet, 2, 0));
            TownSprites.Add("grass-w", ExtractSprite(sheet, 0, 1));
            TownSprites.Add("grass-c", ExtractSprite(sheet, 1, 1));
            TownSprites.Add("grass-e", ExtractSprite(sheet, 2, 1));
            TownSprites.Add("grass-sw", ExtractSprite(sheet, 0, 2));
            TownSprites.Add("grass-s", ExtractSprite(sheet, 1, 2));
            TownSprites.Add("grass-se", ExtractSprite(sheet, 2, 2));

            // Dirt
            TownSprites.Add("dirt-nw", ExtractSprite(sheet, 3, 0));
            TownSprites.Add("dirt-n", ExtractSprite(sheet, 4, 0));
            TownSprites.Add("dirt-ne", ExtractSprite(sheet, 5, 0));
            TownSprites.Add("dirt-w", ExtractSprite(sheet, 3, 1));
            TownSprites.Add("dirt-c", ExtractSprite(sheet, 4, 1));
            TownSprites.Add("dirt-e", ExtractSprite(sheet, 5, 1));
            TownSprites.Add("dirt-sw", ExtractSprite(sheet, 3, 2));
            TownSprites.Add("dirt-s", ExtractSprite(sheet, 4, 2));
            TownSprites.Add("dirt-se", ExtractSprite(sheet, 5, 2));

            // Fences
            TownSprites.Add("fence-nw", ExtractSprite(sheet, 6, 0));
            TownSprites.Add("fence-n", ExtractSprite(sheet, 7, 0));
            TownSprites.Add("fence-ne", ExtractSprite(sheet, 8, 0));
            TownSprites.Add("fence-w", ExtractSprite(sheet, 6, 1));
            TownSprites.Add("fence-c", ExtractSprite(sheet, 7, 1));
            TownSprites.Add("fence-e", ExtractSprite(sheet, 8, 1));
            TownSprites.Add("fence-sw", ExtractSprite(sheet, 6, 2));
            TownSprites.Add("fence-s", ExtractSprite(sheet, 7, 2));
            TownSprites.Add("fence-se", ExtractSprite(sheet, 8, 2));

            // Houses
            //TownSprites.Add("house-01", Global.ContentManager.Load<Texture2D>("Sprites/Map/Town/House01"));
            //TownSprites.Add("house-02", Global.ContentManager.Load<Texture2D>("Sprites/Map/Town/House02"));
            TownSprites.Add("sign-forsale", Global.ContentManager.Load<Texture2D>("Sprites/Map/Town/Sign_ForSale"));

            // Building Blocks
            var blockSheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/Town/house_sprites2");
            BuildingBlockSprites = new Dictionary<string, Texture2D>
            {
                { "wall-nw", ExtractSprite(blockSheet, 0, 0) },
                { "wall-n", ExtractSprite(blockSheet, 1, 0) },
                { "wall-ne", ExtractSprite(blockSheet, 2, 0) },
                { "wall-w", ExtractSprite(blockSheet, 1, 1) },
                { "wall-e", ExtractSprite(blockSheet, 2, 1) },
                { "wall-sw", ExtractSprite(blockSheet, 5, 1) },
                { "wall-s", ExtractSprite(blockSheet, 0, 2) },
                { "wall-se", ExtractSprite(blockSheet, 1, 2) },

                { "wall-ext-nw", ExtractSprite(blockSheet, 3, 4) },
                { "wall-ext-ne", ExtractSprite(blockSheet, 4, 4) },
                { "wall-ext-sw", ExtractSprite(blockSheet, 5, 4) },
                { "wall-ext-se", ExtractSprite(blockSheet, 0, 5) },

                { "wall-junc-esw", ExtractSprite(blockSheet, 1, 5) },
                { "wall-junc-new", ExtractSprite(blockSheet, 2, 5) },
                { "wall-junc-nes", ExtractSprite(blockSheet, 3, 5) },
                { "wall-junc-nsw", ExtractSprite(blockSheet, 4, 5) },
                { "wall-junc-nesw", ExtractSprite(blockSheet, 5, 5) },



                { "wall-int-cap-n", ExtractSprite(blockSheet, 2, 6) },
                { "wall-int-cap-e", ExtractSprite(blockSheet, 1, 6) },                
                { "wall-int-cap-s", ExtractSprite(blockSheet, 3, 6) },
                { "wall-int-cap-w", ExtractSprite(blockSheet, 0, 6) },

                { "wall-pillar", ExtractSprite(blockSheet, 0, 10) },



                { "floor-nw", ExtractSprite(blockSheet, 3, 0) },
                { "floor-n", ExtractSprite(blockSheet, 4, 0)  },
                { "floor-ne", ExtractSprite(blockSheet, 5, 0)  },
                { "floor-w", ExtractSprite(blockSheet, 3, 1)  },
                { "floor-c", ExtractSprite(blockSheet, 0, 1)  },
                { "floor-e", ExtractSprite(blockSheet, 4, 1)  },
                { "floor-sw", ExtractSprite(blockSheet, 2, 2)  },
                { "floor-s", ExtractSprite(blockSheet, 3, 2)  },
                { "floor-se", ExtractSprite(blockSheet, 4, 2)  },

                { "floor-se-ext", ExtractSprite(blockSheet, 0, 5)  },
                { "floor-sw-ext", ExtractSprite(blockSheet, 5, 4)  },
                { "floor-ne-ext", ExtractSprite(blockSheet, 4, 4)  },
                { "floor-nw-ext", ExtractSprite(blockSheet, 3, 4)  },

                { "floor-junc-ns", ExtractSprite(blockSheet, 4, 6) },
                { "floor-junc-ew", ExtractSprite(blockSheet, 5, 6) },

                { "floor-doublecorner-bottom", ExtractSprite(blockSheet, 0, 7) },
                { "floor-doublecorner-top", ExtractSprite(blockSheet, 1, 7) },
                { "floor-doublecorner-right", ExtractSprite(blockSheet, 2, 7) },
                { "floor-doublecorner-left", ExtractSprite(blockSheet, 3, 7) },

                { "floor-odd-nsw-only", ExtractSprite(blockSheet, 4, 7) },
                { "floor-odd-missing-ne,sw,w,nw", ExtractSprite(blockSheet, 5, 7) },

                { "floor-odd-missing-ew", ExtractSprite(blockSheet, 0, 9) },
                { "floor-odd-missing-w-[o]sw", ExtractSprite(blockSheet, 1, 9) },
                { "floor-odd-missing-e-[o]se", ExtractSprite(blockSheet, 2, 9) },                
                { "floor-odd-missing-nw,n,ne,se,s,sw,w", ExtractSprite(blockSheet, 4, 9) },                
                { "floor-odd-missing-nw,n,ne,se,s,sw,w2", ExtractSprite(blockSheet, 6, 1) },                
                { "floor-odd-missing-3", ExtractSprite(blockSheet, 6, 1) },                
                { "floor-odd-missing-4", ExtractSprite(blockSheet, 7, 1) },                
                { "floor-odd-missing-nw,n,ne,e,se,s,sw", ExtractSprite(blockSheet, 5, 9) },                
                { "floor-doorway-vertical", ExtractSprite(blockSheet, 3, 9) },

                { "floor-nw-ext-stair", ExtractSprite(blockSheet, 0, 8) },
                { "floor-ne-ext-stair", ExtractSprite(blockSheet, 1, 8) },
                { "floor-sw-ext-stair", ExtractSprite(blockSheet, 2, 8) },
                { "floor-se-ext-stair", ExtractSprite(blockSheet, 3, 8) },

                




                { "stairs-top-01", ExtractSprite(blockSheet, 6, 5) },
                { "stairs-top-02", ExtractSprite(blockSheet, 7, 5) },
                { "stairs-top-03", ExtractSprite(blockSheet, 8, 5) },
                { "stairs-bottom-01", ExtractSprite(blockSheet, 6, 6) },
                { "stairs-bottom-02", ExtractSprite(blockSheet, 7, 6) },
                { "stairs-bottom-03", ExtractSprite(blockSheet, 8, 6) },

                { "door-int", ExtractSprite(blockSheet, 0, 3) },




                //{ "stairs-01", ExtractSprite(blockSheet, 6, 0) },
                //{ "stairs-02", ExtractSprite(blockSheet, 7, 0) },
                //{ "stairs-03", ExtractSprite(blockSheet, 6, 1) },
                //{ "stairs-04", ExtractSprite(blockSheet, 7, 1) },
                //{ "stairs-05", ExtractSprite(blockSheet, 6, 2) },
                //{ "stairs-06", ExtractSprite(blockSheet, 7, 2) },
                //{ "stairs-07", ExtractSprite(blockSheet, 6, 3) },
                //{ "stairs-08", ExtractSprite(blockSheet, 7, 3) },
                //{ "stairs-09", ExtractSprite(blockSheet, 6, 4) },
                //{ "stairs-10", ExtractSprite(blockSheet, 7, 4) },

                { "roof", Global.MISSING_TEXTURE }
            };

            //blockSheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/Town/house_piece");
            //BuildingBlockSprites.Add("floor-nw-int", ExtractSprite(blockSheet, 3, 0));
            //BuildingBlockSprites.Add("floor-n", ExtractSprite(blockSheet, 4, 0) );
            //BuildingBlockSprites.Add("floor-ne-int", ExtractSprite(blockSheet, 5, 0) );
            //BuildingBlockSprites.Add("floor-w", ExtractSprite(blockSheet, 3, 1) );
            //BuildingBlockSprites.Add("floor-c", ExtractSprite(blockSheet, 4, 1) );
            //BuildingBlockSprites.Add("floor-e", ExtractSprite(blockSheet, 5, 1) );
            //BuildingBlockSprites.Add("floor-sw-int", ExtractSprite(blockSheet, 3, 2) );
            //BuildingBlockSprites.Add("floor-s", ExtractSprite(blockSheet, 4, 2) );
            //BuildingBlockSprites.Add("floor-se-int", ExtractSprite(blockSheet, 5, 2) );
            //BuildingBlockSprites.Add("floor-se-ext", ExtractSprite(blockSheet, 6, 0) );
            //BuildingBlockSprites.Add("floor-sw-ext", ExtractSprite(blockSheet, 7, 0) );
            //BuildingBlockSprites.Add("floor-ne-ext", ExtractSprite(blockSheet, 6, 1) );
            //BuildingBlockSprites.Add("floor-nw-ext", ExtractSprite(blockSheet, 7, 1) );

            ////BuildingBlockSprites.Add("roof", ExtractSprite(blockSheet, 2, 3));
            //BuildingBlockSprites.Add("roof", Global.MISSING_TEXTURE);




            //var blockSheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/Town/Build'[ingTiles2");
            //BuildingBlockSprites = new Dictionary<string, Texture2D>()
            //{
            //    { "wall-face", ExtractSprite(blockSheet, 1, 1) },

            //    { "wall-cap-nw", ExtractSprite(blockSheet, 0, 0) },
            //    { "wall-cap-n", ExtractSprite(blockSheet, 1, 0) },
            //    { "wall-cap-ne", ExtractSprite(blockSheet, 2, 0) },
            //    { "wall-cap-w", ExtractSprite(blockSheet, 0, 1) },
            //    { "wall-cap-e", ExtractSprite(blockSheet, 2, 1) },
            //    { "wall-cap-sw", ExtractSprite(blockSheet, 0, 2) },
            //    { "wall-cap-s", ExtractSprite(blockSheet, 1, 2) },
            //    { "wall-cap-se", ExtractSprite(blockSheet, 2, 2) },

            //    { "floor-nw-int", ExtractSprite(blockSheet, 3, 0) },
            //    { "floor-n", ExtractSprite(blockSheet, 4, 0) },
            //    { "floor-ne-int", ExtractSprite(blockSheet, 5, 0) },

            //    { "floor-w", ExtractSprite(blockSheet, 3, 1) },
            //    { "floor-c", ExtractSprite(blockSheet, 4, 1) },
            //    { "floor-e", ExtractSprite(blockSheet, 5, 1) },

            //    { "floor-sw-int", ExtractSprite(blockSheet, 3, 2) },
            //    { "floor-s", ExtractSprite(blockSheet, 4, 2) },
            //    { "floor-se-int", ExtractSprite(blockSheet, 5, 2) },

            //    { "floor-se-ext", ExtractSprite(blockSheet, 6, 0) },
            //    { "floor-sw-ext", ExtractSprite(blockSheet, 7, 0) },
            //    { "floor-ne-ext", ExtractSprite(blockSheet, 6, 1) },
            //    { "floor-nw-ext", ExtractSprite(blockSheet, 7, 1) },

            //    { "door-ext", ExtractSprite(blockSheet, 3, 3) },
            //    { "door-int", ExtractSprite(blockSheet, 4, 3) },

            //    { "roof", ExtractSprite(blockSheet, 2, 3) },

            //    // Stairs
            //    // ....
            //};



            //BuildingBlockSprites.Add("building-wall-nw", ExtractSprite(blockSheet, 0, 0));
            //BuildingBlockSprites.Add("building-wall-n", ExtractSprite(blockSheet, 1, 0));
            //BuildingBlockSprites.Add("building-wall-ne", ExtractSprite(blockSheet, 2, 0));
            //BuildingBlockSprites.Add("building-wall-w", ExtractSprite(blockSheet, 0, 1));
            //BuildingBlockSprites.Add("building-floor", ExtractSprite(blockSheet, 1, 1));
            //BuildingBlockSprites.Add("building-wall-e", ExtractSprite(blockSheet, 2, 1));
            //BuildingBlockSprites.Add("building-wall-sw", ExtractSprite(blockSheet, 0, 2));
            //BuildingBlockSprites.Add("building-wall-s", ExtractSprite(blockSheet, 1, 2));
            //BuildingBlockSprites.Add("building-wall-se", ExtractSprite(blockSheet, 2, 2));
            //BuildingBlockSprites.Add("building-roof-thatching", ExtractSprite(blockSheet, 3, 0));
            //BuildingBlockSprites.Add("building-door-ext", ExtractSprite(blockSheet, 3, 2));

        }

        private void LoadItems()
        {
            ItemsSprites = new Dictionary<string, Texture2D>();

            ItemsSprites.Add("wood-log", Global.ContentManager.Load<Texture2D>("Sprites/UI/Items/wood-log-01"));
            ItemsSprites.Add("rock", ExtractSprite(Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Rock01"), 0, 0, Global.TileSize, Global.TileSize));
        }

        private void LoadGrass()
        {
            // Grass
            lib.Add("grass-NW", ExtractTileSprite(6, 0));
            lib.Add("grass-N", ExtractTileSprite(7, 0));
            lib.Add("grass-NE", ExtractTileSprite(8, 0));
            lib.Add("grass-W", ExtractTileSprite(6, 1));
            lib.Add("grass", ExtractTileSprite(7, 1));
            lib.Add("grass-E", ExtractTileSprite(8, 1));
            lib.Add("grass-SW", ExtractTileSprite(6, 2));
            lib.Add("grass-S", ExtractTileSprite(7, 2));
            lib.Add("grass-SE", ExtractTileSprite(8, 2));

            // Inverted grass
            lib.Add("grass-inv-NW", ExtractTileSprite(9, 0));
            lib.Add("grass-inv-N", ExtractTileSprite(10, 0));
            lib.Add("grass-inv-NE", ExtractTileSprite(11, 0));
            lib.Add("grass-inv-W", ExtractTileSprite(9, 1));
            lib.Add("grass-inv", ExtractTileSprite(10, 1));
            lib.Add("grass-inv-E", ExtractTileSprite(11, 1));
            lib.Add("grass-inv-SW", ExtractTileSprite(9, 2));
            lib.Add("grass-inv-S", ExtractTileSprite(10, 2));
            lib.Add("grass-inv-SE", ExtractTileSprite(11, 2));
        }

        private void LoadWorldMapSprites()
        {
            var trees = new List<Texture2D>();
            trees.Add(Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Tree2"));
            WorldMapSprites.Add("tree-02", trees);

            var treeStumps = new List<Texture2D>();
            treeStumps.Add(Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Tree-stump-export"));
            WorldMapSprites.Add("tree-stump", treeStumps);

            var treesWinter = new List<Texture2D>();
            treesWinter.Add(Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Tree2-winter"));
            WorldMapSprites.Add("tree-02-winter", treesWinter);

            var rocks = new List<Texture2D>();
            rocks.Add(Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Rock01"));
            WorldMapSprites.Add("rock-01", rocks);

            var cactus = new List<Texture2D>();
            cactus.Add(Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Cactus01"));
            WorldMapSprites.Add("cactus-01", cactus);

            var rockSmashed = new List<Texture2D>();
            var rockSheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Rock01");
            rockSmashed.Add(ExtractSprite(rockSheet, 4 * Global.TileSize, 0, Global.TileSize, Global.TileSize));
            WorldMapSprites.Add("rock-smashed", rockSmashed);
        }

        private void LoadSpriteSheets()
        {
            spriteSheetTiles = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/WorldTerrain01");
            spriteSheetBiomes = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Biomes");
            spriteSheetForest = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Forest");
            spriteSheetMountain = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Mountain");
            spriteSheetCastle = Global.ContentManager.Load<Texture2D>("Sprites/WorldStructures/Castle");

            sheets.Add("biome-grasslands", Global.ContentManager.Load<Texture2D>("Sprites/Map/World/BiomeGrasslands"));
            sheets.Add("player", Global.ContentManager.Load<Texture2D>("Sprites/Player/Player"));
        }

        private void LoadDungeonSprites()
        {
            var sheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/Dungeon/dungeon_tiles");            

            DungeonSprites = new Dictionary<string, List<Texture2D>>
            {
                { "wall-nw", new List<Texture2D> { ExtractSprite(sheet, 0, 0) } },
                { "wall-n", new List<Texture2D> { ExtractSprite(sheet, 1, 0) } },
                { "wall-ne", new List<Texture2D> { ExtractSprite(sheet, 4, 0) } },
                { "wall-w", new List<Texture2D> { ExtractSprite(sheet, 0, 1) } },
                { "wall-e", new List<Texture2D> { ExtractSprite(sheet, 4, 1) } },
                { "wall-sw", new List<Texture2D> { ExtractSprite(sheet, 0, 4) } },
                { "wall-s", new List<Texture2D> { ExtractSprite(sheet, 1, 4) } },
                { "wall-se", new List<Texture2D> { ExtractSprite(sheet, 4, 4) } },

                { "wall-ext-nw", new List<Texture2D> { ExtractSprite(sheet, 5, 0) } },
                { "wall-ext-ne", new List<Texture2D> { ExtractSprite(sheet, 7, 0) } },
                { "wall-ext-sw", new List<Texture2D> { ExtractSprite(sheet, 5, 2) } },
                { "wall-ext-se", new List<Texture2D> { ExtractSprite(sheet, 5, 2) } },
                { "floor-01", new List<Texture2D>() }
            };

            for (int i = 1; i < 4; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    DungeonSprites["floor-01"].Add(ExtractSprite(sheet, i, j));
                }
            }
        }

        private void LoadDungeonSpritesORIGINALWORKING()
        {
            var sheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/Dungeon/sheet");
            sheets.Add("dungeon-layout", sheet);
            //var sheet = _content.Load<Texture2D>("Sprites/Map/Dungeon/Tileset_Dungeon02");

            DungeonSprites = new Dictionary<string, List<Texture2D>>
            {
                { "wall-nw", new List<Texture2D> { ExtractSprite(sheet, 0, 0) } },
                { "wall-n", new List<Texture2D> { ExtractSprite(sheet, 1, 0) } },
                { "wall-ne", new List<Texture2D> { ExtractSprite(sheet, 2, 0) } },
                { "wall-w", new List<Texture2D> { ExtractSprite(sheet, 0, 1) } },
                { "wall-e", new List<Texture2D> { ExtractSprite(sheet, 2, 1) } },
                { "wall-sw", new List<Texture2D> { ExtractSprite(sheet, 0, 2) } },
                { "wall-s", new List<Texture2D> { ExtractSprite(sheet, 1, 2) } },
                { "wall-se", new List<Texture2D> { ExtractSprite(sheet, 2, 2) } },
                { "wall-ext-nw", new List<Texture2D> { ExtractSprite(sheet, 3, 0) } },
                { "wall-ext-ne", new List<Texture2D> { ExtractSprite(sheet, 4, 0) } },
                { "wall-ext-sw", new List<Texture2D> { ExtractSprite(sheet, 3, 1) } },
                { "wall-ext-se", new List<Texture2D> { ExtractSprite(sheet, 4, 1) } },
                { "floor-01", new List<Texture2D>() }
            };

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    DungeonSprites["floor-01"].Add(ExtractSprite(sheet, i + 6, j));
                }
            }
        }

        private void LoadDungeonPopulationSprites()
        {
            var sheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/Dungeon/stairs");

            DungeonSprites.Add("stairs-nw", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 0, 0) } );
            DungeonSprites.Add("stairs-n", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 1, 0) } );
            DungeonSprites.Add("stairs-ne", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 2, 0) } );
            DungeonSprites.Add("stairs-w", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 0, 1) } );
            DungeonSprites.Add("stairs-c", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 1, 1) } );
            DungeonSprites.Add("stairs-e", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 2, 1) } );
            DungeonSprites.Add("stairs-sw", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 0, 2) } );
            DungeonSprites.Add("stairs-s", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 1, 2) } );
            DungeonSprites.Add("stairs-se", new List<Texture2D> { SpriteTools.ExtractSprite(sheet, 2, 2) } );
        }

        private void LoadBiomeGrasslands()
        {
            var sheet = sheets["biome-grasslands"];

            lib.Add("grasslands-ext-NW", ExtractSprite(sheet, 0, 0));
            lib.Add("grasslands-ext-N", ExtractSprite(sheet, 1, 0));
            lib.Add("grasslands-ext-NE", ExtractSprite(sheet, 2, 0));
            lib.Add("grasslands-ext-W", ExtractSprite(sheet, 0, 1));
            lib.Add("grasslands-ext-C", ExtractSprite(sheet, 1, 1));
            lib.Add("grasslands-ext-E", ExtractSprite(sheet, 2, 1));
            lib.Add("grasslands-ext-SW", ExtractSprite(sheet, 0, 2));
            lib.Add("grasslands-ext-S", ExtractSprite(sheet, 1, 2));
            lib.Add("grasslands-ext-SE", ExtractSprite(sheet, 2, 2));

            lib.Add("grasslands-int-NW", ExtractSprite(sheet, 3, 0));
            lib.Add("grasslands-int-N", ExtractSprite(sheet, 4, 0));
            lib.Add("grasslands-int-NE", ExtractSprite(sheet, 5, 0));
            lib.Add("grasslands-int-W", ExtractSprite(sheet, 3, 1));
            lib.Add("grasslands-int-C", ExtractSprite(sheet, 4, 1));
            lib.Add("grasslands-int-E", ExtractSprite(sheet, 5, 1));
            lib.Add("grasslands-int-SW", ExtractSprite(sheet, 3, 2));
            lib.Add("grasslands-int-S", ExtractSprite(sheet, 4, 2));
            lib.Add("grasslands-int-SE", ExtractSprite(sheet, 5, 2));

            lib["grasslands-ext-NW"].Name = "grasslands-ext-NW";
            lib["grasslands-ext-N"].Name = "grasslands-ext-N";
            lib["grasslands-ext-NE"].Name = "grasslands-ext-NE";
            lib["grasslands-ext-W"].Name = "grasslands-ext-W";
            lib["grasslands-ext-C"].Name = "grasslands-ext-C";
            lib["grasslands-ext-E"].Name = "grasslands-ext-E";
            lib["grasslands-ext-SW"].Name = "grasslands-ext-SW";
            lib["grasslands-ext-S"].Name = "grasslands-ext-S";
            lib["grasslands-ext-SE"].Name = "grasslands-ext-SE";

            lib["grasslands-int-NW"].Name = "grasslands-int-NW";
            lib["grasslands-int-N"].Name = "grasslands-int-N";
            lib["grasslands-int-NE"].Name = "grasslands-int-NE";
            lib["grasslands-int-W"].Name = "grasslands-int-W";
            lib["grasslands-int-C"].Name = "grasslands-int-C";
            lib["grasslands-int-E"].Name = "grasslands-int-E";
            lib["grasslands-int-SW"].Name = "grasslands-int-SW";
            lib["grasslands-int-S"].Name = "grasslands-int-S";
            lib["grasslands-int-SE"].Name = "grasslands-int-SE";
        }

        private void LoadPlayerNEW()
        {
            var sheet = sheets["player"];

            lib.Add("player-N", ExtractSprite(sheet, 1, 0));
            lib.Add("player-E", ExtractSprite(sheet, 2, 1));
            lib.Add("player-S", ExtractSprite(sheet, 1, 2));
            lib.Add("player-W", ExtractSprite(sheet, 0, 1));
        }

        private void LoadMountain()
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

        private void LoadForest()
        {
            // Forest
            ExtractSprites("forest", spriteSheetForest, 3, 3);
            lib.Add("forest-inv-NW", ExtractSprite(spriteSheetForest, 3, 0));
            lib.Add("forest-inv-N", ExtractSprite(spriteSheetForest, 4, 0));
            lib.Add("forest-inv-NE", ExtractSprite(spriteSheetForest, 5, 0));
            lib.Add("forest-inv-W", ExtractSprite(spriteSheetForest, 3, 1));
            lib.Add("forest-inv-E", ExtractSprite(spriteSheetForest, 5, 1));
            lib.Add("forest-inv-SW", ExtractSprite(spriteSheetForest, 3, 2));
            lib.Add("forest-inv-S", ExtractSprite(spriteSheetForest, 4, 2));
            lib.Add("forest-inv-SE", ExtractSprite(spriteSheetForest, 5, 2));

            lib.Add("forest-frost-NW", ExtractSprite(spriteSheetForest, 0, 3));
            lib.Add("forest-frost-N", ExtractSprite(spriteSheetForest, 1, 3));
            lib.Add("forest-frost-NE", ExtractSprite(spriteSheetForest, 2, 3));
            lib.Add("forest-frost-W", ExtractSprite(spriteSheetForest, 0, 4));
            lib.Add("forest-frost-C", ExtractSprite(spriteSheetForest, 1, 4));
            lib.Add("forest-frost-E", ExtractSprite(spriteSheetForest, 2, 4));
            lib.Add("forest-frost-SW", ExtractSprite(spriteSheetForest, 0, 5));
            lib.Add("forest-frost-S", ExtractSprite(spriteSheetForest, 1, 5));
            lib.Add("forest-frost-SE", ExtractSprite(spriteSheetForest, 2, 5));

            lib.Add("forest-frost-inv-NW", ExtractSprite(spriteSheetForest, 3, 3));
            lib.Add("forest-frost-inv-N", ExtractSprite(spriteSheetForest, 4, 3));
            lib.Add("forest-frost-inv-NE", ExtractSprite(spriteSheetForest, 5, 3));
            lib.Add("forest-frost-inv-W", ExtractSprite(spriteSheetForest, 3, 4));
            lib.Add("forest-frost-inv-C", ExtractSprite(spriteSheetForest, 4, 4));
            lib.Add("forest-frost-inv-E", ExtractSprite(spriteSheetForest, 5, 4));
            lib.Add("forest-frost-inv-SW", ExtractSprite(spriteSheetForest, 3, 5));
            lib.Add("forest-frost-inv-S", ExtractSprite(spriteSheetForest, 4, 5));
            lib.Add("forest-frost-inv-SE", ExtractSprite(spriteSheetForest, 5, 5));
        }

        private void LoadCoast()
        {
            // Coast
            lib.Add("coast-NW", ExtractTileSprite(0, 0));
            lib.Add("coast-N", ExtractTileSprite(1, 0));
            lib.Add("coast-NE", ExtractTileSprite(2, 0));
            lib.Add("coast-W", ExtractTileSprite(0, 1));
            lib.Add("coast", ExtractTileSprite(1, 1));
            lib.Add("coast-E", ExtractTileSprite(2, 1));
            lib.Add("coast-SW", ExtractTileSprite(0, 2));
            lib.Add("coast-S", ExtractTileSprite(1, 2));
            lib.Add("coast-SE", ExtractTileSprite(2, 2));

            // Inverted coast
            lib.Add("coast-inv-NW", ExtractTileSprite(3, 0));
            lib.Add("coast-inv-N", ExtractTileSprite(4, 0));
            lib.Add("coast-inv-NE", ExtractTileSprite(5, 0));
            lib.Add("coast-inv-W", ExtractTileSprite(3, 1));
            lib.Add("coast-inv", ExtractTileSprite(4, 1));
            lib.Add("coast-inv-E", ExtractTileSprite(5, 1));
            lib.Add("coast-inv-SW", ExtractTileSprite(3, 2));
            lib.Add("coast-inv-S", ExtractTileSprite(4, 2));
            lib.Add("coast-inv-SE", ExtractTileSprite(5, 2));
        }

        private void LoadWater()
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

        private void LoadMouseCursor()
        {
            var sheet = Global.ContentManager.Load<Texture2D>("Sprites/UI/MouseCursor");

            lib.Add("cursor", ExtractSprite(sheet, 0, 0));
        }

        private void LoadUIScroll()
        {
            var sheet = Global.ContentManager.Load<Texture2D>("Sprites/UI/Scroll");

            lib.Add("scroll-left", ExtractSprite(sheet, 0, 0));
            lib.Add("scroll-middle", ExtractSprite(sheet, 1, 0));
            lib.Add("scroll-right", ExtractSprite(sheet, 2, 0));
        }

        private void LoadStructureTown()
        {
            spriteSheetTown = Global.ContentManager.Load<Texture2D>("Sprites/WorldStructures/Town01");
            lib.Add("town", spriteSheetTown);

            //for (int i = 0; i < 4; i++)
            //{
            //    lib.Add($"town-{i + 1:D2}-NW", ExtractSprite(spriteSheetTown, 0 + i * 3, 0));
            //    lib.Add($"town-{i + 1:D2}-N", ExtractSprite(spriteSheetTown, 1 + i * 3, 0));
            //    lib.Add($"town-{i + 1:D2}-NE", ExtractSprite(spriteSheetTown, 2 + i * 3, 0));

            //    lib.Add($"town-{i + 1:D2}-W", ExtractSprite(spriteSheetTown, 0 + i * 3, 1));
            //    lib.Add($"town-{i + 1:D2}-C", ExtractSprite(spriteSheetTown, 1 + i * 3, 1));
            //    lib.Add($"town-{i + 1:D2}-E", ExtractSprite(spriteSheetTown, 2 + i * 3, 1));

            //    lib.Add($"town-{i + 1:D2}-SW", ExtractSprite(spriteSheetTown, 0 + i * 3, 2));
            //    lib.Add($"town-{i + 1:D2}-S", ExtractSprite(spriteSheetTown, 1 + i * 3, 2));
            //    lib.Add($"town-{i + 1:D2}-SE", ExtractSprite(spriteSheetTown, 2 + i * 3, 2));
            //}
        }

        private void LoadStructureCastle()
        {
            spriteSheetCastle = Global.ContentManager.Load<Texture2D>("Sprites/WorldStructures/Castle");
            lib.Add("castle", spriteSheetCastle);

            var frameCount = 1;
            var rowCount = 9;
            var colCount = 9;

            for (int i = 0; i < frameCount; i++)
            {
                for (int j = 0; j < rowCount; j++)
                {
                    for (int k = 0; k < colCount; k++)
                    {
                        var sprite = ExtractSprite(spriteSheetCastle, j + i * 3, k);
                        lib.Add($"castle-{i + 1:D2}-{j}-{k}", sprite);
                    }
                }
            }
        }

        private void LoadDialogueSystem()
        {
            sheets.Add("dialogue", Global.ContentManager.Load<Texture2D>("Sprites/UI/Dialogue"));

            lib.Add("dialogue-NW", ExtractSprite(sheets["dialogue"], 0, 0));
            lib.Add("dialogue-N", ExtractSprite(sheets["dialogue"], 1, 0));
            lib.Add("dialogue-NE", ExtractSprite(sheets["dialogue"], 2, 0));

            lib.Add("dialogue-W", ExtractSprite(sheets["dialogue"], 0, 1));
            lib.Add("dialogue-C", ExtractSprite(sheets["dialogue"], 1, 1));
            lib.Add("dialogue-E", ExtractSprite(sheets["dialogue"], 2, 1));

            lib.Add("dialogue-SW", ExtractSprite(sheets["dialogue"], 0, 2));
            lib.Add("dialogue-S", ExtractSprite(sheets["dialogue"], 1, 2));
            lib.Add("dialogue-SE", ExtractSprite(sheets["dialogue"], 2, 2));
        }

        public void ExtractSprites(string spriteSet, Texture2D spriteSheet, int xCount, int yCount)
        {
            for (int x = 0; x < xCount; x++)
            {
                for (int y = 0; y < yCount; y++)
                {
                    var sprite = ExtractSprite(spriteSheet, x, y);
                    var direction = DetermineSpriteDirection(x, y);

                    lib.Add($"{spriteSet}-{direction}", sprite);
                }
            }
        }

        private string DetermineSpriteDirection(int x, int y)
        {
            if (x == 0 && y == 0) return "NW";
            if (x == 1 && y == 0) return "N";
            if (x == 2 && y == 0) return "NE";

            if (x == 0 && y == 1) return "W";
            if (x == 1 && y == 1) return "C";
            if (x == 2 && y == 1) return "E";

            if (x == 0 && y == 2) return "SW";
            if (x == 1 && y == 2) return "S";
            if (x == 2 && y == 2) return "SE";

            else
                return "C";
        }

        private Texture2D ExtractSprite(Texture2D spriteSheet, int x, int y)
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

        public Texture2D GetSprite(string key)
        {
            return lib[key];
        }

        private Texture2D ExtractBiomeSprite(int x, int y)
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

        private Texture2D ExtractTileSprite(int x, int y)
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

        private Texture2D ExtractForestSprite(int x, int y)
        {
            var width = Global.TileSize;
            var height = Global.TileSize;

            x *= width;
            y *= height;

            Rectangle sourceRectangle = new Rectangle(x, y, width, height);

            // Extract the pixel data from the spritesheet
            Color[] data = new Color[width * height];
            spriteSheetForest.GetData(0, sourceRectangle, data, 0, data.Length);

            // Create a new texture for the sprite and set the pixel data
            Texture2D sprite = new Texture2D(Global.GraphicsDevice, width, height);
            sprite.SetData(data);

            // Store the new texture in the array
            return sprite;
        }
    }
}
