using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace ProjectDonut.ProceduralGeneration
{
    public class Tilemap
    {
        public Tile[,] Map { get; set; }
        public Vector2 WorldPosition { get; set; }

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

        public void UpdateDrawValues(GameTime gameTime)
        {

           foreach (var tile in Map)
           {
                if (tile == null)
                    continue;

                tile.UpdateDrawValues();
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

        public void DrawThumbnail(GameTime gameTime)
        {
            foreach (var tile in Map)
            {
                if (tile == null)
                    continue;

                tile.DrawThumbnail(gameTime);
            }
        }

        public void DrawOutline(GameTime gameTime)
        {
            for (var x = 0; x < Map.GetLength(0); x++)
            {
                var xStart = WorldPosition.X + (x * Global.TileSize);
                var yStart = WorldPosition.Y;
                var xEnd = WorldPosition.X + (x * Global.TileSize);
                var yEnd = WorldPosition.Y + (Map.GetLength(1) * Global.TileSize);

                Global.SpriteBatch.DrawLine(
                    new Vector2(xStart, yStart),
                    new Vector2(xEnd, yEnd),
                    Color.Magenta,
                    1);
            }

            for (var y = 0; y < Map.GetLength(0); y++)
            {
                var xStart = WorldPosition.X;
                var yStart = WorldPosition.Y + (y * Global.TileSize);
                var xEnd = WorldPosition.X + (Map.GetLength(0) * Global.TileSize);
                var yEnd = WorldPosition.Y + (y * Global.TileSize);

                Global.SpriteBatch.DrawLine(
                    new Vector2(xStart, yStart),
                    new Vector2(xEnd, yEnd),
                    Color.Magenta,
                    1);
            }
        }


    }
}
