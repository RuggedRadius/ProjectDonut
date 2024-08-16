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

                    case GraphicsDevice graphicsDevice:
                        this.graphicsDevice = graphicsDevice;
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
                        var x = -player.ChunkPosX + i;
                        var y = -player.ChunkPosY + j;

                        var chunk = GetChunk((x, y));
                        if (chunk == null)
                        {
                            Task.Run(() =>
                            {
                                chunk = CreateChunk(x, y);

                                if (AllChunks.ContainsKey((x, y)) == false)
                                {
                                    AllChunks.Add((x, y), chunk);
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
        }

        public override void Initialize()
        { 
            //ChunksBeingGenerated = new List<(int, int)>();

            //// Player chunk position
            PlayerChunkPosition = (player.ChunkPosX, player.ChunkPosY);

            // All chunks dictionary - initialised with starting 9 chunks
            AllChunks = new Dictionary<(int, int), WorldChunk>();
            int testsize = 1;
            for (int x = -testsize; x <= testsize; x++)
            {
                for (int y = -testsize; y <= testsize; y++)
                {
                    var key = (x, y);
                    var chunk = CreateChunk(x, y);
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
            //var tilemapForest = WorldGen.GenerateForestMap(Settings.Width, Settings.Height);

            chunk.tilemaps.Add("base", tilemapBase);
            //chunk.tilemaps.Add("forest", tilemapForest);

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

                    var chunkX = -player.ChunkPosX + i;
                    var chunkY = -player.ChunkPosY + j;

                    //var chunk = GetChunk((i, j));

                    //playerChunks.Add(GetChunk((i, j)));
                    if (AllChunks.ContainsKey((chunkX, chunkY)))
                    {
                        playerChunks.Add(AllChunks[(chunkX, chunkY)]);
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
            if (AllChunks.ContainsKey(chunkCoords))
            {
                return AllChunks[chunkCoords];
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
