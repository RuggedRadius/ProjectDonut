using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.GameObjects;
using ProjectDonut.GameObjects.PlayerComponents;
using ProjectDonut.Interfaces;
using ProjectDonut.Pathfinding;
using ProjectDonut.ProceduralGeneration;

namespace ProjectDonut.Core.SceneManagement
{
    public class SceneManager : IGameObject
    {
        public Scene CurrentScene { get; set; }

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

            if (scene.GetType() == typeof(InstanceScene))
            {
                var instance = ((InstanceScene)scene);

                // Initialise occupancy grid for pathfinding
                //Astar.InitialiseOccupiedCells(instance.DataMap.GetLength(0), instance.DataMap.GetLength(1));
                //foreach (var enemy in instance.Enemies)
                //{
                //    var dataMapCoords = new Vector2(enemy.Position.X / 32, enemy.Position.Y / 32);
                //    Astar.SetOccupiedCell((int)dataMapCoords.X, (int)dataMapCoords.Y, true);
                //}
            }
        }

        public WorldScene CreateWorldScene()
        {
            var scene = new WorldScene(SceneType.World, Global.SpriteLibrary);
            scene.Initialize();
            scene.LoadContent(Global.ContentManager);

            return scene;
        }

        public InstanceScene CreateInstanceScene()
        {
            var scene = new InstanceScene(SceneType.Instance, Global.SpriteLibrary);
            scene.Initialize();
            scene.LoadContent(Global.ContentManager);

            return scene;
        }
    }
}
