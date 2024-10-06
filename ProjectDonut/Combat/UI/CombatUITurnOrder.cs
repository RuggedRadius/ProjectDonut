using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Combat.Combatants;
using ProjectDonut.Core;
using ProjectDonut.Core.Sprites;

namespace ProjectDonut.Combat.UI
{
    public class TurnOrderEntry
    {
        public Combatant Combatant { get; set; }
        public string Name { get; set; }
        public Texture2D Avatar { get; set; }
        public Rectangle Bounds { get; set; }
        public Rectangle AvatarBounds { get; set; }
        public Vector2 NamePosition { get; set; }
        public bool IsPlayerTeam { get; set; }
        public Color BackgroundColour { get; set; }
    }

    public class CombatUITurnOrder
    {
        private List<TurnOrderEntry> _turnsToDraw;
        private CombatManager _manager;

        private int TurnsToDrawCount = 10;

        private Rectangle RectBackground { get; set; }

        private int TurnOrderEntryHeight = 50;
        private int TurnOrderEntryWidth = 200;

        public CombatUITurnOrder(CombatManager manager)
        {
            _manager = manager;
            _turnsToDraw = new List<TurnOrderEntry>();

            RectBackground = new Rectangle(
                0, 
                250, 
                TurnOrderEntryWidth, 
                TurnsToDrawCount * TurnOrderEntryHeight);
        }

        public void Update(GameTime gameTime)
        {
            _turnsToDraw.Clear();

            for (int i = 0; i < TurnsToDrawCount; i++)
            {
                if (i >= _manager.TurnOrder.Count || _manager.TurnOrder[i] == null)
                    break;

                var avatar = _manager.TurnOrder[i].Team == TeamType.Player ?
                    SpriteLib.Combat.Avatars["player"] :
                    SpriteLib.Combat.Avatars["enemy"];
                var isPlayerTeam = _manager.TurnOrder[i].Team == TeamType.Player;

                _turnsToDraw.Add(new TurnOrderEntry()
                {
                    Combatant = _manager.TurnOrder[i],
                    Name = _manager.TurnOrder[i].Details.Name.ToString(),
                    Avatar = avatar,
                    IsPlayerTeam = isPlayerTeam,
                    BackgroundColour = isPlayerTeam ? new Color(153, 255, 153) * 0.5f : new Color(255, 153, 153) * 0.5f,
                    Bounds = new Rectangle(
                        0,
                        250 + (i * TurnOrderEntryHeight),
                        TurnOrderEntryWidth,
                        TurnOrderEntryHeight),
                    AvatarBounds = new Rectangle(
                        0,
                        250 + (i * TurnOrderEntryHeight),
                        50,
                        50),
                    NamePosition = new Vector2(0 + 50 + 20, 250 + 5 + (i * TurnOrderEntryHeight))
                });               
            }
        }

        public void Draw(GameTime gameTime)
        {
            // Draw background
            //Global.SpriteBatch.Draw(Global.BLANK_TEXTURE, RectBackground, null, Color.Black * 0.5f);

            for (int i = 0; i < TurnsToDrawCount; i++)
            {
                if (i >= _manager.TurnOrder.Count || _manager.TurnOrder[i] == null)
                    break;

                // Draw bounds
                Global.SpriteBatch.Draw(Global.BLANK_TEXTURE, _turnsToDraw[i].Bounds, null, _turnsToDraw[i].BackgroundColour);

                // Draw avatar
                Global.SpriteBatch.Draw(_turnsToDraw[i].Avatar, _turnsToDraw[i].AvatarBounds, null, Color.White);

                // Draw name
                Global.SpriteBatch.DrawString(Global.FontDebug, _turnsToDraw[i].Name, _turnsToDraw[i].NamePosition, Color.White);

                // Draw health bar (TODO: mana bar too?)
                var combatantStats = _turnsToDraw[i].Combatant.Stats;
                var healthBarPosition = _turnsToDraw[i].NamePosition + new Vector2(0, 25);
                Global.SpriteBatch.Draw(SpriteLib.UI.HealthBar["left"], healthBarPosition, Color.White);

                for (int j = 0; j < _turnsToDraw[i].Combatant.Stats.MaxHealth; j++)
                {
                    Global.SpriteBatch.Draw(SpriteLib.UI.HealthBar["empty"], healthBarPosition + new Vector2(j + 1, 0), Color.White);
                }

                for (int j = 0; j < _turnsToDraw[i].Combatant.Stats.Health; j++)
                {
                    Global.SpriteBatch.Draw(SpriteLib.UI.HealthBar["full"], healthBarPosition + new Vector2(j + 1, 0), Color.White);
                }

                Global.SpriteBatch.Draw(SpriteLib.UI.HealthBar["right"], healthBarPosition + new Vector2(100 + 1, 0), Color.White);

            }
        }
    }
}
