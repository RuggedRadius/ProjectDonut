using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Debugging;
using ProjectDonut.GameObjects;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World.Generators;
using ProjectDonut.ProceduralGeneration.World.TileRules;
using ProjectDonut.UI.ScrollDisplay;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IGameComponent = ProjectDonut.Interfaces.IGameComponent;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class WorldChunkManager : IGameObject
    {
        public (int, int) PlayerChunkPosition { get; set; }
        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }
        public Texture2D Texture { get; set; }
        public bool IsVisible { get; set; }

        private List<object> Dependencies;

        private WorldGenerator WorldGen;
        private WorldMapSettings Settings;

        public Dictionary<(int, int), WorldChunk> _chunks;
        public List<WorldChunk> CurrentChunks;
        public WorldChunk PlayerChunk;

        private SpriteLibrary spriteLib;
        private FastNoiseLite _noise;

        private HeightGenerator genHeight;
        private BiomeGenerator genBiomes;
        private ForestGenerator genForest;
        private RiverGenerator genRiver;
        private MountainGenerator genMountain;
        private StructureGenerator genStructure;

        private GrasslandsRules rulesGrasslands;

        private Texture2D tempTexture;

        private int surroundChunkCount = 1;

        public List<ChunkStructure> StructuresInCenterChunk = new List<ChunkStructure>();

        public WorldChunkManager(WorldMapSettings settings)
        {
            Settings = settings;

            var random = new Random();
            var worldSeed = random.Next(int.MinValue, int.MaxValue);

            _noise = new FastNoiseLite();
            _noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            _noise.SetSeed(worldSeed);

            tempTexture = new Texture2D(Global.GraphicsDevice, 1, 1);
            tempTexture.SetData(new[] { Color.Green });

            WorldGen = new WorldGenerator(settings);
            genHeight = new HeightGenerator(settings);
            genBiomes = new BiomeGenerator(settings);
            genForest = new ForestGenerator(settings);
            genRiver = new RiverGenerator(settings);
            genMountain = new MountainGenerator(settings);
            genStructure = new StructureGenerator(settings);

            rulesGrasslands = new GrasslandsRules(spriteLib);
        }

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < CurrentChunks.Count; i++)
            {
                CurrentChunks[i].Draw(gameTime);
            }

            foreach (var structure in StructuresInCenterChunk)
            {
                Global.SpriteBatch.Draw(tempTexture, structure.Rectangle, Color.White);
            }
        }

        public void Update(GameTime gameTime)
        {
            var chunkPosChanged = false;

            if (Global.Player.ChunkPosX != PlayerChunkPosition.Item1)
            {
                chunkPosChanged = true;
            }

            if (Global.Player.ChunkPosY != PlayerChunkPosition.Item2)
            {
                chunkPosChanged = true;
            }

            if (chunkPosChanged)
            {
                PlayerChunkPosition = (Global.Player.ChunkPosX, Global.Player.ChunkPosY);


                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        var x = Global.Player.ChunkPosX + i;
                        var y = Global.Player.ChunkPosY + j;

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

            PlayerChunk = GetCurrentChunk();

            foreach (var chunk in CurrentChunks)
            {
                chunk.Update(gameTime);
            }
        }

        public void Initialize()
        {
            //ChunksBeingGenerated = new List<(int, int)>();

            //// Player chunk position
            PlayerChunkPosition = (Global.Player.ChunkPosX, Global.Player.ChunkPosY);

            // All chunks dictionary - initialised with starting 9 chunks
            _chunks = new Dictionary<(int, int), WorldChunk>();
            for (int x = -surroundChunkCount; x <= surroundChunkCount; x++)
            {
                for (int y = -surroundChunkCount; y <= surroundChunkCount; y++)
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


        //private bool tempONETREEONLY = false;
        private WorldChunk CreateChunk(int chunkX, int chunkY)
        {
            var chunk = new WorldChunk(chunkX, chunkY, this);
            chunk.HeightData = genHeight.GenerateHeightMap(Settings.Width, Settings.Height, chunkX, chunkY);
            chunk.BiomeData = genBiomes.GenerateBiomes(Settings.Width, Settings.Height, chunkX, chunkY);

            genRiver.GenerateRivers(chunk);
            genForest.GenerateForestData(chunk);
            genStructure.GenerateStructureData(chunk);

            var tilemapBase = genHeight.CreateBaseTilemap(chunk);
            //var tilemapForest = genForest.CreateTileMap(chunk);
            var tilemapStructures = genStructure.CreateTileMap(chunk);
            var tilemapMountains = genMountain.CreateTilemap(chunk);

            tilemapBase = rulesGrasslands.ApplyRules(tilemapBase);

            chunk.Tilemaps.Add("base", tilemapBase);
            //chunk.Tilemaps.Add("forest", tilemapForest);
            chunk.Tilemaps.Add("mountains", tilemapMountains);
            chunk.Tilemaps.Add("structures", tilemapStructures);

            chunk.SceneObjects = new Dictionary<string, List<ISceneObject>>();
            chunk.SceneObjects.Add("trees", genForest.GenerateTrees(chunk));
            chunk.SceneObjects.Add("trees-winter", genForest.GenerateWinterTrees(chunk));
            chunk.SceneObjects.Add("rocks", genForest.GenerateRocks(chunk));
            chunk.SceneObjects.Add("trees-loose", genForest.GenerateLooseTrees(chunk));
            chunk.SceneObjects.Add("cactus", genForest.GenerateCactai(chunk));

            chunk.Initialize();

            return chunk;
        }

        public void LoadContent()
        {
            foreach (var chunk in _chunks)
            {
                chunk.Value.LoadContent();
            }
        }

        public WorldChunk GetCurrentChunk()
        {
            return _chunks[(Global.Player.ChunkPosX, Global.Player.ChunkPosY)];
        }

        private List<WorldChunk> GetPlayerSurroundingChunks()
        {
            var playerChunks = new List<WorldChunk>();

            for (int i = -surroundChunkCount; i <= surroundChunkCount; i++)
            {
                for (int j = -surroundChunkCount; j <= surroundChunkCount; j++)
                {
                    var chunkX = Global.Player.ChunkPosX + i;
                    var chunkY = Global.Player.ChunkPosY + j;

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
