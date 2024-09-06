
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
using ProjectDonut.Core;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.GameObjects;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World;

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
        public Vector2 Position
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
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, Global.TileSize, Global.TileSize);
        }

        public void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            UpdateObjectVisibility();
            HandleAnimation(gameTime);
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

            float distance = Math.Abs(Vector2.Distance(Global.Player.Position, Position));
            IsVisible = (distance <= Global.FOG_OF_WAR_RADIUS) ? true : false;

            if (IsVisible && !IsExplored)
                IsExplored = true;
        }


        public void Draw(GameTime gameTime)
        {
            if (!IsExplored)
                return;

            if (Global.SceneManager.CurrentScene is InstanceScene)
            {
                if (!IsVisible)
                    return;

                var dist = Vector2.Distance(Position, Global.Player.Position);
                var absValue = Math.Abs(dist);
                var alphaValue = (float)Normalize(dist, 1000, 0);
                Global.SpriteBatch.Draw(Texture, Position, null, Color.White * alphaValue);
            }
            else
            {
                if (!IsVisible)
                {
                    Global.SpriteBatch.Draw(Texture, Position, null, Color.Gray);
                }
                else
                {
                    Global.SpriteBatch.Draw(Texture, Position, null, Color.White);
                }
            }
        }

        double Normalize(double value, double min, double max)
        {
            return (value - min) / (max - min);
        }

    }
}
