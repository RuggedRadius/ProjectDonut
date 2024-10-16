﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Interfaces;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class SceneObjectStatic : ISceneObject
    {
        public Vector2 WorldPosition { get; set; }
        public Texture2D Texture { get; set; }
        public int ZIndex { get; set; }

        public int DrawOrder => throw new NotImplementedException();

        public bool IsVisible { get; set; }
        public bool IsExplored { get; set; }
        public Rectangle TextureBounds { get; set; }

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public void Initialize()
        {
            TextureBounds = new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, Texture.Width, Texture.Height);
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
            if (!IsExplored) return;


            if (!IsVisible)
            {
                //Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
                Global.SpriteBatch.Draw(Texture, WorldPosition, Color.Gray);
                //Global.SpriteBatch.End();
            }
            else
            {
                //Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
                Global.SpriteBatch.Draw(Texture, WorldPosition, Color.White);
                //Global.SpriteBatch.End();
            }
        }

        public void UpdateObjectVisibility()
        {
            if (Global.SHOW_FOG_OF_WAR == false)
            {
                IsVisible = true;
                IsExplored = true;
                return;
            }

            float distance = Math.Abs(Vector2.Distance(Global.PlayerObj.WorldPosition, WorldPosition));
            IsVisible = (distance <= Global.FOG_OF_WAR_RADIUS) ? true : false;

            if (IsVisible && !IsExplored)
                IsExplored = true;
        }
    }
}
