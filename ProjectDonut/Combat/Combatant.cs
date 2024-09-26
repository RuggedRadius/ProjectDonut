using Microsoft.Xna.Framework.Graphics;
using System;
using MonoGame.Extended.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Interfaces;
using MonoGame.Extended.Animations;
using Microsoft.Xna.Framework;

namespace ProjectDonut.Combat
{
    public enum AttackType
    {
        Melee,
        Ranged,
        Magic
    }

    public interface ICombatant
    {
        AnimatedSprite Sprite { get; set; }
        Vector2 ScreenPosition { get; set; }

        void Attack(AttackType attackType);
        void Defend();
        void UseItem();
        void UseAbility(); //TODO: Implement abilities
        void Flee();

        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }

    public class Combatant : ICombatant
    {
        public AnimatedSprite Sprite { get; set; }
        public Vector2 ScreenPosition { get; set; }

        private SpriteSheet _spriteSheet;       

        public Combatant()
        {
           InitialiseSprites();
        }

        private void InitialiseSprites()
        {
            var sheetTexture = Global.ContentManager.Load<Texture2D>("Sprites/Combat/placeholder-combatant");
            var atlas = Texture2DAtlas.Create("combatant", sheetTexture, Global.TileSize, Global.TileSize);
            _spriteSheet = new SpriteSheet("SpriteSheet/combatant", atlas);

            _spriteSheet.DefineAnimation("idle", builder =>
            {
                builder.IsLooping(true)
                    .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(1))
                    .AddFrame(regionIndex: 1, duration: TimeSpan.FromSeconds(1));
            });

            var cellTime = 0.25f;

            _spriteSheet.DefineAnimation("attack", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(regionIndex: 2, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(3, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(4, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(5, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(6, duration: TimeSpan.FromSeconds(cellTime));
            });

            Sprite = new AnimatedSprite(_spriteSheet, "idle");
        }

        public void Attack(AttackType attackType) // TODO: Add target argument here, think of multiple targets also
        {
            Sprite.SetAnimation("attack").OnAnimationEvent += (sender, trigger) =>
            {
                if (trigger == AnimationEventTrigger.AnimationCompleted)
                {
                    Sprite.SetAnimation("idle");
                }
            };
        }

        public void Defend()
        {
            throw new NotImplementedException();
        }

        public void UseItem()
        {
            throw new NotImplementedException();
        }

        public void UseAbility()
        {
            throw new NotImplementedException();
        }

        public void Flee()
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Draw(Sprite, ScreenPosition, 0.0f, Vector2.One * 5);
        }
    }
}