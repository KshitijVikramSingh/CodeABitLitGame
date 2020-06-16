using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Linq;

namespace CodeABitLitGame
{
    class BoardLayout
    {
        public static BoardLayout currentBoard { get; private set; }

        public List<Wall> walls;
        public List<Enemy> enemies;
        public List<Item> items;
        public List<ItemPair> itemPairs;
        public List<TexturedRect> texturedRects;

        public Vector2 playerPosition;

        public BoardLayout(ContentManager content, string Name)
        {
            LevelManager.LoadMapDataFromCSVFile(Name);

            int[,] levelLayout = LevelManager.LevelLayout;

            walls = new List<Wall>();
            enemies = new List<Enemy>();
            items = new List<Item>();
            itemPairs = new List<ItemPair>();
            texturedRects = new List<TexturedRect>();

            for (int x = 0; x < LevelManager.MapWidth; x++)
            {
                for (int y = 0; y < LevelManager.MapHeight; y++)
                {
                    Vector2 tilePosition = new Vector2(x * LevelManager.TileWidth, y * LevelManager.TileHeight);

                    if (levelLayout[x, y] >= 0 && levelLayout[x, y] <= 12)
                    {
                        walls.Add(new Wall(content.Load<Texture2D>("Wall"), tilePosition, levelLayout[x, y]));
                    }

                    if (levelLayout[x, y] == 14)
                    {
                        enemies.Add(new Enemy(content, tilePosition));
                    }

                    if (levelLayout[x, y] == 15)
                    {
                        items.Add(new Item(content.Load<Texture2D>("Key"), tilePosition, "Key"));
                    }

                    if (levelLayout[x, y] == 16)
                    {
                        itemPairs.Add(new ItemPair(content.Load<Texture2D>("Door"), tilePosition, "Door"));
                    }

                    if (levelLayout[x, y] == 17)
                    {
                        items.Add(new Item(content.Load<Texture2D>("Axe"), tilePosition, "Axe"));
                    }

                    if (levelLayout[x, y] == 18)
                    {
                        itemPairs.Add(new ItemPair(content.Load<Texture2D>("Tree"), tilePosition, "Tree"));
                    }

                    if (levelLayout[x, y] == 19)
                    {
                        texturedRects.Add(new TexturedRect(content.Load<Texture2D>("Start"), tilePosition, "Start"));
                        playerPosition = tilePosition;
                    }

                    if (levelLayout[x, y] == 20)
                    {
                        texturedRects.Add(new TexturedRect(content.Load<Texture2D>("End"), tilePosition, "End"));
                    }
                }
            }

            currentBoard = this;
        }

        public bool HasSpaceForMovement(Rectangle rectangle, GameObject gameObject)
        {
            foreach(Wall wall in walls)
            {
                if (wall.rectangle.Intersects(rectangle))
                {
                    return false;
                }
            }

            foreach (ItemPair itemPair in itemPairs)
            {
                if (itemPair.rectangle.Intersects(rectangle))
                {
                    if(gameObject is Player)
                        Game1.player.currentItemPair = itemPair;

                    return false;
                }
            }

            Game1.player.currentItemPair = null;

            if(gameObject is Enemy)
            {
                if (Game1.player.positionRectangle.Intersects(rectangle))
                {
                    return false;
                }

                foreach (GameObject enemy in enemies)
                {
                    if (enemy is Enemy)
                    {
                        Enemy en = enemy as Enemy;

                        if (en.targetPositionRectangle.Intersects(rectangle))
                        {
                            enemies[enemies.IndexOf(gameObject as Enemy)].turnComplete = true;
                            return false;
                        }
                    }
                }
            }
            else
            {
                foreach (Enemy enemy in enemies)
                {
                    if (enemy.positionRectangle.Intersects(rectangle))
                    {
                        if (Game1.player.currentItem != null && Game1.player.currentItem.Name == "Axe")
                        {
                            enemy.health--;
                            Game1.PlaySound("Attack");
                            Game1.player.canMakeMove = false;
                        }
                        else if(!enemy.turnComplete)
                        {
                            Game1.player.health--;
                            Game1.PlaySound("Hit");
                            enemy.turnComplete = true;
                        }

                        return false;
                    }
                }
            }

            return true;
        }
    }

    class LevelManager
    {
        public const int TileWidth = 48;
        public const int TileHeight = 48;

        public const int MapWidth = 25;
        public const int MapHeight = 15;

        public static int[,] LevelLayout;

        public static void LoadMapDataFromCSVFile(string Name)
        {
            string path = "Content/" + Name + ".csv";

            int height = File.ReadLines(path).Count();

            StreamReader stream = new StreamReader(path);

            string line = stream.ReadLine();

            string[] tileNo = line.Split(',');

            stream.Close();

            int width = tileNo.Count();

            LevelLayout = new int[width, height];

            stream = new StreamReader(path);

            for (int y = 0; y < height; y++)
            {
                line = stream.ReadLine();

                tileNo = line.Split(',');

                for (int x = 0; x < width; x++)
                {
                    LevelLayout[x, y] = int.Parse(tileNo[x]);
                }
            }
        }
    }
}
