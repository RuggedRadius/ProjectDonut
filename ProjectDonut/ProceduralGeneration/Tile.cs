using System.Collections.Generic;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
using ProjectDonut.Core;
using ProjectDonut.GameObjects;
using ProjectDonut.Interfaces;

namespace ProjectDonut.ProceduralGeneration
{
    public enum WorldTileType
    {        
        Coast,
        Water,
        Ground,
        Mountain,
        Forest
    }

    public enum Biome
    {
        Grasslands,
        Desert,
        Winterlands,
        Plains,
        Wetlands
    }

    public class Tile : IGameObject
    {
        public int ChunkX { get; set; }
        public int ChunkY { get; set; }
        public int xIndex { get; set; }
        public int yIndex { get; set; }
        public Vector2 LocalPosition { get; set; }
        public Vector2 Size { get; set; }
        public Texture2D Texture { get; set; }
        public WorldTileType WorldTileType { get; set; }
        public Biome Biome { get; set; }
        private SpriteBatch _spriteBatch;

        public bool IsAnimated { get; set; }
        public List<Texture2D> Frames { get; set; }
        private double _frameTimer { get; set; }
        private double _frameInterval { get; set; }
        private int _frameIndex { get; set; }
        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }

        public Tile(SpriteBatch spriteBatch, bool isAnimated)
        {
            _spriteBatch = spriteBatch;
            IsAnimated = isAnimated;

            if (IsAnimated)
            {
                _frameInterval = 0.5f;
                _frameIndex = 0;
                _frameTimer = 0f;
            }
        }

        public void Draw(GameTime gameTime)
        {
            var x = (ChunkX * Global.ChunkSize * Global.TileSize) + (LocalPosition.X);
            var y = (ChunkY * Global.ChunkSize * Global.TileSize) + (LocalPosition.Y);
            var position = new Vector2(x, y);

            Global.SpriteBatch.Draw(Texture, position, null, Color.White);
        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            if (Frames == null || Frames.Count == 0)
            {
                return;
            }

            if (IsAnimated)
            {
                _frameTimer += gameTime.ElapsedGameTime.TotalSeconds;

                if (_frameTimer >= _frameInterval)
                {
                    _frameTimer = 0;
                    _frameIndex++;
                    
                    if (_frameIndex >= Frames.Count)
                    {
                        _frameIndex = 0;
                    }

                    Texture = Frames[_frameIndex];
                }
            }
        }
    }
}
