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
        Vector2 PREDICTED_LOC;
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

            m_Pos.X += m_CurrentState.ThumbSticks.Left.X * speed;
            m_Pos.Y -= m_CurrentState.ThumbSticks.Left.Y * speed;

            PREDICTED_LOC = m_Pos;
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                m_Pos.Y -= 3;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                m_Pos.Y += 3;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                m_Pos.X -= 3;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                m_Pos.X += 3;
            }
            //Console.WriteLine(m_Pos);

            foreach (CollisionTiles tile in tiles) // this adds the collision to the blocks
            {
                if (tile.IMPASSABLE == true)
                {
                    
                     if () //This is for the bottom of the block with the collision  
                     {

                     }
                     else if () //This is for the top of the block for the collison
                     {

                     }
                     else if () //This is for the left of the block for the collision 
                     {

                     }
                     else if () //This is for the right of the block for the collision 
                     {

                     }
                }


            }
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
