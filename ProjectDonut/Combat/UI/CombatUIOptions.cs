using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Combat.Combatants;
using ProjectDonut.Core;
using ProjectDonut.Core.Input;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using ProjectDonut.Core.Sprites;

namespace ProjectDonut.Combat.UI
{
    public enum CombatUIOptionsType
    {
        Attack,
        Ability,
        Item,
        CombatAction // flee, change position
    }

    public class CombatUIOptions : ITargetableCombatUI
    { 
        public bool IsShown { get; set; }

        private CombatManager _manager;

        private CombatUIOptionsType _selectedOption;

        private Rectangle RectBackground { get; set; }
        private Vector2 ScreenPositionBackground { get; set; }
        private Vector2 ScreenPositionAttack { get; set; }
        private Vector2 ScreenPositionAbility { get; set; }
        private Vector2 ScreenPositionItem { get; set; }
        private Vector2 ScreenPositionCombatActions { get; set; }

        private Texture2D Indicator;

        private int height = 220;
        private int width = 300;
        private int margin = 10;
        private int padding = 10;
        private int indicatorWidth = 32;
        private int optionHeight = 50;
        private int indicatorHeighOffset;

        public CombatUIOptions()
        {
            IsShown = true;

            ScreenPositionBackground = new Vector2(margin, Global.GraphicsDeviceManager.PreferredBackBufferHeight - margin - height);
            RectBackground = new Rectangle((int)ScreenPositionBackground.X, (int)ScreenPositionBackground.Y, width, height);

            ScreenPositionAttack = new Vector2(ScreenPositionBackground.X + (3 * padding) + indicatorWidth, ScreenPositionBackground.Y + (2 * padding));
            ScreenPositionAbility = new Vector2(ScreenPositionBackground.X + (3 * padding) + indicatorWidth, ScreenPositionBackground.Y + (2 * padding) + optionHeight);
            ScreenPositionItem = new Vector2(ScreenPositionBackground.X + (3 * padding) + indicatorWidth, ScreenPositionBackground.Y + (2 * padding) + (2 * optionHeight));
            ScreenPositionCombatActions = new Vector2(ScreenPositionBackground.X + (3 * padding) + indicatorWidth, ScreenPositionBackground.Y + (2 * padding) + (3 * optionHeight));

            Indicator = SpriteLib.Combat.Indicators["pointer-right"];
            _manager = CombatScene.Instance.Manager;

            indicatorHeighOffset = ((int)Global.FontDebug.MeasureString("ABC").Y / 2) + Indicator.Height / 2;
        }

        public void Update(GameTime gameTime)
        {
            if (_manager.IsExecutingTurn)
                return;

            if (_manager.TurnOrder[0].Team == TeamType.Enemy)
                return;

            if (CombatScene.Instance.CurrentTargetUI != this)
                return;

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

            HandleComponentWindowVisibility();
            HandleInput();
        }

        private void HandleInput()
        {
            if (InputManager.IsKeyPressed(Keys.Space) == false)
            {
                return;
            }

            switch (_selectedOption)
            {
                case CombatUIOptionsType.Attack:
                    CombatScene.Instance.Manager.CombatTurnCurrent.Action = CombatTurnAction.PhysicalAttack;
                    CombatScene.Instance.ChangeTargetUI(CombatScene.Instance.TargetPickerUI);
                    break;

                case CombatUIOptionsType.Ability:
                    CombatScene.Instance.Manager.CombatTurnCurrent.Action = CombatTurnAction.MagicAttack;
                    CombatScene.Instance.ChangeTargetUI(CombatScene.Instance.AbilityUI);
                    break;

                case CombatUIOptionsType.Item:
                    CombatScene.Instance.Manager.CombatTurnCurrent.Action = CombatTurnAction.UseItem;
                    CombatScene.Instance.ChangeTargetUI(CombatScene.Instance.ItemUI);
                    break;

                case CombatUIOptionsType.CombatAction:
                    CombatScene.Instance.Manager.CombatTurnCurrent.Action = CombatTurnAction.UseCombatAction;
                    CombatScene.Instance.ChangeTargetUI(CombatScene.Instance.CombatActionsUI);
                    break;
            }
        }

        private void HandleComponentWindowVisibility()
        {
            switch (_selectedOption)
            {
                case CombatUIOptionsType.Attack:
                    CombatScene.Instance.AbilityUI.IsShown = false;
                    CombatScene.Instance.ItemUI.IsShown = false;
                    CombatScene.Instance.CombatActionsUI.IsShown = false;
                    break;

                case CombatUIOptionsType.Ability:
                    CombatScene.Instance.AbilityUI.IsShown = true;
                    CombatScene.Instance.ItemUI.IsShown = false;
                    CombatScene.Instance.CombatActionsUI.IsShown = false;
                    break;

                case CombatUIOptionsType.Item:
                    CombatScene.Instance.ItemUI.IsShown = true;
                    CombatScene.Instance.AbilityUI.IsShown = false;
                    CombatScene.Instance.CombatActionsUI.IsShown = false;
                    break;

                case CombatUIOptionsType.CombatAction:
                    CombatScene.Instance.CombatActionsUI.IsShown = true;
                    CombatScene.Instance.AbilityUI.IsShown = false;
                    CombatScene.Instance.ItemUI.IsShown = false;
                    break;
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (_manager.IsExecutingTurn)
                return;

            // Draw background
            if (CombatScene.Instance.CurrentTargetUI != this)
                Global.SpriteBatch.Draw(Global.BLANK_TEXTURE, RectBackground, null, Color.Black * 0.5f);
            else
                Global.SpriteBatch.Draw(Global.BLANK_TEXTURE, RectBackground, null, Color.Yellow * 0.5f);

            // Draw the combat UI options
            Global.SpriteBatch.DrawString(Global.FontDebug, "Melee Attack", ScreenPositionAttack, _selectedOption == CombatUIOptionsType.Attack ? Color.White : Color.Gray);
            Global.SpriteBatch.DrawString(Global.FontDebug, "Ability", ScreenPositionAbility, _selectedOption == CombatUIOptionsType.Ability ? Color.White : Color.Gray);
            Global.SpriteBatch.DrawString(Global.FontDebug, "Item", ScreenPositionItem, _selectedOption == CombatUIOptionsType.Item ? Color.White : Color.Gray);
            Global.SpriteBatch.DrawString(Global.FontDebug, "Strategy Action", ScreenPositionCombatActions, _selectedOption == CombatUIOptionsType.CombatAction ? Color.White : Color.Gray);

            // Draw selection indicator
            if (CombatScene.Instance.CurrentTargetUI != this)
                return;

            if (_manager.TurnOrder[0].Team == TeamType.Player)
            {
                switch (_selectedOption)
                {
                    case CombatUIOptionsType.Attack:
                        Global.SpriteBatch.Draw(Indicator, ScreenPositionAttack - new Vector2(32 + padding, 7), Color.White);
                        break;
                    case CombatUIOptionsType.Ability:
                        Global.SpriteBatch.Draw(Indicator, ScreenPositionAbility - new Vector2(32 + padding, 7), Color.White);
                        break;
                    case CombatUIOptionsType.Item:
                        Global.SpriteBatch.Draw(Indicator, ScreenPositionItem - new Vector2(32 + padding, 7), Color.White);
                        break;
                    case CombatUIOptionsType.CombatAction:
                        Global.SpriteBatch.Draw(Indicator, ScreenPositionCombatActions - new Vector2(32 + padding, 7), Color.White);
                        break;
                }
            }
        }
    }
}
