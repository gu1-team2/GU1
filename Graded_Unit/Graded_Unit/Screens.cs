using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Graded_Unit
{

    /// <summary>
    /// This is the stuff that was done by archie 
    /// 
    /// </summary>
    class Screens
    {
        Texture2D Display, A_Button;
        SpriteFont font;
        Rectangle Position, ButtonPos;
        Vector2 WordPos;

        float Timer;
        public int Seconds;

        public Screens(Texture2D DISPLAY, Texture2D a_Buttn, SpriteFont fnt)
        {
            Display = DISPLAY;
            A_Button = a_Buttn;
            font = fnt;

            Position = new Rectangle(0, 0, Display.Width, Display.Height);
            ButtonPos = new Rectangle(1920 - (A_Button.Width/2), 1080 - (A_Button.Height/2), A_Button.Width/2, A_Button.Height/2);

            WordPos = new Vector2(ButtonPos.X - 240, ButtonPos.Y + A_Button.Height/4);
        }
        public void Update(GameTime gameTime)
        {
            Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Seconds += (int)Timer;
            if (Timer >= 1f)
            {
                Timer = 0f;
            }

        }

        public void Draw(SpriteBatch SB)
        {
            SB.Draw(Display, Position, Color.White);
            if (Seconds > 5)
            {
                SB.Draw(A_Button, ButtonPos, Color.White);
                SB.DrawString(font, "Continue", WordPos, Color.White);
            }
        }
    }
}
