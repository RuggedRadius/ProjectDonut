using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.GameObjects;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration;

namespace ProjectDonut.UI.DialogueSystem
{
    internal class DialogueManager : IScreenObject
    {
        private List<Dialogue> _dialogues;

        private SpriteBatch _spriteBatch;
        private SpriteLibrary spriteLib;
        private Camera camera;
        private ContentManager ContentManager;

        private SpriteFont dialogueFont;

        private const int TileSize = 32;

        private int charCount = 0;
        private float charTimer = 0f;
        private float charInterval = 0.0125f;

        public int ZIndex { get; set; }
        public Vector2 Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public DialogueManager(SpriteLibrary spriteLib, SpriteBatch spriteBatch, Camera camera, ContentManager content)
        {
            this.spriteLib = spriteLib;
            this._spriteBatch = spriteBatch;
            this.camera = camera;
            ContentManager = content;

            ZIndex = -500;
        }

        public void Initialize()
        {
            _dialogues = new List<Dialogue>();
        }

        public void LoadContent()
        {
            dialogueFont = ContentManager.Load<SpriteFont>("Fonts/Default");
            //dialogueFont = ContentManager.Load<SpriteFont>("Fonts/OldeEnglishDesc");
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < _dialogues.Count; i++)
            {
                var timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                var d = _dialogues[i];
                d.CharTimer += timeElapsed;
                d.ShowTimer += timeElapsed;

                if (charTimer >= charInterval)
                {
                    d.CharCounter++;
                    d.CharTimer = 0f;
                }

                if (d.ShowTimer >= d.ShowTime)
                {
                    _dialogues.Remove(d);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(transformMatrix: Matrix.Identity);

            for (int i = 0; i < _dialogues.Count; i++)
            {
                var dialogue = _dialogues[i];
                if (dialogue.IsActive)
                {
                    DrawDialogueBox(dialogue);
                    DrawDialogueText(dialogue);
                }
            }
            _spriteBatch.End();
        }

        public void CloseAllDialogues()
        {
            _dialogues.Clear();
            charCount = 0;
            charTimer = 0f;
        }

        public Dialogue CreateDialogue(Rectangle rect, string text, float time)
        {
            return new Dialogue
            {
                X = rect.X,
                Y = rect.Y,
                Width = rect.Width,
                Height = rect.Height,
                Text = text,
                IsActive = true,
                ShowTime = time
            };
        }

        private void DrawDialogueBox(Dialogue dialogue)
        {
            for (int i = 0; i < dialogue.Height; i++)
            {
                if (i == 0)
                {
                    // Top Left
                    var x = dialogue.X;
                    var y = dialogue.Y;
                    _spriteBatch.Draw(spriteLib.GetSprite("dialogue-NW"), new Vector2(x, y), Color.White);

                    // Top-Middle
                    for (int j = 1; j < dialogue.Width - 1; j++)
                    {
                        x = (dialogue.X + j * TileSize);
                        y = (dialogue.Y + i * TileSize);
                        _spriteBatch.Draw(spriteLib.GetSprite("dialogue-N"), new Vector2(x, y), Color.White);
                    }

                    // Top-Right
                    x = (dialogue.X + (dialogue.Width - 1) * TileSize);
                    y = (dialogue.Y + i);
                    _spriteBatch.Draw(spriteLib.GetSprite("dialogue-NE"), new Vector2(x, y), Color.White);
                }
                else if (i == dialogue.Height - 1)
                {
                    // Bottom Left
                    var x = dialogue.X;
                    var y = (dialogue.Y + i * TileSize);
                    _spriteBatch.Draw(spriteLib.GetSprite("dialogue-SW"), new Vector2(x, y), Color.White);

                    // Bottom-Middle
                    for (int j = 1; j < dialogue.Width - 1; j++)
                    {
                        x = (dialogue.X + j * TileSize);
                        y = (dialogue.Y + i * TileSize);
                        _spriteBatch.Draw(spriteLib.GetSprite("dialogue-S"), new Vector2(x, y), Color.White);
                    }

                    // Bottom-Right
                    x = (dialogue.X + (dialogue.Width - 1) * TileSize);
                    y = (dialogue.Y + i * TileSize);
                    _spriteBatch.Draw(spriteLib.GetSprite("dialogue-SE"), new Vector2(x, y), Color.White);
                }
                else
                {
                    // Middle Left
                    var x = dialogue.X;
                    var y = (dialogue.Y + i * TileSize);
                    _spriteBatch.Draw(spriteLib.GetSprite("dialogue-W"), new Vector2(x, y), Color.White);

                    // Middle Middle
                    for (int j = 1; j < dialogue.Width - 1; j++)
                    {
                        x = (dialogue.X + j * TileSize);
                        y = (dialogue.Y + i * TileSize);
                        _spriteBatch.Draw(spriteLib.GetSprite("dialogue-C"), new Vector2(x, y), Color.White);
                    }

                    // Middle Right
                    x = (dialogue.X + (dialogue.Width - 1) * TileSize);
                    y = (dialogue.Y + i * TileSize);
                    _spriteBatch.Draw(spriteLib.GetSprite("dialogue-E"), new Vector2(x, y), Color.White);
                }
            }
        }

        private void DrawDialogueText(Dialogue dialogue)
        {
            var x = (dialogue.X + TileSize);
            var y = (dialogue.Y + TileSize);

            for (int j = 0; j < dialogue.Text.Length; j++)
            {
                if (j < dialogue.Text.Length)
                {
                    if (x >= (dialogue.X + (dialogue.Width - 1) * TileSize))
                    {
                        x = (dialogue.X + TileSize);
                        y += 25;
                    }

                    _spriteBatch.DrawString(dialogueFont, dialogue.Text[j].ToString(), new Vector2(x, y), Color.White);
                    x += TileSize / 2;
                }
            }
        }

        public List<Dialogue> CreateTestDialogue()
        {
            var width = 22;
            var height = 3;
            var startX = -(width * 32) / 2;
            var startY = -(height * 32) / 2;
            startX += (camera._graphicsDevice.Viewport.Width / 2);
            startY += (camera._graphicsDevice.Viewport.Height / 2);
            var rect = new Rectangle(startX, startY, width, height);

            var lines = new List<Dialogue>()
            {
                CreateDialogue(rect, "Hello, welcome to Flandaria! A place of nonsense and whimsical adventure!", 3000f),
                CreateDialogue(rect, "I hope you enjoy your stay!", 3000f),
                CreateDialogue(rect, "Goodbye!", 3000f),
            };

            return lines;
        }

        public void ExecuteMultipleLines(List<Dialogue> lines)
        {
            foreach (var line in lines)
            {
                _dialogues.Add(line);
                Thread.Sleep((int)line.ShowTime);
                CloseAllDialogues();
            }
        }
    }
}
