﻿using Microsoft.Xna.Framework;
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

namespace ProjectDonut.ProceduralGeneration.World
{
    public enum MineableObjectType
    {
        Tree,
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
        public Vector2 Position { get; set; }

        public bool InRangeOfPlayer { get; set; }
        public Rectangle InteractBounds { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }

        public MineableObjectAnimationState MineableObjectAnimationState { get; set; }
        public MineableObjectType MineableObjectType { get; set; }

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
                (int)Position.X + (Global.TileSize / 2),// - Global.TileSize, 
                (int)Position.Y + (Global.TileSize),// - Global.TileSize,
                Global.TileSize,// + (2 * Global.TileSize),
                Global.TileSize);// + (2 * Global.TileSize));

            MaxHealth = 1000;
            Health = MaxHealth;


        }

        public void LoadContent()
        {
            switch (MineableObjectType) 
            {
                case MineableObjectType.Tree:
                    //Texture = Global.ContentManager.Load<Texture2D>("Sprites/World/Objects/tree");
                    InventoryIcon = Global.ContentManager.Load<Texture2D>("Sprites/UI/Items/wood-log-01");


                    var sheetTexture = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Tree2-Sheet-export");
                    var atlas = Texture2DAtlas.Create("tree", sheetTexture, 128, 128);
                    _spriteSheet = new SpriteSheet("SpriteSheet/tree", atlas);

                    _spriteSheet.DefineAnimation("idle", builder =>
                    {
                        builder.IsLooping(false)
                            .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(1));
                    });

                    _spriteSheet.DefineAnimation("hit", builder =>
                    {
                        builder.IsLooping(false)
                            .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(0.1))
                            .AddFrame(1, duration: TimeSpan.FromSeconds(0.1))
                            .AddFrame(2, duration: TimeSpan.FromSeconds(0.1))
                            .AddFrame(3, duration: TimeSpan.FromSeconds(0.1));
                    });

                    _tree = new AnimatedSprite(_spriteSheet, "idle");
                    _tree.SetAnimation("hit").OnAnimationEvent += (sender, trigger) =>
                    {
                        if (trigger == AnimationEventTrigger.AnimationCompleted)
                        {
                            Debug.WriteLine("Animation completed");
                            _tree.SetAnimation("idle");
                        }
                    };

                    break;

                case MineableObjectType.Rock:
                    break;

                case MineableObjectType.Ore:
                    break;

                case MineableObjectType.Bush:
                    break;
            }
        }

        public void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.End();
            Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix(), samplerState: SamplerState.PointClamp);


            Global.SpriteBatch.Draw(_tree, Position, 0.0f, Vector2.One);
            //Global.SpriteBatch.Draw(Texture, Position, Color.White);
            Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, InteractBounds, Color.Blue * 0.1f);


            Global.SpriteBatch.End();
            Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
        }

        public void Update(GameTime gameTime)
        {
            _tree.Update(gameTime);
            //_animControllerHit.Update(gameTime);
            InRangeOfPlayer = InteractBounds.Intersects(Global.Player.InteractBounds);

            if (!InRangeOfPlayer) 
                return;

            if (InputManager.KeyboardState.IsKeyDown(Keys.E))
            {
                //if (_tree.CurrentAnimation.)

                MineableObjectAnimationState = MineableObjectAnimationState.Hit;
                _tree.SetAnimation("hit");
                Health -= Global.TEMP_PLAYER_DAMAGE;

                if (Health <= 0)
                {
                    var item = new InventoryItem
                    {
                        Name = "Wood Log",
                        Icon = InventoryIcon,
                        ItemType = ItemType.Consumable
                    };

                    Player.Inventory.AddItemToInventory(item);
                    var treeCount = Global.WorldChunkManager.PlayerChunk.MineableObjects["trees"].Count;
                    //Global.WorldChunkManager.PlayerChunk.MineableObjects["trees"].Remove(this);
                    treeCount = Global.WorldChunkManager.PlayerChunk.MineableObjects["trees"].Count;
                }
            }
        }
    }
}
