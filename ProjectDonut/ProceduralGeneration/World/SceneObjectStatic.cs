using System;
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

        public int DrawOrder => throw new NotImplementedException();

        public bool Visible { get; set; }
        public bool Explored { get; set; }

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public void Initialize()
        {
        }

        public void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            UpdateObjectVisibility();
        }

        public void Draw(GameTime gameTime)
        {
            if (!Explored) return;


            if (!Visible)
            {
                Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
                Global.SpriteBatch.Draw(Texture, Position, Color.Gray);
                Global.SpriteBatch.End();
            }
            else
            {
                Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
                Global.SpriteBatch.Draw(Texture, Position, Color.White);
                Global.SpriteBatch.End();
            }
        }

        public void UpdateObjectVisibility()
        {
            if (Global.SHOW_FOG_OF_WAR == false)
            {
                Visible = true;
                Explored = true;
                return;
            }

            float distance = Math.Abs(Vector2.Distance(Global.Player.Position, Position));
            Visible = (distance <= Global.FOG_OF_WAR_WORLD_RADIUS) ? true : false;

            if (Visible && !Explored)
                Explored = true;
        }
    }
}
