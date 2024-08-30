using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.WFC
{
    public class WaveFunctionCollapse
    {
        //private Dictionary<string, Texture2D> _textures;

        //// Define tile types as integers
        //const int GRASS = 0;
        //const int WATER = 1;
        //const int SAND = 2;
        //const int MOUNTAIN = 3;

        //// Define tile constraints (e.g., WATER can be adjacent to SAND or WATER)
        //static Dictionary<int, List<int>> tileConstraints = new Dictionary<int, List<int>>()
        //{
        //    { GRASS, new List<int> { GRASS, SAND } },
        //    { WATER, new List<int> { WATER, SAND } },
        //    { SAND, new List<int> { GRASS, WATER, SAND } },
        //    { MOUNTAIN, new List<int> { MOUNTAIN, GRASS } }
        //};


        //private Dictionary<string, Dictionary<string, List<string>>> adjacencyRules = new Dictionary<string, Dictionary<string, List<string>>>
        //{
        //    ["N"] = new Dictionary<string, List<string>>
        //    {
        //        {"Top", new List<string> { "Wall" }},
        //        {"Bottom", new List<string> { "Floor" }},
        //        {"Left", new List<string> { "Wall", "Floor" }},
        //        {"Right", new List<string> { "Wall", "Floor" }}
        //    },
        //    ["E"] = new Dictionary<string, List<string>>
        //    {
        //        {"Top", new List<string> { "Wall", "Floor" }},
        //        {"Bottom", new List<string> { "Wall", "Floor" }},
        //        {"Left", new List<string> { "Floor" }},
        //        {"Right", new List<string> { "Wall" }}
        //    },
        //    // Continue defining rules for all other tiles...
        //};



        //public WaveFunctionCollapse(Dictionary<string, Texture2D> textures)
        //{
        //    _textures = textures;
        //}

        //public void Collapse()
        //{
        //    var gridSize = 10;
        //    var grid = InitialiseGrid(gridSize, gridSize);
        //    var collapsed = InitialiseCollapseGrid(gridSize, gridSize);

        //    // Collapse the tilemap using wave function collapse
        //    while (collapsed.Cast<bool>().Any(c => !c))
        //    {
        //        // Select the cell with the lowest entropy
        //        (int x, int y) = FindLowestEntropyCell(grid, collapsed);

        //        // Collapse the cell
        //        CollapseCell(grid, x, y);
        //        collapsed[x, y] = true;

        //        // Propagate constraints to neighboring cells
        //        PropagateConstraints(grid, x, y);
        //    }

        //    // Print the resulting tilemap (e.g., 0: GRASS, 1: WATER, 2: SAND, 3: MOUNTAIN)
        //    for (int y = 0; y < gridSize; y++)
        //    {
        //        for (int x = 0; x < gridSize; x++)
        //        {
        //            Console.Write(grid[x, y] + " ");
        //        }
        //        Console.WriteLine();
        //    }
        //}

        //private Texture2D[,] InitialiseGrid(int width, int height)
        //{
        //    var grid = new Texture2D[width, height];

        //    // Initialize the grid with -1 (unresolved)
        //    for (int x = 0; x < width; x++)
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            grid[x, y] = null;
        //        }
        //    }

        //    return grid;
        //}

        //private bool[,] InitialiseCollapseGrid(int width, int height)
        //{
        //    bool[,] collapsed = new bool[width, height];

        //    for (int x = 0; x < width; x++)
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            collapsed[x, y] = false;
        //        }
        //    }

        //    return collapsed;
        //}

        //private (int, int) FindLowestEntropyCell(Texture2D[,] grid, bool[,] collapsed)
        //{
        //    List<(int, int)> lowestEntropyCells = new List<(int, int)>();
        //    int minEntropy = int.MaxValue;

        //    for (int x = 0; x < grid.GetLength(0); x++)
        //    {
        //        for (int y = 0; y < grid.GetLength(1); y++)
        //        {
        //            if (!collapsed[x, y])
        //            {
        //                int entropy = CalculateEntropy(grid, x, y);
        //                if (entropy < minEntropy)
        //                {
        //                    minEntropy = entropy;
        //                    lowestEntropyCells.Clear();
        //                    lowestEntropyCells.Add((x, y));
        //                }
        //                else if (entropy == minEntropy)
        //                {
        //                    lowestEntropyCells.Add((x, y));
        //                }
        //            }
        //        }
        //    }

        //    // Randomly select a cell if multiple have the same lowest entropy
        //    Random random = new Random();
        //    return lowestEntropyCells[random.Next(lowestEntropyCells.Count)];
        //}

        //private int CalculateEntropy(Texture2D[,] grid, int x, int y)
        //{
        //    // Calculate the number of valid tiles that can be placed at (x, y)
        //    return tileConstraints.Keys.Count(tile => IsValid(grid, x, y, tile));
        //}

        //private void CollapseCell(Texture2D[,] grid, int x, int y)
        //{
        //    // Randomly choose a valid tile for the cell
        //    Random random = new Random();
        //    List<int> validTiles = tileConstraints.Keys.Where(tile => IsValid(grid, x, y, tile)).ToList();
        //    grid[x, y] = validTiles[random.Next(validTiles.Count)];
        //}

        //private void PropagateConstraints(int[,] grid, int x, int y)
        //{
        //    // Propagate constraints to neighbors
        //    int gridSize = grid.GetLength(0);
        //    int[] dx = { -1, 1, 0, 0 };
        //    int[] dy = { 0, 0, -1, 1 };

        //    for (int i = 0; i < 4; i++)
        //    {
        //        int nx = x + dx[i];
        //        int ny = y + dy[i];

        //        if (nx >= 0 && nx < gridSize && ny >= 0 && ny < gridSize && grid[nx, ny] == -1)
        //        {
        //            List<int> allowedTiles = tileConstraints.Keys.Where(tile => IsValid(grid, nx, ny, tile)).ToList();

        //            // If there's only one valid tile, collapse it immediately
        //            if (allowedTiles.Count == 1)
        //            {
        //                grid[nx, ny] = allowedTiles[0];
        //            }
        //        }
        //    }
        //}

        //private bool IsValid(Texture2D[,] grid, int x, int y, int tile)
        //{
        //    // Check constraints against neighboring tiles
        //    int gridSize = grid.GetLength(0);
        //    int[] dx = { -1, 1, 0, 0 };
        //    int[] dy = { 0, 0, -1, 1 };

        //    for (int i = 0; i < 4; i++)
        //    {
        //        int nx = x + dx[i];
        //        int ny = y + dy[i];

        //        if (nx >= 0 && nx < gridSize && ny >= 0 && ny < gridSize)
        //        {
        //            int neighborTile = grid[nx, ny];
        //            if (neighborTile != -1 && !tileConstraints[tile].Contains(neighborTile))
        //            {
        //                return false;
        //            }
        //        }
        //    }

        //    return true;
        //}
    }
}
