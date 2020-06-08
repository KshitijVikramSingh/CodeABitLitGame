using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CodeABitLitGame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static KeyboardState newState, oldState;

        BoardLayout boardLayout;

        Player player;

        int LevelCount = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            boardLayout = new BoardLayout(Content, "Level" + LevelCount.ToString());

            player = new Player(Content, boardLayout.playerPosition);
        }

        protected override void UnloadContent()
        {
            
        }

        public static bool JustKeyPressed(Keys key)
        {
            return newState.IsKeyDown(key) && oldState.IsKeyUp(key);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            newState = Keyboard.GetState();

            player.Update();

            oldState = newState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.AntiqueWhite);

            spriteBatch.Begin();

            player.Draw(spriteBatch);

            foreach(Wall wall in boardLayout.walls)
            {
                wall.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    class Wall
    {
        Texture2D texture;
        Vector2 position;

        public Rectangle rectangle;

        public Wall(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;

            rectangle = new Rectangle(position.ToPoint(), new Point(32, 32));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
