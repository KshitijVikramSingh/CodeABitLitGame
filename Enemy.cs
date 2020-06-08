using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CodeABitLitGame
{
    class Enemy : GameObject
    {
        int speed = 4;

        public enum awarenessState { aware, unaware };
        public awarenessState awareness = awarenessState.aware;

        public Queue<Vector2> chaseTargets;

        public Enemy(ContentManager content, Vector2 position)
        {
            Texture = content.Load<Texture2D>("Enemy");

            this.position = position;
            targetPosition = position;

            chaseTargets = new Queue<Vector2>();
            chaseTargets.Enqueue(targetPosition);
        }

        public override void Update()
        {
            if (targetPosition != position)
            {
                if (position.X < targetPosition.X) { position.X += speed; }
                else if (position.X > targetPosition.X) { position.X -= speed; }
                else if (position.Y < targetPosition.Y) { position.Y += speed; }
                else if (position.Y > targetPosition.Y) { position.Y -= speed; }
            }
            else
            {
                if (Game1.canMove && chaseTargets.Count > 0)
                {
                    Vector2 newTarget = chaseTargets.Dequeue();
                    Game1.canMove = false;

                    if (newTarget.X > targetPosition.X && BoardLayout.currentBoard.HasSpaceForMovement(rightRectangle, this))
                    {
                        targetPosition.X += 32;
                    }
                    else if (newTarget.X < targetPosition.X && BoardLayout.currentBoard.HasSpaceForMovement(leftRectangle, this))
                    {
                        targetPosition.X -= 32;
                    }
                    else if (newTarget.Y < targetPosition.Y && BoardLayout.currentBoard.HasSpaceForMovement(upRectangle, this))
                    {
                        targetPosition.Y -= 32;
                    }
                    else if (newTarget.Y > targetPosition.Y && BoardLayout.currentBoard.HasSpaceForMovement(downRectangle, this))
                    {
                        targetPosition.Y += 32;
                    }
                }
            }

            if (awareness == awarenessState.aware)
            {
                if(targetPosition != Game1.player.targetPosition && Game1.canMove)
                {
                    chaseTargets.Enqueue(Game1.player.targetPosition);
                    Game1.canMove = false;
                }
            }

            base.Update();
        }
    }
}
