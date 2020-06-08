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
        public awarenessState awareness = awarenessState.unaware;

        public Queue<Vector2> chaseTargets;

        Line playerVisiblityLine;

        public Enemy(ContentManager content, Vector2 position)
        {
            Texture = content.Load<Texture2D>("Enemy");

            this.position = position;
            targetPosition = position;

            chaseTargets = new Queue<Vector2>();
            chaseTargets.Enqueue(targetPosition);
        }

        bool isPlayerVisible()
        {
            foreach(Wall wall in BoardLayout.currentBoard.walls)
            {
                if(LineHelperMethods.checkLineRectangleIntersection(playerVisiblityLine, wall.rectangle))
                {
                    return false;
                }
            }

            foreach (ItemPair itemPair in BoardLayout.currentBoard.itemPairs)
            {
                if (LineHelperMethods.checkLineRectangleIntersection(playerVisiblityLine, itemPair.rectangle))
                {
                    return false;
                }
            }

            return true;
        }

        public override void Update()
        {
            playerVisiblityLine = new Line(positionRectangle.Center, Game1.player.positionRectangle.Center);

            if(awareness == awarenessState.unaware)
            {
                if(playerVisiblityLine.Length() <= 160 && isPlayerVisible()) { awareness = awarenessState.aware; }
            }
            else
            {
                if(playerVisiblityLine.Length() >= 240)
                {
                    awareness = awarenessState.unaware;

                    chaseTargets.Clear();
                    chaseTargets.Enqueue(targetPosition);
                }
            }

            if (targetPosition != position)
            {
                if (position.X < targetPosition.X) { position.X += speed; }
                else if (position.X > targetPosition.X) { position.X -= speed; }
                else if (position.Y < targetPosition.Y) { position.Y += speed; }
                else if (position.Y > targetPosition.Y) { position.Y -= speed; }
            }

            Vector2 newTarget = chaseTargets.Peek();

            if (targetPosition == chaseTargets.Peek() && chaseTargets.Count > 1)
            {
                newTarget = chaseTargets.Dequeue();
                Game1.canMove = false;
            }

            if (Vector2.Distance(Game1.player.position, position) < Vector2.Distance(newTarget, position))
            {
                chaseTargets.Clear();
                chaseTargets.Enqueue(Game1.player.targetPosition);
                newTarget = Game1.player.targetPosition;
            }

            if (Game1.canMove)
            {
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

            if (awareness == awarenessState.aware)
            {
                if (targetPosition != Game1.player.targetPosition && Game1.canMove)
                {
                    chaseTargets.Enqueue(Game1.player.targetPosition);
                    Game1.canMove = false;
                }
            }

            base.Update();
        }
    }
}
