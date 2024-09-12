using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.BSP;
using System;
using System.Collections.Generic;
using ProjectDonut.Core.SceneManagement.SceneTypes.Town.Building;

namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town
{
    public class Plot : IGameObject
    {
        public TownScene Town { get; set; }

        // Plot
        //public Room Area { get; set; } // TODO: Eventually not need this? have PlotBounds now
        public Vector2 WorldPosition { get; set; }
        public int[,] PlotMap { get; set; }
        public int[,] FenceMap { get; set; }

        public Tilemap _groundTileMap { get; set; }
        public Tilemap _fenceTileMap { get; set; }
        //public List<Tilemap> _tilemaps { get; set; }

        // House
        //public List<int[,]> HouseMap { get; set; }
        //public Rectangle HouseBounds { get; set; }
        public Rectangle PlotBounds { get; set; }
        private int _baseWidth;
        private int _baseHeight;
        //public Vector2 HouseOffset 
        //{ 
        //    get
        //    {
        //        return new Vector2(
        //            HouseBounds.X - WorldPosition.X, 
        //            HouseBounds.Y - WorldPosition.Y);
        //    }
        //}
        public BuildingObj Building { get; set; }

        private BSP _bsp;
        private Random _random;

        // Shouldnt need these...
        public int ZIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsVisible => throw new NotImplementedException();
        public Texture2D Texture { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        public Plot(Room area, TownScene town)
        {
            Town = town;

            PlotBounds = new Rectangle(
                (int)area.Bounds.X,
                (int)area.Bounds.Y,
                area.Bounds.Width,
                area.Bounds.Height
                );

            _bsp = new BSP();
            _random = new Random();
        }

        public void Build()
        {
            // Generate data maps
            PlotMap = PlotDataMapper.GeneratePlotMap(PlotBounds);
            FenceMap = PlotDataMapper.GenerateFenceMap(PlotBounds);

            _groundTileMap = GenerateGroundTilemap(PlotMap);
            _fenceTileMap = GenerateFenceTilemap(FenceMap);

            Building = new BuildingObj(this, 1);
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
                        LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize) + WorldPosition,
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
                        Texture = TextureDecider.DetermineTownFenceTexture(map, i, j),
                        TileType = TileType.Instance,
                        IsExplored = true
                    };

                    tm.Map[i, j] = tile;
                }
            }

            return tm;
        }

        //private Tilemap GenerateHouseFloorMap(int[,] map)
        //{
        //    var tm = new Tilemap(map.GetLength(0), map.GetLength(1));

        //    for (int i = 0; i < map.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < map.GetLength(1); j++)
        //        {
        //            if (map[i, j] == 0)
        //            {
        //                continue;
        //            }

        //            var tile = new Tile(false)
        //            {
        //                ChunkX = 0,
        //                ChunkY = 0,
        //                xIndex = i,
        //                yIndex = j,
        //                LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize) + WorldPosition,
        //                Size = new Vector2(Global.TileSize, Global.TileSize),
        //                Texture = Global.SpriteLibrary.BuildingBlockSprites["building-floor"],
        //                TileType = TileType.Instance,
        //                IsExplored = true
        //            };

        //            tm.Map[i, j] = tile;
        //        }
        //    }

        //    return tm;
        //}

        //private Tilemap GenerateHouseWallMap(int[,] map)
        //{
        //    var tm = new Tilemap(map.GetLength(0), map.GetLength(1));

        //    for (int i = 0; i < map.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < map.GetLength(1); j++)
        //        {
        //            if (map[i, j] == 0)
        //            {
        //                continue;
        //            }

        //            var tile = new Tile(false)
        //            {
        //                ChunkX = 0,
        //                ChunkY = 0,
        //                xIndex = i,
        //                yIndex = j,
        //                LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize) + WorldPosition,
        //                Size = new Vector2(Global.TileSize, Global.TileSize),
        //                Texture = TextureDecider.DetermineBuildingWallTexture(map, HouseMap[0], i, j),
        //                TileType = TileType.Instance,
        //                IsExplored = true
        //            };

        //            tm.Map[i, j] = tile;
        //        }
        //    }

        //    return tm;
        //}

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


        public void Initialize()
        {
        }

        public void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            _groundTileMap.Update(gameTime);
            _fenceTileMap.Update(gameTime);
            Building.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            _groundTileMap.Draw(gameTime);
            _fenceTileMap.Draw(gameTime);
            Building.Draw(gameTime);
        }
    }
}
