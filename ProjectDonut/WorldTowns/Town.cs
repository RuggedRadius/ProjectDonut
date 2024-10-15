using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.ProceduralGeneration;
using ProjectDonut.ProceduralGeneration.World;

namespace ProjectDonut.WorldTowns
{
    public class Town
    {
        public Vector2 CenterWorldPosition { get; set; }
        public Vector2 CenterLocalPosition { get; set; }

        public List<TownPlot> Plots { get; set; }

        public Dictionary<string, Tilemap> Tilemaps { get; set; }

        public WorldChunk Chunk { get; set; }
        public Texture2D RenderedTexture { get; set; }

        public Town(Vector2 centerWorldPosition, Vector2 centerLocalPosition)
        {
            CenterWorldPosition = centerWorldPosition;
            CenterLocalPosition = centerLocalPosition;

            Plots = new List<TownPlot>();
            Tilemaps = new Dictionary<string, Tilemap>();
        }

        public void Update(GameTime gameTime)
        {
            Tilemaps["fences"].Update(gameTime);
            Tilemaps["walls"].Update(gameTime);
            Tilemaps["roofs"].Update(gameTime);
            //foreach (var tilemap in Tilemaps)
            //{
            //    tilemap.Value.Update(gameTime);
            //}
        }

        public void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Draw(RenderedTexture, Chunk.ChunkBounds, Color.White);

            //Tilemaps["base"].Draw(gameTime);
            ////Tilemaps["road"].Draw(gameTime);
            //Tilemaps["fences"].Draw(gameTime);
            //Tilemaps["floor"].Draw(gameTime);
            //Tilemaps["walls"].Draw(gameTime);
            Tilemaps["fences"].Draw(gameTime);
            Tilemaps["walls"].Draw(gameTime);
            Tilemaps["roofs"].Draw(gameTime);

            //foreach (var plot in Plots)
            //{
            //    Global.SpriteBatch.DrawRectangle(plot.WorldBounds, Color.Red, 1);
            //    Global.SpriteBatch.DrawRectangle(plot.Building.WorldBounds, Color.Blue, 1);
            //}
        }
    }
}
