using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectDonut.Interfaces;
using IGameComponent = ProjectDonut.Interfaces.IGameComponent;

namespace ProjectDonut.Core.SceneManagement
{
    public interface IScene
    {
        SceneType SceneType { get; set; }
        Vector2 Position { get; set; }

        Dictionary<string, IGameObject> _gameObjects { get; set; }
        Dictionary<string, IScreenObject> _screenObjects { get; set; }
        Dictionary<string, IGameComponent> _gameComponents { get; set; }
        Dictionary<string, List<ISceneObject>> _sceneObjects{ get; set; }

        void PrepareForPlayerEntry();
        void PrepareForPlayerExit();

        void Initialize();
        void LoadContent();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
        void DrawMinimap(GameTime gameTime);

    }
}
