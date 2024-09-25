using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Core.Sprites;
using ProjectDonut.GameObjects.Doodads.Chests;
using ProjectDonut.GameObjects.Doodads;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.Interfaces;
using ProjectDonut.NPCs.Enemy;
using ProjectDonut.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using ProjectDonut.GameObjects.Doodads.Barrels;

namespace ProjectDonut.ProceduralGeneration.Dungeons.DungeonPopulation
{
    public class DungeonPopulater
    {
        private int[,] _datamap;
        private int MapWidth;
        private int MapHeight;

        private string[,] _popData;

        private Random _random = new Random();

        public DungeonPopulater(int[,] datamap)
        {
            _datamap = datamap;

            MapWidth = _datamap.GetLength(0);
            MapHeight = _datamap.GetLength(1);
        }

        public void PopulateDungeon(DungeonLevelSettings settings, ref DungeonScene scene)
        {
            _popData = new string[MapWidth, MapHeight];

            // Populate the dungeon with enemies, items, etc
            PlaceStairs();
            PlaceInteractableDoodads(ref scene);
        }

        private void PlaceInteractableDoodads(ref DungeonScene scene)
        {
            var allFloorCoords = GetAllFloorCoords();

            PlaceBarrels(50, ref scene, ref allFloorCoords);
            PlaceChests(50, ref scene, ref allFloorCoords);
        }

        public List<IGameObject> CreateEnemies(DungeonLevelSettings settings)
        {
            var enemies = new List<IGameObject>();
            var allFloorCoords = GetAllFloorCoords();

            for (int i = 0; i < settings.EnemyCount; i++)
            {
                var randomIndex = _random.Next(0, allFloorCoords.Count);
                var randomCoord = allFloorCoords[randomIndex];

                var enemy = new OrcGrunt()
                {
                    WorldPosition = new Vector2(randomCoord.x * Global.TileSize, randomCoord.y * Global.TileSize),
                    ZIndex = 0
                };

                enemies.Add(enemy);

                allFloorCoords.RemoveAt(randomIndex);
            }

            enemies.ForEach(x => x.Initialize());
            enemies.ForEach(x => x.LoadContent());

            return enemies;
        }

        private List<(int x, int y)> GetAllFloorCoords()
        {
            var floorCoords = new List<(int x, int y)>();

            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    if (_datamap[i, j] == 2)
                    {
                        floorCoords.Add((i, j));
                    }
                }
            }

            return floorCoords;
        }

        private void PlaceBarrels(int count, ref DungeonScene scene, ref List<(int x, int y)> allFloorCoords)
        {
            for (int i = 0; i < count; i++)
            {
                var randomIndex = _random.Next(0, allFloorCoords.Count);
                var coord = allFloorCoords[randomIndex];

                var rect = new Rectangle(
                    (int)coord.x * Global.TileSize,
                    (int)coord.y * Global.TileSize,
                    Global.TileSize,
                    Global.TileSize);

                var items = new List<InventoryItem>();
                items.Add(new InventoryItem() { Name = "Stone", Icon = SpriteLib.UI.Items["rock"], ItemType = ItemType.Consumable, Quantity = 5 });
                items.Add(new InventoryItem() { Name = "Wood Log", Icon = SpriteLib.UI.Items["wood-log"], ItemType = ItemType.Consumable, Quantity = 5 });

                scene.Interactables.Add(new Barrel(rect, items));

                allFloorCoords.RemoveAt(randomIndex);
            }
        }

        private void PlaceStairs()
        {
            var startLocation = FindSuitableStairLocation();

            _popData[startLocation.Item1 - 1, startLocation.Item2 - 1] = "stairs-nw";
            _popData[startLocation.Item1 - 0, startLocation.Item2 - 1] = "stairs-n";
            _popData[startLocation.Item1 + 1, startLocation.Item2 - 1] = "stairs-ne";

            _popData[startLocation.Item1 - 1, startLocation.Item2 - 0] = "stairs-w";
            _popData[startLocation.Item1 - 0, startLocation.Item2 - 0] = "stairs-c";
            _popData[startLocation.Item1 + 1, startLocation.Item2 - 0] = "stairs-e";

            _popData[startLocation.Item1 - 1, startLocation.Item2 + 1] = "stairs-sw";
            _popData[startLocation.Item1 - 0, startLocation.Item2 + 1] = "stairs-s";
            _popData[startLocation.Item1 + 1, startLocation.Item2 + 1] = "stairs-se";
        }

        private (int, int) FindSuitableStairLocation()
        {
            var startLocation = (0, 0);
            var suitableLocationFound = false;

            do
            {
                var x = _random.Next(0, MapWidth);
                var y = _random.Next(0, MapHeight);

                if (_datamap[x, y] == 2)
                {
                    var surroundingGridClear = true;
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (x + i < 0 || x + i >= MapWidth || y + j < 0 || y + j >= MapHeight)
                                continue;

                            if (_datamap[x + i, y + j] != 2)
                            {
                                surroundingGridClear = false;
                            }
                        }
                    }

                    if (surroundingGridClear)
                    {
                        startLocation = (x, y);
                        suitableLocationFound = true;
                    }
                }
            }
            while (!suitableLocationFound);

            return startLocation;
        }

        public Tilemap CreateTileMap()
        {
            var width = _popData.GetLength(0);
            var height = _popData.GetLength(1);

            var tilemap = new Tilemap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (string.IsNullOrWhiteSpace(_popData[i, j]))
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
                        Texture = DetermineTexture(_popData[i, j]),
                        TileType = TileType.Instance,
                    };
                    tile.Initialize();
                    tilemap.Map[i, j] = tile;
                }
            }

            return tilemap;
        }

        public void PlaceChests(int count, ref DungeonScene scene, ref List<(int x, int y)> allFloorCoords)
        {
            for (int i = 0; i < count; i++)
            {
                var randomIndex = _random.Next(0, allFloorCoords.Count);
                var coord = allFloorCoords[randomIndex];

                var rect = new Rectangle(
                    (int)coord.x * Global.TileSize,
                    (int)coord.y * Global.TileSize,
                    Global.TileSize,
                    Global.TileSize);

                var items = new List<InventoryItem>();
                items.Add(new InventoryItem() { Name = "Stone", Icon = SpriteLib.UI.Items["rock"], ItemType = ItemType.Consumable, Quantity = 50 });
                items.Add(new InventoryItem() { Name = "Wood Log", Icon = SpriteLib.UI.Items["wood-log"], ItemType = ItemType.Consumable, Quantity = 50 });

                scene.Interactables.Add(new Chest(rect, items));

                allFloorCoords.RemoveAt(randomIndex);
            }
        }

        private Texture2D DetermineTexture(string popValue)
        {
            switch (popValue)
            {
                case "stairs-nw":
                    return SpriteLib.Dungeon.DungeonSprites["stairs-nw"][0];

                case "stairs-n":
                    return SpriteLib.Dungeon.DungeonSprites["stairs-n"][0];

                case "stairs-ne":
                    return SpriteLib.Dungeon.DungeonSprites["stairs-ne"][0];

                case "stairs-w":
                    return SpriteLib.Dungeon.DungeonSprites["stairs-w"][0];

                case "stairs-c":
                    return SpriteLib.Dungeon.DungeonSprites["stairs-c"][0];

                case "stairs-e":
                    return SpriteLib.Dungeon.DungeonSprites["stairs-e"][0];

                case "stairs-sw":
                    return SpriteLib.Dungeon.DungeonSprites["stairs-sw"][0];

                case "stairs-s":
                    return SpriteLib.Dungeon.DungeonSprites["stairs-s"][0];

                case "stairs-se":
                    return SpriteLib.Dungeon.DungeonSprites["stairs-se"][0];

                default:
                    return null;
            }
        }

        public (int, int) GetStairsLocation()
        {
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    if (_popData[i, j] == "stairs-s")
                    {
                        return (i * Global.TileSize, j * Global.TileSize);
                    }
                }
            }

            return (0, 0); // uh oh..
        }

        public List<(int, int)> GetExitLocations()
        {
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    if (_popData[i, j] == "stairs-n")
                    {
                        return new List<(int, int)> { (i * Global.TileSize, j * Global.TileSize)};
                    }
                }
            }

            return new List<(int, int)> { (0, 0) }; ; // uh oh..
        }
    }
}
