using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using ProjectDonut.Core.Sprites;
using ProjectDonut.Debugging;
using ProjectDonut.GameObjects.Doodads;
using ProjectDonut.GameObjects.Doodads.Chests;
using ProjectDonut.GameObjects.PlayerComponents;
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

        public List<IInteractable> Interactables { get; set; }

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
            Interactables = new List<IInteractable>();

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
            PlaceTESTChests();
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
            FloorTileMap.Update(gameTime);
            WallTileMap.Update(gameTime);

            Interactables.ForEach(x => x.Update(gameTime));

            if (StairTileMap != null)
                StairTileMap.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            FloorTileMap.Draw(gameTime);
            WallTileMap.Draw(gameTime);

            Interactables.ForEach(x => x.Draw(gameTime));

            if (StairTileMap != null)
                StairTileMap.Draw(gameTime);
        }

        // TODO: Only for testing
        public void PlaceTESTChests()
        {
            var random = new Random();
            var floorTiles = new List<Tile>();

            for (int i = 0; i < FloorTileMap.Map.GetLength(0); i++)
            {
                for (int j = 0; j < FloorTileMap.Map.GetLength(1); j++)
                {
                    if (FloorTileMap.Map[i, j] == null)
                        continue;

                    floorTiles.Add(FloorTileMap.Map[i, j]);
                }
            }

            for (int i = 0; i < 5; i++)
            {
                // Get random floor posit   ion
                var floorTile = floorTiles[random.Next(floorTiles.Count)];

                var rect = new Rectangle(
                    (int)floorTile.WorldPosition.X,
                    (int)floorTile.WorldPosition.Y,
                    Global.TileSize,
                    Global.TileSize);

                var items = new List<InventoryItem>();
                items.Add(new InventoryItem() { Name = "Stone", Icon = SpriteLib.UI.Items["rock"], ItemType = ItemType.Consumable, Quantity = 50 });
                items.Add(new InventoryItem() { Name = "Wood Log", Icon = SpriteLib.UI.Items["wood-log"], ItemType = ItemType.Consumable, Quantity = 50 });

                Interactables.Add(new Chest(rect, items));

                floorTiles.Remove(floorTile);
            }
        }
    }
}
