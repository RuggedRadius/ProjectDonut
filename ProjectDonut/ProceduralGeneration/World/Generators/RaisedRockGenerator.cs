using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectDonut.Core.Sprites;

namespace ProjectDonut.ProceduralGeneration.World.Generators
{
    public class RaisedRockGenerator
    {
        private WorldMapSettings settings;
        private FastNoiseLite _noise;
        private Random _random = new Random();

        public RaisedRockGenerator(WorldMapSettings settings)
        {
            this.settings = settings;

            _noise = new FastNoiseLite();
            _noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            _noise.SetSeed(_random.Next(int.MinValue, int.MaxValue));
            _noise.SetFrequency(0.02f);
        }

        public int[,] GenerateHeightMap(int width, int height, int xOffset, int yOffset)
        {
            int[,] heightData = new int[height, width];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var x = xOffset * settings.Width + i;
                    var y = yOffset * settings.Height + j;

                    var heightValue = (int)(_noise.GetNoise(x, y) * Global.ChunkSize) + 35;

                    heightData[i, j] = heightValue;
                }
            }

            return heightData;
        }

        public Tilemap CreateTerrainTilemap(WorldChunk chunk)
        {
            var tmBase = new Tilemap(chunk.Width, chunk.Height);

            for (int i = 0; i < chunk.Width; i++)
            {
                for (int j = 0; j < chunk.Height; j++)
                {
                    var rockValue = chunk.RaisedRockData[i, j];

                    if (chunk.RaisedRockData[i, j] < 70)
                    {
                        continue;
                    }

                    if (chunk.Tilemaps["base"].Map[i, j].WorldTileType == WorldTileType.Water)
                    {
                        continue;
                    }

                    var tile = new Tile()
                    {
                        ChunkX = chunk.ChunkCoordX,
                        ChunkY = chunk.ChunkCoordY,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * settings.TileSize, j * settings.TileSize),
                        Size = new Vector2(settings.TileSize, settings.TileSize),
                        Texture = SpriteLib.World.RaisedRock["s"],
                        TileType = TileType.World,
                        WorldTileType = WorldTileType.Ground,
                        Biome = (Biome)chunk.BiomeData[i, j],
                        IsCollidable = true
                    };
                    
                    tile.Initialize();
                    tmBase.Map[i, j] = tile;
                }
            }

            return tmBase;
        }
    }
}
