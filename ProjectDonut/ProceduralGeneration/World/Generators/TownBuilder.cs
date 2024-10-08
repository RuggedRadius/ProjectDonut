using ProjectDonut.Core.Sprites;
using ProjectDonut.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Penumbra;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDonut.ProceduralGeneration.World.Generators
{
    public class TownBuilder
    {
        private WorldMapSettings Settings;
        private Random _random = new Random();

        public TownBuilder(WorldMapSettings settings)
        {
            Settings = settings;
        }

        public Tilemap BuildTown(ref WorldChunk chunk)
        {
            var townTileMap = new Tilemap(Settings.Width, Settings.Height);
            townTileMap.WorldPosition = new Vector2(chunk.WorldCoordX, chunk.WorldCoordY);

            // get position
            var localPosition = new Vector2(
                (Settings.Width * Settings.TileSize) / 4,
                (Settings.Height * Settings.TileSize) / 4);

            var position = new Vector2(
                chunk.WorldCoordX + localPosition.X,
                chunk.WorldCoordY + localPosition.Y);

            var width = 10;
            var height = 10;

            // Check for placement
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (chunk.Tilemaps["base"].Map[i, j].WorldTileType == WorldTileType.Water)
                    {
                        return null;
                    }
                }
            }

            // Place floor
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var tile = new Tile(false)
                    {
                        ChunkX = chunk.ChunkCoordX,
                        ChunkY = chunk.ChunkCoordY,
                        xIndex = i,
                        yIndex = j,
                        LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize) + localPosition,
                        Size = new Vector2(Global.TileSize, Global.TileSize),
                        Texture = SpriteLib.Town.Floor["floor-c"],
                        TileType = TileType.Instance,
                        IsExplored = true
                    };

                    townTileMap.Map[i, j] = tile;
                }
            }

            return townTileMap;
        }

        public void Build(ref WorldChunk chunk)
        {
            // Create floor tile map
            chunk.TownFloor = new List<Tilemap>();
            //chunk.TownFloor = new Tilemap(Settings.Width, Settings.Height);
            

            // Choose center point in local space
            var localPosition = new Vector2(
                _random.Next((Settings.Width / 2) * Settings.TileSize) + (Settings.Width / 4),
                _random.Next((Settings.Height / 2) * Settings.TileSize) + (Settings.Height / 4));

            var worldPosition = new Vector2(
                chunk.WorldCoordX + localPosition.X,
                chunk.WorldCoordY + localPosition.Y);

            // Generate building plots around center
            var plots = CreatePlots(worldPosition, localPosition, ref chunk);

            FloorPlots(plots, worldPosition, ref chunk);
        }

        private void FloorPlots(List<Rectangle> plots, Vector2 worldPosition, ref WorldChunk chunk)
        {
            var tiles = new List<Texture2D> 
            {
                SpriteLib.Town.Floor["floor-c"],
                SpriteLib.Town.Walls["wall-n"],
                SpriteLib.Town.Stairs["stairs-top-02"],

            };
            var tempCounter = 0;

            foreach (var plot in plots)
            {
                var tm = new Tilemap(plot.Width / Global.TileSize, plot.Height / Global.TileSize);
                tm.WorldPosition = new Vector2(chunk.WorldCoordX, chunk.WorldCoordY);
                chunk.TownFloor.Add(tm);

                var plotWidthInTiles = plot.Width / Global.TileSize;
                var plotHeightInTiles = plot.Height / Global.TileSize;

                var startX = ((plot.X - chunk.WorldCoordX) / Global.TileSize);
                var startY = ((plot.Y - chunk.WorldCoordY) / Global.TileSize);

                for (int i = 0; i < 0 + plotWidthInTiles; i++)
                {
                    for (int j = 0; j < 0 + plotHeightInTiles; j++)
                    {
                        var localX = ((plot.X - chunk.WorldCoordX)) + (i * Global.TileSize);
                        var localY = ((plot.Y - chunk.WorldCoordY)) + (j * Global.TileSize);

                        var tile = new Tile(false)
                        {
                            ChunkX = chunk.ChunkCoordX,
                            ChunkY = chunk.ChunkCoordY,
                            xIndex = i,
                            yIndex = j,
                            //LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize) + localPosition,
                            LocalPosition = new Vector2(localX, localY),
                            Size = new Vector2(Global.TileSize, Global.TileSize),
                            //Texture = SpriteLib.Town.Floor["floor-c"],
                            Texture = tiles[tempCounter],
                            TileType = TileType.Instance,
                            IsExplored = true
                        };

                        if (tm.Map[i, j] == null)
                            tm.Map[i, j] = tile;
                    }
                }

                tempCounter++;
            }
        }

        private List<Rectangle> CreatePlots(Vector2 worldPosition, Vector2 localPosition, ref WorldChunk chunk)
        {
            var spreadFromCenter = 20 * Global.TileSize;
            var plots = new List<Rectangle>();
            for (int i = 0; i < 3; i++)
            {
                var houseWidth = _random.Next(5, 10);
                var houseHeight = _random.Next(5, 10);

                var offsetFromCenterX = _random.Next(-spreadFromCenter, spreadFromCenter);
                var offsetFromCenterY = _random.Next(-spreadFromCenter, spreadFromCenter);

                var plot = new Rectangle(
                    (int)worldPosition.X + offsetFromCenterX, //+ (int)localPosition.X,
                    (int)worldPosition.Y + offsetFromCenterY, //+ (int)localPosition.Y,
                    houseWidth * Global.TileSize,
                    houseHeight * Global.TileSize);

                var plotViable = true;

                var testCorners = new List<Vector2> {
                    new Vector2(plot.X, plot.Y),
                    new Vector2(plot.X + plot.Width, plot.Y),
                    new Vector2(plot.X, plot.Y + plot.Height),
                    new Vector2(plot.X + plot.Width, plot.Y + plot.Height)
                };

                foreach (var testCorner in testCorners)
                {
                    if (chunk.ChunkBounds.Contains(testCorner) == false)
                    {
                        plotViable = false;
                    }
                }

                if (plotViable)
                    plots.Add(plot);
            }

            return plots;
        }
    }
}
