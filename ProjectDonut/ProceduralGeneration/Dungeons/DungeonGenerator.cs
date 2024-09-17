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
using ProjectDonut.Core;
using ProjectDonut.Core.SpriteLibrary;
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

                    var tile = new Tile(false)
                    {
                        ChunkX = 0,
                        ChunkY = 0,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize),
                        Size = new Vector2(Global.TileSize, Global.TileSize),
                        Texture = DetermineTexture(data, i, j),
                        TileType = TileType.World,
                        DungeonTileType = DetermineTileType(data, i, j)
                    };

                    tilemap.Map[i, j] = tile;
                }
            }

            return tilemap;
        }

        private DungeonTileType DetermineTileType(int[,] map, int x, int y)
        {
            switch (map[x, y])
            {
                case 0:
                    return DungeonTileType.None;
                case 1:
                    return DungeonTileType.Wall;
                case 2:
                    return DungeonTileType.Floor;
                default:
                    return DungeonTileType.None;
            }
        }

        private Texture2D ExtractSprite(Texture2D spriteSheet, int x, int y)
        {
            var width = Global.TileSize;
            var height = Global.TileSize;

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

        public enum DungeonInteriorTileType
        {
            Empty = 0,
            Wall = 1,
            Floor = 2
        }
        private Texture2D DetermineTexture(int[,] map, int x, int y)
        {
            var width  = map.GetLength(0);
            var height = map.GetLength(1);
            var cell = (DungeonInteriorTileType)map[x, y];

            var z = GetNeighbours(map, x, y);
            var n = z["n"];
            var e = z["e"];
            var s = z["s"];
            var w = z["w"];
            var ne = z["ne"];
            var nw = z["nw"];
            var se = z["se"];
            var sw = z["sw"];

            if (cell == DungeonInteriorTileType.Wall)
            {
                // Wall internal
                if (n != DungeonInteriorTileType.Floor && e == DungeonInteriorTileType.Wall && s == DungeonInteriorTileType.Wall && w != DungeonInteriorTileType.Floor && se == DungeonInteriorTileType.Floor)
                    return GetRandomTextureFor("wall-nw");

                if (n != DungeonInteriorTileType.Floor && e == DungeonInteriorTileType.Wall && s == DungeonInteriorTileType.Floor && w == DungeonInteriorTileType.Wall) 
                    return GetRandomTextureFor("wall-n");

                if (n != DungeonInteriorTileType.Floor && e != DungeonInteriorTileType.Floor && s == DungeonInteriorTileType.Wall && w == DungeonInteriorTileType.Wall && sw == DungeonInteriorTileType.Floor) 
                    return GetRandomTextureFor("wall-ne");

                if (n == DungeonInteriorTileType.Wall && e == DungeonInteriorTileType.Floor && s == DungeonInteriorTileType.Wall && w != DungeonInteriorTileType.Floor) 
                    return GetRandomTextureFor("wall-w");

                if (n == DungeonInteriorTileType.Wall && e != DungeonInteriorTileType.Floor && s == DungeonInteriorTileType.Wall && w == DungeonInteriorTileType.Floor) 
                    return GetRandomTextureFor("wall-e");

                if (n == DungeonInteriorTileType.Wall && e == DungeonInteriorTileType.Wall && s != DungeonInteriorTileType.Floor && w != DungeonInteriorTileType.Floor && ne == DungeonInteriorTileType.Floor) 
                    return GetRandomTextureFor("wall-sw");

                if (n == DungeonInteriorTileType.Floor && e == DungeonInteriorTileType.Wall && s != DungeonInteriorTileType.Floor && w == DungeonInteriorTileType.Wall) 
                    return GetRandomTextureFor("wall-s");

                if (n == DungeonInteriorTileType.Wall && e != DungeonInteriorTileType.Floor && s != DungeonInteriorTileType.Floor && w == DungeonInteriorTileType.Wall && nw == DungeonInteriorTileType.Floor) 
                    return GetRandomTextureFor("wall-se");

                // Walls external
                if (n == DungeonInteriorTileType.Floor && e == DungeonInteriorTileType.Wall && s == DungeonInteriorTileType.Wall && w == DungeonInteriorTileType.Floor)
                    return GetRandomTextureFor("wall-ext-nw");

                if (n == DungeonInteriorTileType.Floor && e == DungeonInteriorTileType.Floor && s == DungeonInteriorTileType.Wall && w == DungeonInteriorTileType.Wall)
                    return GetRandomTextureFor("wall-ext-ne");

                if (n == DungeonInteriorTileType.Wall && e == DungeonInteriorTileType.Wall && s == DungeonInteriorTileType.Floor && w == DungeonInteriorTileType.Floor)
                    return GetRandomTextureFor("wall-ext-sw");

                if (n == DungeonInteriorTileType.Wall && e == DungeonInteriorTileType.Floor && s == DungeonInteriorTileType.Floor && w == DungeonInteriorTileType.Wall)
                    return GetRandomTextureFor("wall-ext-se");

                // No walls found, return floor
                return GetRandomTextureFor("floor-01");
            }
            else
            {
                return GetRandomTextureFor("floor-01");
            }
        }

        private Dictionary<string, DungeonInteriorTileType> GetNeighbours(int[,] map, int x, int y)
        {
            var width = map.GetLength(0);
            var height = map.GetLength(1);
            var neighbours = new Dictionary<string, DungeonInteriorTileType>();

            var nw = (DungeonInteriorTileType)0;
            var n = (DungeonInteriorTileType)0;
            var ne = (DungeonInteriorTileType)0;
            var w = (DungeonInteriorTileType)0;
            var e = (DungeonInteriorTileType)0;
            var sw = (DungeonInteriorTileType)0;
            var s = (DungeonInteriorTileType)0;
            var se = (DungeonInteriorTileType)0;

            if (y > 0 && x > 0)
            {
                nw = (DungeonInteriorTileType)map[x - 1, y - 1];
            }

            if (y > 0)
            {
                n = (DungeonInteriorTileType)map[x, y - 1];
            }

            if (y > 0 && x < width - 1)
            {
                ne = (DungeonInteriorTileType)map[x + 1, y - 1];
            }

            if (x > 0)
            {
                w = (DungeonInteriorTileType)map[x - 1, y];
            }

            if (x < width - 1)
            {
                e = (DungeonInteriorTileType)map[x + 1, y];
            }

            if (y < height - 1)
            {
                s = (DungeonInteriorTileType)map[x, y + 1];
            }

            if (y < height - 1 && x > 0)
            {
                sw = (DungeonInteriorTileType)map[x - 1, y + 1];
            }

            if (y < height - 1 && x < width - 1)
            {
                se = (DungeonInteriorTileType)map[x + 1, y + 1];
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
            var textures = SpriteLib.DungeonSprites[key];
            return textures[random.Next(textures.Count)];
        }
    }
}
