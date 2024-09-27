using Microsoft.Xna.Framework.Graphics;
using System;
using MonoGame.Extended.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Interfaces;
using MonoGame.Extended.Animations;
using Microsoft.Xna.Framework;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.Combat.UI;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using MonoGame.Extended;

namespace ProjectDonut.Combat
{
    public enum AttackType
    {
        Melee,
        Ranged,
        Magic
    }

    public enum TeamType
    {
        Player,
        Enemy
    }

    public interface ICombatant
    {
        AnimatedSprite Sprite { get; set; }
        Vector2 ScreenPosition { get; set; }
        TeamType Team { get; set; }

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
        public TeamType Team { get; set; }

        public FloatingTextDisplay TextDisplay { get; set; }
        public CombatantOHD OHD { get; set; }
        public CombatantStats Stats { get; set; }

        private SpriteSheet _spriteSheet;       
        private SpriteEffects _spriteEffects;

        public Rectangle Bounds;

        public Combatant(TeamType team)
        {
            InitialiseSprites();
            Team = team;

            TextDisplay = new FloatingTextDisplay(this);
            OHD = new CombatantOHD(this);
            Stats = new CombatantStats();

            _spriteEffects = (Team == TeamType.Player) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Bounds = new Rectangle(
                (int)ScreenPosition.X,
                (int)ScreenPosition.Y,
                Global.TileSize * CombatScene.SceneScale,
                Global.TileSize * CombatScene.SceneScale);
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

            Sprite.Effect = (Team == TeamType.Player) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        public void Attack(AttackType attackType) // TODO: Add target argument here, think of multiple targets also
        {
            switch (attackType)
            {
                case AttackType.Melee:
                case AttackType.Ranged:
                case AttackType.Magic:
                default:
                    Sprite.SetAnimation("attack").OnAnimationEvent += (sender, trigger) =>
                    {
                        if (trigger == AnimationEventTrigger.AnimationCompleted)
                        {
                            Sprite.SetAnimation("idle");
                        }
                    };
                    break;
            }
        }

        public void TakeDamage(int damage)
        {                      
            TextDisplay.AddText(damage.ToString(), 0, Vector2.Zero, Color.Red);
        }

        public void Defend() // TODO: Make DEFEND an ability
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
            Bounds = new Rectangle(
                (int)ScreenPosition.X,
                (int)ScreenPosition.Y,
                Global.TileSize * CombatScene.SceneScale,
                Global.TileSize * CombatScene.SceneScale);

            Sprite.Update(gameTime);
            TextDisplay.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Draw(
                texture: Sprite.TextureRegion.Texture,
                position: ScreenPosition,
                sourceRectangle: Sprite.TextureRegion.Bounds,
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: Vector2.One * CombatScene.SceneScale,
                effects: _spriteEffects,
                layerDepth: 0f
            );
            TextDisplay.Draw(gameTime);
        }

        public void DrawBounds()
        {                     
            Global.SpriteBatch.DrawRectangle(Bounds, Color.Red, 1);
        }
    }
}