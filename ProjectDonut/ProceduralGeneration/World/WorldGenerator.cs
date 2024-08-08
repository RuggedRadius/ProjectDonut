using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class WorldGenerator
    {
        private int[,] biomeData;

        private int[,] heightData;
        private Tilemap tmBase;
        
        private int[,] forestData;
        private Tilemap tmForest;

        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        private WorldTileRuler rules;
        private SpriteLibrary spriteLib;

        private WorldMapSettings settings;

        public WorldGenerator(ContentManager content, GraphicsDevice graphicsDevice, WorldMapSettings settings)
        {
            this.content = content;
            this.graphicsDevice = graphicsDevice;
            this.spriteLib = new SpriteLibrary(content, graphicsDevice);
            this.settings = settings;
        }

        public Tilemap GenerateBaseMap(int width, int height)
        {
            spriteLib.LoadSpriteLibrary();

            heightData = GenerateTerrain(width, height);
            biomeData = GenerateBiomes(width, height);

            var genRivers = new RiverGenerator(heightData, settings);
            heightData = genRivers.CarveRivers(width, height);          
            
            tmBase = CreateBaseTilemap(heightData);

            rules = new WorldTileRuler(spriteLib, tmBase);
            tmBase = rules.ApplyBaseMapTileRules();

            return tmBase;
        }

        public Tilemap GenerateForestMap(int width, int height)
        {
            var gen = new ForestGenerator(spriteLib, heightData, biomeData, settings);

            forestData = gen.GenerateForestData(width, height);

            tmForest = gen.CreateForestTilemap(forestData);
            tmForest = rules.ApplyForestRules(tmForest);

            return tmForest;
        }

        private Tilemap CreateBaseTilemap(int[,] mapData)
        {
            var tmBase = new Tilemap(mapData.GetLength(0), mapData.GetLength(1));

            for (int i = 0; i < mapData.GetLength(0); i++)
            {
                for (int j = 0; j < mapData.GetLength(1); j++)
                {
                    var tile = new Tile
                    {
                        xIndex = i,
                        yIndex = j,
                        Position = new Vector2(i * settings.TileSize, j * settings.TileSize),
                        Size = new Vector2(settings.TileSize, settings.TileSize),
                        Texture = DetermineTexture(i, j),
                        TileType = DetermineTileType(i, j)
                    };

                    tmBase.Map[i, j] = tile;
                }
            }

            return tmBase;
        }

        public int[,] GenerateTerrain(int width, int height)
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

            return intData;
        }

        public int[,] GenerateBiomes(int width, int height)
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

            return intData;
        }

        

        

        private Texture2D DetermineTexture(int x, int y)
        {
            var biomeIndex = biomeData[x, y];
            var heightIndex = heightData[x, y];

            var biome = (Biome) biomeIndex;

            if (heightIndex > settings.MountainHeightMin)
            {
                return spriteLib.GetSprite("mountain");
            }
            else if (heightIndex > settings.WaterHeightMax)
            {
                switch (biome)
                {
                    case Biome.Desert: 
                        return spriteLib.GetSprite("desert");

                    case Biome.Grasslands:
                        return spriteLib.GetSprite("grasslands");

                    case Biome.Winterlands: 
                        return spriteLib.GetSprite("winterlands");

                    default: 
                        return spriteLib.GetSprite("grasslands");
                }
            }
            else
            {
                return spriteLib.GetSprite("coast-inv");
            }
        }

        private TileType DetermineTileType(int x, int y)
        {
            var heightIndex = heightData[x, y];

            if (heightIndex > settings.MountainHeightMin)
            {
                return TileType.Mountain;
            }
            else if (heightIndex > settings.WaterHeightMax)
            {
                return TileType.Ground;
            }
            else
            {
                return TileType.Water;
            }
        }
    }
}
