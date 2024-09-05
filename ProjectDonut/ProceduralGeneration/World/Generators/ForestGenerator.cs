using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
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

            tileRuler = new WorldTileRuler(Global.SpriteLibrary);
        }

        public List<ISceneObject> GenerateWinterTrees(WorldChunk chunk)
        {
            var trees = new List<ISceneObject>();

            for (int i = 0; i < chunk.Width; i++)
            {
                for (int j = 0; j < chunk.Height; j++)
                {
                    if (chunk.ForestData[i, j] == 0)
                    {
                        continue;
                    }

                    if (chunk.BiomeData[i, j] != (int)Biome.Winterlands)
                    {
                        continue;
                    }

                    var treeCount = _random.Next(2, 4);
                    var halfTileSize = Global.TileSize / 2;

                    for (int k = 0; k < treeCount; k++)
                    {
                        var chunkPosX = i * Global.TileSize;
                        var chunkPosY = j * Global.TileSize;

                        var randomiseX = _random.Next(-halfTileSize, halfTileSize);
                        var randomiseY = _random.Next(-halfTileSize, halfTileSize);

                        var globaliseX = chunk.ChunkCoordX * Global.TileSize * Global.ChunkSize;
                        var globaliseY = chunk.ChunkCoordY * Global.TileSize * Global.ChunkSize;

                        var worldXPos = chunkPosX + randomiseX + globaliseX;
                        var worldYPos = chunkPosY + randomiseY + globaliseY;

                        var texture = Global.SpriteLibrary.WorldMapSprites["tree-02-winter"][0];

                        var tree = new SceneObjectStatic
                        {
                            Position = new Vector2(worldXPos, worldYPos),
                            Texture = texture,
                            ZIndex = (int)worldYPos + texture.Height - 16 // TODO: MAGIC NUMBER HERE, 1/4 THE SIZE OF PLAYER SPRITE
                        };

                        trees.Add(tree);
                    }
                }
            }

            return trees;
        }

        public List<ISceneObject> GenerateTrees(WorldChunk chunk)
        {
            var trees = new List<ISceneObject>();

            for (int i = 0; i < chunk.Width; i++)
            {
                for (int j = 0; j < chunk.Height; j++)
                {
                    if (chunk.ForestData[i, j] == 0)
                    {
                        continue;
                    }

                    if (chunk.BiomeData[i, j] != (int)Biome.Grasslands)
                    {
                        continue;
                    }

                    var treeCount = _random.Next(2, 4);
                    var halfTileSize = Global.TileSize / 2;

                    for (int k = 0; k < treeCount; k++)
                    {
                        var chunkPosX = i * Global.TileSize;
                        var chunkPosY = j * Global.TileSize;

                        var randomiseX = _random.Next(-halfTileSize, halfTileSize);
                        var randomiseY = _random.Next(-halfTileSize, halfTileSize);

                        var globaliseX = chunk.ChunkCoordX * Global.TileSize * Global.ChunkSize;
                        var globaliseY = chunk.ChunkCoordY * Global.TileSize * Global.ChunkSize;

                        var worldXPos = chunkPosX + randomiseX + globaliseX;
                        var worldYPos = chunkPosY + randomiseY + globaliseY;

                        var texture = Global.SpriteLibrary.WorldMapSprites["tree-02"][0];

                        var tree = new SceneObjectStatic
                        {
                            Position = new Vector2(worldXPos, worldYPos),
                            Texture = texture,
                            ZIndex = (int)worldYPos + texture.Height - 16 // TODO: MAGIC NUMBER HERE, 1/4 THE SIZE OF PLAYER SPRITE
                        };

                        trees.Add(tree);
                    }
                }
            }

            return trees;
        }


        /// <summary>
        /// ********************************************************************************************************************
        /// 
        /// TODO: 
        ///     - Move this and other similar methods to a separate class
        ///     - When placing loose trees, check the randomised location is not water, or structure, or anything else
        ///     
        /// ********************************************************************************************************************
        /// </summary>
        /// <param name="chunk"></param>
        /// <returns></returns>
        public List<ISceneObject> GenerateLooseTrees(WorldChunk chunk)
        {
            var trees = new List<ISceneObject>();

            for (int i = 0; i < chunk.Width; i++)
            {
                for (int j = 0; j < chunk.Height; j++)
                {
                    if (chunk.ForestData[i, j] != 0)
                    {
                        continue;
                    }

                    if (chunk.HeightData[i, j] < settings.GroundHeightMin ||
                        chunk.HeightData[i, j] > settings.GroundHeightMax)
                    {
                        continue;
                    }

                    if (chunk.BiomeData[i, j] != (int)Biome.Grasslands)
                    {
                        continue;
                    }

                    if (_random.NextDouble() < 0.995)
                    {
                        continue;
                    }


                    var halfTileSize = Global.TileSize / 2;

                    var chunkPosX = i * Global.TileSize;
                    var chunkPosY = j * Global.TileSize;

                    var randomiseX = _random.Next(-halfTileSize, halfTileSize);
                    var randomiseY = _random.Next(-halfTileSize, halfTileSize);

                    var globaliseX = chunk.ChunkCoordX * Global.TileSize * Global.ChunkSize;
                    var globaliseY = chunk.ChunkCoordY * Global.TileSize * Global.ChunkSize;

                    var worldXPos = chunkPosX + randomiseX + globaliseX;
                    var worldYPos = chunkPosY + randomiseY + globaliseY;

                    var texture = Global.SpriteLibrary.WorldMapSprites["tree-02"][0];

                    var tree = new SceneObjectStatic
                    {
                        Position = new Vector2(worldXPos, worldYPos),
                        Texture = texture,
                        ZIndex = (int)worldYPos + texture.Height - 16 // TODO: MAGIC NUMBER HERE, 1/4 THE SIZE OF PLAYER SPRITE
                    };

                    trees.Add(tree);
                }
            }

            return trees;
        }

        public List<ISceneObject> GenerateCactai(WorldChunk chunk)
        {
            var cactai = new List<ISceneObject>();

            for (int i = 0; i < chunk.Width; i++)
            {
                for (int j = 0; j < chunk.Height; j++)
                {
                    if (chunk.HeightData[i, j] < settings.GroundHeightMin ||
                        chunk.HeightData[i, j] > settings.GroundHeightMax)
                    {
                        continue;
                    }

                    if (chunk.BiomeData[i, j] != (int)Biome.Desert)
                    {
                        continue;
                    }

                    if (_random.NextDouble() < 0.995)
                    {
                        continue;
                    }


                    var halfTileSize = Global.TileSize / 2;

                    var chunkPosX = i * Global.TileSize;
                    var chunkPosY = j * Global.TileSize;

                    var randomiseX = _random.Next(-halfTileSize, halfTileSize);
                    var randomiseY = _random.Next(-halfTileSize, halfTileSize);

                    var globaliseX = chunk.ChunkCoordX * Global.TileSize * Global.ChunkSize;
                    var globaliseY = chunk.ChunkCoordY * Global.TileSize * Global.ChunkSize;

                    var worldXPos = chunkPosX + randomiseX + globaliseX;
                    var worldYPos = chunkPosY + randomiseY + globaliseY;

                    var texture = Global.SpriteLibrary.WorldMapSprites["cactus-01"][0];

                    var cactus = new SceneObjectStatic
                    {
                        Position = new Vector2(worldXPos, worldYPos),
                        Texture = texture,
                        ZIndex = (int)worldYPos + texture.Height - 16 // TODO: MAGIC NUMBER HERE, 1/4 THE SIZE OF PLAYER SPRITE
                    };

                    cactai.Add(cactus);
                }
            }

            return cactai;
        }

        public List<ISceneObject> GenerateRocks(WorldChunk chunk)
        {
            var rocks = new List<ISceneObject>();

            for (int i = 0; i < chunk.Width; i++)
            {
                for (int j = 0; j < chunk.Height; j++)
                {
                    if (chunk.HeightData[i, j] < settings.GroundHeightMin ||
                        chunk.HeightData[i, j] > settings.GroundHeightMax)
                    {
                        continue;
                    }

                    if (_random.NextDouble() < 0.995)
                    {
                        continue;
                    }

                    var halfTileSize = Global.TileSize / 2;

                    var chunkPosX = i * Global.TileSize;
                    var chunkPosY = j * Global.TileSize;

                    var randomiseX = _random.Next(-halfTileSize, halfTileSize);
                    var randomiseY = _random.Next(-halfTileSize, halfTileSize);

                    var globaliseX = chunk.ChunkCoordX * Global.TileSize * Global.ChunkSize;
                    var globaliseY = chunk.ChunkCoordY * Global.TileSize * Global.ChunkSize;

                    var worldXPos = chunkPosX + randomiseX + globaliseX;
                    var worldYPos = chunkPosY + randomiseY + globaliseY;

                    var texture = Global.SpriteLibrary.WorldMapSprites["rock-01"][0];

                    var rock = new SceneObjectStatic
                    {
                        Position = new Vector2(worldXPos, worldYPos),
                        Texture = texture,
                        ZIndex = (int)worldYPos + texture.Height - 16 // TODO: MAGIC NUMBER HERE, 1/4 THE SIZE OF PLAYER SPRITE
                    };

                    rocks.Add(rock);
                }
            }

            return rocks;
        }

        public void GenerateForestData(WorldChunk chunk)
        {
            chunk.ForestData = new int[chunk.Width, chunk.Height];

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
            if (chunk.HeightData[x, y] < settings.GroundHeightMin + 10)
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
                case Biome.Plains:
                case Biome.Winterlands:
                case Biome.Grasslands:
                    return true;

                default:
                    return false;
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

                    var tile = new Tile(Global.SpriteBatch, false)
                    {
                        ChunkX = chunk.ChunkCoordX,
                        ChunkY = chunk.ChunkCoordY,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * settings.TileSize, j * settings.TileSize),
                        Size = new Vector2(settings.TileSize, settings.TileSize),
                        //Texture = DetermineTexture(i, j, chunk.BiomeData),
                        Texture = Global.SpriteLibrary.WorldMapSprites["tree-02"][0],
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
                    return Global.SpriteLibrary.GetSprite("forest-C");

                case Biome.Desert:
                    return Global.SpriteLibrary.GetSprite("forest-C"); // Change this later?

                case Biome.Winterlands:
                    return Global.SpriteLibrary.GetSprite("forest-frost-C");

                default:
                    return Global.SpriteLibrary.GetSprite("forest-C");
            }
        }
    }
}
