using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Graded_Unit
{
    class Player
    {
        private static ContentManager content;
        public static ContentManager Content
        {
            protected get { return content; }
            set { content = value; }
        }


        Texture2D m_txr, BulletTxr;
        SoundEffect BulletSound;
        Rectangle rectangle, CollisionRect, HudLeft, HudRight,Visibility;
        Vector2 m_Origin;
        public Vector2 m_Pos;
        Vector2 Movement;
        int speed;
        List<Bullet> Bullets = new List<Bullet>();
        public List<CollisionTiles> tiles = new List<CollisionTiles>();

        float m_Rotation, m_Scale;

        int Width, Height;

        GamePadState m_CurrentState, OldState;

        public Player(int S, List<CollisionTiles> TILES, int screenWidth, int screenHeight)
        {
            m_txr = Content.Load<Texture2D>("PlayerAim");
            BulletTxr = Content.Load<Texture2D>("Bullet");
            BulletSound = Content.Load<SoundEffect>("Gun");
            tiles = TILES;
            //move the 4 below into the for each statement
            m_Pos = new Vector2(400, 400);
            rectangle = new Rectangle(400, 400, m_txr.Width, m_txr.Height);
            CollisionRect = new Rectangle(400, 400, 50, 50);
            Visibility = new Rectangle((int)m_Pos.X,(int)m_Pos.Y,320,320);
            //stop here

            Width = screenWidth;
            Height = screenHeight;
            HudLeft = new Rectangle((int)m_Pos.X - (Width / 2), (int)m_Pos.Y - (Height / 2), 400, 200); // I think I know how to get this to work

            foreach (CollisionTiles tile in tiles)
            {
                if (tile.START)
                {
                    // put the position 
                }
            }


            m_Origin = new Vector2(m_txr.Width / 2, m_txr.Height / 2);

            m_Scale = 0.66f;

            speed = S;
        }

        public void Update(GamePadState Currpad)
        {

            m_CurrentState = Currpad; // always at start of update

            m_Rotation = (float)Math.Atan2(m_CurrentState.ThumbSticks.Left.X, m_CurrentState.ThumbSticks.Left.Y); // This makes the player face where the Left stick is pointed at
            if (m_CurrentState.ThumbSticks.Right.X != 0 || m_CurrentState.ThumbSticks.Right.Y != 0)
            {
                m_Rotation = (float)Math.Atan2(m_CurrentState.ThumbSticks.Right.X, m_CurrentState.ThumbSticks.Right.Y); // This makes the player face where the right stick is pointed at
            }

            if (m_CurrentState.Buttons.RightStick == ButtonState.Pressed)
            {

            }
            Movement.X = m_CurrentState.ThumbSticks.Left.X * speed;
            Movement.Y = m_CurrentState.ThumbSticks.Left.Y * speed;

            if (m_CurrentState.Triggers.Right >= 0.5 && OldState.Triggers.Right < 0.5 && Bullets.Count < 6)
            {
                Bullets.Add(new Bullet(BulletTxr, (int)m_Pos.X, (int)m_Pos.Y, m_Rotation));
                BulletSound.Play();
            }

            //bullet movement update
            foreach (Bullet bullet in Bullets)
            {
                bullet.Update();
            }

            foreach (CollisionTiles tile in tiles) // this adds the collision to the blocks
            {

                Collision(tile.Rectangle, tile.IMPASSABLE);
                if (Visibility.Intersects(tile.Rectangle))
                {
                    tile.VISITED = true;
                }
                //end of player collision with the walls

                //remove a bullet whenever it hits a wall
                for (int i = Bullets.Count - 1; i >= 0; i--)
                {
                    if (Bullets[i].Collision.Intersects(tile.Rectangle) && tile.IMPASSABLE == true)
                    {
                        Bullets.RemoveAt(i);
                    }

                }
            }

            //removes a bullet if it goes off screen
            for (int i = Bullets.Count - 1; i >= 0; i--)
            {
                if (Bullets[i].Collision.X > (m_Pos.X + 960) || Bullets[i].Collision.X < (m_Pos.X - 960))
                {
                    Bullets.RemoveAt(i);
                }
                else if (Bullets[i].Collision.Y > (m_Pos.Y + 540) || Bullets[i].Collision.Y < (m_Pos.Y - 540))
                {
                    Bullets.RemoveAt(i);
                }

            }




            m_Pos.X += Movement.X;
            m_Pos.Y -= Movement.Y;

            rectangle.X = (int)m_Pos.X - m_txr.Width / 2;
            rectangle.Y = (int)m_Pos.Y - m_txr.Height / 2;

            CollisionRect.X = (int)m_Pos.X - CollisionRect.Width/2;
            CollisionRect.Y = (int)m_Pos.Y -CollisionRect.Height/2  ;

            Visibility.X = (int)m_Pos.X - Visibility.Width / 2;
            Visibility.Y = (int)m_Pos.Y - Visibility.Height / 2;

            //HudLeft.X = (int)m_Pos.X - (Width / 2);
            //HudLeft.Y = (int)m_Pos.Y - (Height / 2);

            OldState = m_CurrentState; //Always at the end of update


        }

        public void Draw(SpriteBatch sb, Texture2D px)
        {
            foreach (Bullet bullet in Bullets)
            {
                bullet.Draw(sb);
            }
            sb.Draw(m_txr, m_Pos, null, Color.White, m_Rotation, m_Origin, m_Scale, SpriteEffects.None, 0f);
            sb.Draw(px, Visibility, Color.Blue * 0.75f);
            sb.Draw(px, new Rectangle((int)m_Pos.X - 1, (int)m_Pos.Y - 1, 3, 3), Color.Yellow);
        }

        public Vector2 getPos()
        {
            return new Vector2(-m_Pos.X + 960, -m_Pos.Y + 540);
        }

        public void Collision(Rectangle newRectangle, bool Impassable)
        {
            if (CollisionRect.TouchTopof(newRectangle) && Impassable == true)
            {
                m_Pos.Y = newRectangle.Y - (CollisionRect.Height / 2) - 3;
                Movement.Y = 0f;
            }
            if (CollisionRect.TouchLeftof(newRectangle) && Impassable == true)
            {
                m_Pos.X = newRectangle.X - (CollisionRect.Width / 2) - 3;
            }
            if (CollisionRect.TouchRightof(newRectangle) && Impassable == true)
            {
                m_Pos.X = newRectangle.X + newRectangle.Width + (CollisionRect.Width / 2) + 3;

            }
            if (CollisionRect.TouchBottomof(newRectangle) && Impassable == true)
            {
                m_Pos.Y = newRectangle.Bottom + (CollisionRect.Height / 2) + 3;
                Movement.Y = 0f;
            }
        }
    }

    /*
░░░░▄▄▄▄▀▀▀▀▀▀▀▀▄▄▄▄▄▄
░░░░█░░░░▒▒▒▒▒▒▒▒▒▒▒▒░░▀▀▄
░░░█░░░▒▒▒▒▒▒░░░░░░░░▒▒▒░░█
░░█░░░░░░▄██▀▄▄░░░░░▄▄▄░░░█
░▀▒▄▄▄▒░█▀▀▀▀▄▄█░░░██▄▄█░░░█
█▒█▒▄░▀▄▄▄▀░░░░░░░░█░░░▒▒▒▒▒█
█▒█░█▀▄▄░░░░░█▀░░░░▀▄░░▄▀▀▀▄▒█
░█▀▄░█▄░█▀▄▄░▀░▀▀░▄▄▀░░░░█░░█
░░█░░▀▄▀█▄▄░█▀▀▀▄▄▄▄▀▀█▀██░█
░░░█░░██░░▀█▄▄▄█▄▄█▄████░█
░░░░█░░░▀▀▄░█░░░█░███████░█
░░░░░▀▄░░░▀▀▄▄▄█▄█▄█▄█▄▀░░█
░░░░░░░▀▄▄░▒▒▒▒░░░░░░░░░░█
░░░░░░░░░░▀▀▄▄░▒▒▒▒▒▒▒▒▒▒░█
░░░░░░░░░░░░░░▀▄▄▄▄▄░░░░░█
 * 
 * 
 * 
　　██░▀██████████████▀░██
　　█▌▒▒░████████████░▒▒▐█
　　█░▒▒▒░██████████░▒▒▒░█
　　▌░▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒░▐
　　░▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒░
　 ███▀▀▀██▄▒▒▒▒▒▒▒▄██▀▀▀██
　 ██░░░▐█░▀█▒▒▒▒▒█▀░█▌░░░█
　 ▐▌░░░▐▄▌░▐▌▒▒▒▐▌░▐▄▌░░▐▌
　　█░░░▐█▌░░▌▒▒▒▐░░▐█▌░░█
　　▒▀▄▄▄█▄▄▄▌░▄░▐▄▄▄█▄▄▀▒
　　░░░░░░░░░░└┴┘░░░░░░░░░
　　██▄▄░░░░░░░░░░░░░░▄▄██
　　████████▒▒▒▒▒▒████████
　　█▀░░███▒▒░░▒░░▒▀██████
　　█▒░███▒▒╖░░╥░░╓▒▐█████
　　█▒░▀▀▀░░║░░║░░║░░█████
　　██▄▄▄▄▀▀┴┴╚╧╧╝╧╧╝┴┴███
　　██████████████████████
 */
}
