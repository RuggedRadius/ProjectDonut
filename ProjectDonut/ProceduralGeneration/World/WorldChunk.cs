using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.ProceduralGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World.Structures;
using ProjectDonut.UI.ScrollDisplay;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.GameObjects;
using ProjectDonut.ProceduralGeneration.World.Generators;
using System.Diagnostics;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.Core;
using IGameComponent = ProjectDonut.Interfaces.IGameComponent;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class WorldChunk : IGameComponent
    {
        public int ChunkCoordX { get; private set; }
        public int ChunkCoordY { get; private set; }

        public int WorldCoordX;
        public int WorldCoordY;

        public int[,] HeightData;
        public int[,] BiomeData;
        public int[,] ForestData;
        public int[,] RiverData;
        public int[,] StructureData;

        public Dictionary<string, Tilemap> Tilemaps;

        public List<Rectangle> StructureBounds;// TODO: NOT SURE THIS SHOULD EXIST...
        public List<StructureData> Structures;
        private WorldChunkManager _manager;

        public Dictionary<string, List<ISceneObject>> SceneObjects;

        public int Width
        {
            get
            {
                if (HeightData == null)
                    return 0;

                return HeightData.GetLength(0);
            }
            set
            {
                Width = value;
            }
        }
        public int Height
        {
            get
            {
                if (HeightData == null)
                    return 0;

                return HeightData.GetLength(1);
            }
            set
            {
                Height = value;
            }
        }

        public Vector2 Position // TODO: Use this instead of individuals int above
        { 
            get; 
            set; 
        }
        public int ZIndex // TODO: Currently not used
        { 
            get; 
            set; 
        }

        private Texture2D tempTexture;

        private ScrollDisplayer _scrollDisplayer;

        public WorldChunk(int chunkXPos, int chunkYPos, ScrollDisplayer scrollDisplayer, WorldChunkManager manager)
        {
            _manager = manager;
            ChunkCoordX = chunkXPos;
            ChunkCoordY = chunkYPos;

            WorldCoordX = chunkXPos * Global.ChunkSize * Global.TileSize;
            WorldCoordY = chunkYPos * Global.ChunkSize * Global.TileSize;

            _scrollDisplayer = scrollDisplayer;

            Tilemaps = new Dictionary<string, Tilemap>();

            // Create a new Texture2D object with the dimensions Global.TileSizexGlobal.TileSize
            tempTexture = new Texture2D(Global.GraphicsDevice, Global.TileSize, Global.TileSize);

            // Create an array to hold the color data
            Color[] colorData = new Color[Global.TileSize * Global.TileSize];

            // Fill the array with Color.White
            for (int i = 0; i < colorData.Length; i++)
            {
                colorData[i] = Color.White;
            }

            // Set the texture data to the array of colors
            tempTexture.SetData(colorData);
        }

        public void Initialize()
        {

        }

        public void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            // Update each tile
            foreach (var tilemap in Tilemaps)
            {
                foreach (var tile in tilemap.Value.Map)
                {
                    if (tile == null)
                        continue;

                    tile.Update(gameTime);
                }
            }

            if (_manager.PlayerChunk == this)
            {
                foreach (var structure in Structures)
                {
                    if (structure.Bounds.Contains(Global.Player.ChunkPosition.X, Global.Player.ChunkPosition.Y))
                    {
                        // TODO: TEMP CODE TO TEST SCENE SWITCHING
                        var worldScene = (WorldScene)Global.SceneManager.CurrentScene;

                        var worldExitPointX = (structure.Bounds.Width/2) + structure.Bounds.X + (Global.Player.ChunkPosX * Global.TileSize * Width); 
                        var worldExitPointY = structure.Bounds.Bottom + Global.TileSize + (Global.Player.ChunkPosY * Global.TileSize * Height); 
                        
                        worldScene.LastExitLocation = new Rectangle(worldExitPointX, worldExitPointY, Global.TileSize, Global.TileSize);

                        Global.SceneManager.SetCurrentScene(structure.Instance, SceneType.Instance);
                        Global.SceneManager.CurrentScene.PrepareForPlayerEntry();
                    }
                }
            }

            if (SceneObjects != null && SceneObjects.ContainsKey("trees") && SceneObjects["trees"].Count > 0)
                Debugging.Debugger.Lines[2] = $"Tree Z-Index: {SceneObjects["trees"][0].ZIndex}";

            // Check for scroll display
            //HandleScrollDisplay();
        }

        // **** BEWARE: THIS IS VERY BROKEN ***
        private void HandleScrollDisplay()
        {
            var playerPos = Global.Player.ChunkPosition;
            Debugging.Debugger.Lines[7] = $"PlayerChunkPos = {playerPos}";
            foreach (var structure in Structures)
            {
                var distance = Vector2.Distance(playerPos, new Vector2(structure.Bounds.X, structure.Bounds.Y));
                Debugging.Debugger.Lines[2] = $"Distance = {distance}";
                if (distance <= 100)
                {
                    // Mouse is hovering over this structure
                    if (_scrollDisplayer.CurrentStructureData == structure)
                    {
                        return;
                    }

                    var x = structure.Bounds.X + (structure.Bounds.Width / 2);
                    var y = 50;

                    _scrollDisplayer.DisplayScroll(structure);
                    return;
                }
            }

            _scrollDisplayer.HideScroll();
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var tilemap in Tilemaps)
            {
                foreach (var tile in tilemap.Value.Map)
                {
                    if (tile == null)
                        continue;

                    tile.Draw(gameTime);
                }
            }

            if (Global.DRAW_WORLD_CHUNK_OUTLINE)
            {
                DrawChunkOutline(gameTime);
            }

            if (Global.DRAW_STRUCTURE_ENTRY_OUTLINE)
            {
                foreach (var structure in Structures)
                {
                    Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, structure.Bounds, Color.White);                    
                }
            }

            return;
        }

        public void DrawSceneObjectsBelowPlayer(GameTime gameTime)
        {
            foreach (var sceneObject in SceneObjects)
            {
                foreach (var obj in sceneObject.Value)
                {
                    if (obj.ZIndex <= Global.Player.Position.Y)
                    {
                        obj.Draw(gameTime);
                    }
                }
            }
        }

        public void DrawSceneObjectsAbovePlayer(GameTime gameTime)
        {
            foreach (var sceneObject in SceneObjects)
            {
                foreach (var obj in sceneObject.Value)
                {
                    if (obj.ZIndex >= Global.Player.Position.Y)
                    {
                        obj.Draw(gameTime);
                    }
                }
            }
        }

        private void DrawChunkOutline(GameTime gameTime)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var position = new Vector2(WorldCoordX + x * Global.TileSize, WorldCoordY + y * Global.TileSize);

                    if (x == 0 || y == 0)
                    {
                        Global.SpriteBatch.Draw(tempTexture, position, null, Color.Magenta);
                    }
                    else if (x == Width - 1 || y == Height - 1)
                    {
                        Global.SpriteBatch.Draw(tempTexture, position, null, Color.Magenta);
                    }
                }
            }
        }        

        public void GrowForest()
        {

        }        
    }
}
