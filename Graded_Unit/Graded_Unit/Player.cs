using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Graded_Unit
{
    class Player
    {
        Texture2D m_txr, BulletTxr;
        Rectangle Collision;
        Vector2 m_Origin;
        Vector2 m_Pos;
        Vector2 Movement;
        int speed;
        List<Bullet> Bullets = new List<Bullet>();

        float m_Rotation, m_Scale;

        GamePadState m_CurrentState, OldState;

        public Player(Texture2D Texture, int X, int Y, int S, Texture2D Bull)
        {
            m_txr = Texture;
            BulletTxr = Bull;

            m_Pos = new Vector2(X, Y);
            Collision = new Rectangle(X, Y, m_txr.Width, m_txr.Height);

            m_Origin = new Vector2(m_txr.Width / 2, m_txr.Height / 2); // this might be wrong

            m_Scale = 1;

            speed = S;
        }

        public void Update(GamePadState Currpad, List<CollisionTiles> tiles)
        {
            m_CurrentState = Currpad; // always at start of update

            m_Rotation = (float)Math.Atan2(m_CurrentState.ThumbSticks.Right.X, m_CurrentState.ThumbSticks.Right.Y); // This makes the player face where the right stick is pointed at

            Movement.X = m_CurrentState.ThumbSticks.Left.X * speed;
            Movement.Y = m_CurrentState.ThumbSticks.Left.Y * speed;

            if (m_CurrentState.Triggers.Right >= 0.5 && OldState.Triggers.Right <0.5)
            {
                Bullets.Add(new Bullet(BulletTxr, (int)m_Pos.X, (int)m_Pos.Y, m_Rotation));
            }

            //bullet movements
            foreach (Bullet bullet in Bullets)
            {
                bullet.Update();
            }

            foreach (CollisionTiles tile in tiles) // this adds the collision to the blocks
            {
                if (tile.IMPASSABLE == true && Collision.Intersects(tile.Rectangle))
                {

                    if (Collision.Left < tile.Rectangle.Right && Collision.Right > tile.Rectangle.Right)
                    {
                        m_Pos.X = (tile.Rectangle.Right + Collision.Width / 2 + 1);
                    }
                    if (Collision.Right > tile.Rectangle.Left && Collision.Left < tile.Rectangle.Left)
                    {
                        m_Pos.X = (tile.Rectangle.Left - Collision.Width / 2 - 1);
                    }
                    if (Collision.Top < tile.Rectangle.Bottom && Collision.Bottom > tile.Rectangle.Bottom)
                    {
                        m_Pos.Y = (tile.Rectangle.Bottom + Collision.Height / 2 + 1);
                    }
                    if (Collision.Bottom > tile.Rectangle.Top && Collision.Top < tile.Rectangle.Top)
                    {
                        m_Pos.Y = (tile.Rectangle.Top - Collision.Height / 2 - 1);
                    }
                }

                //remove a bullet whenever it hits a wall
                for (int i = Bullets.Count - 1; i >= 0; i--)
                {
                    if (Bullets[i].Collision.Intersects(tile.Rectangle) && tile.IMPASSABLE == true)
                    {
                        Bullets.RemoveAt(i);
                    }
                }
            }

            m_Pos.X += Movement.X;
            m_Pos.Y -= Movement.Y;
            Collision.X = (int)m_Pos.X - m_txr.Width / 2;
            Collision.Y = (int)m_Pos.Y - m_txr.Height / 2;


            OldState = m_CurrentState; //Always at the end of update


        }

        public void Draw(SpriteBatch sb, Texture2D px)
        {
            foreach(Bullet bullet in Bullets)
            {
                bullet.Draw(sb);
            }
            sb.Draw(m_txr, m_Pos, null, Color.Red, m_Rotation, m_Origin, m_Scale, SpriteEffects.None, 0f);
            sb.Draw(px, Collision, Color.Blue * 0.75f);
            sb.Draw(px, new Rectangle((int)m_Pos.X - 1, (int)m_Pos.Y - 1, 3, 3), Color.Yellow);
        }

        public Vector2 getPos()
        {
            return new Vector2(-m_Pos.X + 960, -m_Pos.Y + 540);
        }

    }
}
