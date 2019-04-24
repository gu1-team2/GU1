﻿using System;
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
    class Main_Menu //Started 24-04-2019 by Rory
    {
        enum Selection //States that hold the displayed text
        {
            Play,
            Info,
            Exit
        }

        private static ContentManager content;
        public static ContentManager Content
        {
            protected get { return content; }
            set { content = value; }
        }

        private Selection Highlighted;

        private Texture2D Background;
        private SpriteFont MenuFont;
        private GamePadState CurrPad;       //usual declarations
        private SpriteBatch sb;
        private GraphicsDeviceManager gdm;

        private string DisplayedText;
        public int i;

        public Main_Menu(SpriteBatch S, GraphicsDeviceManager G)
        {
            switch(Highlighted)
            {
                case Selection.Play:
                    DisplayedText = ("Info" + Environment.NewLine + "Play" +Environment.NewLine + "Exit");
                    break;

                case Selection.Info:
                    DisplayedText = ("Exit" + Environment.NewLine + "Info" + Environment.NewLine + "Play");
                    break;

                case Selection.Exit:
                    DisplayedText = ("Play" + Environment.NewLine + "Exit" + Environment.NewLine + "Info");
                    break;

            }

            i = (int)Selection.Play;
            Background = Content.Load<Texture2D>("background");
            MenuFont = Content.Load<SpriteFont>("");
            sb = S;
        }
        public int ReturnSelection() // Function returns value of i to hook into Game1
        {
            return i;
        }

        public void Update() //Updates the selection when the gamepad is pressed
        {
            CurrPad = GamePad.GetState(PlayerIndex.One);
            if (CurrPad.DPad.Up == ButtonState.Pressed)
                i++;

            if (CurrPad.DPad.Down == ButtonState.Pressed)
                i--;

            if (i == 0)
                Highlighted = Selection.Play;
            if (i == 1)
                Highlighted = Selection.Info;
            if (i == 2)
                Highlighted = Selection.Exit;

            if (i > 2)
                i = 0;

            if (CurrPad.Buttons.A == ButtonState.Pressed) //Calls ReturnSelection to return i into Game1
            {
                ReturnSelection();
            }
        }

        public void Draw() //Draws the menu assets
        {
            sb.Draw(Background, new Rectangle(0, 0, gdm.PreferredBackBufferWidth, gdm.PreferredBackBufferHeight), Color.White);
            sb.DrawString(MenuFont, DisplayedText, new Vector2(100, 100), Color.White);
        }
    }
}