using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.GameObjects;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;

namespace ProjectDonut.Core.SceneManagement
{
    public class SceneManager : IGameObject
    {
        public WorldScene CurrentScene { get; set; }
        public InstanceScene InstanceScene { get; set; }

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
            //CurrentScene.Initialize();
            InstanceScene.Initialize();
        }

        public void LoadContent(ContentManager content)
        {
            //CurrentScene.LoadContent(content);
            InstanceScene.LoadContent(content);
        }

        public void Update(GameTime gameTime)
        {
            //CurrentScene.Update(gameTime);
            InstanceScene.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //CurrentScene.Draw(gameTime, spriteBatch);
            InstanceScene.Draw(gameTime, spriteBatch);
        }

        public void CreateWorldScene()
        {
            var scene = new WorldScene(
                SceneType.World, 
                _player, 
                _content, 
                _spriteBatch, 
                _graphicsDevice,
                _camera,
                _spriteLib);

            CurrentScene = scene;
        }

        public void CreateInstanceScene()
        {
            var scene = new InstanceScene(
                SceneType.Instance,
                _player,
                _content,
                _spriteBatch,
                _graphicsDevice,
                _camera,
                _spriteLib);

            InstanceScene = scene;
        }
    }
}
