using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectDonut.Core;

namespace ProjectDonut.Combat
{
    public class CombatUIAbility
    {
        public bool IsShown { get; set; } = false;

        private CombatManager _manager;

        private Rectangle Bounds { get; set; }
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

        }

        public void Draw(GameTime gameTime)
        {
            if (!IsShown)
                return;

            // Draw background
            Global.SpriteBatch.Draw(Global.BLANK_TEXTURE, Bounds, Color.Black * 0.5f);
        }
    }
}
