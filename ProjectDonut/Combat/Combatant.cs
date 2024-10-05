using Microsoft.Xna.Framework.Graphics;
using System;
using MonoGame.Extended.Graphics;
using ProjectDonut.Core;
using MonoGame.Extended.Animations;
using Microsoft.Xna.Framework;
using ProjectDonut.Combat.UI;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using MonoGame.Extended;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using RenderingLibrary.Graphics;

namespace ProjectDonut.Combat
{
    public enum CombatantMoveState
    {
        Idle,
        MovingToPosition
    }

    public enum CombatantActionState
    {
        Idle,
        TurnInProgress,
        TurnComplete
    }

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
        CombatantMoveState MoveState { get; set; }

        int ExperienceGiven { get; set; }

        //void Attack(AttackType attackType, Combatant target);
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
        public Vector2 BaseScreenPosition { get; set; }
        public Vector2 ScreenPosition { get; set; }
        public Vector2 TargetScreenPosition { get; set; }
        public TeamType Team { get; set; }
        public CombatantMoveState MoveState { get; set; }
        public CombatantActionState ActionState { get; set; }

        public FloatingTextDisplay TextDisplay { get; set; }
        public CombatantOHD OHD { get; set; }
        public CombatantStats Stats { get; set; }
        public CombatantDetails Details { get; set; }
        public CombatManager _manager { get; set; }
        public int ExperienceGiven { get; set; }

        public bool IsKOd => Stats.Health <= 0;

        private SpriteSheet _spriteSheet;       
        public SpriteEffects _spriteEffects;

        public Rectangle Bounds;

        private float _moveTimer = 0f;
        private float _moveTime = 1.5f;

        private float _damageFlashDuration = 0.75f; // Duration of the entire flash effect
        private float _damageFlashTimer = 0f;      // Timer to track how long to keep flashing
        private bool _isDamaged = false;           // Flag to indicate if the player took damage
        private int _flashFrameCounter = 0;        // Counter to alternate between red and white
        private int _framesPerFlash = 8;           // Number of frames before switching colors
        private Color _drawColour;

        private Texture2D _arrowSprite;

        private List<Projectile> Projectiles { get; set; } = new List<Projectile>();

        public Combatant(TeamType team, CombatManager manager)
        {
            _manager = manager;
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

            MoveState = CombatantMoveState.Idle;
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

            _spriteSheet.DefineAnimation("melee", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(regionIndex: 2, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(3, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(4, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(5, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(6, duration: TimeSpan.FromSeconds(cellTime));
            });

            _spriteSheet.DefineAnimation("magic", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(7, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(8, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(9, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(10, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(11, duration: TimeSpan.FromSeconds(cellTime));
            });

            _spriteSheet.DefineAnimation("ranged", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(12, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(13, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(14, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(15, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(16, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(17, duration: TimeSpan.FromSeconds(cellTime));
            });

            Sprite = new AnimatedSprite(_spriteSheet, "idle");

            Sprite.Effect = (Team == TeamType.Player) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            _arrowSprite = Global.ContentManager.Load<Texture2D>("Sprites/Combat/placeholder-arrow");
        }

        public void MoveToScreenPosition(Vector2 screenPosition)
        {
            TargetScreenPosition = screenPosition;
            MoveState = CombatantMoveState.MovingToPosition;
            _moveTimer = 0f;
        }

        public void TakeDamage(int damage)
        {                      
            TextDisplay.AddText(damage.ToString(), 0, Vector2.Zero, Color.Red);
            _isDamaged = true;
            _damageFlashTimer = _damageFlashDuration; // Start the flash timer
            _flashFrameCounter = 0;                  // Reset the frame counter

            Stats.Health -= damage;
        }

        // TODO: Make DEFEND an ability
        public void Defend() => throw new NotImplementedException();
        public void UseItem() => throw new NotImplementedException();
        public void UseAbility() => throw new NotImplementedException();
        public void Flee() => throw new NotImplementedException();

        public void Update(GameTime gameTime)
        {
            Bounds = new Rectangle(
                (int)ScreenPosition.X,
                (int)ScreenPosition.Y,
                Global.TileSize * CombatScene.SceneScale,
                Global.TileSize * CombatScene.SceneScale);

            _moveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_isDamaged)
            {
                _damageFlashTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                _flashFrameCounter++;  // Increment the frame counter on each update

                if (_damageFlashTimer <= 0)
                {
                    _isDamaged = false; // Stop flashing when the timer expires
                    _damageFlashTimer = 0f;
                }

                // Reset the frame counter if it exceeds the flash interval
                if (_flashFrameCounter >= _framesPerFlash)
                {
                    _flashFrameCounter = 0;
                }
            }

            _drawColour = Color.White;

            if (_isDamaged)
            {
                // Flash red or white depending on the current frame counter
                _drawColour = (_flashFrameCounter < _framesPerFlash / 2) ? Color.Red : Color.White;
            }


            switch (MoveState)
            {
               case CombatantMoveState.Idle:
                    break;

                case CombatantMoveState.MovingToPosition:
                    

                    if (ScreenPosition != TargetScreenPosition)
                    {
                        ScreenPosition = Vector2.Lerp(ScreenPosition, TargetScreenPosition, _moveTimer/_moveTime);
                    }
                    else
                    {
                        ScreenPosition = TargetScreenPosition;
                        MoveState = CombatantMoveState.Idle;
                    }
                    break;
                    
                default:
                    break;
            }

            Sprite.Update(gameTime);
            TextDisplay.Update(gameTime);

            Projectiles.ForEach(p => p.Update(gameTime));
            Projectiles.Where(p => p.HasReachedDestination()).ToList().ForEach(p =>
            {
                Projectiles.Remove(p);
                p.Target.TakeDamage(50);
                //ActionState = CombatantActionState.Idle;
            });
        }

        public void Draw(GameTime gameTime)
        {
            //Global.SpriteBatch.DrawString(
            //    Global.FontDebug,
            //    _flashSprite.ToString(),
            //    ScreenPosition,
            //    Color.White
            //);

            if (IsKOd)
            {
                Global.SpriteBatch.Draw(
                    texture: Sprite.TextureRegion.Texture,
                    position: ScreenPosition,
                    sourceRectangle: Sprite.TextureRegion.Bounds,
                    color: Color.Gray * 0.25f,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: Vector2.One * CombatScene.SceneScale,
                    effects: _spriteEffects,
                    layerDepth: 0f
                );
            }
            else
            {
                Global.SpriteBatch.Draw(
                    texture: Sprite.TextureRegion.Texture,
                    position: ScreenPosition,
                    sourceRectangle: Sprite.TextureRegion.Bounds,
                    color: _drawColour,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: Vector2.One * CombatScene.SceneScale,
                    effects: _spriteEffects,
                    layerDepth: 0f
                );
            }

            Projectiles.ForEach(p => p.Draw(gameTime));

            TextDisplay.Draw(gameTime);

            OHD.Draw(gameTime);

            
        }

        public void DrawBounds()
        {                     
            Global.SpriteBatch.DrawRectangle(Bounds, Color.Red, 1);
        }

        public void MoveToMeleePosition(Combatant targetCombatant)
        {
            if (ScreenPosition.X > targetCombatant.ScreenPosition.X)
            {
                MoveToScreenPosition(targetCombatant.ScreenPosition + new Vector2(Global.TileSize * CombatScene.SceneScale, 0));
            }
            else
            {
                MoveToScreenPosition(targetCombatant.ScreenPosition - new Vector2(Global.TileSize * CombatScene.SceneScale, 0));
            }
        }

        public void MoveToRangedPosition()
        {
            if (Team == TeamType.Player)
            {               
                MoveToScreenPosition(BaseScreenPosition + new Vector2(Global.TileSize * CombatScene.SceneScale * 1, 0));
            }
            else
            {
                MoveToScreenPosition(BaseScreenPosition - new Vector2(Global.TileSize * CombatScene.SceneScale * 1, 0));
            }
        }

        public async void AttackCombatant(Combatant target, AttackType attackType)
        {
            if (ActionState != CombatantActionState.Idle)
            {
                return;
            }

            ActionState = CombatantActionState.TurnInProgress;

            await Task.Run(() =>
            {
                switch (attackType)
                {
                    case AttackType.Melee:
                        MoveToMeleePosition(target);

                        while (MoveState != CombatantMoveState.Idle) { }

                        Sprite.SetAnimation("melee").OnAnimationEvent += (sender, trigger) =>
                        {
                            if (trigger == AnimationEventTrigger.AnimationCompleted)
                            {
                                Sprite.SetAnimation("idle");
                            }
                        };

                        while (Sprite.Controller.IsAnimating && Sprite.CurrentAnimation == "melee") { }
                        target.TakeDamage(50);
                        break;

                    case AttackType.Ranged:
                        MoveToRangedPosition();

                        while (MoveState != CombatantMoveState.Idle) { }

                        Sprite.SetAnimation("ranged").OnAnimationEvent += (sender, trigger) =>
                        {
                            if (trigger == AnimationEventTrigger.AnimationCompleted)
                            {
                                Sprite.SetAnimation("idle");
                                //Projectiles.Add(new Projectile(
                                //    _arrowSprite,
                                //    20f,
                                //    ScreenPosition,
                                //    target
                                //));
                            }
                        };

                        while (Sprite.Controller.IsAnimating && Sprite.CurrentAnimation == "ranged") { }
                        //while (Projectiles.Count > 0) { }
                        target.TakeDamage(50);
                        break;

                    case AttackType.Magic:
                        MoveToRangedPosition();

                        while (MoveState != CombatantMoveState.Idle) { }

                        Sprite.SetAnimation("magic").OnAnimationEvent += (sender, trigger) =>
                        {
                            if (trigger == AnimationEventTrigger.AnimationCompleted)
                            {
                                Sprite.SetAnimation("idle");
                            }
                        };

                        while (Sprite.Controller.IsAnimating && Sprite.CurrentAnimation == "magic") { }
                        target.TakeDamage(50);
                        break;
                }

                MoveToScreenPosition(BaseScreenPosition);
                while (MoveState != CombatantMoveState.Idle) { }

                ActionState = CombatantActionState.TurnComplete;

                CombatScene.Instance.Log.AddLogEntry($"{Details.Name} attacked {target.Details.Name} for 50 damage");

                if (target.IsKOd)
                {
                    CombatScene.Instance.Log.AddLogEntry($"{target.Details.Name} has been KO'd");
                }
            });

            _manager.TurnOrder.RemoveAt(0);

            ActionState = CombatantActionState.Idle;
        }
    }
}