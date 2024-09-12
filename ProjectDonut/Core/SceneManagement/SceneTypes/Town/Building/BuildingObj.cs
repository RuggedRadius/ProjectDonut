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
        public int LevelCount { get; set; }
        public int PlayerOccupyLevel { get; set; }

        public Dictionary<int, List<Rectangle>> Rooms { get; set; }
        public List<BuildingRoom> BuildingRooms { get; set; }

        public Dictionary<int, int[,]> FloorDataMaps { get; set; }
        public Dictionary<int, int[,]> WallDataMaps { get; set; }
        public Dictionary<int, int[,]> WallCapDataMaps { get; set; }
        public Dictionary<int, int[,]> StairDataMaps { get; set; }
        public int[,] RoofDataMap { get; set; }

        public Dictionary<int, Tilemap> FloorTileMaps { get; set; }
        public Dictionary<int, Tilemap> WallTileMaps { get; set; }
        public Dictionary<int, Tilemap> WallCapTileMaps { get; set; }
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

            LevelCount = levels;
            BuildingBounds = BuildingDataMapper.CalculateHouseBounds(Plot);
        }

        public void Build()
        {
            MakeRooms(LevelCount);
            //GenerateLayerData(LevelCount);
            //GenerateLayerTilemaps();
            //PlaceExternalDoor(WallTileMaps[0]);
        }

        private void MakeRooms(int levels)
        {
            //// ************ TEMPORARY ************
            //Rooms = new Dictionary<int, List<Rectangle>>();
            //Rooms.Add(0, new List<Rectangle>() { BuildingBounds });
            //return;
            //// ***********************************


            Rooms = new Dictionary<int, List<Rectangle>>();

            for (int i = 0; i < levels; i++)
            {
                var roomBounds = RoomGenerator.GenerateRooms(BuildingBounds, new Vector2(6, 6));

                foreach (var rect in roomBounds)
                {
                    if (BuildingBounds.Contains(rect) == false)
                    {
                        throw new Exception("Room out of building bounds");
                    }
                }

                roomBounds.Remove(roomBounds[_random.Next(0, roomBounds.Count)]);

                Rooms.Add(i, roomBounds);
            }

            BuildingRooms = new List<BuildingRoom>();
            foreach (var room in Rooms) 
            {
                foreach (var r in room.Value)
                {
                    BuildingRooms.Add(new BuildingRoom(this, r, room.Key));
                }
            }

            foreach (var room in BuildingRooms)
            {
                room.Initialize();
            }
        }

        private void GenerateLayerData(int levels)
        {


            FloorDataMaps = new Dictionary<int, int[,]>();
            WallDataMaps = new Dictionary<int, int[,]>();
            WallCapDataMaps = new Dictionary<int, int[,]>();
            StairDataMaps = new Dictionary<int, int[,]>();

            for (int i = 0; i < levels; i++)
            {
                var floorData = BuildingDataMapper.GenerateFloorDataMap(Plot, Rooms[i]);
                FloorDataMaps.Add(i, floorData);

                var wallData = BuildingDataMapper.GenerateWallDataMap(Plot, Rooms[i]);
                WallDataMaps.Add(i, wallData);

                //var wallCapData = BuildingDataMapper.GenerateWallCapDataMap(Plot, Rooms[i]);
                //WallCapDataMaps.Add(i, wallCapData);

                if (i >= levels - 1)
                {
                    // Create roof
                    var roofData = BuildingDataMapper.GenerateRoofDataMap(Plot, Rooms[i]);
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
            WallCapTileMaps = new Dictionary<int, Tilemap>();
            StairTileMaps = new Dictionary<int, Tilemap>();

            for (int i = 0; i < FloorDataMaps.Count; i++)
            {
                FloorTileMaps.Add(i, BuildingTileMapper.GenerateFloorTileMap(FloorDataMaps[i], Plot));
            }

            for (int i = 0; i < WallDataMaps.Count; i++)
            {
                WallTileMaps.Add(i, BuildingTileMapper.GenerateWallTileMap(WallDataMaps[i], FloorDataMaps[i], Plot));
            }

            for (int i = 0; i < WallCapDataMaps.Count; i++)
            {
                WallCapTileMaps.Add(i, BuildingTileMapper.GenerateWallCapTileMap(WallCapDataMaps[i], FloorDataMaps[i], i, Plot));
            }

            for (int i = 0; i < StairDataMaps.Count; i++)
            {
                StairTileMaps.Add(i, BuildingTileMapper.GenerateStairsTileMap(StairDataMaps[i]));
            }
                        
            RoofTileMap = BuildingTileMapper.GenerateRoofTileMap(RoofDataMap, Plot);
        }

        // TODO: later, not being used
        public void Initialize()
        {
            foreach (var room in BuildingRooms)
            {
                room.Initialize();
            }
        }

        // TODO: later, not being used
        public void LoadContent()
        {
            foreach (var room in BuildingRooms)
            {
                room.LoadContent();
            }
        }

        // TODO: later, not being used
        public void Update(GameTime gameTime)
        {
            foreach (var room in BuildingRooms)
            {
                room.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var room in BuildingRooms)
            {
                room.Draw(gameTime);
            }

            //for (var i = 0; i < FloorTileMaps.Count; i++)
            //{
            //    //if (PlayerOccupyLevel >= i)
            //    //{
            //    //    break;
            //    //}

            //    // Floor
            //    FloorTileMaps[i].Draw(gameTime);
            //    WallTileMaps[i].Draw(gameTime);
            //    //WallCapTileMaps[i].Draw(gameTime);

            //    // Stairs
            //    if (i < StairDataMaps.Count)
            //    {
            //        StairTileMaps[i].Draw(gameTime);
            //    }
            //}

            //// Roof
            //if (BuildingBounds.Contains(Global.PlayerObj.WorldPosition) == false)
            //{
            //    ;
            //    //RoofTileMap.Draw(gameTime);
                
            //}

            //if (Global.SHOW_GRID_OUTLINE)
            //    RoofTileMap.DrawOutline(gameTime);
        }


        // TODO: Fix and implement this
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
    }
}
