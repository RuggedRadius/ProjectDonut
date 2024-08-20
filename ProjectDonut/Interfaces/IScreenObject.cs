using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectDonut.Interfaces
{
    public interface IScreenObject
    {
        int ZIndex { get; set; }

        void Initialize();
        void LoadContent();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
