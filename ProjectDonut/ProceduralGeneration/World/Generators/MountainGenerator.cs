using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Core.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.World.Generators
{
    public class MountainGenerator
    {
        private WorldMapSettings settings;
        private FastNoiseLite[] _noise;

        private float OctaveBlend = 0.125f;

        public MountainGenerator(WorldMapSettings settings)
        {
            this.settings = settings;

            _noise = new FastNoiseLite[2];
            _noise[0] = new FastNoiseLite();
            //_noise[0].SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            _noise[0].SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            _noise[0].SetSeed(new Random().Next(int.MinValue, int.MaxValue));

            _noise[1] = new FastNoiseLite();
            _noise[1].SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            //_noise[1].SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            _noise[1].SetSeed(new Random().Next(int.MinValue, int.MaxValue));
            _noise[1].SetFrequency(0.5f);
            _noise[1].SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Manhattan);

        }

        public Tilemap CreateTilemap(WorldChunk chunk)
        {
            var tmBase = new Tilemap(chunk.Width, chunk.Height);

            for (int i = 0; i < chunk.Width; i++)
            {
                for (int j = 0; j < chunk.Height; j++)
                {
                    var biomeValue = chunk.BiomeData[i, j];
                    var heightValue = chunk.HeightData[i, j];

                    if (heightValue < settings.GroundHeightMax)
                    {
                        continue;
                    }

                    var tile = new Tile(false)
                    {
                        ChunkX = chunk.ChunkCoordX,
                        ChunkY = chunk.ChunkCoordY,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * settings.TileSize, j * settings.TileSize),
                        Size = new Vector2(settings.TileSize, settings.TileSize),
                        Texture = SpriteLib.GetSprite("mountain"),
                        TileType = TileType.World,
                        WorldTileType = WorldTileType.Mountain,
                        Biome = (Biome)chunk.BiomeData[i, j]
                    };

                    tmBase.Map[i, j] = tile;
                }
            }

            return tmBase;
        }

        // TODO: Make mountains for each biome
        //private Texture2D DetermineTexture(int x, int y, int biomeValue, int heightValue)
        //{
        //    return SpriteLib.GetSprite("mountain");
        //}

    }
}
