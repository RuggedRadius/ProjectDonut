using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World.Structures;
using ProjectDonut.UI.ScrollDisplay;
using ProjectDonut.Core;
using IGameComponent = ProjectDonut.Interfaces.IGameComponent;
using ProjectDonut.Core.Input;
using ProjectDonut.ProceduralGeneration.World.Generators;
using ProjectDonut.Core.SceneManagement.SceneTypes;
using ProjectDonut.ProceduralGeneration.World.MineableItems;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class WorldChunk : IGameComponent
    {
        public int ChunkCoordX { get; private set; }
        public int ChunkCoordY { get; private set; }

        public int WorldCoordX;
        public int WorldCoordY;

        public float[,] HeightData;
        public int[,] BiomeData;
        public float[,] ForestData;
        public float[,] RiverData;
        public float[,] StructureData;

        // State
        public bool IsPostProcessed { get; set; }

        public Rectangle ChunkBounds { get; private set; }

        public Dictionary<string, Tilemap> Tilemaps;

        public List<WorldStructure> Structures;
        private WorldChunkManager _manager;

        private Texture2D tempTexture;

        public Dictionary<string, List<ISceneObject>> SceneObjects;
        public Dictionary<string, List<IMineable>> MineableObjects;

        public WorldChunk NeighbourNorthEast { get; set; }
        public WorldChunk NeighbourNorthWest { get; set; }
        public WorldChunk NeighbourNorth { get; set; }
        public WorldChunk NeighbourEast { get; set; }
        public WorldChunk NeighbourSouth { get; set; }
        public WorldChunk NeighbourSouthEast { get; set; }
        public WorldChunk NeighbourSouthWest { get; set; }
        public WorldChunk NeighbourWest { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public Vector2 Position // TODO: Use this instead of individuals int above
        { 
            get; 
            set; 
        }
        public int ZIndex // TODO: Currently not used, defaulted to 0
        { 
            get; 
            set; 
        }

        public WorldChunk(int chunkXPos, int chunkYPos, WorldChunkManager manager)
        {
            _manager = manager;
            ChunkCoordX = chunkXPos;
            ChunkCoordY = chunkYPos;

            WorldCoordX = chunkXPos * Global.ChunkSize * Global.TileSize;
            WorldCoordY = chunkYPos * Global.ChunkSize * Global.TileSize;

            Tilemaps = new Dictionary<string, Tilemap>();

            tempTexture = new Texture2D(Global.GraphicsDevice, Global.TileSize, Global.TileSize);
            Color[] colorData = new Color[Global.TileSize * Global.TileSize];
            for (int i = 0; i < colorData.Length; i++)
            {
                colorData[i] = Color.White;
            }

            // Set the texture data to the array of colors
            tempTexture.SetData(colorData);

            ChunkBounds = new Rectangle(WorldCoordX, WorldCoordY, Global.ChunkSize * Global.TileSize, Global.ChunkSize * Global.TileSize);

            AllocateNeighbours();
        }

        public void AllocateNeighbours()
        {
            if (_manager._chunks.ContainsKey((ChunkCoordX - 1, ChunkCoordY - 1)))
            {
                NeighbourNorthWest = _manager._chunks[(ChunkCoordX - 1, ChunkCoordY - 1)];
            }
            if (_manager._chunks.ContainsKey((ChunkCoordX, ChunkCoordY - 1)))
            {
                NeighbourNorth= _manager._chunks[(ChunkCoordX, ChunkCoordY - 1)];
            }
            if (_manager._chunks.ContainsKey((ChunkCoordX + 1, ChunkCoordY - 1)))
            {
                NeighbourNorthEast = _manager._chunks[(ChunkCoordX + 1, ChunkCoordY - 1)];
            }
            if (_manager._chunks.ContainsKey((ChunkCoordX - 1, ChunkCoordY)))
            {
                NeighbourWest = _manager._chunks[(ChunkCoordX - 1, ChunkCoordY)];
            }
            if (_manager._chunks.ContainsKey((ChunkCoordX + 1, ChunkCoordY)))
            {
                NeighbourEast = _manager._chunks[(ChunkCoordX + 1, ChunkCoordY)];
            }
            if (_manager._chunks.ContainsKey((ChunkCoordX - 1, ChunkCoordY + 1)))
            {
                NeighbourSouthWest = _manager._chunks[(ChunkCoordX - 1, ChunkCoordY + 1)];
            }
            if (_manager._chunks.ContainsKey((ChunkCoordX, ChunkCoordY + 1)))
            {
                NeighbourSouth = _manager._chunks[(ChunkCoordX, ChunkCoordY + 1)];
            }
            if (_manager._chunks.ContainsKey((ChunkCoordX + 1, ChunkCoordY + 1)))
            {
                NeighbourSouthEast = _manager._chunks[(ChunkCoordX + 1, ChunkCoordY + 1)];
            }

        }

            public void Initialize()
        {
            //_fog = new FogOfWar(Width, Height);
        }

        public void LoadContent()
        {
            if (Structures == null)
            {
                Structures = new List<WorldStructure>();

                foreach (var kvp in SceneObjects)
                {
                    foreach (var obj in kvp.Value)
                    {
                        if (obj is WorldStructure)
                        {
                            Structures.Add((WorldStructure)obj);
                        }
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            // Update each tile
            foreach (var tilemap in Tilemaps)
            {
                tilemap.Value.Update(gameTime);
            }

            foreach (var kvp in SceneObjects)
            {
                foreach (var obj in kvp.Value)
                {
                    obj.Update(gameTime);
                }
            }

            if (Global.SceneManager.CurrentScene is WorldScene &&
                Global.WorldChunkManager.CurrentChunks.Contains(this))
            {
                //if (!Structures.Where(x => x.PlayerWithinScrollBounds).Any())
                //{
                //    ScrollDisplayer.CurrentStructure = null;
                //    Global.ScrollDisplay.HideScroll();
                //}
            }

            // Create a copy of the MineableObjects collection
            var mineableObjectsCopy = MineableObjects.Values.ToList();

            // Iterate over the copied collection
            foreach (var mineableList in mineableObjectsCopy)
            {
                // Create a copy of the inner list
                var mineableListCopy = mineableList.ToList();

                // Iterate over the copied inner list
                foreach (var mineable in mineableListCopy)
                {
                    if (mineable == null)
                    {
                        continue;
                    }

                    mineable.Update(gameTime);
                }
            }

            // Remove mineable objects with health <= 0
            MineableObjects["trees"].RemoveAll(x => x.Health <= 0);
            MineableObjects["rocks"].RemoveAll(x => x.Health <= 0);
        }

        public void Draw(GameTime gameTime)
        {
            //Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            foreach (var tilemap in Tilemaps)
            {
                tilemap.Value.Draw(gameTime);
            }

            // TODO: Stop creating a variable every frame for this
            var objs = new List<ISceneObject>();
            SceneObjects
                .Select(x => x.Value)
                .ToList()
                .ForEach(x => objs.AddRange(x));

            MineableObjects
                .Select(x => x.Value)
                .ToList()
                .ForEach(x => objs.AddRange(x));

            objs
                .OrderBy(x => x.ZIndex)
                .ThenBy(x => x.WorldPosition.Y)
                .ToList()
                .ForEach(x => x.Draw(gameTime));

            if (Global.DRAW_WORLD_CHUNK_OUTLINE)
            {
                DrawChunkOutline(gameTime);
            }

            //Global.SpriteBatch.End();

            //MineableObjects.Values.ToList().ForEach(x => x.ForEach(y => y.Draw(gameTime)));

            return;
        }

        public void DrawSceneObjectsBelowPlayer(GameTime gameTime)
        {
            var objs = new List<ISceneObject>();

            foreach (var sceneObject in SceneObjects)
            {
                var validObjs = sceneObject.Value.Where(x => x.ZIndex <= Global.PlayerObj.WorldPosition.Y).ToList();
                objs.AddRange(validObjs);
            }

            //Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            objs
                .OrderBy(x => x.WorldPosition.Y)
                .ThenByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));
            //Global.SpriteBatch.End();
        }

        public void DrawMineableObjectsBelowPlayer(GameTime gameTime)
        {
            var objs = new List<IMineable>();

            foreach (var mineableObj in MineableObjects)
            {
                var validObjs = mineableObj.Value.Where(x => x.ZIndex <= Global.PlayerObj.WorldPosition.Y).ToList();
                objs.AddRange(validObjs);
            }

            //Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            objs
                .OrderBy(x => x.WorldPosition.Y)
                .ThenByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));
            //Global.SpriteBatch.End();
        }

        public void DrawSceneObjectsAbovePlayer(GameTime gameTime)
        {
            var objs = new List<ISceneObject>();

            foreach (var sceneObject in SceneObjects)
            {
                var validObjs = sceneObject.Value.Where(x => x.ZIndex > Global.PlayerObj.WorldPosition.Y).ToList();
                objs.AddRange(validObjs);
            }

            //Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            objs
                .OrderBy(x => x.WorldPosition.Y)
                .ThenByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));
            //Global.SpriteBatch.End();
        }

        public void DrawMineableObjectsAbovePlayer(GameTime gameTime)
        {
            var objs = new List<IMineable>();

            foreach (var mineableObj in MineableObjects)
            {
                var validObjs = mineableObj.Value.Where(x => x.ZIndex > Global.PlayerObj.WorldPosition.Y).ToList();
                objs.AddRange(validObjs);
            }

            //Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            objs
                .OrderBy(x => x.WorldPosition.Y)
                .ThenByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));
            //Global.SpriteBatch.End();
        }

        private void DrawChunkOutline(GameTime gameTime)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var position = new Vector2(WorldCoordX + x * Global.TileSize, WorldCoordY + y * Global.TileSize);

                    if (x == 0 || y == 0)
                    {
                        Global.SpriteBatch.Draw(tempTexture, position, null, Color.Magenta * 0.1f);
                    }
                    else if (x == Width - 1 || y == Height - 1)
                    {
                        Global.SpriteBatch.Draw(tempTexture, position, null, Color.Magenta * 0.1f);
                    }
                }
            }
        }        

        public void GrowForest()
        {

        }        
    }
}
