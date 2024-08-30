using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.Dungeons.DungeonPopulation
{
    public class DungeonPopulater
    {
        private int[,] _datamap;
        private int MapWidth;
        private int MapHeight;

        private string[,] _popData;

        private Random _random = new Random();
        private Dictionary<string, Texture2D> _textures;

        public DungeonPopulater(int[,] datamap)
        {
            _datamap = datamap;

            MapWidth = _datamap.GetLength(0);
            MapHeight = _datamap.GetLength(1);

            LoadTextures();
        }

        private void LoadTextures()
        {
            var sheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/Dungeon/stairs");
            _textures = new Dictionary<string, Texture2D>();

            _textures.Add("stairs-nw", SpriteTools.ExtractSprite(sheet, 0, 0));
            _textures.Add("stairs-n", SpriteTools.ExtractSprite(sheet, 1, 0));
            _textures.Add("stairs-ne", SpriteTools.ExtractSprite(sheet, 2, 0));
            _textures.Add("stairs-w", SpriteTools.ExtractSprite(sheet, 0, 1));
            _textures.Add("stairs-c", SpriteTools.ExtractSprite(sheet, 1, 1));
            _textures.Add("stairs-e", SpriteTools.ExtractSprite(sheet, 2, 1));
            _textures.Add("stairs-sw", SpriteTools.ExtractSprite(sheet, 0, 2));
            _textures.Add("stairs-s", SpriteTools.ExtractSprite(sheet, 1, 2));
            _textures.Add("stairs-se", SpriteTools.ExtractSprite(sheet, 2, 2));
        }

        public void PopulateDungeon()
        {
            _popData = new string[MapWidth, MapHeight];

            // Populate the dungeon with enemies, items, etc
            PlaceStairs();
        }

        private void PlaceStairs()
        {
            var startLocation = FindSuitableStairLocation();

            _popData[startLocation.Item1 - 1, startLocation.Item2 - 1] = "stairs-nw";
            _popData[startLocation.Item1 - 0, startLocation.Item2 - 1] = "stairs-n";
            _popData[startLocation.Item1 + 1, startLocation.Item2 - 1] = "stairs-ne";

            _popData[startLocation.Item1 - 1, startLocation.Item2 - 0] = "stairs-w";
            _popData[startLocation.Item1 - 0, startLocation.Item2 - 0] = "stairs-c";
            _popData[startLocation.Item1 + 1, startLocation.Item2 - 0] = "stairs-e";

            _popData[startLocation.Item1 - 1, startLocation.Item2 + 1] = "stairs-sw";
            _popData[startLocation.Item1 - 0, startLocation.Item2 + 1] = "stairs-s";
            _popData[startLocation.Item1 + 1, startLocation.Item2 + 1] = "stairs-se";

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {

                }
            }
        }

        private (int, int) FindSuitableStairLocation()
        {
            var startLocation = (0, 0);
            var suitableLocationFound = false;

            do
            {
                var x = _random.Next(0, MapWidth);
                var y = _random.Next(0, MapHeight);

                if (_datamap[x, y] == 2)
                {
                    var surroundingGridClear = true;
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (x + i < 0 || x + i >= MapWidth || y + j < 0 || y + j >= MapHeight)
                                continue;

                            if (_datamap[x + i, y + j] != 2)
                            {
                                surroundingGridClear = false;
                            }
                        }
                    }

                    if (surroundingGridClear)
                    {
                        startLocation = (x, y);
                        suitableLocationFound = true;
                    }
                }
            }
            while (!suitableLocationFound);

            return startLocation;
        }

        public Tilemap CreateTileMap()
        {
            var width = _popData.GetLength(0);
            var height = _popData.GetLength(1);

            var tilemap = new Tilemap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (string.IsNullOrWhiteSpace(_popData[i, j]))
                    {
                        continue;
                    }

                    var tile = new Tile(Global.SpriteBatch, false)
                    {
                        ChunkX = 0,
                        ChunkY = 0,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * 32, j * 32),
                        Size = new Vector2(32, 32),
                        Texture = DetermineTexture(_popData[i, j])
                    };

                    tilemap.Map[i, j] = tile;
                }
            }

            return tilemap;
        }

        private Texture2D DetermineTexture(string popValue)
        {
            switch (popValue)
            {
                case "stairs-nw":
                    return _textures["stairs-nw"];

                case "stairs-n":
                    return _textures["stairs-n"];

                case "stairs-ne":
                    return _textures["stairs-ne"];

                case "stairs-w":
                    return _textures["stairs-w"];

                case "stairs-c":
                    return _textures["stairs-c"];

                case "stairs-e":
                    return _textures["stairs-e"];

                case "stairs-sw":
                    return _textures["stairs-sw"];

                case "stairs-s":
                    return _textures["stairs-s"];

                case "stairs-se":
                    return _textures["stairs-se"];

                default:
                    return null;
            }
        }

        public (int, int) GetStairsLocation()
        {
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    if (_popData[i, j] == "stairs-c")
                    {
                        return (i * 32, j * 32);
                    }
                }
            }

            return (0, 0); // uh oh..
        }

        public List<(int, int)> GetExitLocations()
        {
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    if (_popData[i, j] == "stairs-n")
                    {
                        return new List<(int, int)> { (i * 32, j * 32)};
                    }
                }
            }

            return new List<(int, int)> { (0, 0) }; ; // uh oh..
        }
    }
}
