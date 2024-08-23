﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using ProjectDonut.Debugging;
using ProjectDonut.GameObjects;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace ProjectDonut.GameObjects
{
    public class Player : IGameObject
    {
        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }

        private Texture2D texture;
        private Texture2D spriteSheet;

        private GraphicsDeviceManager _graphics;
        private GraphicsDevice _graphicsDevice;
        private ContentManager _content;
        private SpriteBatch _spriteBatch;

        private Camera _camera;

        private Vector2 spriteSize;
        private Rectangle currentFrame;
        
        private int speed;

        private int _frameWidth;
        private int _frameHeight;
        private int _frameCount;
        private int _currentFrame;
        private float _frameTime;
        private float _timer;

        private Rectangle rectTop;
        private Rectangle rectRight;
        private Rectangle rectLeft;
        private Rectangle rectBottom;
        private Texture2D debugTexture;

        private int TileSize = 32;
        private int ChunkSize = 100;

        public int ChunkPosX { get; set; }
        public int ChunkPosY { get; set; }

        public Player(
            GraphicsDeviceManager graphics,
            GraphicsDevice graphicsDevice,
            ContentManager content,
            SpriteBatch spriteBatch,
            Camera camera)
        {
            this._graphics = graphics;
            this._graphicsDevice = graphicsDevice;
            this._content = content;
            this._spriteBatch = spriteBatch;
            this._camera = camera;
        }

        public void Initialize()
        {
            Position = new Vector2(50, 50);
            speed = 2000;
            spriteSize = new Vector2(TileSize, TileSize);
            ZIndex = -100;

            _frameWidth = TileSize; // Width of a single frame
            _frameHeight = TileSize; // Height of a single frame
            _frameCount = 9; // Total number of frames in the sprite sheet
            _currentFrame = 0; // Start at the first frame
            _frameTime = 0.1f; // Duration of each frame in seconds
            _timer = 0f;

            debugTexture = CreateTexture(_graphicsDevice, 1, 1, Color.White);
        }

        Texture2D CreateTexture(GraphicsDevice graphicsDevice, int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, width, height);
            Color[] colorData = new Color[width * height];
            for (int i = 0; i < colorData.Length; ++i) colorData[i] = color;
            texture.SetData(colorData);
            return texture;
        }

        public void LoadContent()
        {
            spriteSheet = _content.Load<Texture2D>("Sprites/TestPlayer");
            currentFrame = new Rectangle(0, 0, (int)spriteSize.X, (int)spriteSize.Y);            
        }

        public void Update(GameTime gameTime)
        {
            var chunkCoords = GetWorldChunkCoords();
            ChunkPosX = chunkCoords.Item1;
            ChunkPosY = chunkCoords.Item2;

            // Update the timer
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Check if it's time to switch to the next frame
            //if (_timer >= _frameTime)
            //{
            //    _currentFrame++;
            //    if (_currentFrame >= _frameCount)
            //    {
            //        _currentFrame = 0; // Loop back to the first frame
            //    }
            //    _timer = 0f; // Reset the timer
            //}

            HandleInput(gameTime);

            Debugger.Lines[0] = $"Player Position-X: {Position.X}";
            Debugger.Lines[1] = $"Player Position-Y: {Position.Y}";
            Debugger.Lines[2] = $"Player Chunk-X: {ChunkPosX}";
            Debugger.Lines[3] = $"Player Chunk-Y: {ChunkPosY}";
        }

        private void HandleInput(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            var movement = new Vector2();

            if (state.IsKeyDown(Keys.W))
            {
                movement.Y -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (state.IsKeyDown(Keys.S))
            {
                movement.Y += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (state.IsKeyDown(Keys.D))
            {
                movement.X += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (state.IsKeyDown(Keys.A))
            {
                movement.X -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            UpdateAnimationFrame(movement);

            Position += movement;
        }

        private void UpdateAnimationFrame(Vector2 movement)
        {
            if (movement.X > 0 && movement.Y > 0)
            {
                currentFrame = new Rectangle(TileSize * 2, TileSize * 2, TileSize, TileSize);
                return;
            }
            if (movement.X < 0 && movement.Y > 0)
            {
                currentFrame = new Rectangle(0, TileSize * 2, TileSize, TileSize);
                return;
            }
            if (movement.X < 0 && movement.Y < 0)
            {
                currentFrame = new Rectangle(0, 0, TileSize, TileSize);
                return;
            }
            if (movement.X > 0 && movement.Y < 0)
            {
                currentFrame = new Rectangle(TileSize * 2, 0, TileSize, TileSize);
                return;
            }
            if (movement.Y < 0)
            {
                currentFrame = new Rectangle(TileSize, 0, TileSize, TileSize);
                return;
            }
            if (movement.Y > 0)
            {
                currentFrame = new Rectangle(TileSize, TileSize * 2, TileSize, TileSize);
                return;
            }
            if (movement.X < 0)
            {
                currentFrame = new Rectangle(0, TileSize, TileSize, TileSize);
                return;
            }
            if (movement.X > 0)
            {
                currentFrame = new Rectangle(TileSize * 2, TileSize, TileSize, TileSize);
                return;
            }
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(transformMatrix: _camera.GetTransformationMatrix());
            _spriteBatch.Draw(spriteSheet, Position, currentFrame, Color.White);
            _spriteBatch.End();

        }

        private void DrawDebugRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            // Draw top side
            spriteBatch.Draw(debugTexture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, 1), color);
            // Draw left side
            spriteBatch.Draw(debugTexture, new Rectangle(rectangle.Left, rectangle.Top, 1, rectangle.Height), color);
            // Draw right side
            spriteBatch.Draw(debugTexture, new Rectangle(rectangle.Right, rectangle.Top, 1, rectangle.Height), color);
            // Draw bottom side
            spriteBatch.Draw(debugTexture, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, 1), color);
        }

        public void PositionPlayerInMiddleOfMap(WorldMapSettings settings)
        {
            var playerStartPosX = (settings.Width * settings.TileSize) / 2;
            var playerStartPosY = (settings.Height * settings.TileSize) / 2;
            
            Position = new Vector2(playerStartPosX, playerStartPosY);
        }

        public (int, int) GetWorldChunkCoords()
        {
            
            var x = (int)((Position.X / (TileSize * ChunkSize)));
            var y = (int)((Position.Y / (TileSize * ChunkSize)));

            if (Position.X < 0)
            {
                x--;
            }

            if (Position.Y < 0)
            {
                y--;
            }

            return (x, y);
        }
    }
}
