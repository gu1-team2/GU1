using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


namespace Graded_Unit
{
    // The most of this was done by callum

    // The reason for why his name doesn't come up on the GitHub authors thing is cause it was uploaded using Kieran's User as at the time it was easier to get him to do it there and then

    class Collectables
    {
        public Texture2D txr,txr2;

        Vector2 Pos;

        public Vector2 MapSize;


        public Rectangle CollisionRect;

        public Random Randy_The_Randy_Number_Gen;

        public float Timer = 0;
        public int Seconds = 0, ImStuckCounter = 0;

        public bool Collected, Exit, Visible, Impassable;

        public List<CollisionTiles> Tiles;


        public Collectables(Texture2D TEXTURE, Vector2 POSITION, Random A_Generator_Of_Random_Numbers, List<CollisionTiles> TILES, Vector2 Size)
        {
            txr = TEXTURE;

            Pos = POSITION;

            CollisionRect = new Rectangle((int)Pos.X, (int)Pos.Y, txr.Width, txr.Height);

            Randy_The_Randy_Number_Gen = A_Generator_Of_Random_Numbers;

            MapSize = Size;

            Tiles = TILES;
        }
        public void Draw(SpriteBatch sb)
        {
            if (Visible)
            {
                sb.Draw(txr, CollisionRect, Color.White);
            }

        }
    }

    class Ammo : Collectables
    {
        public Ammo(Texture2D TEXTURE, Vector2 POSITION, Random A_Generator_Of_Random_Numbers, List<CollisionTiles> TILES, Vector2 Size) : base(TEXTURE, POSITION, A_Generator_Of_Random_Numbers, TILES, Size)
        {
            Collected = false;
            Exit = false;
            Visible = false;
            Impassable = false;
            CollisionRect.Width = CollisionRect.Width / 2;
            CollisionRect.Height = CollisionRect.Height / 2;
        }
        public void Update(GameTime gameTime)
        {
            Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Seconds += (int)Timer;
            if (Timer >= 1f)
            {
                Timer = 0f;
            }
            foreach (CollisionTiles tile in Tiles)
            {
                if (CollisionRect.Intersects(tile.Rectangle) && tile.IMPASSABLE)
                {
                    ImStuckCounter++;
                }
            }

            if (Seconds >= 2 && ImStuckCounter >= 100)
            {
                CollisionRect.X = Randy_The_Randy_Number_Gen.Next(160, (int)MapSize.X - 160);
                CollisionRect.Y = Randy_The_Randy_Number_Gen.Next(160, (int)MapSize.Y - 160);
                Seconds = 0;
                ImStuckCounter = 0;
            }
            else if (Seconds >= 2 && ImStuckCounter < 100)
            {
                Seconds = 0;
                ImStuckCounter = 0;
            }
        }
    }

    class Intel : Collectables
    {
        public Intel(Texture2D TEXTURE,Texture2D TEXTURE2, Vector2 POSITION, Random A_Generator_Of_Random_Numbers, List<CollisionTiles> TILES, Vector2 Size) : base(TEXTURE, POSITION, A_Generator_Of_Random_Numbers, TILES, Size)
        {
            Collected = false;
            Exit = false;
            Visible = false;
            Impassable = true;
            txr = TEXTURE;
            txr2 = TEXTURE2;
        }

        public void Update(GameTime gameTime)
        {
            Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Seconds += (int)Timer;
            if (Timer >= 1f)
            {
                Timer = 0f;
            }
            foreach (CollisionTiles tile in Tiles)
            {
                if (CollisionRect.Intersects(tile.Rectangle) && tile.IMPASSABLE)
                {
                    ImStuckCounter++;
                    Visible = false;
                }
            }

            if (Seconds >= 2 && ImStuckCounter >= 100)
            {
                CollisionRect.X = Randy_The_Randy_Number_Gen.Next(160, (int)MapSize.X - 160);
                CollisionRect.Y = Randy_The_Randy_Number_Gen.Next(160, (int)MapSize.Y - 160);
                Seconds = 0;
                ImStuckCounter = 0;
            }
            else if (Seconds >= 2 && ImStuckCounter < 100)
            {
                Seconds = 0;
                ImStuckCounter = 0;
            }
            if(Collected)
            {
                txr = txr2;  
            }
        }
    }
    class Exit : Collectables
    {

        public Exit(Texture2D TEXTURE, Vector2 POSITION, Random A_Generator_Of_Random_Numbers, List<CollisionTiles> TILES, Vector2 Size) : base(TEXTURE, POSITION, A_Generator_Of_Random_Numbers, TILES, Size)
        {
            Collected = false;
            Exit = true;
            Visible = false;
            Impassable = false;

        }

        public void Update()
        {
            foreach (CollisionTiles tile in Tiles)
            {
                if (tile.EXIT)
                {
                    CollisionRect.X = (tile.Rectangle.X + CollisionRect.Width / 3);
                    CollisionRect.Y = (tile.Rectangle.Y + CollisionRect.Height / 3);
                }
            }
        }
    }
}

