using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectDonut.Interfaces
{
    public interface IGameComponent
    {
        void Initialize();
        void LoadContent();
        void Update(GameTime gameTime);
    }
}
