using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using ProjectDonut.Combat.Combatants.Base;

namespace ProjectDonut.Combat.Combatants
{
    public class Slime : Combatant
    {
        public Slime(TeamType team, CombatManager manager) : base(team, manager)
        {
            InitialiseSprites();
            InitialiseAbilities();
        }

        public override void InitialiseAbilities()
        {
            base.InitialiseAbilities();
        }

        public override void InitialiseSprites()
        {
            var sheetTexture = Global.ContentManager.Load<Texture2D>("Sprites/Combat/Characters/Slime");
            var atlas = Texture2DAtlas.Create("combatant", sheetTexture, Global.TileSize, Global.TileSize);
            _spriteSheet = new SpriteSheet("SpriteSheet/combatant", atlas);

            _spriteSheet.DefineAnimation("idle", builder =>
            {
                builder.IsLooping(true)
                    .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(1))
                    .AddFrame(regionIndex: 1, duration: TimeSpan.FromSeconds(1));
            });

            var cellTime = 0.025f;

            _spriteSheet.DefineAnimation("walk", builder =>
            {
                builder.IsLooping(true)
                    .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(1))
                    .AddFrame(regionIndex: 1, duration: TimeSpan.FromSeconds(1));
            });

            _spriteSheet.DefineAnimation("melee", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(2, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(3, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(4, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(5, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(6, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(7, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(8, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(9, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(10, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(11, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(12, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(13, duration: TimeSpan.FromSeconds(cellTime * 2f));
            });

            _spriteSheet.DefineAnimation("magic", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(2, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(3, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(4, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(5, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(6, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(7, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(8, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(9, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(10, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(11, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(12, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(13, duration: TimeSpan.FromSeconds(cellTime * 2f));
            });

            _spriteSheet.DefineAnimation("ranged", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(2, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(3, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(4, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(5, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(6, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(7, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(8, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(9, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(10, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(11, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(12, duration: TimeSpan.FromSeconds(cellTime * 2f))
                    .AddFrame(13, duration: TimeSpan.FromSeconds(cellTime * 2f));
            });

            Sprite = new AnimatedSprite(_spriteSheet, "idle");

            Sprite.Effect = Team == TeamType.Player ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            ArrowSprite = Global.ContentManager.Load<Texture2D>("Sprites/Combat/placeholder-arrow");
        }
    }
}
