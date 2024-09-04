using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.GameObjects;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.Interfaces;
using IGameComponent = ProjectDonut.Interfaces.IGameComponent;

namespace ProjectDonut.Core.SceneManagement
{
    public class SceneManager : IGameComponent
    {
        public Scene CurrentScene { get; set; }
        public SceneType CurrentSceneType { get; set; }

        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }

        public Dictionary<string, Scene> Scenes;

        public SceneManager()
        {
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

        public void SetCurrentScene(Scene scene, SceneType sceneType)
        {
            CurrentScene = scene;
            CurrentSceneType = sceneType;
        }

        public WorldScene CreateWorldScene()
        {
            var scene = new WorldScene(SceneType.World);
            scene.Initialize();
            scene.LoadContent();

            return scene;
        }

        public InstanceScene CreateInstanceScene()
        {
            var scene = new InstanceScene(SceneType.Instance);
            scene.Initialize();
            scene.LoadContent();

            return scene;
        }
    }
}
