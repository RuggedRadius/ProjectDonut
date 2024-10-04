using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectDonut.Core;
using ProjectDonut.Core.SceneManagement.SceneTypes;

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

            // Gather player party
            var playerTeam = new List<Combatant>()
            {
                new Combatant(TeamType.Player),
                new Combatant(TeamType.Player),
                new Combatant(TeamType.Player),
                new Combatant(TeamType.Player),
            };

            // Create enemy party
            var enemyTeam = new List<Combatant>()
            {
                new Combatant(TeamType.Enemy),
                new Combatant(TeamType.Enemy),
                new Combatant(TeamType.Enemy),
                new Combatant(TeamType.Enemy),
            };

            // Create scene
            var scene = CreateCombatScene(playerTeam, enemyTeam);

            // TODO: Transition FX HERE

            // Switch to combat scene
            Global.SceneManager.SetCurrentScene(scene);
        }

        private CombatScene CreateCombatScene(List<Combatant> playerTeam, List<Combatant> enemyTeam)
        {
            var scene = new CombatScene(playerTeam, enemyTeam, Global.SceneManager.CurrentScene);
            scene.Initialize();
            scene.LoadContent();

            return scene;
        }
    }
}
