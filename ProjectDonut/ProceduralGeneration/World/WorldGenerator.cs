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
            CarveRivers(width, height);
            GenerateForests(width, height);
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

        public void GenerateForests(int width, int height)
        {
            int forestCount = 250;
            int minWalk = 250;
            int maxWalk = 1000;
            int walkRadius = 5;

            var grasslandCoords = new List<(int, int)>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (biomeData[x, y] == (int)Biome.Grasslands)
                    {
                        grasslandCoords.Add((x, y));
                    }
                }
            }
            
            forestData = new int[width, height];
            var randy = new Random();
            
            for (int x = 0; x < forestCount; x++)
            {
                var randomIndex = randy.Next(0, grasslandCoords.Count);
                var coords = grasslandCoords[randomIndex];

                forestData[coords.Item1, coords.Item2] = 1;
                var walkLength = randy.Next(minWalk, maxWalk);

                for (int y = 0; y < walkLength; y++)
                {
                    var direction = randy.Next(0, 4);
                    switch (direction)
                    {
                        case 0:
                            if (coords.Item1 + 1 < width)
                            {
                                coords.Item1 += 1;
                            }
                            break;
                        case 1:
                            if (coords.Item1 - 1 >= 0)
                            {
                                coords.Item1 -= 1;
                            }
                            break;
                        case 2:
                            if (coords.Item2 + 1 < height)
                            {
                                coords.Item2 += 1;
                            }
                            break;
                        case 3:
                            if (coords.Item2 - 1 >= 0)
                            {
                                coords.Item2 -= 1;
                            }
                            break;
                    }

                    for (int i = -walkRadius; i < walkRadius; i++)
                    {
                        for (int j = -walkRadius; j < walkRadius; j++)
                        {
                            var xCoord = coords.Item1 + i;
                            var yCoord = coords.Item2 + j;

                            if (xCoord < 0 || xCoord >= width || yCoord < 0 || yCoord >= height)
                            {
                                continue;
                            }

                            if (biomeData[xCoord, yCoord] == (int)Biome.Grasslands)
                            {
                                forestData[xCoord, yCoord] = 1;
                            }
                        }
                    }
                }

                grasslandCoords.Remove(coords);
            }

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

                if (heightData[startX, startY] > waterHeight)
                {
                    if (heightData[startX, startY] >= mountainHeight)
                    {
                        return;
                    }

                    heightData[startX, startY] = waterHeight;
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
                    if (heightData[x,y] <= waterHeight)
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

            var isCoastNorth = heightData[x, y - 1] > waterHeight;
            var isCoastEast = heightData[x + 1, y] > waterHeight;
            var isCoastSouth = heightData[x - 1, y] > waterHeight;
            var isCoastWest = heightData[x, y + 1] > waterHeight;

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

        private int mountainHeight = 8;
        private int waterHeight = 2;

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
                    case Biome.Desert: 
                        return (spriteLib.GetSprite("desert"), TileType.Grass);

                    case Biome.Grasslands:
                        if (forestData[x, y] == 1)
                        {
                            return (spriteLib.GetSprite("forest-C"), TileType.Grass);
                        }
                        else
                        {
                            return (spriteLib.GetSprite("grasslands"), TileType.Grass);
                        }
                        

                    case Biome.Winterlands: 
                        return (spriteLib.GetSprite("winterlands"), TileType.Grass);

                    default: 
                        return (spriteLib.GetSprite("grasslands"), TileType.Grass);
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
