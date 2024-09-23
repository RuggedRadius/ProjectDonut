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
using System.Linq;
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
        public WorldChunk PlayerChunk;

        private SpriteLib spriteLib;
        private FastNoiseLite _noise;

        private TerrainGenerator genHeight;
        private BiomeGenerator genBiomes;
        private ForestGenerator genForest;
        private RiverGenerator genRiver;
        private MountainGenerator genMountain;
        private StructureGenerator genStructure;
        private ScenaryGenerator _genScenary;

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

            rulesGrasslands = new GrasslandsRules();
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


                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
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
            }
            else
            {
                if (CurrentChunks.Count < 9)
                {
                    CurrentChunks = GetPlayerSurroundingChunks();
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


        // TODO: convert sceneobjects into mineables
        private WorldChunk CreateChunk(int chunkX, int chunkY)
        {
            var chunk = new WorldChunk(chunkX, chunkY, this);
            chunk.HeightData = genHeight.GenerateHeightMap(Settings.Width, Settings.Height, chunkX, chunkY);
            //chunk.BiomeData = genBiomes.GenerateBiomes(Settings.Width, Settings.Height, chunkX, chunkY);
            chunk.BiomeData = genBiomes.GenBiomes(Settings.Width, Settings.Height, chunkX, chunkY); // TODO: NEW TEMP + HUMIDITY METHOD, NEEDS WORK

            genRiver.GenerateRivers(chunk);
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

            CarvePathsToTownsThroughMineables(ref chunk);

            // TODO: FIX THIS!
            var structures = new List<ISceneObject>();
            structures.AddRange(chunk.SceneObjects["towns"]);
            structures.AddRange(chunk.SceneObjects["castles"]);
            foreach (var structure in structures)
            {
                //CarveRiverToPosition(ref chunk, structure.WorldPosition);
            }

            return chunk;
        }

        // TODO: FIX THIS!
        private void CarveRiverToPosition(ref WorldChunk chunk, Vector2 worldPosition)
        {
            var random = new Random();

            //var chunkXCoord = (int)(worldPosition.X / Global.ChunkSize) / Global.TileSize;
            //var chunkYCoord = (int)(worldPosition.Y / Global.ChunkSize) / Global.TileSize;

            var restrictedDirection = random.Next(0, 4);
            var pathLength = random.Next(60, 100);

            // Find nearest water tile
            var targetTexture = SpriteLib.GetSprite("coast-inv-c");
            var map = chunk.Tilemaps["base"].Map;
            Tile nearestWaterTile = null;
            var nearestWaterTileDistance = float.MaxValue;
            var targetPosX = 0;
            var targetPosY = 0;
            
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j].Texture != targetTexture)
                        continue;

                    var tilePos = new Vector2(map[i, j].WorldPosition.X, map[i, j].WorldPosition.Y);
                    var distance = Vector2.Distance(tilePos, worldPosition);

                    if (distance < nearestWaterTileDistance)
                    {
                        nearestWaterTile = map[i, j];
                        nearestWaterTileDistance = distance;
                        targetPosX = i;
                        targetPosY = j;
                    }
                }
            }

            if (nearestWaterTile == null)
                return;

            // TODO: Write global function to convert world positions to chunk positions
            var curPosX = (int)(worldPosition.X / Global.ChunkSize) / Global.TileSize;
            var curPosY = (int)(worldPosition.Y / Global.ChunkSize) / Global.TileSize;
            do
            {
                if (curPosX < targetPosX)
                    curPosX += 1;
                else if (curPosX > targetPosX)
                    curPosX -= 1;

                if (curPosY < targetPosY)
                    curPosY += 1;
                else if (curPosY > targetPosY)
                    curPosY -= 1;

                chunk.Tilemaps["base"].Map[curPosX, curPosY].Texture = SpriteLib.lib["coast-c"];
            }
            while (curPosX != targetPosX && curPosY != targetPosY);
        }

        private void CarvePathsToTownsThroughMineables(ref WorldChunk chunk)
        {
            var random = new Random();
            var scanRadius = 2;
            var scanSize = new Vector2(Global.TileSize * scanRadius, Global.TileSize * scanRadius);
            var structures = new List<ISceneObject>();
            structures.AddRange(chunk.SceneObjects["towns"]);
            structures.AddRange(chunk.SceneObjects["castles"]);
            foreach (var castle in structures)
            {
                var restrictedDirection = random.Next(0, 4);
                var pathLength = random.Next(60, 100);

                var startXPos = castle.TextureBounds.Left + (castle.TextureBounds.Width / 2);
                var startYPos = castle.TextureBounds.Bottom + (castle.TextureBounds.Height / 2);
                var scanRect = new Rectangle(startXPos, startYPos, (int)scanSize.X, (int)scanSize.Y);
                for (int i = 0; i < pathLength; i++) 
                {
                    var nextDirection = -1;
                    do
                    {
                        nextDirection = random.Next(0, 4);
                    }
                    while (nextDirection == restrictedDirection);

                    switch (nextDirection)
                    {
                        case 0: startYPos -= Global.TileSize; break;
                        case 1: startXPos += Global.TileSize; break;
                        case 2: startYPos += Global.TileSize; break;
                        case 3: startXPos -= Global.TileSize; break;
                    }

                    scanRect.X = startXPos;
                    scanRect.Y = startYPos;

                    var mineablesToRemove = new List<IMineable>();
                    foreach (var mineableType in chunk.MineableObjects.Values)
                    {
                        foreach (var mineableObj in mineableType)
                        {
                            if (mineableObj.InteractBounds.Intersects(scanRect))
                            {
                                mineablesToRemove.Add(mineableObj);
                            }
                        }
                    }

                    if (mineablesToRemove.Count > 0)
                    {
                        ;
                    }

                    foreach (var mineable in mineablesToRemove)
                    {
                        chunk.MineableObjects.Values.ToList().ForEach(x => x.Remove(mineable));
                    }


                }
            }
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
