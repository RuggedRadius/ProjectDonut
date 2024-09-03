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
using ProjectDonut.Interfaces;

namespace ProjectDonut.Core.Input
{
    public class InputManager :IGameObject
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

        public void LoadContent(ContentManager content)
        {            
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();


        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        private void HandleDebugInput()
        {
            if (KeyboardState.IsKeyDown(Keys.F8))
            {
                Global.SceneManager.SetCurrentScene(Global.SceneManager.Scenes["world"], SceneType.World);
                Global.SceneManager.CurrentScene.PrepareForPlayerEntry();
            }

            if (KeyboardState.IsKeyDown(Keys.F9))
            {
                var worldScene = (WorldScene)Global.SceneManager.CurrentScene;
                worldScene.LastExitLocation = new Rectangle((int)Global.Player.Position.X, (int)Global.Player.Position.Y, Global.TileSize, Global.TileSize);
                Global.SceneManager.SetCurrentScene(Global.SceneManager.Scenes["instance"], SceneType.Instance);
                Global.SceneManager.CurrentScene.PrepareForPlayerEntry();
            }
        }
    }
}
