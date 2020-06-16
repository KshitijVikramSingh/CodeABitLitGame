using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CodeABitLitGame
{
    public class GameObject
    {
        public Rectangle positionRectangle, targetPositionRectangle, leftRectangle, rightRectangle, upRectangle, downRectangle;
        public Rectangle leftUp, rightUp, leftDown, rightDown;

        public Vector2 position, targetPosition;

        public Texture2D Texture { get; set; }

        public List<Rectangle> rectangles = new List<Rectangle>();
        public List<Rectangle> validRectangles = new List<Rectangle>();

        protected Rectangle sourceRectangle = new Rectangle(0, 0, 48, 48);

        public int health = 1;

        public bool alive = true;

        public Color color = Color.White;

        public virtual void Update()
        {
            positionRectangle = new Rectangle(position.ToPoint(), new Point(48, 48));
            targetPositionRectangle = new Rectangle(targetPosition.ToPoint(), new Point(48, 48));

            leftRectangle = positionRectangle;
            rightRectangle = positionRectangle;
            upRectangle = positionRectangle;
            downRectangle = positionRectangle;

            leftUp = positionRectangle;
            rightUp = positionRectangle;
            leftDown = positionRectangle;
            rightDown = positionRectangle;

            leftRectangle.Offset(new Point(-48, 0));
            rightRectangle.Offset(new Point(48, 0));
            upRectangle.Offset(new Point(0, -48));
            downRectangle.Offset(new Point(0, 48));

            leftUp.Offset(new Point(-48, -48));
            rightUp.Offset(new Point(48, -48));
            leftDown.Offset(new Point(-48, 48));
            rightDown.Offset(new Point(48, 48));

            rectangles = new List<Rectangle>() { positionRectangle, targetPositionRectangle, leftRectangle, rightRectangle, upRectangle, downRectangle, leftUp, rightUp, leftDown, rightDown };
            validRectangles = new List<Rectangle>() { leftRectangle, rightRectangle, upRectangle, downRectangle };

            if (health <= 0) { alive = false; }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(alive)
                spriteBatch.Draw(Texture, position, sourceRectangle, color);
        }
    }
}
