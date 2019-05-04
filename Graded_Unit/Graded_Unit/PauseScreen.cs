using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graded_Unit
{
    class PauseScreen
    {
        Color Editable;
        SpriteBatch SB;
        GraphicsDevice GD;
        SpriteFont font;

        private static ContentManager content;
        public static ContentManager Content
        {
            protected get { return content; }
            set { content = value; }
        }

        public PauseScreen(SpriteBatch sb, GraphicsDevice gd)
        {
            Editable = new Color(Color.Black, 0.5f);
            SB = sb;
            GD = gd;
            font = content.Load<SpriteFont>("File");
        }

        public void Draw()
        {
            GD.Clear(Editable);
            SB.DrawString(font, "PAUSED", new Vector2(840, 520), Color.White);
        }
    }
}
