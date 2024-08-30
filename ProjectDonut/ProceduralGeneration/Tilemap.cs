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
    }
}
