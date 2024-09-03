using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Core.SceneManagement
{
    public class DungeonScene : InstanceScene
    {
        public DungeonLevel[] Levels;

        public DungeonScene(int levelCount) : base()
        {
            Levels = new DungeonLevel[levelCount];

            for (int i = 0; i < levelCount; i++)
            {
                Levels[i] = new DungeonLevel();
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            for (int i = 0; i < Levels.Length; i++)
            {
                Levels[i].Initialize();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int i = 0; i < Levels.Length; i++)
            {
                Levels[i].Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            for (int i = 0; i < Levels.Length; i++)
            {
                Levels[i].Draw(gameTime, spriteBatch);
            }
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            for (int i = 0; i < Levels.Length; i++)
            {
                Levels[i].LoadContent(content);
            }
        }
    }
}
