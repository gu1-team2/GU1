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
        Texture2D m_txr;
        Rectangle Collision;
        Vector2 m_Origin;
        Vector2 m_Pos;
        Vector2 Movement;
        int speed;


        Game1 instance;

        float m_Rotation, m_Scale;

        GamePadState m_CurrentState, OldState;

        public Player(Texture2D Texture, int X, int Y, int S)
        {
            m_txr = Texture;

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


            //Console.WriteLine(m_Pos);

            foreach (CollisionTiles tile in tiles) // this adds the collision to the blocks
            {
                if (tile.IMPASSABLE == true && Collision.Intersects(tile.Rectangle))
                {
                    if (Collision.Top < tile.Rectangle.Bottom)
                    {
                        m_Pos.Y = tile.Rectangle.X + tile.Rectangle.Height;
                    }
                    if (Collision.Bottom > tile.Rectangle.Top)
                    {
                        m_Pos.Y = tile.Rectangle.Y - 5;
                    }
                    if (Collision.Left < tile.Rectangle.Right)
                    {
                        m_Pos.X -= Movement.X;
                    }
                    if (Collision.Right > tile.Rectangle.Left)
                    {
                        m_Pos.X -= Movement.X ;
                    }
                }

            }
            /*
             * static class RectangleHelper
    {
        public static bool TouchTopOf(this Rectangle r1, Rectangle r2)
        {
            return (r1.Bottom >= r2.Top - 1 &&
                    r1.Bottom <= r2.Top + (r2.Height / 2) &&
                    r1.Right >= r2.Left + (r2.Width / 5) &&
                    r1.Left <= r2.Right - (r2.Width / 5));
        }

        public static bool TouchBottomOf(this Rectangle r1, Rectangle r2)
        {
            return (r1.Top <= r2.Bottom + (r2.Height / 5) &&
                    r1.Top >= r2.Bottom - 1 &&
                    r1.Right >= r2.Left + (r2.Width / 5) &&
                    r1.Left <= r2.Right - (r2.Width / 5));
        }

        public static bool TouchLeftOf(this Rectangle r1, Rectangle r2)
        {
            return (r1.Right <= r2.Right &&
                    r1.Right >= r2.Left - 5 &&
                    r1.Top <= r2.Bottom - (r2.Width / 4) &&
                    r1.Bottom >= r2.Top + (r2.Width / 4));
        }

        public static bool TouchRightOf(this Rectangle r1, Rectangle r2)
        {
            return (r1.Left >= r2.Left &&
                    r1.Left <= r2.Right + 5 &&
                    r1.Top <= r2.Bottom - (r2.Width / 4) &&
                    r1.Bottom >= r2.Top + (r2.Width / 4));

        }

    }
             * 
             * 
             * 
             * */
            m_Pos.X += Movement.X;
            m_Pos.Y -= Movement.Y;
            Collision.X = (int)m_Pos.X - m_txr.Width / 2;
            Collision.Y = (int)m_Pos.Y - m_txr.Height / 2;

            OldState = m_CurrentState; //Always at the end of update
        }

        public void Draw(SpriteBatch sb, Texture2D px)
        {
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
