﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDonut.ProceduralGeneration.World
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

        private WorldMapSettings settings;

        // Generators
        private HeightMapGenerator heightMapGen;
        private BiomeMapGenerator biomeMapGen;
        private WaterGenerator water;
        private ForestGenerator forest;

        public WorldGenerator(ContentManager content, GraphicsDevice graphicsDevice, WorldMapSettings settings, SpriteLibrary spriteLib)
        {
            this.content = content;
            this.graphicsDevice = graphicsDevice;
            this.spriteLib = spriteLib;
            this.settings = settings;

            heightMapGen = new HeightMapGenerator(settings, spriteLib);
            biomeMapGen = new BiomeMapGenerator(settings);
            water = new WaterGenerator(settings);
            forest = new ForestGenerator(spriteLib, settings);

            rules = new WorldTileRuler(spriteLib);

            spriteLib.LoadSpriteLibrary();
        }

        private int[,] TEMPCreateDummyBiomeData(int width, int height)
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

        public Tilemap GenerateBaseMap(int width, int height, int xOffset, int yOffset)
        {
            // TEMP
            biomeData = TEMPCreateDummyBiomeData(width, height);

            //biomeData = biomeMapGen.GenerateBiomes(width, height, xOffset, yOffset);
            //foreach (Biome biome in Enum.GetValues(typeof(Biome)))
            //{
            //    biomeData = water.ErodeBiomeBorder(biome, biomeData);
            //}

            heightData = heightMapGen.GenerateHeightMap(width, height, xOffset, yOffset);

            //heightData = water.ErodeMountains(heightData);
            //heightData = water.CarveRivers(heightData);
            //heightData = water.ErodeCoast(heightData);
            //heightData = water.ErodeDeepWater(heightData);

            tmBase = heightMapGen.CreateBaseTilemap(heightData, biomeData);
            tmBase = rules.ApplyBaseRules(tmBase);

            // TEMP
            tmBase = TEMPBorderAroundChunk(tmBase);

            return tmBase;
        }

        private Tilemap TEMPBorderAroundChunk(Tilemap map)
        {
            for (int x = 0; x < settings.Width; x++)
            {
                for (int y = 0; y < settings.Height; y++)
                {
                    if (x == 0 || x == settings.Width - 1)
                    {
                        map.Map[x, y] = new Tile()
                        {
                            Texture = spriteLib.GetSprite("mountain")
                        };
                    }

                    if (y == 0 || y == settings.Height - 1)
                    {
                        map.Map[x, y] = new Tile()
                        {
                            Texture = spriteLib.GetSprite("mountain")
                        };
                    }
                }
            }

            return map;
        }

        private void WriteMapToFile(int[,] map, string filePath)
        {
            var lines = new List<string>();

            for (int y = 0; y < map.GetLength(1); y++)
            {
                var line = "";
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    line += map[x, y] + " ";
                }
                lines.Add(line);
            }

            File.WriteAllLines(filePath, lines);
        }

        public Tilemap GenerateForestMap(int width, int height)
        {
            forestData = forest.GenerateForestData(heightData, biomeData);

            tmForest = forest.CreateForestTilemap(forestData, biomeData);
            tmForest = rules.ApplyForestRules(tmForest);

            return tmForest;
        }
    }
}
