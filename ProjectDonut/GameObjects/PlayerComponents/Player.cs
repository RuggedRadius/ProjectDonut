using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using ProjectDonut.Core;
using ProjectDonut.Debugging;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace ProjectDonut.GameObjects.PlayerComponents
{
    public class Player : IGameObject
    {
        public Vector2 Position { get; set; }
        public Vector2 ChunkPosition
        {
            get
            {
                var offsetX = ChunkPosX * Global.ChunkSize * Global.TileSize;
                var offsetY = ChunkPosY * Global.ChunkSize * Global.TileSize;
                var offset = new Vector2(offsetX, offsetY);
                return Position - offset;
            }
        }
        public int ZIndex { get; set; }

        public Texture2D Texture { get; set; }
        private Texture2D spriteSheet;


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

        public int ChunkPosX { get; set; }
        public int ChunkPosY { get; set; }

        //private SpriteLibrary _spriteLib;

        private PlayerInventory _inventory;
        private GameCursor _cursor;

        private Dictionary<string, Texture2D> _textures;

        public Player()
        {
        }

        public void Initialize()
        {
            Position = new Vector2(50, 50);
            speed = 200;
            spriteSize = new Vector2(Global.TileSize, Global.TileSize);
            ZIndex = 0;

            _frameWidth = Global.TileSize; // Width of a single frame
            _frameHeight = Global.TileSize; // Height of a single frame
            _frameCount = 9; // Total number of frames in the sprite sheet
            _currentFrame = 0; // Start at the first frame
            _frameTime = 0.1f; // Duration of each frame in seconds
            _timer = 0f;

            _textures = new Dictionary<string, Texture2D>();

            debugTexture = CreateTexture(Global.GraphicsDevice, 1, 1, Color.White);

            _inventory = new PlayerInventory(Global.ContentManager, _cursor);
            _inventory.Initialize();
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
            spriteSheet = Global.ContentManager.Load<Texture2D>("Sprites/TestPlayer");

            _textures.Add("walk-north-01", Global.ContentManager.Load<Texture2D>("Sprites/Player/Player-Walk-N-01"));
            _textures.Add("walk-east-01", Global.ContentManager.Load<Texture2D>("Sprites/Player/Player-Walk-E-01"));
            _textures.Add("walk-south-01", Global.ContentManager.Load<Texture2D>("Sprites/Player/Player-Walk-S-01"));
            _textures.Add("walk-west-01", Global.ContentManager.Load<Texture2D>("Sprites/Player/Player-Walk-W-01"));

            currentFrame = new Rectangle(0, 0, (int)spriteSize.X, (int)spriteSize.Y);

            _inventory.LoadContent();

            Texture = _textures["walk-south-01"];
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

            Debugger.Lines[0] = $"Player World Position: [{(int)Position.X}, {(int)Position.Y}]";
            Debugger.Lines[1] = $"World Chunk Coords: [{ChunkPosX}, {ChunkPosY}]";
            Debugger.Lines[3] = $"Player Chunk Position = [{(int)ChunkPosition.X}, {(int)ChunkPosition.Y}]";

            _inventory.Update(gameTime);
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

            // TODO: REMOVE THIS LATER ***********************************************************
            if (state.IsKeyDown(Keys.K))
            {
                speed -= 5;
            }
            else if (state.IsKeyDown(Keys.L))
            {
                speed += 5;
            }
            // ***********************************************************************************

            UpdateAnimationFrame(movement);

            Position += movement;
        }

        private Vector2 _textureOrigin;
        private void UpdateAnimationFrame(Vector2 movement)
        {
            if (movement.X > 0)
            {
                Texture = _textures["walk-east-01"];
            }
            else if (movement.X < 0)
            {
                Texture = _textures["walk-west-01"];
            }
            else if (movement.Y < 0)
            {
                Texture = _textures["walk-north-01"];
            }
            else if (movement.Y > 0)
            {
                Texture = _textures["walk-south-01"];
            }

            if (Texture != null)
            {
                _textureOrigin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
            }
        }

        public void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            //_spriteBatch.Draw(spriteSheet, Position, currentFrame, Color.White);
            Global.SpriteBatch.Draw(Texture, Position, null, Color.White, 0, _textureOrigin, 1f, SpriteEffects.None, 0);
            Global.SpriteBatch.End();

            _inventory.Draw(gameTime);
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
            var playerStartPosX = settings.Width * Global.TileSize / 2;
            var playerStartPosY = settings.Height * Global.TileSize / 2;

            Position = new Vector2(playerStartPosX, playerStartPosY);
        }

        public (int, int) GetWorldChunkCoords()
        {

            var x = (int)(Position.X / (Global.TileSize * Global.ChunkSize));
            var y = (int)(Position.Y / (Global.TileSize * Global.ChunkSize));

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
