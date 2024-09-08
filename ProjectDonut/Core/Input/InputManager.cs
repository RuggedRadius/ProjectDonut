using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using ProjectDonut.Interfaces;

namespace ProjectDonut.Core.Input
{
    public class InputManager : Interfaces.IGameComponent
    {
        public static InputManager Instance;

        public static KeyboardState KeyboardState;
        public static MouseState MouseState;

        public InputManager()
        {
            if (Instance == null)
            {
                Instance = this;
            }            
        }

        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }

        public void Initialize()
        {
            
        }

        public void LoadContent()
        {            
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();
        }

        public void Draw(GameTime gameTime)
        {
        }

        //private void HandleDebugInput()
        //{
        //    if (KeyboardState.IsKeyDown(Keys.F8))
        //    {
        //        Global.SceneManager.SetCurrentScene(Global.SceneManager.Scenes["world"]);
        //        Global.SceneManager.CurrentScene.PrepareForPlayerEntry();
        //    }

        //    if (KeyboardState.IsKeyDown(Keys.F9))
        //    {
        //        var worldScene = (WorldScene)Global.SceneManager.CurrentScene;
        //        worldScene.LastExitLocation = new Rectangle((int)Global.Player.WorldPosition.X, (int)Global.Player.WorldPosition.Y, Global.TileSize, Global.TileSize);
        //        Global.SceneManager.SetCurrentScene(Global.SceneManager.Scenes["instance"]);
        //        Global.SceneManager.CurrentScene.PrepareForPlayerEntry();
        //    }
        //}
    }
}
