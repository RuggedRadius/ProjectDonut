using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using ProjectDonut.Core;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using ProjectDonut.ProceduralGeneration;

namespace ProjectDonut.Combat
{
    public class CombatTerrain
    {
        public Dictionary<string, Tilemap> Tilemaps { get; set; }

        public CombatTerrain()
        {
            var tileCountWidth = Global.GraphicsDeviceManager.PreferredBackBufferWidth / Global.TileSize;
            var tileCountHeight = Global.GraphicsDeviceManager.PreferredBackBufferHeight / Global.TileSize;

            Tilemaps = new Dictionary<string, Tilemap>()
            {
                { "base", new Tilemap(tileCountWidth, tileCountHeight) },
                { "obstacle", new Tilemap(tileCountWidth, tileCountHeight) },
                { "decorative", new Tilemap(tileCountWidth, tileCountHeight) }
            };
        }

        public void Update(GameTime gameTime)
        {
            foreach (var tilemap in Tilemaps)
            {
                tilemap.Value.Update(gameTime);
            }
        }

        public void DrawTerrainLayer(GameTime gameTime, string layerName)
        {
            if (Tilemaps.ContainsKey(layerName))
            {
                Tilemaps[layerName].Draw(gameTime);
            }
        }

        public void DrawGrid(GameTime gameTime)
        {
            for (int i = 0; i < Global.GraphicsDeviceManager.PreferredBackBufferWidth; i += (Global.TileSize * CombatScene.SceneScale))
            {
                Global.SpriteBatch.DrawLine(i, 0, i, Global.GraphicsDeviceManager.PreferredBackBufferHeight, Color.Cyan, 1);
            }

            for (int i = 0; i < Global.GraphicsDeviceManager.PreferredBackBufferHeight; i += (Global.TileSize * CombatScene.SceneScale))
            {
                Global.SpriteBatch.DrawLine(0, i, Global.GraphicsDeviceManager.PreferredBackBufferWidth, i, Color.Cyan, 1);
            }
        }
    }
}
