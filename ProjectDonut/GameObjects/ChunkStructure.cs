﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World.Generators;

namespace ProjectDonut.GameObjects
{
    public class ChunkStructure : IGameObject
    {
        public string StructureName { get; set; }
        public Structure StructureType { get; set; }
        public Rectangle Rectangle { get; set; }
        public Vector2 Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ZIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //public override void Initialize()
        //{
        //}

        //public override void LoadContent()
        //{
        //}

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime)
        {
        }

        public void Initialize()
        {
            //throw new NotImplementedException();
        }

        public void LoadContent()
        {
            //throw new NotImplementedException();
        }
    }
}
