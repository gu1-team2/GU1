using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Graded_Unit
{
    class Bullet
    {
        Texture2D Texture;
        Vector2 Pos;
        Vector2 Origin;
        Rectangle Collision;
        float Rotation;
        int Scale = 1;

        public Bullet(Texture2D TXR, int X, int Y,float ROT)
        {
            Texture = TXR;
            Origin = new Vector2(TXR.Width / 2, TXR.Height / 2);
            Pos = new Vector2(X, Y);
            Rotation = ROT;
            Collision = new Rectangle(X, Y, TXR.Width, TXR.Height);
        }
        public void Update()
        {

        }
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, Pos, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0f);
        }

    }
}
