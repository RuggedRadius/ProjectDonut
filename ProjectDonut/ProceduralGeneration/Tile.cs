﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using ProjectDonut.Core.SceneManagement.SceneTypes.Town;
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

        // Animation
        public bool IsAnimated { get; set; }
        public List<Texture2D> Frames { get; set; }
        private double _frameTimer { get; set; }
        private double _frameInterval { get; set; }
        private int _frameIndex { get; set; }

        // Visibility and Appearance
        public bool IsVisible { get; set; }
        public bool IsExplored { get; set; }
        public bool IsBlocked{ get; set; }
        public Texture2D Texture { get; set; }
        public bool IsPlayerBlocking { get; set; }

        public Rectangle Bounds { get; set; }

        public Tile(bool isAnimated)
        {
            IsAnimated = isAnimated;

            if (IsAnimated)
            {
                _frameInterval = 0.5f;
                _frameIndex = 0;
                _frameTimer = 0f;
            }
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
            UpdateObjectVisibility();
            HandleAnimation(gameTime);
            UpdateDrawValues();
        }

        private float alphaValue;
        private Color drawColour;
        private float distanceToPlayer;
        private float distanceToPlayerAbsolute;
        private void UpdateDrawValues()
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
            else if (Global.SceneManager.CurrentScene is WorldScene)
            {
                if (!IsVisible)
                {
                    drawColour = Color.Gray;
                    alphaValue = 1f;
                }
                else
                {
                    drawColour = Color.White;
                    alphaValue = 1f;
                }
            }
            //else if (Global.SceneManager.CurrentScene is TownScene)
            //{
            //    if (!IsVisible)
            //    {
            //        Global.SpriteBatch.Draw(Texture, WorldPosition, null, Color.Gray);
            //    }
            //    else
            //    {
            //        Global.SpriteBatch.Draw(Texture, WorldPosition, null, Color.White);
            //        //Global.SpriteBatch.DrawString(Global.FontDebug, WorldTileType.ToString(), Position, Color.Red);

            //    }
            //}
        }

        private void HandleAnimation(GameTime gameTime)
        {
            if (Frames == null || Frames.Count == 0)
            {
                return;
            }

            if (IsAnimated)
            {
                _frameTimer += gameTime.ElapsedGameTime.TotalSeconds;

                if (_frameTimer >= _frameInterval)
                {
                    _frameTimer = 0;
                    _frameIndex++;

                    if (_frameIndex >= Frames.Count)
                    {
                        _frameIndex = 0;
                    }

                    Texture = Frames[_frameIndex];
                }
            }
        }

        public void UpdateObjectVisibility()
        {
            if (Global.SHOW_FOG_OF_WAR == false)
            {
                IsVisible = true;
                IsExplored = true;
                return;
            }

            float distance = Math.Abs(Vector2.Distance(Global.PlayerObj.WorldPosition, WorldPosition));
            IsVisible = (distance <= Global.FOG_OF_WAR_RADIUS) ? true : false;

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
            //return Global.Camera.OrthoCamera.BoundingRectangle.Intersects(Bounds);
            return Global.Camera.OrthoCamera.BoundingRectangle.Contains(WorldPosition);
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
