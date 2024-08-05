using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace ProjectGorilla.GameObjects
{
    public class Player : GameObject
    {
        private Texture2D texture;
        private Texture2D spriteSheet;

        private GraphicsDeviceManager _graphics;
        private GraphicsDevice _graphicsDevice;
        private ContentManager _content;
        private SpriteBatch _spriteBatch;

        private Vector2 spriteSize;
        private Rectangle currentFrame;

        private Vector2 position;
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


        public Player(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spriteBatch)
        {
            this._graphics = graphics;
            this._graphicsDevice = graphicsDevice;
            this._content = content;
            this._spriteBatch = spriteBatch;
        }

        public override void Initialize()
        {
            position = new Vector2(50, 50);
            speed = 200;
            spriteSize = new Vector2(32, 32);
            ZIndex = 10;

            _frameWidth = 32; // Width of a single frame
            _frameHeight = 32; // Height of a single frame
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
                currentFrame = new Rectangle(64, 64, 32, 32);
                return;
            }
            if (movement.X < 0 && movement.Y > 0)
            {
                currentFrame = new Rectangle(0, 64, 32, 32);
                return;
            }
            if (movement.X < 0 && movement.Y < 0)
            {
                currentFrame = new Rectangle(0, 0, 32, 32);
                return;
            }
            if (movement.X > 0 && movement.Y < 0)
            {
                currentFrame = new Rectangle(64, 0, 32, 32);
                return;
            }
            if (movement.Y < 0)
            {
                currentFrame = new Rectangle(32, 0, 32, 32);
                return;
            }
            if (movement.Y > 0)
            {
                currentFrame = new Rectangle(32, 64, 32, 32);
                return;
            }
            if (movement.X < 0)
            {
                currentFrame = new Rectangle(0, 32, 32, 32);
                return;
            }
            if (movement.X > 0)
            {
                currentFrame = new Rectangle(64, 32, 32, 32);
                return;
            }
        }

        public override void Draw(GameTime gameTime)
        {            
            _spriteBatch.Draw(spriteSheet, position, currentFrame, Color.White);
            _spriteBatch.DrawString(debugFont, $"X:{position.X}", new Vector2(0, 0), Color.Green);
            _spriteBatch.DrawString(debugFont, $"Y:{position.Y}", new Vector2(0, 20), Color.Green);
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
    }
}
