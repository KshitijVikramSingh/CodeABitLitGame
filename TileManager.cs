using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CodeABitLitGame
{
    class BoardLayout
    {
        public static BoardLayout currentBoard { get; private set; }

        public List<Wall> walls;
        public List<Enemy> enemies;
        public List<Item> items;
        public List<ItemPair> itemPairs;

        public Vector2 playerPosition;

        public BoardLayout(ContentManager content, string Name)
        {
            LevelManager.LoadMapDataFromImage(content, Name);

            char[,] levelLayout = LevelManager.LevelLayout;

            walls = new List<Wall>();
            enemies = new List<Enemy>();
            items = new List<Item>();
            itemPairs = new List<ItemPair>();

            for (int x = 0; x < LevelManager.MapWidth; x++)
            {
                for (int y = 0; y < LevelManager.MapHeight; y++)
                {
                    Vector2 tilePosition = new Vector2(x * LevelManager.TileWidth, y * LevelManager.TileHeight);

                    if (levelLayout[x, y] == '#')
                    {
                        walls.Add(new Wall(content.Load<Texture2D>("Wall"), tilePosition));
                    }

                    if (levelLayout[x, y] == '!')
                    {
                        enemies.Add(new Enemy(content, tilePosition));
                    }

                    if (levelLayout[x, y] == 'K')
                    {
                        items.Add(new Item(content.Load<Texture2D>("Key"), tilePosition, "Key"));
                    }

                    if (levelLayout[x, y] == 'D')
                    {
                        itemPairs.Add(new ItemPair(content.Load<Texture2D>("Door"), tilePosition, "Door"));
                    }

                    if (levelLayout[x, y] == '@')
                    {
                        playerPosition = tilePosition;
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
                    Game1.player.currentItemPair = itemPair;
                    return false;
                }
            }

            Game1.player.currentItemPair = null;

            if(gameObject is Enemy)
            {
                if (Game1.player.positionRectangle.Intersects(rectangle))
                {
                    Game1.player.health--;
                    return false;
                }
            }
            else
            {
                foreach (GameObject enemy in enemies)
                {
                    if (enemy.positionRectangle.Intersects(rectangle))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    class LevelManager
    {
        public const int TileWidth = 32;
        public const int TileHeight = 32;

        public const int MapWidth = 25;
        public const int MapHeight = 15;

        public static char[,] LevelLayout;

        public static void LoadMapDataFromImage(ContentManager content, string Name)
        {
            Texture2D mapTexture = content.Load<Texture2D>(Name);

            Color[] mapRawData = new Color[mapTexture.Width * mapTexture.Height];

            mapTexture.GetData(mapRawData);

            int Width = mapTexture.Width;
            int Height = mapTexture.Height;

            LevelLayout = new char[Width, Height];

            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    if (mapRawData[row * Width + column].A != 0)
                    {
                        if (mapRawData[row * Width + column].ToVector4() == new Vector4(0, 0, 0, 1))
                        {
                            LevelLayout[column, row] = '#';
                        }

                        if (mapRawData[row * Width + column].ToVector4() == new Vector4(1, 0, 0, 1))
                        {
                            LevelLayout[column, row] = '!';
                        }

                        if (mapRawData[row * Width + column].ToVector4() == new Vector4(1, 1, 0, 1))
                        {
                            LevelLayout[column, row] = 'K';
                        }

                        if (mapRawData[row * Width + column].ToVector4() == new Vector4(0, 0, 1, 1))
                        {
                            LevelLayout[column, row] = 'D';
                        }

                        if (mapRawData[row * Width + column].ToVector4() == new Vector4(1, 1, 1, 1))
                        {
                            LevelLayout[column, row] = '@';
                        }
                    }
                    else if (mapRawData[row * Width + column].A == 0)
                    {
                        LevelLayout[column, row] = '.';
                    }
                }
            }
        }
    }
}
