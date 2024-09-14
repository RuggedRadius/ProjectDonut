using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Core.Input;
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

            for (int i = 0; i < LevelCount; i++)
            {
                var level = new BuildingLevel(Plot, this, i);
                level.BuildLevel();
                Levels.Add(i, level);
                levelsList.Add(level);
            }

            for (int i = 0; i < LevelCount - 1; i++)
            {
                Levels[i].BuildStairs(levelsList);
            }

            RoofDataMap = BuildingDataMapper.GenerateRoofDataMap(Plot, Levels[0].RoomRects);
            RoofTileMap = BuildingTileMapper.GenerateRoofTileMap(RoofDataMap, Plot);

            //DebugMapData.WriteMapData(Levels[0].FloorDataMap, $"FloorDataMap_{tmpBuildingIndex}");
            //DebugMapData.WriteMapData(RoofDataMap, $"RoofDataMap_{tmpBuildingIndex}");
            
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
        }

        public void Draw(GameTime gameTime)
        {
            if (BuildingWorldBounds.Contains(Global.PlayerObj.WorldPosition) == true)
            {
                for (int i = 0; i < PlayerOccupyLevel + 1; i++)
                {
                    Levels[i].Draw(gameTime);
                }

                //Levels[PlayerOccupyLevel].Draw(gameTime);
            }
            else
            {
                foreach (var level in Levels)
                {
                    level.Value.Draw(gameTime);
                }

                RoofTileMap.Draw(gameTime);
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
