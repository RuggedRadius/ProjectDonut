using System.Collections.Generic;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
using ProjectDonut.GameObjects;
using ProjectDonut.Interfaces;

namespace ProjectDonut.ProceduralGeneration
{
    public enum TileType
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
        public TileType TileType { get; set; }
        public Biome Biome { get; set; }
        private SpriteBatch _spriteBatch;

        public bool IsAnimated { get; set; }
        public List<Texture2D> Frames { get; set; }
        private double _frameTimer { get; set; }
        private double _frameInterval { get; set; }
        private int _frameIndex { get; set; }
        public Vector2 Position { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int ZIndex { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

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

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var x = (ChunkX * 100 * 32) + (LocalPosition.X);
            var y = (ChunkY * 100 * 32) + (LocalPosition.Y);
            var position = new Vector2(x, y);
            _spriteBatch.Draw(Texture, position, null, Color.White);
        }

        public void Initialize()
        {
        }

        public void LoadContent(ContentManager content)
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

    public class Tilemap
    {
        public Tile[,] Map { get; set; }

        public Tilemap(int width, int height) 
        { 
            Map = new Tile[width, height];
        }

        public Tile GetTile(int x, int y)
        {
            return Map[x, y];
        }
    }
}
