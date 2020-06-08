using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CodeABitLitGame
{
    public class AnimationStrip
    {
        public Texture2D texture;

        public int frameWidth;
        public int frameHeight;
        public int currentFrame;
        int oldFrame;

        public float frameTimer = 0;
        public float frameDelay = 0.06f;

        public bool loopAnimation = true;
        public bool finishedPlaying = false;
        public bool justEnteredFrame = false;

        public string name;
        public string nextAnimation;

        public int frameCount
        {
            get { return texture.Width / frameWidth; }
        }
        public Rectangle frameRectangle
        {
            get { return new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight); }
        }

        public AnimationStrip(Texture2D texture, int frameWidth, string name)
        {
            this.texture = texture;
            this.frameWidth = frameWidth;
            this.frameHeight = texture.Height;
            this.name = name;
        }

        public void Play()
        {
            currentFrame = 0;
            finishedPlaying = false;
        }

        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameTimer += elapsedTime;
            if (frameTimer >= frameDelay)
            {
                currentFrame++;
                if (currentFrame == frameCount)
                {
                    if (loopAnimation)
                    {
                        currentFrame = 0;
                    }
                    else
                    {
                        currentFrame = frameCount - 1;
                        finishedPlaying = true;
                    }
                }
                frameTimer = 0;
            }

            justEnteredFrame = (oldFrame != currentFrame);

            oldFrame = currentFrame;
        }
    }
}
