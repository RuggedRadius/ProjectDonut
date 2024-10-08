using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using ProjectDonut.Core.Sprites;
using ProjectDonut.GameObjects;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World.Generators;
using ProjectDonut.ProceduralGeneration.World.MineableItems;
using ProjectDonut.ProceduralGeneration.World.TileRules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class WorldChunkManager :  IGameObject
    {
        public (int, int) PlayerChunkPosition { get; set; }
        public Vector2 WorldPosition { get; set; }
        public int ZIndex { get; set; }
        public Texture2D Texture { get; set; }
        public bool IsVisible { get; set; }

        private List<object> Dependencies;

        //private WorldGenerator WorldGen;
        private WorldMapSettings Settings;

        public Dictionary<(int, int), WorldChunk> _chunks;
        public List<WorldChunk> CurrentChunks;
        public List<WorldChunk> MinimapCurrentChunks;
        public WorldChunk PlayerChunk;
        public List<(int, int)> ExistingChunks;

        private SpriteLib spriteLib;
        private FastNoiseLite _noise;

        private TerrainGenerator genHeight;
        private BiomeGenerator genBiomes;
        private ForestGenerator genForest;
        private RiverGenerator genRiver;
        private MountainGenerator genMountain;
        private StructureGenerator genStructure;
        private ScenaryGenerator _genScenary;
        private TownBuilder _townBuilder;

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

            //WorldGen = new WorldGenerator(settings);
            genHeight = new TerrainGenerator(settings);
            genBiomes = new BiomeGenerator(settings);
            genForest = new ForestGenerator(settings);
            genRiver = new RiverGenerator(settings);
            genMountain = new MountainGenerator(settings);
            genStructure = new StructureGenerator(settings);
            _genScenary = new ScenaryGenerator(settings);
            _townBuilder = new TownBuilder(settings);

            rulesGrasslands = new GrasslandsRules();

            ExistingChunks = new List<(int, int)>();
        }

        public void Update(GameTime gameTime)
        {
            if (Global.SceneManager.CurrentScene.SceneType != Core.SceneManagement.SceneType.World)
                return;

            var chunkPosChanged = false;

            if (Global.PlayerObj.ChunkPosX != PlayerChunkPosition.Item1)
            {
                chunkPosChanged = true;
            }

            if (Global.PlayerObj.ChunkPosY != PlayerChunkPosition.Item2)
            {
                chunkPosChanged = true;
            }

            if (chunkPosChanged)
            {
                PlayerChunkPosition = (Global.PlayerObj.ChunkPosX, Global.PlayerObj.ChunkPosY);


                for (int i = -2; i < 3; i++)
                {
                    for (int j = -2; j < 3; j++)
                    {
                        var x = Global.PlayerObj.ChunkPosX + i;
                        var y = Global.PlayerObj.ChunkPosY + j;

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
                MinimapCurrentChunks = GetPlayerSurroundingMinimapChunks();
            }
            else
            {
                if (CurrentChunks.Count < 9)
                {
                    CurrentChunks = GetPlayerSurroundingChunks();
                    MinimapCurrentChunks = GetPlayerSurroundingMinimapChunks();
                }
            }

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

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < CurrentChunks.Count; i++)
            {
                CurrentChunks[i].Draw(gameTime);
            }

            //for (int i = 0; i < CurrentChunks.Count; i++)
            //{
            //    CurrentChunks[i].MineableObjects.Values.ToList().ForEach(x => x.ForEach(y => y.Draw(gameTime)));
            //}

            //foreach (var structure in StructuresInCenterChunk)
            //{
            //    Global.SpriteBatch.Draw(tempTexture, structure.Rectangle, Color.White);
            //}
        }

        public void Initialize()
        {
            //ChunksBeingGenerated = new List<(int, int)>();

            //// Player chunk position
            PlayerChunkPosition = (Global.PlayerObj.ChunkPosX, Global.PlayerObj.ChunkPosY);

            // All chunks dictionary - initialised with starting 9 chunks
            _chunks = new Dictionary<(int, int), WorldChunk>();
            for (int x = -surroundChunkCount - 2; x <= surroundChunkCount + 2; x++)
            {
                for (int y = -surroundChunkCount - 2; y <= surroundChunkCount + 2; y++)
                {
                    var key = (x, y);
                    var chunk = CreateChunk(x, y);
                    _chunks.Add(key, chunk);
                }
            }

            CurrentChunks = GetPlayerSurroundingChunks();
            MinimapCurrentChunks = GetPlayerSurroundingMinimapChunks();

            foreach (var chunk in _chunks)
            {
                chunk.Value.Initialize();
            }
        }


        // TODO: convert sceneobjects into mineables
        private WorldChunk CreateChunk(int chunkX, int chunkY)
        {
            var chunk = new WorldChunk(chunkX, chunkY, this);
            chunk.HeightData = genHeight.GenerateHeightMap(Settings.Width, Settings.Height, chunkX, chunkY);
            chunk.BiomeData = genBiomes.GenerateBiomes(Settings.Width, Settings.Height, chunkX, chunkY);
            chunk.ChunkBounds = new Rectangle(
                chunkX * Global.ChunkSize * Global.TileSize, 
                chunkY * Global.ChunkSize * Global.TileSize, 
                Settings.Width * Global.TileSize, 
                Settings.Height * Global.TileSize);

            //genRiver.GenerateRivers(chunk);
            genForest.GenerateForestData(chunk);
            //genStructure.GenerateStructureData(chunk);

            var tilemapBase = genHeight.CreateTerrainTilemap(chunk);
            //var tilemapForest = genForest.CreateTileMap(chunk);
            //var tilemapStructures = genStructure.CreateTileMap(chunk);
            var tilemapMountains = genMountain.CreateTilemap(chunk);

            tilemapBase = rulesGrasslands.ApplyRules(tilemapBase);

            chunk.Tilemaps.Add("base", tilemapBase);
            //chunk.Tilemaps.Add("forest", tilemapForest);
            chunk.Tilemaps.Add("mountains", tilemapMountains);
            //chunk.Tilemaps.Add("structures", tilemapStructures);

            chunk.SceneObjects = new Dictionary<string, List<ISceneObject>>();
            chunk.MineableObjects = new Dictionary<string, List<IMineable>>();

            
            chunk.MineableObjects.Add("rocks", _genScenary.GenerateRocks(chunk));
            //chunk.SceneObjects.Add("trees", _genScenary.GenerateLooseTrees(chunk)); // TEMP TURNED OFF
            chunk.SceneObjects.Add("cactus", _genScenary.GenerateCactai(chunk));

            chunk.MineableObjects.Add("trees", _genScenary.GenerateTrees(chunk));
            chunk.MineableObjects["trees"].AddRange(_genScenary.GenerateWinterTrees(chunk));

            chunk.SceneObjects.Add("castles", genStructure.GenerateCastles(chunk));
            chunk.SceneObjects.Add("towns", genStructure.GenerateTowns(chunk));

            chunk.Initialize();
            chunk.LoadContent();

            ExistingChunks.Add((chunkX, chunkY));

            if (new Random().Next(0, 100) > 95)
                _townBuilder.Build(ref chunk);  

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
            return _chunks[(Global.PlayerObj.ChunkPosX, Global.PlayerObj.ChunkPosY)];
        }

        private List<WorldChunk> GetPlayerSurroundingChunks()
        {
            var playerChunks = new List<WorldChunk>();

            for (int i = -surroundChunkCount; i <= surroundChunkCount; i++)
            {
                for (int j = -surroundChunkCount; j <= surroundChunkCount; j++)
                {
                    var chunkX = Global.PlayerObj.ChunkPosX + i;
                    var chunkY = Global.PlayerObj.ChunkPosY + j;

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

            //foreach (var chunk in _chunks)
            //{
            //    if (chunk.Value == null)
            //    {
            //        continue;
            //    }

            //    if (playerChunks.Contains(chunk.Value) == false)
            //    {
            //        SaveChunkToFile(chunk.Value);

            //        // TODO: Need to clear the chunk data from memory
            //        //chunk.Value = null;
            //    }
            //}

            return playerChunks;
        }

        private List<WorldChunk> GetPlayerSurroundingMinimapChunks()
        {
            var minimapChunks = new List<WorldChunk>();

            for (int i = -4; i <= 4; i++)
            {
                for (int j = -4; j <= 4; j++)
                {
                    var chunkX = Global.PlayerObj.ChunkPosX + i;
                    var chunkY = Global.PlayerObj.ChunkPosY + j;

                    if (_chunks.ContainsKey((chunkX, chunkY)))
                    {
                        minimapChunks.Add(_chunks[(chunkX, chunkY)]);
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return minimapChunks;
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

        //private WorldChunk LoadChunkFromFile((int, int) chunkCoords)
        //{
        //    if (ExistingChunks.Contains(chunkCoords) == false)
        //    {
        //        return;
        //    }

        //    string filePath = $"[{chunkCoords.Item1}][{chunkCoords.Item2}].json";

        //    // Read the JSON string from the file
        //    string jsonString = File.ReadAllText(filePath);

        //    // Deserialize the JSON string back into the custom object
        //    var chunk = JsonSerializer.Deserialize<WorldChunk>(jsonString);

        //    return chunk;
        //}

        private void SaveChunkToFile(WorldChunk chunk)
        {
            string filePath = $"[{chunk.WorldCoordX}][{chunk.WorldCoordY}].json"; // Define the file path

            // Serialize the object to JSON
            string jsonString = JsonSerializer.Serialize(chunk);

            // Write the JSON string to a file
            File.WriteAllText(filePath, jsonString);
        }
    }
}
