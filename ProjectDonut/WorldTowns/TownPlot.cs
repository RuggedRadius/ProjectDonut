using Microsoft.Xna.Framework;

namespace ProjectDonut.WorldTowns
{
    public class TownPlot
    {
        public Vector2 WorldPosition { get; set; }
        public Vector2 LocalPosition { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public TownBuilding Building { get; set; }

        public Rectangle LocalBounds
        {
            get
            {
                return new Rectangle((int)LocalPosition.X, (int)LocalPosition.Y, Width, Height);
            }
        }

        public Rectangle WorldBounds
        {
            get
            {
                return new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, Width, Height);
            }
        }
    }
}
