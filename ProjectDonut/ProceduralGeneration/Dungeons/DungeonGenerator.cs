using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


            //_textures.Add("wall-nw", new List<Texture2D> { ExtractSprite(sheet, 0, 0) });
            //_textures.Add("wall-n", new List<Texture2D> { ExtractSprite(sheet, 1, 0) });
            //_textures.Add("wall-ne", new List<Texture2D> { ExtractSprite(sheet, 2, 0) });
            //_textures.Add("wall-w", new List<Texture2D> { ExtractSprite(sheet, 0, 1) });
            //_textures.Add("wall-e", new List<Texture2D> { ExtractSprite(sheet, 2, 1) });
            //_textures.Add("wall-sw", new List<Texture2D> { ExtractSprite(sheet, 0, 2) });
            //_textures.Add("wall-s", new List<Texture2D> { ExtractSprite(sheet, 1, 2) });
            //_textures.Add("wall-se", new List<Texture2D> { ExtractSprite(sheet, 2, 2) });

            //_textures.Add("wall-ext-nw", new List<Texture2D> { ExtractSprite(sheet, 5, 1) });
            //_textures.Add("wall-ext-ne", new List<Texture2D> { ExtractSprite(sheet, 6, 1) });
            //_textures.Add("wall-ext-sw", new List<Texture2D> { ExtractSprite(sheet, 5, 2) });
            //_textures.Add("wall-ext-se", new List<Texture2D> { ExtractSprite(sheet, 6, 2) });

            //_textures.Add("floor-01", new List<Texture2D> { ExtractSprite(sheet, 1, 1) });


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
            spriteSheet.GetData(0, sourceRectangle, data, 0, data.Length);

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


        //public enum TileType
        //{
        //    Empty = 0,
        //    Wall = 1
        //}

        //public enum WallSprite
        //{
        //    None = 0,     // 0000
        //    North = 1,    // 0001
        //    East = 2,     // 0010
        //    South = 4,    // 0100
        //    West = 8,     // 1000
        //    NorthEast = North | East,    // 0011
        //    NorthWest = North | West,    // 1001
        //    SouthEast = South | East,    // 0110
        //    SouthWest = South | West,    // 1100
        //    AllSides = North | East | South | West  // 1111
        //}

        //public WallSprite[,] DetermineWallSprites(int[,] data)
        //{
        //    int rows = data.GetLength(0);
        //    int cols = data.GetLength(1);
        //    WallSprite[,] spriteMap = new WallSprite[rows, cols];

        //    for (int row = 0; row < rows; row++)
        //    {
        //        for (int col = 0; col < cols; col++)
        //        {
        //            if (data[row, col] == 0)
        //            {
        //                spriteMap[row, col] = WallSprite.None;
        //                continue;
        //            }

        //            int bitmask = 0;

        //            if (row > 0 && data[row - 1, col] == 1) // North
        //                bitmask |= (int)WallSprite.North;

        //            if (col < cols - 1 && data[row, col + 1] == 1) // East
        //                bitmask |= (int)WallSprite.East;

        //            if (row < rows - 1 && data[row + 1, col] == 1) // South
        //                bitmask |= (int)WallSprite.South;

        //            if (col > 0 && data[row, col - 1] == 1) // West
        //                bitmask |= (int)WallSprite.West;

        //            spriteMap[row, col] = (WallSprite)bitmask;
        //        }
        //    }

        //    return spriteMap;
        //}



        //public static char[,] Generate(int width, int height, int minAreaSize)
        //{
        //    var startingArea = new Area(0, 0, width, height, null, minRoomWidth, minRoomHeight);
        //    areas = new List<Area> { startingArea };

        //    while (areas.Any(a => a.IsPartitionable))
        //    {
        //        var areaToPartition = areas.FirstOrDefault(a => a.IsPartitionable);
        //        areas.Remove(areaToPartition);
        //        areas.AddRange(PartitionArea(areaToPartition));
        //    }

        //    ReplaceAreasWithRooms(areas);
        //    var map = CompileTo2DArray(areas, width, height);
        //    map = LinkBrotherRooms(areas, map);
        //    return map;
        //}

        //private static char[,] LinkBrotherRooms(List<Area> areas, char[,] map)
        //{
        //    while (areas.Count > 0)
        //    {
        //        var area = areas.First();
        //        areas.Remove(area);


        //        var brother = area.brother;

        //        if (brother != null)
        //        {
        //            areas.Remove(area.brother);

        //            var x1 = randy.Next(area.xBottom, area.xTop);
        //            var y1 = randy.Next(area.yBottom, area.yTop);

        //            var x2 = randy.Next(brother.xBottom, brother.xTop);
        //            var y2 = randy.Next(brother.yBottom, brother.yTop);

        //            map = DrawCorridor(x1, y1, x2, y2, map);
        //        }


        //    }

        //    return map;
        //}

        //private static char[,] DrawCorridor(int x1, int y1, int x2, int y2, char[,] map)
        //{
        //    // Check if the coordinates are within the bounds of the map
        //    if (x1 < 0 || x1 >= map.GetLength(0) || y1 < 0 || y1 >= map.GetLength(1) ||
        //        x2 < 0 || x2 >= map.GetLength(0) || y2 < 0 || y2 >= map.GetLength(1))
        //    {
        //        throw new ArgumentException("Invalid coordinates");
        //    }

        //    // Draw a corridor between the two points using the ASCII character '|'
        //    if (x1 == x2)
        //    {
        //        // Vertical corridor
        //        int minY = Math.Min(y1, y2);
        //        int maxY = Math.Max(y1, y2);

        //        for (int y = minY; y <= maxY; y++)
        //        {
        //            map[x1, y] = '#';
        //        }
        //    }
        //    else if (y1 == y2)
        //    {
        //        // Horizontal corridor
        //        int minX = Math.Min(x1, x2);
        //        int maxX = Math.Max(x1, x2);

        //        for (int x = minX; x <= maxX; x++)
        //        {
        //            map[x, y1] = '#';
        //        }
        //    }
        //    else
        //    {
        //        int minY = Math.Min(y1, y2);
        //        int maxY = Math.Max(y1, y2);
        //        int minX = Math.Min(x1, x2);
        //        int maxX = Math.Max(x1, x2);

        //        for (int x = minX; x <= maxX; x++)
        //        {
        //            map[x, y1] = '#';
        //        }

        //        for (int y = minY; y <= maxY; y++)
        //        {
        //            map[x1, y] = '#';
        //        }
        //    }

        //    return map;
        //}


        //private static void ReplaceAreasWithRooms(List<Area> areas)
        //{
        //    DungeonGenerator.areas = new List<Area>();

        //    foreach (var area in areas)
        //    {
        //        if (area.Width > minRoomWidth && area.Height > minRoomHeight)
        //        {
        //            DungeonGenerator.areas.Add(CreateRoomInArea(area));
        //        }
        //    }
        //}

        //private static Area CreateRoomInArea(Area area)
        //{
        //    var startX = randy.Next(area.xBottom, area.xTop - minRoomWidth);
        //    var startY = randy.Next(area.yBottom, area.yTop - minRoomHeight);

        //    if (startX <= 0 || startY <= 0)
        //    {
        //        return area;
        //    }

        //    var endX = randy.Next(startX + minRoomWidth, area.xTop);
        //    var endY = randy.Next(startY + minRoomHeight, area.yTop);

        //    return new Area(startX, startY, endX, endY, area.brother, minRoomWidth, minRoomHeight);
        //}

        //private static char[,] CompileTo2DArray(List<Area> areas, int initialWidth, int initialHeight)
        //{
        //    var map = new char[initialWidth, initialHeight];

        //    for (int i = 0; i < initialWidth; i++)
        //    {
        //        for (int j = 0; j < initialHeight; j++)
        //        {
        //            map[i, j] = '_';
        //        }
        //    }

        //    int charCount = 65;

        //    foreach (var area in areas)
        //    {
        //        for (int i = area.xBottom; i < area.xTop; i++)
        //        {
        //            for (int j = area.yBottom; j < area.yTop; j++)
        //            {
        //                map[i, j] = (char)charCount;
        //            }
        //        }

        //        charCount++;
        //    }

        //    return map;
        //}

        //private static List<Area> PartitionArea(Area area)
        //{
        //    var results = new List<Area>();

        //    if (area.Width > area.Height)
        //    {
        //        var partitionPoint = randy.Next(area.xBottom + minRoomWidth, area.xTop - minRoomWidth);

        //        var leftArea = new Area(area.xBottom, area.yBottom, partitionPoint, area.yTop, null, minRoomWidth, minRoomHeight);
        //        var rightArea = new Area(partitionPoint, area.yBottom, area.xTop, area.yTop, leftArea, minRoomWidth, minRoomHeight);
        //        leftArea.brother = rightArea;

        //        results.Add(leftArea);
        //        results.Add(rightArea);
        //    }
        //    else
        //    {
        //        var partitionPoint = randy.Next(area.yBottom + minRoomHeight, area.yTop - minRoomHeight);

        //        var topArea = new Area(area.xBottom, partitionPoint, area.xTop, area.yTop, null, minRoomWidth, minRoomHeight);
        //        var bottomArea = new Area(area.xBottom, area.yBottom, area.xTop, partitionPoint, topArea, minRoomWidth, minRoomHeight);
        //        topArea.brother = bottomArea;

        //        results.Add(topArea);
        //        results.Add(bottomArea);
        //    }

        //    return results;
        //}
    }
}
