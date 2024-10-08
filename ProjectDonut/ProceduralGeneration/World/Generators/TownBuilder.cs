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
using ProjectDonut.ProceduralGeneration.World.MineableItems;

namespace ProjectDonut.ProceduralGeneration.World.Generators
{
    public class Town
    {
        public Vector2 CenterWorldPosition { get; set; }
        public Vector2 CenterLocalPosition { get; set; }

        public List<TownPlot> Plots { get; set; }

        public Dictionary<string, Tilemap> Tilemaps { get; set; }

        public Town(Vector2 centerWorldPosition, Vector2 centerLocalPosition)
        {
            CenterWorldPosition = centerWorldPosition;
            CenterLocalPosition = centerLocalPosition;

            Plots = new List<TownPlot>();
            Tilemaps = new Dictionary<string, Tilemap>();
        }

        public void Update(GameTime gameTime)
        {
            foreach (var tilemap in Tilemaps)
            {
                tilemap.Value.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            //Tilemaps["base"].Draw(gameTime);
            Tilemaps["road"].Draw(gameTime);
            Tilemaps["floor"].Draw(gameTime);
        }
    }

    public class TownPlot
    {
        public Vector2 WorldPosition { get; set; }
        public Vector2 LocalPosition { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public Rectangle LocalBounds
        {
            get
            {
                return new Rectangle((int)LocalPosition.X, (int)LocalPosition.Y, Width, Height);
            }
        }

        public Rectangle WorldBounds
        {
            get
            {
                return new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, Width, Height);
            }
        }
    }

    public class TownBuilder
    {
        private WorldMapSettings Settings;
        private Random _random = new Random();

        private int spreadFromCenter = 50 * Global.TileSize;

        public TownBuilder(WorldMapSettings settings)
        {
            Settings = settings;
        }

        public void Build(ref WorldChunk chunk)
        {
            var localCenterPosition = new Vector2(
                _random.Next((Settings.Width / 2) * Settings.TileSize) + (Settings.Width / 4),
                _random.Next((Settings.Height / 2) * Settings.TileSize) + (Settings.Height / 4));
            var worldCenterPosition = new Vector2(
                chunk.WorldCoordX + localCenterPosition.X,
                chunk.WorldCoordY + localCenterPosition.Y);

            var town = new Town(worldCenterPosition, localCenterPosition);
            chunk.Town = town;

            CreatePlots(10, ref chunk);
            FloorPlots(ref chunk);

            // Clear trees around center point and each plot in circular fashion
            ClearObstaclesAroundTown(ref chunk);

            // Create roads between plots and the center point
            CreateRoadsBetweenPlotsAndCenterPoint(ref chunk);

            // Create roads between plots
            CreateRoadsBetweenPlots(ref chunk);

            CreateBaseDirt(ref chunk);
        }

        private void FloorPlots(ref WorldChunk chunk)
        {
            var tilemap = new Tilemap(chunk.Width, chunk.Height);
            tilemap.WorldPosition = new Vector2(chunk.WorldCoordX, chunk.WorldCoordY);

            foreach (var plot in chunk.Town.Plots)
            {
                for (int i = plot.LocalBounds.Left; i <= plot.LocalBounds.Right; i += Global.TileSize)
                {
                    for (int j = plot.LocalBounds.Top; j <= plot.LocalBounds.Bottom; j += Global.TileSize)
                    {
                        tilemap.Map[i / Global.TileSize, j / Global.TileSize] = new Tile(false)
                        {
                            ChunkX = chunk.ChunkCoordX,
                            ChunkY = chunk.ChunkCoordY,
                            xIndex = i / Global.TileSize,
                            yIndex = j / Global.TileSize,
                            LocalPosition = new Vector2(i, j),
                            Size = new Vector2(Global.TileSize, Global.TileSize),
                            Texture = SpriteLib.Town.Floor["floor-c"],
                            TileType = TileType.Instance,
                            IsExplored = true
                        };
                    }
                }
            }

            chunk.Town.Tilemaps.Add("floor", tilemap);
            return;
        }

        private void CreatePlots(int plotCount, ref WorldChunk chunk)
        {
            chunk.Town.Plots = new List<TownPlot>();

            for (int i = 0; i < plotCount; i++)
            {
                var plotCreated = false;
                var maxTries = 100;

                while (plotCreated == false)
                {
                    var plotWidth = _random.Next(5, 10);
                    var plotHeight = _random.Next(5, 10);

                    var offsetFromCenterX = _random.Next(-spreadFromCenter, spreadFromCenter);
                    var offsetFromCenterY = _random.Next(-spreadFromCenter, spreadFromCenter);

                    var townPlot = new TownPlot()
                    {
                        WorldPosition = chunk.Town.CenterWorldPosition + new Vector2(offsetFromCenterX, offsetFromCenterY),
                        LocalPosition = chunk.Town.CenterLocalPosition + new Vector2(offsetFromCenterX, offsetFromCenterY),
                        Width = plotWidth * Global.TileSize,
                        Height = plotHeight * Global.TileSize
                    };

                    maxTries--;

                    if (IsPlotViable(townPlot, chunk))
                    {
                        chunk.Town.Plots.Add(townPlot);
                        plotCreated = true;
                    }
                    else
                    {
                        if (maxTries <= 0)
                            break;
                    }
                }
            }
        }

        private bool IsPlotViable(TownPlot townPlot, WorldChunk chunk)
        {
            // Is plot within chunk bounds
            var testCorners = new List<Vector2> {
                new Vector2(townPlot.WorldBounds.Left, townPlot.WorldBounds.Top),
                new Vector2(townPlot.WorldBounds.Left, townPlot.WorldBounds.Bottom),
                new Vector2(townPlot.WorldBounds.Right, townPlot.WorldBounds.Top),
                new Vector2(townPlot.WorldBounds.Right, townPlot.WorldBounds.Bottom)
            };
            foreach (var testCorner in testCorners)
            {
                if (chunk.ChunkBounds.Contains(testCorner) == false)
                {
                    return false;
                }
            }

            // Is plot intersecting with other plots
            foreach (var p in chunk.Town.Plots)
            {
                if (p.LocalBounds.Intersects(townPlot.LocalBounds))
                {
                    return false;
                }
            }

            // Is plot intersecting with water
            for (int i = townPlot.LocalBounds.Left; i <= townPlot.LocalBounds.Right; i += Global.TileSize)
            {
                for (int j = townPlot.LocalBounds.Top; j <= townPlot.LocalBounds.Bottom; j += Global.TileSize)
                {
                    if (chunk.Tilemaps["base"].Map[i / Global.TileSize, j / Global.TileSize].WorldTileType == WorldTileType.Water)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    
        private void CreateRoadsBetweenPlotsAndCenterPoint(ref WorldChunk chunk)
        {
            // Create roads between plots and the center point
            var tilemap = new Tilemap(chunk.Width, chunk.Height);
            tilemap.WorldPosition = new Vector2(chunk.WorldCoordX, chunk.WorldCoordY);

            foreach (var plot in chunk.Town.Plots)
            {
                var center = chunk.Town.CenterLocalPosition;
                var plotCenter = plot.LocalPosition + new Vector2(plot.Width / 2, plot.Height / 2);

                var road = new List<Vector2>();
                road.Add(center);
                road.Add(plotCenter);

                foreach (var point in road)
                {
                    var x = (int)point.X;
                    var y = (int)point.Y;

                    while (x != plotCenter.X || y != plotCenter.Y)
                    {
                        if (x < plotCenter.X)
                        {
                            x++;
                        }
                        else if (x > plotCenter.X)
                        {
                            x--;
                        }

                        if (y < plotCenter.Y)
                        {
                            y++;
                        }
                        else if (y > plotCenter.Y)
                        {
                            y--;
                        }

                        if (chunk.Tilemaps["base"].Map[x / Global.TileSize, y / Global.TileSize].WorldTileType == WorldTileType.Water)
                            continue;

                        tilemap.Map[x / Global.TileSize, y / Global.TileSize] = new Tile(false)
                        {
                            ChunkX = chunk.ChunkCoordX,
                            ChunkY = chunk.ChunkCoordY,
                            xIndex = x / Global.TileSize,
                            yIndex = y / Global.TileSize,
                            LocalPosition = new Vector2(x, y),
                            Size = new Vector2(Global.TileSize, Global.TileSize),
                            //Texture = SpriteLib.Town.Road["road-c"],
                            Texture = Global.MISSING_TEXTURE,
                            TileType = TileType.Instance,
                            IsExplored = true
                        };
                    }
                }
            }

            chunk.Town.Tilemaps.Add("road", tilemap);
        }
    
        private void CreateRoadsBetweenPlots(ref WorldChunk chunk)
        {
            var tilemap = chunk.Town.Tilemaps["road"];

            foreach (var plot in chunk.Town.Plots)
            {
                foreach (var plot2 in chunk.Town.Plots)
                {
                    if (plot == plot2)
                        continue;

                    var target = plot2;
                    var plotCenter = plot.LocalPosition + new Vector2(plot.Width / 2, plot.Height / 2);

                    var road = new List<Vector2>();
                    road.Add(target.LocalPosition);
                    road.Add(plotCenter);

                    foreach (var point in road)
                    {
                        var x = (int)point.X;
                        var y = (int)point.Y;

                        while (x != plotCenter.X || y != plotCenter.Y)
                        {
                            if (x < plotCenter.X)
                            {
                                x++;
                            }
                            else if (x > plotCenter.X)
                            {
                                x--;
                            }

                            if (y < plotCenter.Y)
                            {
                                y++;
                            }
                            else if (y > plotCenter.Y)
                            {
                                y--;
                            }

                            tilemap.Map[x / Global.TileSize, y / Global.TileSize] = new Tile(false)
                            {
                                ChunkX = chunk.ChunkCoordX,
                                ChunkY = chunk.ChunkCoordY,
                                xIndex = x / Global.TileSize,
                                yIndex = y / Global.TileSize,
                                LocalPosition = new Vector2(x, y),
                                Size = new Vector2(Global.TileSize, Global.TileSize),
                                //Texture = SpriteLib.Town.Road["road-c"],
                                Texture = Global.MISSING_TEXTURE,
                                TileType = TileType.Instance,
                                IsExplored = true
                            };
                        }
                    }
                }
            }
        }
    
        private void CreateBaseDirt(ref WorldChunk chunk)
        {
            var dirtTilemap = new Tilemap(chunk.Width, chunk.Height);
            dirtTilemap.WorldPosition = new Vector2(chunk.WorldCoordX, chunk.WorldCoordY);

            int pathWidth = 1;
            var obstaclesToClear = new List<IMineable>();

            for (int i = 0; i < chunk.Width; i++)
            {
                for (int j = 0; j < chunk.Height; j++)
                {
                    if (chunk.Town.Tilemaps["road"].Map[i, j] != null)
                    {                        
                        for (int x = -pathWidth; x <= pathWidth; x++)
                        {
                            for (int y = -pathWidth; y <= pathWidth; y++)
                            {
                                var coordX = i + x;
                                var coordY = j + y;

                                if (coordX < 0 || coordX >= chunk.Width || coordY < 0 || coordY >= chunk.Height)
                                    continue;

                                dirtTilemap.Map[i + x, j + y] = new Tile(false)
                                {
                                    ChunkX = chunk.ChunkCoordX,
                                    ChunkY = chunk.ChunkCoordY,
                                    xIndex = i + x,
                                    yIndex = j + y,
                                    LocalPosition = new Vector2((i + x) * Global.TileSize, (j + y) * Global.TileSize),
                                    Size = new Vector2(Global.TileSize, Global.TileSize),
                                    Texture = SpriteLib.Town.Terrain["dirt-c"],
                                    TileType = TileType.Instance,
                                    IsExplored = true,
                                    Bounds = new Rectangle(
                                        (i + x) * Global.TileSize,
                                        (j + y) * Global.TileSize, 
                                        Global.TileSize, 
                                        Global.TileSize)
                                };

                                foreach (var mineables in chunk.MineableObjects.Values)
                                {
                                    foreach (var mineable in mineables)
                                    {
                                        if (mineable.InteractBounds.Intersects(dirtTilemap.Map[i + x, j + y].Bounds))
                                        {
                                            obstaclesToClear.Add(mineable);
                                        }
                                    }
                                }
                            }
                        }                    
                    }
                }
            }

            chunk.Town.Tilemaps.Add("base", dirtTilemap);

            chunk.MineableObjects.Values.ToList().ForEach(x => x.RemoveAll(y => obstaclesToClear.Contains(y)));
        }
    
        private void ClearObstaclesAroundTown(ref WorldChunk chunk)
        {
            var obstaclesToClear = new List<IMineable>();

            foreach (var mineables in chunk.MineableObjects.Values)
            {
                foreach(var mineable in mineables)
                {
                    foreach (var plot in chunk.Town.Plots)
                    {
                        if (mineable.InteractBounds.Intersects(plot.WorldBounds))
                        {
                            obstaclesToClear.Add(mineable);
                        }
                    }
                }
            }

            chunk.MineableObjects.Values.ToList().ForEach(x => x.RemoveAll(y => obstaclesToClear.Contains(y)));
        }
    }
}
