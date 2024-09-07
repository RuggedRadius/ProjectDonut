using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World.Structures;
using ProjectDonut.UI.ScrollDisplay;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.Core;
using IGameComponent = ProjectDonut.Interfaces.IGameComponent;
using ProjectDonut.Core.Input;
using ProjectDonut.ProceduralGeneration.World.Generators;

namespace ProjectDonut.ProceduralGeneration.World
{
    public class WorldChunk : IGameComponent
    {
        public int ChunkCoordX { get; private set; }
        public int ChunkCoordY { get; private set; }

        public int WorldCoordX;
        public int WorldCoordY;

        public int[,] HeightData;
        public int[,] BiomeData;
        public int[,] ForestData;
        public int[,] RiverData;
        public int[,] StructureData;

        public Dictionary<string, Tilemap> Tilemaps;

        public List<WorldStructure> Structures;
        private WorldChunkManager _manager;

        private Texture2D tempTexture;

        public Dictionary<string, List<ISceneObject>> SceneObjects;
        public Dictionary<string, List<IMineable>> MineableObjects;

        public int Width
        {
            get
            {
                if (HeightData == null)
                    return 0;

                return HeightData.GetLength(0);
            }
            set
            {
                Width = value;
            }
        }
        public int Height
        {
            get
            {
                if (HeightData == null)
                    return 0;

                return HeightData.GetLength(1);
            }
            set
            {
                Height = value;
            }
        }

        public Vector2 Position // TODO: Use this instead of individuals int above
        { 
            get; 
            set; 
        }
        public int ZIndex // TODO: Currently not used, well.. defaults to 0
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
                foreach (var tile in tilemap.Value.Map)
                {
                    if (tile == null)
                        continue;

                    tile.Update(gameTime);
                }
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
                if (!Structures.Where(x => x.PlayerWithinScrollBounds).Any())
                {
                    ScrollDisplayer.CurrentStructure = null;
                    Global.ScrollDisplay.HideScroll();
                }
            }


            MineableObjects.Values.ToList().ForEach(x => x.ForEach(y => y.Update(gameTime)));
            MineableObjects.Values.ToList().ForEach(x => x.Where(y => y.Health <= 0).ToList().ForEach(x => MineableObjects["trees"].Remove(x)));
            //MineableObjects["trees"].Where(x => x.Health <= 0).ToList().ForEach(x => MineableObjects["trees"].Remove(x));
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var tilemap in Tilemaps)
            {
                foreach (var tile in tilemap.Value.Map)
                {
                    if (tile == null)
                        continue;

                    tile.Draw(gameTime);
                }
            }

            if (Global.DRAW_WORLD_CHUNK_OUTLINE)
            {
                DrawChunkOutline(gameTime);
            }

            //MineableObjects.Values.ToList().ForEach(x => x.ForEach(y => y.Draw(gameTime)));

            return;
        }

        public void DrawSceneObjectsBelowPlayer(GameTime gameTime)
        {
            var objs = new List<ISceneObject>();

            foreach (var sceneObject in SceneObjects)
            {
                var validObjs = sceneObject.Value.Where(x => x.ZIndex <= Global.Player.WorldPosition.Y).ToList();
                objs.AddRange(validObjs);
            }

            Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            objs
                .OrderBy(x => x.WorldPosition.Y)
                .ThenByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));
            Global.SpriteBatch.End();
        }

        public void DrawMineableObjectsBelowPlayer(GameTime gameTime)
        {
            var objs = new List<IMineable>();

            foreach (var mineableObj in MineableObjects)
            {
                var validObjs = mineableObj.Value.Where(x => x.ZIndex <= Global.Player.WorldPosition.Y).ToList();
                objs.AddRange(validObjs);
            }

            Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            objs
                .OrderBy(x => x.WorldPosition.Y)
                .ThenByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));
            Global.SpriteBatch.End();
        }

        public void DrawSceneObjectsAbovePlayer(GameTime gameTime)
        {
            var objs = new List<ISceneObject>();

            foreach (var sceneObject in SceneObjects)
            {
                var validObjs = sceneObject.Value.Where(x => x.ZIndex > Global.Player.WorldPosition.Y).ToList();
                objs.AddRange(validObjs);
            }

            Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            objs
                .OrderBy(x => x.WorldPosition.Y)
                .ThenByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));
            Global.SpriteBatch.End();
        }

        public void DrawMineableObjectsAbovePlayer(GameTime gameTime)
        {
            var objs = new List<IMineable>();

            foreach (var mineableObj in MineableObjects)
            {
                var validObjs = mineableObj.Value.Where(x => x.ZIndex > Global.Player.WorldPosition.Y).ToList();
                objs.AddRange(validObjs);
            }

            Global.SpriteBatch.Begin(transformMatrix: Global.Camera.GetTransformationMatrix());
            objs
                .OrderBy(x => x.WorldPosition.Y)
                .ThenByDescending(x => x.ZIndex)
                .ToList()
                .ForEach(x => x.Draw(gameTime));
            Global.SpriteBatch.End();
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
                        Global.SpriteBatch.Draw(tempTexture, position, null, Color.Magenta);
                    }
                    else if (x == Width - 1 || y == Height - 1)
                    {
                        Global.SpriteBatch.Draw(tempTexture, position, null, Color.Magenta);
                    }
                }
            }
        }        

        public void GrowForest()
        {

        }        
    }
}
