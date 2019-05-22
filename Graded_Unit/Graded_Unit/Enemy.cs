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
    Death,
}

/// <summary>
///  
/// this bit for the most part was completed kieran
/// 
/// everyone else had input on the collisions and agreed on changes
/// 
/// </summary>

namespace Graded_Unit
{
    class Enemy
    {
        public Type EnemyMovement;

        private static ContentManager content;
        public static ContentManager Content
        {
            protected get { return content; }
            set { content = value; }
        }

        Random R;

        Texture2D Texture, BulletTexture;

        Vector2 Position, Origin;
        Vector2 MapSize;
        public Rectangle CollisionRect, Detection;

        float Scale, Timer;
        int ResetLocation, S, Seconds, Speed;

        List<CollisionTiles> Tiles;

        public float Colour = 1;

        public bool VISIBLE = false;

        public Enemy(Random rng, int Width, int Height, List<CollisionTiles> tiles)
        {

            R = rng;


            Position = new Vector2(R.Next(160, Width - 160), R.Next(160, Height - 160));


            MapSize = new Vector2(Width, Height); // this is used for assisting in reseting the player location.


            Scale = 0.66f;


            S = R.Next(0, 3);

            Tiles = tiles;

            // decides what state each enemy will be in when it starts 
            if (S == 0)
            {
                EnemyMovement = Type.MovingUp;
                Texture = Content.Load<Texture2D>("Enemy_V");
            }
            if (S == 1)
            {
                EnemyMovement = Type.MovingLeft;
                Texture = Content.Load<Texture2D>("Enemy_H");
            }
            if (S == 2)
            {
                EnemyMovement = Type.MovingRight;
                Texture = Content.Load<Texture2D>("Enemy_H");
            }
            if (S == 3)
            {
                EnemyMovement = Type.MovingDown;
                Texture = Content.Load<Texture2D>("Enemy_V");
            }
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            CollisionRect = new Rectangle((int)Position.X - 25, (int)Position.Y - 25, 50, 50);

            Speed = R.Next(1, 4);
            Timer = 0;



        }

        public void Update(GameTime gameTime, List<Intel> Intellegence)
        {

            if (Colour <= 0)
            {
                CollisionRect.X = -2000;
                Position.X = -2000;
                Colour = 0;
            }
            if (Scale <= 0)
            {
                CollisionRect.X = -2000;
                Position.X = -2000;
                Scale = 0;
            }
            switch (EnemyMovement)
            {

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
                case Type.Death:
                    Colour -= 0.02f;
                    Scale -= 0.02f;
                    break;
            }
            if (Colour < 1)
            {
                Detection.X = -2000;
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
                if(EnemyMovement != Type.Death)
                {
                    Collision(tile.Rectangle, tile.IMPASSABLE);
                }
                
            }
            foreach (Intel intel in Intellegence)
            {
                if (EnemyMovement != Type.Death)
                {
                    Collision(intel.CollisionRect, intel.Impassable);
                }
                
            }

            if (Seconds >= 2 && ResetLocation >= 100)
            {
                VISIBLE = false;
                Position = new Vector2(R.Next(160, (int)MapSize.X - 160), R.Next(160, (int)MapSize.Y - 160));
                Seconds = 0;
                ResetLocation = 0;
            }
            else if (Seconds >= 2 && ResetLocation < 100)
            {
                Seconds = 0;
                ResetLocation = 0;
            }



            CollisionRect.X = (int)Position.X - CollisionRect.Width / 2;
            CollisionRect.Y = (int)Position.Y - CollisionRect.Height / 2;

        }
        public void Draw(SpriteBatch SB)
        {
            if (VISIBLE)
            {

                //SB.Draw(Content.Load<Texture2D>("pixel"), CollisionRect, Color.Pink * 0.25f);

                switch (EnemyMovement)
                {

                    case Type.MovingUp:
                        SB.Draw(Texture, Position, null, Color.White * Colour, 0f, Origin, Scale, SpriteEffects.None, 0f);
                        break;

                    case Type.MovingLeft:
                        SB.Draw(Texture, Position, null, Color.White * Colour, 0f, Origin, Scale, SpriteEffects.FlipHorizontally, 0f);
                        break;

                    case Type.MovingRight:
                        SB.Draw(Texture, Position, null, Color.White * Colour, 0f, Origin, Scale, SpriteEffects.None, 0f);
                        break;

                    case Type.MovingDown:
                        SB.Draw(Texture, Position, null, Color.White * Colour, 0f, Origin, Scale, SpriteEffects.FlipVertically, 0f);
                        break;
                    case Type.Death:
                        SB.Draw(Texture, Position, null, Color.White * Colour, 0f, Origin, Scale, SpriteEffects.None, 0f);
                        break;

                }
                //SB.Draw(Content.Load<Texture2D>("pixel"), new Rectangle((int)Position.X - 1, (int)Position.Y - 1, 3, 3), Color.Black);
            }


        }

        public void Collision(Rectangle newRectangle, bool Impassable)
        {
            //Textures get reloaded in here to fix an issue where it puts the wrong one for moving 
            if (CollisionRect.TouchTopof(newRectangle) && Impassable == true)
            {
                EnemyMovement = Type.MovingUp;
                Texture = Content.Load<Texture2D>("Enemy_V");


            }
            if (CollisionRect.TouchLeftof(newRectangle) && Impassable == true)
            {
                EnemyMovement = Type.MovingLeft;
                Texture = Content.Load<Texture2D>("Enemy_H");

            }
            if (CollisionRect.TouchRightof(newRectangle) && Impassable == true)
            {
                EnemyMovement = Type.MovingRight;
                Texture = Content.Load<Texture2D>("Enemy_H");

            }
            if (CollisionRect.TouchBottomof(newRectangle) && Impassable == true)
            {
                EnemyMovement = Type.MovingDown;
                Texture = Content.Load<Texture2D>("Enemy_V");

            }
        }


    }

}

