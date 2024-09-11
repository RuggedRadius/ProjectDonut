using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Timers;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.BSP;
using ProjectDonut.ProceduralGeneration.Dungeons;
using ProjectDonut.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town.Building
{
    public class BuildingObj : IGameObject
    {
        public Plot Plot { get; set; }
        public Rectangle BuildingBounds { get; set; }
        
        public int PlayerOccupyLevel { get; set; }

        public Dictionary<int, List<Rectangle>> Rooms { get; set; }
        public Dictionary<int, int[,]> FloorDataMaps { get; set; }
        public Dictionary<int, int[,]> WallDataMaps { get; set; }
        public Dictionary<int, int[,]> StairDataMaps { get; set; }
        public int[,] RoofDataMap { get; set; }

        public Dictionary<int, Tilemap> FloorTileMaps { get; set; }
        public Dictionary<int, Tilemap> WallTileMaps { get; set; }
        public Dictionary<int, Tilemap> StairTileMaps { get; set; }
        public Tilemap RoofTileMap { get; set; }

        // Required by IGameObject
        public bool IsVisible => throw new NotImplementedException();
        public Texture2D Texture { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Vector2 WorldPosition => throw new NotImplementedException();
        public int ZIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private Random _random;

        public BuildingObj(Plot plot, int levels)
        {
            Plot = plot;
            _random = new Random();

            BuildingBounds = BuildingDataMapper.CalculateHouseBounds(Plot);

            if (plot.PlotBounds.Contains(BuildingBounds) == false)
            {
                throw new Exception("Building out of plot bounds");
            }

            MakeRooms(levels);
            GenerateLayerData(levels);
            GenerateLayerTilemaps();
            //PlaceExternalDoor(WallTileMaps[0]);
        }

        private void MakeRooms(int levels)
        {
            Rooms = new Dictionary<int, List<Rectangle>>();

            for (int i = 0; i < levels; i++)
            {
                var roomBounds = RoomGenerator.GenerateRooms(BuildingBounds, new Vector2(5 * Global.TileSize, 3 * Global.TileSize));

                foreach (var rect in roomBounds)
                {
                    if (BuildingBounds.Contains(rect) == false)
                    {
                        throw new Exception("Room out of building bounds");
                    }
                }

                Rooms.Add(i, roomBounds);
            }
        }

        private void GenerateLayerData(int levels)
        {
            FloorDataMaps = new Dictionary<int, int[,]>();
            WallDataMaps = new Dictionary<int, int[,]>();
            StairDataMaps = new Dictionary<int, int[,]>();

            for (int i = 0; i < levels; i++)
            {
                var floorData = BuildingDataMapper.GenerateFloorDataMap(Plot.PlotBounds, Rooms[i]);
                FloorDataMaps.Add(i, floorData);

                var wallData = BuildingDataMapper.GenerateWallDataMap(Plot.PlotBounds, Rooms[i]);
                WallDataMaps.Add(i, wallData);

                if (i == levels - 1)
                {
                    // Create roof
                    var roofData = BuildingDataMapper.GenerateRoofDataMap(Plot.PlotBounds, Rooms[i]);
                    RoofDataMap = roofData;
                }
                else
                {
                    // Create stairs
                    var stairsData = BuildingDataMapper.GenerateStairDataMap(Plot.PlotBounds, Rooms[i]);
                    StairDataMaps.Add(i, stairsData);
                }
            }
        }

        private void GenerateLayerTilemaps()
        {
            FloorTileMaps = new Dictionary<int, Tilemap>();
            WallTileMaps = new Dictionary<int, Tilemap>();
            StairTileMaps = new Dictionary<int, Tilemap>();

            for (int i = 0; i < FloorDataMaps.Count; i++)
            {
                FloorTileMaps.Add(i, BuildingTileMapper.GenerateHouseFloorMap(FloorDataMaps[i], Plot));
            }

            for (int i = 0; i < WallDataMaps.Count; i++)
            {
                WallTileMaps.Add(i, BuildingTileMapper.GenerateHouseWallMap(WallDataMaps[i], FloorDataMaps[i], Plot));
            }

            for (int i = 0; i < StairDataMaps.Count; i++)
            {
                StairTileMaps.Add(i, BuildingTileMapper.GenerateStairsTileMap(StairDataMaps[i]));
            }

            RoofTileMap = BuildingTileMapper.GenerateHouseRoofMap(RoofDataMap, Plot);
        }

        // TODO: later, not being used
        public void Initialize()
        {
            //foreach (var layer in Layers)
            //{
            //    layer.Initialize();
            //}
        }

        // TODO: later, not being used
        public void LoadContent()
        {
            //foreach (var layer in Layers)
            //{
            //    layer.LoadContent();
            //}
        }

        // TODO: later, not being used
        public void Update(GameTime gameTime)
        {
            //foreach (var layer in Layers)
            //{
            //    layer.Update(gameTime);
            //}
        }

        public void Draw(GameTime gameTime)
        {
            for (var i = 0; i < FloorTileMaps.Count; i++)
            {
                if (PlayerOccupyLevel >= i)
                {
                    break;
                }

                // Floor
                foreach (var tile in FloorTileMaps[i].Map)
                {
                    tile.Draw(gameTime);
                }

                // Wall
                foreach (var tile in WallTileMaps[i].Map)
                {
                    tile.Draw(gameTime);
                }

                // Stairs
                if (i < StairDataMaps.Count)
                {
                    foreach (var tile in StairTileMaps[i].Map)
                    {
                        tile.Draw(gameTime);
                    }
                }
            }

            // Roof
            if (BuildingBounds.Contains(Global.PlayerObj.WorldPosition) == false)
            {
                RoofTileMap.Draw(gameTime);
            }
        }

        #region Datamap Generation

        #endregion

        #region Tilemap Generation
        private void PlaceExternalDoor(Tilemap bottomFloorWallTileMap)
        {
            var targetTexture = Global.SpriteLibrary.BuildingBlockSprites["building-wall-s"];
            var southWalls = new List<Tile>();
            foreach (var tile in bottomFloorWallTileMap.Map)
            {
                if (tile == null)
                    continue;

                if (tile.Texture == targetTexture)
                {
                    southWalls.Add(tile);
                }
            }

            var doorTile = southWalls[_random.Next(0, southWalls.Count)];
            doorTile.Texture = Global.SpriteLibrary.BuildingBlockSprites["building-door-ext"];
        }
        #endregion
    }
}
