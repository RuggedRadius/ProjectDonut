using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

public class RectangleLinker
{
    public List<(Rectangle, Rectangle)> LinkRectangles(List<Rectangle> rectangles)
    {
        var links = new List<(Rectangle, Rectangle)>();
        var edges = new List<Edge>();

        // Step 1: Calculate all edges between rectangles
        for (int i = 0; i < rectangles.Count; i++)
        {
            for (int j = i + 1; j < rectangles.Count; j++)
            {
                var distance = CalculateDistance(rectangles[i], rectangles[j]);
                edges.Add(new Edge(rectangles[i], rectangles[j], distance));
            }
        }

        // Step 2: Sort edges by distance (Kruskal's algorithm)
        edges.Sort((a, b) => a.Distance.CompareTo(b.Distance));

        // Step 3: Use Kruskal's algorithm to form MST
        var disjointSet = new DisjointSet(rectangles.Count);

        foreach (var edge in edges)
        {
            int rectAIndex = rectangles.IndexOf(edge.RectangleA);
            int rectBIndex = rectangles.IndexOf(edge.RectangleB);

            if (disjointSet.Find(rectAIndex) != disjointSet.Find(rectBIndex))
            {
                disjointSet.Union(rectAIndex, rectBIndex);
                links.Add((edge.RectangleA, edge.RectangleB));
            }

            if (links.Count == rectangles.Count - 1)
            {
                break;
            }
        }

        return links;
    }

    private double CalculateDistance(Rectangle rectA, Rectangle rectB)
    {
        var centerA = new Point(rectA.X + rectA.Width / 2, rectA.Y + rectA.Height / 2);
        var centerB = new Point(rectB.X + rectB.Width / 2, rectB.Y + rectB.Height / 2);
        return Math.Sqrt(Math.Pow(centerA.X - centerB.X, 2) + Math.Pow(centerA.Y - centerB.Y, 2));
    }

    public class Edge
    {
        public Rectangle RectangleA { get; }
        public Rectangle RectangleB { get; }
        public double Distance { get; }

        public Edge(Rectangle rectA, Rectangle rectB, double distance)
        {
            RectangleA = rectA;
            RectangleB = rectB;
            Distance = distance;
        }
    }

    public class DisjointSet
    {
        private int[] parent;

        public DisjointSet(int size)
        {
            parent = new int[size];
            for (int i = 0; i < size; i++)
                parent[i] = i;
        }

        public int Find(int i)
        {
            if (parent[i] == i)
                return i;
            return parent[i] = Find(parent[i]);
        }

        public void Union(int i, int j)
        {
            int irep = Find(i);
            int jrep = Find(j);
            parent[irep] = jrep;
        }
    }
}
