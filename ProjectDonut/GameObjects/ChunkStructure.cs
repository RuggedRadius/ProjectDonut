using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectDonut.ProceduralGeneration.World;

namespace ProjectDonut.GameObjects
{
    public class ChunkStructure : GameObject
    {
        public string StructureName { get; set; }
        public Structure StructureType { get; set; }
        public Rectangle Rectangle { get; set; }

        //public override void Initialize()
        //{
        //}

        //public override void LoadContent()
        //{
        //}

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
        }
    }
}
