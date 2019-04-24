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
        Rectangle Collision, HudLeft, HudRight;
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
            m_txr = Content.Load<Texture2D>("Tile1");
            BulletTxr = Content.Load<Texture2D>("Bullet");
            BulletSound = Content.Load<SoundEffect>("Gun");
            tiles = TILES;
            m_Pos = new Vector2(400, 400);
            Collision = new Rectangle(400, 400, m_txr.Width, m_txr.Height);
            Width = screenWidth;
            Height = screenHeight;
            HudLeft = new Rectangle((int)m_Pos.X - (Width / 2), (int)m_Pos.Y - (Height / 2), 400, 200);

            foreach (CollisionTiles tile in tiles)
            {
                if (tile.START)
                {
                    // put the position 
                }
            }


            m_Origin = new Vector2(m_txr.Width / 2, m_txr.Height / 2); // this might be wrong

            m_Scale = 1;

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
                if (tile.IMPASSABLE == true && Collision.Intersects(tile.Rectangle))
                {

                    if (Collision.Left <= tile.Rectangle.Right && Collision.Right >= tile.Rectangle.Right)
                    {
                        if (Movement.X < 0)
                        {
                            Movement.X = 0;
                        }
                    }
                    else if (Collision.Right >= tile.Rectangle.Left && Collision.Left <= tile.Rectangle.Left)
                    {
                        if (Movement.X > 0)
                        {
                            Movement.X = 0;
                        }
                    }
                    else if (Collision.Top <= tile.Rectangle.Bottom && Collision.Bottom >= tile.Rectangle.Bottom)
                    {
                        if (Movement.Y > 0)
                        {
                            Movement.Y = 0;
                        }
                    }
                    else if (Collision.Bottom >= tile.Rectangle.Top && Collision.Top <= tile.Rectangle.Top)
                    {
                        if (Movement.Y < 0)
                        {
                            Movement.Y = 0;
                        }
                    }
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
                if (Bullets[i].Collision.Y > (m_Pos.Y + 540) || Bullets[i].Collision.Y < (m_Pos.Y - 540))
                {
                    Bullets.RemoveAt(i);
                }

            }


            m_Pos.X += Movement.X;
            m_Pos.Y -= Movement.Y;
            Collision.X = (int)m_Pos.X - m_txr.Width / 2;
            Collision.Y = (int)m_Pos.Y - m_txr.Height / 2;
            HudLeft.X = (int)m_Pos.X - (Width / 2);
            HudLeft.Y = (int)m_Pos.Y - (Height / 2);

            OldState = m_CurrentState; //Always at the end of update


        }

        public void Draw(SpriteBatch sb, Texture2D px)
        {
            foreach (Bullet bullet in Bullets)
            {
                bullet.Draw(sb);
            }
            sb.Draw(m_txr, m_Pos, null, Color.Red, m_Rotation, m_Origin, m_Scale, SpriteEffects.None, 0f);
            sb.Draw(px, Collision, Color.Blue * 0.75f);
            sb.Draw(px, new Rectangle((int)m_Pos.X - 1, (int)m_Pos.Y - 1, 3, 3), Color.Yellow);
            sb.Draw(px, HudLeft, Color.Blue);
        }

        public Vector2 getPos()
        {
            return new Vector2(-m_Pos.X + 960, -m_Pos.Y + 540);
        }

        public void ResetPlayer(List<CollisionTiles> Tiles)
        {
            foreach (CollisionTiles Tile in Tiles)
            {
                if (Tile.START)
                {
                    m_Pos = new Vector2(Tile.Rectangle.X, Tile.Rectangle.Y);
                }
            }
            tiles = Tiles;
        }
    }
}
