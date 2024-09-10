using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Core.Input;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.Interfaces;

namespace ProjectDonut.ProceduralGeneration.World.MineableItems
{
    public class MineableRock : IMineable
    {
        public int ZIndex { get; set; }
        public Texture2D Texture { get; set; }
        public Texture2D InventoryIcon { get; set; }
        public Vector2 WorldPosition { get; set; }

        public bool IsVisible { get; set; }
        public bool IsExplored { get; set; }
        public bool InRangeOfPlayer { get; set; }
        public Rectangle InteractBounds { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public Rectangle TextureBounds { get; set; }

        private SpriteSheet _spriteSheetRock;
        private AnimationController _animControllerHit;
        private AnimatedSprite _sprite;


        public void Intialize()
        {
            InteractBounds = new Rectangle(
                (int)WorldPosition.X,
                //(int)WorldPosition.Y + ((Global.TileSize / 2) * 3),
                (int)WorldPosition.Y,
                Global.TileSize,
                Global.TileSize);

            MaxHealth = 1000;
            Health = MaxHealth;
        }

        public void LoadContent()
        {
            InventoryIcon = Global.SpriteLibrary.ItemsSprites["rock"];

            var sheetTexture = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Rock01");
            var atlas = Texture2DAtlas.Create("rock", sheetTexture, 64, 64);
            _spriteSheetRock = new SpriteSheet("SpriteSheet/rock", atlas);

            _spriteSheetRock.DefineAnimation("idle", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(1));
            });

            var cellTime = 0.25f;

            _spriteSheetRock.DefineAnimation("hit", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(1, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(2, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(3, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(0, duration: TimeSpan.FromSeconds(cellTime));
            });

            _sprite = new AnimatedSprite(_spriteSheetRock, "idle");
            _sprite.SetAnimation("hit").OnAnimationEvent += (sender, trigger) =>
            {
                if (trigger == AnimationEventTrigger.AnimationCompleted)
                {
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
            //Global.SpriteBatch.End();
            //Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix(), samplerState: SamplerState.PointClamp);

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
                Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, InteractBounds, Color.Blue * 0.1f);
            }

            //Global.SpriteBatch.End();
            //Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
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

                var replacementRockRubble = new SceneObjectStatic()
                {
                    WorldPosition = WorldPosition,
                    Texture = Global.SpriteLibrary.WorldMapSprites["rock-smashed"][0],
                    IsVisible = IsVisible,
                    IsExplored = IsExplored
                };
                replacementRockRubble.Initialize();

                //Global.SceneManager.CurrentScene.AddSceneObject(replacementTree); TODO: I WISH
                if (Global.SceneManager.CurrentScene._sceneObjects.ContainsKey("rock-rubble"))
                {
                    Global.SceneManager.CurrentScene._sceneObjects["rock-rubble"].Add(replacementRockRubble);
                }
                else
                {
                    Global.SceneManager.CurrentScene._sceneObjects.Add("rock-rubble", new List<ISceneObject> { replacementRockRubble });
                }
            }
        }

        private InventoryItem CreateInventoryItem()
        {
            var mineableItem = new InventoryItem();

            mineableItem.Name = "Stone";
            mineableItem.Icon = InventoryIcon;
            mineableItem.ItemType = ItemType.Consumable;

            return mineableItem;
        }

    }
}
