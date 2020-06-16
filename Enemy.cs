using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CodeABitLitGame
{
    class Enemy : GameObject
    {
        int speed = 8;

        public enum awarenessState { aware, unaware };
        public awarenessState awareness = awarenessState.unaware;

        public Queue<Vector2> chaseTargets;

        Line playerVisiblityLine;

        public bool turnComplete = false;

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

        public void UpdatePosition()
        {
            if (targetPosition != position)
            {
                if (position.X < targetPosition.X) { position.X += speed; }
                else if (position.X > targetPosition.X) { position.X -= speed; }
                else if (position.Y < targetPosition.Y) { position.Y += speed; }
                else if (position.Y > targetPosition.Y) { position.Y -= speed; }
            }

            base.Update();
        }

        public override void Update()
        {
            playerVisiblityLine = new Line(positionRectangle.Center, Game1.player.positionRectangle.Center);

            if(awareness == awarenessState.unaware)
            {
                if(playerVisiblityLine.Length() <= 384  && playerVisiblityLine.angle() % MathHelper.PiOver2 == 0 && isPlayerVisible()) { awareness = awarenessState.aware; }
            }
            else
            {
                if(playerVisiblityLine.Length() >= 480)
                {
                    awareness = awarenessState.unaware;

                    chaseTargets.Clear();
                    chaseTargets.Enqueue(targetPosition);
                }
            }

            if(awareness == awarenessState.aware && !turnComplete && !anyValidRectanglesIntersect(Game1.player))
            {
                Vector2 newTarget = chaseTargets.Peek();

                while (targetPosition == chaseTargets.Peek())
                {
                    if (targetPosition == chaseTargets.Peek() && chaseTargets.Count > 1)
                    {
                        newTarget = chaseTargets.Dequeue();
                    }
                    else { break; }                   
                }

                if (newTarget.X > targetPosition.X && BoardLayout.currentBoard.HasSpaceForMovement(rightRectangle, this))
                {
                    targetPosition.X += 48;
                    turnComplete = true;
                }
                else if (newTarget.X < targetPosition.X && BoardLayout.currentBoard.HasSpaceForMovement(leftRectangle, this))
                {
                    targetPosition.X -= 48;
                    turnComplete = true;
                }
                else if (newTarget.Y < targetPosition.Y && BoardLayout.currentBoard.HasSpaceForMovement(upRectangle, this))
                {
                    targetPosition.Y -= 48;
                    turnComplete = true;
                }
                else if (newTarget.Y > targetPosition.Y && BoardLayout.currentBoard.HasSpaceForMovement(downRectangle, this))
                {
                    targetPosition.Y += 48;
                    turnComplete = true;
                }
                else { turnComplete = true; }
            }
            else
            {
                if (anyValidRectanglesIntersect(Game1.player) && !turnComplete)
                {
                    Game1.player.health--;
                    Game1.PlaySound("Hit");
                    turnComplete = true;
                }
                else
                {
                    chaseTargets.Clear();
                    chaseTargets.Enqueue(Game1.player.targetPosition);
                    turnComplete = true;
                }
            }

            if (awareness == awarenessState.aware)
            {
                if (targetPosition != Game1.player.targetPosition)
                {
                    chaseTargets.Enqueue(Game1.player.targetPosition);
                }
            }
            else { turnComplete = true; }
        }

        bool anyRectanglesIntersect(GameObject gameObject)
        {
            foreach (Rectangle rect in rectangles)
            {
                if (gameObject.positionRectangle.Intersects(rect)) { return true; }
            }

            return false;
        }

        bool anyValidRectanglesIntersect(GameObject gameObject)
        {
            foreach (Rectangle rect in validRectangles)
            {
                if (gameObject.positionRectangle.Intersects(rect)) { return true; }
            }

            return false;
        }        
    }
}
