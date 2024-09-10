using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using ProjectDonut.Interfaces;
using ProjectDonut.Tools;
using ProjectDonut.UI.ScrollDisplay;
using IScene = ProjectDonut.Core.SceneManagement.IScene;

namespace ProjectDonut.ProceduralGeneration.World.Structures
{
    public enum WorldStructureType
    {        
        Village,
        Town,
        City,
        Castle,
    }

    public class WorldStructure : ISceneObject
    {
        public string StructureName { get; set; }


        // Properties
        public WorldChunk WorldChunk { get; set; }
        public IScene Instance { get; set; }
        public WorldStructureType StructureType { get; set; }

        // Position
        public Vector2 WorldPosition { get; set; }
        public Vector2 ChunkPosition { get; set; }

        // Bounds
        public Rectangle TextureBounds { get; set; }
        public Rectangle[] EntryBounds { get; set; }
        public Rectangle InteractBounds { get; set; }

        // Drawing
        public int ZIndex { get; set; }
        public Texture2D Texture { get; set; }

        // State
        public bool PlayerWithinScrollBounds { get; set; }
        public bool IsExplored { get; set; }
        public bool IsVisible { get; set; }

        private ScrollDisplay _associatedScrollDisplay;

        private Random _random = new Random();

        public WorldStructure(Vector2 worldPosition, WorldChunk chunk, WorldStructureType structureType)
        {
            WorldChunk = chunk;
            WorldPosition = worldPosition;
            StructureType = structureType;
        }

        public void Initialize()
        {
            switch (StructureType)
            {
                case WorldStructureType.Village:
                    break;

                case WorldStructureType.Town:
                    StructureName = NameGenerator.GenerateRandomName(_random.Next(2, 5));
                    Instance = new TownScene(this);
                    Texture = Global.SpriteLibrary.GetSprite("town");
                    break;

                case WorldStructureType.City: // ??? really needed?
                    break;

                case WorldStructureType.Castle:
                    StructureName = NameGenerator.GenerateRandomName(_random.Next(2, 5));
                    Instance = new DungeonScene();
                    Texture = Global.SpriteLibrary.GetSprite("castle");
                    break;
            }
            
            Instance.Initialize();
            Instance.LoadContent();

            CalculateBounds();
        }

        public void LoadContent()
        {
        }

        private void CalculateBounds()
        {
            switch (StructureType)
            {
                case WorldStructureType.Village:
                    break;

                case WorldStructureType.Town:
                    CalculateTownBounds();
                    break;

                case WorldStructureType.City: // ??? really needed?
                    break;

                case WorldStructureType.Castle:
                    CalculateCastleBounds();
                    break;
            }
        }

        private void CalculateTownBounds()
        {
            // Chunk position
            var chunkPosX = WorldPosition.X - (WorldChunk.ChunkCoordX * Global.ChunkSize * Global.TileSize);
            var chunkPosY = WorldPosition.Y - (WorldChunk.ChunkCoordY * Global.ChunkSize * Global.TileSize);
            ChunkPosition = new Vector2(chunkPosX, chunkPosY);

            // Texture Rect
            TextureBounds = new Rectangle(
                (int)WorldPosition.X,
                (int)WorldPosition.Y,
                Texture.Width,
                Texture.Height);

            // Entry Rect
            EntryBounds = CreateEntryBounds();

            // Scroll Rect
            var bufferZoneSize = 2 * Global.TileSize;
            InteractBounds = new Rectangle(
                (int)WorldPosition.X - bufferZoneSize,
                (int)WorldPosition.Y - bufferZoneSize,
                Texture.Width + bufferZoneSize * 2,
                Texture.Height + bufferZoneSize * 2);
        }

        private Rectangle[] CreateEntryBounds()
        {
            var entries = new Rectangle[4];

            // North
            var entryRectX = WorldPosition.X;// + (Texture.Width / 2) - (Global.TileSize / 2);
            var entryRectY = WorldPosition.Y;
            entries[0] = new Rectangle(
                (int)entryRectX,
                (int)entryRectY,
                Global.TileSize * 3,
                Global.TileSize);

            // East
            entryRectX = WorldPosition.X + Texture.Width - Global.TileSize;
            entryRectY = WorldPosition.Y;// + (Texture.Height / 2) - (Global.TileSize / 2);
            entries[1] = new Rectangle(
                (int)entryRectX,
                (int)entryRectY,
                Global.TileSize,
                Global.TileSize * 3);

            // South
            entryRectX = WorldPosition.X;// + (Texture.Width / 2) - (Global.TileSize / 2);
            entryRectY = WorldPosition.Y + Texture.Height - Global.TileSize;
            entries[2] = new Rectangle(
                (int)entryRectX,
                (int)entryRectY,
                Global.TileSize * 3,
                Global.TileSize);

            // West
            entryRectX = WorldPosition.X;
            entryRectY = WorldPosition.Y;// + (Texture.Height / 2) - (Global.TileSize / 2);
            entries[3] = new Rectangle(
                (int)entryRectX,
                (int)entryRectY,
                Global.TileSize,
                Global.TileSize * 3);

            return entries;
        }

        private void CalculateCastleBounds()
        {
            // Chunk position
            var chunkPosX = WorldPosition.X - (WorldChunk.ChunkCoordX * Global.ChunkSize * Global.TileSize);
            var chunkPosY = WorldPosition.Y - (WorldChunk.ChunkCoordY * Global.ChunkSize * Global.TileSize);
            ChunkPosition = new Vector2(chunkPosX, chunkPosY);

            // Texture Rect
            TextureBounds = new Rectangle(
                (int)WorldPosition.X,
                (int)WorldPosition.Y,
                Texture.Width,
                Texture.Height);

            // Entry Rect
            EntryBounds = new Rectangle[1];
            var entryRectX = WorldPosition.X + (4 * Global.TileSize);
            var entryRectY = WorldPosition.Y + (8 * Global.TileSize);
            EntryBounds[0] = new Rectangle(
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
        }

        public void Update(GameTime gameTime)
        {
            if (Global.SceneManager.CurrentScene is WorldScene)
            {
                foreach (var entry in EntryBounds)
                {
                    if (entry.Contains(Global.PlayerObj.WorldPosition.X, Global.PlayerObj.WorldPosition.Y))
                    {
                        var worldScene = (WorldScene)Global.SceneManager.CurrentScene;

                        var worldExitPointX = (entry.Width / 2) - Global.TileSize + Global.PlayerObj.WorldPosition.X;
                        var worldExitPointY = entry.Bottom + Global.TileSize;
                        worldScene.LastExitLocation = new Rectangle((int)worldExitPointX, (int)worldExitPointY, Global.TileSize, Global.TileSize);

                        Global.ScrollDisplay.ClearAllScrolls();
                        Global.SceneManager.SetCurrentScene(Instance);
                        Global.SceneManager.CurrentScene.PrepareForPlayerEntry();

                        if (StructureType != WorldStructureType.Town)
                        {
                            break;
                        }

                        if (entry == EntryBounds[0]) // North
                        {
                            var xPos = 50 * Global.TileSize;
                            var yPos = 1 * Global.TileSize;
                            Global.PlayerObj.WorldPosition = new Vector2(xPos, yPos);
                        }
                        else if (entry == EntryBounds[1]) // East
                        {
                            var xPos = (100 * Global.TileSize) - (1 * Global.TileSize);
                            var yPos = 50 * Global.TileSize;
                            Global.PlayerObj.WorldPosition = new Vector2(xPos, yPos);
                        }
                        else if (entry == EntryBounds[2]) // South
                        {
                            var xPos = 50 * Global.TileSize;
                            var yPos = (100 * Global.TileSize) - (1 * Global.TileSize);
                            Global.PlayerObj.WorldPosition = new Vector2(xPos, yPos);
                        }
                        else if (entry == EntryBounds[3]) // West
                        {
                            var xPos = 1 * Global.TileSize;
                            var yPos = 50 * Global.TileSize;
                            Global.PlayerObj.WorldPosition = new Vector2(xPos, yPos);
                        }
                    }
                }

                if (Global.WorldChunkManager.CurrentChunks.Contains(WorldChunk))
                {
                    if (InteractBounds.Contains(Global.GameCursor.CursorWorldPosition))
                    {
                        if (_associatedScrollDisplay == null)
                        {
                            _associatedScrollDisplay = new ScrollDisplay()
                            {
                                Text = StructureName,
                                SubText = StructureType.ToString(),
                                IsTimed = false,
                                ScrollOutDuration = 1f,
                                ShowDuration = 5f
                            };

                            Global.ScrollDisplay.DisplayScroll(_associatedScrollDisplay);
                        }
                    }
                    else
                    {
                        if (_associatedScrollDisplay != null)
                        {
                            Global.ScrollDisplay.HideScroll(_associatedScrollDisplay);
                            _associatedScrollDisplay = null;
                        }
                    }
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
                Math.Abs(Vector2.Distance(Global.PlayerObj.WorldPosition, new Vector2(TextureBounds.Left, TextureBounds.Top))),
                Math.Abs(Vector2.Distance(Global.PlayerObj.WorldPosition, new Vector2(TextureBounds.Right, TextureBounds.Top))),
                Math.Abs(Vector2.Distance(Global.PlayerObj.WorldPosition, new Vector2(TextureBounds.Right, TextureBounds.Bottom))),
                Math.Abs(Vector2.Distance(Global.PlayerObj.WorldPosition, new Vector2(TextureBounds.Left, TextureBounds.Bottom)))
            ];

            if (distances.Min() <= (Global.FOG_OF_WAR_RADIUS))
                return true;
            else
                return false;
        }

        public void Draw(GameTime gameTime)
        {
            //Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());

            if (Global.DRAW_STRUCTURE_DEBUG)
            {
                Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, InteractBounds, Color.Red * 0.25f);

                foreach (var entry in EntryBounds)
                {
                    Global.SpriteBatch.Draw(Global.DEBUG_TEXTURE, entry, Color.White * 0.5f);
                }                
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
                Global.SpriteBatch.Draw(Texture, WorldPosition, Color.White);
            }



            //Global.SpriteBatch.End();
        }
    }
}
