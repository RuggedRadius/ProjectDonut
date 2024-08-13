using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using ProjectDonut.GameObjects;
using ProjectDonut.ProceduralGeneration.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace ProjectDonut.GameObjects
{
    public class Player : GameObject
    {
        private Texture2D texture;
        private Texture2D spriteSheet;

        private GraphicsDeviceManager _graphics;
        private GraphicsDevice _graphicsDevice;
        private ContentManager _content;
        private SpriteBatch _spriteBatch;

        private Camera camera;

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

        private SpriteFont debugFont;

        private int TileSize = 32;
        private FogOfWar fog;

        public int ChunkPosX { get; set; }
        public int ChunkPosY { get; set; }

        public Player(
            GraphicsDeviceManager graphics, 
            GraphicsDevice graphicsDevice, 
            ContentManager content, 
            SpriteBatch spriteBatch, 
            Camera camera,
            FogOfWar fog)
        {
            this._graphics = graphics;
            this._graphicsDevice = graphicsDevice;
            this._content = content;
            this._spriteBatch = spriteBatch;
            this.camera = camera;
            this.fog = fog;
        }

        public override void Initialize()
        {
            position = new Vector2(50, 50);
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

        public override void LoadContent()
        {
            spriteSheet = _content.Load<Texture2D>("Sprites/TestPlayer");
            currentFrame = new Rectangle(0, 0, (int)spriteSize.X, (int)spriteSize.Y);
            debugFont = _content.Load<SpriteFont>("Fonts/Default");
        }

        public override void Update(GameTime gameTime)
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

            fog.UpdateFogOfWar((int)position.X, (int)position.Y);

            HandleInput(gameTime);
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

            position += movement;
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

        public override void Draw(GameTime gameTime)
        {            
            _spriteBatch.Draw(spriteSheet, position, currentFrame, Color.White);

            var debugPos = camera.Position;

            _spriteBatch.DrawString(debugFont, $"Chunk-X:{ChunkPosX}", debugPos + new Vector2(0, 0), Color.Red);
            _spriteBatch.DrawString(debugFont, $"Chunk-Y:{ChunkPosY}", debugPos + new Vector2(0, 20), Color.Red);
            _spriteBatch.DrawString(debugFont, $"Tile-X:{(int)(position.X / 32)}", debugPos + new Vector2(0, 40), Color.Red);
            _spriteBatch.DrawString(debugFont, $"Tile-Y:{(int)(position.Y / 32)}", debugPos + new Vector2(0, 60), Color.Red);
            _spriteBatch.DrawString(debugFont, $"X:{position.X}", debugPos + new Vector2(0, 80), Color.Red);
            _spriteBatch.DrawString(debugFont, $"Y:{position.Y}", debugPos + new Vector2(0, 100), Color.Red);
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
            
            position = new Vector2(playerStartPosX, playerStartPosY);
        }

        public (int, int) GetWorldChunkCoords()
        {
            
            var x = (int)((position.X / 3200));
            var y = (int)((position.Y / 3200));

            if (position.X < 0)
            {
                x--;
            }

            if (position.Y < 0)
            {
                y--;
            }

            if (x != 0 || y != 0)
            {
            }

            return (x, y);
        }
    }
}
