using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.GameObjects.PlayerComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Interfaces
{
    public interface IUIComponent : IScreenObject
    {
        UIComponentState State { get; set; }
        Texture2D Texture { get; set; }
        Vector2 ScreenPosition { get; set; }

        void ToggleVisibility();
    }
}
