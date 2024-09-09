using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Core.Input;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.Interfaces;
using System;
using MonoGame.Extended.Animations;
using System.Diagnostics.Tracing;
using System.Diagnostics;
using System.Collections.Generic;

namespace ProjectDonut.ProceduralGeneration.World
{
    public enum MineableObjectType
    {
        Tree,
        TreeWinter,
        Rock,
        Ore,
        Bush
    }

    public enum MineableObjectAnimationState
    {
        Idle,
        Hit,
        Destroyed
    }

    public class MineableObject : IMineable
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

        public MineableObjectType MineableObjectType { get; set; }
        public Rectangle TextureBounds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private SpriteSheet _spriteSheet;
        private AnimationController _animControllerHit;
        private AnimatedSprite _tree;

        public MineableObject(MineableObjectType objectType)
        {
            this.MineableObjectType = objectType;
        }

        public void Intialize()
        {
            InteractBounds = new Rectangle(
                (int)WorldPosition.X + (Global.TileSize / 2),
                (int)WorldPosition.Y + ((Global.TileSize / 2) * 3),
                Global.TileSize,
                Global.TileSize);

            MaxHealth = 1000;
            Health = MaxHealth;
        }

        public void LoadContent()
        {
            switch (MineableObjectType) 
            {
                case MineableObjectType.Tree:
                    InitialiseTreeAnimations();
                    break;

                case MineableObjectType.TreeWinter:
                    InitialiseWinterTreeAnimations();
                    break;

                case MineableObjectType.Rock:
                    break;

                case MineableObjectType.Ore:
                    break;

                case MineableObjectType.Bush:
                    break;
            }
        }

        private void InitialiseTreeAnimations()
        {
            InventoryIcon = Global.ContentManager.Load<Texture2D>("Sprites/UI/Items/wood-log-01");

            var sheetTexture = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Tree2-Sheet-export");
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

            _tree = new AnimatedSprite(_spriteSheet, "idle");
            _tree.SetAnimation("hit").OnAnimationEvent += (sender, trigger) =>
            {
                if (trigger == AnimationEventTrigger.AnimationCompleted)
                {
                    //Debug.WriteLine("Animation completed");
                    _tree.SetAnimation("idle");
                }
            };
        }

        private void InitialiseWinterTreeAnimations()
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

            _tree = new AnimatedSprite(_spriteSheet, "idle");
            _tree.SetAnimation("hit").OnAnimationEvent += (sender, trigger) =>
            {
                if (trigger == AnimationEventTrigger.AnimationCompleted)
                {
                    //Debug.WriteLine("Animation completed");
                    _tree.SetAnimation("idle");
                }
            };
        }



        public void Update(GameTime gameTime)
        {
            _tree.Update(gameTime);

            UpdateObjectVisibility();

            InRangeOfPlayer = InteractBounds.Intersects(Global.PlayerObj.InteractBounds);

            if (!InRangeOfPlayer) 
                return;

            if (InputManager.KeyboardState.IsKeyDown(Keys.E))
            {
                HandleAction();
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
            IsVisible = (distance <= Global.FOG_OF_WAR_RADIUS) ? true : false;

            if (IsVisible && !IsExplored)
                IsExplored = true;
        }

        private void HandleAction()
        {
            //MineableObjectAnimationState = MineableObjectAnimationState.Hit;

            _tree.SetAnimation("hit");

            Health -= Global.TEMP_PLAYER_DAMAGE;

            if (Health <= 0)
            {
                var mineableItem = new InventoryItem();
                switch (MineableObjectType)
                {
                    case MineableObjectType.Tree:
                        mineableItem.Name = "Wood Log";
                        mineableItem.Icon = InventoryIcon;
                        mineableItem.ItemType = ItemType.Consumable;
                        break;

                    case MineableObjectType.Rock:
                        mineableItem.Name = "Stone";
                        mineableItem.Icon = InventoryIcon;
                        mineableItem.ItemType = ItemType.Consumable;
                        break;

                    case MineableObjectType.Ore:
                        mineableItem.Name = "Ore";
                        mineableItem.Icon = InventoryIcon;
                        mineableItem.ItemType = ItemType.Consumable;
                        break;

                    case MineableObjectType.Bush:
                        mineableItem.Name = "Berry";
                        mineableItem.Icon = InventoryIcon;
                        mineableItem.ItemType = ItemType.Consumable;
                        break;
                }                

                Global.Player.Inventory.AddItemToInventory(mineableItem);
                Global.Player.TextDisplay.AddText("+1 " + mineableItem.Name, 2, 10, 0, true, Color.Green);

                switch (MineableObjectType)
                {
                    case MineableObjectType.Tree:
                        var replacementTree = new SceneObjectStatic()
                        {
                            WorldPosition = WorldPosition,
                            Texture = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Tree-stump-export"),
                            IsVisible = IsVisible,
                            IsExplored = IsExplored
                        };
                        replacementTree.Initialize();

                        //Global.SceneManager.CurrentScene.AddSceneObject(replacementTree); TODO: I WISH
                        if (Global.SceneManager.CurrentScene._sceneObjects.ContainsKey("tree-stump"))
                        {
                            Global.SceneManager.CurrentScene._sceneObjects["tree-stump"].Add(replacementTree);
                        }
                        else
                        {
                            Global.SceneManager.CurrentScene._sceneObjects.Add("tree-stump", new List<ISceneObject> { replacementTree });
                        }                        
                        break;

                    case MineableObjectType.Rock:
                        break;

                    case MineableObjectType.Ore:
                        break;

                    case MineableObjectType.Bush:
                        break;
                }
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
                _tree.Color = Color.Gray;
                Global.SpriteBatch.Draw(_tree, WorldPosition, 0.0f, Vector2.One);
            }
            else
            {
                _tree.Color = Color.White;
                Global.SpriteBatch.Draw(_tree, WorldPosition, 0.0f, Vector2.One);
                Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, InteractBounds, Color.Blue * 0.1f);
            }

            //Global.SpriteBatch.End();
            //Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
        }
    }
}
