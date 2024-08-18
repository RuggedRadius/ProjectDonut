﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.GameObjects;

namespace ProjectDonut.ProceduralGeneration.World
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
        private int TownCountMax = 3;

        private SpriteLibrary spriteLib;
        private WorldMapSettings settings;
        private SpriteBatch _spriteBatch;
        private Random random = new Random();

        public StructureGenerator(SpriteLibrary spriteLib, WorldMapSettings settings, SpriteBatch spriteBatch) 
        {
            this.spriteLib = spriteLib;
            this.settings = settings;
            _spriteBatch = spriteBatch;
        }

        public void GenerateStructureData(WorldChunk chunk)
        {
            chunk.StructureData = new int[chunk.Width, chunk.Height];

            var viableLocations = GetViableStructureLocations(chunk);

            var castleCount = random.Next(CastleCountMin, CastleCountMax);
            var townCount = random.Next(TownCountMin, TownCountMax);

            for (int i = 0; i < castleCount; i++)
            {
                var location = viableLocations[random.Next(0, viableLocations.Count)];
                viableLocations.Remove(location);

                chunk.StructureData[location.Item1, location.Item2] = 1;
            }

            for (int i = 0; i < townCount; i++)
            {
                var location = viableLocations[random.Next(0, viableLocations.Count)];
                viableLocations.Remove(location);

                chunk.StructureData[location.Item1, location.Item2] = 2;
            }            
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
            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
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

        public Tilemap GenerateChunkStructuresTileMap(WorldChunk chunk)
        {
            var directions = new List<string>{ "NW", "N", "NE", "W", "C", "E", "SW", "S", "SE" };
            var tmStructures = new Tilemap(chunk.Width, chunk.Height);

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

            for (int j = -1; j <= 1; j++)
            {
                for (int i = -1; i <= 1; i++)
                {
                    var tile = new Tile(_spriteBatch, true)
                    {
                        ChunkX = chunk.ChunkCoordX,
                        ChunkY = chunk.ChunkCoordY,
                        xIndex = i + x,
                        yIndex = j + y,
                        LocalPosition = new Vector2((i + x) * settings.TileSize, (j + y) * settings.TileSize),
                        Size = new Vector2(settings.TileSize, settings.TileSize),
                        Texture = DetermineTexture(Structure.Town, directions[counter]),
                        TileType = TileType.Forest,
                        Biome = (Biome)chunk.BiomeData[(i + x), (j + y)],
                        Frames = GetFrames(Structure.Town, directions[counter], 4)
                    };

                    map.Map[i + x, j + y] = tile;
                    counter++;
                }
            }
        }

        private Texture2D DetermineTexture(Structure structure, string direction)
        {
            switch (structure)
            {
                case Structure.Castle:
                    return spriteLib.GetSprite("castle");

                case Structure.Town:
                    return spriteLib.GetSprite($"town-01-{direction}");

                default:
                    return spriteLib.GetSprite("grasslands");
            }
        }

        private List<Texture2D> GetFrames(Structure structure, string direction, int frameCount)
        {
            var results = new List<Texture2D>();

            switch (structure)
            {
                case Structure.Castle:
                    results.Add(spriteLib.GetSprite("castle"));
                    break;

                case Structure.Town:
                    for (int i = 0; i < frameCount; i++)
                    {
                        var key = $"town-{i + 1:D2}-{direction}";
                        results.Add(spriteLib.GetSprite(key));
                    }
                    break;
            }

            return results;
        }
    }
}
