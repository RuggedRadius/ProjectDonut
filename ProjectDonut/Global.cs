using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core.Input;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.GameObjects;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.UI.DialogueSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut
{
    public static class Global
    {
        public static ContentManager ContentManager;
        public static SpriteBatch SpriteBatch;
        public static GraphicsDevice GraphicsDevice;
        public static GraphicsDeviceManager GraphicsDeviceManager;

        public static Player Player;
        public static Camera Camera;
        public static GameCursor GameCursor;
        public static SpriteLibrary SpriteLibrary;

        public static InputManager InputManager;
        public static SceneManager SceneManager;
        public static DialogueManager DialogueManager;
    }
}
