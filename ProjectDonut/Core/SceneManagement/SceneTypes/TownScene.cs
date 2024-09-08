using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.BSP;
using ProjectDonut.ProceduralGeneration.Dungeons;
using ProjectDonut.ProceduralGeneration.World.Structures;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectDonut.Core.SceneManagement.SceneTypes
{
    internal class TownScene : BaseScene
    {
        public int[,] DataMap;
        private BSP _bsp;

        private Tilemap _tilemap;

        private Dictionary<string, Rectangle> ExitLocations;

        public List<IGameObject> NPCs { get; set; }

        private WorldStructure _worldStructure;

        public TownScene(WorldStructure worldStructure)
        {
            SceneType = SceneType.Town;
            _worldStructure = worldStructure;

            _bsp = new BSP();
            ExitLocations = new Dictionary<string, Rectangle>();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            GenerateDataMap();
            GenerateTileMap(DataMap);
            GenerateExitLocations();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var tile in _tilemap.Map)
            {
                tile.Update(gameTime);
            }

            foreach (var exitPoint in ExitLocations)
            {
                if (exitPoint.Value.Contains(Global.Player.WorldPosition))
                {
                    Global.SceneManager.SetCurrentScene(Global.SceneManager.Scenes["world"]);

                    var worldScene = (WorldScene)Global.SceneManager.CurrentScene;

                    switch (exitPoint.Key)
                    {
                        case "north":
                            worldScene.LastExitLocation = new Rectangle(
                                (int)_worldStructure.TextureBounds.Left + (_worldStructure.TextureBounds.Width / 2),
                                (int)_worldStructure.TextureBounds.Top - Global.TileSize,
                                Global.TileSize,
                                Global.TileSize);
                            break;

                        case "south":
                            worldScene.LastExitLocation = new Rectangle(
                                (int)_worldStructure.TextureBounds.Left + (_worldStructure.TextureBounds.Width / 2),
                                (int)_worldStructure.TextureBounds.Bottom + Global.TileSize,
                                Global.TileSize,
                                Global.TileSize);
                            break;

                        case "east":
                            worldScene.LastExitLocation = new Rectangle(
                                (int)_worldStructure.TextureBounds.Right + Global.TileSize,
                                (int)_worldStructure.TextureBounds.Top + (_worldStructure.TextureBounds.Height / 2),
                                Global.TileSize,
                                Global.TileSize);
                            break;

                        case "west":
                            worldScene.LastExitLocation = new Rectangle(
                                (int)_worldStructure.TextureBounds.Left - Global.TileSize,
                                (int)_worldStructure.TextureBounds.Top + (_worldStructure.TextureBounds.Height / 2),
                                Global.TileSize,
                                Global.TileSize);
                            break;
                    }

                    Global.SceneManager.CurrentScene.PrepareForPlayerEntry();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            
            foreach (var tile in _tilemap.Map)
            {
                if (tile == null)
                {
                    continue;
                }

                tile.Draw(gameTime);
            }

            if (Global.DRAW_INSTANCE_EXIT_LOCATIONS_OUTLINE)
            {
                foreach (var exitPoint in ExitLocations)
                {
                    Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, exitPoint.Value, Color.White);
                }
            }

            Global.SpriteBatch.End();
        }

        public override void PrepareForPlayerEntry()
        {
            base.PrepareForPlayerEntry();

            Global.Player.WorldPosition = new Vector2(500, 500); // TEMP
        }

        public override void PrepareForPlayerExit()
        {
            base.PrepareForPlayerExit();
        }

        private void GenerateExitLocations()
        {
            // North
            var rectNorth = new Rectangle(
                _tilemap.Map[0,0].Bounds.X,
                _tilemap.Map[0,0].Bounds.Y - Global.TileSize,
                Global.TileSize * _tilemap.Map.GetLength(0),
                Global.TileSize);

            ExitLocations.Add("north", rectNorth);

            // South
            var rectSouth = new Rectangle(
                _tilemap.Map[0, 0].Bounds.X,
                rectNorth.Y + ((_tilemap.Map.GetLength(0) + 1) * Global.TileSize),
                Global.TileSize * _tilemap.Map.GetLength(0),
                Global.TileSize);

            ExitLocations.Add("south", rectSouth);

            // East
            var rectEast = new Rectangle(
                _tilemap.Map[0, 0].Bounds.Right + (Global.TileSize * _tilemap.Map.GetLength(0)),
                _tilemap.Map[_tilemap.Map.GetLength(0) - 1, 0].Bounds.Y,
                Global.TileSize,
                Global.TileSize * _tilemap.Map.GetLength(1));

            ExitLocations.Add("east", rectEast);

            // West
            var rectWest = new Rectangle(
                _tilemap.Map[0, 0].Bounds.X - Global.TileSize,
                _tilemap.Map[0, 0].Bounds.Y,
                Global.TileSize,
                Global.TileSize * _tilemap.Map.GetLength(1));

            ExitLocations.Add("west", rectWest);
        }

        public void GenerateDataMap()
        {
            var width = 100;
            var height = 100;

            // Generate data map
            var areaGens = _bsp.GenerateRooms(width, height);
            var areas = _bsp.SquashRooms(areaGens[areaGens.Count - 1], width, height);
            var map = _bsp.CreateDataMap(areas, width, height, 1);

            //// Link rooms
            //var rects = areas.Select(x => x.Bounds).ToList();
            //var rectLinker = new RectangleLinker();
            //var links = rectLinker.LinkRectangles(rects);
            //var linkages = _bsp.LinkAllRooms(links, map);
            //map = BSP.MergeArrays(map, linkages);

            DataMap = map;
        }

        private void GenerateTileMap(int[,] data)
        {
            var width = data.GetLength(0);
            var height = data.GetLength(1);

            var tilemap = new Tilemap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (data[i, j] == 0)
                    {
                        continue;
                    }

                    var tile = new Tile(false)
                    {
                        ChunkX = 0,
                        ChunkY = 0,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize),
                        Size = new Vector2(Global.TileSize, Global.TileSize),
                        Texture = DetermineTexture(data, i, j),
                        TileType = TileType.Instance,
                        IsExplored = true
                        //DungeonTileType = DetermineTileType(data, i, j)
                    };

                    tilemap.Map[i, j] = tile;
                }
            }

            _tilemap = tilemap;
        }

        private Texture2D DetermineTexture(int[,] map, int x, int y)
        {
            var cellValue = map[x, y];

            switch (cellValue)
            {
                case 0:
                    return Global.SpriteLibrary.TownSprites["dirt-c"];

                case 1:
                    return Global.SpriteLibrary.TownSprites["dirt-c"];

                case 2:
                    if (IsNeighbourCellRoad(map, x, y))
                    {
                        return Global.SpriteLibrary.TownSprites["fence-s"];
                    }
                    else
                    {
                        return Global.SpriteLibrary.TownSprites["grass-c"];
                    }

                default:
                    return Global.SpriteLibrary.TownSprites["dirt-c"];
            }
        }

        private bool IsNeighbourCellRoad(int[,] map, int x, int y)
        {
            var n = 0;
            var e = 0;
            var s = 0; 
            var w = 0;

            if (y > 0)
                n = map[x, y - 1];
            if (x > 0)
                w = map[x - 1, y];
            if (x < map.GetLength(0) - 1)
                e = map[x + 1, y];
            if (y < map.GetLength(1) - 1)
                s = map[x, y + 1];

            return n == 1 || e == 1 || s == 1 || w == 1;
        }
    }
}
