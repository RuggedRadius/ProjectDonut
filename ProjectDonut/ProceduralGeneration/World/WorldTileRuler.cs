using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.ProceduralGeneration.World.TileRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class WorldTileRuler
    {
        private SpriteLibrary spriteLib;

        private CoastTileRules coastRules;
        private ForestTileRules forestRules;

        public WorldTileRuler(SpriteLibrary spriteLib)
        {
            this.spriteLib = spriteLib;

            coastRules = new CoastTileRules(spriteLib);
            forestRules = new ForestTileRules(spriteLib);
        }

        public Tilemap ApplyBaseRules(Tilemap tilemap)
        {
            tilemap = forestRules.ApplyForestRules(tilemap);

            return tilemap;
        }

        public Tilemap ApplyForestRules(Tilemap tilemap)
        {
            tilemap = forestRules.ApplyForestRules(tilemap);

            return tilemap;
        }
    }
}
