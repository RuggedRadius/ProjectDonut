using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.WFC
{
    public class WaveFunctionCollapse
    {
        // Define tile types as integers
        const int GRASS = 0;
        const int WATER = 1;
        const int SAND = 2;
        const int MOUNTAIN = 3;

        // Define tile constraints (e.g., WATER can be adjacent to SAND or WATER)
        static Dictionary<int, List<int>> tileConstraints = new Dictionary<int, List<int>>()
        {
            { GRASS, new List<int> { GRASS, SAND } },
            { WATER, new List<int> { WATER, SAND } },
            { SAND, new List<int> { GRASS, WATER, SAND } },
            { MOUNTAIN, new List<int> { MOUNTAIN, GRASS } }
        };

        static void Main(string[] args)
        {
            int gridSize = 10;
            int[,] grid = new int[gridSize, gridSize];
            bool[,] collapsed = new bool[gridSize, gridSize];

            // Initialize the grid with -1 (unresolved)
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    grid[x, y] = -1;
                    collapsed[x, y] = false;
                }
            }

            // Collapse the tilemap using wave function collapse
            while (collapsed.Cast<bool>().Any(c => !c))
            {
                // Select the cell with the lowest entropy
                (int x, int y) = FindLowestEntropyCell(grid, collapsed);

                // Collapse the cell
                CollapseCell(grid, x, y);
                collapsed[x, y] = true;

                // Propagate constraints to neighboring cells
                PropagateConstraints(grid, x, y);
            }

            // Print the resulting tilemap (e.g., 0: GRASS, 1: WATER, 2: SAND, 3: MOUNTAIN)
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    Console.Write(grid[x, y] + " ");
                }
                Console.WriteLine();
            }
        }

        static (int, int) FindLowestEntropyCell(int[,] grid, bool[,] collapsed)
        {
            List<(int, int)> lowestEntropyCells = new List<(int, int)>();
            int minEntropy = int.MaxValue;

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (!collapsed[x, y])
                    {
                        int entropy = CalculateEntropy(grid, x, y);
                        if (entropy < minEntropy)
                        {
                            minEntropy = entropy;
                            lowestEntropyCells.Clear();
                            lowestEntropyCells.Add((x, y));
                        }
                        else if (entropy == minEntropy)
                        {
                            lowestEntropyCells.Add((x, y));
                        }
                    }
                }
            }

            // Randomly select a cell if multiple have the same lowest entropy
            Random random = new Random();
            return lowestEntropyCells[random.Next(lowestEntropyCells.Count)];
        }

        static int CalculateEntropy(int[,] grid, int x, int y)
        {
            // Calculate the number of valid tiles that can be placed at (x, y)
            return tileConstraints.Keys.Count(tile => IsValid(grid, x, y, tile));
        }

        static void CollapseCell(int[,] grid, int x, int y)
        {
            // Randomly choose a valid tile for the cell
            Random random = new Random();
            List<int> validTiles = tileConstraints.Keys.Where(tile => IsValid(grid, x, y, tile)).ToList();
            grid[x, y] = validTiles[random.Next(validTiles.Count)];
        }

        static void PropagateConstraints(int[,] grid, int x, int y)
        {
            // Propagate constraints to neighbors
            int gridSize = grid.GetLength(0);
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (nx >= 0 && nx < gridSize && ny >= 0 && ny < gridSize && grid[nx, ny] == -1)
                {
                    List<int> allowedTiles = tileConstraints.Keys.Where(tile => IsValid(grid, nx, ny, tile)).ToList();

                    // If there's only one valid tile, collapse it immediately
                    if (allowedTiles.Count == 1)
                    {
                        grid[nx, ny] = allowedTiles[0];
                    }
                }
            }
        }

        static bool IsValid(int[,] grid, int x, int y, int tile)
        {
            // Check constraints against neighboring tiles
            int gridSize = grid.GetLength(0);
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (nx >= 0 && nx < gridSize && ny >= 0 && ny < gridSize)
                {
                    int neighborTile = grid[nx, ny];
                    if (neighborTile != -1 && !tileConstraints[tile].Contains(neighborTile))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
