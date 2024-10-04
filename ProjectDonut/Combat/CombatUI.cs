using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGameGum.GueDeriving;
using RenderingLibrary;
using MonoGameGum.Forms;
using ProjectDonut.Core;
using MonoGameGum.Forms.Controls;

namespace ProjectDonut.Combat
{
    public class CombatUI
    {
        private ContainerRuntime Root;
        private CombatManager _manager;

        public CombatUI(CombatManager manager)
        {
            _manager = manager;
        }

        public void Initialize()
        {
            SystemManagers.Default = new SystemManagers();
            SystemManagers.Default.Initialize(Global.GraphicsDevice, fullInstantiation: true);
            FormsUtilities.InitializeDefaults();

            Root = new ContainerRuntime();
            Root.Width = 0;
            Root.Height = 0;
            Root.WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToContainer;
            Root.HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToContainer;
            Root.AddToManagers();

            CreateButtons();
        }

        private void CreateButtons()
        {
            var width = 200;
            var height = 75;
            var spacing = 30;
            var bottom = Global.GraphicsDeviceManager.PreferredBackBufferHeight;

            // Attack
            var btnAttack = new Button();
            Root.Children.Add(btnAttack.Visual);
            btnAttack.X = spacing;
            btnAttack.Y = bottom - (spacing * 2) - (height * 2);
            btnAttack.Width = width;
            btnAttack.Height = height;
            btnAttack.Text = "Attack";
            //int clickCount = 0;
            btnAttack.Click += (_, _) =>
            {
                //clickCount++;
                //btnAttack.Text = $"Clicked {clickCount} times";
            };

            // Item
            var btnItem = new Button();
            Root.Children.Add(btnItem.Visual);
            btnItem.X = (spacing * 2) + width;
            btnItem.Y = bottom - (spacing * 2) - (height * 2);
            btnItem.Width = width;
            btnItem.Height = height;
            btnItem.Text = "Items";
            btnItem.Click += (_, _) =>
            {
                //clickCount++;
                //btnAttack.Text = $"Clicked {clickCount} times";
            };

            // Item
            var btnAbility = new Button();
            Root.Children.Add(btnAbility.Visual);
            btnAbility.X = spacing;
            btnAbility.Y = bottom - spacing - height;
            btnAbility.Width = width;
            btnAbility.Height = height;
            btnAbility.Text = "Abilities";
            btnAbility.Click += (_, _) =>
            {
                //clickCount++;
                //btnAttack.Text = $"Clicked {clickCount} times";
            };

            // Flee
            var btnFlee = new Button();
            Root.Children.Add(btnFlee.Visual);
            btnFlee.X = (spacing * 2) + width;
            btnFlee.Y = bottom - spacing - height;
            btnFlee.Width = width;
            btnFlee.Height = height;
            btnFlee.Text = "Flee";
            btnFlee.Click += (_, _) =>
            {
                //clickCount++;
                //btnAttack.Text = $"Clicked {clickCount} times";
            };
        }

        public void Update(GameTime gameTime)
        {
            FormsUtilities.Update(gameTime, Root);
            SystemManagers.Default.Activity(gameTime.TotalGameTime.TotalSeconds);
        }

        public void Draw(GameTime gameTime)
        {
            //Global.GraphicsDevice.Clear(Color.CornflowerBlue);
            //SystemManagers.Default.Draw();
        }
    }
}
