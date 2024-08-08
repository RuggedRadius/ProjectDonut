using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class WorldGenerator
    {
        private int[,] biomeData;

        private int[,] heightData;
        private Tilemap tmBase;
        
        private int[,] forestData;
        private Tilemap tmForest;

        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        private WorldTileRuler rules;
        private SpriteLibrary spriteLib;

        private WorldMapSettings settings;

        // Generators
        private BaseGenerator baseGen;
        private BiomeGenerator biomes;
        private RiverGenerator rivers;
        private ForestGenerator forest;

        public WorldGenerator(ContentManager content, GraphicsDevice graphicsDevice, WorldMapSettings settings)
        {
            this.content = content;
            this.graphicsDevice = graphicsDevice;
            this.spriteLib = new SpriteLibrary(content, graphicsDevice);
            this.settings = settings;

            baseGen = new BaseGenerator(settings, spriteLib);
            biomes = new BiomeGenerator();
            rivers = new RiverGenerator(settings);
            forest = new ForestGenerator(spriteLib, settings);

            rules = new WorldTileRuler(spriteLib);

            spriteLib.LoadSpriteLibrary();
        }

        public Tilemap GenerateBaseMap(int width, int height)
        {
            biomeData = biomes.GenerateBiomes(width, height);
            heightData = baseGen.GenerateHeightMap(width, height);
            //WriteMapToFile(heightData, @"C:\map1.txt");
            heightData = rivers.CarveRivers(heightData);
            //WriteMapToFile(heightData, @"C:\map2.txt");

            tmBase = baseGen.CreateBaseTilemap(heightData, biomeData);
            tmBase = rules.ApplyBaseRules(tmBase);

            return tmBase;
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

            tmForest = forest.CreateForestTilemap(forestData);
            tmForest = rules.ApplyForestRules(tmForest);

            return tmForest;
        }
    }
}
