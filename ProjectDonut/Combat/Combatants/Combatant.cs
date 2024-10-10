using Microsoft.Xna.Framework.Graphics;
using System;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Animations;
using Microsoft.Xna.Framework;
using ProjectDonut.Combat.UI;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using MonoGame.Extended;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using RenderingLibrary.Graphics;
using System.ComponentModel;

namespace ProjectDonut.Combat.Combatants
{
    public enum CombatantMoveState
    {
        Idle,
        MovingToPosition,
        MovingBackToBasePosition
    }

    public enum CombatantActionState
    {
        Idle,
        TurnInProgress,
        TurnComplete
    }

    public enum CombatTurnAction
    {
        Undecided,
        PhysicalAttack,
        MagicAttack,
        UseItem,
        StrategyAction
    }

    public enum StrategyAction
    {
        [Description("None")] None,
        [Description("Flee")] Flee,
        [Description("Taunt")] Taunt,
        [Description("Defend")] Defend,
        [Description("Swap Positions")] MovePosition,        
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
        public Vector2 LastPosition { get; set; }
        public TeamType Team { get; set; }
        public CombatantMoveState MoveState { get; set; }
        public CombatantActionState ActionState { get; set; }

        public List<CombatAbility> Abilities { get; set; }


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

        private Random _random = new Random();

        public Combatant(TeamType team, CombatManager manager)
        {
            _manager = manager;

            if (_random.Next(2) == 0)
                InitialiseSpritesGoblin();
            else
                InitialiseSpritesSkeleton();

            Team = team;

            TextDisplay = new FloatingTextDisplay(this);
            OHD = new CombatantOHD(this);
            Stats = new CombatantStats();
            Stats.TEST_RandomiseStats();

            _spriteEffects = Team != TeamType.Player ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Bounds = new Rectangle(
                (int)ScreenPosition.X,
                (int)ScreenPosition.Y,
                Global.TileSize * CombatScene.SceneScale,
                Global.TileSize * CombatScene.SceneScale);

            MoveState = CombatantMoveState.Idle;

            ExperienceGiven = _random.Next(100, 201);

            InitialiseAbilities();
        }

        private void InitialiseAbilities()
        {
            Abilities = new List<CombatAbility>();

            Abilities.Add(new CombatAbility
            {
                Name = "Lighting Shot",
                Description = "A lightning attack.",
                ManaCost = 0,
                EnergyCost = 0,
                DamageMin = 5,
                DamageMax = 10,
                DamageType = DamageType.ElementalThunder
            });

            Abilities.Add(new CombatAbility
            {
                Name = "Fireball",
                Description = "A fireball attack.",
                ManaCost = 10,
                EnergyCost = 0,
                DamageMin = 10,
                DamageMax = 20,
                DamageType = DamageType.ElementalFire
            });

            Abilities.Add(new CombatAbility
            {
                Name = "Water Blast",
                Description = "An ice attack.",
                ManaCost = 10,
                EnergyCost = 0,
                DamageMin = 10,
                DamageMax = 20,
                DamageType = DamageType.ElementalWater
            });

            Abilities.Add(new CombatAbility
            {
                Name = "Earthquake",
                Description = "An earth attack.",
                ManaCost = 10,
                EnergyCost = 0,
                DamageMin = 10,
                DamageMax = 20,
                DamageType = DamageType.ElementalEarth
            });

            Abilities.Add(new CombatAbility
            {
                Name = "Heal",
                Description = "Heal yourself.",
                ManaCost = 10,
                EnergyCost = 0,
                DamageMin = 10,
                DamageMax = 20,
                DamageType = DamageType.Holy
            });

            Abilities.Add(new CombatAbility
            {
                Name = "Revive",
                Description = "Revive a fallen ally.",
                ManaCost = 10,
                EnergyCost = 0,
                DamageMin = 10,
                DamageMax = 20,
                DamageType = DamageType.Holy
            });

            Abilities.Add(new CombatAbility
            {
                Name = "Defend",
                Description = "Increase your defense.",
                ManaCost = 0,
                EnergyCost = 0,
                DamageMin = 0,
                DamageMax = 0,
                DamageType = DamageType.None
            });

            Abilities.Add(new CombatAbility
            {
                Name = "Taunt",
                Description = "Taunt the enemy.",
                ManaCost = 0,
                EnergyCost = 0,
                DamageMin = 0,
                DamageMax = 0,
                DamageType = DamageType.None
            });

            Abilities.Add(new CombatAbility
            {
                Name = "Flee",
                Description = "Run away.",
                ManaCost = 0,
                EnergyCost = 0,
                DamageMin = 0,
                DamageMax = 0,
                DamageType = DamageType.None
            });

            Abilities.Add(new CombatAbility
            {
                Name = "Move",
                Description = "Move to a new position.",
                ManaCost = 0,
                EnergyCost = 0,
                DamageMin = 0,
                DamageMax = 0,
                DamageType = DamageType.None
            });

            Abilities.Add(new CombatAbility
            {
                Name = "Item",
                Description = "Use an item.",
                ManaCost = 0,
                EnergyCost = 0,
                DamageMin = 0,
                DamageMax = 0,
                DamageType = DamageType.None
            });

            Abilities.Add(new CombatAbility
            {
                Name = "Physical Attack",
                Description = "A physical attack.",
                ManaCost = 0,
                EnergyCost = 0,
                DamageMin = 5,
                DamageMax = 10,
                DamageType = DamageType.PhysicalBlunt
            });

            Abilities.Add(new CombatAbility
            {
                Name = "Magic Attack",
                Description = "A magic attack.",
                ManaCost = 10,
                EnergyCost = 0,
                DamageMin = 10,
                DamageMax = 20,
                DamageType = DamageType.ElementalThunder
            });

            Abilities.Add(new CombatAbility
            {
                Name = "Item",
                Description = "Use an item.",
                ManaCost = 0,
                EnergyCost = 0,
                DamageMin = 0,
                DamageMax = 0,
                DamageType = DamageType.None
            });

            Abilities.Add(new CombatAbility
            {
                Name = "Combat Action",
                Description = "Flee, change position.",
                ManaCost = 0,
                EnergyCost = 0,
                DamageMin = 0,
                DamageMax = 0,
                DamageType = DamageType.None
            });

            Abilities.Add(new CombatAbility
            {
                Name = "Physical Attack",
                Description = "A physical attack.",
                ManaCost = 0,
                EnergyCost = 0,
                DamageMin = 5,
                DamageMax = 10,
                DamageType = DamageType.PhysicalBlunt
            });

            Abilities.Add(new CombatAbility
            {
                Name = "Magic Attack",
                Description = "A magic attack.",
                ManaCost = 10,
                EnergyCost = 0,
                DamageMin = 10,
                DamageMax = 20,
                DamageType = DamageType.ElementalThunder
            });

            Abilities.Add(new CombatAbility
            {
                Name = "Item",
                Description = "Use an item.",
                ManaCost = 0,
                EnergyCost = 0,
                DamageMin = 0,
                DamageMax = 0,
                DamageType = DamageType.None
            });
        }


        //private void InitialiseSprites()
        //{
        //    var sheetTexture = Global.ContentManager.Load<Texture2D>("Sprites/Combat/placeholder-combatant");
        //    var atlas = Texture2DAtlas.Create("combatant", sheetTexture, Global.TileSize, Global.TileSize);
        //    _spriteSheet = new SpriteSheet("SpriteSheet/combatant", atlas);

        //    _spriteSheet.DefineAnimation("idle", builder =>
        //    {
        //        builder.IsLooping(true)
        //            .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(1))
        //            .AddFrame(regionIndex: 1, duration: TimeSpan.FromSeconds(1));
        //    });

        //    var cellTime = 0.25f;

        //    _spriteSheet.DefineAnimation("melee", builder =>
        //    {
        //        builder.IsLooping(false)
        //            .AddFrame(regionIndex: 2, duration: TimeSpan.FromSeconds(cellTime))
        //            .AddFrame(3, duration: TimeSpan.FromSeconds(cellTime))
        //            .AddFrame(4, duration: TimeSpan.FromSeconds(cellTime))
        //            .AddFrame(5, duration: TimeSpan.FromSeconds(cellTime))
        //            .AddFrame(6, duration: TimeSpan.FromSeconds(cellTime));
        //    });

        //    _spriteSheet.DefineAnimation("magic", builder =>
        //    {
        //        builder.IsLooping(false)
        //            .AddFrame(7, duration: TimeSpan.FromSeconds(cellTime))
        //            .AddFrame(8, duration: TimeSpan.FromSeconds(cellTime))
        //            .AddFrame(9, duration: TimeSpan.FromSeconds(cellTime))
        //            .AddFrame(10, duration: TimeSpan.FromSeconds(cellTime))
        //            .AddFrame(11, duration: TimeSpan.FromSeconds(cellTime));
        //    });

        //    _spriteSheet.DefineAnimation("ranged", builder =>
        //    {
        //        builder.IsLooping(false)
        //            .AddFrame(12, duration: TimeSpan.FromSeconds(cellTime))
        //            .AddFrame(13, duration: TimeSpan.FromSeconds(cellTime))
        //            .AddFrame(14, duration: TimeSpan.FromSeconds(cellTime))
        //            .AddFrame(15, duration: TimeSpan.FromSeconds(cellTime))
        //            .AddFrame(16, duration: TimeSpan.FromSeconds(cellTime))
        //            .AddFrame(17, duration: TimeSpan.FromSeconds(cellTime));
        //    });

        //    Sprite = new AnimatedSprite(_spriteSheet, "idle");

        //    Sprite.Effect = Team == TeamType.Player ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        //    _arrowSprite = Global.ContentManager.Load<Texture2D>("Sprites/Combat/placeholder-arrow");
        //}

        private void InitialiseSpritesGoblin()
        {
            var sheetTexture = Global.ContentManager.Load<Texture2D>("Sprites/Combat/Characters/Goblin");
            var atlas = Texture2DAtlas.Create("combatant", sheetTexture, Global.TileSize, Global.TileSize);
            _spriteSheet = new SpriteSheet("SpriteSheet/combatant", atlas);

            _spriteSheet.DefineAnimation("idle", builder =>
            {
                builder.IsLooping(true)
                    .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(1))
                    .AddFrame(regionIndex: 1, duration: TimeSpan.FromSeconds(1));
            });

            var cellTime = 0.1f;

            _spriteSheet.DefineAnimation("walk", builder =>
            {
                builder.IsLooping(true)
                    .AddFrame(2, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(3, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(4, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(5, duration: TimeSpan.FromSeconds(cellTime));
            });

            _spriteSheet.DefineAnimation("melee", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(8, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(9, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(10, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(11, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(12, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(13, duration: TimeSpan.FromSeconds(cellTime));
            });

            _spriteSheet.DefineAnimation("magic", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(6, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(7, duration: TimeSpan.FromSeconds(cellTime));
            });

            _spriteSheet.DefineAnimation("ranged", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(8, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(9, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(10, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(11, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(12, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(13, duration: TimeSpan.FromSeconds(cellTime));
            });

            Sprite = new AnimatedSprite(_spriteSheet, "idle");

            Sprite.Effect = Team == TeamType.Player ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            _arrowSprite = Global.ContentManager.Load<Texture2D>("Sprites/Combat/placeholder-arrow");
        }

        private void InitialiseSpritesSkeleton()
        {
            var sheetTexture = Global.ContentManager.Load<Texture2D>("Sprites/Combat/Characters/Skeleton2");
            var atlas = Texture2DAtlas.Create("combatant", sheetTexture, Global.TileSize, Global.TileSize);
            _spriteSheet = new SpriteSheet("SpriteSheet/combatant", atlas);

            _spriteSheet.DefineAnimation("idle", builder =>
            {
                builder.IsLooping(true)
                    .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(1))
                    .AddFrame(regionIndex: 1, duration: TimeSpan.FromSeconds(1));
            });

            var cellTime = 0.05f;

            _spriteSheet.DefineAnimation("walk", builder =>
            {
                builder.IsLooping(true)
                    .AddFrame(2, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(3, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(4, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(5, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(6, duration: TimeSpan.FromSeconds(cellTime));
            });

            _spriteSheet.DefineAnimation("melee", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(7, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(8, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(9, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(10, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(11, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(12, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(13, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(14, duration: TimeSpan.FromSeconds(cellTime));
            });

            _spriteSheet.DefineAnimation("magic", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(7, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(8, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(9, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(10, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(11, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(12, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(13, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(14, duration: TimeSpan.FromSeconds(cellTime));
            });

            _spriteSheet.DefineAnimation("ranged", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(7, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(8, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(9, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(10, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(11, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(12, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(13, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(14, duration: TimeSpan.FromSeconds(cellTime));
            });

            Sprite = new AnimatedSprite(_spriteSheet, "idle");

            Sprite.Effect = Team == TeamType.Player ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

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
            if (damage < 0)
                damage = 0;

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
                _drawColour = _flashFrameCounter < _framesPerFlash / 2 ? Color.Red : Color.White;
            }


            switch (MoveState)
            {
                case CombatantMoveState.Idle:
                    break;

                case CombatantMoveState.MovingToPosition:
                    if (ScreenPosition != TargetScreenPosition)
                    {
                        if (BaseScreenPosition.X < TargetScreenPosition.X)
                        {
                            if (ScreenPosition.X > TargetScreenPosition.X)
                            {
                                ScreenPosition = TargetScreenPosition;
                                MoveState = CombatantMoveState.Idle;
                                _spriteEffects = Team != TeamType.Player ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                            }
                            else
                            {
                                ScreenPosition = Vector2.Lerp(BaseScreenPosition, TargetScreenPosition, _moveTimer / _moveTime);
                            }
                        }                        
                        else if (BaseScreenPosition.X > TargetScreenPosition.X)
                        {
                            if (ScreenPosition.X < TargetScreenPosition.X)
                            {
                                ScreenPosition = TargetScreenPosition;
                                MoveState = CombatantMoveState.Idle;
                                _spriteEffects = Team != TeamType.Player ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                            }
                            else
                            {
                                ScreenPosition = Vector2.Lerp(BaseScreenPosition, TargetScreenPosition, _moveTimer / _moveTime);
                            }
                        }
                    }
                    else
                    {
                        ScreenPosition = TargetScreenPosition;
                        MoveState = CombatantMoveState.Idle;
                        _spriteEffects = Team != TeamType.Player ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    }
                    break;

                case CombatantMoveState.MovingBackToBasePosition:
                    if (ScreenPosition != BaseScreenPosition)
                    {
                        if (LastPosition.X < BaseScreenPosition.X)
                        {
                            if (ScreenPosition.X > BaseScreenPosition.X)
                            {
                                ScreenPosition = BaseScreenPosition;
                                MoveState = CombatantMoveState.Idle;
                                _spriteEffects = Team != TeamType.Player ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                            }
                            else
                            {
                                ScreenPosition = Vector2.Lerp(LastPosition, BaseScreenPosition, _moveTimer / _moveTime);
                            }
                        }
                        else if (LastPosition.X > BaseScreenPosition.X)
                        {
                            if (ScreenPosition.X < BaseScreenPosition.X)
                            {
                                ScreenPosition = BaseScreenPosition;
                                MoveState = CombatantMoveState.Idle;
                                _spriteEffects = Team != TeamType.Player ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                            }
                            else
                            {
                                ScreenPosition = Vector2.Lerp(LastPosition, BaseScreenPosition, _moveTimer / _moveTime);
                            }
                        }
                    }
                    else
                    {
                        ScreenPosition = BaseScreenPosition;
                        MoveState = CombatantMoveState.Idle;
                        _spriteEffects = Team != TeamType.Player ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
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

        public void MoveBackToBasePosition()
        {
            _spriteEffects = Team == TeamType.Player ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            LastPosition = ScreenPosition;
            MoveState = CombatantMoveState.MovingBackToBasePosition;
            _moveTimer = 0f;
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

        #region Combat Options
        public void UseCombatAction(ref CombatTurn turn)
        {
            if (ActionState != CombatantActionState.Idle)
            {
                return;
            }

            ActionState = CombatantActionState.TurnInProgress;

            switch (turn.StrategyAction)
            {
                case StrategyAction.Flee:
                    // Flee
                    break;

                case StrategyAction.MovePosition:
                    // Move to a new position
                    break;

                case StrategyAction.Taunt:
                    // Taunt
                    break;
            }

            ActionState = CombatantActionState.Idle;
        }

        public void PhysicalAttack(ref CombatTurn turn)
        {
            if (ActionState != CombatantActionState.Idle)
            {
                return;
            }

            ActionState = CombatantActionState.TurnInProgress;

            // Move into position
            Sprite.SetAnimation("walk");
            MoveToMeleePosition(turn.Target);
            while (MoveState != CombatantMoveState.Idle) { }

            // Animate
            Sprite.SetAnimation("melee").OnAnimationEvent += (sender, trigger) =>
            {
                if (trigger == AnimationEventTrigger.AnimationCompleted)
                {
                    Sprite.SetAnimation("idle");
                }
            };
            while (Sprite.Controller.IsAnimating && Sprite.CurrentAnimation == "melee") { }

            // Deal damage
            var damage = CalculatePhysicalDamage(turn.Target);
            turn.DamageDealt = damage;
            turn.Target.TakeDamage(damage);

            // Move back to base position
            Sprite.SetAnimation("walk");
            MoveBackToBasePosition();
            while (MoveState != CombatantMoveState.Idle) { }
            Sprite.SetAnimation("idle");

            ActionState = CombatantActionState.TurnComplete;
            ActionState = CombatantActionState.Idle;
        }

        public void UseAbility(ref CombatTurn turn)
        {
            if (ActionState != CombatantActionState.Idle)
            {
                return;
            }

            ActionState = CombatantActionState.TurnInProgress;

            // Move into position
            Sprite.SetAnimation("walk");
            MoveToRangedPosition();
            while (MoveState != CombatantMoveState.Idle) { }

            // Animate
            Sprite.SetAnimation("magic").OnAnimationEvent += (sender, trigger) =>
            {
                if (trigger == AnimationEventTrigger.AnimationCompleted)
                {
                    Sprite.SetAnimation("idle");
                }
            };
            while (Sprite.Controller.IsAnimating && Sprite.CurrentAnimation == "magic") { }

            // Deal damage
            var damage = _random.Next(turn.Ability.DamageMin, turn.Ability.DamageMax);
            turn.DamageDealt = damage;
            turn.Target.TakeDamage(damage);

            // Move back to base position
            Sprite.SetAnimation("walk");
            MoveBackToBasePosition();
            while (MoveState != CombatantMoveState.Idle) { }
            Sprite.SetAnimation("idle");
            ActionState = CombatantActionState.TurnComplete;

            ActionState = CombatantActionState.Idle;
        }

        public void UseItem(ref CombatTurn turn)
        {
            if (ActionState != CombatantActionState.Idle)
            {
                return;
            }

            ActionState = CombatantActionState.TurnInProgress;

            // Move into position
            Sprite.SetAnimation("walk");
            MoveToRangedPosition();
            while (MoveState != CombatantMoveState.Idle) { }

            // Animate
            Sprite.SetAnimation("magic").OnAnimationEvent += (sender, trigger) =>
            {
                if (trigger == AnimationEventTrigger.AnimationCompleted)
                {
                    Sprite.SetAnimation("idle");
                }
            };
            while (Sprite.Controller.IsAnimating && Sprite.CurrentAnimation == "magic") { }

            // TODO: Apply item effects
            // ...

            // Move back to base position
            Sprite.SetAnimation("walk");
            MoveBackToBasePosition();
            while (MoveState != CombatantMoveState.Idle) { }
            Sprite.SetAnimation("idle");
            ActionState = CombatantActionState.TurnComplete;

            ActionState = CombatantActionState.Idle;
        }

        #endregion

        private int CalculatePhysicalDamage(Combatant target)
        {
            // TODO: Use equipped weapons stats here...

            var baseDamage = _random.Next(1, 11);
            baseDamage *= Stats.Strength;

            // Factor attacker's buffs/debuffs
            // ...

            // Factor target's defence
            var defence = target.Stats.Defence;

            // Factor target's equipment i.e. Armour
            // ...

            // Factor target's buffs/debuffs
            // ...

            // Factor level difference between two combatants

            var damage = baseDamage - defence;

            return damage;
        }
    }
}