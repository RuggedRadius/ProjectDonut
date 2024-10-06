using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Penumbra;
using ProjectDonut.Combat.Combatants;
using ProjectDonut.Core;
using ProjectDonut.Core.Input;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using ProjectDonut.Core.Sprites;

namespace ProjectDonut.Combat.UI
{
    public class CombatUIStrategyActions : ITargetableCombatUI
    {
        public bool IsShown { get; set; } = false;

        private CombatManager _manager;

        public Rectangle Bounds { get; set; }

        private int linesOffset = 0;

        private int linesToShowCount = 8;
        private int lineHeight = 20;
        private int padding = 10;
        private int Width = 600;
        private int Height = 220;
        private int margin = 10;

        // Indicator
        private Texture2D Indicator;
        private Vector2 IndicatorPosition;
        private int _indcatorIndex = 0;

        // More indicators
        private Texture2D IndicatorMoreUp;
        private Texture2D IndicatorMoreDown;

        public bool IsFirstFrame { get; set; } // Gross


        public CombatUIStrategyActions()
        {
            _manager = CombatScene.Instance.Manager;

            Height = linesToShowCount * lineHeight + padding * 2 + 2 * lineHeight;

            Bounds = new Rectangle(
                320,
                Global.GraphicsDeviceManager.PreferredBackBufferHeight - Height - margin,
                Width,
                Height);

            Indicator = SpriteLib.Combat.Indicators["pointer-right"];
            IndicatorMoreUp = SpriteLib.Combat.Indicators["arrow-up"];
            IndicatorMoreDown = SpriteLib.Combat.Indicators["arrow-down"];
        }

        public void Update(GameTime gameTime)
        {
            if (IsFirstFrame)
            {
                IsFirstFrame = false;
                return;
            }

            if (CombatScene.Instance.CurrentTargetUI != this)
                return;

            if (InputManager.IsKeyPressed(Keys.Escape) || InputManager.IsKeyPressed(Keys.Back))
            {
                CombatScene.Instance.ReturnToPreviousTargetUI();
            }

            if (InputManager.IsKeyPressed(Keys.Up))
            {
                if (_indcatorIndex > 0)
                {
                    _indcatorIndex--;

                    if (_indcatorIndex < linesOffset)
                        linesOffset--;
                }
            }

            if (InputManager.IsKeyPressed(Keys.Down))
            {
                if (_indcatorIndex < Enum.GetValues(typeof(StrategyAction)).Length - 2)
                {
                    _indcatorIndex++;

                    if (_indcatorIndex >= linesToShowCount)
                        linesOffset++;
                }
            }

            IndicatorPosition = new Vector2(
                Bounds.X + padding,
                Bounds.Y + padding + _indcatorIndex * lineHeight - linesOffset * lineHeight + lineHeight);

            if (InputManager.IsKeyPressed(Keys.Space))
            {
                CombatScene.Instance.Manager.CombatTurnCurrent.StrategyAction = (StrategyAction)_indcatorIndex + 1;
                CombatScene.Instance.Manager.CombatTurnCurrent.Action = CombatTurnAction.StrategyAction;

                switch ((StrategyAction)_indcatorIndex + 1)
                {                    
                    case StrategyAction.Taunt:
                    case StrategyAction.MovePosition:
                        CombatScene.Instance.ChangeTargetUI(CombatScene.Instance.TargetPickerUI);
                        break;

                    case StrategyAction.Flee:
                    case StrategyAction.Defend:
                    default:    
                        break;

                    case StrategyAction.None:
                        throw new Exception("This shouldn't be possible");
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (!IsShown)
                return;

            if (_manager.TurnOrder[0].Team == TeamType.Enemy)
                return;

            DrawBackground();
            DrawIndicator();
            DrawStrategyActions();
            DrawMoreIndicators();
        }

        private void DrawBackground()
        {
            if (CombatScene.Instance.CurrentTargetUI == this)
            {
                Global.SpriteBatch.Draw(Global.BLANK_TEXTURE, Bounds, Color.Black * 0.5f);
                Global.SpriteBatch.Draw(Global.BLANK_TEXTURE, Bounds, Color.Yellow * 0.25f);
            }
            else
            {
                Global.SpriteBatch.Draw(Global.BLANK_TEXTURE, Bounds, Color.Black * 0.5f);
            }
        }

        private void DrawIndicator()
        {
            if (CombatScene.Instance.CurrentTargetUI == this)
            {
                Global.SpriteBatch.Draw(Indicator, IndicatorPosition, Color.White);
            }
        }

        private void DrawStrategyActions()
        {
            int counter = 0;
            for (int i = 0 + linesOffset; i < Enum.GetValues(typeof(StrategyAction)).Length - 1; i++)
            {
                var stratAction = (StrategyAction)i + 1;

                if (i == _indcatorIndex)
                {
                    Global.SpriteBatch.DrawString(
                        Global.FontDebug,
                        GetEnumDescription(stratAction),
                        new Vector2(
                            Bounds.X + padding + 32 + padding,
                            Bounds.Y + padding + i * 20 - linesOffset * 20 + lineHeight),
                        Color.White);
                }
                else
                {
                    Global.SpriteBatch.DrawString(
                        Global.FontDebug,
                        GetEnumDescription(stratAction),
                        new Vector2(
                            Bounds.X + padding + 32 + padding,
                            Bounds.Y + padding + i * 20 - linesOffset * 20 + lineHeight),
                        Color.Gray);
                }

                counter++;

                if (counter >= linesToShowCount)
                    break;
            }
        }

        private string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        private void DrawMoreIndicators()
        {
            if (linesOffset > 0)
            {
                Global.SpriteBatch.Draw(IndicatorMoreUp,
                    new Vector2(
                        Bounds.X + Width / 2 - IndicatorMoreUp.Width / 2,
                        Bounds.Y + padding),
                    Color.Gray);
            }

            if (linesOffset < Enum.GetValues(typeof(StrategyAction)).Length - linesToShowCount)
            {
                Global.SpriteBatch.Draw(IndicatorMoreDown,
                    new Vector2(
                        Bounds.X + Width / 2 - IndicatorMoreDown.Width / 2,
                        Bounds.Y + Height - padding - lineHeight),
                    Color.Gray);
            }
        }
    }
}
