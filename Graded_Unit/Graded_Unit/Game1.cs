using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

enum GameStates
{
    Start,
    Instructions,
    Controls,
    Playing,
    Pause,
    Reset,
    DisplayIntel,
}
enum Levels
{

    Level0, //tutorial level
    Level1,
    Level2,
    Level3,

}

namespace Graded_Unit
{

    public class Game1 : Game
    {
        Texture2D debugpixel;

        GameStates CurrentState = GameStates.Start;
        Levels CurrentLevels = Levels.Level0;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Random RNG;
        GamePadState CurrPad, Oldpad;

        Player player;

        Map Tutorial, Level1, Level2, Level3;

        int MinEnemies, MaxEnemies;


        Main_Menu Main;
        PauseScreen Pausing;
        List<Enemy> Enemies;
        List<Intel> Intellegence;
        List<Ammo> Ammunition;

        Exit LevelExit;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            RNG = new Random();
            Tutorial = new Map();
            Level1 = new Map();
            Level2 = new Map();
            Level3 = new Map();
            MinEnemies = 2;
            MaxEnemies = 4;
            Enemies = new List<Enemy>();
            Intellegence = new List<Intel>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            debugpixel = Content.Load<Texture2D>("pixel");

            CollisionTiles.Content = Content; // This allows for the textures to be loaded into the class directly
            Enemy.Content = Content;
            Main_Menu.Content = Content;
            PauseScreen.Content = Content;
            Player.Content = Content;

            Main = new Main_Menu(spriteBatch, graphics);
            Pausing = new PauseScreen(spriteBatch, GraphicsDevice);



            Tutorial.Generate(new int[,]
            {
                {1,1,1,1,1,1,1,1,1,1,1,1},//1
                {1,2,2,2,1,1,1,2,2,2,1,1},//2
                {1,2,3,2,1,1,1,2,2,2,1,1},//3
                {1,1,2,1,1,1,1,2,2,2,1,1},//4
                {1,1,2,1,1,1,1,2,1,1,1,1},//5
                {1,1,2,2,2,2,2,2,2,2,1,1},//6
                {1,1,1,1,1,1,1,1,1,2,1,1},//7
                {1,1,1,1,1,1,1,2,2,2,1,1},//8
                {1,1,1,1,1,1,1,2,2,4,1,1},//9
                {1,1,1,1,1,1,1,2,2,2,1,1},//10
                {1,1,1,1,1,1,1,1,1,1,1,1},//11

            }, 160);


            Level1.Generate(new int[,]
            {
                // This is how the levels map will be generated. if the value is greater than 0 this will place a block
                
                //the far left and right of this also should be ones as well to create an edge around the outside

                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//1 should be all ones,       48 x 27 grid
                {1,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//2
                {1,2,3,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//3       2 for block fills on the ground
                {1,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//4       3 for start
                {1,1,1,2,1,1,1,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//5       4 for Exit
                {1,1,1,2,1,1,1,2,2,2,1,1,1,1,1,1,1,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//6
                {1,1,1,2,1,1,1,2,2,2,1,1,1,1,1,1,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//7
                {1,1,1,2,1,1,1,2,2,2,2,2,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//8
                {1,1,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//9
                {1,1,1,2,1,1,1,2,2,2,2,2,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//10
                {1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//11
                {1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//12
                {1,1,1,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//13
                {1,1,1,2,1,2,1,1,1,1,2,2,2,1,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//14
                {1,1,1,1,1,2,1,1,1,1,2,2,2,1,1,1,1,1,1,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//15
                {1,1,1,1,1,2,2,2,2,2,2,2,2,1,1,1,1,1,1,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//16
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//17
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//18
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//19
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//20
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//21
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//22
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//23
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//24
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//25
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//26
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//27 should be all ones
                
            }, 160);

            Level2.Generate(new int[,]
            {

                //the far left and right of this also should be ones as well to create an edge around the outside

                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//1 should be all ones,       48 x 27 grid
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,3,3,1,1,1},//2
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//3       1 for block fills on the ground
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//4       3 for start
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//5
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//6
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//7
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//8
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//9
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//10
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//11
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//12
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//13
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//14
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//15
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//16
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//17
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//18
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//19
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//20
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//21
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//22
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//23
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//24
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//25
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//26
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//27 should be all ones

            }, 160);
            Level3.Generate(new int[,]
            {
                
                //the far left and right of this also should be ones as well to create an edge around the outside

                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//1 should be all ones,       
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//2
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//3       2 for block fills on the ground
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//4       
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//5
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//6
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//7
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//8
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//9
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//10
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//11
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//12
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//13
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//14
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//15
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//16
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//17
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//18
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//19
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//20
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//21
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//22
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//23
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//24
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//25
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//26
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//27 should be all ones

            }, 160);

            
            for (int i = 0; i < RNG.Next(MinEnemies, MaxEnemies); i++)
            {
                Enemies.Add(new Enemy(RNG, Tutorial.Width, Tutorial.Height, Tutorial.CollisionTiles));

            }

            for(int i = 0; i < 1; i++)
            {
                Intellegence.Add(new Intel(Content.Load<Texture2D>("intel"), new Vector2(RNG.Next(160, Tutorial.Width - 160), RNG.Next(160, Tutorial.Height - 160)),RNG,Tutorial.CollisionTiles,new Vector2(Tutorial.Width,Tutorial.Height)));
            }

            LevelExit = new Exit(Content.Load<Texture2D>("hatch-door"), new Vector2(0, 0), RNG, Tutorial.CollisionTiles, new Vector2(0, 0));

            player = new Player(3, Tutorial.CollisionTiles, Ammunition, Intellegence, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight); // CHANGE THE LIST WHEN FINISHED 


            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {

            CurrPad = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.None);

            if (CurrPad.Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            switch (CurrentState)
            {
                case GameStates.Start:

                    Main.Update(CurrPad);
                    //if (Keyboard.GetState().IsKeyDown(Keys.A)) //for debug puporses currently
                    if (CurrPad.Buttons.A == ButtonState.Pressed && Oldpad.Buttons.A == ButtonState.Released)
                    {
                        if (Main.ReturnSelection() == 0)
                            CurrentState = GameStates.Playing;
                        else if (Main.ReturnSelection() == 1)
                            CurrentState = GameStates.Instructions;
                        else if (Main.ReturnSelection() == 2)
                            Exit();
                    }
                    break;

                case GameStates.Instructions:

                    break;

                case GameStates.Controls:

                    break;

                case GameStates.Playing:

                    LevelExit.Update();

                    player.Update(CurrPad, Enemies,CurrentState,LevelExit);

                    foreach (Enemy enemy in Enemies)
                    {
                        enemy.Update(gameTime);
                    }
                    foreach (Intel intel in Intellegence)
                    {
                        intel.Update(gameTime);
                    }
                    //Pauses the game
                    switch (CurrentLevels)
                    {
                        case Levels.Level0:
                            if (CurrPad.Buttons.Start == ButtonState.Pressed && Oldpad.Buttons.Start == ButtonState.Released)
                            {
                                CurrentState = GameStates.Pause;
                            }
                            break;

                        case Levels.Level1:
                            if (CurrPad.Buttons.Start == ButtonState.Pressed && Oldpad.Buttons.Start == ButtonState.Released)
                            {
                                CurrentState = GameStates.Pause;
                            }
                            break;

                        case Levels.Level2:
                            if (CurrPad.Buttons.Start == ButtonState.Pressed && Oldpad.Buttons.Start == ButtonState.Released)
                            {
                                CurrentState = GameStates.Pause;
                            }
                            break;

                        case Levels.Level3:
                            if (CurrPad.Buttons.Start == ButtonState.Pressed && Oldpad.Buttons.Start == ButtonState.Released)
                            {
                                CurrentState = GameStates.Pause;
                            }
                            break;
                    }


                    break;

                case GameStates.Pause:
                    //Resumes the game
                    if (CurrPad.Buttons.Start == ButtonState.Pressed && Oldpad.Buttons.Start == ButtonState.Released)
                    {
                        CurrentState = GameStates.Playing;
                    }
                    break;
                case GameStates.DisplayIntel:
                    switch (CurrentLevels)
                    {
                        case Levels.Level0:
                            if (CurrPad.Buttons.A == ButtonState.Pressed && Oldpad.Buttons.A == ButtonState.Released)
                            {
                                CurrentState = GameStates.Reset;
                            }
                            break;

                        case Levels.Level1:
                            if (CurrPad.Buttons.A == ButtonState.Pressed && Oldpad.Buttons.A == ButtonState.Released)
                            {
                                CurrentState = GameStates.Reset;
                            }

                            break;

                        case Levels.Level2:
                            if (CurrPad.Buttons.A == ButtonState.Pressed && Oldpad.Buttons.A == ButtonState.Released)
                            {
                                CurrentState = GameStates.Reset;
                            }
                            break;
                        case Levels.Level3:
                            if (CurrPad.Buttons.A == ButtonState.Pressed && Oldpad.Buttons.A == ButtonState.Released)
                            {
                                CurrentState = GameStates.Reset;
                            }
                            break;
                    }


                    break;


                case GameStates.Reset:
                    switch (CurrentLevels)
                    {
                        case Levels.Level0:
                            SwitchMap(Level1, MinEnemies, MaxEnemies);
                            CurrentLevels = Levels.Level1;
                            CurrentState = GameStates.Playing; //always at the end so it goes back to playing

                            break;
                        case Levels.Level1:
                            SwitchMap(Level2, MinEnemies, MaxEnemies);
                            CurrentLevels = Levels.Level2;
                            CurrentState = GameStates.Playing; //always at the end so it goes back to playing
                            break;
                        case Levels.Level2:
                            SwitchMap(Level3, MinEnemies, MaxEnemies);
                            CurrentLevels = Levels.Level3;
                            CurrentState = GameStates.Playing; //always at the end so it goes back to playing
                            break;
                    }
                    break;
            }


            Oldpad = CurrPad;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(); //this is specfically for drawing the menu stuff
            switch (CurrentState)
            {
                case GameStates.Start:

                    Main.Draw();
                    break;
                case GameStates.Instructions:

                    break;

            }
            spriteBatch.End();

            /*
             * 
             * 
             * This next draw is specifically for the playing aspect of the game
             * 
             * 
             * 
             */

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateTranslation(new Vector3(player.getPos().X, player.getPos().Y, 0)));

            switch (CurrentState)
            {

                case GameStates.Playing:

                    switch (CurrentLevels)
                    {
                        case Levels.Level0:

                            Tutorial.Draw(spriteBatch);

                            break;

                        case Levels.Level1:

                            Level1.Draw(spriteBatch);

                            break;

                        case Levels.Level2:
                            Level2.Draw(spriteBatch);
                            break;
                        case Levels.Level3:
                            Level3.Draw(spriteBatch);
                            break;
                    }

                    foreach (Enemy enemy in Enemies)
                    {
                        enemy.Draw(spriteBatch);
                    }
                    foreach(Intel intel in Intellegence)
                    {
                        intel.Draw(spriteBatch);
                    }
                    LevelExit.Draw(spriteBatch);
                    player.Draw(spriteBatch, debugpixel);


                    break;
                case GameStates.Pause: // Keeps eveything drawing whilst not updating

                    switch (CurrentLevels)
                    {
                        case Levels.Level0:

                            Tutorial.Draw(spriteBatch);

                            break;

                        case Levels.Level1:

                            Level1.Draw(spriteBatch);

                            break;

                        case Levels.Level2:

                            break;
                        case Levels.Level3:

                            break;
                    }

                    foreach (Enemy enemy in Enemies)
                    {
                        enemy.Draw(spriteBatch);
                    }
                    player.Draw(spriteBatch, debugpixel);


                    break;
            }


            spriteBatch.End();

            /*
             * 
             * This is the draw part for static images
             * 
             * 
             * 
             */


            spriteBatch.Begin(); //this is specfically for drawing the HUD and pause screen
            switch (CurrentState)
            {
                // you know when you dont know if something is going to work and then are surprised when it does
                // that is this
                case GameStates.Playing:
                    player.Hud_Draw(spriteBatch);
                    break;

                case GameStates.Pause:
                    Pausing.Draw();
                    break;

            }
            spriteBatch.End();


            base.Draw(gameTime);
        }
        void SwitchMap(Map Current, int min, int max)
        {
            Enemies.Clear();
            MinEnemies = min;
            MaxEnemies = max;
            for (int i = 0; i < RNG.Next(MinEnemies, MaxEnemies); i++)
            {
                Enemies.Add(new Enemy(RNG, Current.Width, Current.Height, Current.CollisionTiles));
            }

            player = new Player(3, Current.CollisionTiles, Ammunition, Intellegence, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }
    }
}
