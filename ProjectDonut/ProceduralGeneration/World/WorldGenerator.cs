using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class WorldGenerator
    {
        private int[,] heightData;
        private int[,] biomeData;
        private int[,] forestData;

        private Texture2D spriteSheetTiles;
        private Texture2D spriteSheetBiomes;
        private Dictionary<string, Texture2D> spriteLib;
        private Tilemap tilemap;

        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        private WorldTileRuler rules;

        public WorldGenerator(ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.content = content;
            this.graphicsDevice = graphicsDevice;
        }

        public Tilemap Generate(int width, int height)
        {
            LoadSpriteLibrary();

            GenerateTerrain(width, height);
            GenerateBiomes(width, height);
            CreateTilemap(heightData);

            rules = new WorldTileRuler(spriteLib, tilemap);
            tilemap = rules.ApplyTileRules();

            return tilemap;
        }        

        private void LoadSpriteLibrary()
        {
            spriteSheetTiles = content.Load<Texture2D>("Sprites/Map/World/WorldTerrain01");
            spriteSheetBiomes = content.Load<Texture2D>("Sprites/Map/World/Biomes");

            spriteLib = new Dictionary<string, Texture2D>();

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

        private Texture2D ExtractBiomeSprite(int x, int y)
        {
            var width = 32;
            var height = 32;

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
            var width = 32;
            var height = 32;

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

        private void CreateTilemap(int[,] mapData)
        {
            tilemap = new Tilemap(mapData.GetLength(0), mapData.GetLength(1));

            for (int i = 0; i < mapData.GetLength(0); i++)
            {
                for (int j = 0; j < mapData.GetLength(1); j++)
                {
                    var determinations = DetermineTexture(i, j);

                    var tile = new Tile
                    {
                        xIndex = i,
                        yIndex = j,
                        Position = new Vector2(i * 32, j * 32),
                        Size = new Vector2(32, 32),
                        Texture = determinations.Item1,
                        TileType = determinations.Item2
                    };

                    tilemap.Map[i, j] = tile;
                }
            }
        }

        public void GenerateTerrain(int width, int height)
        {
            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            noise.SetSeed(new Random().Next(int.MinValue, int.MaxValue));

            // Gather noise data
            float[,] noiseData = new float[height, width];
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    noiseData[x, y] = noise.GetNoise(x, y);

                    if (noiseData[x, y] < minValue)
                        minValue = noiseData[x, y];
                    if (noiseData[x, y] > maxValue)
                        maxValue = noiseData[x, y];
                }
            }

            // Normalize and convert to integer
            int[,] intData = new int[height, width];
            float range = maxValue - minValue;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Normalize value to the range [0, 1]
                    float normalizedValue = (noiseData[x, y] - minValue) / range;

                    // Scale to integer range (e.g., 0 to 255)
                    intData[x, y] = (int)(normalizedValue * 9);
                }
            }

            heightData = intData;
        }

        public void GenerateBiomes(int width, int height)
        {
            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            noise.SetSeed(new Random().Next(int.MinValue, int.MaxValue));
            
            noise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
            noise.SetCellularJitter(1.0f);
            noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);

            noise.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            noise.SetDomainWarpAmp(100.0f);
            noise.SetFrequency(0.0075f);

            noise.SetFractalGain(0.5f);
            noise.SetFractalType(FastNoiseLite.FractalType.DomainWarpIndependent);
            noise.SetFractalOctaves(3);
            noise.SetFractalLacunarity(2.0f);

            // Gather noise data
            float[,] noiseData = new float[height, width];
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    noiseData[x, y] = noise.GetNoise(x, y);

                    if (noiseData[x, y] < minValue)
                        minValue = noiseData[x, y];
                    if (noiseData[x, y] > maxValue)
                        maxValue = noiseData[x, y];
                }
            }

            // Normalize and convert to integer
            int[,] intData = new int[height, width];
            float range = maxValue - minValue;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Normalize value to the range [0, 1]
                    float normalizedValue = (noiseData[x, y] - minValue) / range;

                    // Scale to integer range (e.g., 0 to 255)
                    intData[x, y] = (int)(normalizedValue * Enum.GetNames(typeof(Biome)).Length);
                }
            }

            biomeData = intData;
        }

        private (Texture2D, TileType) DetermineTexture(int x, int y)
        {
            var biomeIndex = biomeData[x, y];
            var heightIndex = heightData[x, y];

            var biome = (Biome) biomeIndex;

            if (heightIndex >= 8)
            {
                return (spriteLib["mountain"], TileType.Mountain);
            }
            else if (heightIndex >= 3)
            {
                switch (biome)
                {
                    case Biome.Desert: return (spriteLib["desert"], TileType.Grass);
                    case Biome.Grasslands: return (spriteLib["grasslands"], TileType.Grass);
                    case Biome.Winterlands: return (spriteLib["winterlands"], TileType.Grass);
                    default: return (spriteLib["grasslands"], TileType.Grass);
                }
            }
            //else if (heightIndex >= 6)
            //{
            //    return (spriteLib["grass"], TileType.Grass);
            //}
            //else if (heightIndex >= 3)
            //{
            //    return (spriteLib["coast"], TileType.Coast);
            //}
            else
            {
                return (spriteLib["coast-inv"], TileType.Water);
            }
        }
    }
}
