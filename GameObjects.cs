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
        public Vector2 velocity, acceleration;

        public bool physicsBody = false;

        public List<Vector2> positionsList = new List<Vector2>();
        public List<Vector2> velocityList = new List<Vector2>();
        public List<Vector2> accelerationList = new List<Vector2>();
        public List<string> animationsList = new List<string>();
        public List<int> framesList = new List<int>();
        public List<bool> flippedList = new List<bool>();
        public List<bool> onGroundList = new List<bool>();

        public Rectangle rect;
        public Rectangle attackRect;
        public Rectangle swordRectangle;

        public bool onGround = true;
        public bool justEnteredAnimation = false;
        public bool rewinding = false;

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
            Rewind();

            if (physicsBody && !rewinding)
            {
                if (!onGround)
                {
                    acceleration.Y = 1.25f;
                }
                else
                {
                    velocity.Y = 0;
                    acceleration.Y = 0;
                }

                position += velocity;
                velocity += acceleration;
            }

            justEnteredAnimation = animations[currentAnimation].currentFrame == 0 && animations[currentAnimation].justEnteredFrame;

            UpdateAnimation(gameTime);
        }

        public void Rewind()
        {
            if (!rewinding)
            {
                positionsList.Add(position);

                velocityList.Add(velocity);

                accelerationList.Add(acceleration);

                animationsList.Add(currentAnimation);

                framesList.Add(animations[currentAnimation].currentFrame);

                flippedList.Add(flipped);

                onGroundList.Add(onGround);
            }
            else
            {
                position = positionsList[positionsList.Count - 1];
                if (positionsList.Count > 1)
                {
                    positionsList.RemoveAt(positionsList.Count - 1);
                }

                velocity = velocityList[velocityList.Count - 1];
                if (velocityList.Count > 1)
                {
                    velocityList.RemoveAt(velocityList.Count - 1);
                }

                acceleration = accelerationList[accelerationList.Count - 1];
                if (accelerationList.Count > 1)
                {
                    accelerationList.RemoveAt(accelerationList.Count - 1);
                }

                currentAnimation = animationsList[animationsList.Count - 1];
                if (animationsList.Count > 1)
                {
                    animationsList.RemoveAt(animationsList.Count - 1);
                }

                animations[currentAnimation].currentFrame = framesList[framesList.Count - 1];
                if (framesList.Count > 1)
                {
                    framesList.RemoveAt(framesList.Count - 1);
                }

                flipped = flippedList[flippedList.Count - 1];
                if (flippedList.Count > 1)
                {
                    flippedList.RemoveAt(flippedList.Count - 1);
                }

                onGround = onGroundList[onGroundList.Count - 1];
                if (onGroundList.Count > 1)
                {
                    onGroundList.RemoveAt(onGroundList.Count - 1);
                }
            }
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