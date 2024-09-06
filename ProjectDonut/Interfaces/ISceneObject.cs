using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDonut.Interfaces
{
    public interface ISceneObject : IDrawable
    {
        bool IsExplored { get; set; }
        void Update(GameTime gameTime);
        new void Draw(GameTime gameTime);
    }
}
