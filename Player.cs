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
        int speed = 6;

        public Item currentItem;
        public ItemPair currentItemPair;

        public bool canMakeMove = true;        

        public Player(ContentManager content, Vector2 position)
        {
            health = 4;

            Texture = content.Load<Texture2D>("Player");

            this.position = position;
            targetPosition = position;

            currentItem = null;
            currentItemPair = null;
        }

        public override void Update()
        {
            sourceRectangle = new Rectangle((health - 1) * 48, 0, 48, 48);

            if(targetPosition == position && canMakeMove)
            {
                if ((Game1.JustKeyPressed(Keys.D) || Game1.JustKeyPressed(Keys.Right)) && BoardLayout.currentBoard.HasSpaceForMovement(rightRectangle, this))
                {
                    targetPosition.X += 48;
                    Game1.PlaySound("Step");
                    canMakeMove = false;
                }
                else if ((Game1.JustKeyPressed(Keys.A) || Game1.JustKeyPressed(Keys.Left)) && BoardLayout.currentBoard.HasSpaceForMovement(leftRectangle, this))
                {
                    targetPosition.X -= 48;
                    Game1.PlaySound("Step");
                    canMakeMove = false;
                }
                else if ((Game1.JustKeyPressed(Keys.W) || Game1.JustKeyPressed(Keys.Up)) && BoardLayout.currentBoard.HasSpaceForMovement(upRectangle, this))
                {
                    targetPosition.Y -= 48;
                    Game1.PlaySound("Step");
                    canMakeMove = false;
                }
                else if ((Game1.JustKeyPressed(Keys.S) || Game1.JustKeyPressed(Keys.Down)) && BoardLayout.currentBoard.HasSpaceForMovement(downRectangle, this))
                {
                    targetPosition.Y += 48;
                    Game1.PlaySound("Step");
                    canMakeMove = false;
                }
            }
            else
            {
                if (position.X < targetPosition.X) { position.X += speed; }
                else if (position.X > targetPosition.X) { position.X -= speed; }
                else if (position.Y < targetPosition.Y) { position.Y += speed; }
                else if (position.Y > targetPosition.Y) { position.Y -= speed; }
            }

            if(targetPosition == position && !canMakeMove)
            {
                Game1.currentTurn = Game1.Turn.enemies;
                canMakeMove = true;
            }

            base.Update();
        }
    }
}
