using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectDonut.Interfaces;
using IGameComponent = ProjectDonut.Interfaces.IGameComponent;

namespace ProjectDonut.Core.SceneManagement.SceneTypes
{
    public abstract class BaseScene : IScene
    {
        public SceneType SceneType { get; set; }
        public Vector2 Position { get; set; }
        public Dictionary<string, IGameObject> _gameObjects { get; set; }
        public Dictionary<string, IScreenObject> _screenObjects { get; set; }
        public Dictionary<string, IGameComponent> _gameComponents { get; set; }
        public Dictionary<string, List<ISceneObject>> _sceneObjects { get; set; }



        public virtual void Initialize()
        {
            _gameObjects = new Dictionary<string, IGameObject>();
            _screenObjects = new Dictionary<string, IScreenObject>();
            _gameComponents = new Dictionary<string, IGameComponent>();
            _sceneObjects = new Dictionary<string, List<ISceneObject>>();
        }

        public virtual void LoadContent()
        {
            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent());
            //_sceneObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent());
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.LoadContent());
        }

        public virtual void Update(GameTime gameTime)
        {
            _gameObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));
            _sceneObjects.Select(x => x.Value).ToList().ForEach(x => x.ForEach(x => x.Update(gameTime)));
            _screenObjects.Select(x => x.Value).ToList().ForEach(x => x.Update(gameTime));
        }

        public virtual void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            _gameObjects
                .Select(x => x.Value)
                .OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));
            //Global.SpriteBatch.End();

            // SceneObjects
            _sceneObjects
                .Select(x => x.Value)
                //.OrderByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.ForEach(x => x.Draw(gameTime)));

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

        public virtual void PrepareForPlayerExit()
        {

        }
    }
}
