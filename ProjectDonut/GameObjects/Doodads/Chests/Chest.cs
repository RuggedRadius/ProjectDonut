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

namespace ProjectDonut.GameObjects.Doodads.Chests
{
    public class Chest : InteractableDoodad
    {
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

            _sprite = new AnimatedSprite(_spriteSheet, "idle");
            _sprite.SetAnimation("open").OnAnimationEvent += (sender, trigger) =>
            {
                if (trigger == AnimationEventTrigger.AnimationCompleted)
                {
                    State = InteractableState.Interacted;
                }
            };

            _sprite.SetAnimation("idle");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.IsKeyPressed(Keys.E))
            {
                if (InteractBounds.Contains(Global.PlayerObj.WorldPosition) && State == InteractableState.Idle)
                {
                    Interact();
                }
            }

            _sprite.Update(gameTime);
        }

        public override void Interact()
        {
            if (State == InteractableState.Interacted)
                return;

            base.Interact();

            //PlayerObj.CurrentInteractedObject = this; // TODO: Change Player to store IInteractbales instead of IMineables
            State = InteractableState.Interacting;
            _sprite.SetAnimation("open");

            // TEMP
            // TODO: SHOW CHEST INVENTORY SCREEN
            for (int i = 0; i < ContainedItems.Count; i++)
            {
                Global.Player.Inventory.AddItemToInventory(ContainedItems[i]);
            }            
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
