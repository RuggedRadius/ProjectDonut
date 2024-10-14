using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using ProjectDonut.Combat.Combatants.Base;

namespace ProjectDonut.Combat.Combatants
{
    public class Rat : Combatant
    {
        public Rat(TeamType team, CombatManager manager) : base(team, manager)
        {
            InitialiseSprites();
            InitialiseAbilities();
        }

        public override void InitialiseSprites()
        {
            var sheetTexture = Global.ContentManager.Load<Texture2D>("Sprites/Combat/Characters/Rat2");
            var atlas = Texture2DAtlas.Create("combatant", sheetTexture, Global.TileSize, Global.TileSize);
            _spriteSheet = new SpriteSheet("SpriteSheet/combatant", atlas);

            var cellTime = 0.05f;
            var idleSpeedMultiplier = 4;

            _spriteSheet.DefineAnimation("idle", builder =>
            {
                builder.IsLooping(true)
                    .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(cellTime * idleSpeedMultiplier))
                    .AddFrame(regionIndex: 1, duration: TimeSpan.FromSeconds(cellTime * idleSpeedMultiplier))
                    .AddFrame(regionIndex: 2, duration: TimeSpan.FromSeconds(cellTime * idleSpeedMultiplier));
            });

            _spriteSheet.DefineAnimation("walk", builder =>
            {
                builder.IsLooping(true)
                    .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(cellTime * idleSpeedMultiplier))
                    .AddFrame(regionIndex: 1, duration: TimeSpan.FromSeconds(cellTime * idleSpeedMultiplier))
                    .AddFrame(regionIndex: 2, duration: TimeSpan.FromSeconds(cellTime * idleSpeedMultiplier));
            });

            _spriteSheet.DefineAnimation("melee", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(3, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(4, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(5, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(6, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(7, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(8, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(9, duration: TimeSpan.FromSeconds(cellTime));
            });

            _spriteSheet.DefineAnimation("magic", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(3, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(4, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(5, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(6, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(7, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(8, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(9, duration: TimeSpan.FromSeconds(cellTime));
            });

            _spriteSheet.DefineAnimation("ranged", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(3, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(4, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(5, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(6, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(7, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(8, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(9, duration: TimeSpan.FromSeconds(cellTime));
            });

            Sprite = new AnimatedSprite(_spriteSheet, "idle");

            Sprite.Effect = Team == TeamType.Player ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            ArrowSprite = Global.ContentManager.Load<Texture2D>("Sprites/Combat/placeholder-arrow");
        }
    }
}
