using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectDonut.Pathfinding;
using ProjectDonut.Core;
using ProjectDonut.Core.SceneManagement.SceneTypes;

namespace ProjectDonut.NPCs.Enemy
{
    public class OrcGrunt : Enemy
    {
        private int MoveTimer;
        private int MoveTime;

        private float DistanceToPlayer;
        private float DetectionDistance;

        private List<Node> CurrentPath;
        private Node CurrentPosition;
        private Node NextPosition;

        public override void Initialize()
        {
            base.Initialize();

            MoveTime = 5;
            MoveTimer = 0;

            DetectionDistance = 1000;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Texture = Global.ContentManager.Load<Texture2D>("Sprites/Enemy/Test_Grunt");
        }

        public override void Update(GameTime gameTime)
        {
            DistanceToPlayer = Distance(WorldPosition, Global.PlayerObj.WorldPosition) / Global.TileSize;

            switch (State)
            {
                case EnemyState.Idle: 
                    
                    //if (CurrentPath == null && DistanceToPlayer <= DetectionDistance)
                    if (DistanceToPlayer <= DetectionDistance)
                    {                        
                        var curInstanceScene = (DungeonScene)Global.SceneManager.CurrentScene;
                        var curPlayerNode = new Node((int)Global.PlayerObj.WorldPosition.X / Global.TileSize, (int)Global.PlayerObj.WorldPosition.Y / Global.TileSize);
                        var curNode = new Node((int)WorldPosition.X / Global.TileSize, (int)WorldPosition.Y / Global.TileSize);
                        CurrentPath = Astar.FindPath(curInstanceScene.DataMap, curNode, curPlayerNode);
                        

                        if (CurrentPath != null)
                        {
                            State = EnemyState.Moving;
                            NextPosition = CurrentPath.FirstOrDefault();
                            CurrentPath.Remove(CurrentPosition);
                        }
                    }
                    break;

                case EnemyState.Moving:
                    if (CurrentPath != null && CurrentPath.Count > 0)
                    {
                        if (WorldPosition.X == NextPosition?.X && WorldPosition.Y == NextPosition?.Y)
                        {
                            //    if (CurrentPosition != null)
                            //    {
                            //        Astar.SetOccupiedCell(CurrentPosition.X / Global.TileSize, CurrentPosition.Y / Global.TileSize, false);
                            //    }

                            CurrentPosition = NextPosition;
                            //Astar.SetOccupiedCell(CurrentPosition.X / Global.TileSize, CurrentPosition.Y / Global.TileSize, true);
                            NextPosition = CurrentPath.FirstOrDefault();
                            CurrentPath.Remove(CurrentPosition);
                        }

                        if (MoveTimer >= MoveTime && NextPosition != null)
                        {
                            MoveTimer = 0;
                            WorldPosition = new Vector2(NextPosition.X, NextPosition.Y);

                            var curInstanceScene = (DungeonScene)Global.SceneManager.CurrentScene;
                            var curPlayerNode = new Node((int)Global.PlayerObj.WorldPosition.X / Global.TileSize, (int)Global.PlayerObj.WorldPosition.Y / Global.TileSize);
                            var curNode = new Node((int)WorldPosition.X / Global.TileSize, (int)WorldPosition.Y / Global.TileSize);
                            CurrentPath = Astar.FindPath(curInstanceScene.DataMap, curNode, curPlayerNode);
                        }
                        else
                        {
                            MoveTimer++;
                        }
                    }
                    else
                    {
                        State = EnemyState.Idle;
                    }
                    break;

                case EnemyState.Attacking: 
                    break;
            }

            base.Update(gameTime);
        }

        private float Distance(Vector2 a, Vector2 b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }    
    }
}
