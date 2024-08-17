﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class BaseGenerator
    {
        private WorldMapSettings settings;
        private SpriteLibrary spriteLib;
        private FastNoiseLite _noise;

        public BaseGenerator(WorldMapSettings settings, SpriteLibrary spriteLib)
        {
            this.settings = settings;
            this.spriteLib = spriteLib;

            this._noise = new FastNoiseLite();
            //noise.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            _noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

            //noise.SetSeed(new Random().Next(int.MinValue, int.MaxValue));
            _noise.SetSeed(1337);
        }

        public int[,] GenerateHeightMap(int width, int height, int xOffset, int yOffset)
        {
            // Gather noise data
            int[,] heightData = new int[height, width];
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    heightData[i, j] = (int)(_noise.GetNoise((xOffset * settings.Width) + i, (yOffset * settings.Height) + j) * 10);
                    //heightData[i, j] = (int)(_noise.GetNoise(xOffset + i, yOffset + j));
                }
            }

            //// Normalize and convert to integer
            //int[,] intData = new int[height, width];
            //float range = maxValue - minValue;

            //for (int x = 0; x < width; x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {
            //        // Normalize value to the range [0, 1]
            //        float normalizedValue = (heightData[x, y] - minValue) / range;

            //        // Scale to integer range (e.g., 0 to 255)
            //        intData[x, y] = (int)(normalizedValue * 9);
            //    }
            //}
            
            return heightData;
            //return intData;
        }

        public Tilemap CreateBaseTilemap(int[,] heightData, int[,] biomeData)
        {
            var tmBase = new Tilemap(heightData.GetLength(0), heightData.GetLength(1));

            for (int i = 0; i < heightData.GetLength(0); i++)
            {
                for (int j = 0; j < heightData.GetLength(1); j++)
                {
                    var biomeValue = biomeData[i, j];
                    var heightValue = heightData[i, j];

                    var tile = new Tile
                    {
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * settings.TileSize, j * settings.TileSize),
                        Size = new Vector2(settings.TileSize, settings.TileSize),
                        Texture = DetermineTexture(i, j, biomeValue, heightValue),
                        TileType = DetermineTileType(i, j, heightValue),
                        Biome = (Biome)biomeData[i, j]
                    };

                    tmBase.Map[i, j] = tile;
                }
            }

            return tmBase;
        }

        private Texture2D DetermineTexture(int x, int y, int biomeValue, int heightValue)
        {
            var biome = (Biome)biomeValue;

            if (heightValue >= settings.MountainHeightMin)
            {
                return spriteLib.GetSprite("mountain");
            }
            else if (heightValue >= settings.GroundHeightMin)
            {
                switch (biome)
                {
                    case Biome.Desert:
                        return spriteLib.GetSprite("desert");

                    case Biome.Grasslands:
                        return spriteLib.GetSprite("grasslands");

                    case Biome.Winterlands:
                        return spriteLib.GetSprite("winterlands");

                    default:
                        return spriteLib.GetSprite("grasslands");
                }
            }
            else
            {
                if (heightValue >= settings.WaterHeightMin)
                {
                    return spriteLib.GetSprite("coast-inv");
                }
                else
                {
                    return spriteLib.GetSprite("deepwater-C");
                }
            }
        }

        private TileType DetermineTileType(int x, int y, int heightValue)
        {
            if (heightValue >= settings.MountainHeightMin)
            {
                return TileType.Mountain;
            }
            else if (heightValue >= settings.GroundHeightMin)
            {
                return TileType.Ground;
            }
            else
            {
                return TileType.Water;
            }
        }
    }
}
