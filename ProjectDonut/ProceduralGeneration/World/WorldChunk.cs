﻿using Microsoft.Xna.Framework;
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

        public List<Rectangle> StructureBounds;// TODO: NOT SURE THIS SHOULD EXIST...
        public List<StructureData> Structures;
        private WorldChunkManager _manager;

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

            WorldCoordX = chunkXPos * 100 * 32;
            WorldCoordY = chunkYPos * 100 * 32;

            _scrollDisplayer = scrollDisplayer;

            Tilemaps = new Dictionary<string, Tilemap>();

            // Create a new Texture2D object with the dimensions 32x32
            tempTexture = new Texture2D(Global.GraphicsDevice, 32, 32);

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

        public void LoadContent(ContentManager content)
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

                        var worldExitPointX = (structure.Bounds.Width/2) + structure.Bounds.X + (Global.Player.ChunkPosX * 32 * Width); 
                        var worldExitPointY = structure.Bounds.Bottom + 10 + (Global.Player.ChunkPosY * 32 * Height); 
                        
                        worldScene.LastExitLocation = new Rectangle(worldExitPointX, worldExitPointY, 32, 32);

                        Global.SceneManager.SetCurrentScene(structure.Instance);
                        Global.SceneManager.CurrentScene.PrepareForPlayerEntry();
                    }
                }
            }
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

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var tilemap in Tilemaps)
            {
                foreach (var tile in tilemap.Value.Map)
                {
                    if (tile == null)
                        continue;

                    tile.Draw(gameTime, spriteBatch);
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
