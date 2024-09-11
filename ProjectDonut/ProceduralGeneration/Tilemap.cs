using Microsoft.Xna.Framework;

namespace ProjectDonut.ProceduralGeneration
{
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

        public void SetTile(int x, int y, Tile tile)
        {
            Map[x, y] = tile;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var tile in Map)
            {
                if (tile == null)
                    continue;

                tile.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var tile in Map)
            {
                if (tile == null)
                    continue;

                tile.Draw(gameTime);
            }
        }
    }
}
