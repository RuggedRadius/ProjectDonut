﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.ProceduralGeneration.World.Structures;
using ProjectDonut.Tools;

namespace ProjectDonut.ProceduralGeneration.World.Generators
{
    public enum Structure
    {
        None,
        Castle,
        Town
    }

    public class StructureGenerator
    {
        private int CastleCountMin = 0;
        private int CastleCountMax = 2;

        private int TownCountMin = 0;
        private int TownCountMax = 2;

        private WorldMapSettings settings;
        private Random random = new Random();

        public StructureGenerator(WorldMapSettings settings)
        {
            this.settings = settings;
        }

        public void GenerateStructureData(WorldChunk chunk)
        {
            chunk.StructureData = new int[chunk.Width, chunk.Height];

            var viableLocations = GetViableStructureLocations(chunk);

            var townChance = random.Next(1, 101);
            var castleChance = random.Next(1, 101);

            var castleCount = castleChance > 0 ? 0 : 0;
            var townCount = townChance > 0 ? 1 : 0;

            for (int i = 0; i < castleCount; i++)
            {
                if (viableLocations.Count == 0)
                {
                    break;
                }

                var location = viableLocations[random.Next(0, viableLocations.Count)];
                viableLocations.Remove(location);

                chunk.StructureData[location.Item1, location.Item2] = 1;
            }

            for (int i = 0; i < townCount; i++)
            {
                if (viableLocations.Count == 0)
                {
                    break;
                }

                var location = viableLocations[random.Next(0, viableLocations.Count)];
                viableLocations.Remove(location);

                chunk.StructureData[location.Item1, location.Item2] = 2;
            }

            //chunk.Structures = GetStructuresData(chunk);
        }

        private List<(int, int)> GetViableStructureLocations(WorldChunk chunk)
        {
            var results = new List<(int, int)>();
            var exclusions = new List<(int, int)>();

            for (int i = 0; i < chunk.Width; i++)
            {
                for (int j = 0; j < chunk.Height; j++)
                {
                    if (IsCellSuitable(chunk, i, j) == false)
                    {
                        continue;
                    }

                    if (exclusions.Contains((i, j)))
                    {
                        continue;
                    }

                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (x != 0 || y != 0)
                            {
                                exclusions.Add((i + x, j + y));
                            }
                        }
                    }

                    results.Add((i, j));
                }
            }

            return results;
        }

        private bool IsCellSuitable(WorldChunk chunk, int i, int j)
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (i + x < 0 || j + y < 0 || i + x >= chunk.Width || j + y >= chunk.Height)
                    {
                        return false; // Out of bounds
                    }

                    if (chunk.StructureData[i + x, j + y] != 0)
                    {
                        return false; // Structure already exists there
                    }

                    if (chunk.HeightData[i + x, j + y] < settings.GroundHeightMin ||
                        chunk.HeightData[i + x, j + y] > settings.GroundHeightMax)
                    {
                        return false;  // Not suitable ground height
                    }

                    if (chunk.ForestData[i + x, j + y] != 0)
                    {
                        return false; // Intersects with forest
                    }
                }
            }

            return true;
        }

        public Tilemap CreateTileMap(WorldChunk chunk)
        {
            var directions = new List<string> { "NW", "N", "NE", "W", "C", "E", "SW", "S", "SE" };
            var tmStructures = new Tilemap(chunk.Width, chunk.Height);

            chunk.Structures = new List<StructureData>();

            for (int i = 0; i < chunk.Width; i++)
            {
                for (int j = 0; j < chunk.Height; j++)
                {
                    if (chunk.StructureData[i, j] == 0)
                    {
                        continue;
                    }

                    if (i == 0 || j == 0 || i == chunk.Width - 1 || j == chunk.Height - 1)
                    {
                        continue;
                    }

                    PlaceStructure(chunk, tmStructures, i, j, chunk.StructureData[i, j]);
                }
            }

            return tmStructures;
        }

        private void PlaceStructure(WorldChunk chunk, Tilemap map, int x, int y, int structureValue)
        {
            var directions = new List<string> { "NW", "N", "NE", "W", "C", "E", "SW", "S", "SE" };
            int counter = 0;

            var structure = (Structure)structureValue;

            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    var tile = new Tile(true)
                    {
                        ChunkX = chunk.ChunkCoordX,
                        ChunkY = chunk.ChunkCoordY,
                        xIndex = i + x,
                        yIndex = j + y,
                        //Position = new Vector2((i + x) * settings.TileSize, (j + y) * settings.TileSize) + chunk.Position,
                        LocalPosition = new Vector2((i + x) * settings.TileSize, (j + y) * settings.TileSize),
                        Size = new Vector2(settings.TileSize, settings.TileSize),
                        Texture = Global.SpriteLibrary.GetSprite($"castle-01-{i}-{j}"),
                        WorldTileType = WorldTileType.Forest,
                        Biome = (Biome)chunk.BiomeData[i + x, j + y],
                        Frames = new List<Texture2D>()
                    };

                    map.Map[i + x, j + y] = tile;
                    counter++;
                }
            }

            var entryRectX = x * Global.TileSize + (4 * Global.TileSize);
            var entryRectY = y * Global.TileSize + (8 * Global.TileSize);

            var scrollBoundsX = x * Global.TileSize - (1 * Global.TileSize);
            var scrollBoundsY = y * Global.TileSize - (1 * Global.TileSize);

            var structureData = new StructureData
            {
                ScrollBounds = new Rectangle(scrollBoundsX, scrollBoundsY, Global.TileSize * 4, Global.TileSize * 4),
                Bounds = new Rectangle(entryRectX, entryRectY, Global.TileSize, Global.TileSize),
                Name = NameGenerator.GenerateRandomName(random.Next(2, 5)),
                Instance = new InstanceScene(SceneType.Instance)
            };

            structureData.Instance.Initialize();
            structureData.Instance.LoadContent();

            chunk.Structures.Add(structureData);
        }

        private Texture2D DetermineTexture(Structure structure, string direction)
        {
            switch (structure)
            {
                case Structure.Castle:
                    return Global.SpriteLibrary.GetSprite($"castle-01-{direction}");

                case Structure.Town:
                    return Global.SpriteLibrary.GetSprite($"town-01-{direction}");

                default:
                    return Global.SpriteLibrary.GetSprite("grasslands");
            }
        }

        private List<Texture2D> GetFrames(Structure structure, string direction, int frameCount)
        {
            var results = new List<Texture2D>();

            switch (structure)
            {
                case Structure.Castle:
                    for (int i = 0; i < frameCount; i++)
                    {
                        var key = $"castle-{i + 1:D2}-{direction}";
                        results.Add(Global.SpriteLibrary.GetSprite(key));
                    }
                    break;

                case Structure.Town:
                    for (int i = 0; i < frameCount; i++)
                    {
                        var key = $"town-{i + 1:D2}-{direction}";
                        results.Add(Global.SpriteLibrary.GetSprite(key));
                    }
                    break;
            }

            return results;
        }

        public List<StructureData> GetStructuresData(WorldChunk chunk)
        {
            var random = new Random();
            var structures = new List<StructureData>();

            var width = chunk.Width;
            var height = chunk.Height;
            var data = chunk.StructureData;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (data[x, y] != 0)
                    {
                        var structureBounds = GetStructureBounds(chunk, x, y);

                        var scrollBoundsX = structureBounds.Left - (1 * Global.TileSize);
                        var scrollBoundsY = structureBounds.Top - (1 * Global.TileSize);

                        structures.Add(new StructureData
                        {
                            ScrollBounds = new Rectangle(scrollBoundsX, scrollBoundsY, Global.TileSize * 4, Global.TileSize * 4),
                            Bounds = structureBounds,
                            Name = NameGenerator.GenerateRandomName(random.Next(2,5))
                        });
                    }
                }
            }

            return structures;
        }

        private Rectangle GetStructureBounds(WorldChunk chunk, int x, int y)
        {
            var posX = x;
            var posY = y;

            int structureWidth = 0;
            int structureHeight = 0;

            var widthFound = false;
            while (!widthFound)
            {
                if (chunk.StructureData[posX, posY] == 0)
                {
                    widthFound = true;
                }
                else
                {
                    structureWidth++;
                    posX++;
                }
            }

            var heightFound = false;
            while (!heightFound)
            {
                if (chunk.StructureData[posX, posY] == 0)
                {
                    heightFound = true;
                }
                else
                {
                    structureHeight++;
                    posY++;
                }
            }

            var chunkPosX = settings.TileSize * x;
            var chunkPosY = settings.TileSize * y;

            return new Rectangle(
                chunkPosX, 
                chunkPosY, 
                structureWidth * settings.TileSize, 
                structureHeight * settings.TileSize);
        }
    }
}
