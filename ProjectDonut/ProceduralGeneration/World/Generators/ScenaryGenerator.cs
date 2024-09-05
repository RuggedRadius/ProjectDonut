﻿using Microsoft.Xna.Framework;
using ProjectDonut.Core;
using ProjectDonut.Interfaces;
using System;
using System.Collections.Generic;

namespace ProjectDonut.ProceduralGeneration.World.Generators
{
    public class ScenaryGenerator
    {
        private WorldMapSettings settings;
        private Random _random = new Random();

        public ScenaryGenerator(WorldMapSettings settings)
        {
            this.settings = settings;
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
    }
}
