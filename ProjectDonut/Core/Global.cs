using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core.Input;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.Debugging;
using ProjectDonut.Debugging.Console;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.Pathfinding;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.World;
using ProjectDonut.UI.DialogueSystem;
using ProjectDonut.UI.ScrollDisplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Core
{
    public static class Global
    {
        // SCREEN SETTINGS
        public static int ScreenWidth = 1920;
        public static int ScreenHeight = 1080;

        // MAP SETTINGS
        public static int TileSize = 64;
        public static int ChunkSize = 50;

        // MINING
        public static int TEMP_PLAYER_DAMAGE = 10;

        // FOG OF WAR SETTINGS
        public static int INSTANCE_SIGHT_RADIUS = 8;
        public static bool SHOW_FOG_OF_WAR = true;
        public static int FOG_OF_WAR_RADIUS = 2000;

        // DEBUG SETTINGS
        public static Texture2D DEBUG_TEXTURE;
        public static bool DRAW_WORLD_CHUNK_OUTLINE = false;
        public static bool DRAW_STRUCTURE_ENTRY_OUTLINE = true;
        public static bool DRAW_INSTANCE_EXIT_LOCATIONS_OUTLINE = true;

        // DEBUG
        public static DevConsole Console;
        public static DebugWindow DebugWindow;

        public static ContentManager ContentManager;
        public static SpriteBatch SpriteBatch;
        public static GraphicsDevice GraphicsDevice;
        public static GraphicsDeviceManager GraphicsDeviceManager;

        public static Player Player;
        public static class PlayerComponents
        {            
            public static PlayerInventory Inventory;
        }


        public static Camera Camera;
        public static GameCursor GameCursor;
        public static SpriteLibrary SpriteLibrary;
        public static ScrollDisplayer ScrollDisplay;


        
        //public static Astar Pathfinding;

        public static WorldChunkManager WorldChunkManager;

        public static InputManager InputManager;
        public static SceneManager SceneManager;
        public static DialogueManager DialogueManager;
    }
}
