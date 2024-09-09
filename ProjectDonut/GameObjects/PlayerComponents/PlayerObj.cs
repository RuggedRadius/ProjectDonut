using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Penumbra;
using ProjectDonut.Core;
using ProjectDonut.Core.Input;
using ProjectDonut.Debugging;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World;
using System.Collections.Generic;

namespace ProjectDonut.GameObjects.PlayerComponents
{
    public class PlayerObj : IGameObject
    {
        public Vector2 WorldPosition { get; set; }
        public Vector2 ChunkPosition
        {
            get
            {
                var offsetX = ChunkPosX * Global.ChunkSize * Global.TileSize;
                var offsetY = ChunkPosY * Global.ChunkSize * Global.TileSize;
                var offset = new Vector2(offsetX, offsetY);
                return WorldPosition - offset;
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

        public Rectangle VisibilityRect;
        public Rectangle InteractBounds { get; set; }

        public int ChunkPosX { get; set; }
        public int ChunkPosY { get; set; }

        public bool IsVisible { get; set; }

        private Dictionary<string, Texture2D> _textures;
        public PointLight Light;

        public PlayerObj()
        {
        }

        public void Initialize()
        {
            IsVisible = true;
            WorldPosition = new Vector2(50, 50);
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

            Light = new PointLight
            {
                Position = this.WorldPosition,
                Scale = new Vector2(3000),
                Color = Color.White,
                Intensity = 1f,
                //Radius = 500,

                ShadowType = ShadowType.Illuminated
            };
            Global.Penumbra.Lights.Add(Light);
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

            Texture = _textures["walk-south-01"];
        }

        public void Update(GameTime gameTime)
        {


            var chunkCoords = GetWorldChunkCoords();
            ChunkPosX = chunkCoords.Item1;
            ChunkPosY = chunkCoords.Item2;

            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleInput(gameTime);

            DebugWindow.Lines[0] = $"World Chunk: [{ChunkPosX}, {ChunkPosY}]";
            DebugWindow.Lines[1] = $"World Position: [{(int)WorldPosition.X}, {(int)WorldPosition.Y}]";
            DebugWindow.Lines[2] = $"Chunk Position: [{(int)ChunkPosition.X}, {(int)ChunkPosition.Y}]";            

            VisibilityRect = new Rectangle(
                (int)WorldPosition.X - 500,
                (int)WorldPosition.Y - 500,
                1000,
                1000
                );

            //InteractBounds = new Rectangle(
            //    (int)Position.X - Global.TileSize,
            //    (int)Position.Y - Global.TileSize,
            //    Global.TileSize * 3,
            //    Global.TileSize * 3);
            InteractBounds = new Rectangle(
                (int)WorldPosition.X - (Global.TileSize / 2),
                (int)WorldPosition.Y - (Global.TileSize / 2),
                Global.TileSize,
                Global.TileSize);

            Light.Position = WorldPosition;
            Global.Penumbra.Transform = Global.Camera.GetTransformationMatrix();
        }

        private void HandleInput(GameTime gameTime)
        {
            var movement = new Vector2();

            if (InputManager.KeyboardState.IsKeyDown(Keys.W))
            {
                movement.Y -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (InputManager.KeyboardState.IsKeyDown(Keys.S))
            {
                movement.Y += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (InputManager.KeyboardState.IsKeyDown(Keys.D))
            {
                movement.X += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (InputManager.KeyboardState.IsKeyDown(Keys.A))
            {
                movement.X -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // TODO: REMOVE THIS LATER ***********************************************************
            if (InputManager.KeyboardState.IsKeyDown(Keys.K))
            {
                speed -= 5;
            }
            else if (InputManager.KeyboardState.IsKeyDown(Keys.L))
            {
                speed += 5;
            }
            // ***********************************************************************************

            UpdateAnimationFrame(movement);

            WorldPosition += movement;
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
            Global.SpriteBatch.Draw(Texture, WorldPosition, null, Color.White, 0, _textureOrigin, 1f, SpriteEffects.None, 0);
            //Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, InteractBounds, Color.Green * 0.25f);
            Global.SpriteBatch.End();
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

            WorldPosition = new Vector2(playerStartPosX, playerStartPosY);
        }

        public (int, int) GetWorldChunkCoords()
        {

            var x = (int)(WorldPosition.X / (Global.TileSize * Global.ChunkSize));
            var y = (int)(WorldPosition.Y / (Global.TileSize * Global.ChunkSize));

            if (WorldPosition.X < 0)
            {
                x--;
            }

            if (WorldPosition.Y < 0)
            {
                y--;
            }

            return (x, y);
        }
    }
}
