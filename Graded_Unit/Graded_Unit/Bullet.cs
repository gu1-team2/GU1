using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Graded_Unit
{
    // this mainly part of the player class and was completed by kieran
    class Bullet
    {
        Texture2D Texture;
        Vector2 Pos;
        Vector2 Direction;
        public Rectangle Collision;
        float Rotation;
        int Speed = 15;
        public bool isVisible;

        public Bullet(Texture2D TXR, int X, int Y, float ROT)
        {
            Texture = TXR;
            Pos = new Vector2(X , Y ); // this makes the bullet centred on the player. The reason for 8 is the bullet is a forth of the size of the texture and needs to be half that
            Rotation = ROT;
            Collision = new Rectangle(X, Y, TXR.Width / 6, TXR.Height / 6);
            isVisible = true;
        }
        public void Update()
        {
            Direction = new Vector2((float)Math.Sin(Rotation), -(float)Math.Cos(Rotation));
            Direction.Normalize();
            Pos += Direction * Speed;
            Collision.X = (int)Pos.X;
            Collision.Y = (int)Pos.Y;
        }
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, Pos, null, Color.White, Rotation, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

    }
}
