using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Interfaces;
using static ProjectDonut.NPCs.Characters.CharacterEnums;

namespace ProjectDonut.NPCs.Characters
{
    public class Character : IGameObject
    {
        public string Name;
        public CharacterRace Race;
        public CharacterProfession Profession;
        public CharacterAttributes Attributes { get; set; }
        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }
        public bool IsVisible { get; set; }
        public Texture2D Texture 
        { 
            get => throw new NotImplementedException(); 
            set => throw new NotImplementedException(); 
        }

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
            UpdateObjectVisibility();
        }

        public void UpdateObjectVisibility()
        {
            float distance = Vector2.Distance(Global.Player.Position, Position);
            IsVisible = (distance <= Global.FOG_OF_WAR_RADIUS) ? true : false;
        }

        public void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
