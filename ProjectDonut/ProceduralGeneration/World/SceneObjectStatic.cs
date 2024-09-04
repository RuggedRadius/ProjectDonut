using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class SceneObjectStatic : ISceneObject
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public int ZIndex { get; set; }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        public void Initialize()
        {
        }

        public void LoadContent(ContentManager content)
        {
        }

        public void Update()
        {
            //    var verticalDistanceFromPlayer = Global.Player.Position.Y - Position.Y;
            //    ZIndex = (int)verticalDistanceFromPlayer * -1; 
        }

        public void Draw()
        {
            Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            Global.SpriteBatch.Draw(Texture, Position, Color.White);
            Global.SpriteBatch.End();
        }
    }
}
