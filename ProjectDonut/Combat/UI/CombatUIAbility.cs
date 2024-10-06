using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Combat.Combatants;
using ProjectDonut.Core;
using ProjectDonut.Core.Input;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using ProjectDonut.Core.Sprites;
using RenderingLibrary.Math.Geometry;

namespace ProjectDonut.Combat.UI
{
    public class CombatUIAbility : ITargetableCombatUI
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

        public CombatUIAbility(CombatManager manager)
        {
            _manager = manager;

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
                if (_indcatorIndex < _manager.TurnOrder[0].Abilities.Count - 1)
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
                CombatScene.Instance.Manager.CombatTurnCurrent.Ability = _manager.TurnOrder[0].Abilities[_indcatorIndex];
                CombatScene.Instance.Manager.CombatTurnCurrent.Action = CombatTurnAction.MagicAttack;
                CombatScene.Instance.ChangeTargetUI(CombatScene.Instance.TargetPickerUI);
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
            DrawAbilities();
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

        private void DrawAbilities()
        {
            int counter = 0;
            for (int i = 0 + linesOffset; i < _manager.TurnOrder[0].Abilities.Count; i++)
            {
                var ability = _manager.TurnOrder[0].Abilities[i];

                if (i == _indcatorIndex)
                {
                    Global.SpriteBatch.DrawString(
                        Global.FontDebug,
                        ability.Name,
                        new Vector2(
                            Bounds.X + padding + 32 + padding,
                            Bounds.Y + padding + i * 20 - linesOffset * 20 + lineHeight),
                        Color.White);
                }
                else
                {
                    Global.SpriteBatch.DrawString(
                        Global.FontDebug,
                        ability.Name,
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

            if (linesOffset < _manager.TurnOrder[0].Abilities.Count - linesToShowCount)
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
