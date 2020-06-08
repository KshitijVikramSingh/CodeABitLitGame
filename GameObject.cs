using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CodeABitLitGame
{
    public class GameObject
    {
        public Rectangle positionRectangle, leftRectangle, rightRectangle, upRectangle, downRectangle;

        public Vector2 position, targetPosition;

        public Texture2D Texture { get; set; }

        public virtual void Update()
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
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, position, Color.White);
        }
    }
}
