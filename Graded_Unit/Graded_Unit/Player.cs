﻿using Microsoft.Xna.Framework;
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

        Texture2D HudRevolver, HudBullet;


        SoundEffect BulletSound;
        Rectangle rectangle, CollisionRect, HudLeft, HudRight, Visibility, CollectionRange;
        Vector2 m_Origin;
        public Vector2 m_Pos;
        Vector2 Movement;
        int speed, IntelCollected, MaxIntel;

        List<Ammo> Ammunition = new List<Ammo>();
        List<Bullet> Bullets = new List<Bullet>();
        public List<CollisionTiles> tiles = new List<CollisionTiles>();
        List<Intel> Intellegence = new List<Intel>();


        Vector2 BulletSpawn;

        float m_Rotation, m_Scale;


        int Width, Height;

        GamePadState m_CurrentState, OldState;

        public Player(int S, List<CollisionTiles> TILES, List<Ammo> AMMUNITION, List<Intel> INTELLEGENCE, int screenWidth, int screenHeight)
        {
            m_txr = Content.Load<Texture2D>("PlayerAim");
            BulletTxr = Content.Load<Texture2D>("Bullet");
            BulletSound = Content.Load<SoundEffect>("Gun");

            HudRevolver = Content.Load<Texture2D>("revolver-hud");
            HudBullet = Content.Load<Texture2D>("bullet-hud");

            tiles = TILES;
            Ammunition = AMMUNITION;
            Intellegence = INTELLEGENCE;

            MaxIntel = Intellegence.Count;

            Width = screenWidth;
            Height = screenHeight;
            HudLeft = new Rectangle(0, 0, 400, 200);  // It WOOOOOOORRRRRRRKKKKKKKKKKKKKSSSSSSSSSSSSSSSS
            HudRight = new Rectangle(Width - 400, 0, 400, 200);

            foreach (CollisionTiles tile in tiles)
            {
                if (tile.START)
                {
                    m_Pos = new Vector2((tile.Rectangle.X + tile.Rectangle.Width / 2), (tile.Rectangle.Y + tile.Rectangle.Width / 2));


                    rectangle = new Rectangle((int)m_Pos.X, (int)m_Pos.Y, m_txr.Width, m_txr.Height);
                    CollisionRect = new Rectangle((int)m_Pos.X, (int)m_Pos.Y, 50, 50);
                    Visibility = new Rectangle((int)m_Pos.X, (int)m_Pos.Y, 320, 320);
                    BulletSpawn = new Vector2(((int)m_Pos.X + rectangle.Width / 6), ((int)m_Pos.Y - (rectangle.Height / 3)));
                    CollectionRange = new Rectangle((int)m_Pos.X, (int)m_Pos.Y, 200, 200);
                }
            }


            m_Origin = new Vector2(m_txr.Width / 2, m_txr.Height / 2);

            m_Scale = 0.66f;

            speed = S;
        }

        public void Update(GamePadState Currpad, List<Enemy> Enemies, GameStates CurrentState, Exit exit)
        {

            m_CurrentState = Currpad; // always at start of update
            PlayerMovement();

            if (m_CurrentState.Buttons.RightStick == ButtonState.Pressed)
            {
                //something
            }

            // this is for moving onto the next level screen
            if (CollectionRange.Intersects(exit.CollisionRect) && m_CurrentState.Buttons.A == ButtonState.Pressed && OldState.Buttons.A == ButtonState.Released && IntelCollected == Intellegence.Count)
            {
                CurrentState = GameStates.DisplayIntel;
            }

            BulletUpdate(Enemies);




            VisibilityCheck(Enemies, exit);
            UpdateCollisions();
            rectMovement(); // this is just for all the rectangle movements so they are all in one place 

            BulletSpawn = new Vector2(((int)m_Pos.X + rectangle.Width / 6), ((int)m_Pos.Y - (rectangle.Height / 3)));
            BulletSpawn = RotateVector2(BulletSpawn, m_Rotation, m_Pos);


            OldState = m_CurrentState; //Always at the end of update
        }
        public void BulletUpdate(List<Enemy> Enemies) // all the bullet stuff is done here
        {
            //add the bullet stuff
            if (m_CurrentState.Triggers.Right >= 0.5 && OldState.Triggers.Right < 0.5 && Bullets.Count < 6)
            {
                Bullets.Add(new Bullet(BulletTxr, (int)BulletSpawn.X, (int)BulletSpawn.Y, m_Rotation));
                BulletSound.Play();
            }

            //bullet movement update and the condition for if it hits an enemy


            foreach (Bullet bullet in Bullets)
            {
                bullet.Update();
                for (int i = 0; i < Enemies.Count; i++)
                {
                    if (bullet.Collision.Intersects(Enemies[i].CollisionRect))
                    {
                        Enemies.RemoveAt(i);
                        bullet.isVisible = false;
                    }
                }
            }

            foreach (CollisionTiles tile in tiles) // this adds the collision to the blocks
            {

                //remove a bullet whenever it hits a wall
                for (int i = Bullets.Count - 1; i >= 0; i--)
                {
                    if (Bullets[i].Collision.Intersects(tile.Rectangle) && tile.IMPASSABLE == true)
                    {
                        Bullets[i].isVisible = false;
                    }

                }
            }



            for (int i = Bullets.Count - 1; i >= 0; i--)
            {
                //removes a bullet if it isn't visble 
                if (!Bullets[i].isVisible)
                {
                    Bullets.RemoveAt(i);
                }
                //removes a bullet if it goes off screen
                else if (Bullets[i].Collision.X > (m_Pos.X + 960) || Bullets[i].Collision.X < (m_Pos.X - 960))
                {
                    Bullets.RemoveAt(i);

                }
                else if (Bullets[i].Collision.Y > (m_Pos.Y + 540) || Bullets[i].Collision.Y < (m_Pos.Y - 540))
                {
                    Bullets.RemoveAt(i);

                }


            }
        }

        public void PlayerMovement()
        {
            //movements
            Movement.X = m_CurrentState.ThumbSticks.Left.X * speed;
            Movement.Y = m_CurrentState.ThumbSticks.Left.Y * speed;


            //pointing stuff
            if (m_CurrentState.ThumbSticks.Left.X != 0 || m_CurrentState.ThumbSticks.Left.Y != 0)
            {
                m_Rotation = (float)Math.Atan2(m_CurrentState.ThumbSticks.Left.X, m_CurrentState.ThumbSticks.Left.Y); // This makes the player face where the Left stick is pointed at

            }
            if (m_CurrentState.ThumbSticks.Right.X != 0 || m_CurrentState.ThumbSticks.Right.Y != 0)
            {
                m_Rotation = (float)Math.Atan2(m_CurrentState.ThumbSticks.Right.X, m_CurrentState.ThumbSticks.Right.Y); // This makes the player face where the right stick is pointed at
            }
        }

        public void UpdateCollisions() // this is to make it easier to find all the collisions
        {

            foreach (CollisionTiles tile in tiles) // this adds the collision to the blocks
            {
                Collision(tile.Rectangle, tile.IMPASSABLE);
                if (Visibility.Intersects(tile.Rectangle))
                {
                    tile.VISITED = true;
                }
            }
            foreach (Intel intel in Intellegence)
            {
                Collision(intel.CollisionRect, intel.Impassable);

            }


        }

        public void VisibilityCheck(List<Enemy> Enemies,Exit exit) // all the visibility bools are done here 
        {
            if (Visibility.Intersects(exit.CollisionRect))
            {
                exit.Visible = true;
            }

            foreach (Enemy enemy in Enemies)
            {
                foreach (CollisionTiles tile in tiles)
                {
                    if (Visibility.Intersects(enemy.CollisionRect) && enemy.CollisionRect.Intersects(tile.Rectangle))
                    {
                        if (tile.IMPASSABLE)
                        {
                            enemy.VISIBLE = false;
                        }
                        else
                        {
                            enemy.VISIBLE = true;
                        }

                    }
                }


            }
            foreach (Intel intel in Intellegence)
            {
                foreach (CollisionTiles tile in tiles)
                {
                    if (Visibility.Intersects(intel.CollisionRect) && intel.CollisionRect.Intersects(tile.Rectangle))
                    {
                        if (tile.IMPASSABLE)
                        {
                            intel.Visible = false;
                        }
                        else
                        {
                            intel.Visible = true;
                        }

                    }
                }
            }
        }

        public void Draw(SpriteBatch sb, Texture2D px)
        {
            foreach (Bullet bullet in Bullets)
            {
                bullet.Draw(sb);
            }
            sb.Draw(m_txr, m_Pos, null, Color.White, m_Rotation, m_Origin, m_Scale, SpriteEffects.None, 0f);
          //  sb.Draw(px, CollectionRange, Color.Blue * 0.75f);
            // sb.Draw(px, new Rectangle((int)BulletSpawn.X - 1, (int)BulletSpawn.Y - 1, 3, 3), Color.Yellow);
        }

        public void Hud_Draw(SpriteBatch sb) //This is for drawing the HUD elements
        {
            sb.Draw(Content.Load<Texture2D>("pixel"), HudLeft, Color.White);
            sb.Draw(Content.Load<Texture2D>("pixel"), HudRight, Color.White);

            sb.Draw(HudBullet, new Vector2(100, 200), Color.White);
        }

        public Vector2 getPos()
        {
            return new Vector2(-m_Pos.X + 960, -m_Pos.Y + 540); // honestly no idea what this does as mark did it
        }

        /// <summary>
        /// 
        ///     This is the stuff relating to the rectangles movement and collisions
        /// 
        /// </summary>


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
        public void rectMovement()
        {
            m_Pos.X += Movement.X;
            m_Pos.Y -= Movement.Y;

            rectangle.X = (int)m_Pos.X - m_txr.Width / 2;
            rectangle.Y = (int)m_Pos.Y - m_txr.Height / 2;

            CollisionRect.X = (int)m_Pos.X - CollisionRect.Width / 2;
            CollisionRect.Y = (int)m_Pos.Y - CollisionRect.Height / 2;

            Visibility.X = (int)m_Pos.X - Visibility.Width / 2;
            Visibility.Y = (int)m_Pos.Y - Visibility.Height / 2;

            CollectionRange.X = (int)m_Pos.X - CollectionRange.Width / 2;
            CollectionRange.Y = (int)m_Pos.Y - CollectionRange.Height / 2;

        }

        /// <summary>
        /// 
        /// 
        ///  this is the stuff for the rotation of the bullet spawn bellow here
        /// 
        /// 
        /// </summary>

        public static Vector2 RotateVector2(Vector2 point, float rotation, Vector2 pivot)
        {
            float cosRotation = (float)Math.Cos(rotation);
            float sinRotation = (float)Math.Sin(rotation);

            Vector2 translatedPoint = new Vector2();

            translatedPoint.X = point.X - pivot.X;
            translatedPoint.Y = point.Y - pivot.Y;

            Vector2 rotatedPoint = new Vector2();
            rotatedPoint.X = translatedPoint.X * cosRotation - translatedPoint.Y * sinRotation + pivot.X;
            rotatedPoint.Y = translatedPoint.X * sinRotation + translatedPoint.Y * cosRotation + pivot.Y;

            return rotatedPoint;
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
