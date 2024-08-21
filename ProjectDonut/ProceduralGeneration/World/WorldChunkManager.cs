using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.GameObjects;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World.Generators;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class WorldChunkManager : IGameObject
    {
        public (int, int) PlayerChunkPosition { get; set; }
        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }

        private int ChunkSize = 100;
        private List<object> Dependencies;

        private WorldGenerator WorldGen;
        private WorldMapSettings Settings;
        private Player player;

        public Dictionary<(int, int), WorldChunk> _chunks;
        private List<WorldChunk> CurrentChunks;

        private GraphicsDevice _graphicsDevice;
        private ContentManager content;
        private SpriteLibrary spriteLib;
        private SpriteBatch _spriteBatch;
        private FastNoiseLite _noise;

        private HeightGenerator genHeight;
        private BiomeGenerator genBiomes;
        private ForestGenerator genForest;
        private RiverGenerator genRiver;
        private MountainGenerator genMountain;
        private StructureGenerator genStructure;

        private int ChunkWidth = 100;
        private int ChunkHeight = 100;

        private Texture2D tempTexture;

        public List<ChunkStructure> StructuresInCenterChunk = new List<ChunkStructure>();

        public WorldChunkManager(List<object> dependencies, WorldMapSettings settings)
        {
            Dependencies = dependencies;
            Settings = settings;

            foreach (var dependency in dependencies)
            {
                switch (dependency)
                {
                    case ContentManager content:
                        this.content = content;
                        break;

                    case GraphicsDevice graphicsDevice:
                        _graphicsDevice = graphicsDevice;
                        break;

                    case SpriteBatch spriteBatch:
                        _spriteBatch = spriteBatch;
                        break;

                    case Player player:
                        this.player = player;
                        break;

                    case SpriteLibrary spriteLib:
                        this.spriteLib = spriteLib;
                        break;

                    default:
                        break;
                }
            }

            var random = new Random();
            var worldSeed = random.Next(int.MinValue, int.MaxValue);

            _noise = new FastNoiseLite();
            _noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            _noise.SetSeed(worldSeed);

            tempTexture = new Texture2D(_graphicsDevice, 1, 1);
            tempTexture.SetData(new[] { Color.Green });

            WorldGen = new WorldGenerator(content, _graphicsDevice, settings, spriteLib, _spriteBatch);
            genHeight = new HeightGenerator(settings, spriteLib, _spriteBatch);
            genBiomes = new BiomeGenerator(settings);
            genForest = new ForestGenerator(spriteLib, settings, _spriteBatch);
            genRiver = new RiverGenerator(spriteLib, settings);
            genMountain = new MountainGenerator(settings, spriteLib, _spriteBatch);
            genStructure = new StructureGenerator(spriteLib, settings, _spriteBatch);
        }

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < CurrentChunks.Count; i++)
            {
                CurrentChunks[i].Draw(gameTime);
            }

            foreach (var structure in StructuresInCenterChunk)
            {
                _spriteBatch.Draw(tempTexture, structure.Rectangle, Color.White);
            }
        }

        public void Update(GameTime gameTime)
        {
            var chunkPosChanged = false;

            if (player.ChunkPosX != PlayerChunkPosition.Item1)
            {
                chunkPosChanged = true;
            }

            if (player.ChunkPosY != PlayerChunkPosition.Item2)
            {
                chunkPosChanged = true;
            }

            if (chunkPosChanged)
            {
                PlayerChunkPosition = (player.ChunkPosX, player.ChunkPosY);


                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        var x = player.ChunkPosX + i;
                        var y = player.ChunkPosY + j;

                        var chunk = GetChunk((x, y));
                        if (chunk == null)
                        {
                            Task.Run(() =>
                            {
                                chunk = CreateChunk(x, y);

                                if (_chunks.ContainsKey((x, y)) == false)
                                {
                                    _chunks.Add((x, y), chunk);
                                }
                            });
                        }
                    }
                }

                CurrentChunks = GetPlayerSurroundingChunks();
            }
            else
            {
                if (CurrentChunks.Count < 9)
                {
                    CurrentChunks = GetPlayerSurroundingChunks();
                }
            }

            // Disabling for now cos collection kept changing duration iterations
            foreach (var chunk in CurrentChunks)
            {
                chunk.Update(gameTime);
            }

            foreach (var structure in StructuresInCenterChunk)
            {
                structure.Update(gameTime);
            }
        }

        private List<ChunkStructure> GetStructuresInCurrentChunks()
        {
            var structures = new List<ChunkStructure>();

            foreach (var chunk in CurrentChunks)
            {
                for (int i = 0; i < chunk.Width; i++)
                {
                    for (int j = 0; j < chunk.Height; j++)
                    {
                        if (chunk.StructureData[i, j] != 0)
                        {
                            var structure = new ChunkStructure()
                            {
                                StructureName = "test",
                                StructureType = (Structure)chunk.StructureData[i, j],
                                Rectangle = new Rectangle(i * ChunkSize, j * ChunkSize, ChunkSize, ChunkSize)
                            };

                            structures.Add(structure);
                        }
                    }
                }
            }

            return structures;
        }

        public void Initialize()
        {
            //ChunksBeingGenerated = new List<(int, int)>();

            //// Player chunk position
            PlayerChunkPosition = (player.ChunkPosX, player.ChunkPosY);

            // All chunks dictionary - initialised with starting 9 chunks
            _chunks = new Dictionary<(int, int), WorldChunk>();
            int testsize = 1;
            for (int x = -testsize; x <= testsize; x++)
            {
                for (int y = -testsize; y <= testsize; y++)
                {
                    var key = (x, y);
                    var chunk = CreateChunk(x, y);
                    _chunks.Add(key, chunk);
                }
            }

            CurrentChunks = GetPlayerSurroundingChunks();

            foreach (var chunk in _chunks)
            {
                chunk.Value.Initialize();
            }
        }

        private WorldChunk CreateChunk(int chunkX, int chunkY)
        {
            var chunk = new WorldChunk(chunkX, chunkY, _graphicsDevice, _spriteBatch);
            chunk.HeightData = genHeight.GenerateHeightMap(Settings.Width, Settings.Height, chunkX, chunkY);
            chunk.BiomeData = genBiomes.GenerateBiomes(Settings.Width, Settings.Height, chunkX, chunkY);

            genRiver.GenerateRivers(chunk);
            genForest.GenerateForestData(chunk);
            genStructure.GenerateStructureData(chunk);

            var tilemapBase = genHeight.CreateBaseTilemap(chunk);
            var tilemapForest = genForest.CreateTileMap(chunk);
            var tilemapStructures = genStructure.CreateTileMap(chunk);
            var tilemapMountains = genMountain.CreateTilemap(chunk);

            chunk.Tilemaps.Add("base", tilemapBase);
            chunk.Tilemaps.Add("forest", tilemapForest);
            chunk.Tilemaps.Add("mountains", tilemapMountains);
            chunk.Tilemaps.Add("structures", tilemapStructures);

            return chunk;
        }

        public void LoadContent()
        {
            foreach (var chunk in _chunks)
            {
                chunk.Value.LoadContent();
            }
        }

        private List<WorldChunk> GetPlayerSurroundingChunks()
        {
            var playerChunks = new List<WorldChunk>();

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    var chunkX = player.ChunkPosX + i;
                    var chunkY = player.ChunkPosY + j;

                    if (_chunks.ContainsKey((chunkX, chunkY)))
                    {
                        playerChunks.Add(_chunks[(chunkX, chunkY)]);
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return playerChunks;
        }

        private WorldChunk GetChunk((int, int) chunkCoords)
        {
            if (_chunks.ContainsKey(chunkCoords))
            {
                return _chunks[chunkCoords];
            }
            else
            {
                return null;
                //var newChunk = CreateChunk(chunkCoords.Item1, chunkCoords.Item2);
                //AllChunks.Add(chunkCoords, newChunk);
                //return newChunk;
            }
        }
    }
}
