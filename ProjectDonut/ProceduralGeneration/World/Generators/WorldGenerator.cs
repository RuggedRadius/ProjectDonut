﻿using System;
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

        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        private WorldTileRuler rules;
        private SpriteLibrary spriteLib;
        private SpriteBatch _spriteBatch;

        private WorldMapSettings settings;

        // Generators
        private HeightGenerator baseGen;
        private BiomeGenerator biomes;
        private WaterGenerator water;
        private ForestGenerator forest;

        public WorldGenerator(ContentManager content, GraphicsDevice graphicsDevice, WorldMapSettings settings, SpriteLibrary spriteLib, SpriteBatch spriteBatch)
        {
            this.content = content;
            this.graphicsDevice = graphicsDevice;
            this.spriteLib = spriteLib;
            this.settings = settings;

            baseGen = new HeightGenerator(settings, spriteLib, spriteBatch);
            biomes = new BiomeGenerator(settings);
            water = new WaterGenerator(settings);
            forest = new ForestGenerator(spriteLib, settings, spriteBatch);

            rules = new WorldTileRuler(spriteLib);

            //spriteLib.LoadSpriteLibrary();
            _spriteBatch = spriteBatch;
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
