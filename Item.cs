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
        bool canBeSelected = false;

        Vector2 playerSelectedPosition;

        public Item(Texture2D texture, Vector2 position, string Name)
        {
            this.texture = texture;
            this.position = position;
            this.Name = Name;
        }

        public void Update()
        {
            rectangle = new Rectangle(position.ToPoint(), new Point(48, 48));

            if (Game1.player.targetPosition == Game1.player.position && Game1.player.targetPosition != playerSelectedPosition && !selected)
            {
                canBeSelected = true;
            }

            if (rectangle.Contains(Game1.player.targetPositionRectangle.Center) && !selected && canBeSelected)
            {
                if (Game1.player.currentItem != null && Game1.player.currentItem != this)
                {
                    Game1.player.currentItem.canBeSelected = false;
                    Game1.player.currentItem.selected = false;
                    Game1.player.currentItem.position = Game1.player.targetPosition;
                }

                selected = true;
                playerSelectedPosition = Game1.player.targetPosition;

                Game1.player.currentItem = this;
                Game1.PlaySound("Bag");
            }

            if (selected)
            {
                position = Game1.player.position;
                canBeSelected = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(!selected || Game1.player.currentItem != this)
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

            rectangle = new Rectangle(position.ToPoint(), new Point(48, 48));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
