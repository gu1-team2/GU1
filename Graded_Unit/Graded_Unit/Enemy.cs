using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

enum Type
{
    Moving,
    Stationary,
}

namespace Graded_Unit
{
    class Enemy
    {
        Type EnemyVariation;

        private static ContentManager content;
        public static ContentManager Content
        {
            protected get { return content; }
            set { content = value; }
        }

        Texture2D Texture, BulletTexture;
        Vector2 Position, Origin;
        Rectangle Collision;
        Random RNG = new Random();
        float Rotation, Scale;

        List<Bullet> En_Bullets;

        public Enemy(int X, int Y, List<CollisionTiles> Tiles,Levels Current)
        {
            switch (EnemyVariation)
            {
                case Type.Moving:

                    break;
                case Type.Stationary:

                    break;
            }

            Texture = Content.Load<Texture2D>("");
            BulletTexture = Content.Load<Texture2D>("");
            Position = new Vector2(X, Y);
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Rotation = 0;
            Scale = 1;
            Collision = new Rectangle(X - Texture.Width / 2, Y - Texture.Height / 2, Texture.Width, Texture.Height);
            switch (Current)
            {
                case Levels.Level0:
                    foreach (CollisionTiles Tile in Tiles)
                    {
                        if (Collision.Intersects(Tile.Rectangle))
                        {
                            Position = new Vector2(RNG.Next(0, Tile.Rectangle.Width * 11), RNG.Next(0, Tile.Rectangle.Height * 11)); // the number is the number of tiles in both the x and y direction
                            Collision = new Rectangle(X - Texture.Width / 2, Y - Texture.Height / 2, Texture.Width, Texture.Height);
                        }
                    }
                    break;

                case Levels.Level1:
                    foreach (CollisionTiles Tile in Tiles)
                    {
                        if (Collision.Intersects(Tile.Rectangle))
                        {
                            Position = new Vector2(RNG.Next(0, Tile.Rectangle.Width * 47), RNG.Next(0, Tile.Rectangle.Height * 26 )); // the number is the number of tiles in both the x and y direction
                            Collision = new Rectangle(X - Texture.Width / 2, Y - Texture.Height / 2, Texture.Width, Texture.Height);
                        }
                    }
                    break;
            }


        }

        public void Update()
        {
            switch (EnemyVariation)
            {
                case Type.Stationary:



                    break;
                case Type.Moving:



                    break;
            }
        }
        public void Draw(SpriteBatch SB)
        {
            SB.Draw(Texture, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0f);
        }
        public void ResetEnemy(List<CollisionTiles> Tiles)
        {

        }
    }

}
