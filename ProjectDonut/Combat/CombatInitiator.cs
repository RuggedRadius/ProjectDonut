using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectDonut.Combat.Combatants;
using ProjectDonut.Combat.Combatants.Base;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using ProjectDonut.Tools;

namespace ProjectDonut.Combat
{
    public class CombatInitiator
    {
        private float _totalDistanceTravelled = 0.0f;
        private float _distanceTravelledSinceLastCombat = 0.0f;

        private float _totalTimeTravelled = 0.0f;
        private float _timeSinceLastCombat = 0.0f;

        private Vector2 _playerCurrentPosition;
        private Vector2 _playerPreviousPosition;

        private float _distanceTravelledThisFrame = 0.0f;

        private float _minTimeBetweenCombats = 3f;

        private Random _random = new Random();

        public CombatInitiator()
        {
            _playerCurrentPosition = Global.PlayerObj.WorldPosition;
            _playerPreviousPosition = _playerCurrentPosition;
        }

        public bool ShouldStartCombat()
        {
            var randomPick = _random.Next(0, 100);

            // TODO: Extend this to include more conditions
            if (_distanceTravelledSinceLastCombat >= 100.0f && 
                _timeSinceLastCombat >= _minTimeBetweenCombats)
            {
                return randomPick < 75;
            }
            else
            {
                return randomPick < 10;
            }
        }

        public void Update(GameTime gameTime)
        {
            _playerPreviousPosition = _playerCurrentPosition;
            _playerCurrentPosition = Global.PlayerObj.WorldPosition;

            _distanceTravelledThisFrame = CalculateDistanceTravelled();

            _totalDistanceTravelled += _distanceTravelledThisFrame;
            _distanceTravelledSinceLastCombat += _distanceTravelledThisFrame;

            _totalTimeTravelled += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timeSinceLastCombat += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timeSinceLastCombat >= 15.0f) //_distanceTravelledSinceLastCombat >= 100.0f && 
            {
                if (ShouldStartCombat())
                {
                    _distanceTravelledSinceLastCombat = 0.0f;
                    _timeSinceLastCombat = 0.0f;


                    StartCombat();
                }
            }
        }

        private float CalculateDistanceTravelled()
        {
            return Vector2.Distance(_playerCurrentPosition, _playerPreviousPosition);
        }

        public void StartCombat()
        {
            // TODO: Freeze player movement/ UI interaction
            //...

            var scene = new CombatScene(Global.SceneManager.CurrentScene);

            var playerTeam = new List<Combatant>();
            var enemyTeam = new List<Combatant>();

            for (int i = 0; i < 4; i++)
            {
                playerTeam.Add(new Combatant(TeamType.Player, scene.Manager)
                {
                    Details = new CombatantDetails()
                    {
                        Name = NameGenerator.GenerateRandomName(2)
                    }
                });
            }

            for (int i = 0; i < 4; i++)
            {
                enemyTeam.Add(new Combatant(TeamType.Enemy, scene.Manager)
                {
                    Details = new CombatantDetails()
                    {
                        Name = NameGenerator.GenerateRandomName(2)
                    }
                });
            }

            scene.Manager.AddTeam(playerTeam, true);
            scene.Manager.AddTeam(enemyTeam, false);

            // TODO: Scene transition FX

            scene.Initialize();
            scene.LoadContent();
            Global.SceneManager.SetCurrentScene(scene);
        }
    }
}
