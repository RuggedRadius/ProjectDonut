using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World;
using ProjectDonut.Tools;

namespace ProjectDonut.ProceduralGeneration
{
    public enum TileType
    {
        World,
        Instance
    }

    public enum DungeonTileType
    {
        None,
        Floor,
        Wall,
        Door
    }

    public class Tile : IGameObject
    {
        // Size and Position
        public int ChunkX { get; set; }
        public int ChunkY { get; set; }
        public int xIndex { get; set; }
        public int yIndex { get; set; }
        public Vector2 LocalPosition { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 WorldPosition
        {
            get
            {
                return new Vector2((ChunkX * Global.ChunkSize * Global.TileSize) + (LocalPosition.X), (ChunkY * Global.ChunkSize * Global.TileSize) + (LocalPosition.Y));
            }
        }
        public int ZIndex { get; set; }
        
        // Attributes
        public TileType TileType { get; set; }
        public DungeonTileType DungeonTileType { get; set; }
        public WorldTileType WorldTileType { get; set; }
        public Biome Biome { get; set; }


        // Visibility and Appearance
        public bool IsVisible { get; set; }
        public bool IsExplored { get; set; }
        public bool IsBlocked{ get; set; }
        public bool IsCollidable { get; set; }

        public Texture2D Texture { get; set; }
        public bool IsPlayerBlocking { get; set; }

        public Rectangle Bounds { get; set; }

        public Tile()
        {
            if (Global.SHOW_FOG_OF_WAR == false)
            {
                IsVisible = true;
                IsExplored = true;
            }

            drawColour = Color.White;
            alphaValue = 1f;
        }

        public void Initialize()
        {
            Bounds = new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, Global.TileSize, Global.TileSize);
        }

        public void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            if (Bounds == Rectangle.Empty)
                Bounds = new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, Global.TileSize, Global.TileSize);

            IsPlayerBlocking = false;
            UpdateObjectVisibility();
            //HandleAnimation(gameTime);
            UpdateDrawValues();
            DetectPlayerCollision();
        }

        private int collisionCorrectionAmount = 16;
        public void DetectPlayerCollision()
        {
            if (IsCollidable && IsInCameraView())
            {
                if (Bounds.Intersects(Global.PlayerObj.InteractBounds))
                {
                    if (Bounds.X < Global.PlayerObj.InteractBounds.X)
                    {
                        //Global.PlayerObj.WorldPosition = Global.PlayerObj.LastWorldPosition;
                        Global.PlayerObj.WorldPosition = new Vector2(Bounds.Right + collisionCorrectionAmount, Global.PlayerObj.LastWorldPosition.Y);
                    }
                    else if (Bounds.X > Global.PlayerObj.InteractBounds.X)
                    {
                        //Global.PlayerObj.WorldPosition = Global.PlayerObj.LastWorldPosition;
                        Global.PlayerObj.WorldPosition = new Vector2(Bounds.Left - collisionCorrectionAmount, Global.PlayerObj.LastWorldPosition.Y);
                    }
                    else if (Bounds.Y < Global.PlayerObj.InteractBounds.Y)
                    {
                        //Global.PlayerObj.WorldPosition = Global.PlayerObj.LastWorldPosition;
                        Global.PlayerObj.WorldPosition = new Vector2(Global.PlayerObj.LastWorldPosition.X, Bounds.Bottom + collisionCorrectionAmount);
                    }
                    else if (Bounds.Y > Global.PlayerObj.InteractBounds.Y)
                    {
                        //Global.PlayerObj.WorldPosition = Global.PlayerObj.LastWorldPosition;
                        Global.PlayerObj.WorldPosition = new Vector2(Global.PlayerObj.LastWorldPosition.X, Bounds.Top - collisionCorrectionAmount);
                    }
                }
            }
        }

        private float alphaValue;
        private Color drawColour;
        private float distanceToPlayer;
        private float distanceToPlayerAbsolute;
        public void UpdateDrawValues()
        {
            if (Global.SceneManager.CurrentScene is DungeonScene)
            {
                if (!IsVisible)
                {
                    alphaValue = 0.05f;
                    drawColour = Color.White;
                }
                else
                {
                    distanceToPlayer = Vector2.Distance(WorldPosition, Global.PlayerObj.WorldPosition);
                    distanceToPlayerAbsolute = Math.Abs(distanceToPlayer);
                    alphaValue = ((float)Normalize(distanceToPlayer, Global.INSTANCE_SIGHT_RADIUS * 65, 0)).Clamp(0.05f, 1f);
                    drawColour = Color.White;
                }
            }
            //else if (Global.SceneManager.CurrentScene is WorldScene)
            //{
            //    if (!IsVisible)
            //    {
            //        drawColour = Color.Gray;
            //        alphaValue = 1f;
            //    }
            //    else
            //    {
            //        drawColour = Color.White;
            //        alphaValue = 1f;
            //    }
            //}
        }

        public void UpdateObjectVisibility()
        {
            if (Global.SHOW_FOG_OF_WAR == false)
                return;

            IsVisible = (Math.Abs(Vector2.Distance(Global.PlayerObj.WorldPosition, WorldPosition)) <= Global.FOG_OF_WAR_RADIUS) ? true : false;

            if (IsVisible && !IsExplored)
                IsExplored = true;
        }


        public void Draw(GameTime gameTime)
        {
            if (IsPlayerBlocking)
                return;

            if (!IsExplored)
                return;

            //if (!IsInCameraView())
            //    return;

            Global.SpriteBatch.Draw(Texture, WorldPosition, null, drawColour * alphaValue);
        }

        private bool IsInCameraView()
        {
            return Global.Camera.OrthoCamera.BoundingRectangle.Intersects(Bounds);
            //return Global.Camera.OrthoCamera.BoundingRectangle.Contains(WorldPosition);
        }

        public void DrawThumbnail(GameTime gameTime)
        {
            Global.SpriteBatch.Draw(
                Texture, 
                new Vector2(xIndex * Global.TileSize, yIndex * Global.TileSize), 
                null, 
                Color.White);
        }

        public void DrawMinimap(GameTime gameTime)
        {
            if (!IsExplored)
                return;

            Global.SpriteBatch.Draw(
                Texture,
                new Vector2(xIndex * Global.TileSize, yIndex * Global.TileSize),
                null,
                Color.White);
        }

        double Normalize(double value, double min, double max)
        {
            return (value - min) / (max - min);
        }

    }
}
