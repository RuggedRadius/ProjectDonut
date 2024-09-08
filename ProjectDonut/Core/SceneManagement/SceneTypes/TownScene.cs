using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.Dungeons;
using ProjectDonut.ProceduralGeneration.Dungeons.BSP;
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

        public TownScene()
        {
            SceneType = SceneType.Town;
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
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var tile in _tilemap.Map)
            {
                tile.Update(gameTime);
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
            Global.SpriteBatch.End();
        }

        public override void PrepareForPlayerEntry()
        {
            base.PrepareForPlayerEntry();
        }

        public override void PrepareForPlayerExit()
        {
            base.PrepareForPlayerExit();
        }

        public void GenerateDataMap()
        {
            var width = 100;
            var height = 100;

            // Generate data map
            var areaGens = _bsp.GenerateRooms(width, height);
            var areas = _bsp.SquashRooms(areaGens[areaGens.Count - 1], width, height);
            var map = _bsp.CreateDataMap(areas, width, height);

            // Link rooms
            var rects = areas.Select(x => x.Bounds).ToList();
            var rectLinker = new RectangleLinker();
            var links = rectLinker.LinkRectangles(rects);
            var linkages = _bsp.LinkAllRooms(links, map);
            map = BSP.MergeArrays(map, linkages);

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

        private Texture2D DetermineTexture(int[,] map, int width, int height)
        {
            var cellValue = map[width, height];

            switch (cellValue)
            {
                case 0:
                    return Global.SpriteLibrary.TownSprites["dirt-c"];

                case 1:
                    return Global.SpriteLibrary.TownSprites["fence-s"];

                case 2:
                        return Global.SpriteLibrary.TownSprites["grass-c"];

                default:
                    return Global.SpriteLibrary.TownSprites["dirt-c"];
            }
        }
    }
}
