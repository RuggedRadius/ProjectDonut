using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Graphics;
using ProjectDonut.Core.Input;
using ProjectDonut.Core.Sprites;
using ProjectDonut.GameObjects.PlayerComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.GameObjects.Doodads.Barrels
{
    public class Barrel : InteractableDoodad
    {
        private List<InventoryItem> ContainedItems { get; set; }

        public Barrel(Rectangle bounds, List<InventoryItem> containedItems) : base(bounds)
        {
            ContainedItems = containedItems;
            InteractBounds = new Rectangle(
                (int)WorldPosition.X - Global.TileSize,
                (int)WorldPosition.Y - Global.TileSize,
                Global.TileSize * 3,
                Global.TileSize * 3);

            Texture = SpriteLib.Doodads.Barrels["barrel-01"];

            InitialiseAnimation();

            State = InteractableState.Acessible;
        }

        private void InitialiseAnimation()
        {
            var cellTime = 0.1f;
            var sheetTexture = Global.ContentManager.Load<Texture2D>("Sprites/Doodads/Barrels/Barrel01");
            var atlas = Texture2DAtlas.Create("barrel", sheetTexture, Global.TileSize, Global.TileSize);
            _spriteSheet = new SpriteSheet("SpriteSheet/barrel", atlas);

            _spriteSheet.DefineAnimation("idle", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(1));
            });

            _spriteSheet.DefineAnimation("explode", builder =>
            {
                builder.IsLooping(false)
                    .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(1, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(2, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(3, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(4, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(5, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(6, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(7, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(8, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(9, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(10, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(11, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(12, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(13, duration: TimeSpan.FromSeconds(cellTime))
                    .AddFrame(14, duration: TimeSpan.FromSeconds(cellTime));
            });

            _sprite = new AnimatedSprite(_spriteSheet, "idle");
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

            _sprite.Update(gameTime);
        }

        public override void Interact()
        {
            base.Interact();

            if (State == InteractableState.Acessible)
            {
                State = InteractableState.Interacted;
                _sprite.SetAnimation("explode").OnAnimationEvent += (sender, trigger) =>
                {
                    if (trigger == AnimationEventTrigger.AnimationCompleted)
                    {
                        State = InteractableState.Interacted;
                    }
                };

                // TEMP
                // TODO: SHOW CHEST INVENTORY SCREEN
                // WARNING: ITEMS NOT PICKED UP WILL CURRENTLY BE LOST
                for (int i = 0; i < ContainedItems.Count; i++)
                {
                    var unplacedItems = Global.Player.Inventory.AddItemToInventory(ContainedItems[i]);
                    ContainedItems[i].Quantity = unplacedItems;
                }
            }
        }
    }
}
