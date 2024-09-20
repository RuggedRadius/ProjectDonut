using Microsoft.Xna.Framework;
using ProjectDonut.Core;
using System;

namespace ProjectDonut.Tools
{
    public static class PositionTools
    {
        public static (int, int) ConvertWorldPositionToChunkCoords(Vector2 worldPosition)
        {
            //var x = (int)(worldPosition.X / (Global.TileSize * Global.ChunkSize));
            //var y = (int)(worldPosition.Y / (Global.TileSize * Global.ChunkSize));

            //return (x, y);

            //// Size of each chunk in tiles (50x50)
            //int chunkSizeInTiles = 50;

            //// Size of each tile in pixels (32x32)
            //int tileSizeInPixels = 32;

            // Calculate the size of the chunk in pixels
            int chunkSizeInPixels = Global.ChunkSize * Global.TileSize;

            // Calculate chunk coordinates by dividing world position by chunk size in pixels
            int chunkX = (int)Math.Floor(worldPosition.X / chunkSizeInPixels);
            int chunkY = (int)Math.Floor(worldPosition.Y / chunkSizeInPixels);

            return (chunkX, chunkY);
        }
    }
}
