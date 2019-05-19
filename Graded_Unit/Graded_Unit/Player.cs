using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

enum PlayerDraw
{
    notShoot,
    Shoot,
}


namespace Graded_Unit
{
    /// <summary>
    /// 
    /// This section was completed by Kieran Thorpe mostly
    /// 
    /// The only part that wasn't Completed by him was the HUD Draw section which was done by Archie Millar
    /// 
    /// The reason for why his name doesn't come up on the GitHub authors thing is cause it was uploaded using Kieran's User as at the time it was easier to get him to do it there and then
    /// 
    /// 
    /// </summary>

    class Player
    {
        private static ContentManager content;
        public static ContentManager Content
        {
            protected get { return content; }
            set { content = value; }
        }

        PlayerDraw animation = PlayerDraw.notShoot;
        Texture2D m_txr, BulletTxr;

        Texture2D ShootSpriteSheet;

        Texture2D Hud, HudBullet, Health, Detection;



        SoundEffectInstance WalkSoundInstance, BulletSoundInstance;
        SoundEffect BulletSound, ReloadSound, WalkSound, IntelSound;
        Rectangle CollisionRect, rectangle, HudLeft, HudRight, Visibility, DetectionBar, HealthBar;
        Vector2 m_Origin;
        public Vector2 m_Pos;
        Vector2 Movement;
        int speed;

        public int IntelCollected, MaxIntel;

        public Rectangle CollectionRange;


        List<Ammo> Ammunition = new List<Ammo>();
        List<Bullet> Bullets = new List<Bullet>();
        public List<CollisionTiles> tiles = new List<CollisionTiles>();
        List<Intel> Intellegence = new List<Intel>();

        List<Rectangle> BulletHudRect = new List<Rectangle>();

        Vector2 BulletSpawn;

        float m_Rotation, m_Scale, frame_timer = 1;

        public float DetectionMeter = 0;

        int Width, Height, AmmoCount;

        GamePadState m_CurrentState, OldState;

        public Player(int S, List<CollisionTiles> TILES, List<Ammo> AMMUNITION, List<Intel> INTELLEGENCE, int screenWidth, int screenHeight)
        {
            m_txr = Content.Load<Texture2D>("PlayerAim");

            ShootSpriteSheet = Content.Load<Texture2D>("Shooting");

            BulletTxr = Content.Load<Texture2D>("Bullet");

            BulletSound = Content.Load<SoundEffect>("Gun");
            BulletSoundInstance = BulletSound.CreateInstance();
            BulletSoundInstance.Volume /= 3;

            IntelSound = Content.Load<SoundEffect>("Intel Collected");
            ReloadSound = Content.Load<SoundEffect>("Reload");


            WalkSound = Content.Load<SoundEffect>("Walk");
            WalkSoundInstance = WalkSound.CreateInstance();
            WalkSoundInstance.Volume /= 6;


            Hud = Content.Load<Texture2D>("hud");
            HudBullet = Content.Load<Texture2D>("bullet-hud");
            Detection = Content.Load<Texture2D>("Detection");
            Health = Content.Load<Texture2D>("Health");

            tiles = TILES;
            Ammunition = AMMUNITION;
            Intellegence = INTELLEGENCE;

            MaxIntel = Intellegence.Count;

            Width = screenWidth;
            Height = screenHeight;
            HudLeft = new Rectangle(0, 0, Hud.Width, Hud.Height);
            HudRight = new Rectangle(Width - Hud.Width, 0, Hud.Width, Hud.Height);

            DetectionBar = new Rectangle(35, 50, (int)((Detection.Width / 4 * 3) * DetectionMeter), Detection.Height / 2);
            HealthBar = new Rectangle(35, 50, Health.Width / 4 * 3, Health.Height / 2);

            AmmoCount = 6;
            for (int R = 1; R < AmmoCount + 1; R++)
            {
                BulletHudRect.Add(new Rectangle(35 * R, 120, 35, 35));
            }

            foreach (CollisionTiles tile in tiles)
            {
                if (tile.START)
                {
                    m_Pos = new Vector2((tile.Rectangle.X + tile.Rectangle.Width / 2), (tile.Rectangle.Y + tile.Rectangle.Width / 2));


                    rectangle = new Rectangle(0, 0, m_txr.Width, m_txr.Height);
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
            if (AmmoCount > 6)
            {
                AmmoCount = 6;
            }
            if (AmmoCount < 0)
            {
                AmmoCount = 0;
            }
            // this is for moving onto the next level screen

            for (int i = BulletHudRect.Count - 1; i >= 0; i--)
            {
                if (m_CurrentState.Triggers.Right >= 0.5 && OldState.Triggers.Right < 0.5 && AmmoCount > 0)
                {
                    animation = PlayerDraw.Shoot;
                    Bullets.Add(new Bullet(BulletTxr, (int)BulletSpawn.X, (int)BulletSpawn.Y, m_Rotation));
                    BulletSoundInstance.Play();

                    AmmoCount--;
                    BulletHudRect.RemoveAt(i);

                    OldState = m_CurrentState;
                }
            }


            BulletUpdate(Enemies);
            Collect(exit, CurrentState);
            Detection_Check(Enemies);


            VisibilityCheck(Enemies, exit);
            UpdateCollisions();
            rectMovement(); // this is just for all the rectangle movements so they are all in one place 

            BulletSpawn = new Vector2(((int)m_Pos.X + rectangle.Width / 6), ((int)m_Pos.Y - (rectangle.Height / 3)));
            BulletSpawn = RotateVector2(BulletSpawn, m_Rotation, m_Pos);


            OldState = m_CurrentState; //Always at the end of update
        }
        void BulletUpdate(List<Enemy> Enemies) // all the bullet stuff is done here
        {
            //add the bullet stuff


            //bullet movement update and the condition for if it hits an enemy


            foreach (Bullet bullet in Bullets)
            {
                bullet.Update();
                for (int i = 0; i < Enemies.Count; i++)
                {
                    if (bullet.Collision.Intersects(Enemies[i].CollisionRect))
                    {
                        Enemies[i].EnemyMovement = Type.Death;
                        bullet.isVisible = false;
                    }
                    if (Enemies[i].Colour <= 0f)
                    {
                        Enemies.RemoveAt(i);
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

        void Collect(Exit exit, GameStates CurrentState)
        {
            foreach (Intel intel in Intellegence)
            {
                if (!intel.Collected && m_CurrentState.Buttons.A == ButtonState.Pressed && OldState.Buttons.A == ButtonState.Released && CollectionRange.Intersects(intel.CollisionRect))
                {
                    IntelCollected++;
                    intel.Collected = true;
                    IntelSound.Play();
                }
            }
            foreach (Ammo ammo in Ammunition)
            {
                for (int i = BulletHudRect.Count +1 ; i < 7; i++)
                {
                    if (!ammo.Collected && m_CurrentState.Buttons.A == ButtonState.Pressed && OldState.Buttons.A == ButtonState.Released && CollectionRange.Intersects(ammo.CollisionRect))
                    {
                        AmmoCount++;
                        ammo.Collected = true;
                        ReloadSound.Play();
                        BulletHudRect.Add(new Rectangle(35 * i, 120, 35, 35));
                    }
                }

            }

            for (int A = 0; A < Ammunition.Count; A++)
            {
                if (Ammunition[A].Collected)
                {
                    Ammunition.RemoveAt(A);
                }
            }

        }
        void Collision(Rectangle newRectangle, bool Impassable)
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
        void Detection_Check(List<Enemy> Enemies)
        {
            foreach (Enemy enemy in Enemies)
            {
                if (CollisionRect.Intersects(enemy.Detection) || CollisionRect.Intersects(enemy.CollisionRect) && enemy.VISIBLE)
                {
                    DetectionMeter += 0.005f;
                }
            }

            DetectionBar = new Rectangle(35, 50, (int)((Detection.Width / 4 * 3) * DetectionMeter), Detection.Height / 2);
        }
        void PlayerMovement()
        {
            //movements
            Movement.X = m_CurrentState.ThumbSticks.Left.X * speed;
            Movement.Y = m_CurrentState.ThumbSticks.Left.Y * speed;


            //pointing stuff
            if (m_CurrentState.ThumbSticks.Left.X != 0 || m_CurrentState.ThumbSticks.Left.Y != 0)
            {
                m_Rotation = (float)Math.Atan2(m_CurrentState.ThumbSticks.Left.X, m_CurrentState.ThumbSticks.Left.Y); // This makes the player face where the Left stick is pointed at
                WalkSoundInstance.Play();

            }
            if (m_CurrentState.ThumbSticks.Right.X != 0 || m_CurrentState.ThumbSticks.Right.Y != 0)
            {
                m_Rotation = (float)Math.Atan2(m_CurrentState.ThumbSticks.Right.X, m_CurrentState.ThumbSticks.Right.Y); // This makes the player face where the right stick is pointed at
            }
        }
        void rectMovement()
        {
            m_Pos.X += Movement.X;
            m_Pos.Y -= Movement.Y;


            CollisionRect.X = (int)m_Pos.X - CollisionRect.Width / 2;
            CollisionRect.Y = (int)m_Pos.Y - CollisionRect.Height / 2;

            Visibility.X = (int)m_Pos.X - Visibility.Width / 2;
            Visibility.Y = (int)m_Pos.Y - Visibility.Height / 2;

            CollectionRange.X = (int)m_Pos.X - CollectionRange.Width / 2;
            CollectionRange.Y = (int)m_Pos.Y - CollectionRange.Height / 2;

        }
        void UpdateCollisions() // this is to make it easier to find all the collisions
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


        void VisibilityCheck(List<Enemy> Enemies, Exit exit) // all the visibility bools are done here 
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
                        else if (tile.IMPASSABLE == false)
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
                        else if (tile.IMPASSABLE == false)
                        {
                            intel.Visible = true;
                        }

                    }
                }
            }
            foreach (Ammo ammo in Ammunition)
            {
                foreach (CollisionTiles tile in tiles)
                {
                    if (Visibility.Intersects(ammo.CollisionRect) && ammo.CollisionRect.Intersects(tile.Rectangle))
                    {
                        if (tile.IMPASSABLE)
                        {
                            ammo.Visible = false;
                        }
                        else if (!tile.IMPASSABLE)
                        {
                            ammo.Visible = true;
                        }
                    }
                }
            }
        }



        public void Draw(SpriteBatch sb, Texture2D px, GameTime gt)
        {

            if (frame_timer <= 0)
            {
                rectangle.X = (rectangle.X + rectangle.Width);

                if (rectangle.X >= ShootSpriteSheet.Width)
                {
                    rectangle.X = 0;
                    animation = PlayerDraw.notShoot;
                }

                frame_timer = 1;
            }
            else
            {
                frame_timer -= (float)gt.ElapsedGameTime.TotalSeconds * 24;
            }




            foreach (Bullet bullet in Bullets)
            {
                bullet.Draw(sb);
            }

            switch (animation)
            {
                case PlayerDraw.notShoot:
                    sb.Draw(m_txr, m_Pos, null, Color.White, m_Rotation, m_Origin, m_Scale, SpriteEffects.None, 0f);

                    break;

                case PlayerDraw.Shoot:
                    sb.Draw(ShootSpriteSheet, m_Pos, rectangle, Color.White, m_Rotation, m_Origin, m_Scale, SpriteEffects.None, 0f);

                    break;
            }

            //sb.Draw(px, CollectionRange, Color.Blue * 0.75f);
            // sb.Draw(px, new Rectangle((int)BulletSpawn.X - 1, (int)BulletSpawn.Y - 1, 3, 3), Color.Yellow);
        }

        public void Hud_Draw(SpriteBatch sb, Exit exit) //This is for drawing the HUD elements and this is the stuff Archie done 
        {
            sb.Draw(Hud, HudLeft, Color.White);
            sb.Draw(Hud, HudRight, Color.White);

            sb.DrawString(Content.Load<SpriteFont>("File"), "Collected " + IntelCollected + " / " + MaxIntel, new Vector2(HudRight.X + 30, 50), Color.Black);

            foreach (Rectangle BulletDis in BulletHudRect)
            {
                sb.Draw(HudBullet, BulletDis, Color.White);
            }

            sb.DrawString(Content.Load<SpriteFont>("Hud Font"), "Detection", new Vector2(35, 20), Color.Black);
            sb.DrawString(Content.Load<SpriteFont>("Hud font"), "Ammo", new Vector2(35, 95), Color.Black);

            sb.Draw(Health, HealthBar, Color.White);
            sb.Draw(Detection, DetectionBar, Color.White);

            foreach (Intel intel in Intellegence)
            {
                if (CollectionRange.Intersects(intel.CollisionRect) && intel.Visible && !intel.Collected)
                {
                    sb.Draw(Content.Load<Texture2D>("A-Button"), new Rectangle(400, 920, 50, 50), Color.White);
                    sb.DrawString(Content.Load<SpriteFont>("file"), "Collect Intel", new Vector2(470, 945), Color.White);
                }
            }
            if (IntelCollected == MaxIntel && CollectionRange.Intersects(exit.CollisionRect))
            {
                sb.Draw(Content.Load<Texture2D>("A-Button"), new Rectangle(400, 920, 50, 50), Color.White);
                sb.DrawString(Content.Load<SpriteFont>("file"), "Exit", new Vector2(470, 945), Color.White);
            }
            foreach (Ammo ammo in Ammunition)
            {
                if (CollectionRange.Intersects(ammo.CollisionRect) && ammo.Visible)
                {
                    sb.Draw(Content.Load<Texture2D>("A-Button"), new Rectangle(400, 920, 50, 50), Color.White);
                    sb.DrawString(Content.Load<SpriteFont>("file"), "Collect Ammo", new Vector2(470, 945), Color.White);
                }

            }
        }

        public Vector2 getPos()
        {
            return new Vector2(-m_Pos.X + 960, -m_Pos.Y + 540); // honestly no idea what this does as mark did it before he never returned
        }

        /// <summary>
        /// 
        ///     This is the stuff relating to the rectangles movement and collisions
        /// 
        /// </summary>





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
