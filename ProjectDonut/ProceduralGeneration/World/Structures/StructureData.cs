using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectDonut.ProceduralGeneration.World.Structures
{
    public class StructureData
    {
        public string Name { get; set; }
        public Rectangle Bounds { get; set; }

        // Probably more properties here about scene linkage etc
    }
}
