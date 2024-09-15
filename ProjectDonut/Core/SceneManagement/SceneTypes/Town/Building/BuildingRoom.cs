using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;

namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town.Building
{
    public class BuildingRoom : IGameObject
    {
        public BuildingObj ParentBuilding { get; set; }
        public Rectangle RoomBounds { get; set; }
        public int RoomLevel { get; set; }
        
        public int[,] FloorDataMap { get; set; }
        public int[,] WallDataMap { get; set; }
        public int[,] StairDataMap { get; set; }
        public int[,] RoofDataMap { get; set; }

        public Tilemap FloorTileMap { get; set; }
        public Tilemap WallTileMap { get; set; }
        public Tilemap StairTileMap { get; set; }   
        public Tilemap RoofTileMap { get; set; }


        // Probably not needed
        public bool IsVisible { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 WorldPosition => throw new NotImplementedException();
        public int ZIndex { get; set; }

        public BuildingRoom(BuildingObj parentBuilding, Rectangle roomBounds, int roomLevel)
        {
            ParentBuilding = parentBuilding;
            RoomBounds = roomBounds;
            RoomLevel = roomLevel;
        }

        public void Initialize()
        {
            //    FloorDataMap = BuildingDataMapper.GenerateRoomDataMap(ParentBuilding.Plot, RoomBounds);
            //    WallDataMap = BuildingDataMapper.GenerateRoomWallDataMap(ParentBuilding.Plot, RoomBounds);

            //    FloorTileMap = BuildingTileMapper.GenerateFloorTileMap(FloorDataMap, ParentBuilding.Plot);
            //    WallTileMap = BuildingTileMapper.GenerateWallTileMap(WallDataMap, FloorDataMap, ParentBuilding.Plot);
        }

        public void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime)
        {
            FloorTileMap.Draw(gameTime);
            WallTileMap.Draw(gameTime);
        }
    }
}
