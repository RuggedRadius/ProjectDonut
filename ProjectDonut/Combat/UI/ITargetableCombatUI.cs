﻿using Microsoft.Xna.Framework;

namespace ProjectDonut.Combat.UI
{
    public interface ITargetableCombatUI
    {
        bool IsShown { get; set; }
        //Rectangle Bounds { get; set; }

        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}