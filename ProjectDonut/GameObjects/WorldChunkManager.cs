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

        public Dictionary<(int, int), WorldChunk> AllChunks;
        private List<WorldChunk> CurrentChunks;

        private GraphicsDevice graphicsDevice;
        private ContentManager content;
        private SpriteLibrary spriteLib;

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
                        this.graphicsDevice = graphicsDevice;
                        break;

                    //case SpriteBatch spriteBatch:
                    //    this.spriteBatch = spriteBatch;
                    //    break;

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

            WorldGen = new WorldGenerator(content, graphicsDevice, settings, spriteLib);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var chunk in CurrentChunks)
            {
                chunk.Draw(gameTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            var chunkPosChanged = false;

            if (player.ChunkPosX != PlayerChunkPosition.Item1)
            {
                chunkPosChanged = true;

                // Handle horizontal chunk loading/generating
            }

            if (player.ChunkPosY != PlayerChunkPosition.Item2) 
            {
                chunkPosChanged = true;

                // Handle vertical chunk loading/generating
            }

            if (chunkPosChanged)
            {
                PlayerChunkPosition = (player.ChunkPosX, player.ChunkPosY);
                CurrentChunks = GetPlayerSurroundingChunks();
            }
        }

        public override void Initialize()
        { 
            //// Player chunk position
            PlayerChunkPosition = (player.ChunkPosX, player.ChunkPosY);

            // All chunks dictionary - initialised with starting 9 chunks
            AllChunks = new Dictionary<(int, int), WorldChunk>();
            int testsize = 2;
            for (int i = -testsize; i <= testsize; i++)
            {
                for (int j = -testsize; j <= testsize; j++)
                {
                    var key = (i, j);
                    var chunk = CreateChunk(i, j);
                    AllChunks.Add(key, chunk);
                }
            }

            CurrentChunks = GetPlayerSurroundingChunks();

            foreach (var chunk in CurrentChunks)
            {
                chunk.Initialize();
            }
        }

        private WorldChunk CreateChunk(int chunkX, int chunkY)
        {
            var chunk = new WorldChunk(Dependencies, Settings, chunkX, chunkY);

            var tilemapBase = WorldGen.GenerateBaseMap(Settings.Width, Settings.Height, chunkX, chunkY);
            var tilemapForest = WorldGen.GenerateForestMap(Settings.Width, Settings.Height);

            chunk.tilemaps.Add("base", tilemapBase);
            chunk.tilemaps.Add("forest", tilemapForest);

            return chunk;
        }

        public override void LoadContent()
        {
            foreach (var chunk in CurrentChunks)
            {
                chunk.LoadContent();
            }
        }

        private List<WorldChunk> GetPlayerSurroundingChunks()
        {
            var playerChunks = new List<WorldChunk>();

            for (int i = -1; i < 2; i++)
            {
                for(int j = -1; j < 2; j++)
                {
                    //var chunkX = PlayerChunkPosition.Item1 + i;
                    //var chunkY = PlayerChunkPosition.Item2 + j;

                    var chunkX = player.ChunkPosX + i;
                    var chunkY = player.ChunkPosY + j;

                    playerChunks.Add(AllChunks[(chunkX, chunkY)]);
                }
            }

            return playerChunks;
        }

        private WorldChunk GetChunk((int, int) chunkCoords)
        {
            return AllChunks[chunkCoords];
        }
    }
}
