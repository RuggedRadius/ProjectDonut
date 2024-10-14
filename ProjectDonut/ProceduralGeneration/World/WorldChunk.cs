using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World.Structures;
using IGameComponent = ProjectDonut.Interfaces.IGameComponent;
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

        public int[,] HeightData;
        public int[,] BiomeData;
        public int[,] ForestData;
        public int[,] RiverData;
        public int[,] StructureData;

        public Dictionary<string, Tilemap> Tilemaps;

        public List<WorldStructure> Structures;
        private WorldChunkManager _manager;

        private Texture2D tempTexture;

        public bool _thumbnailRendered = false;
        public RenderTarget2D ChunkRenderTarget;
        //private Texture2D ChunkTexture;

        public Dictionary<string, List<ISceneObject>> SceneObjects;
        public Dictionary<string, List<IMineable>> MineableObjects;

        public Rectangle ChunkBounds { get; set; }

        //public List<Tilemap> TownFloor { get; set; }
        public Town Town { get; set; }

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
        
        public void RenderThumbnail(GameTime gameTime)
        {
            foreach (var tile in Tilemaps["base"].Map)
            {
                if (tile == null)
                    return;
            }

            ChunkRenderTarget = new RenderTarget2D(
                Global.GraphicsDevice, 
                Global.ChunkSize * Global.TileSize, 
                Global.ChunkSize * Global.TileSize
                );

            Global.GraphicsDevice.SetRenderTarget(ChunkRenderTarget);
            Global.GraphicsDevice.Clear(Color.Transparent);

            Global.SpriteBatch.Begin();
            Tilemaps["base"].DrawThumbnail(gameTime);
            Global.SpriteBatch.End();

            Global.GraphicsDevice.SetRenderTarget(null);

            //DebugMapData.SaveMapThumbnail(ChunkCoordX, ChunkCoordY, ChunkRenderTarget);

            _thumbnailRendered = true;
        }

        public void Update(GameTime gameTime)
        {
            if (Town != null)
            {
                foreach (var plot in Town.Plots)
                {
                    if (plot.Building.WorldBounds.Contains(Global.PlayerObj.WorldPosition))
                    {
                        foreach (var tile in Town.Tilemaps["roofs"].Map)
                        {
                            if (tile == null)
                                continue;

                            if (tile.Bounds.Intersects(plot.LocalBounds))
                            {
                                tile.IsPlayerBlocking = true;
                            }
                            else
                            {
                                tile.IsPlayerBlocking = false;
                            }
                        }                        
                    }
                    else
                    {
                        foreach (var tile in Town.Tilemaps["roofs"].Map)
                        {
                            if (tile == null)
                                continue;

                            if (tile.Bounds.Intersects(plot.LocalBounds))
                            {
                                tile.IsPlayerBlocking = false;
                            }
                        }
                    }
                }
            }

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


            MineableObjects.Values.ToList().ForEach(x => x.ForEach(y => y.Update(gameTime)));
            MineableObjects.Values.ToList().ForEach(x => x.Where(y => y.Health <= 0).ToList().ForEach(x => MineableObjects["trees"].Remove(x)));
            MineableObjects.Values.ToList().ForEach(x => x.Where(y => y.Health <= 0).ToList().ForEach(x => MineableObjects["rocks"].Remove(x)));
            //MineableObjects["trees"].Where(x => x.Health <= 0).ToList().ForEach(x => MineableObjects["trees"].Remove(x));
        
            if (_thumbnailRendered == false)
            {
                //if (ChunkCoordX == 0 && ChunkCoordY == 1) // Test for another chunk
                //{
                    RenderThumbnail(gameTime);
                //}
                //RenderThumbnail(gameTime);                
            }

            if (Town != null)
            {
                Town.Update(gameTime);
            }
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


            if (Town != null)
            {
                Town.Draw(gameTime);
            }
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
                        Global.SpriteBatch.Draw(tempTexture, position, null, Color.Magenta * 0.25f);
                    }
                    else if (x == Width - 1 || y == Height - 1)
                    {
                        Global.SpriteBatch.Draw(tempTexture, position, null, Color.Magenta * 0.25f);
                    }
                }
            }
        }        

        public void GrowForest()
        {

        }        
    }
}
