using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectDonut.Core;

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

            if (_distanceTravelledSinceLastCombat >= 100.0f || _timeSinceLastCombat >= 10.0f)
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
            // Create scene

            // Gather player party

            // Create enemy party

            // Switch to combat scene
        }
    }
}
