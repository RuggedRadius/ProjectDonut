using ProjectDonut.Core.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Penumbra;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.ProceduralGeneration.World.MineableItems;
using MonoGame.Extended;
using ProjectDonut.Tools;

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
            Tilemaps["base"].Draw(gameTime);
            //Tilemaps["road"].Draw(gameTime);
            Tilemaps["fences"].Draw(gameTime);
            Tilemaps["floor"].Draw(gameTime);
            Tilemaps["walls"].Draw(gameTime);
            Tilemaps["roofs"].Draw(gameTime);

            //foreach (var plot in Plots)
            //{
            //    Global.SpriteBatch.DrawRectangle(plot.WorldBounds, Color.Red, 1);
            //    Global.SpriteBatch.DrawRectangle(plot.Building.WorldBounds, Color.Blue, 1);
            //}
        }
    }

    public class TownPlot
    {
        public Vector2 WorldPosition { get; set; }
        public Vector2 LocalPosition { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public TownBuilding Building { get; set; }

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

    public class TownBuilding
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

        // Settings
        private int spreadFromCenter = 50 * Global.TileSize;
        private int roadWidth = 1;
        private Vector2Int PlotSizeMin = new Vector2Int(10, 10);
        private Vector2Int PlotSizeMax = new Vector2Int(15, 15);
        private Vector2Int BuildingSizeMin = new Vector2Int(6, 6);
        //private Vector2Int BuildingSizeMax = new Vector2Int(9, 9);

        public TownBuilder(WorldMapSettings settings)
        {
            Settings = settings;
        }

        public void Build(ref WorldChunk chunk)
        {
            if (Global.WorldSettings.BUILD_TOWNS == false)
                return;

            var localCenterPosition = new Vector2(
                _random.Next((Settings.Width / 2) * Settings.TileSize) + (Settings.Width / 4),
                _random.Next((Settings.Height / 2) * Settings.TileSize) + (Settings.Height / 4));
            var worldCenterPosition = new Vector2(
                chunk.WorldCoordX + localCenterPosition.X,
                chunk.WorldCoordY + localCenterPosition.Y);

            var town = new Town(worldCenterPosition, localCenterPosition);
            chunk.Town = town;

            CreatePlots(10, ref chunk);
            CreateBuildingsInPlots(ref chunk);
            FloorBuildings(ref chunk);
            WallBuildings(ref chunk);
            FencePlots(ref chunk);
            CreateBuildingRoofs(ref chunk);
            PlaceDoorsOnBuildings(ref chunk);

            // Clear trees around center point and each plot in circular fashion
            ClearObstaclesAroundTown(ref chunk);

            // Create roads between plots and the center point
            CreateRoadsBetweenPlotsAndCenterPoint(ref chunk);

            // Create roads between plots
            CreateRoadsBetweenPlots(ref chunk);

            CreateBaseDirtAroundRoads(ref chunk);
            CreateDirtAroundPlots(ref chunk);
        }

        private void FloorBuildings(ref WorldChunk chunk)
        {
            var tilemap = new Tilemap(chunk.Width, chunk.Height);
            tilemap.WorldPosition = new Vector2(chunk.WorldCoordX, chunk.WorldCoordY);

            foreach (var plot in chunk.Town.Plots)
            {
                for (int i = plot.Building.LocalBounds.Left; i < plot.Building.LocalBounds.Right; i += Global.TileSize)
                {
                    for (int j = plot.Building.LocalBounds.Top; j < plot.Building.LocalBounds.Bottom; j += Global.TileSize)
                    {
                        tilemap.Map[i / Global.TileSize, j / Global.TileSize] = new Tile()
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
            RuleTiler.Town.ApplyRulesToBuildingForFloors(ref chunk);
        }

        private void WallBuildings(ref WorldChunk chunk)
        {
            var tilemap = new Tilemap(chunk.Width, chunk.Height);
            tilemap.WorldPosition = new Vector2(chunk.WorldCoordX, chunk.WorldCoordY);

            foreach (var plot in chunk.Town.Plots)
            {
                for (int i = plot.Building.LocalBounds.Left; i < plot.Building.LocalBounds.Right; i += Global.TileSize)
                {
                    for (int j = plot.Building.LocalBounds.Top; j < plot.Building.LocalBounds.Bottom; j += Global.TileSize)
                    {
                        if (i == plot.Building.LocalBounds.Left || i == plot.Building.LocalBounds.Right - Global.TileSize ||
                            j == plot.Building.LocalBounds.Top || j == plot.Building.LocalBounds.Bottom - Global.TileSize)
                        {
                            tilemap.Map[i / Global.TileSize, j / Global.TileSize] = new Tile()
                            {
                                ChunkX = chunk.ChunkCoordX,
                                ChunkY = chunk.ChunkCoordY,
                                xIndex = i / Global.TileSize,
                                yIndex = j / Global.TileSize,
                                LocalPosition = new Vector2(i, j),
                                Size = new Vector2(Global.TileSize, Global.TileSize),
                                Texture = SpriteLib.Town.Walls["wall-n"],
                                TileType = TileType.Instance,
                                IsExplored = true
                            };
                        }
                    }
                }
            }

            chunk.Town.Tilemaps.Add("walls", tilemap);
            RuleTiler.Town.ApplyRulesToBuildingForWalls(ref chunk);
        }

        private void FencePlots(ref WorldChunk chunk)
        {
            var tilemap = new Tilemap(chunk.Width, chunk.Height);
            tilemap.WorldPosition = new Vector2(chunk.WorldCoordX, chunk.WorldCoordY);

            foreach (var plot in chunk.Town.Plots)
            {
                for (int i = plot.LocalBounds.Left; i < plot.LocalBounds.Right; i += Global.TileSize)
                {
                    for (int j = plot.LocalBounds.Top; j < plot.LocalBounds.Bottom; j += Global.TileSize)
                    {
                        if (i == plot.LocalBounds.Left || i == plot.LocalBounds.Right - Global.TileSize ||
                            j == plot.LocalBounds.Top || j == plot.LocalBounds.Bottom - Global.TileSize)
                        {
                            tilemap.Map[i / Global.TileSize, j / Global.TileSize] = new Tile()
                            {
                                ChunkX = chunk.ChunkCoordX,
                                ChunkY = chunk.ChunkCoordY,
                                xIndex = i / Global.TileSize,
                                yIndex = j / Global.TileSize,
                                LocalPosition = new Vector2(i, j),
                                Size = new Vector2(Global.TileSize, Global.TileSize),
                                Texture = SpriteLib.Town.Fences["fence-n"],
                                TileType = TileType.Instance,
                                IsExplored = true
                            };
                        }
                    }
                }
            }

            chunk.Town.Tilemaps.Add("fences", tilemap);
            RuleTiler.Town.ApplyRulesToPlotForFences(ref chunk);
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
                    var plotWidth = _random.Next(PlotSizeMin.X, PlotSizeMax.X);
                    var plotHeight = _random.Next(PlotSizeMin.Y, PlotSizeMax.Y);

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

        private void CreateBuildingsInPlots(ref WorldChunk chunk)
        {
            foreach (var plot in chunk.Town.Plots)
            {
                var plotWidth = plot.Width / Global.TileSize;
                var plotHeight = plot.Height / Global.TileSize;

                // Randomise building size
                var buildingWidth = _random.Next(BuildingSizeMin.X, plotWidth - 2);
                var buildingHeight = _random.Next(BuildingSizeMin.Y, plotHeight - 2);

                if (buildingHeight > buildingWidth)
                {
                    if (buildingWidth % 2 != 0)
                        buildingWidth -= 1;

                    if (buildingHeight % 2 != 0)
                        buildingHeight -= 1;
                }



                // Randomise building position
                var buildingX = _random.Next(1, plotWidth - buildingWidth - 1);
                var buildingY = _random.Next(1, plotHeight - buildingHeight - 1);

                // Create building
                var building = new TownBuilding()
                {
                    WorldPosition = new Vector2(plot.WorldPosition.X + (buildingX * Global.TileSize), plot.WorldPosition.Y + (buildingY * Global.TileSize)),
                    LocalPosition = new Vector2(plot.LocalPosition.X + (buildingX * Global.TileSize), plot.LocalPosition.Y + (buildingY * Global.TileSize)),
                    Width = buildingWidth * Global.TileSize,
                    Height = buildingHeight * Global.TileSize
                };

                plot.Building = building;
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

                        tilemap.Map[x / Global.TileSize, y / Global.TileSize] = new Tile()
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

                            tilemap.Map[x / Global.TileSize, y / Global.TileSize] = new Tile()
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
    
        private void CreateBaseDirtAroundRoads(ref WorldChunk chunk)
        {
            var dirtTilemap = new Tilemap(chunk.Width, chunk.Height);
            dirtTilemap.WorldPosition = new Vector2(chunk.WorldCoordX, chunk.WorldCoordY);

            var obstaclesToClear = new List<IMineable>();

            for (int i = 0; i < chunk.Width; i++)
            {
                for (int j = 0; j < chunk.Height; j++)
                {
                    if (chunk.Town.Tilemaps["road"].Map[i, j] != null)
                    {
                        for (int x = -roadWidth; x <= roadWidth; x++)
                        {
                            for (int y = -roadWidth; y <= roadWidth; y++)
                            {
                                var coordX = i + x;
                                var coordY = j + y;

                                if (coordX < 0 || coordX >= chunk.Width || coordY < 0 || coordY >= chunk.Height)
                                    continue;

                                dirtTilemap.Map[i + x, j + y] = new Tile()
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

            if (obstaclesToClear.Count > 0)
                chunk.MineableObjects.Values.ToList().ForEach(x => x.RemoveAll(y => obstaclesToClear.Contains(y)));
        }

        private void CreateDirtAroundPlots(ref WorldChunk chunk)
        {
            var map = chunk.Town.Tilemaps["base"];
            var objectsToClear = new List<IMineable>();

            foreach (var plot in chunk.Town.Plots)
            {
                // Get the plot center
                var plotCenterX = (plot.LocalBounds.Left + plot.LocalBounds.Right) / 2 / Global.TileSize;
                var plotCenterY = (plot.LocalBounds.Top + plot.LocalBounds.Bottom) / 2 / Global.TileSize;
                // Set radius in tiles for the circular area around the plot
                var radius = 10;

                // Calculate bounds with some padding
                var startX = (plot.LocalBounds.Left / Global.TileSize) - radius;
                var startY = (plot.LocalBounds.Top / Global.TileSize) - radius;
                var endX = (plot.LocalBounds.Right / Global.TileSize) + radius;
                var endY = (plot.LocalBounds.Bottom / Global.TileSize) + radius;

                if (startX < 0)
                    startX = 0;
                if (startY < 0)
                    startY = 0;
                if (endX > chunk.Width)
                    endX = chunk.Width;
                if (endY > chunk.Height)
                    endY = chunk.Height;

                // Loop through the bounding box but only modify tiles within the radius
                for (int i = startX; i < endX; i++)
                {
                    for (int j = startY; j < endY; j++)
                    {
                        // Calculate the distance from the plot center to the current tile
                        var distX = i - plotCenterX;
                        var distY = j - plotCenterY;
                        var distance = Math.Sqrt(distX * distX + distY * distY);

                        // Check if the tile is within the circular radius
                        if (distance <= radius)
                        {
                            map.Map[i, j] = new Tile()
                            {
                                ChunkX = chunk.ChunkCoordX,
                                ChunkY = chunk.ChunkCoordY,
                                xIndex = i,
                                yIndex = j,
                                LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize),
                                Size = new Vector2(Global.TileSize, Global.TileSize),
                                Texture = SpriteLib.Town.Terrain["dirt-c"],
                                TileType = TileType.Instance,
                                IsExplored = true,
                                Bounds = new Rectangle(
                                            i * Global.TileSize,
                                            j * Global.TileSize,
                                            Global.TileSize,
                                            Global.TileSize)
                            };

                            foreach (var mineables in chunk.MineableObjects.Values)
                            {
                                foreach (var mineable in mineables)
                                {
                                    if (mineable.InteractBounds.Intersects(map.Map[i, j].Bounds))
                                    {
                                        objectsToClear.Add(mineable);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (var plot in chunk.Town.Plots)
            {
                // Calculate bounds with some padding
                var startX = (plot.LocalBounds.Left / Global.TileSize);
                var startY = (plot.LocalBounds.Top / Global.TileSize);
                var endX = (plot.LocalBounds.Right / Global.TileSize);
                var endY = (plot.LocalBounds.Bottom / Global.TileSize);

                if (startX < 0)
                    startX = 0;
                if (startY < 0)
                    startY = 0;
                if (endX > chunk.Width)
                    endX = chunk.Width;
                if (endY > chunk.Height)
                    endY = chunk.Height;

                // Loop through the bounding box but only modify tiles within the radius
                for (int i = startX; i < endX; i++)
                {
                    for (int j = startY; j < endY; j++)
                    {
                        map.Map[i, j] = new Tile()
                        {
                            ChunkX = chunk.ChunkCoordX,
                            ChunkY = chunk.ChunkCoordY,
                            xIndex = i,
                            yIndex = j,
                            LocalPosition = new Vector2(i * Global.TileSize, j * Global.TileSize),
                            Size = new Vector2(Global.TileSize, Global.TileSize),
                            Texture = SpriteLib.Town.Terrain["dirt-c"],
                            TileType = TileType.Instance,
                            IsExplored = true,
                            Bounds = new Rectangle(
                                        i * Global.TileSize,
                                        j * Global.TileSize,
                                        Global.TileSize,
                                        Global.TileSize)
                        };

                        foreach (var mineables in chunk.MineableObjects.Values)
                        {
                            foreach (var mineable in mineables)
                            {
                                if (mineable.InteractBounds.Intersects(map.Map[i, j].Bounds))
                                {
                                    objectsToClear.Add(mineable);
                                }
                            }
                        }
                    }
                }
            }
        
            if (objectsToClear.Count > 0)
                chunk.MineableObjects.Values.ToList().ForEach(x => x.RemoveAll(y => objectsToClear.Contains(y)));
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
    
        private void CreateBuildingRoofs(ref WorldChunk chunk)
        {
            var tilemap = new Tilemap(chunk.Width, chunk.Height);
            tilemap.WorldPosition = new Vector2(chunk.WorldCoordX, chunk.WorldCoordY);

            foreach (var plot in chunk.Town.Plots)
            {
                var widthInTiles = plot.Building.Width / Global.TileSize;
                var heightInTiles = plot.Building.Height / Global.TileSize;

                var xCounter = 0;
                var yCounter = 0;

                for (int i = plot.Building.LocalBounds.Left; i < plot.Building.LocalBounds.Right; i += Global.TileSize)
                {
                    yCounter = 0;
                    for (int j = plot.Building.LocalBounds.Top; j < plot.Building.LocalBounds.Bottom + Global.TileSize; j += Global.TileSize)
                    {
                        Texture2D texture = null;
                        if (yCounter == 0) // Top row
                        {
                            if (xCounter == 0)
                            {
                                texture = SpriteLib.Town.Roof3["nw"];
                            }
                            else if (xCounter == widthInTiles - 1)
                            {
                                texture = SpriteLib.Town.Roof3["ne"];
                            }
                            else
                            {
                                texture = SpriteLib.Town.Roof3["n"];
                            }
                        }
                        else if (yCounter == heightInTiles - 1) // Bottom row
                        {
                            if (xCounter == 0)
                            {
                                texture = SpriteLib.Town.Roof3["sw"];
                            }
                            else if (xCounter == widthInTiles - 1)
                            {
                                texture = SpriteLib.Town.Roof3["se"];
                            }
                            else
                            {
                                texture = SpriteLib.Town.Roof3["s"];
                            }
                        }
                        else if (yCounter >= heightInTiles) // Gutter
                        {
                            texture = SpriteLib.Town.Roof3["gutter"];
                        }
                        else // Middle row
                        {
                            if (xCounter == 0)
                            {
                                texture = SpriteLib.Town.Roof3["w"];
                            }
                            else if (xCounter == widthInTiles - 1)
                            {
                                texture = SpriteLib.Town.Roof3["e"];
                            }
                            else
                            {
                                texture = SpriteLib.Town.Roof3["c"];
                            }
                        }

                        tilemap.Map[i / Global.TileSize, j / Global.TileSize] = new Tile()
                        {
                            ChunkX = chunk.ChunkCoordX,
                            ChunkY = chunk.ChunkCoordY,
                            xIndex = (i / Global.TileSize) - 0,
                            yIndex = (j / Global.TileSize) - 1,
                            LocalPosition = new Vector2(i, j - Global.TileSize),
                            Size = new Vector2(Global.TileSize, Global.TileSize),
                            Texture = texture,
                            TileType = TileType.Instance,
                            IsExplored = true,
                            Bounds = new Rectangle(i, j - Global.TileSize, Global.TileSize, Global.TileSize)
                        };

                        yCounter++;
                    }

                    xCounter++;
                }
            }

            chunk.Town.Tilemaps.Add("roofs", tilemap);
        }

        private void PlaceDoorsOnBuildings(ref WorldChunk chunk)
        {
            var tm = chunk.Town.Tilemaps["walls"];

            foreach (var plot in chunk.Town.Plots)
            {
                var building = plot.Building;
                var doorx = building.LocalPosition.X + ((_random.Next(building.Width / Global.TileSize) * Global.TileSize));
                var doorPosition = new Vector2(doorx, building.LocalPosition.Y + building.Height - Global.TileSize);

                tm.Map[(int)doorPosition.X / Global.TileSize, (int)doorPosition.Y / Global.TileSize] = new Tile()
                {
                    ChunkX = chunk.ChunkCoordX,
                    ChunkY = chunk.ChunkCoordY,
                    xIndex = (int)doorPosition.X / Global.TileSize,
                    yIndex = (int)doorPosition.Y / Global.TileSize,
                    LocalPosition = new Vector2(doorPosition.X, doorPosition.Y),
                    Size = new Vector2(Global.TileSize, Global.TileSize),
                    Texture = SpriteLib.Town.Doors["door-int"],
                    TileType = TileType.Instance,
                    IsExplored = true
                };
            }
        }
    }
}
