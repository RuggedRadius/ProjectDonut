using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.GameObjects;
using ProjectDonut.ProceduralGeneration;

namespace ProjectDonut.UI
{
    public class Dialogue
    {
        public bool IsActive { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Text { get; set; }
    }

    internal class DialogueSystem : GameObject
    {
        private List<Dialogue> _dialogues;

        private SpriteBatch spriteBatch;
        private SpriteLibrary spriteLib;
        private Camera camera;
        private ContentManager ContentManager;

        private SpriteFont dialogueFont;

        private const int TileSize = 32;

        private int charCount = 0;
        private float charTimer = 0f;
        private float charInterval = 0.0125f;

        public DialogueSystem(SpriteLibrary spriteLib, SpriteBatch spriteBatch, Camera camera, ContentManager content)
        {
            this.spriteLib = spriteLib;
            this.spriteBatch = spriteBatch;
            this.camera = camera;
            ContentManager = content;

            ZIndex = -500;
        }

        public override void Initialize()
        {
            _dialogues = new List<Dialogue>();

            base.Initialize();
        }

        public override void LoadContent()
        {
            dialogueFont = ContentManager.Load<SpriteFont>("Fonts/Default");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (_dialogues.Where(x => x.IsActive).Any())
            {
                charTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (charTimer >= charInterval)
                {
                    charCount++;
                    charTimer = 0f;
                }
            }
            else
            {
                charTimer = 0f;
                charCount = 0;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var dialogue in _dialogues)
            {
                if (dialogue.IsActive)
                {
                    DrawDialogueBox(dialogue);
                    DrawDialogueText(dialogue);
                }
            }

            base.Draw(gameTime);
        }

        public void CloseAllDialogues()
        {
            _dialogues.Clear();
            charCount = 0;
            charTimer = 0f;
        }

        public void CreateDialogue(Rectangle rect, string text)
        {
            _dialogues.Add(new Dialogue
            {
                X = rect.X,
                Y = rect.Y,
                Width = rect.Width,
                Height = rect.Height,
                Text = text,
                IsActive = true
            });
        }

        private void DrawDialogueBox(Dialogue dialogue)
        {
            for (int i = 0; i < dialogue.Height; i++)
            {
                if (i == 0)
                {
                    // Top Left
                    var x = camera.Position.X + (dialogue.X);
                    var y = camera.Position.Y + (dialogue.Y);
                    spriteBatch.Draw(spriteLib.GetSprite("dialogue-NW"), new Vector2(x, y), Color.White);

                    // Top-Middle
                    for (int j = 1; j < dialogue.Width - 1; j++)
                    {
                        x = camera.Position.X + (dialogue.X + (j * TileSize));
                        y = camera.Position.Y + (dialogue.Y + (i * TileSize));
                        spriteBatch.Draw(spriteLib.GetSprite("dialogue-N"), new Vector2(x, y), Color.White);
                    }

                    // Top-Right
                    x = camera.Position.X + (dialogue.X + ((dialogue.Width - 1) * TileSize));
                    y = camera.Position.Y + (dialogue.Y + i);
                    spriteBatch.Draw(spriteLib.GetSprite("dialogue-NE"), new Vector2(x, y), Color.White);
                }
                else if (i == dialogue.Height - 1)
                {
                    // Bottom Left
                    var x = camera.Position.X + (dialogue.X);
                    var y = camera.Position.Y + (dialogue.Y + (i * TileSize));
                    spriteBatch.Draw(spriteLib.GetSprite("dialogue-SW"), new Vector2(x, y), Color.White);

                    // Bottom-Middle
                    for (int j = 1; j < dialogue.Width - 1; j++)
                    {
                        x = camera.Position.X + (dialogue.X + (j * TileSize));
                        y = camera.Position.Y + (dialogue.Y + (i * TileSize));
                        spriteBatch.Draw(spriteLib.GetSprite("dialogue-S"), new Vector2(x, y), Color.White);
                    }

                    // Bottom-Right
                    x = camera.Position.X + (dialogue.X + ((dialogue.Width - 1) * TileSize));
                    y = camera.Position.Y + (dialogue.Y + (i * TileSize));
                    spriteBatch.Draw(spriteLib.GetSprite("dialogue-SE"), new Vector2(x, y), Color.White);
                }
                else
                {
                    // Middle Left
                    var x = camera.Position.X + (dialogue.X);
                    var y = camera.Position.Y + (dialogue.Y + (i * TileSize));
                    spriteBatch.Draw(spriteLib.GetSprite("dialogue-W"), new Vector2(x, y), Color.White);

                    // Middle Middle
                    for (int j = 1; j < dialogue.Width - 1; j++)
                    {
                        x = camera.Position.X + (dialogue.X + (j * TileSize));
                        y = camera.Position.Y + (dialogue.Y + (i * TileSize));
                        spriteBatch.Draw(spriteLib.GetSprite("dialogue-C"), new Vector2(x, y), Color.White);
                    }

                    // Middle Right
                    x = camera.Position.X + (dialogue.X + ((dialogue.Width - 1) * TileSize));
                    y = camera.Position.Y + (dialogue.Y + (i * TileSize));
                    spriteBatch.Draw(spriteLib.GetSprite("dialogue-E"), new Vector2(x, y), Color.White);
                }
            }
        }

        private void DrawDialogueText(Dialogue dialogue)
        {
            var x = camera.Position.X + (dialogue.X + TileSize);
            var y = camera.Position.Y + (dialogue.Y + TileSize);

            for (int j = 0; j < charCount; j++)
            {
                if (j < dialogue.Text.Length)
                {
                    if (x >= camera.Position.X + (dialogue.X + ((dialogue.Width - 1) * TileSize)))
                    {
                        x = camera.Position.X + (dialogue.X + TileSize);
                        y += 25;
                    }

                    spriteBatch.DrawString(dialogueFont, dialogue.Text[j].ToString(), new Vector2(x, y), Color.White);
                    x += TileSize / 2;
                }
            }
        }
    }
}
