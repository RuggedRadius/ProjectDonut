﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
using ProjectDonut.Core;
using ProjectDonut.Core.Input;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.Core.Sprites;
using ProjectDonut.Debugging;
using ProjectDonut.Environment;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.ProceduralGeneration.World;
using ProjectDonut.UI.DialogueSystem;
using ProjectDonut.UI.ScrollDisplay;
using QuakeConsole;

namespace ProjectDonut
{
    public static class Global
    {
        public static class TownSettings
        {
            public static Vector2 TOWN_SIZE = new Vector2(200, 100);
            public static Vector2 MIN_PLOT_SIZE = new Vector2(60, 40);
            public static Vector2 MIN_BUILDING_SIZE = new Vector2(25, 15);
            public static Vector2 MIN_ROOM_SIZE = new Vector2(15, 10);
        }

        public static class DungeonSettings
        {
            public static Vector2 MIN_ROOM_SIZE = new Vector2(20, 20);
        }

        public static class WorldSettings
        {
            public static bool BUILD_TOWNS = true;
        }


        // DAY/NIGHT
        public static DayNightCycle DayNightCycle;
        public static float timeOfDay = 0f; // Start at midnight
        //public static float timeSpeed = 1f / 60f;
        public static float timeSpeed = 1f;

        // LIGHTING
        public static PenumbraComponent Penumbra;

        // SCREEN SETTINGS
        public static int ScreenWidth = 1920;
        public static int ScreenHeight = 1080;

        // MAP SETTINGS
        public static int TileSize = 32;
        public static int ChunkSize = 100;

        // MINING
        public static int TEMP_PLAYER_DAMAGE = 10;

        // FOG OF WAR SETTINGS
        public static int INSTANCE_SIGHT_RADIUS = 8;
        public static bool SHOW_FOG_OF_WAR = false;
        public static int FOG_OF_WAR_RADIUS = 1500;

        // DEBUG SETTINGS
        public static bool SHOW_GRID_OUTLINE = true;
        public static bool LIGHTING_ENABLED = false;
        public static Texture2D DEBUG_TEXTURE;
        public static Texture2D MISSING_TEXTURE;
        public static Texture2D BLANK_TEXTURE;
        public static bool DRAW_WORLD_CHUNK_OUTLINE = true;
        public static bool DRAW_STRUCTURE_DEBUG = true;
        public static bool DRAW_INSTANCE_EXIT_LOCATIONS_OUTLINE = true;

        // DEBUG
        public static SpriteFont FontDebug;
        public static DebugWindow DebugWindow;

        public static class Debug
        {
            public static ConsoleComponent Console;
            //public static DebugWindow DebugWindow;
        }

        public static ContentManager ContentManager;
        public static SpriteBatch SpriteBatch;
        public static GraphicsDevice GraphicsDevice;
        public static GraphicsDeviceManager GraphicsDeviceManager;


        public static PlayerObj PlayerObj;
        public static class Player
        {
            public static PlayerInventory Inventory;
            public static PlayerTextDisplay TextDisplay;
            public static PlayerEquipment Equipment;
        }


        public static Camera Camera;
        public static CameraMinimap CameraMinimap;
        public static GameCursor GameCursor;
        public static SpriteLib SpriteLibrary;
        public static ScrollDisplayer ScrollDisplay;

        public static WorldChunkManager WorldChunkManager;

        public static InputManager InputManager;
        public static SceneManager SceneManager;
        public static DialogueManager DialogueManager;
    }
}
