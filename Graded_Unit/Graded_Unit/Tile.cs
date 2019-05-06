using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Graded_Unit
{
    class Tile
    {
        protected Texture2D texture;

        private Rectangle rectangle;
        public Rectangle Rectangle
        {
            get { return rectangle; }
            protected set { rectangle = value; }
        }
        private static ContentManager content;
        public static ContentManager Content // this is for having the content loader present to any child classes
        {
            protected get { return content; }
            set { content = value; }
        }

        public bool IMPASSABLE, EXIT, START, VISITED;

        public void Draw(SpriteBatch spriteBatch)
        {
            if (VISITED)
            {
                spriteBatch.Draw(texture, rectangle, Color.White);
            }
            else
            {
                spriteBatch.Draw(texture, rectangle, Color.White * 0.1f);
            }

        }
    }
    class CollisionTiles : Tile
    {

        public CollisionTiles(int i, Rectangle newRectangle)
        {
            texture = Content.Load<Texture2D>("Tile" + i);
            this.Rectangle = newRectangle;
            VISITED = false;
            if (i == 1)
            {
                IMPASSABLE = true;
            }
            if (i == 2)
            {
                START = true;
            }
        }
    }
}
