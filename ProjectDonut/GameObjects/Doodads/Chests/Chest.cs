using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Core.Input;
using ProjectDonut.Core.Sprites;
using ProjectDonut.GameObjects.PlayerComponents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectDonut.GameObjects.Doodads.Chests
{
    //public enum ChestState
    //{
    //    Closed,
    //    Open,
    //}

    public class Chest : InteractableDoodad
    {
        public bool IsLocked { get; set; }
        public bool IsOpen { get; set; }
        private List<InventoryItem> ContainedItems { get; set; }

        public Chest(Rectangle bounds, List<InventoryItem> containedItems) : base(bounds)
        {
            ContainedItems = containedItems;
            InteractBounds = new Rectangle(
                (int)WorldPosition.X - Global.TileSize,
                (int)WorldPosition.Y - Global.TileSize,
                Global.TileSize * 3,
                Global.TileSize * 3);

            Texture = SpriteLib.Doodads.Chests["chest-01"];

            InitialiseAnimation();

            State = InteractableState.Acessible;
        }

        private void InitialiseAnimation()
        {
            var cellTime = 0.1f;
            var sheetTexture = Global.ContentManager.Load<Texture2D>("Sprites/Doodads/Chests/Chest01");
            var atlas = Texture2DAtlas.Create("chest", sheetTexture, Global.TileSize, Global.TileSize);
            _spriteSheet = new SpriteSheet("SpriteSheet/chest", atlas);

            _spriteSheet.DefineAnimation("idle", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(1));
            });

            _spriteSheet.DefineAnimation("open", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(1, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(2, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(3, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(4, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(5, duration: TimeSpan.FromSeconds(cellTime));
            });

            _spriteSheet.DefineAnimation("close", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(regionIndex: 5, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(4, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(3, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(2, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(1, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(0, duration: TimeSpan.FromSeconds(cellTime));
            });

            _sprite = new AnimatedSprite(_spriteSheet, "idle");
            _sprite.SetAnimation("open").OnAnimationEvent += (sender, trigger) =>
            {
                //if (trigger == AnimationEventTrigger.AnimationCompleted)
                //{
                //    State = InteractableState.Interacted;
                //}
            };

            _sprite.SetAnimation("idle");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (State == InteractableState.Disabled)
                return;

            if (PlayerInRange)
            {
                if (State == InteractableState.Acessible && 
                    InputManager.IsKeyPressed(Keys.E) &&
                    _interactTimer >= _interactCooldown)
                {
                    Interact();
                }
            }
            else
            {
                if (IsOpen)
                {
                    _sprite.SetAnimation("close");
                    IsOpen = false;
                }
            }

            _sprite.Update(gameTime);
        }

        public override void Interact()
        {
            if (State == InteractableState.Interacted || 
                State == InteractableState.Inacessible || 
                State == InteractableState.Disabled)
                return;

            base.Interact();

            IsOpen = true;

            //PlayerObj.CurrentInteractedObject = this; // TODO: Change Player to store IInteractbales instead of IMineables
            
            _sprite.SetAnimation("open");

            // TEMP
            // TODO: SHOW CHEST INVENTORY SCREEN
            for (int i = 0; i < ContainedItems.Count; i++)
            {
                var unplacedItems = Global.Player.Inventory.AddItemToInventory(ContainedItems[i]);
                ContainedItems[i].Quantity = unplacedItems;
            }       
            
            ContainedItems.Where(x => x.Quantity == 0).ToList().ForEach(x => ContainedItems.Remove(x));
        }

        public override void Draw(GameTime gameTime)
        {
            if (State == InteractableState.Disabled)
                return;

            base.Draw(gameTime);
        }
    }
}
