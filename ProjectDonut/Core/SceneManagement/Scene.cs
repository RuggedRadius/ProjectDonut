using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Core.SceneManagement
{
    public class Scene : IGameObject
    {
        public SceneType SceneType { get; set; }
        public Dictionary<string, IGameObject> _gameObjects;
        public Dictionary<string, IScreenObject> _screenObjects;
        //public Dictionary<string, ISceneObject> _sceneObjects;

        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }



        public virtual void Initialize()
        {
            _gameObjects = new Dictionary<string, IGameObject>();
            _screenObjects = new Dictionary<string, IScreenObject>();
            //_sceneObjects = new Dictionary<string, ISceneObject>();
        }

        public virtual void LoadContent(ContentManager content)
        {
            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent(content));
            //_sceneObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent(content));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent());
        }

        public virtual void Update(GameTime gameTime)
        {
            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));
            //_sceneObjects.Select(x => x.Value).ToList().ForEach(x => x.Update());
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            _gameObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime, spriteBatch));
            Global.SpriteBatch.End();

            // ScreenObjects
            _screenObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));
        }

        public virtual void PrepareForPlayerEntry()
        {

        }
    }
}
