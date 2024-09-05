using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDonut.NPCs.Characters
{
    public class CharacterGenerator
    {
        private Dictionary<string, Texture2D> headSprites;
        private Dictionary<string, Texture2D> feetSprites;
        private Dictionary<string, Texture2D> armsSprites;
        private Dictionary<string, Texture2D> bodySprites;
        private Dictionary<string, Texture2D> outfitSprites;

        private ContentManager _content;
        private SpriteBatch _spriteBatch;

        public CharacterGenerator(ContentManager content, SpriteBatch spriteBatch)
        {
            _content = content;
            _spriteBatch = spriteBatch;
        }


    }
}
