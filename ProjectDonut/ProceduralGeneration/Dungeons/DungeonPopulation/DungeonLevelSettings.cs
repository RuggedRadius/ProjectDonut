using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.ProceduralGeneration.Dungeons.DungeonPopulation
{
    public class DungeonLevelSettings
    {
        public int EnemyCount { get; set; }
        public Dictionary<string, int> PossibleEnemies { get; set; }
        public bool HasSubsequentLevel { get; set; }
    }
}
