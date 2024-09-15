﻿using System;
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
        public Vector2 WorldPosition { get; set; }
        public int ZIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private BSP _bsp;
        private Random _random;

        public BuildingLevel(Plot plot, BuildingObj parentBuilding, int levelIndex)
        {
            Plot = plot;
            ParentBuilding = parentBuilding;
            LevelIndex = levelIndex;
            WorldPosition = parentBuilding.BuildingWorldBounds.Location.ToVector2();

            _bsp = new BSP();
            _random = new Random();
        }

        public void BuildLevel()
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

            //DebugMapData.WriteMapData(FloorDataMap, $"{Plot.WorldPosition.X}-{Plot.WorldPosition.Y}_FloorDataMap");
            //DebugMapData.WriteMapData(WallDataMap, $"{Plot.WorldPosition.X}-{Plot.WorldPosition.Y}_WallDataMap");
            //DebugMapData.WriteMapData(StairDataMap, $"{Plot.WorldPosition.X}-{Plot.WorldPosition.Y}_StairDataMap");

            BuildTileMaps();            
        }

        public void BuildTileMaps()
        {
            FloorTileMap = BuildingTileMapper.GenerateFloorTileMap(FloorDataMap, Plot, this);
            WallTileMap = BuildingTileMapper.GenerateWallTileMap(WallDataMap, FloorDataMap, Plot);
        }

        public void BuildStairs(List<BuildingLevel> levels)
        {
            var levelBelow = (LevelIndex == 0) ? null : levels[LevelIndex - 1];
            var level = this;
            var levelAbove = levels[LevelIndex + 1];

            StairDataMap = BuildingDataMapper.GenerateStairsDataMap(Plot, ref levelAbove, ref level, ref levelAbove);
            StairTileMap = BuildingTileMapper.GenerateStairsTileMap(StairDataMap, Plot);

            levelAbove.BuildTileMaps();
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

            if (StairTileMap != null)
                StairTileMap.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            //if (ParentBuilding.Levels[ParentBuilding.PlayerOccupyLevel] == this)
            //{
            //    foreach (var tile in FloorTileMap.Map)
            //    {
            //        if (tile == null)
            //            continue;


            //    }

            //    FloorTileMap.Draw(gameTime);
            //    WallTileMap.Draw(gameTime);
            //}
            //else
            //{
                FloorTileMap.Draw(gameTime);
                WallTileMap.Draw(gameTime);
            //}




            //WallTileMap.DrawOutline(gameTime);


            if (StairTileMap != null)
                StairTileMap.Draw(gameTime);
        }

        #region NEW Room Linking

        #endregion
    }
}
