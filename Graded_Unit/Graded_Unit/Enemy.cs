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
        Vector2 MapSize;
        Rectangle CollisionRect, Detection;
        float Rotation, Scale, Timer;
        int ResetLocation, S, Seconds, Speed;
        List<Bullet> En_Bullets;
        List<CollisionTiles> Tiles;

        public Enemy(Random rng, int Width, int Height, List<CollisionTiles> tiles)
        {

            R = rng;
            Texture = Content.Load<Texture2D>("PlayerAim");
            MovingTexture = Content.Load<Texture2D>("PlayerAim");

            BulletTexture = Content.Load<Texture2D>("Bullet");

            Position = new Vector2(R.Next(160, Width - 160), R.Next(160, Height - 160));
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            MapSize = new Vector2(Width, Height); // this is used for assisting in reseting the player location.


            Rotation = 0;
            Scale = 0.66f;
            CollisionRect = new Rectangle((int)Position.X - Texture.Width / 2, (int)Position.Y - Texture.Height / 2, Texture.Width, Texture.Height);

            S = R.Next(0, 4);

            Tiles = tiles;

            // decides what state each enemy will be in when it starts 
            if (S == 0)
            {
                EnemyMovement = Type.Stationary;

            }
            if (S == 1)
            {
                EnemyMovement = Type.MovingUp;
                Rotation = 1;
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
            Timer = 0;

            

        }

        public void Update(GameTime gameTime)
        {
            switch (EnemyMovement)
            {
                case Type.Stationary: //doesn't move but rather rotates every so often

                    Rotation -= 0.1f;
                    break;
                // movements from here

                case Type.MovingUp:
                    Position.Y -= Speed;
                    Detection = new Rectangle((int)Position.X - (Texture.Width / 2), (int)Position.Y - Texture.Height / 2 - 300, Texture.Width, 300);

                    break;

                case Type.MovingLeft:
                    Position.X -= Speed;
                    Detection = new Rectangle((int)Position.X - 300 - (Texture.Width / 2), (int)Position.Y - Texture.Height / 2, 300, Texture.Height);

                    break;

                case Type.MovingRight:
                    Position.X += Speed;
                    Detection = new Rectangle((int)Position.X + Texture.Width / 2, (int)Position.Y - Texture.Height / 2, 300, Texture.Height);

                    break;

                case Type.MovingDown:
                    Position.Y += Speed;
                    Detection = new Rectangle((int)Position.X - (Texture.Width / 2), (int)Position.Y + Texture.Height / 2, Texture.Width, 300);

                    break;
            }

            Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Seconds += (int)Timer;
            if (Timer >= 1f)
            {
                Timer = 0f;
            }


            foreach (CollisionTiles tile in Tiles)
            {
                if (tile.Rectangle.Intersects(CollisionRect) && tile.IMPASSABLE)
                {
                    ResetLocation++;
                }
                Collision(tile.Rectangle, tile.IMPASSABLE);
            }

            if(Seconds >=2 && ResetLocation >= 100)
            {
                Position = new Vector2(R.Next(160, (int)MapSize.X - 160), R.Next(160, (int)MapSize.Y - 160));
                Seconds = 0;
                ResetLocation = 0; 
            }
            else if (Seconds >= 2 && ResetLocation < 100)
            {
                //Position = new Vector2(R.Next(160, (int)MapSize.X - 160), R.Next(160, (int)MapSize.Y - 160));
                Seconds = 0;
                ResetLocation = 0;
            }

            CollisionRect.X = (int)Position.X - Texture.Width / 2;
            CollisionRect.Y = (int)Position.Y - Texture.Height / 2;

        }
        public void Draw(SpriteBatch SB)
        {
            SB.Draw(Texture, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0f);
            SB.Draw(Content.Load<Texture2D>("pixel"), Detection, Color.Pink * 0.25f);

            switch (EnemyMovement)
            {

                case Type.MovingUp:
                    SB.Draw(Content.Load<Texture2D>("pixel"), CollisionRect, Color.Green * 0.5f);
                    break;

                case Type.MovingLeft:
                    SB.Draw(Content.Load<Texture2D>("pixel"), CollisionRect, Color.Blue * 0.5f);
                    break;

                case Type.MovingRight:
                    SB.Draw(Content.Load<Texture2D>("pixel"), CollisionRect, Color.Red * 0.5f);
                    break;

                case Type.MovingDown:
                    SB.Draw(Content.Load<Texture2D>("pixel"), CollisionRect, Color.Yellow * 0.5f);
                    break;
            }
            SB.Draw(Content.Load<Texture2D>("pixel"), new Rectangle((int)Position.X - 1, (int)Position.Y - 1, 3, 3), Color.Black);

        }

        public void Collision(Rectangle newRectangle, bool Impassable)
        {
            if (CollisionRect.TouchTopof(newRectangle) && Impassable == true)
            {
                EnemyMovement = Type.MovingUp;


            }
            if (CollisionRect.TouchLeftof(newRectangle) && Impassable == true)
            {
                EnemyMovement = Type.MovingLeft;

            }
            if (CollisionRect.TouchRightof(newRectangle) && Impassable == true)
            {
                EnemyMovement = Type.MovingRight;
            }
            if (CollisionRect.TouchBottomof(newRectangle) && Impassable == true)
            {
                EnemyMovement = Type.MovingDown;
            }
        }
    }

}

