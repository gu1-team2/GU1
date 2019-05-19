using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Graded_Unit
{

    class Map
    {
        private List<CollisionTiles> collisionTiles = new List<CollisionTiles>();

        public List<CollisionTiles> CollisionTiles
        {
            get { return collisionTiles; }
        }

        private int width, height;

        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }
        public Map() { }

        public void Generate(int[,] map, int size)
        {

            for (int x = 0; x < map.GetLength(1); x++)
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    int number = map[y, x];

                    if (number > 0)
                    {
                        collisionTiles.Add(new CollisionTiles(number, new Rectangle(x * size, y * size, size, size)));
                    }

                    width = (x + 1) * size;

                    height = (y + 1) * size;

                }

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (CollisionTiles Tile in collisionTiles)
            {
                Tile.Draw(spriteBatch);
            }
        }

    }
}
