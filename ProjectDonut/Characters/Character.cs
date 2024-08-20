using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Interfaces;
using static ProjectDonut.Characters.CharacterEnums;

namespace ProjectDonut.Characters
{
    public class Character : IGameObject
    {
        public string Name;
        public CharacterRace Race;
        public CharacterProfession Profession;
        public CharacterAttributes Attributes { get; set; }
        public Vector2 Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ZIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private Texture2D spriteHead;
        private Texture2D spriteFeet;
        private Texture2D spriteArms;
        private Texture2D spriteBody;
        private Texture2D spriteOutfit;

        private ContentManager _content;
        private SpriteBatch _spriteBatch;

        public Character(string name, CharacterRace race, CharacterProfession profession, CharacterAttributes attributes, ContentManager content, SpriteBatch spriteBatch)
        {
            Name = name;
            Race = race;
            Profession = profession;
            Attributes = attributes;

            _content = content;
            _spriteBatch = spriteBatch;
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void LoadContent()
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
