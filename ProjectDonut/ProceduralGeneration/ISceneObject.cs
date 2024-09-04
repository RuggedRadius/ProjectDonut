using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDonut.ProceduralGeneration
{
    public interface ISceneObject : IDrawable
    {
        void Update();
        void Draw();
    }
}
