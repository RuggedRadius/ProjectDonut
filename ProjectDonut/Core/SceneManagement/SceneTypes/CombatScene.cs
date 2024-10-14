using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using ProjectDonut.Combat;
using ProjectDonut.Combat.UI;
using ProjectDonut.Core.Input;

namespace ProjectDonut.Core.SceneManagement.SceneTypes
{
    public class CombatScene : IScene
    {
        public static CombatScene Instance { get; private set; }

        public SceneType SceneType { get; set; }
        public Vector2 Position { get; set; }

        //private CombatUI _combatUI;
        public CombatManager Manager;
        private CombatUITurnOrder _turnOrder;

        public CombatUIOptions OptionsUI;        
        public CombatUILog LogUI;
        public CombatUILogWriter LogWriter;
        public CombatUIAbility AbilityUI;
        public CombatUIItem ItemUI;
        public CombatUIStrategyActions CombatActionsUI;
        public CombatUITargetPicker TargetPickerUI;
        public List<ITargetableCombatUI> TargetUIHistory;
        public ITargetableCombatUI CurrentTargetUI;

        private Texture2D _background;

        public List<CombatItem> PlayerItems;

        public AnimatedSprite Background { get; set; }
        private SpriteSheet _spriteSheet;

        public static int SceneScale = 8;

        private IScene PreviousScene { get; set; }


        public CombatScene(IScene previousScene)
        {
            Instance = this;

            SceneType = SceneType.Combat;

            Manager = new CombatManager();
            _turnOrder = new CombatUITurnOrder(Manager);

            LogUI = new CombatUILog();
            LogWriter = new CombatUILogWriter();
            AbilityUI = new CombatUIAbility(Manager);
            ItemUI = new CombatUIItem();
            TargetPickerUI = new CombatUITargetPicker();
            OptionsUI = new CombatUIOptions();
            CombatActionsUI = new CombatUIStrategyActions();

            TargetUIHistory = new List<ITargetableCombatUI>();
            CurrentTargetUI = OptionsUI;

            PreviousScene = previousScene;



            TESTPopulatePlayerItems();
        }

        private void TESTPopulatePlayerItems()
        {
            PlayerItems = new List<CombatItem>();

            PlayerItems.Add(new CombatItem()
            {
                Name = "Health Potion",
                Description = "Heals 50 HP",
                Quantity = 5
            });
            PlayerItems.Add(new CombatItem()
            {
                Name = "Mana Potion",
                Description = "Restores 50 MP",
                Quantity = 5
            });
            PlayerItems.Add(new CombatItem()
            {
                Name = "Energy Potion",
                Description = "Restores 50 EP",
                Quantity = 5
            });
        }

        public void Initialize()
        {
            //_combatUI.Initialize();

            var sheetTexture = Global.ContentManager.Load<Texture2D>("Sprites/Combat/bg-dungeon");
            var atlas = Texture2DAtlas.Create("combatant", sheetTexture, 480, 270);
            _spriteSheet = new SpriteSheet("SpriteSheet/combatant", atlas);

            var frameTime = 0.1f;

            _spriteSheet.DefineAnimation("idle", builder =>
            {
                builder.IsLooping(true)
                    .AddFrame(regionIndex: 0, duration: TimeSpan.FromSeconds(frameTime))
                    .AddFrame(regionIndex: 1, duration: TimeSpan.FromSeconds(frameTime))
                    .AddFrame(regionIndex: 2, duration: TimeSpan.FromSeconds(frameTime))
                    .AddFrame(regionIndex: 3, duration: TimeSpan.FromSeconds(frameTime))
                    .AddFrame(regionIndex: 4, duration: TimeSpan.FromSeconds(frameTime))
                    .AddFrame(regionIndex: 5, duration: TimeSpan.FromSeconds(frameTime))
                    .AddFrame(regionIndex: 6, duration: TimeSpan.FromSeconds(frameTime))
                    .AddFrame(regionIndex: 7, duration: TimeSpan.FromSeconds(frameTime));
            });

            Background = new AnimatedSprite(_spriteSheet, "idle");
            Background.SetAnimation("idle");
        }

        public void ChangeTargetUI(ITargetableCombatUI newTarget)
        {
            TargetUIHistory.Add(CurrentTargetUI);
            CurrentTargetUI = newTarget;
            CurrentTargetUI.IsShown = true;

            if (CurrentTargetUI is CombatUITargetPicker)
            {
                var targetPicker = CurrentTargetUI as CombatUITargetPicker;
                targetPicker.DecrementIndex(targetPicker.TargetTeam);
                targetPicker.IncrementIndex(targetPicker.TargetTeam);     
                
                targetPicker.IsFirstFrame = true;
            }

            if (CurrentTargetUI is CombatUIAbility)
            {
                var abilityUI = CurrentTargetUI as CombatUIAbility;
                abilityUI.IsFirstFrame = true;
            }

            if (CurrentTargetUI is CombatUIItem)
            {
                var itemUI = CurrentTargetUI as CombatUIItem;
                itemUI.IsFirstFrame = true;
            }

            if (CurrentTargetUI is CombatUIStrategyActions)
            {
                var combatActionsUI = CurrentTargetUI as CombatUIStrategyActions;
                combatActionsUI.IsFirstFrame = true;
            }
        }

        public void ReturnToPreviousTargetUI()
        {
            CurrentTargetUI = TargetUIHistory[0];
            TargetUIHistory.RemoveAt(0);
        }

        public void LoadContent()
        {
            //_background = Global.ContentManager.Load<Texture2D>("Sprites/Combat/Backgrounds/bg-dungeon");
        }

        public void Update(GameTime gameTime)
        {
            Manager.Update(gameTime);
            //_combatUI.Update(gameTime);
            //_terrain.Update(gameTime);

            Background.Update(gameTime);

            OptionsUI.Update(gameTime);
            _turnOrder.Update(gameTime);
            AbilityUI.Update(gameTime);
            ItemUI.Update(gameTime);
            TargetPickerUI.Update(gameTime);
            CombatActionsUI.Update(gameTime);

            if (Manager.PlayerTeam.Where(x => !x.IsKOd).Count() == 0)
            {
                // Game over
                //...

            }
            else if (Manager.EnemyTeam.Where(x => !x.IsKOd).Count() == 0)
            {
                // Victory
                
                // Display victory screen
                // EXP, Items, etc..

                Global.SceneManager.SetCurrentScene(PreviousScene);
            }
        }

        public void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Draw background
            Global.SpriteBatch.Draw(Background, Vector2.Zero, 0.0f, Vector2.One * (4 )); // TEMP TODO: 4 should be combat.SceneScale here

            // Draw player team
            foreach (var combatant in Manager.PlayerTeam)
            {
                combatant.Draw(gameTime);
                //combatant.DrawBounds();
            }

            // Draw enemy team
            foreach (var combatant in Manager.EnemyTeam)
            {
                combatant.Draw(gameTime);
                //combatant.DrawBounds();
            }

            // Draw combat UI options
            _turnOrder.Draw(gameTime);
                       
            LogUI.Draw(gameTime);
            AbilityUI.Draw(gameTime);
            ItemUI.Draw(gameTime);
            TargetPickerUI.Draw(gameTime);
            CombatActionsUI.Draw(gameTime);
            OptionsUI.Draw(gameTime);

            Global.SpriteBatch.End();
        }

        public void PrepareForPlayerEntry()
        {
            
        }

        public void PrepareForPlayerExit()
        {
            
        }
    }
}
