using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using ProjectDonut.Interfaces;

namespace ProjectDonut.Environment
{
    public class DayNightCycle : IGameObject
    {
 // Speed of time (how fast time passes)

        public bool IsVisible { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 WorldPosition { get; set; }
        public int ZIndex { get; set; }

        private Rectangle DebugRect;
        private Rectangle TintRect;
        private Vector2 TimeDisplayPosition;

        private float TargetFOW;
        private float TargetLightScale;

        private SpriteFont _fontText;
        private SpriteFont _fontSubText;

        public void Initialize()
        {
            DebugRect = new Rectangle(
                Global.ScreenWidth - 200,
                0,
                200,
                50
                );

            TintRect = new Rectangle(0, 0, Global.ScreenWidth, Global.ScreenHeight);

            TimeDisplayPosition = new Vector2(Global.GraphicsDeviceManager.PreferredBackBufferWidth - 245, 485);
        }

        public void LoadContent()
        {
            Texture = new Texture2D(Global.GraphicsDevice, 1, 1);
            Texture.SetData(new[] { Color.White });

            _fontText = Global.ContentManager.Load<SpriteFont>("Fonts/OldeEnglishDesc");
            _fontSubText = Global.ContentManager.Load<SpriteFont>("Fonts/OldeEnglishDescSubText");

        }

        public void Update(GameTime gameTime)
        {
            // Update the time of day based on Global.timeSpeed
            Global.timeOfDay += Global.timeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Keep timeOfDay within a 24-hour range
            if (Global.timeOfDay >= 24f)
                Global.timeOfDay -= 24f;


            if (Global.SceneManager.CurrentScene is WorldScene)
            {
                if (Global.timeOfDay < 6f || Global.timeOfDay >= 20f)
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

            if (TargetLightScale < Global.PlayerObj.Light.Scale.X)
            {
                Global.PlayerObj.Light.Scale = new Vector2(Global.PlayerObj.Light.Scale.X - 10);
            }
            else if (TargetLightScale > Global.PlayerObj.Light.Scale.X)
            {
                Global.PlayerObj.Light.Scale = new Vector2(Global.PlayerObj.Light.Scale.X + 10);
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

            //Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, DebugRect, Color.Black);
            Global.SpriteBatch.DrawString(_fontSubText, 
                $"{ConvertFloatToTime(Global.timeOfDay)}",
                TimeDisplayPosition, 
                Color.Black);

            // Draw the rest of your game as normal (characters, objects, etc.)
            // They could also use `timeOfDayTint` if you want everything tinted.

            Global.SpriteBatch.End();
        }

        private string ConvertFloatToTime(float hours)
        {
            // Clamp the value to the 0-24 range
            if (hours < 1.0f) hours = 1.0f;
            if (hours > 24.0f) hours = 24.0f;

            // Calculate the hours and minutes
            int hour = (int)hours; // Get the integer part as hours
            //int minutes = (int)((hours - hour) * 60); // Get the fractional part as minutes

            // Adjust for 12-hour format and determine AM/PM
            //string suffix = (hour >= 12 && hour < 24) ? "PM" : "AM";
            //if (hour == 0) hour = 12; // Midnight is 12 AM
            //else 
            if (hour > 24) hour = 1; // Convert to 12-hour format

            // Format the time string
            if (hour % 10 == 1)
            {
                if (hour == 11)
                    return $"{hour}th Hour";
                else
                    return $"{hour}st Hour";
            }
            else if (hour % 10 == 2)
            {
                if (hour == 12)
                    return $"{hour}th Hour";
                else
                    return $"{hour}nd Hour";
            }
            else if (hour % 10 == 3)
            {
                if (hour == 13)
                    return $"{hour}th Hour";
                else
                    return $"{hour}rd Hour";
            }
            else
            {
                return $"{hour}th Hour";
            }


            //return $"{hour:D2}:{minutes:D2} {suffix}";
        }


        public Color GetTimeOfDayColor()
        {
            if (Global.timeOfDay >= 6f && Global.timeOfDay < 18f)
            {
                // Daytime: Bright and natural lighting
                return Color.White;
            }
            else if (Global.timeOfDay >= 18f && Global.timeOfDay < 20f)
            {
                // Evening: Darker, more orange/red lighting
                return new Color(255, 200, 150); // Orange tint
            }
            else if (Global.timeOfDay >= 4f && Global.timeOfDay < 6f)
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
