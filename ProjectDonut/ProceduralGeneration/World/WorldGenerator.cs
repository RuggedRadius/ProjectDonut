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
        private int[,] mapData;

        private Texture2D spriteSheet;
        private Dictionary<string, Texture2D> spriteLib;
        private Tilemap tilemap;

        private ContentManager content;
        private GraphicsDevice graphicsDevice;

        public WorldGenerator(ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.content = content;
            this.graphicsDevice = graphicsDevice;
        }

        public Tilemap Generate(int width, int height)
        {
            LoadSpriteLibrary();

            GenerateTerrain(width, height);            
            CreateTilemap(mapData);
            ApplyTileRules();

            return tilemap;
        }

        private void ApplyTileRules()
        {
            int counter = 0;
            foreach (var tile in tilemap.Map)
            {
                int x = tile.xIndex;
                int y = tile.yIndex;

                try
                {
                    if (x == 0 || y == 0 || x == tilemap.Map.GetLength(0) - 1 || y == tilemap.Map.GetLength(1) - 1)
                    {
                        //tile.Texture = spriteLib["coast-inv"];
                        counter++;
                        continue;
                    }

                    if (isNorthWestCoast(x, y))
                    {
                        tile.Texture = spriteLib["coast-NW"];
                    }

                    if (isNorthEastCoast(x, y))
                    {
                        tile.Texture = spriteLib["coast-NE"];
                    }

                    if (isSouthEastCoast(x, y))
                    {
                        tile.Texture = spriteLib["coast-SE"];
                    }

                    if (isSouthWestCoast(x, y))
                    {
                        tile.Texture = spriteLib["coast-SW"];
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                counter++;
            }
        }

        private bool isNorthWestCoast(int x, int y)
        {
            var neighbours = GetNeighbours(x, y);
            
            if (neighbours.Where(x => x == null).Any())
            {
                return false;
            }

            var northWestTile = neighbours[0].TileType;
            var westTile = neighbours[1].TileType;
            var southWestTile = neighbours[2].TileType;
            var northTile = neighbours[3].TileType;
            var currentTile = neighbours[4].TileType;
            var southTile = neighbours[5].TileType;
            var northEastTile = neighbours[6].TileType;
            var eastTile = neighbours[7].TileType;
            var southEastTile = neighbours[8].TileType;

            if (northWestTile == TileType.Water &&
                northTile == TileType.Water && 
                westTile == TileType.Water &&
                southWestTile == TileType.Coast &&
                northEastTile == TileType.Coast &&
                eastTile == TileType.Coast &&
                southEastTile == TileType.Coast &&
                southTile == TileType.Coast)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isSouthWestCoast(int x, int y)
        {
            var neighbours = GetNeighbours(x, y);

            if (neighbours.Where(x => x == null).Any())
            {
                return false;
            }

            var northWestTile = neighbours[0].TileType;
            var westTile = neighbours[1].TileType;
            var southWestTile = neighbours[2].TileType;
            var northTile = neighbours[3].TileType;
            var currentTile = neighbours[4].TileType;
            var southTile = neighbours[5].TileType;
            var northEastTile = neighbours[6].TileType;
            var eastTile = neighbours[7].TileType;
            var southEastTile = neighbours[8].TileType;

            if (northWestTile == TileType.Coast &&
                northTile == TileType.Coast &&
                westTile == TileType.Water &&
                southWestTile == TileType.Water &&
                northEastTile == TileType.Coast &&
                eastTile == TileType.Coast &&
                southEastTile == TileType.Coast &&
                southTile == TileType.Water)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isNorthEastCoast(int x, int y)
        {
            var neighbours = GetNeighbours(x, y);

            if (neighbours.Where(x => x == null).Any())
            {
                return false;
            }

            var northWestTile = neighbours[0].TileType;
            var westTile = neighbours[1].TileType;
            var southWestTile = neighbours[2].TileType;
            var northTile = neighbours[3].TileType;
            var currentTile = neighbours[4].TileType;
            var southTile = neighbours[5].TileType;
            var northEastTile = neighbours[6].TileType;
            var eastTile = neighbours[7].TileType;
            var southEastTile = neighbours[8].TileType;

            if (northWestTile == TileType.Coast &&
                northTile == TileType.Water &&
                westTile == TileType.Coast &&
                southWestTile == TileType.Coast &&
                northEastTile == TileType.Water &&
                eastTile == TileType.Water &&
                southEastTile == TileType.Coast &&
                southTile == TileType.Coast)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isSouthEastCoast(int x, int y)
        {
            var neighbours = GetNeighbours(x, y);

            if (neighbours.Where(x => x == null).Any())
            {
                return false;
            }

            var northWestTile = neighbours[0].TileType;
            var westTile = neighbours[1].TileType;
            var southWestTile = neighbours[2].TileType;
            var northTile = neighbours[3].TileType;
            var currentTile = neighbours[4].TileType;
            var southTile = neighbours[5].TileType;
            var northEastTile = neighbours[6].TileType;
            var eastTile = neighbours[7].TileType;
            var southEastTile = neighbours[8].TileType;

            if (northWestTile == TileType.Coast &&
                northTile == TileType.Coast &&
                westTile == TileType.Coast &&
                southWestTile == TileType.Coast &&
                northEastTile == TileType.Coast &&
                eastTile == TileType.Water &&
                southEastTile == TileType.Water &&
                southTile == TileType.Water)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<Tile> GetNeighbours(int x, int y)
        {
            var neighbours = new List<Tile>();

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    neighbours.Add(tilemap.Map[x + i, y + j]);
                }
            }

            return neighbours;
        }

        private void LoadSpriteLibrary()
        {
            spriteSheet = content.Load<Texture2D>("Sprites/Map/World/WorldTerrain01");

            spriteLib = new Dictionary<string, Texture2D>();

            // Coast
            spriteLib.Add("coast-NW", ExtractSprite(0, 0));
            spriteLib.Add("coast-N", ExtractSprite(1, 0));
            spriteLib.Add("coast-NE", ExtractSprite(2, 0));
            spriteLib.Add("coast-W", ExtractSprite(0, 1));
            spriteLib.Add("coast", ExtractSprite(1, 1));
            spriteLib.Add("coast-E", ExtractSprite(2, 1));
            spriteLib.Add("coast-SW", ExtractSprite(0, 2));
            spriteLib.Add("coast-S", ExtractSprite(1, 2));
            spriteLib.Add("coast-SE", ExtractSprite(2, 2));

            // Inverted coast
            spriteLib.Add("coast-inv-NW", ExtractSprite(3, 0));
            spriteLib.Add("coast-inv-N", ExtractSprite(4, 0));
            spriteLib.Add("coast-inv-NE", ExtractSprite(5, 0));
            spriteLib.Add("coast-inv-W", ExtractSprite(3, 1));
            spriteLib.Add("coast-inv", ExtractSprite(4, 1));
            spriteLib.Add("coast-inv-E", ExtractSprite(5, 1));
            spriteLib.Add("coast-inv-SW", ExtractSprite(3, 2));
            spriteLib.Add("coast-inv-S", ExtractSprite(4, 2));
            spriteLib.Add("coast-inv-SE", ExtractSprite(5, 2));

            // Grass
            spriteLib.Add("grass-NW", ExtractSprite(6, 0));
            spriteLib.Add("grass-N", ExtractSprite(7, 0));
            spriteLib.Add("grass-NE", ExtractSprite(8, 0));
            spriteLib.Add("grass-W", ExtractSprite(6, 1));
            spriteLib.Add("grass", ExtractSprite(7, 1));
            spriteLib.Add("grass-E", ExtractSprite(8, 1));
            spriteLib.Add("grass-SW", ExtractSprite(6, 2));
            spriteLib.Add("grass-S", ExtractSprite(7, 2));
            spriteLib.Add("grass-SE", ExtractSprite(8, 2));

            // Inverted grass
            spriteLib.Add("grass-inv-NW", ExtractSprite(9, 0));
            spriteLib.Add("grass-inv-N", ExtractSprite(10, 0));
            spriteLib.Add("grass-inv-NE", ExtractSprite(11, 0));
            spriteLib.Add("grass-inv-W", ExtractSprite(9, 1));
            spriteLib.Add("grass-inv", ExtractSprite(10, 1));
            spriteLib.Add("grass-inv-E", ExtractSprite(11, 1));
            spriteLib.Add("grass-inv-SW", ExtractSprite(9, 2));
            spriteLib.Add("grass-inv-S", ExtractSprite(10, 2));
            spriteLib.Add("grass-inv-SE", ExtractSprite(11, 2));

            // Mountain
            spriteLib.Add("mountain-NW", ExtractSprite(12, 0));
            spriteLib.Add("mountain-N", ExtractSprite(13, 0));
            spriteLib.Add("mountain-NE", ExtractSprite(14, 0));
            spriteLib.Add("mountain-W", ExtractSprite(12, 1));
            spriteLib.Add("mountain", ExtractSprite(13, 1));
            spriteLib.Add("mountain-E", ExtractSprite(14, 1));
            spriteLib.Add("mountain-SW", ExtractSprite(12, 2));
            spriteLib.Add("mountain-S", ExtractSprite(13, 2));
            spriteLib.Add("mountain-SE", ExtractSprite(14, 2));

            // Inverted mountain
            spriteLib.Add("mountain-inv-NW", ExtractSprite(15, 0));
            spriteLib.Add("mountain-inv-N", ExtractSprite(16, 0));
            spriteLib.Add("mountain-inv-NE", ExtractSprite(17, 0));
            spriteLib.Add("mountain-inv-W", ExtractSprite(15, 1));
            spriteLib.Add("mountain-inv", ExtractSprite(16, 1));
            spriteLib.Add("mountain-inv-E", ExtractSprite(17, 1));
            spriteLib.Add("mountain-inv-SW", ExtractSprite(15, 2));
            spriteLib.Add("mountain-inv-S", ExtractSprite(16, 2));
            spriteLib.Add("mountain-inv-SE", ExtractSprite(17, 2));
        }

        private Texture2D ExtractSprite(int x, int y)
        {
            var width = 32;
            var height = 32;

            x *= width;
            y *= height;

            Rectangle sourceRectangle = new Rectangle(x, y, width, height);

            // Extract the pixel data from the spritesheet
            Color[] data = new Color[width * height];
            spriteSheet.GetData(0, sourceRectangle, data, 0, data.Length);

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
                    var determinations = DetermineTexture(mapData[i, j]);

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

            mapData = intData;
        }

        private (Texture2D, TileType) DetermineTexture(int dataValue)
        {
            if (dataValue >= 8)
            {
                return (spriteLib["mountain"], TileType.Mountain);
            }
            else if (dataValue >= 6)
            {
                return (spriteLib["grass"], TileType.Grass);
            }
            else if (dataValue >= 3)
            {
                return (spriteLib["coast"], TileType.Coast);
            }
            else
            {
                return (spriteLib["coast-inv"], TileType.Water);
            }
        }
    }
}
