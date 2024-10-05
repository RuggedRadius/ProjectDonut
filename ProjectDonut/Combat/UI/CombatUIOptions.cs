using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Core;
using ProjectDonut.Core.Input;

namespace ProjectDonut.Combat.UI
{
    public enum CombatUIOptionsType
    {
        Attack,
        Ability,
        Item,
        Flee
    }

    public class CombatUIOptions
    { 
        private CombatManager _manager;

        private CombatUIOptionsType _selectedOption;

        public bool MenuAttackShown { get; set; }
        public bool MenuAbilityShown { get; set; }
        public bool MenuItemShown { get; set; }

        private Rectangle RectBackground { get; set; }
        private Vector2 ScreenPositionBackground { get; set; }
        private Vector2 ScreenPositionAttack { get; set; }
        private Vector2 ScreenPositionAbility { get; set; }
        private Vector2 ScreenPositionItem { get; set; }
        private Vector2 ScreenPositionFlee { get; set; }

        private Texture2D Indicator;

        public CombatUIOptions(CombatManager manager)
        {
            MenuAttackShown = false;
            MenuAbilityShown = false;
            MenuItemShown = false;

            // 20px spacing in + up
            // 50px per option
            // 10px internal padding
            ScreenPositionBackground = new Vector2(20, Global.GraphicsDeviceManager.PreferredBackBufferHeight - 240);
            RectBackground = new Rectangle((int)ScreenPositionBackground.X, (int)ScreenPositionBackground.Y, 300, 220);

            ScreenPositionAttack = new Vector2(ScreenPositionBackground.X + 50, ScreenPositionBackground.Y + 10);
            ScreenPositionAbility = new Vector2(ScreenPositionBackground.X + 50, ScreenPositionBackground.Y + 10 + 50);
            ScreenPositionItem = new Vector2(ScreenPositionBackground.X + 50, ScreenPositionBackground.Y + 10 + 100);
            ScreenPositionFlee = new Vector2(ScreenPositionBackground.X + 50, ScreenPositionBackground.Y + 10 + 150);

            Indicator = Global.ContentManager.Load<Texture2D>("Sprites/Combat/indicator");
            _manager = manager;
        }

        public void Update(GameTime gameTime)
        {
            if (_manager.TurnOrder[0].Team == TeamType.Enemy)
            {
                return;
            }

            // Update the combat UI options
            if (InputManager.IsKeyPressed(Keys.Up))
            {
                var index = (int)_selectedOption;
                index--;

                if (index < 0)
                {
                    index = Enum.GetValues(typeof(CombatUIOptionsType)).Length - 1;
                }

                _selectedOption = (CombatUIOptionsType)index;
            }

            if (InputManager.IsKeyPressed(Keys.Down))
            {
                var index = (int)_selectedOption;
                index++;

                if (index >= Enum.GetValues(typeof(CombatUIOptionsType)).Length)
                {
                    index = 0;
                }

                _selectedOption = (CombatUIOptionsType)index;
            }

            // Update the combat UI options
            if (InputManager.IsKeyPressed(Keys.Space))
            {
                switch (_selectedOption)
                {
                    case CombatUIOptionsType.Attack:
                        // TEMP **************************
                        _manager.TESTDoRandomPlayerTeamAttack();
                        break;
                        // *******************************

                        MenuAttackShown = !MenuAttackShown;

                        MenuAbilityShown = false;
                        MenuItemShown = false;
                        break;

                    case CombatUIOptionsType.Ability:
                        MenuAbilityShown = !MenuAbilityShown;

                        MenuAttackShown = false;
                        MenuItemShown = false;
                        break;

                    case CombatUIOptionsType.Item:
                        MenuItemShown = !MenuItemShown;

                        MenuAttackShown = false;
                        MenuAbilityShown = false;
                        break;

                    case CombatUIOptionsType.Flee:
                        break;
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            // Draw background
            Global.SpriteBatch.Draw(Global.BLANK_TEXTURE, RectBackground, null, Color.Black * 0.5f);

            // Draw the combat UI options
            Global.SpriteBatch.DrawString(Global.FontDebug, "Attack", ScreenPositionAttack, Color.White);
            Global.SpriteBatch.DrawString(Global.FontDebug, "Ability", ScreenPositionAbility, Color.White);
            Global.SpriteBatch.DrawString(Global.FontDebug, "Item", ScreenPositionItem, Color.White);
            Global.SpriteBatch.DrawString(Global.FontDebug, "Flee", ScreenPositionFlee, Color.White);

            // Draw selection indicator
            if (_manager.TurnOrder[0].Team == TeamType.Player)
            {
                switch (_selectedOption)
                {
                    case CombatUIOptionsType.Attack:
                        Global.SpriteBatch.Draw(Indicator, ScreenPositionAttack - new Vector2(32, 0), Color.White);
                        break;
                    case CombatUIOptionsType.Ability:
                        Global.SpriteBatch.Draw(Indicator, ScreenPositionAbility - new Vector2(32, 0), Color.White);
                        break;
                    case CombatUIOptionsType.Item:
                        Global.SpriteBatch.Draw(Indicator, ScreenPositionItem - new Vector2(32, 0), Color.White);
                        break;
                    case CombatUIOptionsType.Flee:
                        Global.SpriteBatch.Draw(Indicator, ScreenPositionFlee - new Vector2(32, 0), Color.White);
                        break;
                }
            }

            // Menus
            if (MenuAttackShown)
            {
                Global.SpriteBatch.DrawString(Global.FontDebug, "Attack Menu", new Vector2(200, 100), Color.White);
            }
            if (MenuAbilityShown)
            {
                Global.SpriteBatch.DrawString(Global.FontDebug, "Ability Menu", new Vector2(200, 120), Color.White);
            }
            if (MenuItemShown)
            {
                Global.SpriteBatch.DrawString(Global.FontDebug, "Item Menu", new Vector2(200, 140), Color.White);
            }
        }
    }
}
