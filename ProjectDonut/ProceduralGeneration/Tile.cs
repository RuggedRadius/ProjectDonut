using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        Winterlands
    }

    public class Tile
    {
        public int xIndex { get; set; }
        public int yIndex { get; set; }
        public Vector2 LocalPosition { get; set; }
        public Vector2 Size { get; set; }
        public Texture2D Texture { get; set; }
        public TileType TileType { get; set; }
        public Biome Biome { get; set; }
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
