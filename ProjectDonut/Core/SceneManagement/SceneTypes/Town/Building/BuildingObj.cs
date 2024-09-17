using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Core.Input;
using ProjectDonut.Core.Sprites;
using ProjectDonut.Debugging;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.BSP;
using ProjectDonut.Tools;
using System;
using System.Collections.Generic;

namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town.Building
{
    public class BuildingObj : IGameObject
    {
        public static int tmpBuildingCounter = 0;
        public static int tmpBuildingIndex;

        public Plot Plot { get; set; }
        public Rectangle BuildingBounds { get; set; }
        public Rectangle BuildingWorldBounds { get; set; }
        public int LevelCount { get; set; }
        public int PlayerOccupyLevel { get; set; }

        public Dictionary<int, BuildingLevel> Levels { get; set; }
        public BuildingRoof Roof { get; set; }

        public int[,] RoofDataMap { get; set; }
        public Tilemap RoofTileMap { get; set; }


        // Required by IGameObject
        public bool IsVisible => throw new NotImplementedException();
        public Texture2D Texture { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Vector2 WorldPosition => throw new NotImplementedException();
        public int ZIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private Random _random;
        private BSP _bsp;


        public BuildingObj(Plot plot, int levels)
        {
            Plot = plot;
            _random = new Random();
            _bsp = new BSP();

            LevelCount = levels;
            BuildingBounds = BuildingDataMapper.CalculateHouseBounds(Plot, Global.TownSettings.MIN_BUILDING_SIZE);
            BuildingWorldBounds = BuildingBounds.Mutliply(Global.TileSize);
            tmpBuildingIndex = tmpBuildingCounter;
            tmpBuildingCounter++;
        }

        public void Build()
        {
            Levels = new Dictionary<int, BuildingLevel>();
            var levelsList = new List<BuildingLevel>();

            // Build Levels
            for (int i = 0; i < LevelCount; i++)
            {
                var level = new BuildingLevel(Plot, this, i);
                level.BuildLevel();
                Levels.Add(i, level);
                levelsList.Add(level);
            }

            // Build Stairs
            for (int i = 0; i < LevelCount - 1; i++)
            {
                Levels[i].BuildStairs(levelsList);
            }

            // Build Doors
            var bottomLevel = Levels[0];
            PlaceExternalDoor(ref bottomLevel);

            // Build Roof
            RoofDataMap = BuildingDataMapper.GenerateRoofDataMap(Plot, Levels[0].RoomRects);
            //RoofTileMap = BuildingTileMapper.GenerateRoofTileMap(RoofDataMap, Plot);

            //DebugMapData.WriteMapData(Levels[0].FloorDataMap, $"FloorDataMap_{tmpBuildingIndex}");
            //DebugMapData.WriteMapData(RoofDataMap, $"RoofDataMap_{tmpBuildingIndex}");

            Roof = new BuildingRoof();
            Roof.BuildRoof(Levels[Levels.Count - 1]);
        }

        public void Initialize()
        {
            foreach (var level in Levels)
            {
                level.Value.Initialize();
            }
        }

        public void LoadContent()
        {
            foreach (var level in Levels)
            {
                level.Value.LoadContent();
            }
        }

        private double tmpTimer = 0f;
        private double levelChangeCoolDown = 0.5f;
        public void Update(GameTime gameTime)
        {
            tmpTimer += gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var level in Levels)
            {
                level.Value.Update(gameTime);
            }

            if (BuildingWorldBounds.Contains(Global.PlayerObj.WorldPosition) == true)
            {
                DebugWindow.Lines[3] = $"Building Index: {tmpBuildingIndex}";

                if (tmpTimer > levelChangeCoolDown && InputManager.IsKeyPressed(Keys.N))
                {
                    if (PlayerOccupyLevel < LevelCount - 1)
                    {
                        PlayerOccupyLevel++;
                        tmpTimer = 0;
                    }
                }
                else if (tmpTimer > levelChangeCoolDown && InputManager.IsKeyPressed(Keys.B))
                {
                    if (PlayerOccupyLevel >= 1)
                    {
                        PlayerOccupyLevel--;
                        tmpTimer = 0;
                    }
                }
            }
            else
            {
                PlayerOccupyLevel = 0;
            }

            foreach (var level in Levels)
            {
                if (level.Value.LevelIndex == PlayerOccupyLevel)
                {
                    foreach (var tile in level.Value.FloorTileMap.Map)
                    {
                        if (tile == null)
                            continue;

                        tile.IsVisible = true;
                    }

                    foreach (var tile in level.Value.WallTileMap.Map)
                    {
                        if (tile == null)
                            continue;

                        tile.IsVisible = true;
                    }

                    if (level.Value.StairTileMap == null)
                        continue;

                    foreach (var tile in level.Value.StairTileMap.Map)
                    {
                        if (tile == null)
                            continue;

                        tile.IsVisible = true;
                    }
                }
                else
                {
                    foreach (var tile in level.Value.FloorTileMap.Map)
                    {
                        if (tile == null)
                            continue;

                        tile.IsVisible = false;
                    }

                    foreach (var tile in level.Value.WallTileMap.Map)
                    {
                        if (tile == null)
                            continue;

                        tile.IsVisible = false;
                    }

                    if (level.Value.StairTileMap == null)
                        continue;

                    foreach (var tile in level.Value.StairTileMap.Map)
                    {
                        if (tile == null)
                            continue;

                        tile.IsVisible = false;
                    }
                }                
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (BuildingWorldBounds.Contains(Global.PlayerObj.WorldPosition) == true)
            {
                for (int i = 0; i < PlayerOccupyLevel + 1; i++)
                {
                    Levels[i].Draw(gameTime);
                }
            }
            else
            {
                foreach (var level in Levels)
                {
                    level.Value.Draw(gameTime);
                }

                Roof.Draw(gameTime);
            }
        }

        private void PlaceExternalDoor(ref BuildingLevel bottomLevel)
        {
            // TODO: Add east ands west doors
            // Get bounds walls (just north and south for now)
            var southWalls = new List<Tile>();
            for (int i = 0; i < BuildingBounds.Width; i += 1)
            {
                for (int j = 0; j < BuildingBounds.Height; j += 1)
                {
                    if (bottomLevel.WallTileMap.Map[i, j] == null)
                        continue;

                    var tile = bottomLevel.WallTileMap.Map[i, j];

                    //if (i != BuildingBounds.Left && i != BuildingBounds.Right)
                    //    continue;


                    //if (j != BuildingBounds.Top && j != BuildingBounds.Bottom)
                    //    continue;

                    //var x = i;// / Global.TileSize;
                    //var y = j;// / Global.TileSize;

                    if (tile.Texture == SpriteLib.TownSprites.Walls["wall-s"])
                    {
                        southWalls.Add(bottomLevel.WallTileMap.Map[i, j]);
                    }
                } 

            }

            //if (southWalls.Count == 0)
            //    throw new Exception("Could not place door on building");

            //var doorTile = southWalls[_random.Next(0, southWalls.Count)];
            //doorTile.Texture = SpriteLib.BuildingBlockSprites["door-int"];

            //bottomLevel.WallDataMap[doorTile.xIndex, doorTile.yIndex] = 0;
            //bottomLevel.FloorDataMap[doorTile.xIndex, doorTile.yIndex] = 1;
        }
    }
}
