//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.Timers;
//using ProjectDonut.Interfaces;
//using ProjectDonut.ProceduralGeneration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town.Building
//{
//    public enum BuilderLayerType
//    {
//        Floor,
//        Walls,
//        Stairs,
//        Roof
//    }

//    public class BuildingLayer : IGameObject
//    {
//        public BuildingObj ParentBuilding { get; set; }
//        public BuilderLayerType Type { get; set; }
//        public int LayerIndex { get; set; }
//        public int[,] Datamap { get; set; }

//        public Tilemap Tilemap { get; set; }

//        // Required by IGameObject
//        public bool IsVisible => throw new NotImplementedException();
//        public Texture2D Texture { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
//        public Vector2 WorldPosition => throw new NotImplementedException();
//        public int ZIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


//        public BuildingLayer(BuildingObj parent, BuilderLayerType type, int layerIndex)
//        {
//            Type = type;

//            switch (Type)
//            {
//                case BuilderLayerType.Floor:
//                    Datamap = parent.FloorDataMaps[layerIndex];
//                    break;

//                case BuilderLayerType.Walls:
//                    Datamap = parent.WallDataMaps[layerIndex];
//                    break;

//                case BuilderLayerType.Stairs:
//                    Datamap = parent.StairDataMaps[layerIndex]; 
//                    break;

//                case BuilderLayerType.Roof:
//                    Datamap = parent.RoofDataMap; 
//                    break;
//            }

//            GenerateTileMap();
//        }

//        private void GenerateTileMap()
//        {
//            switch (Type)
//            {
//                case BuilderLayerType.Floor:
//                    Tilemap = BuildingTileMapper.GenerateHouseFloorMap(Datamap, null);
//                    break;

//                case BuilderLayerType.Walls:
//                    Tilemap = BuildingTileMapper.GenerateHouseWallMap(
//                        Datamap, 
//                        ParentBuilding.FloorDataMaps[LayerIndex], 
//                        ParentBuilding.Plot);
//                    break;

//                case BuilderLayerType.Stairs:
//                    Tilemap = BuildingTileMapper.GenerateStairsTileMap(Datamap, null);
//                    break;

//                case BuilderLayerType.Roof:
//                    Tilemap = BuildingTileMapper.GenerateHouseRoofMap(Datamap, null);
//                    break;
//            }
//        }

//        public void Initialize()
//        {
//            foreach (var tile in Tilemap.Map)
//            {
//                tile.Initialize();
//            }
//        }

//        public void LoadContent()
//        {
//            foreach (var tile in Tilemap.Map)
//            {
//                tile.LoadContent();
//            }
//        }

//        public void Update(GameTime gameTime)
//        {
//            foreach (var tile in Tilemap.Map)
//            {
//                tile.Update(gameTime);
//            }
//        }

//        public void Draw(GameTime gameTime)
//        {
//            foreach (var tile in Tilemap.Map)
//            {
//                tile.Draw(gameTime);
//            }
//        }
//    }
//}
