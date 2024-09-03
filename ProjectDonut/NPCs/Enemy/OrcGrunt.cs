using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectDonut.Pathfinding;
using ProjectDonut.Core.SceneManagement;
using ProjectDonut.Core;

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

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            _sprite = Global.ContentManager.Load<Texture2D>("Sprites/Enemy/Test_Grunt");
        }

        public override void Update(GameTime gameTime)
        {
            DistanceToPlayer = Distance(Position, Global.Player.Position) / 32;

            switch (State)
            {
                case EnemyState.Idle: 
                    
                    //if (CurrentPath == null && DistanceToPlayer <= DetectionDistance)
                    if (DistanceToPlayer <= DetectionDistance)
                    {                        
                        var curInstanceScene = (InstanceScene)Global.SceneManager.CurrentScene;
                        var curPlayerNode = new Node((int)Global.Player.Position.X / 32, (int)Global.Player.Position.Y / 32);
                        var curNode = new Node((int)Position.X / 32, (int)Position.Y / 32);
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
                        if (Position.X == NextPosition?.X && Position.Y == NextPosition?.Y)
                        {
                            //    if (CurrentPosition != null)
                            //    {
                            //        Astar.SetOccupiedCell(CurrentPosition.X / 32, CurrentPosition.Y / 32, false);
                            //    }

                            CurrentPosition = NextPosition;
                            //Astar.SetOccupiedCell(CurrentPosition.X / 32, CurrentPosition.Y / 32, true);
                            NextPosition = CurrentPath.FirstOrDefault();
                            CurrentPath.Remove(CurrentPosition);
                        }

                        if (MoveTimer >= MoveTime && NextPosition != null)
                        {
                            MoveTimer = 0;
                            Position = new Vector2(NextPosition.X, NextPosition.Y);

                            var curInstanceScene = (InstanceScene)Global.SceneManager.CurrentScene;
                            var curPlayerNode = new Node((int)Global.Player.Position.X / 32, (int)Global.Player.Position.Y / 32);
                            var curNode = new Node((int)Position.X / 32, (int)Position.Y / 32);
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

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }    
    }
}
