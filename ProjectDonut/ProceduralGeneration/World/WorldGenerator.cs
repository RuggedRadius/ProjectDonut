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
        private int[,] heightData;
        private int[,] biomeData;
        private int[,] forestData;

        private Tilemap tmBase;
        private Tilemap tmForest;


        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        private WorldTileRuler rules;
        private SpriteLibrary spriteLib;

        // Sub-generators
        private ForestGenerator genForest;

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

            GenerateTerrain(width, height);
            GenerateBiomes(width, height);
            CarveRivers(width, height);            
            tmBase = CreateBaseTilemap(heightData);

            genForest = new ForestGenerator(spriteLib, heightData, biomeData, settings);

            rules = new WorldTileRuler(spriteLib, tmBase);
            tmBase = rules.ApplyBaseMapTileRules();

            return tmBase;
        }

        public Tilemap GenerateForestMap(int width, int height)
        {
            var gen = new ForestGenerator(spriteLib, heightData, biomeData, settings);
            var tilemap = gen.CreateForestTilemap(width, height);
            //tilemap = rules.ApplyForestRules(tilemap);
            return tilemap;
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

        

        public void CarveRivers(int width, int height)
        {
            int riverCount = 50;
            int minLength = 50;
            int maxLength = 500;

            var randy = new Random();

            // Find coast
            var coastCoords = FindCoastCoords(width, height);

            // Random walk + splinter
            for (int i = 0; i < riverCount; i++)
            {
                var startindex = randy.Next(0, coastCoords.Count);
                var start = coastCoords[startindex];
                var length = randy.Next(minLength, maxLength);

                CarveRiver(width, height, length, start.Item3, start.Item1, start.Item2);
            }
        }

        private void CarveRiver(int width, int height, int length, int startDirection, int startX, int startY)
        {
            double forkChance = 0.0025f;
            int minForkLength = 5;

            var randy = new Random();

            var bannedDirection = 0;
            if (startDirection == 0)
                bannedDirection = 2;
            if (startDirection == 1)
                bannedDirection = 3;
            if (startDirection == 2)
                bannedDirection = 0;
            if (startDirection == 3)
                bannedDirection = 1;

            for (int j = 0; j < length; j++)
            {
                if (randy.NextDouble() <= forkChance && (length - j) > minForkLength)
                {
                    var forkLength = randy.Next(minForkLength, length - j);
                    var forkDirection = randy.Next(0, 4);
                    int forkCounter = 0;
                    var suitableForkDirectionFound = false;
                    while (suitableForkDirectionFound == false)
                    {
                        forkDirection = randy.Next(0, 4);

                        if (forkDirection != bannedDirection)
                        {
                            suitableForkDirectionFound = true;
                        }

                        forkCounter++;

                        if (forkCounter > 1000)
                        {
                            continue;
                        }
                    }

                    CarveRiver(width, height, forkLength, forkDirection, startX, startY);
                }

                var direction = randy.Next(0, 4);
                int counter = 0;
                var suitableDirectionFound = false;
                while (suitableDirectionFound == false)
                {
                    direction = randy.Next(0, 4);

                    if (direction != bannedDirection)
                    {
                        suitableDirectionFound = true;
                    }

                    counter++;

                    if (counter > 1000)
                    {
                        continue;
                    }
                }

                switch (direction)
                {
                    case 0:
                        if (startX + 1 < width)
                        {
                            startX += 1;
                        }
                        break;
                    case 1:
                        if (startX - 1 >= 0)
                        {
                            startX -= 1;
                        }
                        break;
                    case 2:
                        if (startY + 1 < height)
                        {
                            startY += 1;
                        }
                        break;
                    case 3:
                        if (startY - 1 >= 0)
                        {
                            startY -= 1;
                        }
                        break;
                }

                if (heightData[startX, startY] > settings.WaterHeightMax)
                {
                    if (heightData[startX, startY] >= settings.MountainHeightMin)
                    {
                        return;
                    }

                    heightData[startX, startY] = settings.WaterHeightMax;
                }
            }
        }

        private List<(int, int, int)> FindCoastCoords(int width, int height)
        {
            var coords = new List<(int, int, int)>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (heightData[x,y] <= settings.WaterHeightMax)
                    {
                        var direction = IsCoastCoord(x, y);

                        if (direction >= 0)
                        {
                            coords.Add((x, y, direction));
                        }
                    }
                }
            }

            return coords;
        }

        private int IsCoastCoord(int x, int y)
        {
            if (x == 0 || x == heightData.GetLength(0) - 1 || y == 0 || y == heightData.GetLength(1) - 1)
                return -1;

            var isCoastNorth = heightData[x, y - 1] > settings.WaterHeightMax;
            var isCoastEast = heightData[x + 1, y] > settings.WaterHeightMax;
            var isCoastSouth = heightData[x - 1, y] > settings.WaterHeightMax;
            var isCoastWest = heightData[x, y + 1] > settings.WaterHeightMax;

            if (isCoastNorth)
                return 0;
            if (isCoastEast)
                return 1;
            if (isCoastSouth)
                return 2;
            if (isCoastWest)
                return 3;
            else
                return -1;
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
