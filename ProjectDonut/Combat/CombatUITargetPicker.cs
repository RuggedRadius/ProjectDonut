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
using ProjectDonut.Core.SceneManagement.SceneTypes;
using ProjectDonut.Core.Sprites;
using ProjectDonut.GameObjects.PlayerComponents;

namespace ProjectDonut.Combat
{
    public class CombatUITargetPicker : ITargetableCombatUI
    {
        public bool IsShown { get; set; } = false;
        
        public TeamType TargetTeam { get; set; }
        public Combatant TargetCombatant { get; set; }
        public int _targetIndex = 0;
        
        private CombatManager _manager;

        private Texture2D _indicatorTexture;
        private Rectangle IndicatorBounds;

        public bool IsFirstFrame { get; set; } // Gross

        public CombatUITargetPicker()
        {
            TargetTeam = TeamType.Enemy;
            _indicatorTexture = SpriteLib.Combat.Indicators["pointer-right"];
            _manager = CombatScene.Instance.Manager;
        }

        public void Update(GameTime gameTime)
        {
            if (IsFirstFrame)
            {
                IsFirstFrame = false;
                return;
            }

            if (IsShown == false)
                return;

            if (CombatScene.Instance.CurrentTargetUI != this)
                return;

            if (InputManager.IsKeyPressed(Keys.Escape) || InputManager.IsKeyPressed(Keys.Back))
            {
                CombatScene.Instance.ReturnToPreviousTargetUI();
            }

            if (InputManager.IsKeyPressed(Keys.Space))
            {
                CombatScene.Instance.Manager.CombatTurnCurrent.Target = TargetCombatant;
                CombatScene.Instance.ReturnToPreviousTargetUI();
            }

            HandleSelectionInput();
            UpdateTargetCombatantFromSelection();
        }

        public void Draw(GameTime gameTime)
        {
            if (IsShown == false)
                return;

            if (CombatScene.Instance.CurrentTargetUI != this)
                return;

            Global.SpriteBatch.Draw(
                _indicatorTexture,
                IndicatorBounds, 
                Color.White);
        }

        private void UpdateTargetCombatantFromSelection()
        {
            if (TargetTeam == TeamType.Player)
            {
                TargetCombatant = _manager.PlayerTeam[_targetIndex];
            }
            else
            {
                TargetCombatant = _manager.EnemyTeam[_targetIndex];
            }

            IndicatorBounds = new Rectangle(
                TargetCombatant.Bounds.X,
                TargetCombatant.Bounds.Y,
                _indicatorTexture.Width,
                _indicatorTexture.Height
                );
        }

        private void HandleSelectionInput()
        {
            // Switch teams
            if (InputManager.IsKeyPressed(Keys.Left))
            {
                if (TargetTeam == TeamType.Player)
                {
                    TargetTeam = TeamType.Enemy;
                    _targetIndex = 0;
                }
                else
                {
                    TargetTeam = TeamType.Player;
                    _targetIndex = 0;
                }
            }
            else if (InputManager.IsKeyPressed(Keys.Right))
            {
                if (TargetTeam == TeamType.Player)
                {
                    TargetTeam = TeamType.Enemy;
                    _targetIndex = 0;
                }
                else
                {
                    TargetTeam = TeamType.Player;
                    _targetIndex = 0;
                }
            }

            // Cycle targets up/down
            if (InputManager.IsKeyPressed(Keys.Up))
            {
                DecrementIndex(TargetTeam);
            }
            else if (InputManager.IsKeyPressed(Keys.Down))
            {
                IncrementIndex(TargetTeam);
            }
        }

        public void IncrementIndex(TeamType teamType)
        {
            var team = teamType == TeamType.Player ?
                CombatManager.Instance.PlayerTeam :
                CombatManager.Instance.EnemyTeam;

            if (_targetIndex < team.Count - 1)
            {
                _targetIndex++;
            }
            else
            {
                _targetIndex = 0;
            }

            int maxTries = team.Count + 1;
            while (team[_targetIndex].IsKOd)
            {
                _targetIndex++;
                maxTries--;

                if (_targetIndex >= team.Count)
                {
                    _targetIndex = 0;
                }

                if (maxTries <= 0)
                {
                    break;
                }
            }
        }

        public void DecrementIndex(TeamType teamType)
        {
            var team = teamType == TeamType.Player ? 
                CombatManager.Instance.PlayerTeam : 
                CombatManager.Instance.EnemyTeam;

            if (_targetIndex > 0)
            {
                _targetIndex--;
            }
            else
            {
                _targetIndex = team.Count - 1;
            }

            int maxTries = team.Count + 1;
            while (team[_targetIndex].IsKOd)
            {
                _targetIndex--;
                maxTries--;

                if (_targetIndex < 0)
                {
                    _targetIndex = team.Count - 1;
                }

                if (maxTries <= 0)
                {
                    break;
                }
            }
        }
    }
}
