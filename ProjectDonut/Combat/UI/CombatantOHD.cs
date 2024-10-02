﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Core.Sprites;

namespace ProjectDonut.Combat.UI
{
    public class CombatantOHD
    {
        private Combatant _combatant;

        private Texture2D _healthTexture;
        private Texture2D _manaTexture;
        private Texture2D _energyTexture2;

        public CombatantOHD(Combatant combatant)
        {
            _combatant = combatant;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime)
        {
            DrawHealthBar(_combatant.Stats.Health, _combatant.Stats.MaxHealth);
        }

        private int healthBarWidth = 100;
        private void DrawHealthBar(int curHealth, int maxHealth)
        {
            Global.SpriteBatch.Draw(SpriteLib.UI.HealthBar["left"], _combatant.ScreenPosition, Color.White);

            for (int i = 0; i < maxHealth; i++)
            {
                Global.SpriteBatch.Draw(SpriteLib.UI.HealthBar["empty"], _combatant.ScreenPosition + new Vector2(i + 1, 0), Color.White);
            }

            for (int i = 0; i < curHealth; i++)
            {
                Global.SpriteBatch.Draw(SpriteLib.UI.HealthBar["full"], _combatant.ScreenPosition + new Vector2(i + 1, 0), Color.White);
            }

            Global.SpriteBatch.Draw(SpriteLib.UI.HealthBar["right"], _combatant.ScreenPosition + new Vector2(healthBarWidth + 1, 0), Color.White);
        }
    }
}
