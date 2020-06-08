using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CodeABitLitGame
{
    public class GameObjects
    {
        public int frameWidth;
        public int frameHeight;
        public bool flipped = false;
        public bool flippedV = false;
        public float scale = 1f;
        public Color color = Color.White;

        public float rotation = 0;
        public Vector2 origin = Vector2.Zero;

        public Vector2 position;

        public Rectangle rect;
        public Rectangle attackRect;
        public Rectangle swordRectangle;

        public bool onGround = true;
        public bool justEnteredAnimation = false;

        public Rectangle positionRectangle
        {
            get
            {
                return new Rectangle(position.ToPoint(), new Point(frameWidth, frameHeight));
            }
        }

        public Rectangle rectangle
        {
            get
            {
                return new Rectangle(position.ToPoint(), new Point(animations[currentAnimation].frameWidth * (int)scale, animations[currentAnimation].frameHeight * (int)scale));
            }
            set
            {
                rectangle = value;
            }
        }

        public Dictionary<string, AnimationStrip> animations = new Dictionary<string, AnimationStrip>();
        public string currentAnimation;

        public void UpdateAnimation(GameTime gameTime)
        {
            if (animations.ContainsKey(currentAnimation))
            {
                if (animations[currentAnimation].finishedPlaying)
                {
                    PlayAnimation(animations[currentAnimation].nextAnimation);
                }
                else
                {
                    animations[currentAnimation].Update(gameTime);
                }
            }
        }

        public void PlayAnimation(string name)
        {
            if (!(name == null) && animations.ContainsKey(name))
            {
                currentAnimation = name;
                animations[name].Play();
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            justEnteredAnimation = animations[currentAnimation].currentFrame == 0 && animations[currentAnimation].justEnteredFrame;

            UpdateAnimation(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch, float XoffSet = 0)
        {
            if (animations.ContainsKey(currentAnimation))
            {
                SpriteEffects spriteEffects = SpriteEffects.None;

                if (flipped)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
                if (flippedV)
                {
                    spriteEffects = SpriteEffects.FlipVertically;
                }

                spriteBatch.Draw(animations[currentAnimation].texture, position - new Vector2(XoffSet, 0), animations[currentAnimation].frameRectangle, color, rotation, origin, scale, spriteEffects, 0);
            }
        }
    }
}