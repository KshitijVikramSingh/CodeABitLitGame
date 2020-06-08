using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace CodeABitLitGame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static KeyboardState newState, oldState;

        BoardLayout boardLayout;

        public static Dictionary<string, string> itemPairDictionary;

        public static Player player;

        public static bool canMove = true;

        int LevelCount = 0;

        float inputTimer, inputDelay = 5;

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

            itemPairDictionary = new Dictionary<string, string>();
            itemPairDictionary.Add("Key", "Door");
            itemPairDictionary.Add("Axe", "Wall");
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

            inputTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            newState = Keyboard.GetState();

            if(inputTimer >= inputDelay)
            {
                if(JustKeyPressed(Keys.A) || JustKeyPressed(Keys.D) || JustKeyPressed(Keys.W) || JustKeyPressed(Keys.S)) { canMove = true; }

                callObjectUpdates();

                inputTimer = 0;
            }

            if(player.currentItem != null && player.currentItemPair != null && itemPairDictionary[player.currentItem.Name] == player.currentItemPair.Name)
            {
                boardLayout.items.Remove(player.currentItem);
                boardLayout.itemPairs.Remove(player.currentItemPair);

                player.currentItem = null;
                player.currentItemPair = null;
            }

            oldState = newState;

            base.Update(gameTime);
        }

        void callObjectUpdates()
        {
            player.Update();

            foreach(Enemy enemy in boardLayout.enemies)
            {
                //enemy.Update();
            }

            foreach (Item item in boardLayout.items)
            {
                item.Update();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.AntiqueWhite);

            spriteBatch.Begin();

            player.Draw(spriteBatch);

            foreach(Enemy enemy in boardLayout.enemies)
            {
                enemy.Draw(spriteBatch);
            }

            foreach(Wall wall in boardLayout.walls)
            {
                wall.Draw(spriteBatch);
            }

            foreach (Item item in boardLayout.items)
            {
                item.Draw(spriteBatch);
            }

            foreach (ItemPair itemPair in boardLayout.itemPairs)
            {
                itemPair.Draw(spriteBatch);
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
