﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.Interfaces;

namespace ProjectDonut.Environment
{
    public class DayNightCycle : IGameObject
    {
        private float timeOfDay = 0f; // Start at midnight
        private float timeSpeed = 1f; // Speed of time (how fast time passes)

        public bool IsVisible { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 WorldPosition { get; set; }
        public int ZIndex { get; set; }

        private Rectangle DebugRect;
        private Rectangle TintRect;

        private float TargetFOW;
        private float TargetLightScale;

        public void Initialize()
        {
            DebugRect = new Rectangle(
                Global.ScreenWidth - 200,
                0,
                200,
                50
                );

            TintRect = new Rectangle(0, 0, Global.ScreenWidth, Global.ScreenHeight);
        }

        public void LoadContent()
        {
            Texture = new Texture2D(Global.GraphicsDevice, 1, 1);
            Texture.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            // Update the time of day based on timeSpeed
            timeOfDay += timeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Keep timeOfDay within a 24-hour range
            if (timeOfDay >= 24f)
                timeOfDay -= 24f;


            if (Global.SceneManager.CurrentScene is WorldScene)
            {
                if (timeOfDay < 6f || timeOfDay >= 20f)
                {
                    TargetFOW = 500;
                    TargetLightScale = 1000f;
                }
                else
                {
                    TargetFOW = 1500;
                    TargetLightScale = 3000f;
                }
            }
            else
            {
                TargetFOW = 1500;
                TargetLightScale = 3000f;
            }

            if (TargetFOW < Global.FOG_OF_WAR_RADIUS)
            {
                Global.FOG_OF_WAR_RADIUS -= 10;
            }
            else if (TargetFOW > Global.FOG_OF_WAR_RADIUS)
            {
                Global.FOG_OF_WAR_RADIUS += 10;
            }

            if (TargetLightScale < Global.Player.Light.Scale.X)
            {
                Global.Player.Light.Scale = new Vector2(Global.Player.Light.Scale.X - 10);
            }
            else if (TargetLightScale > Global.Player.Light.Scale.X)
            {
                Global.Player.Light.Scale = new Vector2(Global.Player.Light.Scale.X + 10);
            }

        }

        public void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Begin();

            // Apply time-of-day tint to your environment rendering
            if (Global.SceneManager.CurrentScene is WorldScene)
            {
                Color timeOfDayTint = GetTimeOfDayColor();
                Global.SpriteBatch.Draw(Texture, TintRect, timeOfDayTint * 0.15f);
                Global.Penumbra.AmbientColor = timeOfDayTint;
            }

            Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, DebugRect, Color.Black);
            Global.SpriteBatch.DrawString(Global.FontDebug, $"{ConvertFloatToTime(timeOfDay)}", new Vector2(DebugRect.X + 10, DebugRect.Y + 10), Color.White);

            // Draw the rest of your game as normal (characters, objects, etc.)
            // They could also use `timeOfDayTint` if you want everything tinted.

            Global.SpriteBatch.End();
        }

        private string ConvertFloatToTime(float hours)
        {
            // Clamp the value to the 0-24 range
            if (hours < 0.0f) hours = 0.0f;
            if (hours > 24.0f) hours = 24.0f;

            // Calculate the hours and minutes
            int hour = (int)hours; // Get the integer part as hours
            int minutes = (int)((hours - hour) * 60); // Get the fractional part as minutes

            // Adjust for 12-hour format and determine AM/PM
            string suffix = (hour >= 12 && hour < 24) ? "PM" : "AM";
            if (hour == 0) hour = 12; // Midnight is 12 AM
            else if (hour > 12) hour -= 12; // Convert to 12-hour format

            // Format the time string
            return $"{hour:D2}:{minutes:D2} {suffix}";
        }


        public Color GetTimeOfDayColor()
        {
            if (timeOfDay >= 6f && timeOfDay < 18f)
            {
                // Daytime: Bright and natural lighting
                return Color.White;
            }
            else if (timeOfDay >= 18f && timeOfDay < 20f)
            {
                // Evening: Darker, more orange/red lighting
                return new Color(255, 200, 150); // Orange tint
            }
            else if (timeOfDay >= 4f && timeOfDay < 6f)
            {
                // Early morning: Slightly blue tint
                return new Color(180, 220, 255); // Light blue tint
            }
            else
            {
                // Night: Dark and bluish
                return new Color(50, 50, 100); // Dark blue tint
            }
        }
    }
}
