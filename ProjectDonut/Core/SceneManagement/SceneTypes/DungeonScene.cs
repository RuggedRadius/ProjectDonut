using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.Dungeons;
using System.IO;
using ProjectDonut.ProceduralGeneration.Dungeons.DungeonPopulation;
using ProjectDonut.Core.Input;
using ProjectDonut.ProceduralGeneration.BSP;
using ProjectDonut.GameObjects.Doodads;
using ProjectDonut.ProceduralGeneration.World;
using ProjectDonut.Combat;

namespace ProjectDonut.Core.SceneManagement.SceneTypes
{
    public class DungeonScene : BaseScene
    {
        //private FogOfWar _fog;
        private Random random = new Random();

        // Instance-related 
        public int[,] DataMap;
        private BSP _bsp;
        private Tilemap _tilemap;
        private Tilemap _populationMap;

        private const int Dimension = 100;

        private Rectangle EntryLocation;

        private Dictionary<string, Rectangle> ExitLocations;
        private List<Rectangle> WallPositions;
        public List<IGameObject> Enemies { get; set; } = new List<IGameObject>();

        public List<InteractableDoodad> Interactables { get; set; } = new List<InteractableDoodad>();

        private Texture2D _debugTexture;

        private int mapWidth;
        private int mapHeight;

        private CombatInitiator _combatInitiator;

        public DungeonScene()
        {
            SceneType = SceneType.Dungeon;
            _bsp = new BSP();
            _combatInitiator = new CombatInitiator();
        }

        public override void Initialize()
        {
            base.Initialize();

            _debugTexture = new Texture2D(Global.GraphicsDevice, 1, 1);
            _debugTexture.SetData(new[] { Color.Magenta });

            GenerateDungeon(true, true);
            WallPositions = FindWallPositions();

            mapWidth = _tilemap.Map.GetLength(0);
            mapHeight = _tilemap.Map.GetLength(1);
        }

        private void GenerateDungeon(bool loadLast, bool SquashRooms)
        {
            _tilemap = GenerateDungeonTileMap(Dimension, Dimension, loadLast, SquashRooms);

            var popSettings = new DungeonLevelSettings()
            {
                EnemyCount = 50,
                PossibleEnemies = new Dictionary<string, int>()
                {
                    { "goblin", 10 },
                    { "orc", 5 }
                },
                HasSubsequentLevel = true
            };

            var dungeonPopulater = new DungeonPopulater(DataMap);
            var scene = this;
            Interactables.Clear();
            dungeonPopulater.PopulateDungeon(popSettings, ref scene);
            _populationMap = dungeonPopulater.CreateTileMap();

            //Enemies = new List<IGameObject>();
            //var enemies = dungeonPopulater.CreateEnemies(popSettings);
            //for (int i = 0; i < enemies.Count; i++)
            //{
            //    _gameObjects.Add($"enemy-{i + 1}", enemies[i]);
            //    Enemies.Add(enemies[i]);
            //}

            var stairsLocation = dungeonPopulater.GetStairsLocation();
            EntryLocation = new Rectangle(stairsLocation.Item1, stairsLocation.Item2, Global.TileSize, Global.TileSize);
            //Global.Player.Position = new Vector2(EntryLocation.X, EntryLocation.Y);

            ExitLocations = new Dictionary<string, Rectangle>();
            var exitPoints = dungeonPopulater.GetExitLocations();
            foreach (var point in exitPoints)
            {
                ExitLocations.Add("world-exit", new Rectangle(point.Item1, point.Item2, Global.TileSize, Global.TileSize));
            }
        }

        private List<Rectangle> FindWallPositions()
        {
            var wallTiles = new List<Rectangle>();

            foreach (var tile in _tilemap.Map)
            {
                if (tile == null)
                    continue;

                if (tile.DungeonTileType == DungeonTileType.Wall)
                {
                    wallTiles.Add(tile.Bounds);
                }

                //if (DataMap[tile.xIndex, tile.yIndex] == 1)
                //{
                //    wallTiles.Add(new Rectangle(
                //        (int)tile.Position.X,
                //        (int)tile.Position.Y,
                //        Global.TileSize,
                //        Global.TileSize));
                //}
            }

            return wallTiles;
        }

        private Tilemap GenerateDungeonTileMap(int width, int height, bool loadLast, bool squashRooms)
        {
            var path = @"C:\DungeonData.txt";
            var dataMap = new int[width, height];
            if (File.Exists(path) && loadLast)
            {
                dataMap = Debugging.DebugWindow.LoadIntArrayFromFile(path);
            }
            else
            {
                // Generate rooms
                var rooms = _bsp.GenerateRooms(width, height, Global.DungeonSettings.MIN_ROOM_SIZE);
                rooms[rooms.Count - 1] = _bsp.CreateRoomsWithinAreas(rooms[rooms.Count - 1]);

                // Squash rooms in
                if (squashRooms)
                {
                    rooms[rooms.Count - 1] = _bsp.SquashRooms(rooms[rooms.Count - 1], width, height);
                }

                // Generate data map
                dataMap = _bsp.CreateDataMap(rooms[rooms.Count - 1], width, height, 0);

                // Link rooms
                var rects = rooms[rooms.Count - 1].Select(x => x.Bounds).ToList();
                var rectLinker = new RectangleLinker();
                var links = rectLinker.LinkRectangles(rects);
                var linkages = _bsp.LinkAllRooms(links, dataMap);
                dataMap = BSP.MergeArrays(dataMap, linkages);
            }

            DataMap = dataMap;

            //Debugging.Debugger.PrintDataMap(dataMap, @"C:\Dungeon.txt");
            //Debugging.Debugger.SaveIntArrayToFile(dataMap, path);

            var generator = new DungeonGenerator();
            return generator.CreateTileMap(dataMap);
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


            if (InputManager.KeyboardState.IsKeyDown(Keys.F1))
            {
                GenerateDungeon(false, false);
            }

            if (InputManager.KeyboardState.IsKeyDown(Keys.F2))
            {
                _gameObjects.Clear();
                GenerateDungeon(false, true);
            }

            if (InputManager.KeyboardState.IsKeyDown(Keys.F4))
            {
                var path = @"C:\DungeonData.txt";
                DataMap = Debugging.DebugWindow.LoadIntArrayFromFile(path);
                _tilemap = GenerateDungeonTileMap(Dimension, Dimension, true, true);
            }

            if (InputManager.KeyboardState.IsKeyDown(Keys.F5))
            {
                var path = @"C:\DungeonData.txt";
                Debugging.DebugWindow.SaveIntArrayToFile(DataMap, path);
            }

            if (InputManager.KeyboardState.IsKeyDown(Keys.C))
            {
                Global.INSTANCE_SIGHT_RADIUS -= 1;
            }

            if (InputManager.KeyboardState.IsKeyDown(Keys.V))
            {
                Global.INSTANCE_SIGHT_RADIUS += 1;
            }

            foreach (var exitPoint in ExitLocations)
            {
                if (exitPoint.Value.Contains(Global.PlayerObj.WorldPosition))
                {

                    Global.SceneManager.SetCurrentScene(Global.SceneManager.Scenes["world"]);
                    Global.SceneManager.CurrentScene.PrepareForPlayerEntry();
                }
            }

            foreach (var enemy in Enemies)
            {
                enemy.Update(gameTime);
            }

            foreach (var interactable in Interactables)
            {
                interactable.Update(gameTime);
            }

            Interactables.Where(x => 
                    x.State == InteractableState.Interacted &&
                    x._sprite.Controller.IsAnimating == false)
                .ToList()
                .ForEach(x => Interactables.Remove(x));

            UpdateVisibility(Global.PlayerObj.WorldPosition, Global.INSTANCE_SIGHT_RADIUS);
        
            _combatInitiator.Update(gameTime);
        }

        public void UpdateVisibility(Vector2 playerPosition, int viewDistance)
        {
            int playerTileX = (int)(playerPosition.X / Global.TileSize);
            int playerTileY = (int)(playerPosition.Y / Global.TileSize);

            // Clear current visibility
            foreach (var tile in _tilemap.Map)
            {
                if (tile == null)
                    continue;

                tile.IsVisible = false;
            }

            // Cast rays in all directions
            for (float angle = 0; angle < 360; angle += 1)
            {
                CastRay(playerTileX, playerTileY, MathHelper.ToRadians(angle), viewDistance);
            }
        }

        // TODO: Refactor this function to use a more efficient algorithm, maybe Bresenham's line algorithm?
        // May be analysing the same tile multiple times
        private void CastRay(int startX, int startY, float angle, int viewDistance)
        {
            float x = startX + 0.5f;
            float y = startY + 0.5f;

            float deltaX = (float)Math.Cos(angle);
            float deltaY = (float)Math.Sin(angle);

            for (int i = 0; i < viewDistance; i++)
            {
                int tileX = (int)x;
                int tileY = (int)y;

                if (tileX < 0 || tileY < 0 || tileX >= mapWidth || tileY >= mapHeight)
                    break;

                if (_tilemap.Map[tileX, tileY] == null)
                    break;

                // Stop ray if it hits a blocked tile
                if (_tilemap.Map[tileX, tileY].IsBlocked)
                    break;

                if (_tilemap.Map[tileX, tileY].DungeonTileType == DungeonTileType.Wall)
                {
                    _tilemap.Map[tileX, tileY].IsVisible = true;
                    _tilemap.Map[tileX, tileY].IsExplored = true;
                    break;
                }

                _tilemap.Map[tileX, tileY].IsVisible = true;
                _tilemap.Map[tileX, tileY].IsExplored = true;

                x += deltaX;
                y += deltaY;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());

            foreach (var tile in _tilemap.Map)
            {
                if (tile == null)
                    continue;

                tile.Draw(gameTime);
            }

            foreach (var tile in _populationMap.Map)
            {
                if (tile == null)
                    continue;

                tile.Draw(gameTime);
            }

            // TODO: SEEMINGLY UNNEEDED CODE?s
            //_gameObjects
            //    .Select(x => x.Value)
            //    .OrderByDescending(x => x.ZIndex)
            //    .ToList()
            //    .ForEach(x => x.Draw(gameTime));

            // Draw interactables
            foreach (var interactable in Interactables)
            {
                interactable.Draw(gameTime);
            }

            Global.PlayerObj.Draw(gameTime);

            Global.SpriteBatch.End();

            if (Global.DRAW_INSTANCE_EXIT_LOCATIONS_OUTLINE)
            {
                foreach (var exitPoint in ExitLocations)
                {
                    Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
                    Global.SpriteBatch.Draw(_debugTexture, exitPoint.Value, Color.White);
                    Global.SpriteBatch.End();
                }
            }

            // ScreenObjects
            _screenObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));
        }

        public override void PrepareForPlayerEntry()
        {
            base.PrepareForPlayerEntry();

            Global.PlayerObj.WorldPosition = new Vector2(EntryLocation.X, EntryLocation.Y);

            // TEMP
            Global.PlayerObj.MovementSpeed = 150;

            Global.CameraMinimap.OrthoCamera.Zoom = 0.25f;
        }

        public override void PrepareForPlayerExit()
        {
            base.PrepareForPlayerExit();

            Global.PlayerObj.MovementSpeed = 2000;
            Global.CameraMinimap.OrthoCamera.Zoom = 0.1f;
        }

        public void DrawMinimap(GameTime gameTime)
        {
            foreach (var tile in _tilemap.Map)
            {
                if (tile == null)
                    continue;

                tile.DrawMinimap(gameTime);
            }
        }
    }
}
