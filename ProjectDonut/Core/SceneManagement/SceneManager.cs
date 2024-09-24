using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using ProjectDonut.GameObjects;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.Interfaces;
using IGameComponent = ProjectDonut.Interfaces.IGameComponent;

namespace ProjectDonut.Core.SceneManagement
{
    public class SceneManager : IGameComponent
    {
        public IScene CurrentScene { get; set; }
        public SceneType CurrentSceneType { get; set; }

        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }

        public Dictionary<string, IScene> Scenes;

        public SceneManager()
        {
            Scenes = new Dictionary<string, IScene>()
            {
                { "world", CreateWorldScene() },
                //{ "instance", CreateInstanceScene() }
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

        public virtual void DrawMinimap(GameTime gameTime)
        {
            CurrentScene.DrawMinimap(gameTime);
        }

        public void SetCurrentScene(IScene scene)
        {
            CurrentScene = scene;
            CurrentSceneType = scene.SceneType;
        }

        public WorldScene CreateWorldScene()
        {
            var scene = new WorldScene(SceneType.World);
            scene.Initialize();
            scene.LoadContent();

            return scene;
        }

        //public IScene CreateInstanceScene(SceneType sceneType)
        //{
        //    IScene scene;

        //    switch (sceneType)
        //    {
        //        case SceneType.Dungeon:
        //            scene = new DungeonScene();
        //            break;

        //        case SceneType.Town:
        //            scene = new TownScene();
        //            break;

        //        default:
        //            return null;
        //    }

        //    scene.Initialize();
        //    scene.LoadContent();

        //    return scene;
        //}
    }
}
