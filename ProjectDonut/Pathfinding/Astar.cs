using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProjectDonut.Pathfinding
{
    public static class Astar
    {
        //public static bool[,] occupiedCells;

        public static List<Node> FindPath(int[,] grid, Node start, Node end)
        {
            var openList = new List<Node>();
            var closedList = new HashSet<Node>();

            if (grid[end.X, end.Y] != 2)
            {
                return null;
            }

            openList.Add(start);

            var maxCount = 50;
            int counter = 0;

            while (openList.Count > 0)
            {
                // Get the node with the lowest F score
                var currentNode = openList.OrderBy(node => node.F).First();

                // If we have reached the end, reconstruct and return the path
                if (currentNode.X == end.X && currentNode.Y == end.Y)
                {
                    return ReconstructPath(currentNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                // Check each neighboring cell
                foreach (var neighbor in GetNeighbors(grid, currentNode))
                {
                    if (closedList.Contains(neighbor) || 
                        grid[neighbor.X, neighbor.Y] != 2)// ||
                        //occupiedCells[neighbor.X, neighbor.Y] == true)
                    {
                        continue;
                    }

                    int tentativeG = currentNode.G + 1;

                    if (!openList.Contains(neighbor))
                    {
                        neighbor.Parent = currentNode;
                        neighbor.G = tentativeG;
                        neighbor.H = Math.Abs(neighbor.X - end.X) + Math.Abs(neighbor.Y - end.Y);
                        openList.Add(neighbor);
                    }
                    else if (tentativeG < neighbor.G)
                    {
                        neighbor.Parent = currentNode;
                        neighbor.G = tentativeG;
                    }
                }

                counter++;

                if (counter >= maxCount)
                {
                    break;
                }
            }

            return null; // No path found
        }

        private static List<Node> GetNeighbors(int[,] grid, Node node)
        {
            var neighbors = new List<Node>();

            // Define possible moves (up, down, left, right)
            var directions = new (int, int)[]
            {
        (0, -1), (0, 1), (-1, 0), (1, 0)
            };

            foreach (var direction in directions)
            {
                int newX = node.X + direction.Item1;
                int newY = node.Y + direction.Item2;

                if (newX >= 0 && newX < grid.GetLength(0) && newY >= 0 && newY < grid.GetLength(1))
                {
                    neighbors.Add(new Node(newX, newY));
                }
            }

            return neighbors;
        }

        private static List<Node> ReconstructPath(Node node)
        {
            var path = new List<Node>();
            while (node != null)
            {
                path.Add(node);
                node = node.Parent;
            }
            path.Reverse();

            for (int i = 0; i < path.Count; i++)
            {
                path[i] = new Node(path[i].X * 32, path[i].Y * 32);
            }

            return path;
        }
    
        //public static void InitialiseOccupiedCells(int width, int height)
        //{
        //    occupiedCells = new bool[width, height];
        //}

        //public static void SetOccupiedCell(int x, int y, bool occupied)
        //{
        //    occupiedCells[x, y] = occupied;
        //}
    }
}
