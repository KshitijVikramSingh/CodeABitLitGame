using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CodeABitLitGame
{
    public class Item
    {
        public Texture2D texture;
        Vector2 position;

        public Rectangle rectangle;
        public string Name;

        bool selected = false;

        public Item(Texture2D texture, Vector2 position, string Name)
        {
            this.texture = texture;
            this.position = position;
            this.Name = Name;

            rectangle = new Rectangle(position.ToPoint(), new Point(32, 32));
        }

        public void Update()
        {
            if (rectangle.Intersects(Game1.player.positionRectangle))
            {
                selected = true;
                Game1.player.currentItem = this;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(!selected)
                spriteBatch.Draw(texture, position, Color.White);
        }
    }

    public class ItemPair
    {
        public Texture2D texture;
        Vector2 position;

        public Rectangle rectangle;
        public string Name;

        public ItemPair(Texture2D texture, Vector2 position, string Name)
        {
            this.texture = texture;
            this.position = position;
            this.Name = Name;

            rectangle = new Rectangle(position.ToPoint(), new Point(32, 32));
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
