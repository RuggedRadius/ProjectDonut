using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.ProceduralGeneration.World;
using static ProjectDonut.ProceduralGeneration.Dungeons.DungeonGenerator;

namespace ProjectDonut.ProceduralGeneration.Dungeons
{
    public class DungeonGenerator
    {
        private static Random randy = new Random();

        private const int minRoomWidth = 8;
        private const int minRoomHeight = 8;

        private static List<Area> areas;
        private static List<Room> rooms2;

        private Dictionary<string, List<Texture2D>> _textures;

        public DungeonGenerator()
        {
            LoadTextures();
        }

        public Tilemap CreateTileMap(int[,] data)
        {
            var width = data.GetLength(0);
            var height = data.GetLength(1);

            var tilemap = new Tilemap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (data[i, j] == 0)
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
                        Texture = DetermineTexture(data, i, j)
                    };

                    tilemap.Map[i, j] = tile;
                }
            }

            return tilemap;
        }

        private void LoadTextures()
        {
            var sheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/Dungeon/sheet");
            //var sheet = _content.Load<Texture2D>("Sprites/Map/Dungeon/Tileset_Dungeon02");

            _textures = new Dictionary<string, List<Texture2D>>
            {
                { "wall-nw", new List<Texture2D> { ExtractSprite(sheet, 0, 0) } },
                { "wall-n", new List<Texture2D> { ExtractSprite(sheet, 1, 0) } },
                { "wall-ne", new List<Texture2D> { ExtractSprite(sheet, 2, 0) } },
                { "wall-w", new List<Texture2D> { ExtractSprite(sheet, 0, 1) } },
                { "wall-e", new List<Texture2D> { ExtractSprite(sheet, 2, 1) } },
                { "wall-sw", new List<Texture2D> { ExtractSprite(sheet, 0, 2) } },
                { "wall-s", new List<Texture2D> { ExtractSprite(sheet, 1, 2) } },
                { "wall-se", new List<Texture2D> { ExtractSprite(sheet, 2, 2) } },
                { "wall-ext-nw", new List<Texture2D> { ExtractSprite(sheet, 3, 0) } },
                { "wall-ext-ne", new List<Texture2D> { ExtractSprite(sheet, 4, 0) } },
                { "wall-ext-sw", new List<Texture2D> { ExtractSprite(sheet, 3, 1) } },
                { "wall-ext-se", new List<Texture2D> { ExtractSprite(sheet, 4, 1) } },
                { "floor-01", new List<Texture2D>() }
            };

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    _textures["floor-01"].Add(ExtractSprite(sheet, i + 6, j));
                }
            }
        }

        private Texture2D ExtractSprite(Texture2D spriteSheet, int x, int y)
        {
            var width = 32;
            var height = 32;

            x *= width;
            y *= height;

            Rectangle sourceRectangle = new Rectangle(x, y, width, height);

            // Extract the pixel data from the spritesheet
            Color[] data = new Color[width * height];


            SynchronizationContext.Current.Post(_ =>
            {
                spriteSheet.GetData(0, sourceRectangle, data, 0, data.Length);
            }, null);

            // Create a new texture for the sprite and set the pixel data
            Texture2D sprite = new Texture2D(Global.GraphicsDevice, width, height);
            sprite.SetData(data);

            return sprite;
        }

        public enum TileType
        {
            Empty = 0,
            Wall = 1,
            Floor = 2
        }
        private Texture2D DetermineTexture(int[,] map, int x, int y)
        {
            var width  = map.GetLength(0);
            var height = map.GetLength(1);
            var cell = (TileType)map[x, y];

            var z = GetNeighbours(map, x, y);
            var n = z["n"];
            var e = z["e"];
            var s = z["s"];
            var w = z["w"];
            var ne = z["ne"];
            var nw = z["nw"];
            var se = z["se"];
            var sw = z["sw"];

            if (cell == TileType.Wall)
            {
                // Wall internal
                if (n != TileType.Floor && e == TileType.Wall && s == TileType.Wall && w != TileType.Floor && se == TileType.Floor)
                    return GetRandomTextureFor("wall-nw");

                if (n != TileType.Floor && e == TileType.Wall && s == TileType.Floor && w == TileType.Wall) 
                    return GetRandomTextureFor("wall-n");

                if (n != TileType.Floor && e != TileType.Floor && s == TileType.Wall && w == TileType.Wall && sw == TileType.Floor) 
                    return GetRandomTextureFor("wall-ne");

                if (n == TileType.Wall && e == TileType.Floor && s == TileType.Wall && w != TileType.Floor) 
                    return GetRandomTextureFor("wall-w");

                if (n == TileType.Wall && e != TileType.Floor && s == TileType.Wall && w == TileType.Floor) 
                    return GetRandomTextureFor("wall-e");

                if (n == TileType.Wall && e == TileType.Wall && s != TileType.Floor && w != TileType.Floor && ne == TileType.Floor) 
                    return GetRandomTextureFor("wall-sw");

                if (n == TileType.Floor && e == TileType.Wall && s != TileType.Floor && w == TileType.Wall) 
                    return GetRandomTextureFor("wall-s");

                if (n == TileType.Wall && e != TileType.Floor && s != TileType.Floor && w == TileType.Wall && nw == TileType.Floor) 
                    return GetRandomTextureFor("wall-se");

                // Walls external
                if (n == TileType.Floor && e == TileType.Wall && s == TileType.Wall && w == TileType.Floor)
                    return GetRandomTextureFor("wall-ext-nw");

                if (n == TileType.Floor && e == TileType.Floor && s == TileType.Wall && w == TileType.Wall)
                    return GetRandomTextureFor("wall-ext-ne");

                if (n == TileType.Wall && e == TileType.Wall && s == TileType.Floor && w == TileType.Floor)
                    return GetRandomTextureFor("wall-ext-sw");

                if (n == TileType.Wall && e == TileType.Floor && s == TileType.Floor && w == TileType.Wall)
                    return GetRandomTextureFor("wall-ext-se");

                // No walls found, return floor
                return GetRandomTextureFor("floor-01");
            }
            else
            {
                return GetRandomTextureFor("floor-01");
            }
        }

        private Dictionary<string, TileType> GetNeighbours(int[,] map, int x, int y)
        {
            var width = map.GetLength(0);
            var height = map.GetLength(1);
            var neighbours = new Dictionary<string, TileType>();

            var nw = (TileType)0;
            var n = (TileType)0;
            var ne = (TileType)0;
            var w = (TileType)0;
            var e = (TileType)0;
            var sw = (TileType)0;
            var s = (TileType)0;
            var se = (TileType)0;

            if (y > 0 && x > 0)
            {
                nw = (TileType)map[x - 1, y - 1];
            }

            if (y > 0)
            {
                n = (TileType)map[x, y - 1];
            }

            if (y > 0 && x < width - 1)
            {
                ne = (TileType)map[x + 1, y - 1];
            }

            if (x > 0)
            {
                w = (TileType)map[x - 1, y];
            }

            if (x < width - 1)
            {
                e = (TileType)map[x + 1, y];
            }

            if (y < height - 1)
            {
                s = (TileType)map[x, y + 1];
            }

            if (y < height - 1 && x > 0)
            {
                sw = (TileType)map[x - 1, y + 1];
            }

            if (y < height - 1 && x < width - 1)
            {
                se = (TileType)map[x + 1, y + 1];
            }

            neighbours.Add("nw", nw);
            neighbours.Add("n", n);
            neighbours.Add("ne", ne);
            neighbours.Add("w", w);
            neighbours.Add("e", e);
            neighbours.Add("sw", sw);
            neighbours.Add("s", s);
            neighbours.Add("se", se);

            return neighbours;
        }

        private Texture2D GetRandomTextureFor(string key)
        {
            var random = new Random();
            var textures = _textures[key];
            return textures[random.Next(textures.Count)];
        }
    }
}
