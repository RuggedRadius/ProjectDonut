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

        public void GenerateStructureData(WorldChunk chunk)
        {
            chunk.StructureData = new int[chunk.Width, chunk.Height];

            var viableLocations = GetViableStructureLocations(chunk);

            var townChance = random.Next(1, 101);
            var castleChance = random.Next(1, 101);

            var castleCount = castleChance > 0 ? 0 : 0;
            var townCount = townChance > 0 ? 1 : 0;

            for (int i = 0; i < castleCount; i++)
            {
                if (viableLocations.Count == 0)
                {
                    break;
                }

                var location = viableLocations[random.Next(0, viableLocations.Count)];
                viableLocations.Remove(location);

                chunk.StructureData[location.Item1, location.Item2] = 1;
            }

            for (int i = 0; i < townCount; i++)
            {
                if (viableLocations.Count == 0)
                {
                    break;
                }

                var location = viableLocations[random.Next(0, viableLocations.Count)];
                viableLocations.Remove(location);

                chunk.StructureData[location.Item1, location.Item2] = 2;
            }

            //chunk.Structures = GetStructuresData(chunk);
        }

        private List<(int, int)> GetViableStructureLocations(WorldChunk chunk)
        {
            var results = new List<(int, int)>();
            var exclusions = new List<(int, int)>();

            for (int i = 0; i < chunk.Width; i++)
            {
                for (int j = 0; j < chunk.Height; j++)
                {
                    if (IsCellSuitable(chunk, i, j) == false)
                    {
                        continue;
                    }

                    if (exclusions.Contains((i, j)))
                    {
                        continue;
                    }

                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (x != 0 || y != 0)
                            {
                                exclusions.Add((i + x, j + y));
                            }
                        }
                    }

                    results.Add((i, j));
                }
            }

            return results;
        }

        private bool IsCellSuitable(WorldChunk chunk, int i, int j)
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (i + x < 0 || j + y < 0 || i + x >= chunk.Width || j + y >= chunk.Height)
                    {
                        return false; // Out of bounds
                    }

                    if (chunk.StructureData[i + x, j + y] != 0)
                    {
                        return false; // Structure already exists there
                    }

                    if (chunk.HeightData[i + x, j + y] < settings.GroundHeightMin ||
                        chunk.HeightData[i + x, j + y] > settings.GroundHeightMax)
                    {
                        return false;  // Not suitable ground height
                    }

                    if (chunk.ForestData[i + x, j + y] != 0)
                    {
                        return false; // Intersects with forest
                    }
                }
            }

            return true;
        }

        public List<ISceneObject> GenerateCastles(WorldChunk chunk)
        {
            var structures = new List<ISceneObject>();

            if (random.Next(0, 100) < 50) // 50% chance of creating a castle in a chunk
            {
                return structures;
            }

            var viableLocations = GetViableStructureLocations(chunk);

            if (viableLocations.Count == 0)
            {
                return structures;
            }

            var viableLocation = viableLocations[random.Next(0, viableLocations.Count)];
            var position = new Vector2(viableLocation.Item1, viableLocation.Item2);
            viableLocations.Remove(viableLocation);

            var castle = new WorldStructure(position, chunk)
            {
                Name = NameGenerator.GenerateRandomName(random.Next(2, 5)),
                Instance = new InstanceScene(SceneType.Instance),
                Texture = Global.SpriteLibrary.GetSprite("castle"),
            };

            castle.Initialize();
            chunk = CullScenaryAtCastleLocation(castle.InteractBounds, chunk);
            structures.Add(castle);

            return structures;
        }

        private WorldChunk CullScenaryAtCastleLocation(Rectangle castleBounds, WorldChunk chunk)
        {
            var sceneObjectsToCull = new List<ISceneObject>();
            foreach (var objList in chunk.SceneObjects?.Values)
            {
                foreach (var obj in objList)
                {
                    if (castleBounds.Intersects(obj.Bounds))
                    {
                        sceneObjectsToCull.Add(obj);
                    }
                }
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
                    if (castleBounds.Intersects(obj.InteractBounds))
                    {
                        mineablesToCull.Add(obj);
                    }
                }
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
