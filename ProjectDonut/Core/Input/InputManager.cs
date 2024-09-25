using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static KeyboardState LastKeyboardState;

        public static MouseState MouseState;
        public static MouseState LastMouseState;

        public static int ScrollWheelDelta;

        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }

        public InputManager()
        {
            if (Instance == null)
            {
                Instance = this;
            }            
        }

        public void Initialize()
        {
            KeyboardState = Keyboard.GetState();
            LastKeyboardState = KeyboardState;
            MouseState = Mouse.GetState();
            LastMouseState = MouseState;
        }

        public void LoadContent()
        {            
        }

        public void Update(GameTime gameTime)
        {
            // Store the previous states
            LastKeyboardState = KeyboardState;
            LastMouseState = MouseState;

            // Update the current states
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();

            // Calculate scroll delta
            ScrollWheelDelta = MouseState.ScrollWheelValue - LastMouseState.ScrollWheelValue;
        }

        public void Draw(GameTime gameTime)
        {
        }

        public static bool IsKeyPressed(Keys key)
        {
            var lastFrame = LastKeyboardState.IsKeyDown(key);
            var thisFrame = KeyboardState.IsKeyDown(key);

            return !lastFrame && thisFrame;
        }
    }
}
