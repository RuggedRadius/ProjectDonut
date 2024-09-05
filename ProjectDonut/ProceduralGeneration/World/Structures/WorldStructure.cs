using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World.Generators;
using ProjectDonut.UI.ScrollDisplay;

namespace ProjectDonut.ProceduralGeneration.World.Structures
{
    public class WorldStructure : ISceneObject
    {
        public string Name { get; set; }

        public WorldChunk WorldChunk { get; set; }
        public InstanceScene Instance { get; set; }

        public Vector2 WorldPosition { get; set; }
        public Vector2 ChunkPosition { get; set; }
        public Vector2 Position { get; set; } // NOT USED?? - HMM not liking this, should more specific, world, local, chunk or whatever

        public Rectangle TextureBounds { get; set; }
        public Rectangle EntryBounds { get; set; }
        public Rectangle ScrollBounds { get; set; }

        public int ZIndex { get; set; }
        public Texture2D Texture { get; set; }

        public bool PlayerWithinScrollBounds { get; set; }


        public WorldStructure(Vector2 chunkPosition, WorldChunk chunk)
        {
            WorldChunk = chunk;
            ChunkPosition = chunkPosition;
        }

        public void Initialize()
        {
            // World Position
            var worldPositionX = (WorldChunk.ChunkCoordX * Global.ChunkSize * Global.TileSize) + ChunkPosition.X;
            var worldPositionY = (WorldChunk.ChunkCoordY * Global.ChunkSize * Global.TileSize) + ChunkPosition.Y;
            WorldPosition = new Vector2(worldPositionX, worldPositionY);

            // Entry Rect - MAY BE WRONG
            var entryRectX = WorldPosition.X + (4 * Global.TileSize);
            var entryRectY = WorldPosition.Y + (8 * Global.TileSize);
            EntryBounds = new Rectangle(
                (int)entryRectX, 
                (int)entryRectY, 
                Global.TileSize * 1,
                Global.TileSize * 1);

            // Scroll Rect
            var bufferZoneSize = 2 * Global.TileSize;
            ScrollBounds = new Rectangle(
                (int)WorldPosition.X - bufferZoneSize,
                (int)WorldPosition.Y - bufferZoneSize,
                Texture.Width + bufferZoneSize * 2,
                Texture.Height + bufferZoneSize * 2);
        }

        public void Update(GameTime gameTime)
        {
            if (EntryBounds.Contains(Global.Player.Position.X, Global.Player.Position.Y))
            {
                var worldScene = (WorldScene)Global.SceneManager.CurrentScene;

                var worldExitPointX = (EntryBounds.Width / 2) + EntryBounds.X + (Global.Player.ChunkPosX * Global.TileSize * WorldChunk.Width);
                var worldExitPointY = EntryBounds.Bottom + Global.TileSize + (Global.Player.ChunkPosY * Global.TileSize * WorldChunk.Height);
                worldScene.LastExitLocation = new Rectangle(worldExitPointX, worldExitPointY, Global.TileSize, Global.TileSize);

                Global.SceneManager.SetCurrentScene(Instance, SceneType.Instance);
                Global.SceneManager.CurrentScene.PrepareForPlayerEntry();
            }

            if (ScrollBounds.Contains(Global.Player.Position))
            {
                PlayerWithinScrollBounds = true;

                if (ScrollDisplayer.CurrentStructure != this)
                {
                    Global.ScrollDisplay.HideScroll();
                    ScrollDisplayer.CurrentStructure = this;
                }
            }
            else
            {
                PlayerWithinScrollBounds = false;
            }
        }

        public void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            Global.SpriteBatch.Draw(Texture, WorldPosition, Color.White);

            if (Global.DRAW_STRUCTURE_ENTRY_OUTLINE)
            {
                Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, ScrollBounds, Color.Red * 0.5f);
                Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, EntryBounds, Color.White * 0.5f);
            }

            Global.SpriteBatch.End();
        }

        // **** BEWARE: THIS IS VERY BROKEN ***
        private void HandleScrollDisplay()
        {

        }
    }
}
