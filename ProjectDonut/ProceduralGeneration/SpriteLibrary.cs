using AsepriteDotNet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration
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


        private ContentManager content;
        private GraphicsDevice graphicsDevice;

        private Dictionary<string, Texture2D> lib;

        private int TileSize = 32;

        public SpriteLibrary(ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.content = content;
            this.graphicsDevice = graphicsDevice;
        }

        public void LoadSpriteLibrary()
        {
            sheets = new Dictionary<string, Texture2D>();
            lib = new Dictionary<string, Texture2D>();

            spriteSheetTiles = content.Load<Texture2D>("Sprites/Map/World/WorldTerrain01");
            spriteSheetBiomes = content.Load<Texture2D>("Sprites/Map/World/Biomes");
            spriteSheetForest = content.Load<Texture2D>("Sprites/Map/World/Forest");
            spriteSheetMountain = content.Load<Texture2D>("Sprites/Map/World/Mountain");
            spriteSheetCastle = content.Load<Texture2D>("Sprites/WorldStructures/Castle");

            LoadStructureCastle();
            LoadDialogueSystem();
            LoadStructureTown();
            
            LoadMouseCursor();
            


            // Water
            lib.Add("deepwater-NW", ExtractSprite(spriteSheetTiles, 18, 0));
            lib.Add("deepwater-N", ExtractSprite(spriteSheetTiles, 19, 0));
            lib.Add("deepwater-NE", ExtractSprite(spriteSheetTiles, 20, 0));
            lib.Add("deepwater-W", ExtractSprite(spriteSheetTiles, 18, 1));
            lib.Add("deepwater-C", ExtractSprite(spriteSheetTiles, 19, 1));
            lib.Add("deepwater-E", ExtractSprite(spriteSheetTiles, 20, 1));
            lib.Add("deepwater-SW", ExtractSprite(spriteSheetTiles, 18, 2));
            lib.Add("deepwater-S", ExtractSprite(spriteSheetTiles, 19, 2));
            lib.Add("deepwater-SE", ExtractSprite(spriteSheetTiles, 20, 2));

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

            // Biomes
            lib.Add("grasslands", ExtractBiomeSprite(0, 0));
            lib.Add("desert", ExtractBiomeSprite(1, 0));
            lib.Add("winterlands", ExtractBiomeSprite(2, 0));

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

        private void LoadMouseCursor()
        {
            var sheet = content.Load<Texture2D>("Sprites/UI/MouseCursor");

            lib.Add("cursor", ExtractSprite(sheet, 0, 0));
        }

        private void LoadUIScroll()
        {
            var sheet = content.Load<Texture2D>("Sprites/UI/Scroll");

            lib.Add("scroll-left", ExtractSprite(sheet, 0, 0));
            lib.Add("scroll-middle", ExtractSprite(sheet, 1, 0));
            lib.Add("scroll-right", ExtractSprite(sheet, 2, 0));
        }

        private void LoadStructureTown()
        {
            spriteSheetTown = content.Load<Texture2D>("Sprites/WorldStructures/Town01");

            for (int i = 0; i < 4; i++)
            {
                lib.Add($"town-{i + 1:D2}-NW", ExtractSprite(spriteSheetTown, 0 + (i * 3), 0));
                lib.Add($"town-{i + 1:D2}-N", ExtractSprite(spriteSheetTown, 1 + (i * 3), 0));
                lib.Add($"town-{i + 1:D2}-NE", ExtractSprite(spriteSheetTown, 2 + (i * 3), 0));

                lib.Add($"town-{i + 1:D2}-W", ExtractSprite(spriteSheetTown, 0 + (i * 3), 1));
                lib.Add($"town-{i + 1:D2}-C", ExtractSprite(spriteSheetTown, 1 + (i * 3), 1));
                lib.Add($"town-{i + 1:D2}-E", ExtractSprite(spriteSheetTown, 2 + (i * 3), 1));

                lib.Add($"town-{i + 1:D2}-SW", ExtractSprite(spriteSheetTown, 0 + (i * 3), 2));
                lib.Add($"town-{i + 1:D2}-S", ExtractSprite(spriteSheetTown, 1 + (i * 3), 2));
                lib.Add($"town-{i + 1:D2}-SE", ExtractSprite(spriteSheetTown, 2 + (i * 3), 2));
            }
        }

        private void LoadStructureCastle()
        {
            spriteSheetCastle = content.Load<Texture2D>("Sprites/WorldStructures/Castle");

            var frameCount = 1;
            var rowCount = 9;
            var colCount = 9;

            for (int i = 0; i < frameCount; i++)
            {
                for (int j = 0; j < rowCount; j++)
                {
                    for (int k = 0; k < colCount; k++)
                    {
                        var sprite = ExtractSprite(spriteSheetCastle, j + (i * 3), k);
                        lib.Add($"castle-{i + 1:D2}-{j}-{k}", sprite);
                    }
                }
            }
        }

        private void LoadDialogueSystem()
        {
            sheets.Add("dialogue", content.Load<Texture2D>("Sprites/UI/Dialogue"));

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
            var width = TileSize;
            var height = TileSize;

            x *= width;
            y *= height;

            Rectangle sourceRectangle = new Rectangle(x, y, width, height);

            // Extract the pixel data from the spritesheet
            Color[] data = new Color[width * height];
            spriteSheet.GetData(0, sourceRectangle, data, 0, data.Length);

            // Create a new texture for the sprite and set the pixel data
            Texture2D sprite = new Texture2D(graphicsDevice, width, height);
            sprite.SetData(data);

            return sprite;
        }

        public Texture2D GetSprite(string key)
        {
            return lib[key];
        }

        private Texture2D ExtractBiomeSprite(int x, int y)
        {
            var width = TileSize;
            var height = TileSize;

            x *= width;
            y *= height;

            Rectangle sourceRectangle = new Rectangle(x, y, width, height);

            // Extract the pixel data from the spritesheet
            Color[] data = new Color[width * height];
            spriteSheetBiomes.GetData(0, sourceRectangle, data, 0, data.Length);

            // Create a new texture for the sprite and set the pixel data
            Texture2D sprite = new Texture2D(graphicsDevice, width, height);
            sprite.SetData(data);

            // Store the new texture in the array
            return sprite;
        }

        private Texture2D ExtractTileSprite(int x, int y)
        {
            var width = TileSize;
            var height = TileSize;

            x *= width;
            y *= height;

            Rectangle sourceRectangle = new Rectangle(x, y, width, height);

            // Extract the pixel data from the spritesheet
            Color[] data = new Color[width * height];
            spriteSheetTiles.GetData(0, sourceRectangle, data, 0, data.Length);

            // Create a new texture for the sprite and set the pixel data
            Texture2D sprite = new Texture2D(graphicsDevice, width, height);
            sprite.SetData(data);

            // Store the new texture in the array
            return sprite;
        }

        private Texture2D ExtractForestSprite(int x, int y)
        {
            var width = TileSize;
            var height = TileSize;

            x *= width;
            y *= height;

            Rectangle sourceRectangle = new Rectangle(x, y, width, height);

            // Extract the pixel data from the spritesheet
            Color[] data = new Color[width * height];
            spriteSheetForest.GetData(0, sourceRectangle, data, 0, data.Length);

            // Create a new texture for the sprite and set the pixel data
            Texture2D sprite = new Texture2D(graphicsDevice, width, height);
            sprite.SetData(data);

            // Store the new texture in the array
            return sprite;
        }
    }
}
