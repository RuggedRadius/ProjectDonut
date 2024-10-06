﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Core;
using ProjectDonut.Core.Input;
using ProjectDonut.Core.SceneManagement.SceneTypes;

namespace ProjectDonut.Combat
{
    public class CombatUIItem : ITargetableCombatUI
    {
        public bool IsShown { get; set; } = false;

        private CombatManager _manager;

        private Rectangle Bounds { get; set; }
        private int linesToShowCount = 10;
        private int lineHeight = 20;
        private int padding = 10;
        private int Width = 600;
        private int Height = 220;
        private int margin = 10;

        public CombatUIItem() 
        {
            _manager = CombatScene.Instance.Manager;

            Bounds = new Rectangle(
                320,
                Global.GraphicsDeviceManager.PreferredBackBufferHeight - Height - margin,
                Width,
                Height);
        }

        public void Update(GameTime gameTime) 
        {
            if (CombatScene.Instance.CurrentTargetUI != this)
                return;

            if (InputManager.IsKeyPressed(Keys.Escape) || InputManager.IsKeyPressed(Keys.Back))
            {
                CombatScene.Instance.ReturnToPreviousTargetUI();
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (!IsShown)
                return;

            if (_manager.TurnOrder[0].Team == TeamType.Enemy)
                return;

            // Draw background
            if (CombatScene.Instance.CurrentTargetUI == this)
            {
                Global.SpriteBatch.Draw(Global.BLANK_TEXTURE, Bounds, Color.Yellow * 0.5f);
            }
            else
            {
                Global.SpriteBatch.Draw(Global.BLANK_TEXTURE, Bounds, Color.Black * 0.5f);
            }

            // Draw items
            for (int i = 0; i < CombatScene.Instance.PlayerItems.Count; i++)
            {
                var item = CombatScene.Instance.PlayerItems[i];
                Global.SpriteBatch.DrawString(
                    Global.FontDebug, 
                    item.Name, 
                    new Vector2(Bounds.X + padding, Bounds.Y + padding + (i * 20)), 
                    Color.White);
            }
        }
    }
}
