using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace CodeABitLitGame
{
    public class Player : GameObject
    {        
        int speed = 4;

        public Item currentItem;
        public ItemPair currentItemPair;

        public int health = 1;

        public Player(ContentManager content, Vector2 position)
        {
            Texture = content.Load<Texture2D>("Player");

            this.position = position;
            targetPosition = position;

            currentItem = null;
            currentItemPair = null;
        }

        public override void Update()
        {
            if(targetPosition == position)
            {
                if (Game1.JustKeyPressed(Keys.D) && BoardLayout.currentBoard.HasSpaceForMovement(rightRectangle, this))
                {
                    targetPosition.X += 32;
                }
                else if (Game1.JustKeyPressed(Keys.A) && BoardLayout.currentBoard.HasSpaceForMovement(leftRectangle, this))
                {
                    targetPosition.X -= 32;
                }
                else if (Game1.JustKeyPressed(Keys.W) && BoardLayout.currentBoard.HasSpaceForMovement(upRectangle, this))
                {
                    targetPosition.Y -= 32;
                }
                else if (Game1.JustKeyPressed(Keys.S) && BoardLayout.currentBoard.HasSpaceForMovement(downRectangle, this))
                {
                    targetPosition.Y += 32;
                }
            }
            else
            {
                if (position.X < targetPosition.X) { position.X += speed; }
                else if (position.X > targetPosition.X) { position.X -= speed; }
                else if (position.Y < targetPosition.Y) { position.Y += speed; }
                else if (position.Y > targetPosition.Y) { position.Y -= speed; }
            }

            //if(currentItem != null) Console.WriteLine(currentItem.Name);
            //if(currentItemPair != null) Console.WriteLine(currentItemPair.Name);

            base.Update();
        }
    }
}
