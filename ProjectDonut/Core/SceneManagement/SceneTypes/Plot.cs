using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.BSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Core.SceneManagement.SceneTypes
{
    public class Plot : IGameObject
    {
        public Room Area { get; set; }
        public TownScene Town { get; set; }
        public Vector2 WorldPosition { get; set; }

        // Plot
        public int[,] PlotMap { get; set; }
        public int[,] FenceMap { get; set; }

        /*
         * 0 = Ground
         * 1 = Fence
         * 2 = House floor
         * 3 = House wall
         *      4 = 2nd storey floor?
         *      5 = 2nd storey walls?
         * Last = House roof
         */
        public List<Tilemap> _tilemaps { get; set; }

        // House
        public List<int[,]> HouseMap { get; set; }
        public Rectangle HouseBounds { get; set; }
        private int _baseWidth;
        private int _baseHeight;

        private BSP _bsp;
        private Random _random;



        

        // Shouldnt need these...
        public int ZIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsVisible => throw new NotImplementedException();
        public Texture2D Texture { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }








        public Plot(Room area, TownScene town)
        {
            Area = area;
            Town = town;

            _bsp = new BSP();
            _tilemaps = new List<Tilemap>();
            _random = new Random();
        }

        public void Build()
        {
            GenerateDataMaps();


            _tilemaps.Add(GenerateGroundTilemap(PlotMap));
            _tilemaps.Add(GenerateFenceTilemap(FenceMap));
            _tilemaps.Add(GenerateHouseFloorMap(HouseMap[0]));
            _tilemaps.Add(GenerateHouseWallMap(HouseMap[1]));
            _tilemaps.Add(GenerateHouseRoofMap(HouseMap[2]));
        }

        public void BuildHouse()
        {
            // Generate house map data
            // ...

            _baseWidth = 12; // set these to data map values
            _baseHeight = 10; // set these to data map values

            HouseBounds = new Rectangle(
                Area.Bounds.X + 6,
                Area.Bounds.Y + 6,
                _baseWidth * Global.TileSize,
                _baseHeight * Global.TileSize);
        }


        public void GenerateDataMaps()
        {
            var width = (int)Area.Bounds.Width;
            var height = (int)Area.Bounds.Height;

            // Ground
            PlotMap = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (IsNeighbourCellRoad(PlotMap, i, j))
                    {
                        PlotMap[i, j] = 3;
                    }
                    else
                    {
                        PlotMap[i, j] = 2;
                    }                    
                }
            }

            // Fence
            FenceMap = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i == 0 || i == width - 1 || j == 0 || j == height - 1)
                    {
                        FenceMap[i, j] = 1;
                    }
                    else
                    {
                        FenceMap[i, j] = 0;
                    }
                }
            }

            // House
            var houseWidth = 12;
            var houseHeight = 8;

            var widthOffset = _random.Next(1, (width - houseWidth) - 1);
            var heightOffset = _random.Next(1, (height - houseHeight - 1));

            HouseBounds = new Rectangle(
                (int)(WorldPosition.X + widthOffset * Global.TileSize),
                (int)(WorldPosition.Y + heightOffset * Global.TileSize),
                houseWidth * Global.TileSize,
                houseHeight * Global.TileSize);

            HouseMap = new List<int[,]>();
            var floorMap = new int[width, height];
            for (int i = 0; i < houseWidth; i++)
            {
                for (int j = 0; j < houseHeight; j++)
                {
                    floorMap[i + widthOffset, j + heightOffset] = 1;
                }
            }

            var wall1Map = new int[width, height];
            for (int i = 0; i < houseWidth; i++)
            {
                for (int j = 0; j < houseHeight; j++)
                {
                    if (i == 0 || i == houseWidth - 1 || j == 0 || j == houseHeight - 1)
                        wall1Map[i + widthOffset, j + heightOffset] = 1;
                    else
                        wall1Map[i + widthOffset, j + heightOffset] = 0;
                }
            }

            var roofMap = new int[width, height];
            for (int i = 0; i < houseWidth; i++)
            {
                for (int j = 0; j < houseHeight; j++)
                {
                    roofMap[i + widthOffset, j + heightOffset] = 1;
                }
            }

            HouseMap.Add(floorMap);
            HouseMap.Add(wall1Map);
            HouseMap.Add(roofMap);
        }

        private Tilemap GenerateGroundTilemap(int[,] map)
        {
            var tm = new Tilemap(map.GetLength(0), map.GetLength(1));

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 0)
                    {
                        continue;
                    }

                    var tile = new Tile(false)
                    {
                        ChunkX = 0,
                        ChunkY = 0,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2((i * Global.TileSize), j * Global.TileSize) + WorldPosition,
                        Size = new Vector2(Global.TileSize, Global.TileSize),
                        Texture = Global.SpriteLibrary.TownSprites["grass-c"],
                        TileType = TileType.Instance,
                        IsExplored = true
                    };

                    tm.Map[i, j] = tile;
                }
            }

            return tm;
        }

        private Tilemap GenerateFenceTilemap(int[,] map)
        {
            var tm = new Tilemap(map.GetLength(0), map.GetLength(1));

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 0)
                    {
                        continue;
                    }

                    var tile = new Tile(false)
                    {
                        ChunkX = 0,
                        ChunkY = 0,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize) + WorldPosition,
                        Size = new Vector2(Global.TileSize, Global.TileSize),
                        Texture = DetermineFenceTexture(map, i, j),
                        TileType = TileType.Instance,
                        IsExplored = true
                    };

                    tm.Map[i, j] = tile;
                }
            }

            return tm;
        }

        private Tilemap GenerateHouseFloorMap(int[,] map)
        {
            var tm = new Tilemap(map.GetLength(0), map.GetLength(1));

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 0)
                    {
                        continue;
                    }

                    var tile = new Tile(false)
                    {
                        ChunkX = 0,
                        ChunkY = 0,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize) + WorldPosition,
                        Size = new Vector2(Global.TileSize, Global.TileSize),
                        Texture = Global.SpriteLibrary.BuildingBlockSprites["building-floor"],
                        TileType = TileType.Instance,
                        IsExplored = true
                    };

                    tm.Map[i, j] = tile;
                }
            }

            return tm;
        }

        private Tilemap GenerateHouseWallMap(int[,] map)
        {
            var tm = new Tilemap(map.GetLength(0), map.GetLength(1));

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 0)
                    {
                        continue;
                    }

                    var tile = new Tile(false)
                    {
                        ChunkX = 0,
                        ChunkY = 0,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize) + WorldPosition,
                        Size = new Vector2(Global.TileSize, Global.TileSize),
                        Texture = DetermineWallTexture(map, i, j),
                        TileType = TileType.Instance,
                        IsExplored = true
                    };

                    tm.Map[i, j] = tile;
                }
            }

            return tm;
        }

        private Tilemap GenerateHouseRoofMap(int[,] map)
        {
            var tm = new Tilemap(map.GetLength(0), map.GetLength(1));

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 0)
                    {
                        continue;
                    }

                    var tile = new Tile(false)
                    {
                        ChunkX = 0,
                        ChunkY = 0,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize) + WorldPosition,
                        Size = new Vector2(Global.TileSize, Global.TileSize),
                        Texture = Global.SpriteLibrary.BuildingBlockSprites["building-roof-thatching"],
                        TileType = TileType.Instance,
                        IsExplored = true
                    };

                    tm.Map[i, j] = tile;
                }
            }

            return tm;
        }

        private Texture2D DetermineFenceTexture(int[,] map, int x, int y)
        {
            var n = false;
            var e = false;
            var s = false;
            var w = false;

            if (y > 0)
                n = map[x, y - 1] == 1;
            if (x > 0)
                w = map[x - 1, y] == 1;
            if (x < map.GetLength(0) - 1)
                e = map[x + 1, y] == 1;
            if (y < map.GetLength(1) - 1)
                s = map[x, y + 1] == 1;

            if (!n && e && s && !w)
                return Global.SpriteLibrary.TownSprites["fence-nw"];
            if (!n && e && !s && w)
            {
                if (y == 0)
                    return Global.SpriteLibrary.TownSprites["fence-n"];
                //else if (map[x, y - 1] == 1)
                //    return Global.SpriteLibrary.TownSprites["fence-s"];
                else
                    return Global.SpriteLibrary.TownSprites["fence-s"];
            }

            if (!n && !e && s && w)
                return Global.SpriteLibrary.TownSprites["fence-ne"];
            if (n && !e && s && !w)
            {
                if (x == 0)
                    return Global.SpriteLibrary.TownSprites["fence-w"];
                //else if (map[x - 1, y] == 1)
                //    return Global.SpriteLibrary.TownSprites["fence-e"];
                else
                    return Global.SpriteLibrary.TownSprites["fence-e"];
            }
            if (n && e && !s && !w)
                return Global.SpriteLibrary.TownSprites["fence-sw"];
            if (n && !e && !s && w)
                return Global.SpriteLibrary.TownSprites["fence-se"];
            else
                return Global.SpriteLibrary.TownSprites["fence-s"];
        }

        private Texture2D DetermineWallTexture(int[,] map, int x, int y)
        {
            var n = false;
            var e = false;
            var s = false;
            var w = false;

            if (y > 0)
                n = map[x, y - 1] == 1;
            if (x < map.GetLength(0) - 1)
                e = map[x + 1, y] == 1;
            if (y < map.GetLength(1) - 1)
                s = map[x, y + 1] == 1;
            if (x > 0)
                w = map[x - 1, y] == 1;

            if (!n && e && s && !w)
                return Global.SpriteLibrary.BuildingBlockSprites["building-wall-nw"];

            if (!n && e && !s && w)
            {
                if (map[x, y - 1] == 1)
                    return Global.SpriteLibrary.BuildingBlockSprites["building-wall-s"];
                else
                    return Global.SpriteLibrary.BuildingBlockSprites["building-wall-n"];
            }

            if (!n && !e && s && w)
                return Global.SpriteLibrary.BuildingBlockSprites["building-wall-ne"];

            if (n && !e && s && !w)
            {
                if (HouseMap[0][x - 1, y] == 1) // useless broken check
                    return Global.SpriteLibrary.BuildingBlockSprites["building-wall-e"];
                else
                    return Global.SpriteLibrary.BuildingBlockSprites["building-wall-w"];
            }
            if (n && e && !s && !w)
                return Global.SpriteLibrary.BuildingBlockSprites["building-wall-sw"];

            if (n && !e && !s && w)
                return Global.SpriteLibrary.BuildingBlockSprites["building-wall-se"];
            else
                return Global.DEBUG_TEXTURE;
        }

        private Texture2D DetermineTexture(int[,] map, int x, int y)
        {
            var cellValue = map[x, y];

            switch (cellValue)
            {
                case 0:
                    return Global.SpriteLibrary.TownSprites["dirt-c"];

                case 1:
                    return Global.SpriteLibrary.TownSprites["dirt-c"];

                case 2:
                    return Global.SpriteLibrary.TownSprites["grass-c"];

                case 3:
                    return Global.SpriteLibrary.TownSprites["grass-c"];

                default:
                    return Global.SpriteLibrary.TownSprites["dirt-c"];
            }
        }

        private bool IsNeighbourCellRoad(int[,] map, int x, int y)
        {
            var n = 0;
            var e = 0;
            var s = 0;
            var w = 0;

            if (y > 0)
                n = map[x, y - 1];
            if (x > 0)
                w = map[x - 1, y];
            if (x < map.GetLength(0) - 1)
                e = map[x + 1, y];
            if (y < map.GetLength(1) - 1)
                s = map[x, y + 1];

            return n == 1 || e == 1 || s == 1 || w == 1;
        }



        public void Initialize()
        {
        }

        public void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            foreach (var tilemap in _tilemaps)
            {
                if (tilemap == null)
                    continue;

                foreach (var tile in tilemap.Map)
                {
                    if (tile == null)
                        continue;

                    tile.Update(gameTime);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (HouseBounds.Contains(Global.PlayerObj.WorldPosition))
            {
                for (int i = 0; i < _tilemaps.Count - 1; i++)
                {
                    if (_tilemaps[i] == null)
                        continue;

                    foreach (var tile in _tilemaps[i].Map)
                    {
                        if (tile == null)
                            continue;

                        tile.Draw(gameTime);
                    }
                }
            }
            else
            {
                foreach (var tilemap in _tilemaps)
                {
                    if (tilemap == null)
                        continue;

                    foreach (var tile in tilemap.Map)
                    {
                        if (tile == null)
                            continue;

                        tile.Draw(gameTime);
                    }
                }
            }

            //Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, HouseBounds, Color.Red * 0.5f);
        }
    }
}
