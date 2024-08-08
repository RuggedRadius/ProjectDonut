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


        private Tilemap tilemap;

        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        private WorldTileRuler rules;
        private SpriteLibrary spriteLib;

        public WorldGenerator(ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.content = content;
            this.graphicsDevice = graphicsDevice;
            this.spriteLib = new SpriteLibrary(content, graphicsDevice);
        }

        public Tilemap Generate(int width, int height)
        {
            spriteLib.LoadSpriteLibrary();

            GenerateTerrain(width, height);
            GenerateBiomes(width, height);
            CreateTilemap(heightData);

            rules = new WorldTileRuler(spriteLib, tilemap);
            tilemap = rules.ApplyTileRules();

            return tilemap;
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
                return (spriteLib.GetSprite("mountain"), TileType.Mountain);
            }
            else if (heightIndex >= 3)
            {
                switch (biome)
                {
                    case Biome.Desert: return (spriteLib.GetSprite("desert"), TileType.Grass);
                    case Biome.Grasslands: return (spriteLib.GetSprite("grasslands"), TileType.Grass);
                    case Biome.Winterlands: return (spriteLib.GetSprite("winterlands"), TileType.Grass);
                    default: return (spriteLib.GetSprite("grasslands"), TileType.Grass);
                }
            }
            //else if (heightIndex >= 6)
            //{
            //    return (spriteLib.GetSprite("grass"), TileType.Grass);
            //}
            //else if (heightIndex >= 3)
            //{
            //    return (spriteLib.GetSprite("coast"), TileType.Coast);
            //}
            else
            {
                return (spriteLib.GetSprite("coast-inv"), TileType.Water);
            }
        }
    }
}
