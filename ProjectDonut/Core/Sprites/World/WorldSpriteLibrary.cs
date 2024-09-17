using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Core.Sprites.World
{
    public static class WorldSpriteLibrary
    {
        public static void Load()
        {
            LoadMineables();
        }

        private static void LoadMineables()
        {
            var trees = new List<Texture2D>();
            trees.Add(Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Tree2"));
            SpriteLib.WorldMapSprites.Add("tree-02", trees);

            var treeStumps = new List<Texture2D>();
            treeStumps.Add(Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Tree-stump-export"));
            SpriteLib.WorldMapSprites.Add("tree-stump", treeStumps);

            var treesWinter = new List<Texture2D>();
            treesWinter.Add(Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Tree2-winter"));
            SpriteLib.WorldMapSprites.Add("tree-02-winter", treesWinter);

            var rocks = new List<Texture2D>();
            rocks.Add(Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Rock01"));
            SpriteLib.WorldMapSprites.Add("rock-01", rocks);

            var cactus = new List<Texture2D>();
            cactus.Add(Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Cactus01"));
            SpriteLib.WorldMapSprites.Add("cactus-01", cactus);

            var rockSmashed = new List<Texture2D>();
            var rockSheet = Global.ContentManager.Load<Texture2D>("Sprites/Map/World/Rock01");
            rockSmashed.Add(SpriteLib.ExtractSprite(rockSheet, 4 * Global.TileSize, 0, Global.TileSize, Global.TileSize));
            SpriteLib.WorldMapSprites.Add("rock-smashed", rockSmashed);
        }
    }
}
