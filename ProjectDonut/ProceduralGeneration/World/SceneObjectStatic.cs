using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Interfaces;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class SceneObjectStatic : ISceneObject
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public int ZIndex { get; set; }

        public void Draw()
        {
            Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            Global.SpriteBatch.Draw(Texture, Position, Color.White);
            Global.SpriteBatch.End();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void LoadContent(ContentManager content)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            //UpdateZIndex();   
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        //private void UpdateZIndex()
        //{
        //    var verticalDistanceFromPlayer = Global.Player.Position.Y - Position.Y;
        //    ZIndex = (int)verticalDistanceFromPlayer * -1;
        //}
    }
}
