using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;

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

        int LevelCount, MaxLevels = 6;

        float inputTimer, inputDelay = 5;

        public enum  Turn { player, enemies };
        public static Turn currentTurn;

        public static Dictionary<string, SoundEffectInstance> Sounds;
        public Dictionary<string, Music> Music;

        public static bool muteMusic = false, muteSounds = false;

        enum GameState { loading, menu, play, end };
        GameState currentState = GameState.loading;

        SpriteFont font, fontLarge;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize()
        {
            currentTurn = Turn.player;

            LevelCount = 1;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Font");
            fontLarge = Content.Load<SpriteFont>("FontLarge");

            Thread loadOnBackground = new Thread(new ThreadStart(LoadGameContent));

            loadOnBackground.IsBackground = true;

            loadOnBackground.Start();
        }

        void LoadGameContent()
        {
            itemPairDictionary = new Dictionary<string, string>();
            itemPairDictionary.Add("Key", "Door");
            itemPairDictionary.Add("Axe", "Tree");

            LoadSounds();

            currentState = GameState.menu;

            PlayMusic("Menu");
        }

        void ChangeLevel()
        {
            boardLayout = new BoardLayout(Content, "LevelLayouts/Level" + LevelCount.ToString());

            player = new Player(Content, boardLayout.playerPosition);
        }

        void LoadSounds()
        {
            Sounds = new Dictionary<string, SoundEffectInstance>();
            Music = new Dictionary<string, Music>();

            Sounds.Add("Attack", Content.Load<SoundEffect>("Sounds/Attack").CreateInstance());
            Sounds.Add("Bag", Content.Load<SoundEffect>("Sounds/Bag").CreateInstance());
            Sounds.Add("Hit", Content.Load<SoundEffect>("Sounds/Hit").CreateInstance());
            Sounds.Add("Step", Content.Load<SoundEffect>("Sounds/Step").CreateInstance());
            Sounds.Add("Tree", Content.Load<SoundEffect>("Sounds/Tree").CreateInstance());
            Sounds.Add("Unlock", Content.Load<SoundEffect>("Sounds/Unlock").CreateInstance());            

            Music.Add("Menu", new Music("Music/Menu"));
            Music.Add("Game", new Music("Music/Game"));
            Music.Add("End", new Music("Music/End"));
        }

        protected override void UnloadContent()
        {
            
        }

        public static bool JustKeyPressed(Keys key)
        {
            return newState.IsKeyDown(key) && oldState.IsKeyUp(key);
        }

        public static void PlaySound(string name)
        {
            if (Sounds.ContainsKey(name) && !muteSounds)
            {
                Sounds[name].Play();
            }
        }

        public void PlayMusic(string name)
        {
            if(Music.ContainsKey(name) && !muteMusic)
            {
                foreach(string names in Music.Keys)
                {
                    if(names != name) { Music[names].Stop(); }
                }

                Music[name].Play();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter))
                ChangeLevel();

            inputTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            newState = Keyboard.GetState();

            if(currentState == GameState.play)
            {
                UpdateGame();
            }

            if(currentState == GameState.menu)
            {
                if (newState.IsKeyUp(Keys.Enter) && oldState.IsKeyDown(Keys.Enter))
                {
                    currentState = GameState.play;
                    PlayMusic("Game");
                }
            }

            if (currentState == GameState.end)
            {
                if (newState.IsKeyUp(Keys.Enter) && oldState.IsKeyDown(Keys.Enter))
                {
                    Initialize();
                }
            }

            oldState = newState;

            base.Update(gameTime);
        }

        void UpdateGame()
        {
            if (inputTimer >= inputDelay)
            {
                callObjectUpdates();

                inputTimer = 0;
            }

            if(player.health <= 0) { ChangeLevel(); }
        }

        void callObjectUpdates()
        {
            if (currentTurn == Turn.player && player.currentItem != null && player.currentItemPair != null && itemPairDictionary[player.currentItem.Name] == player.currentItemPair.Name)
            {
                if (player.currentItem.Name == "Key")
                {
                    boardLayout.items.Remove(player.currentItem);
                    player.currentItem = null;

                    PlaySound("Unlock");
                }
                else
                {
                    PlaySound("Tree");
                }
                
                boardLayout.itemPairs.Remove(player.currentItemPair);
                player.currentItemPair = null;
            }

            for (int i = boardLayout.enemies.Count - 1; i >= 0; i--)
            {
                boardLayout.enemies[i].UpdatePosition();

                if (!boardLayout.enemies[i].alive)
                {
                    boardLayout.enemies.RemoveAt(i);
                }
            }

            if (currentTurn == Turn.enemies)
            {
                for (int i = 0; i < boardLayout.enemies.Count; i++)
                {
                    if(i == 0 || (boardLayout.enemies[i - 1].turnComplete && boardLayout.enemies[i - 1].targetPosition == boardLayout.enemies[i - 1].position))
                        boardLayout.enemies[i].Update();
                }

                bool turnComplete = true;

                foreach(Enemy enemy in boardLayout.enemies)
                {
                    if (!enemy.turnComplete)
                    {
                        turnComplete = false;
                        break;
                    }
                }

                if (turnComplete) { currentTurn = Turn.player; }
            }

            foreach (Item item in boardLayout.items)
            {
                item.Update();
            }

            if(currentTurn == Turn.player)
            {
                player.Update();

                foreach(TexturedRect texturedRect in boardLayout.texturedRects)
                {
                    if (texturedRect.tag == "End" && player.position == player.targetPosition && player.targetPositionRectangle.Intersects(texturedRect.rect))
                    {
                        if (LevelCount != MaxLevels)
                        {
                            LevelCount++;
                            ChangeLevel();
                        }
                        else
                        {
                            currentState = GameState.end;
                            PlayMusic("End");
                        }
                    }
                }

                foreach(Enemy enemy in boardLayout.enemies) { enemy.turnComplete = false; }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0, 9, 11));

            if (currentState == GameState.play)
            {
                DrawGame();
            }
            else
            {
                DrawStates();
            }

            base.Draw(gameTime);
        }

        void DrawStates()
        {
            string text = "";

            if (currentState == GameState.loading)
            {
                text = "LOADING...";
            }
            else if(currentState == GameState.menu)
            {
                text = "PRESS ENTER TO PLAY";
            }
            else if(currentState == GameState.end)
            {
                text = "THE END...? PRESS ENTER";
            }

            spriteBatch.Begin();

            spriteBatch.DrawString(fontLarge, "TINY DUNGEON", new Vector2(600, 180) - fontLarge.MeasureString("TINY DUNGEON") / 2, new Color(215, 0, 70));

            spriteBatch.DrawString(font, text, new Vector2(600, 540) - font.MeasureString(text) / 2, new Color(193, 179, 168) * Math.Abs((float)Math.Sin(inputTimer / 1000)));

            spriteBatch.End();
        }

        void DrawGame()
        {
            spriteBatch.Begin();

            foreach (TexturedRect texturedRect in boardLayout.texturedRects)
            {
                texturedRect.Draw(spriteBatch);
            }

            player.Draw(spriteBatch);

            foreach (Enemy enemy in boardLayout.enemies)
            {
                enemy.Draw(spriteBatch);
            }

            foreach (Wall wall in boardLayout.walls)
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
        }
    }

    class Wall
    {
        Texture2D texture;
        Vector2 position;

        public Rectangle rectangle;

        Rectangle sourceRect;

        public Wall(Texture2D texture, Vector2 position, int sourceImageIndex)
        {
            this.texture = texture;
            this.position = position;

            rectangle = new Rectangle(position.ToPoint(), new Point(48, 48));
            sourceRect = new Rectangle(sourceImageIndex * 48, 0, 48, 48);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, sourceRect, Color.White);
        }
    }

    class TexturedRect
    {
        public Texture2D texture;
        public Rectangle rect;
        public Vector2 position;
        public string tag;

        public TexturedRect(Texture2D texture, Vector2 position, string tag)
        {
            this.texture = texture;
            this.position = position;
            rect = new Rectangle(position.ToPoint(), new Point(texture.Width, texture.Height));
            this.tag = tag;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
