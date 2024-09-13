using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.BSP;
using System;
using System.Collections.Generic;

namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town.Building
{
    public class BuildingObj : IGameObject
    {
        public Plot Plot { get; set; }
        public Rectangle BuildingBounds { get; set; }
        public int LevelCount { get; set; }
        public int PlayerOccupyLevel { get; set; }

        public Dictionary<int, BuildingLevel> Levels { get; set; }


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
            BuildingBounds = BuildingDataMapper.CalculateHouseBounds(Plot);
        }

        public void Build()
        {
            Levels = new Dictionary<int, BuildingLevel>();

            for (int i = 0; i < LevelCount; i++)
            {
                var level = new BuildingLevel(Plot, this, i);
                level.BuildLevel();
                Levels.Add(i, level);
            }
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

        public void Update(GameTime gameTime)
        {
            foreach (var level in Levels)
            {
                level.Value.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var level in Levels)
            {
                level.Value.Draw(gameTime);
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
