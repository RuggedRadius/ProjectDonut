using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDonut.Core;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World.MineableItems;
using ProjectDonut.ProceduralGeneration.World.Structures;
using ProjectDonut.Tools;

namespace ProjectDonut.ProceduralGeneration.World.Generators
{
    public enum Structure
    {
        None,
        Castle,
        Town
    }

    public class StructureGenerator
    {
        private int CastleCountMin = 0;
        private int CastleCountMax = 2;

        private int TownCountMin = 0;
        private int TownCountMax = 2;

        private WorldMapSettings settings;
        private Random random = new Random();

        public StructureGenerator(WorldMapSettings settings)
        {
            this.settings = settings;
        }

        private List<Tile> GetPossibleLocations(WorldChunk chunk)
        {
            var allGroundTiles = chunk.Tilemaps["base"].Map
                .Cast<Tile>()
                .Where(x => x.WorldTileType == WorldTileType.Ground)
                .ToList();

            var checkedPos = new List<Tile>();
            foreach (var pos in allGroundTiles)
            {
                var allGround = true;

                for (int x = 0; x < 13; x++)
                {
                    for (int y = 0; y < 13; y++)
                    {
                        // Check its within chunk bounds
                        if (pos.xIndex + x < 0 || pos.yIndex + y < 0 || pos.xIndex + x >= chunk.Width || pos.yIndex + y >= chunk.Height)
                        {
                            allGround = false;
                            break;
                        }

                        // Check if its a ground tile
                        if (chunk.Tilemaps["base"].Map[pos.xIndex + x, pos.yIndex + y].WorldTileType != WorldTileType.Ground)
                        {
                            allGround = false;
                            break;
                        }
                    }
                }

                if (allGround)
                {
                    checkedPos.Add(pos);
                }
            }

            return checkedPos;
        }

        public List<ISceneObject> GenerateTowns(WorldChunk chunk)
        {
            var structures = new List<ISceneObject>();

            if (random.Next(0, 100) < 50) // 50% chance of creating a town in a chunk
            {
                return structures;
            }

            //var viableLocations = GetViableStructureLocations(chunk);
            var viableLocations = GetPossibleLocations(chunk);

            if (viableLocations.Count == 0)
            {
                return structures;
            }

            var viableLocation = viableLocations[random.Next(0, viableLocations.Count)];
            var position = viableLocation.WorldPosition;
            viableLocations.Remove(viableLocation);

            var town = new WorldStructure(position, chunk, WorldStructureType.Town);

            town.Initialize();
            town.LoadContent();
            chunk = CullScenaryInRectangle(town.InteractBounds, chunk);
            structures.Add(town);

            return structures;
        }

        public List<ISceneObject> GenerateCastles(WorldChunk chunk)
        {
            var structures = new List<ISceneObject>();

            if (random.Next(0, 100) < 90) // 50% chance of creating a castle in a chunk
            {
                return structures;
            }

            //var viableLocations = GetViableStructureLocations(chunk);
            var viableLocations = GetPossibleLocations(chunk);

            if (viableLocations.Count == 0)
            {
                return structures;
            }

            var viableLocation = viableLocations[random.Next(0, viableLocations.Count)];
            var position = viableLocation.WorldPosition;
            viableLocations.Remove(viableLocation);

            var castle = new WorldStructure(position, chunk, WorldStructureType.Castle);

            castle.Initialize();
            castle.LoadContent();
            chunk = CullScenaryInRectangle(castle.InteractBounds, chunk);
            structures.Add(castle);

            return structures;
        }

        private WorldChunk CullScenaryInRectangle(Rectangle bounds, WorldChunk chunk)
        {
            var sceneObjectsToCull = new List<ISceneObject>();
            foreach (var objList in chunk.SceneObjects?.Values)
            {
                foreach (var obj in objList)
                {
                    if (bounds.Intersects(obj.TextureBounds))
                    {
                        sceneObjectsToCull.Add(obj);
                    }
                }
            }

            if (sceneObjectsToCull.Count > 0)
            {
                int i = 0;
            }

            foreach (var obj in sceneObjectsToCull)
            {
                foreach (var kvp in chunk.SceneObjects)
                {
                    if (kvp.Value.Remove(obj))
                    {
                        break;
                    }
                }
            }

            var mineablesToCull = new List<IMineable>();
            foreach (var objList in chunk.MineableObjects?.Values)
            {
                foreach (var obj in objList)
                {
                    if (bounds.Intersects(obj.InteractBounds))
                    {
                        mineablesToCull.Add(obj);
                    }
                }
            }

            if (mineablesToCull.Count > 0)
            {
                int i = 0;
            }

            foreach (var obj in mineablesToCull)
            {
                foreach (var kvp in chunk.MineableObjects)
                {
                    if (kvp.Value.Remove(obj))
                    {
                        break;
                    }
                }
            }

            return chunk;
        }
    }
}
