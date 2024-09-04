using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World.Generators;

namespace ProjectDonut.GameObjects
{
    public class ChunkStructure : IGameObject
    {
        public string StructureName { get; set; }
        public Structure StructureType { get; set; }
        public Rectangle Rectangle { get; set; }
        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }
        public Texture2D Texture { get; set; }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void LoadContent()
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime)
        {
        }
    }
}
