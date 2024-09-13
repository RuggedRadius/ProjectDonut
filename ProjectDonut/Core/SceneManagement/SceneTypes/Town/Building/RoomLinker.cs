using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectDonut.Debugging;

namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town.Building
{
    public class RoomLinker2
    {
        public static int[,] LinkRooms(Plot plot, int[,] map, List<Rectangle> rects)
        {
            var random = new Random();

            //var plotOffsetX = 0;
            //var plotOffsetY = 0;
            var plotOffsetX = -plot.PlotBounds.X;
            var plotOffsetY = -plot.PlotBounds.Y;

            var unlinkedRooms = new List<Rectangle>(rects);

            var maxTries = 100;

            do
            {
                var r1 = unlinkedRooms[random.Next(unlinkedRooms.Count)];

                foreach (var r2 in rects)
                {
                    if (r1 == r2)
                        continue;

                    if (unlinkedRooms.Contains(r1) == false)
                        continue;

                    if (unlinkedRooms.Contains(r2) == false)
                        continue;

                    if (r1.Top == r2.Bottom)
                    {
                        // Select a random point on the top edge of r1
                        var x = r1.Left + random.Next(r1.Width - 4) + 2;
                        var y = r1.Top;

                        map[plotOffsetX + x, plotOffsetY + y] = 2; // 2 is a path tile in the tilemap

                        DebugMapData.WriteMapData(map, "PlotDebug");

                        unlinkedRooms.Remove(r1);
                        unlinkedRooms.Remove(r2);
                        //return map;
                        break;
                    }

                    if (r1.Bottom == r2.Top)
                    {
                        // Select a random point on the top edge of r1
                        var x = r1.Left + random.Next(r1.Width - 4) + 2;
                        var y = r1.Bottom;

                        map[plotOffsetX + x, plotOffsetY + y] = 2; // 2 is a path tile in the tilemap

                        DebugMapData.WriteMapData(map, "PlotDebug");

                        unlinkedRooms.Remove(r1);
                        unlinkedRooms.Remove(r2);
                        //return map;
                        break;
                    }

                    if (r1.Left == r2.Right)
                    {
                        // Select a random point on the top edge of r1
                        var x = r1.Left;
                        var y = r1.Top + random.Next(r1.Height - 4) + 2;

                        map[plotOffsetX + x, plotOffsetY + y] = 2; // 2 is a path tile in the tilemap

                        DebugMapData.WriteMapData(map, "PlotDebug");

                        unlinkedRooms.Remove(r1);
                        unlinkedRooms.Remove(r2);
                        //return map;
                        break;
                    }

                    if (r1.Right == r2.Left)
                    {
                        // Select a random point on the top edge of r1
                        var x = r1.Right;
                        var y = r1.Top + random.Next(r1.Height - 4) + 2;

                        map[plotOffsetX + x, plotOffsetY + y] = 2; // 2 is a path tile in the tilemap

                        DebugMapData.WriteMapData(map, "PlotDebug");

                        unlinkedRooms.Remove(r1);
                        unlinkedRooms.Remove(r2);
                        //return map;
                        break;
                    }
                }

                maxTries--;

                if (maxTries <= 0)
                    break;
            }
            while (unlinkedRooms.Count > 1);

            //foreach (var r1 in rects)
            //{
            //    foreach (var r2 in rects)
            //    {
            //        if (r1 == r2)
            //            continue;

            //        if (unlinkedRooms.Contains(r2) == false)
            //            continue;

            //        if (r1.Top == r2.Bottom)
            //        {
            //            // Select a random point on the top edge of r1
            //            var x = r1.Left + random.Next(r1.Width - 4) + 2;
            //            var y = r1.Top;

            //            map[plotOffsetX + x, plotOffsetY + y] = 2; // 2 is a path tile in the tilemap
                    
            //            unlinkedRooms.Remove(r1);
            //            unlinkedRooms.Remove(r2);
            //        }

            //        if (r1.Bottom == r2.Top)
            //        {
            //            // Select a random point on the top edge of r1
            //            var x = r1.Left + random.Next(r1.Width - 4) + 2;
            //            var y = r1.Bottom;

            //            map[plotOffsetX + x, plotOffsetY + y] = 2; // 2 is a path tile in the tilemap

            //            unlinkedRooms.Remove(r1);
            //            unlinkedRooms.Remove(r2);
            //        }

            //        if (r1.Left == r2.Right)
            //        {
            //            // Select a random point on the top edge of r1
            //            var x = r1.Left;
            //            var y = r1.Top + random.Next(r1.Height - 4) + 2;

            //            map[plotOffsetX + x, plotOffsetY + y] = 2; // 2 is a path tile in the tilemap

            //            unlinkedRooms.Remove(r1);
            //            unlinkedRooms.Remove(r2);
            //        }

            //        if (r1.Right == r2.Left)
            //        {
            //            // Select a random point on the top edge of r1
            //            var x = r1.Right;
            //            var y = r1.Top + random.Next(r1.Height - 4) + 2;

            //            map[plotOffsetX + x, plotOffsetY + y] = 2; // 2 is a path tile in the tilemap

            //            unlinkedRooms.Remove(r1);
            //            unlinkedRooms.Remove(r2);
            //        }
            //    }
            //}

            return map;
        }
    }

    public class Edge
    {
        public Rectangle A { get; set; }
        public Rectangle B { get; set; }
        public int Weight { get; set; } // Distance between rectangles (Manhattan distance)

        public Edge(Rectangle a, Rectangle b)
        {
            A = a;
            B = b;
            Weight = Math.Abs(a.Center.X - b.Center.X) + Math.Abs(a.Center.Y - b.Center.Y); // Manhattan distance
        }
    }

    public class DisjointSet
    {
        private Dictionary<int, int> parent = new Dictionary<int, int>();

        public void MakeSet(int v)
        {
            parent[v] = v;
        }

        public int FindSet(int v)
        {
            if (parent[v] == v)
                return v;
            return parent[v] = FindSet(parent[v]);
        }

        public void UnionSets(int a, int b)
        {
            int rootA = FindSet(a);
            int rootB = FindSet(b);
            if (rootA != rootB)
            {
                parent[rootB] = rootA;
            }
        }
    }

    public class MapGenerator
    {
        // Assuming your int[,] map and list of rectangles are passed in
        public static int[,] ConnectRectangles(int[,] map, List<Rectangle> rectangles)
        {
            // Step 1: Create edges between all neighboring rectangles (Manhattan distance for weight)
            List<Edge> edges = new List<Edge>();
            for (int i = 0; i < rectangles.Count; i++)
            {
                for (int j = i + 1; j < rectangles.Count; j++)
                {
                    // Create an edge only if the rectangles are adjacent (horizontally or vertically)
                    Rectangle rectA = rectangles[i];
                    Rectangle rectB = rectangles[j];

                    // Check if they are horizontally or vertically aligned neighbors
                    if (rectA.Center.X == rectB.Center.X || rectA.Center.Y == rectB.Center.Y)
                    {
                        edges.Add(new Edge(rectA, rectB));
                    }
                }
            }

            // Step 2: Sort the edges by weight (Manhattan distance)
            edges.Sort((a, b) => a.Weight.CompareTo(b.Weight));

            // Step 3: Initialize disjoint sets for Kruskal's algorithm
            DisjointSet disjointSet = new DisjointSet();
            for (int i = 0; i < rectangles.Count; i++)
            {
                disjointSet.MakeSet(i); // Use index as the unique identifier
            }

            // Step 4: Apply Kruskal's algorithm to find the Minimum Spanning Tree
            List<Edge> mstEdges = new List<Edge>();
            foreach (var edge in edges)
            {
                // Use the indices of the rectangles in the list as identifiers
                int indexA = rectangles.IndexOf(edge.A);
                int indexB = rectangles.IndexOf(edge.B);

                if (disjointSet.FindSet(indexA) != disjointSet.FindSet(indexB))
                {
                    mstEdges.Add(edge);
                    disjointSet.UnionSets(indexA, indexB);
                }
            }

            // Step 5: Carve paths between rectangles based on the MST
            foreach (var edge in mstEdges)
            {
                map = CarvePath(map, edge.A, edge.B);
            }

            return map;
        }

        private static int[,] CarvePath(int[,] map, Rectangle a, Rectangle b)
        {
            int x1 = a.Center.X, y1 = a.Center.Y;
            int x2 = b.Center.X, y2 = b.Center.Y;

            for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
            {
                if (y1 >= 0 && y1 < map.GetLength(0) && x >= 0 && x < map.GetLength(1))
                {
                    map[y1, x] = 2;
                }
            }

            for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
            {
                if (y >= 0 && y < map.GetLength(0) && x2 >= 0 && x2 < map.GetLength(1))
                {
                    map[y, x2] = 2;
                }
            }

            return map;
        }
    }


}
