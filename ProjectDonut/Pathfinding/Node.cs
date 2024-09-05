using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Pathfinding
{
    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int G { get; set; } // Cost from start to this node
        public int H { get; set; } // Heuristic cost from this node to the end
        public int F => G + H; // Total cost
        public Node Parent { get; set; }

        public Node(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

}
