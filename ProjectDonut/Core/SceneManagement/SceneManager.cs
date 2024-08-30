using System;
using System.Collections.Generic;
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
        public Scene CurrentScene { get; set; }

        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }

        private SpriteLibrary _spriteLib;

        public Dictionary<string, Scene> Scenes;

        public SceneManager(SpriteLibrary spriteLib)
        {
            _spriteLib = spriteLib;
            Scenes = new Dictionary<string, Scene>()
            {
                { "world", CreateWorldScene() },
                { "instance", CreateInstanceScene() }
            };

            CurrentScene = Scenes["world"];
        }

        public void Initialize()
        {
            CurrentScene.Initialize();
        }

        public void LoadContent(ContentManager content)
        {
            CurrentScene.LoadContent(content);
        }

        public void Update(GameTime gameTime)
        {
            CurrentScene.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            CurrentScene.Draw(gameTime, spriteBatch);
        }

        public void SetCurrentScene(Scene scene)
        {
            CurrentScene = scene;
        }

        public WorldScene CreateWorldScene()
        {
            var scene = new WorldScene(SceneType.World, _spriteLib);
            scene.Initialize();
            scene.LoadContent(Global.ContentManager);

            return scene;
        }

        public InstanceScene CreateInstanceScene()
        {
            var scene = new InstanceScene(SceneType.Instance, _spriteLib);
            scene.Initialize();
            scene.LoadContent(Global.ContentManager);

            return scene;
        }
    }
}
