using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CodeABitLitGame
{
    class Enemy
    {
        Texture2D texture;
        Vector2 position, targetPosition;

        int speed = 4;

        public enum awarenessState { aware, unaware };
        public awarenessState awareness = awarenessState.aware;

        public Queue<Vector2> chaseTargets;

        public Enemy(ContentManager content, Vector2 position)
        {
            texture = content.Load<Texture2D>("Enemy");

            this.position = position;
            targetPosition = position;

            chaseTargets = new Queue<Vector2>();
            chaseTargets.Enqueue(targetPosition);
        }

        public void Update()
        {
            targetPosition = chaseTargets.Peek();

            if (targetPosition != position)
            {
                if (position.X < targetPosition.X) { position.X += speed; }
                else if (position.X > targetPosition.X) { position.X -= speed; }
                else if (position.Y < targetPosition.Y) { position.Y += speed; }
                else if (position.Y > targetPosition.Y) { position.Y -= speed; }
            }
            else
            {
                if (Game1.canMove && chaseTargets.Count > 1)
                {
                    chaseTargets.Dequeue();
                    Game1.canMove = false;
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
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
