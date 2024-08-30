using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.GameObjects;
using ProjectDonut.GameObjects.PlayerComponents;
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

        public static SceneManager SceneManager;
    }
}
