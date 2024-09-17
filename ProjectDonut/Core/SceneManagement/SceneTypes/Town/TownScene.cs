using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectDonut.Core.Input;
using ProjectDonut.Core.Sprites;
using ProjectDonut.Debugging;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.BSP;
using ProjectDonut.ProceduralGeneration.World.Structures;
using Tilemap = ProjectDonut.ProceduralGeneration.Tilemap;

namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town
{
    public class TownScene : BaseScene
    {
        private int[,] _dataMap;
        private BSP _bsp;

        //public Tilemap _tilemap;
        //public Tilemap _tilemapFences;
        //public Tilemap[] _tilemapsBuildings; 

        public Tilemap _baseTilemap;

        private Dictionary<string, Rectangle> ExitLocations;

        public List<IGameObject> NPCs { get; set; }

        private WorldStructure _worldStructure;

        private List<Plot> _plots;
        private List<ISceneObject> _sceneObjs;

        public Vector2 MapSize;
        private Random _random = new Random();
        public Rectangle TownBounds { get; set; }

        public TownScene(WorldStructure worldStructure)
        {
            SceneType = SceneType.Town;
            _worldStructure = worldStructure;
            MapSize = Global.TownSettings.TOWN_SIZE;

            _bsp = new BSP();
            ExitLocations = new Dictionary<string, Rectangle>();
            //TownBounds = new Rectangle(
            //    (int)worldStructure.WorldPosition.X,
            //    (int)worldStructure.WorldPosition.Y, 
            //    (int)MapSize.X * Global.TileSize, 
            //    (int)MapSize.Y * Global.TileSize);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _baseTilemap = GenerateGroundTilemap(new int[(int)MapSize.X, (int)MapSize.Y]);
            GenerateExitLocations();
            BuildTown();
        }

        public void BuildTown()
        {
            var areas = GenerateAreas();
            _plots = GeneratePlots(areas);
            _dataMap = _bsp.CreateDataMap(areas, (int)MapSize.X, (int)MapSize.Y, 1);

            foreach (var plot in _plots)
            {
                plot.Build();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            DebugWindow.Lines[3] = $"Building Index:";

            foreach (var plot in _plots)
            {
                plot.Update(gameTime);
            }

            foreach (var exitPoint in ExitLocations)
            {
                if (exitPoint.Value.Contains(Global.PlayerObj.WorldPosition))
                {
                    Global.SceneManager.SetCurrentScene(Global.SceneManager.Scenes["world"]);

                    var worldScene = (WorldScene)Global.SceneManager.CurrentScene;

                    switch (exitPoint.Key)
                    {
                        case "north":
                            worldScene.LastExitLocation = new Rectangle(
                                _worldStructure.TextureBounds.Left + _worldStructure.TextureBounds.Width / 2,
                                _worldStructure.TextureBounds.Top - (2 * Global.TileSize),
                                Global.TileSize,
                                Global.TileSize);
                            break;

                        case "south":
                            worldScene.LastExitLocation = new Rectangle(
                                _worldStructure.TextureBounds.Left + _worldStructure.TextureBounds.Width / 2,
                                _worldStructure.TextureBounds.Bottom + (2 * Global.TileSize),
                                Global.TileSize,
                                Global.TileSize);
                            break;

                        case "east":
                            worldScene.LastExitLocation = new Rectangle(
                                _worldStructure.TextureBounds.Right + (2 * Global.TileSize),
                                _worldStructure.TextureBounds.Top + _worldStructure.TextureBounds.Height / 2,
                                Global.TileSize,
                                Global.TileSize);
                            break;

                        case "west":
                            worldScene.LastExitLocation = new Rectangle(
                                _worldStructure.TextureBounds.Left - (2 * Global.TileSize),
                                _worldStructure.TextureBounds.Top + _worldStructure.TextureBounds.Height / 2,
                                Global.TileSize,
                                Global.TileSize);
                            break;
                    }

                    Global.PlayerObj.ChunkPosX = _worldStructure.WorldChunk.ChunkCoordX;
                    Global.PlayerObj.ChunkPosY = _worldStructure.WorldChunk.ChunkCoordY;
                    Global.SceneManager.CurrentScene.PrepareForPlayerEntry();
                }
            }

            if (Global.SceneManager.CurrentScene == this)
            {
                if (InputManager.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.G))
                {
                    BuildTown();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {


            Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());

            foreach (var tile in _baseTilemap.Map)
            {
                tile.Draw(gameTime);
            }

            foreach (var plot in _plots)
            {
                plot.Draw(gameTime);
            }

            Global.SpriteBatch.End();

            base.Draw(gameTime);

            if (Global.DRAW_INSTANCE_EXIT_LOCATIONS_OUTLINE)
            {
                Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
                foreach (var exitPoint in ExitLocations)
                {
                    Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, exitPoint.Value, Color.White);
                }
                Global.SpriteBatch.End();
            }
        }

        private Tilemap GenerateGroundTilemap(int[,] map)
        {
            var extra = 5;
            var tm = new Tilemap(map.GetLength(0) + extra, map.GetLength(1) + extra);

            for (int i = 0; i < tm.Map.GetLength(0); i++)
            {
                for (int j = 0; j < tm.Map.GetLength(1); j++)
                {
                    var tile = new Tile(false)
                    {
                        ChunkX = 0,
                        ChunkY = 0,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize),
                        Size = new Vector2(Global.TileSize, Global.TileSize),
                        Texture = SpriteLib.Town.Terrain["dirt-c"],
                        TileType = TileType.Instance,
                        IsExplored = true
                    };

                    tm.Map[i, j] = tile;
                }
            }

            return tm;
        }

        public override void PrepareForPlayerEntry()
        {
            base.PrepareForPlayerEntry();

            Global.PlayerObj.WorldPosition = new Vector2(500, 500); // TEMP
        }

        public override void PrepareForPlayerExit()
        {
            base.PrepareForPlayerExit();
        }

        private void GenerateExitLocations()
        {
            var width = _baseTilemap.Map.GetLength(0) * Global.TileSize;
            var height = _baseTilemap.Map.GetLength(1) * Global.TileSize;

            // North
            var rectNorth = new Rectangle(
                0,
                -Global.TileSize,
                width,
                Global.TileSize);

            ExitLocations.Add("north", rectNorth);

            // South
            var rectSouth = new Rectangle(
                0,
                height,
                width,
                Global.TileSize);

            ExitLocations.Add("south", rectSouth);

            // East
            var rectEast = new Rectangle(
                width,
                0,
                Global.TileSize,
                height);

            ExitLocations.Add("east", rectEast);

            // West
            var rectWest = new Rectangle(
                -Global.TileSize,
                0,
                Global.TileSize,
                height);

            ExitLocations.Add("west", rectWest);
        }



        private List<Room> GenerateAreas()
        {
            var width = (int)MapSize.X;
            var height = (int)MapSize.Y;

            var areaGens = _bsp.GenerateRooms(width, height, Global.TownSettings.MIN_PLOT_SIZE);

            return _bsp.SquashRooms(areaGens[areaGens.Count - 1], width, height);
        }

        public List<Plot> GeneratePlots(List<Room> areas)
        {
            var plots = new List<Plot>();

            foreach (var area in areas)
            {
                area.Bounds = new Rectangle(
                    area.Bounds.X + 3,
                    area.Bounds.Y + 3,
                    area.Bounds.Width - 2,
                    area.Bounds.Height - 2);

                plots.Add(new Plot(area, this)
                {
                    WorldPosition = new Vector2(area.Bounds.X * Global.TileSize, area.Bounds.Y * Global.TileSize),
                });
            }

            return plots;
        }
    }
}