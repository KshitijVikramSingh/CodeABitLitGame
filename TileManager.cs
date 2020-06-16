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
            LevelManager.LoadMapDataFromImage(content, Name);
            //LevelManager.LoadMapDataFromTextFile(Name);

            string[,] levelLayout = LevelManager.LevelLayout;

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

                    if (levelLayout[x, y] == "#")
                    {
                        int index = FindSourceImageIndex(x, y);

                        walls.Add(new Wall(content.Load<Texture2D>("Wall"), tilePosition, index));
                    }

                    if (levelLayout[x, y] == "!")
                    {
                        enemies.Add(new Enemy(content, tilePosition));
                    }

                    if (levelLayout[x, y] == "K")
                    {
                        items.Add(new Item(content.Load<Texture2D>("Key"), tilePosition, "Key"));
                    }

                    if (levelLayout[x, y] == "D")
                    {
                        itemPairs.Add(new ItemPair(content.Load<Texture2D>("Door"), tilePosition, "Door"));
                    }

                    if (levelLayout[x, y] == "X")
                    {
                        items.Add(new Item(content.Load<Texture2D>("Axe"), tilePosition, "Axe"));
                    }

                    if (levelLayout[x, y] == "^")
                    {
                        itemPairs.Add(new ItemPair(content.Load<Texture2D>("Tree"), tilePosition, "Tree"));
                    }

                    if (levelLayout[x, y] == "@")
                    {
                        texturedRects.Add(new TexturedRect(content.Load<Texture2D>("Start"), tilePosition, "Start"));
                        playerPosition = tilePosition;
                    }

                    if (levelLayout[x, y] == "*")
                    {
                        texturedRects.Add(new TexturedRect(content.Load<Texture2D>("End"), tilePosition, "End"));
                    }
                }
            }

            currentBoard = this;
        }

        int FindSourceImageIndex(int X, int Y)
        {
            var level = LevelManager.LevelLayout;

            int bitMask = 0b00000000;

            if (level[X - 1, Y - 1] == "#") { bitMask |= 0b10000000; }
            if (level[X    , Y - 1] == "#") { bitMask |= 0b01000000; }
            if (level[X + 1, Y - 1] == "#") { bitMask |= 0b00100000; }
            if (level[X - 1, Y    ] == "#") { bitMask |= 0b00010000; }
            if (level[X + 1, Y    ] == "#") { bitMask |= 0b00001000; }
            if (level[X - 1, Y + 1] == "#") { bitMask |= 0b00000100; }
            if (level[X    , Y + 1] == "#") { bitMask |= 0b00000010; }
            if (level[X + 1, Y + 1] == "#") { bitMask |= 0b00000001; }

            return bitMask;
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
                        }
                        else if(!enemy.turnComplete) { Game1.player.health--; enemy.turnComplete = true; }

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

        public static string[,] LevelLayout;

        public static void LoadMapDataFromImage(ContentManager content, string Name)
        {
            Texture2D mapTexture = content.Load<Texture2D>(Name);

            Color[] mapRawData = new Color[mapTexture.Width * mapTexture.Height];

            mapTexture.GetData(mapRawData);

            int Width = mapTexture.Width;
            int Height = mapTexture.Height;

            LevelLayout = new string[Width, Height];

            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    if (mapRawData[row * Width + column].A != 0)
                    {
                        if (mapRawData[row * Width + column].ToVector4() == new Vector4(0, 0, 0, 1))
                        {
                            LevelLayout[column, row] = "#";
                        }

                        if (mapRawData[row * Width + column].ToVector4() == new Vector4(1, 0, 0, 1))
                        {
                            LevelLayout[column, row] = "!";
                        }

                        if (mapRawData[row * Width + column].ToVector4() == new Vector4(1, 1, 0, 1))
                        {
                            LevelLayout[column, row] = "K";
                        }

                        if (mapRawData[row * Width + column].ToVector4() == new Vector4(0, 0, 1, 1))
                        {
                            LevelLayout[column, row] = "D";
                        }

                        if (mapRawData[row * Width + column].ToVector4() == new Vector4(0, 1, 1, 1))
                        {
                            LevelLayout[column, row] = "X";
                        }

                        if (mapRawData[row * Width + column].ToVector4() == new Vector4(0, 1, 0, 1))
                        {
                            LevelLayout[column, row] = "^";
                        }

                        if (mapRawData[row * Width + column].ToVector4() == new Vector4(1, 1, 1, 1))
                        {
                            LevelLayout[column, row] = "@";
                        }

                        if (mapRawData[row * Width + column].PackedValue == Color.DarkSlateBlue.PackedValue)
                        {
                            LevelLayout[column, row] = "*";
                        }
                    }
                    else if (mapRawData[row * Width + column].A == 0)
                    {
                        LevelLayout[column, row] = ".";
                    }
                }
            }

            Console.WriteLine(new Color(1, 0, 1, 1));

            for (int row = 0;  row < Height; row++)
            {
                for(int column = 0; column < Width; column++)
                {
                    Console.Write(LevelLayout[column, row] + ",");
                }

                Console.WriteLine();
            }
        }

        public static void LoadMapDataFromTextFile(string Name)
        {
            string path = "Content/" + Name + ".txt";

            int height = File.ReadLines(path).Count();

            StreamReader stream = new StreamReader(path);

            string line = stream.ReadLine();

            string[] tileNo = line.Split(',');

            stream.Close();

            int width = tileNo.Count();

            LevelLayout = new string[width, height];

            stream = new StreamReader(path);

            for (int y = 0; y < height; y++)
            {
                line = stream.ReadLine();

                tileNo = line.Split(',');

                for (int x = 0; x < width; x++)
                {
                    LevelLayout[x, y] = tileNo[x];
                }
            }
        }
    }
}
