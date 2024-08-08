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


        private ContentManager content;
        private GraphicsDevice graphicsDevice;

        private Dictionary<string, Texture2D> spriteLib;

        private int TileSize = 32;

        public SpriteLibrary(ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.content = content;
            this.graphicsDevice = graphicsDevice;
        }

        public void LoadSpriteLibrary()
        {
            spriteSheetTiles = content.Load<Texture2D>("Sprites/Map/World/WorldTerrain01");
            spriteSheetBiomes = content.Load<Texture2D>("Sprites/Map/World/Biomes");
            spriteSheetForest = content.Load<Texture2D>("Sprites/Map/World/Forest");

            spriteLib = new Dictionary<string, Texture2D>();

            // Water
            spriteLib.Add("deepwater-NW", ExtractSprite(spriteSheetTiles, 18, 0));
            spriteLib.Add("deepwater-N",  ExtractSprite(spriteSheetTiles, 19, 0));
            spriteLib.Add("deepwater-NE", ExtractSprite(spriteSheetTiles, 20, 0));
            spriteLib.Add("deepwater-W", ExtractSprite(spriteSheetTiles, 18, 1));
            spriteLib.Add("deepwater-C", ExtractSprite(spriteSheetTiles, 19, 1));
            spriteLib.Add("deepwater-E", ExtractSprite(spriteSheetTiles, 20, 1));
            spriteLib.Add("deepwater-SW", ExtractSprite(spriteSheetTiles, 18, 2));
            spriteLib.Add("deepwater-S",  ExtractSprite(spriteSheetTiles, 19, 2));
            spriteLib.Add("deepwater-SE", ExtractSprite(spriteSheetTiles, 20, 2));

            // Forest
            ExtractSprites("forest", spriteSheetForest, 3, 3);
            spriteLib.Add("forest-inv-NW", ExtractSprite(spriteSheetForest, 3, 0));
            spriteLib.Add("forest-inv-N",  ExtractSprite(spriteSheetForest, 4, 0));
            spriteLib.Add("forest-inv-NE", ExtractSprite(spriteSheetForest, 5, 0));
            spriteLib.Add("forest-inv-W", ExtractSprite(spriteSheetForest, 3, 1));
            spriteLib.Add("forest-inv-E", ExtractSprite(spriteSheetForest, 5, 1));
            spriteLib.Add("forest-inv-SW", ExtractSprite(spriteSheetForest, 3, 2));
            spriteLib.Add("forest-inv-S",  ExtractSprite(spriteSheetForest, 4, 2));
            spriteLib.Add("forest-inv-SE", ExtractSprite(spriteSheetForest, 5, 2));

            // Biomes
            spriteLib.Add("grasslands", ExtractBiomeSprite(0, 0));
            spriteLib.Add("desert", ExtractBiomeSprite(1, 0));
            spriteLib.Add("winterlands", ExtractBiomeSprite(2, 0));

            // Coast
            spriteLib.Add("coast-NW", ExtractTileSprite(0, 0));
            spriteLib.Add("coast-N", ExtractTileSprite(1, 0));
            spriteLib.Add("coast-NE", ExtractTileSprite(2, 0));
            spriteLib.Add("coast-W", ExtractTileSprite(0, 1));
            spriteLib.Add("coast", ExtractTileSprite(1, 1));
            spriteLib.Add("coast-E", ExtractTileSprite(2, 1));
            spriteLib.Add("coast-SW", ExtractTileSprite(0, 2));
            spriteLib.Add("coast-S", ExtractTileSprite(1, 2));
            spriteLib.Add("coast-SE", ExtractTileSprite(2, 2));

            // Inverted coast
            spriteLib.Add("coast-inv-NW", ExtractTileSprite(3, 0));
            spriteLib.Add("coast-inv-N", ExtractTileSprite(4, 0));
            spriteLib.Add("coast-inv-NE", ExtractTileSprite(5, 0));
            spriteLib.Add("coast-inv-W", ExtractTileSprite(3, 1));
            spriteLib.Add("coast-inv", ExtractTileSprite(4, 1));
            spriteLib.Add("coast-inv-E", ExtractTileSprite(5, 1));
            spriteLib.Add("coast-inv-SW", ExtractTileSprite(3, 2));
            spriteLib.Add("coast-inv-S", ExtractTileSprite(4, 2));
            spriteLib.Add("coast-inv-SE", ExtractTileSprite(5, 2));

            // Grass
            spriteLib.Add("grass-NW", ExtractTileSprite(6, 0));
            spriteLib.Add("grass-N", ExtractTileSprite(7, 0));
            spriteLib.Add("grass-NE", ExtractTileSprite(8, 0));
            spriteLib.Add("grass-W", ExtractTileSprite(6, 1));
            spriteLib.Add("grass", ExtractTileSprite(7, 1));
            spriteLib.Add("grass-E", ExtractTileSprite(8, 1));
            spriteLib.Add("grass-SW", ExtractTileSprite(6, 2));
            spriteLib.Add("grass-S", ExtractTileSprite(7, 2));
            spriteLib.Add("grass-SE", ExtractTileSprite(8, 2));

            // Inverted grass
            spriteLib.Add("grass-inv-NW", ExtractTileSprite(9, 0));
            spriteLib.Add("grass-inv-N", ExtractTileSprite(10, 0));
            spriteLib.Add("grass-inv-NE", ExtractTileSprite(11, 0));
            spriteLib.Add("grass-inv-W", ExtractTileSprite(9, 1));
            spriteLib.Add("grass-inv", ExtractTileSprite(10, 1));
            spriteLib.Add("grass-inv-E", ExtractTileSprite(11, 1));
            spriteLib.Add("grass-inv-SW", ExtractTileSprite(9, 2));
            spriteLib.Add("grass-inv-S", ExtractTileSprite(10, 2));
            spriteLib.Add("grass-inv-SE", ExtractTileSprite(11, 2));

            // Mountain
            spriteLib.Add("mountain-NW", ExtractTileSprite(12, 0));
            spriteLib.Add("mountain-N", ExtractTileSprite(13, 0));
            spriteLib.Add("mountain-NE", ExtractTileSprite(14, 0));
            spriteLib.Add("mountain-W", ExtractTileSprite(12, 1));
            spriteLib.Add("mountain", ExtractTileSprite(13, 1));
            spriteLib.Add("mountain-E", ExtractTileSprite(14, 1));
            spriteLib.Add("mountain-SW", ExtractTileSprite(12, 2));
            spriteLib.Add("mountain-S", ExtractTileSprite(13, 2));
            spriteLib.Add("mountain-SE", ExtractTileSprite(14, 2));

            // Inverted mountain
            spriteLib.Add("mountain-inv-NW", ExtractTileSprite(15, 0));
            spriteLib.Add("mountain-inv-N", ExtractTileSprite(16, 0));
            spriteLib.Add("mountain-inv-NE", ExtractTileSprite(17, 0));
            spriteLib.Add("mountain-inv-W", ExtractTileSprite(15, 1));
            spriteLib.Add("mountain-inv", ExtractTileSprite(16, 1));
            spriteLib.Add("mountain-inv-E", ExtractTileSprite(17, 1));
            spriteLib.Add("mountain-inv-SW", ExtractTileSprite(15, 2));
            spriteLib.Add("mountain-inv-S", ExtractTileSprite(16, 2));
            spriteLib.Add("mountain-inv-SE", ExtractTileSprite(17, 2));
        }

        public void ExtractSprites(string spriteSet, Texture2D spriteSheet, int xCount, int yCount)
        {
            for (int x = 0; x < xCount; x++)
            {
                for (int y = 0; y < yCount; y++)
                {
                    var sprite = ExtractSprite(spriteSheet, x, y);
                    var direction = DetermineSpriteDirection(x, y);

                    spriteLib.Add($"{spriteSet}-{direction}", sprite);
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
            return spriteLib[key];
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
