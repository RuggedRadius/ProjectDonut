using Microsoft.Xna.Framework;
using ProjectDonut.Core;
using ProjectDonut.Core.Sprites;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World.MineableItems;
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

        public List<IMineable> GenerateWinterTrees(WorldChunk chunk)
        {
            var trees = new List<IMineable>();

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

                    //var treeCount = _random.Next(2, 4);
                    var treeCount = 1;
                    var halfTileSize = Global.TileSize / 2;

                    for (int k = 0; k < treeCount; k++)
                    {
                        var chunkPosX = i * Global.TileSize;
                        var chunkPosY = j * Global.TileSize;

                        var positionVariantX = _random.Next(-settings.TileSize / 2, settings.TileSize / 2);
                        var positionVariantY = _random.Next(-settings.TileSize / 2, settings.TileSize / 2);

                        var globaliseX = chunk.ChunkCoordX * Global.TileSize * Global.ChunkSize;
                        var globaliseY = chunk.ChunkCoordY * Global.TileSize * Global.ChunkSize;

                        var offsetX = -Global.TileSize;
                        var offsetY = -Global.TileSize * 2;

                        var worldXPos = chunkPosX + globaliseX + halfTileSize + positionVariantX + offsetX;
                        var worldYPos = chunkPosY + globaliseY + halfTileSize + positionVariantY + offsetY;

                        var texture = SpriteLib.Mineables.Sprites["tree-02-winter"][0];

                        var tree = new MineableTreeWinter()
                        {
                            WorldPosition = new Vector2(worldXPos, worldYPos),
                            Texture = texture,
                            ZIndex = (int)worldYPos + texture.Height - 16, // TODO: MAGIC NUMBER HERE, 1/4 THE SIZE OF PLAYER SPRITE
                            InventoryIcon = SpriteLib.UI.Items["wood-log"],
                        };

                        tree.Intialize();
                        tree.LoadContent();
                        trees.Add(tree);
                    }
                }
            }

            return trees;
        }

        public List<IMineable> GenerateTrees(WorldChunk chunk)
        {
            var trees = new List<IMineable>();

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

                    //var treeCount = _random.Next(2, 4);
                    var treeCount = 1;
                    var halfTileSize = Global.TileSize / 2;

                    for (int k = 0; k < treeCount; k++)
                    {
                        var chunkPosX = i * Global.TileSize;
                        var chunkPosY = j * Global.TileSize;

                        var positionVariantX = _random.Next(-settings.TileSize / 2, settings.TileSize / 2);
                        var positionVariantY = _random.Next(-settings.TileSize / 2, settings.TileSize / 2);

                        var globaliseX = chunk.ChunkCoordX * Global.TileSize * Global.ChunkSize;
                        var globaliseY = chunk.ChunkCoordY * Global.TileSize * Global.ChunkSize;

                        var offsetX = -Global.TileSize;
                        var offsetY = -Global.TileSize * 2;

                        var worldXPos = chunkPosX + globaliseX + halfTileSize + positionVariantX + offsetX;                        
                        var worldYPos = chunkPosY + globaliseY + halfTileSize + positionVariantY + offsetY;

                        var texture = SpriteLib.Mineables.Sprites["tree-02"][0];

                        var tree = new MineableTree()
                        {
                            WorldPosition = new Vector2(worldXPos, worldYPos),
                            Texture = texture,
                            ZIndex = (int)worldYPos + texture.Height - 16, // TODO: MAGIC NUMBER HERE, 1/4 THE SIZE OF PLAYER SPRITE
                            InventoryIcon = SpriteLib.UI.Items["wood-log"],
                        };

                        tree.Intialize();
                        tree.LoadContent();
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
        public List<IMineable> GenerateLooseTrees(WorldChunk chunk)
        {
            var trees = new List<IMineable>();
            var halfTileSize = Global.TileSize / 2;

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


                    var chunkPosX = i * Global.TileSize;
                    var chunkPosY = j * Global.TileSize;

                    var positionVariantX = _random.Next(-settings.TileSize / 2, settings.TileSize / 2);
                    var positionVariantY = _random.Next(-settings.TileSize / 2, settings.TileSize / 2);

                    var globaliseX = chunk.ChunkCoordX * Global.TileSize * Global.ChunkSize;
                    var globaliseY = chunk.ChunkCoordY * Global.TileSize * Global.ChunkSize;

                    var offsetX = -Global.TileSize;
                    var offsetY = -Global.TileSize * 2;

                    var worldXPos = chunkPosX + globaliseX + halfTileSize + positionVariantX + offsetX;
                    var worldYPos = chunkPosY + globaliseY + halfTileSize + positionVariantY + offsetY;

                    var texture = SpriteLib.Mineables.Sprites["tree-02"][0];

                    var tree = new MineableTree()
                    {
                        WorldPosition = new Vector2(worldXPos, worldYPos),
                        Texture = texture,
                        ZIndex = (int)worldYPos + texture.Height - 16, // TODO: MAGIC NUMBER HERE, 1/4 THE SIZE OF PLAYER SPRITE
                        InventoryIcon = SpriteLib.UI.Items["wood-log"],
                    };

                    tree.Intialize();
                    tree.LoadContent();
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

                    var texture = SpriteLib.Mineables.Sprites["cactus-01"][0];

                    var cactus = new SceneObjectStatic
                    {
                        WorldPosition = new Vector2(worldXPos, worldYPos),
                        Texture = texture,
                        ZIndex = (int)worldYPos + texture.Height - 16 // TODO: MAGIC NUMBER HERE, 1/4 THE SIZE OF PLAYER SPRITE
                    };

                    cactai.Add(cactus);
                }
            }

            return cactai;
        }

        public List<IMineable> GenerateRocks(WorldChunk chunk)
        {
            var rocks = new List<IMineable>();

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

                    if (chunk.BiomeData[i, j] != (int)Biome.Ashlands)
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

                    //var texture = SpriteLib.WorldMapSprites["rock-01"][0];
                    var texture = SpriteLib.Mineables.Sprites["rock"][0];

                    var rock = new MineableRock()
                    {
                        WorldPosition = new Vector2(worldXPos, worldYPos),
                        Texture = texture,
                        ZIndex = (int)worldYPos + texture.Height - 16, // TODO: MAGIC NUMBER HERE, 1/4 THE SIZE OF PLAYER SPRITE
                        InventoryIcon = SpriteLib.UI.Items["rock"],
                    };

                    rock.Intialize();
                    rock.LoadContent();
                    rocks.Add(rock);
                }
            }

            return rocks;
        }
    }
}
