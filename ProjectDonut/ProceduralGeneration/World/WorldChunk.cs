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

namespace ProjectDonut.ProceduralGeneration.World
{
    public class WorldChunk : IGameObject
    {
        private bool DRAW_DEBUG_OUTLINE = true;

        public int ChunkCoordX { get; private set; }
        public int ChunkCoordY { get; private set; }

        public int WorldCoordX;
        public int WorldCoordY;

        public int[,] HeightData;
        public int[,] BiomeData;
        public int[,] ForestData;
        public int[,] RiverData;
        public int[,] StructureData;

        private int TileSize = 32;

        public Dictionary<string, Tilemap> Tilemaps;

        public List<Rectangle> StructureBounds;

        public List<StructureData> Structures;

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
            get => throw new NotImplementedException(); 
            set => throw new NotImplementedException(); 
        }
        public int ZIndex // TODO: Currently not used
        { 
            get; 
            set; 
        }

        private Texture2D tempTexture;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;

        private ScrollDisplayer _scrollDisplayer;
        private Camera _camera;
        private Player _player;

        public WorldChunk(
            int chunkXPos, 
            int chunkYPos, 
            GraphicsDevice graphicsDevice, 
            SpriteBatch spriteBatch, 
            ScrollDisplayer scrollDisplayer,
            Camera camera,
            Player player)
        {
            ChunkCoordX = chunkXPos;
            ChunkCoordY = chunkYPos;

            WorldCoordX = chunkXPos * 100 * 32;
            WorldCoordY = chunkYPos * 100 * 32;

            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            _scrollDisplayer = scrollDisplayer;
            _camera = camera;
            _player = player;

            Tilemaps = new Dictionary<string, Tilemap>();

            // Create a new Texture2D object with the dimensions 32x32
            tempTexture = new Texture2D(_graphicsDevice, 32, 32);

            // Create an array to hold the color data
            Color[] colorData = new Color[32 * 32];

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


            // Check for scroll display
            //HandleScrollDisplay();
        }

        // **** BEWARE: THIS IS VERY BROKEN ***
        private void HandleScrollDisplay()
        {
            var playerPos = _player.ChunkPosition;
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

            if (DRAW_DEBUG_OUTLINE)
            {
                DrawChunkOutline(gameTime);
            }

            return;
        }

        private void DrawChunkOutline(GameTime gameTime)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var position = new Vector2(WorldCoordX + x * 32, WorldCoordY + y * 32);

                    if (x == 0 || y == 0)
                    {
                        _spriteBatch.Draw(tempTexture, position, null, Color.Magenta);
                    }
                    else if (x == Width - 1 || y == Height - 1)
                    {
                        _spriteBatch.Draw(tempTexture, position, null, Color.Magenta);
                    }
                }
            }
        }        

        public void GrowForest()
        {

        }        
    }
}
