using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.Interfaces;
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
        public Rectangle InteractBounds { get; set; }

        public int ZIndex { get; set; }
        public Texture2D Texture { get; set; }

        public bool PlayerWithinScrollBounds { get; set; }
        public bool IsExplored { get; set; }
        public bool IsVisible { get; set; }
        public Rectangle Bounds { get; set; }

        public WorldStructure(Vector2 worldPosition, WorldChunk chunk)
        {
            WorldChunk = chunk;
            WorldPosition = worldPosition;
        }

        public void Initialize()
        {
            // World Position
            //var worldPositionX = (WorldChunk.ChunkCoordX * Global.ChunkSize * Global.TileSize) + ChunkPosition.X;
            //var worldPositionY = (WorldChunk.ChunkCoordY * Global.ChunkSize * Global.TileSize) + ChunkPosition.Y;
            //WorldPosition = new Vector2(worldPositionX, worldPositionY);

            // Chunk position
            var chunkPosX = WorldPosition.X - (WorldChunk.ChunkCoordX * Global.ChunkSize * Global.TileSize);
            var chunkPosY = WorldPosition.Y - (WorldChunk.ChunkCoordY * Global.ChunkSize * Global.TileSize);
            ChunkPosition = new Vector2(chunkPosX, chunkPosY);

            TextureBounds = new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, Texture.Width, Texture.Height);

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
            InteractBounds = new Rectangle(
                (int)WorldPosition.X - bufferZoneSize,
                (int)WorldPosition.Y - bufferZoneSize,
                Texture.Width + bufferZoneSize * 2,
                Texture.Height + bufferZoneSize * 2);

            Bounds = new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, Texture.Width, Texture.Height);

            Instance.Initialize();
        }

        public void Update(GameTime gameTime)
        {
            if (EntryBounds.Contains(Global.Player.Position.X, Global.Player.Position.Y))
            {
                var worldScene = (WorldScene)Global.SceneManager.CurrentScene;

                var worldExitPointX = (EntryBounds.Width / 2) - Global.TileSize + Global.Player.Position.X;
                var worldExitPointY = EntryBounds.Bottom + Global.TileSize;
                worldScene.LastExitLocation = new Rectangle((int)worldExitPointX, (int)worldExitPointY, Global.TileSize, Global.TileSize);

                Global.ScrollDisplay.HideScroll();
                Global.SceneManager.SetCurrentScene(Instance, SceneType.Instance);
                Global.SceneManager.CurrentScene.PrepareForPlayerEntry();
            }

            if (Global.SceneManager.CurrentScene is WorldScene && Global.WorldChunkManager.PlayerChunk == WorldChunk)
            {
                //if (ScrollBounds.Contains(Global.Player.Position))
                if (InteractBounds.Contains(Global.GameCursor.CursorWorldPosition))
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

            UpdateObjectVisibility();
        }

        public void UpdateObjectVisibility()
        {
            if (Global.SHOW_FOG_OF_WAR == false)
            {
                IsVisible = true;
                IsExplored = true;
                return;
            }


            IsVisible = IsStructureVisible();

            if (IsVisible && !IsExplored)
                IsExplored = true;
        }

        private bool IsStructureVisible()
        {
            float[] distances = 
            [
                Math.Abs(Vector2.Distance(Global.Player.Position, new Vector2(TextureBounds.Left, TextureBounds.Top))),
                Math.Abs(Vector2.Distance(Global.Player.Position, new Vector2(TextureBounds.Right, TextureBounds.Top))),
                Math.Abs(Vector2.Distance(Global.Player.Position, new Vector2(TextureBounds.Right, TextureBounds.Bottom))),
                Math.Abs(Vector2.Distance(Global.Player.Position, new Vector2(TextureBounds.Left, TextureBounds.Bottom)))
            ];

            if (distances.Min() <= (Global.FOG_OF_WAR_RADIUS))
                return true;
            else
                return false;
        }

        public void Draw(GameTime gameTime)
        {
            //Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());

            if (Global.DRAW_STRUCTURE_ENTRY_OUTLINE)
            {
                Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, InteractBounds, Color.Red * 0.25f);
                //Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, EntryBounds, Color.White * 0.5f);
            }

            if (!IsExplored)
            {
                return;
            }
            else if (!IsVisible)
            {
                Global.SpriteBatch.Draw(Texture, WorldPosition, Color.Gray);
            }
            else
            {
                //if (Global.DRAW_STRUCTURE_ENTRY_OUTLINE)
                //{
                //    Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, InteractBounds, Color.Red * 0.25f);
                //    Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, EntryBounds, Color.White * 0.5f);
                //}

                Global.SpriteBatch.Draw(Texture, WorldPosition, Color.White);
            }



            //Global.SpriteBatch.End();
        }
    }
}
