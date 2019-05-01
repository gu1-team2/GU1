using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

enum Type
{
    MovingUp,
    MovingLeft,
    MovingRight,
    MovingDown,
    Stationary,
}

namespace Graded_Unit
{
    class Enemy
    {
        Type EnemyMovement;

        private static ContentManager content;
        public static ContentManager Content
        {
            protected get { return content; }
            set { content = value; }
        }

        Random R;

        Texture2D Texture, MovingTexture, BulletTexture;
        Vector2 Position, Origin;
        Rectangle CollisionRect, Detection;
        float Rotation, Scale;
        int S, Speed;
        List<Bullet> En_Bullets;
        List<CollisionTiles> Tiles;

        public Enemy(Random rng, int Width, int Height, List<CollisionTiles> Tiles)
        {
            Texture = Content.Load<Texture2D>("");
            MovingTexture = Content.Load<Texture2D>("");

            BulletTexture = Content.Load<Texture2D>("");
            Position = new Vector2(R.Next(0, Width), R.Next(0, Height));
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Rotation = 0;
            Scale = 1;
            CollisionRect = new Rectangle((int)Position.X - Texture.Width / 2, (int)Position.Y - Texture.Height / 2, Texture.Width, Texture.Height);
            Detection = new Rectangle((int)Position.X,(int)Position.Y - Texture.Height /2,300,Texture.Height);

            S = R.Next(0, 4);


            // decides what state each enemy will be in when it starts 
            if (S == 0)
            {
                EnemyMovement = Type.Stationary;
            }
            if (S == 1)
            {
                EnemyMovement = Type.MovingUp;
            }
            if (S == 2)
            {
                EnemyMovement = Type.MovingLeft;
            }
            if (S == 3)
            {
                EnemyMovement = Type.MovingRight;
            }
            if (S == 4)
            {
                EnemyMovement = Type.MovingDown;
            }

            Speed = R.Next(1, 5);

            foreach (CollisionTiles Tile in Tiles)
            {
                if (CollisionRect.Intersects(Tile.Rectangle))
                {
                    Position = new Vector2(R.Next(0, Width), R.Next(0, Height)); // the number is the number of tiles in both the x and y direction
                    CollisionRect = new Rectangle((int)Position.X - Texture.Width / 2, (int)Position.Y - Texture.Height / 2, Texture.Width, Texture.Height);
                }
            }

        }

        public void Update()
        {
            switch (EnemyMovement)
            {
                case Type.Stationary: //doesn't move but rather rotates every so often



                    break;

                case Type.MovingUp:
                    Position.Y -= Speed;
                    break;

                case Type.MovingLeft:
                    Position.X -= Speed;
                    break;

                case Type.MovingRight:
                    Position.X += Speed;
                    break;

                case Type.MovingDown:
                    Position.Y += Speed;
                    break;
            }




            foreach (CollisionTiles tile in Tiles)
            {
                Collision(tile.Rectangle, tile.IMPASSABLE); //this for checking the enemy Collision with the tiles
            }
            CollisionRect.X = (int)Position.X - Texture.Width / 2;
            CollisionRect.Y = (int)Position.Y - Texture.Width / 2;
        }
        public void Draw(SpriteBatch SB)
        {
            SB.Draw(Texture, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0f);
        }

        public void Collision(Rectangle newRectangle, bool Impassable)
        {
            if (CollisionRect.TouchTopof(newRectangle) && Impassable == true)
            {
                S = R.Next(0, 2);
                if (S == 0)
                {
                    EnemyMovement = Type.MovingUp;
                }
                if (S == 1)
                {
                    EnemyMovement = Type.MovingLeft;
                }
                if (S == 2)
                {
                    EnemyMovement = Type.MovingRight;
                }
                Position.Y = newRectangle.Y - (CollisionRect.Height / 2) - Speed;
            }
            if (CollisionRect.TouchLeftof(newRectangle) && Impassable == true)
            {
                S = R.Next(0, 2);
                if (S == 0)
                {
                    EnemyMovement = Type.MovingUp;
                }
                if (S == 1)
                {
                    EnemyMovement = Type.MovingLeft;
                }
                if (S == 2)
                {
                    EnemyMovement = Type.MovingDown;
                }
                Position.X = newRectangle.X - (CollisionRect.Width / 2) - Speed;
            }
            if (CollisionRect.TouchRightof(newRectangle) && Impassable == true)
            {
                S = R.Next(0, 2);
                if (S == 0)
                {
                    EnemyMovement = Type.MovingUp;
                }
                if (S == 1)
                {
                    EnemyMovement = Type.MovingRight;
                }
                if (S == 2)
                {
                    EnemyMovement = Type.MovingDown;
                }
                Position.X = newRectangle.X + (CollisionRect.Width / 2) + Speed;

            }
            if (CollisionRect.TouchBottomof(newRectangle) && Impassable == true)
            {
                S = R.Next(0, 2);
                if (S == 0)
                {
                    EnemyMovement = Type.MovingDown;
                }
                if (S == 1)
                {
                    EnemyMovement = Type.MovingLeft;
                }
                if (S == 2)
                {
                    EnemyMovement = Type.MovingRight;
                }
                Position.Y = newRectangle.Y + (CollisionRect.Height / 2) + Speed;

            }
        }
    }

}
