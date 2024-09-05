using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;

namespace ProjectDonut.ProceduralGeneration.World.Generators
{
    public class WorldGenerator
    {
        private int[,] biomeData;

        public int[,] heightData;
        private Tilemap tmBase;

        private int[,] forestData;
        private Tilemap tmForest;

        private WorldTileRuler rules;

        private WorldMapSettings settings;

        // Generators
        private HeightGenerator baseGen;
        private BiomeGenerator biomes;
        private WaterGenerator water;
        private ForestGenerator forest;

        public WorldGenerator(WorldMapSettings settings)
        {
            this.settings = settings;

            baseGen = new HeightGenerator(settings);
            biomes = new BiomeGenerator(settings);
            water = new WaterGenerator(settings);
            forest = new ForestGenerator(settings);

            rules = new WorldTileRuler();
        }

        public int[,] TEMPCreateDummyBiomeData(int width, int height)
        {
            var data = new int[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    data[i, j] = 0;
                }
            }

            return data;
        }
    }
}
