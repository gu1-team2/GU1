﻿using Microsoft.Xna.Framework;
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
        Vector2 m_Origin,m_Pos;

        float m_Rotation,m_Scale;

        GamePadState m_CurrentState, OldState;
        
        public Player(Texture2D Texture, int X, int Y)
        {
            m_txr = Texture;

            m_Pos = new Vector2(X, Y);
            Collision = new Rectangle(m_txr.Width, m_txr.Height, X, Y);

            m_Origin = new Vector2(m_txr.Width / 2, m_txr.Height / 2); // this might be wrong

            m_Scale = 1;
        }
        public void Update(GamePadState Currpad)
        {
            m_CurrentState = Currpad; // always at start of update


            m_Rotation = (float) Math.Atan2(m_CurrentState.ThumbSticks.Right.X,m_CurrentState.ThumbSticks.Right.Y); // This makes the player face where the right stick is pointed at



            Collision.X = (int)m_Pos.X;
            Collision.Y = (int)m_Pos.Y;

            OldState = m_CurrentState; //Always at the end of update
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(m_txr,m_Pos , null, Color.Red, m_Rotation, m_Origin, m_Scale, SpriteEffects.None, 0f);
        }
    }
}