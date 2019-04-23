using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

enum Type
{
    Moving,
    Stationary,
}

namespace Graded_Unit
{
    class Enemy
    {
        Type EnemyVariation;

        private static ContentManager content;
        public static ContentManager Content
        {
            protected get { return content; }
            set { content = value; }
        }

        Texture2D Texture, BulletTexture;
        Vector2 Position, Origin;
        Rectangle Collision;
        Random RNG = new Random();
        float Rotation, Scale;

        List<Bullet> En_Bullets;

        public Enemy(int X, int Y, List<CollisionTiles> Tiles)
        {
            switch (EnemyVariation)
            {
                case Type.Moving:

                    break;
                case Type.Stationary:

                    break;
            }

            Texture = Content.Load<Texture2D>("");
            BulletTexture = Content.Load<Texture2D>("");
            Position = new Vector2(X, Y);
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Rotation = 0;
            Scale = 1;
            Collision = new Rectangle(X - Texture.Width / 2, Y - Texture.Height / 2, Texture.Width, Texture.Height);
            foreach (CollisionTiles Tile in Tiles)
            {
                if (Collision.Intersects(Tile.Rectangle))
                {
                    Position = new Vector2(RNG.Next(0, Tile.Rectangle.Width * Tiles.Count),RNG.Next(0,Tile.Rectangle.Height* Tiles.Count)); // this doesn't work as it gets the total count outside the list
                    Collision = new Rectangle(X - Texture.Width / 2, Y - Texture.Height / 2, Texture.Width, Texture.Height);
                }
            }
        }
        public void Update()
        {

        }
        public void Draw(SpriteBatch SB)
        {
            SB.Draw(Texture, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0f);
        }
    }

}
