using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace CodeABitLitGame
{
    public class Player
    {
        public Rectangle positionRectangle, leftRectangle, rightRectangle, upRectangle, downRectangle;

        Texture2D texture;

        public Vector2 position, targetPosition;

        int speed = 4;

        public Player(ContentManager content, Vector2 position)
        {
            texture = content.Load<Texture2D>("Player");

            this.position = position;
            targetPosition = position;
        }

        public void Update()
        {
            positionRectangle = new Rectangle(position.ToPoint(), new Point(32, 32));

            leftRectangle = positionRectangle;
            rightRectangle = positionRectangle;
            upRectangle = positionRectangle;
            downRectangle = positionRectangle;

            leftRectangle.Offset(new Point(-16, 0));
            rightRectangle.Offset(new Point(16, 0));
            upRectangle.Offset(new Point(0, -16));
            downRectangle.Offset(new Point(0, 16));

            if(targetPosition == position)
            {
                if (Game1.JustKeyPressed(Keys.D) && BoardLayout.currentBoard.HasSpaceForMovement(rightRectangle))
                {
                    targetPosition.X += 32;
                    Game1.canMove = true;
                }
                else if (Game1.JustKeyPressed(Keys.A) && BoardLayout.currentBoard.HasSpaceForMovement(leftRectangle))
                {
                    targetPosition.X -= 32;
                    Game1.canMove = true;
                }
                else if (Game1.JustKeyPressed(Keys.W) && BoardLayout.currentBoard.HasSpaceForMovement(upRectangle))
                {
                    targetPosition.Y -= 32;
                    Game1.canMove = true;
                }
                else if (Game1.JustKeyPressed(Keys.S) && BoardLayout.currentBoard.HasSpaceForMovement(downRectangle))
                {
                    targetPosition.Y += 32;
                    Game1.canMove = true;
                }
            }
            else
            {
                if (position.X < targetPosition.X) { position.X += speed; }
                else if (position.X > targetPosition.X) { position.X -= speed; }
                else if (position.Y < targetPosition.Y) { position.Y += speed; }
                else if (position.Y > targetPosition.Y) { position.Y -= speed; }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
