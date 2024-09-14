using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Debugging;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.BSP;
using ProjectDonut.Tools;

namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town.Building
{
    public class BuildingLevel : IGameObject
    {
        public Plot Plot { get; set; }
        public BuildingObj ParentBuilding { get; set; }
        public int LevelIndex { get; set; }

        public int[,] FloorDataMap { get; set; }
        public int[,] WallDataMap { get; set; }
        public int[,] StairDataMap { get; set; }

        public List<Rectangle> RoomRects { get; set; }

        public Tilemap FloorTileMap { get; set; }
        public Tilemap WallTileMap { get; set; }
        public Tilemap StairTileMap { get; set; }

        public bool IsVisible => throw new NotImplementedException();
        public Texture2D Texture { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Vector2 WorldPosition => throw new NotImplementedException();
        public int ZIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private BSP _bsp;
        private Random _random;

        public BuildingLevel(Plot plot, BuildingObj parentBuilding, int levelIndex)
        {
            Plot = plot;
            ParentBuilding = parentBuilding;
            LevelIndex = levelIndex;

            _bsp = new BSP();
            _random = new Random();
        }

        public void BuildLevel(bool hasNextLevel)
        {
            FloorDataMap = new int[
                (int)Plot.Town.MapSize.X,
                (int)Plot.Town.MapSize.Y];
            WallDataMap = new int[
                (int)Plot.Town.MapSize.X,
                (int)Plot.Town.MapSize.Y];
            StairDataMap = new int[
                (int)Plot.Town.MapSize.X,
                (int)Plot.Town.MapSize.Y];

            // Build rooms
            RoomRects = RoomGenerator.GenerateRooms(ParentBuilding.BuildingBounds, Global.TownSettings.MIN_ROOM_SIZE);
            //RoomRects.Remove(RoomRects[_random.Next(RoomRects.Count)]);

            FloorDataMap = BuildingDataMapper.GenerateFloorDataMap(Plot, RoomRects);
            WallDataMap = BuildingDataMapper.GenerateWallDataMap(Plot, RoomRects);
            WallDataMap = RoomLinker2.LinkRooms(Plot, WallDataMap, FloorDataMap, RoomRects);

            if (hasNextLevel)
                StairDataMap = BuildingDataMapper.GenerateStairsDataMap(Plot, RoomRects);

            //DebugMapData.WriteMapData(FloorDataMap, $"{Plot.WorldPosition.X}-{Plot.WorldPosition.Y}_FloorDataMap");
            //DebugMapData.WriteMapData(WallDataMap, $"{Plot.WorldPosition.X}-{Plot.WorldPosition.Y}_WallDataMap");
            //DebugMapData.WriteMapData(StairDataMap, $"{Plot.WorldPosition.X}-{Plot.WorldPosition.Y}_StairDataMap");

            FloorTileMap = BuildingTileMapper.GenerateFloorTileMap(FloorDataMap, Plot);
            WallTileMap = BuildingTileMapper.GenerateWallTileMap(WallDataMap, FloorDataMap, Plot);
            StairTileMap = BuildingTileMapper.GenerateStairsTileMap(StairDataMap, Plot);
        }

        public void Initialize()
        {
            //foreach (var room in Rooms)
            //{
            //    room.Initialize();
            //}
        }

        public void LoadContent()
        {
            //foreach (var room in Rooms)
            //{
            //    room.LoadContent();
            //}
        }

        public void Update(GameTime gameTime)
        {
            //foreach (var room in Rooms)
            //{
            //    room.Update(gameTime);
            //}

            FloorTileMap.Update(gameTime);
            WallTileMap.Update(gameTime);
            StairTileMap.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            //foreach (var room in Rooms)
            //{
            //    room.Draw(gameTime);
            //}

            FloorTileMap.Draw(gameTime);
            WallTileMap.Draw(gameTime);
            //WallTileMap.DrawOutline(gameTime);
            StairTileMap.Draw(gameTime);
        }

        #region NEW Room Linking

        #endregion
    }
}
