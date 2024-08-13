using Microsoft.Xna.Framework;
using ProjectDonut.ProceduralGeneration.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.GameObjects
{
    public class WorldChunkManager : GameObject
    {
        public Dictionary<Vector2Int, WorldChunk> Chunks;        
        public Vector2Int PlayerChunkPosition { get; set; }

        private int ChunkSize = 100;
        private List<object> Dependencies;
        private WorldMapSettings Settings;

        public WorldChunkManager(List<object> dependencies, WorldMapSettings settings)
        {
            Dependencies = dependencies;
            Settings = settings;
        }

        public override void Draw(GameTime gameTime)
        {

        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Initialize()
        {
            Chunks = new Dictionary<Vector2Int, WorldChunk>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var key = new Vector2Int(i - 1, j - 1);
                    var value = new WorldChunk(Dependencies, Settings, i - 1, j - 1);

                    Chunks.Add(key, value);
                }
            }
        }

        public override void LoadContent()
        {

        }

        private WorldChunk[,] GetPlayerSurroundingChunks()
        {
            var playerChunks = new WorldChunk[3,3];

            for (int i = -1; i < 2; i++)
            {
                for(int j = -1; j < 2; j++)
                {
                    var chunkX = PlayerChunkPosition.X + i;
                    var chunkY = PlayerChunkPosition.Y + j;

                    playerChunks[i + 1, j + 1] = GetChunk(new Vector2Int(chunkX, chunkY));
                }
            }

            return playerChunks;
        }

        private WorldChunk GetChunk(Vector2Int key)
        {
            return Chunks[key];
        }
    }
}
