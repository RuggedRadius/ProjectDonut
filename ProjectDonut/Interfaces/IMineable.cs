using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Interfaces
{
    public interface IMineable : ISceneObject
    {
        bool InRangeOfPlayer { get; set; }
        Rectangle InteractBounds { get; set; }
        Texture2D InventoryIcon { get; set; }
        int Health { get; set; }
        int MaxHealth { get; set; }
    }
}
