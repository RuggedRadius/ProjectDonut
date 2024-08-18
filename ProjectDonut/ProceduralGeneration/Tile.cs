using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
using ProjectDonut.GameObjects;

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

        public Tile(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

        public void Draw(GameTime gameTime)
        {
            var x = (ChunkX * 100 * 32) + (LocalPosition.X);
            var y = (ChunkY * 100 * 32) + (LocalPosition.Y);
            var position = new Vector2(x, y);
            _spriteBatch.Draw(Texture, position, null, Color.White);
        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
        }
    }

    public class AnimatedTile : Tile
    {
        public int Frame { get; set; }
        public int FrameCount { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public int FrameSpeed { get; set; }
        public List<Texture2D> Frames { get; set; }

        public AnimatedTile(SpriteBatch spriteBatch) : base(spriteBatch)
        {

        }

        public void Update(GameTime gameTime)
        {
            Frame++;
            if (Frame >= FrameCount)
            {
                Frame = 0;
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
