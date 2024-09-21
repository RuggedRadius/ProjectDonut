using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using ProjectDonut.Core;
using ProjectDonut.Core.Sprites;
using ProjectDonut.Interfaces;
using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectDonut.ProceduralGeneration.World.Generators
{
    public class ForestData
    {
        public int[,] HeightData { get; set; }
        public int[,] BiomeData { get; set; }
    }

    public class ForestGenerator
    {
        private WorldMapSettings settings;

        private FastNoiseLite _noise;
        private WorldTileRuler tileRuler;

        private Random _random = new Random();

        public ForestGenerator(WorldMapSettings mapSettings)
        {
            settings = mapSettings;

            var random = new Random();
            var worldSeed = random.Next(int.MinValue, int.MaxValue);

            _noise = new FastNoiseLite();
            _noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            _noise.SetSeed(worldSeed);

            tileRuler = new WorldTileRuler();
        }
        
        public void GenerateForestData(WorldChunk chunk)
        {
            chunk.ForestData = new float[chunk.Width, chunk.Height];

            for (int i = 0; i < chunk.Width; i++)
            {
                for (int j = 0; j < chunk.Height; j++)
                {
                    var x = chunk.ChunkCoordX * settings.Width + i;
                    var y = chunk.ChunkCoordY * settings.Height + j;

                    var isSuitable = IsCellAppropriateForForest(chunk, i, j);
                    var heightValue = 0;

                    if (isSuitable)
                    {
                        var noiseValue = _noise.GetNoise(x, y);
                        if (noiseValue > 0.0f)
                        {
                            heightValue = 1;
                        }
                    }

                    chunk.ForestData[i, j] = heightValue;
                }
            }

            //Debugger.PrintDataMap(chunk.ForestData, @$"C:\Forest_X-{chunk.ChunkCoordX}_Y-{chunk.ChunkCoordY}.txt");
        }

        private bool IsCellAppropriateForForest(WorldChunk chunk, int x, int y)
        {
            if (chunk.HeightData[x, y] < settings.WaterHeightMax)
            {
                return false;
            }

            if (chunk.HeightData[x, y] > settings.GroundHeightMax)
            {
                return false;
            }

            var biome = (Biome)chunk.BiomeData[x, y];
            switch (biome)
            {
                //case Biome.Winterlands:
                //case Biome.Grasslands:
                //    return true;

                default:
                    return true;
            }
        }

        public Tilemap CreateTileMap(WorldChunk chunk)
        {
            var tmForest = new Tilemap(chunk.Width, chunk.Height);

            for (int i = 0; i < chunk.Width; i++)
            {
                for (int j = 0; j < chunk.Height; j++)
                {
                    if (chunk.ForestData[i, j] == 0)
                    {
                        continue;
                    }

                    var positionVariantX = _random.Next(-settings.TileSize / 4, settings.TileSize / 4);
                    var positionVariantY = _random.Next(-settings.TileSize / 4, settings.TileSize / 4);

                    var tile = new Tile(false)
                    {
                        ChunkX = chunk.ChunkCoordX,
                        ChunkY = chunk.ChunkCoordY,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * settings.TileSize, j * settings.TileSize) + new Vector2(positionVariantX, positionVariantY),
                        Size = new Vector2(settings.TileSize, settings.TileSize),
                        Texture = SpriteLib.Mineables.Sprites["tree-02"][0],
                        TileType = TileType.World,
                        WorldTileType = WorldTileType.Forest,
                        Biome = (Biome)chunk.BiomeData[i, j]
                    };

                    tmForest.Map[i, j] = tile;
                }
            }

            //tmForest = tileRuler.ApplyForestRules(tmForest);

            return tmForest;
        }

        private Texture2D DetermineTexture(int x, int y, int[,] biomeData)
        {
            var tileType = (Biome)biomeData[x, y];

            switch (tileType)
            {
                case Biome.Grasslands:
                    return SpriteLib.GetSprite("forest-C");

                case Biome.Desert:
                    return SpriteLib.GetSprite("forest-C"); // Change this later?

                case Biome.Winterlands:
                    return SpriteLib.GetSprite("forest-frost-C");

                default:
                    return SpriteLib.GetSprite("forest-C");
            }
        }
    }
}
