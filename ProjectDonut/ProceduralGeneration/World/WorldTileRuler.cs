using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
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
        private CoastTileRules coastRules;
        private ForestTileRules forestRules;

        public WorldTileRuler()
        {

            coastRules = new CoastTileRules();
            forestRules = new ForestTileRules();
        }

        public Tilemap ApplyBaseRules(Tilemap tilemap)
        {
            tilemap = coastRules.ApplyCoastLineRules(tilemap);

            return tilemap;
        }

        public Tilemap ApplyForestRules(Tilemap tilemap)
        {
            tilemap = forestRules.ApplyForestRules(tilemap);

            return tilemap;
        }
    }
}
