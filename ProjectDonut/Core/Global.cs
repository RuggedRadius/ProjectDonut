using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core.Input;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.GameObjects;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.Pathfinding;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.World;
using ProjectDonut.UI.DialogueSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Core
{
    public static class Global
    {
        // MAP SETTINGS
        public static int TileSize = 64;
        public static int ChunkSize = 50;

        // DEBUG SETTINGS
        public static Texture2D DEBUG_TEXTURE;
        public static bool DRAW_WORLD_CHUNK_OUTLINE = false;
        public static bool DRAW_STRUCTURE_ENTRY_OUTLINE = true;
        public static bool DRAW_INSTANCE_EXIT_LOCATIONS_OUTLINE = true;

        public static ContentManager ContentManager;
        public static SpriteBatch SpriteBatch;
        public static GraphicsDevice GraphicsDevice;
        public static GraphicsDeviceManager GraphicsDeviceManager;

        public static Player Player;
        public static PlayerInventory PlayerInventory;
        public static Camera Camera;
        public static GameCursor GameCursor;
        public static SpriteLibrary SpriteLibrary;
        //public static Astar Pathfinding;

        public static WorldChunkManager WorldChunkManager;

        public static InputManager InputManager;
        public static SceneManager SceneManager;
        public static DialogueManager DialogueManager;
    }
}
