using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace CodeABitLitGame
{
    class Player
    {
        public Rectangle positionRectangle, leftRectangle, rightRectangle, upRectangle, downRectangle;

        Texture2D texture;

        public Vector2 position;

        public Player(ContentManager content, Vector2 position)
        {
            texture = content.Load<Texture2D>("Player");

            this.position = position;
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

            if (Game1.JustKeyPressed(Keys.D) && BoardLayout.currentBoard.HasSpaceForMovement(rightRectangle))
            {
                position.X += 32;
            }
            else if (Game1.JustKeyPressed(Keys.A) && BoardLayout.currentBoard.HasSpaceForMovement(leftRectangle))
            {
                position.X -= 32;
            }
            else if (Game1.JustKeyPressed(Keys.W) && BoardLayout.currentBoard.HasSpaceForMovement(upRectangle))
            {
                position.Y -= 32;
            }
            else if (Game1.JustKeyPressed(Keys.S) && BoardLayout.currentBoard.HasSpaceForMovement(downRectangle))
            {
                position.Y += 32;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
