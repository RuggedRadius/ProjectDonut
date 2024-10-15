using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Graphics;
using ProjectDonut.Core.Input;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.Interfaces;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Core.SceneManagement.SceneTypes;

namespace ProjectDonut.ProceduralGeneration.World.MineableItems
{
    public class MineableTreeWinter : IMineable
    {
        public bool InRangeOfPlayer { get; set; }
        public Rectangle InteractBounds { get; set; }
        public Texture2D InventoryIcon { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public bool IsVisible { get; set; }
        public bool IsExplored { get; set; }
        public Rectangle TextureBounds { get; set; }
        public Texture2D Texture { get; set; }


        private SpriteSheet _spriteSheet;
        private AnimationController _animControllerHit;
        private AnimatedSprite _sprite;

        private Random _random;

        public int ZIndex { get; set; }

        public Vector2 WorldPosition { get; set; }

        public MineableTreeWinter()
        {
            _random = new Random();
        }

        public void Intialize()
        {
            InteractBounds = new Rectangle(
                (int)WorldPosition.X + Global.TileSize / 2,
                (int)WorldPosition.Y + ((Global.TileSize / 2) * 3),
                Global.TileSize,
                Global.TileSize);

            MaxHealth = 1000;
            Health = MaxHealth;
        }

        public void LoadContent()
        {
            InventoryIcon = Global.ContentManager.Load<Texture2D>("Sprites/UI/Items/wood-log-01");

            var sheetTexture = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Tree2-winter");
            var atlas = Texture2DAtlas.Create("tree", sheetTexture, 128, 128);
            _spriteSheet = new SpriteSheet("SpriteSheet/tree", atlas);

            _spriteSheet.DefineAnimation("idle", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(1));
            });

            var cellTime = 0.25f;

            _spriteSheet.DefineAnimation("hit", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(1, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(2, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(3, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(2, duration: TimeSpan.FromSeconds(cellTime));
            });

            _sprite = new AnimatedSprite(_spriteSheet, "idle");
            _sprite.SetAnimation("hit").OnAnimationEvent += (sender, trigger) =>
            {
                if (trigger == AnimationEventTrigger.AnimationCompleted)
                {
                    //Debug.WriteLine("Animation completed");
                    _sprite.SetAnimation("idle");
                }
            };
        }

        public void Update(GameTime gameTime)
        {
            _sprite.Update(gameTime);

            UpdateObjectVisibility();

            InRangeOfPlayer = InteractBounds.Intersects(Global.PlayerObj.InteractBounds);

            if (!InRangeOfPlayer)
                return;

            if (PlayerObj.CurrentInteractedObject != null)
                return;

            if (InputManager.KeyboardState.IsKeyDown(Keys.E))
            {
                PlayerObj.CurrentInteractedObject = this;
                HandleAction();
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (!IsExplored)
            {
                return;
            }
            else if (!IsVisible)
            {
                _sprite.Color = Color.Gray;
                Global.SpriteBatch.Draw(_sprite, WorldPosition, 0.0f, Vector2.One);
            }
            else
            {
                _sprite.Color = Color.White;
                Global.SpriteBatch.Draw(_sprite, WorldPosition, 0.0f, Vector2.One);
                //Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, InteractBounds, Color.Blue * 0.1f);
            }
        }

        public void UpdateObjectVisibility()
        {
            if (Global.SHOW_FOG_OF_WAR == false)
            {
                IsVisible = true;
                IsExplored = true;
                return;
            }

            float distance = Math.Abs(Vector2.Distance(Global.PlayerObj.WorldPosition, WorldPosition));
            IsVisible = distance <= Global.FOG_OF_WAR_RADIUS ? true : false;

            if (IsVisible && !IsExplored)
                IsExplored = true;
        }

        private void HandleAction()
        {
            _sprite.SetAnimation("hit");

            Health -= Global.TEMP_PLAYER_DAMAGE;

            if (Health <= 0)
            {
                var mineableItem = CreateInventoryItem();
                Global.Player.Inventory.AddItemToInventory(mineableItem);
                Global.Player.TextDisplay.AddText("+1 " + mineableItem.Name, 0, Vector2.Zero, Color.White);

                CreateReplacementTreeStump();
            }
        }

        private void CreateReplacementTreeStump()
        {
            var replacementTree = new SceneObjectStatic()
            {
                WorldPosition = WorldPosition,
                Texture = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Tree-stump-export"),
                IsVisible = IsVisible,
                IsExplored = IsExplored
            };
            replacementTree.Initialize();

            //Global.SceneManager.CurrentScene.AddSceneObject(replacementTree); TODO: I WISH
            if (Global.SceneManager.CurrentScene is WorldScene worldscene)
            {
                if (worldscene._sceneObjects.ContainsKey("tree-stump"))
                {
                    worldscene._sceneObjects["tree-stump"].Add(replacementTree);
                }
                else
                {
                    worldscene._sceneObjects.Add("tree-stump", new List<ISceneObject> { replacementTree });
                }
            }

        }

        private InventoryItem CreateInventoryItem()
        {
            var mineableItem = new InventoryItem()
            {
                Name = "Wood Log",
                Icon = InventoryIcon,
                ItemType = ItemType.Consumable,
                Quantity = _random.Next(1, 4)
            };

            return mineableItem;
        }
    }
}
