using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Core;
using ProjectDonut.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Debugging.Console
{
    public class DevConsole : IScreenObject
    {
        public bool IsShown { get; set; }

        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }

        private Rectangle DisplayRect;
        private Rectangle InputRect;

        private SpriteFont _font;
        private string _currentInput;

        private string[] _history;

        private int _inputLineHeight = 50;
        private int _historyLineHeight = 30;
        private int _windowWidth = 600;

        private float _inputTimer = 0f;
        private float _inputDelay = 0.1f;

        public void Initialize()
        {
            _history = new string[10];
            _currentInput = string.Empty;

            var inputY = Global.ScreenHeight - _inputLineHeight;
            InputRect = new Rectangle(0, inputY, _windowWidth, _inputLineHeight);
            
            var dispHeight = (_history.Length * _historyLineHeight);
            var dispWidth = _windowWidth;
            var dispY = Global.ScreenHeight - _inputLineHeight - dispHeight;
            DisplayRect = new Rectangle(0, dispY, dispWidth, dispHeight);
        }

        public void LoadContent()
        {
            Texture = new Texture2D(Global.GraphicsDevice, 1, 1);
            Color[] colorData = [Color.White];
            Texture.SetData(colorData);

            _font = Global.ContentManager.Load<SpriteFont>("Fonts/Default");
        }

        public void Update(GameTime gameTime)
        {
            _inputTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_inputTimer < _inputDelay)
                return;

            var kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.OemTilde))
            {
                IsShown = !IsShown;
                Global.DebugWindow.IsShown = IsShown;
                _inputTimer = 0f;
            }

            if (!IsShown) return;

            for (int i = 65; i < 65 + 91; i++)
            {
                if (kbState.IsKeyDown((Keys)i))
                {
                    _currentInput += (char)i;
                    _inputTimer = 0f;
                }
            }

            if (kbState.IsKeyDown(Keys.Space))
            {
                _currentInput += " ";
                _inputTimer = 0f;
            }

            if (kbState.IsKeyDown(Keys.OemMinus))
            {
                _currentInput += "-";
                _inputTimer = 0f;
            }

            if (kbState.IsKeyDown(Keys.OemPlus))
            {
                _currentInput += "+";
                _inputTimer = 0f;
            }

            if (kbState.IsKeyDown(Keys.Enter))
            {
                _inputTimer = 0f;

                if (string.IsNullOrEmpty(_currentInput))
                    return;
                else
                {
                    ExecuteCommand(_currentInput);
                    AddCommandToHistory(_currentInput);
                    _currentInput = string.Empty;
                }
            }
        }

        public void AddCommandToHistory(string command)
        {
            for (int i = _history.Length - 1; i > 0; i--)
            {
                _history[i] = _history[i - 1];
            }

            _history[0] = command;
        }

        public void Draw(GameTime gameTime)
        {
            if (IsShown)
            {
                Global.SpriteBatch.Begin(transformMatrix: Matrix.Identity);
                Global.SpriteBatch.Draw(Texture, DisplayRect, Color.Black * 0.5f);
                Global.SpriteBatch.Draw(Texture, InputRect, Color.Gray * 0.5f);

                // Input line
                var inputTextPos = new Vector2(InputRect.X + 10, InputRect.Bottom - _inputLineHeight);
                Global.SpriteBatch.DrawString(_font, _currentInput, inputTextPos, Color.White);

                // History lines
                for (int i = _history.Length - 1; i >= 0; i--)
                {
                    if (_history[i] == null)
                        continue;

                    var pos = new Vector2(DisplayRect.X + 10, DisplayRect.Bottom - ( _historyLineHeight * (i + 1)));
                    Global.SpriteBatch.DrawString(_font, _history[i], pos, Color.Chartreuse);
                }

                Global.SpriteBatch.End();
            }
        }

        private void ExecuteCommand(string command)
        {
            var parts = command.Split(' ');

            switch (parts[0])
            {
                case "fog":
                    if (parts.Length < 2)
                        return;
                    if (parts[1] == "on")
                        Global.SHOW_FOG_OF_WAR = true;
                    else if (parts[1] == "off")
                        Global.SHOW_FOG_OF_WAR = false;
                    break;
                default:
                    break;
            }
        }
    }
}
