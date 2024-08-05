using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGorilla.GameObjects
{
    public class GameObject : IGameObject
    {
        public int ZIndex { get; set; }


        public virtual void Draw(GameTime gameTime)
        {
        }

        public virtual void Initialize()
        {
        }

        public virtual void LoadContent()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
