﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Core;
using ProjectDonut.Core.Input;
using ProjectDonut.Core.SceneManagement.SceneTypes;

namespace ProjectDonut.Combat
{
    public class CombatUIAbility : ITargetableCombatUI
    {
        public bool IsShown { get; set; } = false;

        private CombatManager _manager;

        public Rectangle Bounds { get; set; }

        private int linesToShowCount = 10;
        private int lineHeight = 20;
        private int padding = 10;
        private int Width = 600;
        private int Height = 220;
        private int margin = 10;

        public CombatUIAbility(CombatManager manager)
        {
            _manager = manager;

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

            // Draw abilities
            for (int i = 0; i < _manager.TurnOrder[0].Abilities.Count; i++)
            {
                var ability = _manager.TurnOrder[0].Abilities[i];
                Global.SpriteBatch.DrawString(Global.FontDebug, ability.Name, new Vector2(Bounds.X + padding, Bounds.Y + padding + (i * 20)), Color.White);
            }
        }
    }
}
