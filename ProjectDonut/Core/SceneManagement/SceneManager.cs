using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.GameObjects;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;

namespace ProjectDonut.Core.SceneManagement
{
    public class SceneManager : IGameObject
    {
        public Scene CurrentScene { get; set; }

        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }

        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphicsDevice;
        private Player _player;
        private SpriteLibrary _spriteLib;
        private Camera _camera;

        public SceneManager(
            ContentManager content, 
            SpriteBatch spriteBatch, 
            GraphicsDevice graphicsDevice, 
            Player player,
            SpriteLibrary spriteLib,
            Camera camera)
        {
            _content = content;
            _spriteBatch = spriteBatch;
            _graphicsDevice = graphicsDevice;
            _player = player;
            _spriteBatch = spriteBatch;
            _spriteLib = spriteLib;
            _camera = camera;
        }

        public void Initialize()
        {
            CurrentScene.Initialize();
        }

        public void LoadContent()
        {
            CurrentScene.LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            CurrentScene.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            CurrentScene.Draw(gameTime);
        }

        public void CreateWorldScene()
        {
            var scene = new Scene(
                SceneType.World, 
                _player, 
                _content, 
                _spriteBatch, 
                _graphicsDevice,
                _camera,
                _spriteLib);

            CurrentScene = scene;

        }
    }
}
