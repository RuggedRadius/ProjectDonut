using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.GameObjects
{
    public class WorldChunkManager : GameObject
    {

        public (int, int) PlayerChunkPosition { get; set; }

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

        private BaseGenerator baseGen;
        private BiomeGenerator biomes;

        private int ChunkWidth = 100;
        private int ChunkHeight = 100;

        private Texture2D tempTexture;

        //private List<(int, int)> ChunksBeingGenerated;

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

                    //case GraphicsDeviceManager graphicsDeviceManager:
                    //    this.graphics = graphicsDeviceManager;
                    //    break;

                    case GraphicsDevice graphicsDevice:
                        this._graphicsDevice = graphicsDevice;
                        break;

                    case SpriteBatch spriteBatch:
                        _spriteBatch = spriteBatch;
                        break;

                    //case Camera camera:
                    //    this.camera = camera;
                    //    break;

                    case Player player:
                        this.player = player;
                        break;

                    //case FogOfWar fog:
                    //    this.fog = fog;
                    //    break;

                    case SpriteLibrary spriteLib:
                        this.spriteLib = spriteLib;
                        break;

                    default:
                        break;
                        //throw new ArgumentException("Unknown dependency type");
                }
            }

            _noise = new FastNoiseLite();
            _noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            _noise.SetSeed(1337);

            tempTexture = new Texture2D(_graphicsDevice, 1, 1);
            tempTexture.SetData(new[] { Color.Green });

            WorldGen = new WorldGenerator(content, _graphicsDevice, settings, spriteLib);
            baseGen = new BaseGenerator(settings, spriteLib);    
            biomes = new BiomeGenerator(settings);
        }

        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < CurrentChunks.Count; i++)
            {
                CurrentChunks[i].Draw(gameTime);
            }
        }

        public override void Update(GameTime gameTime)
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
            //foreach (var chunk in _chunks)
            //{
            //    chunk.Value.Update(gameTime);
            //}
        }

        public override void Initialize()
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

            chunk.HeightData = baseGen.GenerateHeightMap(Settings.Width, Settings.Height, chunkX, chunkY);
            //chunk.BiomeData = WorldGen.TEMPCreateDummyBiomeData(Settings.Width, Settings.Height);
            chunk.BiomeData = biomes.GenerateBiomes(Settings.Width, Settings.Height, chunkX, chunkY);

            var tilemapBase = baseGen.CreateBaseTilemap(chunk.HeightData, chunk.BiomeData);
            //var tilemapForest = WorldGen.GenerateForestMap(Settings.Width, Settings.Height);

            chunk.Tilemaps.Add("base", tilemapBase);
            //chunk.tilemaps.Add("forest", tilemapForest);

            return chunk;
        }

        public override void LoadContent()
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
                for(int j = -1; j < 2; j++)
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
