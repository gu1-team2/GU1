using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
    Detected,
    GameOver,
    DisplayIntel,
    Completed,
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

        SoundEffect alarm;
        SoundEffectInstance InstanceAlarm;


        int Countdown = 3;
        float CountDownTimer;

        int Respawn = 5;
        float RespawnCounter;

        Player player;

        Map Tutorial, Level1, Level2, Level3;

        int MinEnemies, MaxEnemies;

        Screens Controls;
        Screens Completed, Instructions, GameOverScreen;

        List<Screens> DisplayIntel;

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
            Ammunition = new List<Ammo>();
            DisplayIntel = new List<Screens>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            debugpixel = Content.Load<Texture2D>("pixel");

            alarm = Content.Load<SoundEffect>("Alarm");
            InstanceAlarm = alarm.CreateInstance();


            CollisionTiles.Content = Content; // This allows for the textures to be loaded into the class directly
            Enemy.Content = Content;
            Main_Menu.Content = Content;
            PauseScreen.Content = Content;
            Player.Content = Content;

            Main = new Main_Menu(spriteBatch, graphics);
            Pausing = new PauseScreen(spriteBatch, GraphicsDevice);

            Controls = new Screens(Content.Load<Texture2D>("Control-screen"), Content.Load<Texture2D>("A-Button"), Content.Load<SpriteFont>("File"));
            Completed = new Screens(Content.Load<Texture2D>("EndGame"), Content.Load<Texture2D>("A-Button"), Content.Load<SpriteFont>("File"));
            GameOverScreen = new Screens(Content.Load<Texture2D>("GameOver"), Content.Load<Texture2D>("A-Button"), Content.Load<SpriteFont>("File"));
            Instructions = new Screens(Content.Load<Texture2D>("EndGame"), Content.Load<Texture2D>("A-Button"), Content.Load<SpriteFont>("File"));

            for (int D = 0; D <= (int)Levels.Level3; D++)
            {
                DisplayIntel.Add(new Screens(Content.Load<Texture2D>("Intel-Piece-" + D), Content.Load<Texture2D>("A-Button"), Content.Load<SpriteFont>("File")));
            }

            Tutorial.Generate(new int[,]
            {
                {1,1,1,1,1,1,1,1,1,1,1,1},//1   1 for walls
                {1,2,2,2,1,1,1,2,2,2,1,1},//2   2 for path
                {1,2,3,2,2,2,2,2,2,2,1,1},//3   3 for start 
                {1,1,2,1,1,1,1,2,2,2,1,1},//4   4 for exit
                {1,1,2,1,1,1,1,2,1,1,1,1},//5
                {1,1,2,2,2,2,2,2,2,2,1,1},//6
                {1,1,2,1,1,1,1,1,1,2,1,1},//7
                {1,1,2,1,1,1,1,2,2,2,1,1},//8
                {1,2,2,2,2,2,2,2,2,4,1,1},//9
                {1,2,2,2,1,1,1,2,2,2,1,1},//10
                {1,1,1,1,1,1,1,1,1,1,1,1},//11

            }, 160);


            Level1.Generate(new int[,]
            {
                // This is how the levels map will be generated. if the value is greater than 0 this will place a block
                
                //the far left and right of this also should be ones as well to create an edge around the outside

                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//1 should be all ones,       48 x 27 grid
                {1,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,2,2,2,1,1,1,1,1,1,1},//2
                {1,2,3,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,1,1,1,1,1,2,2,2,2,2,2,1,1,1,2,2,2,2,2,1,1,1,1,1},//3       2 for block fills on the ground
                {1,2,2,2,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,1,1,1,2,1,2,2,2,2,1,1,1,2,2,2,1,2,1,1,1,1,1},//4       3 for start
                {1,1,1,2,1,1,1,2,2,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,1,2,1,1,1,2,1,1,1,1,2,1,1,1,2,2,2,1,2,2,1,1,1,1},//5       4 for Exit
                {1,1,1,2,1,1,1,2,2,2,1,1,1,1,1,1,1,2,2,2,2,2,1,1,1,2,2,2,2,2,1,1,1,1,2,2,2,2,2,2,2,1,1,2,1,1,1,1},//6
                {1,1,1,2,1,1,1,2,2,2,1,1,1,1,1,1,2,2,1,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1},//7
                {1,1,1,2,1,1,1,2,2,2,2,2,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,1,1,1,1},//8
                {1,1,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1,1,1,1,1,1,2,2,2,1,2,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1},//9
                {1,1,1,2,1,1,1,2,2,2,2,2,1,1,1,1,2,1,1,2,2,2,2,2,2,2,1,2,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1},//10
                {1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1},//11
                {1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1},//12
                {1,1,1,2,2,2,1,1,1,1,1,1,1,1,2,2,2,1,1,2,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,2,1,1,1,1,1,1,1,1,1},//13
                {1,1,1,2,1,2,1,1,1,1,2,2,2,1,2,1,2,2,1,2,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,2,2,2,1,1,1,1,1,1,1,1},//14
                {1,1,1,2,1,2,1,1,1,1,2,2,2,2,2,1,1,1,1,2,2,2,2,1,1,1,1,1,2,2,2,2,2,4,2,1,2,2,2,2,1,1,1,1,1,1,1,1},//15
                {1,1,1,2,1,2,2,2,2,2,2,2,2,1,1,1,1,1,1,2,2,2,2,1,1,1,1,1,2,1,1,2,2,2,2,1,2,2,2,2,1,1,1,1,1,1,1,1},//16
                {1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,1,1,2,1,1,1,2,1,1,1,2,2,2,2,1,1,1,1,1,1,1,1},//17
                {1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,1,1,2,1,1,1,2,2,2,2,2,1,1,1,1,1,2,1,1,1,1,1},//18
                {1,1,1,2,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1,1,1,2,1,1,1,1,1,1,1,1,1,2,1,1,1,1,1},//19
                {1,1,2,2,2,1,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1,2,1,1,1,1,1,2,2,2,2,2,1,1,1,1,1},//20
                {1,1,2,2,2,1,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1,2,1,1,1,1,1,2,1,1,1,2,1,1,1,1,1},//21
                {1,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1,1,1,1,2,1,1,2,2,2,2,2,1,1,1,1,2,2,2,2,2,1,2,1,1,1,2,1,1,1,1,1},//22
                {1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,2,2,2,1,1,2,1,1,1,2,1,1,1,1,1,2,2,2,2,2,2,1,1,2,2,2,1,1,1,1},//23
                {1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,1,2,1,1,2,1,1,1,2,2,2,2,2,2,2,2,2,2,1,1,1,1,2,2,2,1,1,1,1},//24
                {1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,1,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,1,1,1,1},//25
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//26
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//27 should be all ones
                
            }, 160); // open these at your own risk

            Level2.Generate(new int[,]
            {

                //the far left and right of this also should be ones as well to create an edge around the outside

                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//1 should be all ones,       48 x 27 grid
                {1,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,2,2,4,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2,1,1},//2
                {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1,1,2,1,1,1,2,2,2,1,1,1,1,1,1,1,1,1,1,1,2,2,1,1,2,1,1,2,2,1},//3       1 for block fills on the wall
                {1,2,2,2,1,1,1,1,1,2,1,1,1,1,1,1,2,2,2,2,2,2,2,1,2,1,2,2,2,1,1,1,1,1,1,1,1,1,2,2,1,2,2,1,1,1,2,1},//4       3 for start
                {1,1,2,1,1,1,1,1,1,2,1,1,1,1,1,1,2,2,2,2,2,1,2,2,2,1,1,2,2,2,2,1,1,1,1,1,1,1,2,2,2,2,1,1,1,1,2,1},//5		2 for ground
                {1,1,2,1,1,1,2,2,2,2,1,1,1,1,1,1,2,2,2,2,2,1,1,1,1,1,1,2,2,2,2,1,1,1,2,2,2,2,2,2,1,1,1,1,1,1,2,1},//6		4 for exit
                {1,1,2,1,1,1,2,2,2,2,1,1,1,1,1,1,1,1,2,2,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,2,2,2,1},//7
                {1,1,2,1,1,1,2,1,1,2,1,1,1,1,1,1,1,1,2,2,1,1,1,1,1,1,1,1,2,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1,2,2,2,1},//8
                {1,1,2,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,2,2,1,1,1,1,1,1,1,1,2,1,1,1,1,2,2,1,1,1,1,1,1,2,2,2,2,2,2,1},//9
                {1,2,2,1,1,1,1,2,2,1,1,1,1,1,1,2,2,2,2,2,2,2,2,2,2,1,1,1,2,1,1,1,1,2,2,2,2,2,1,1,1,2,1,1,2,2,2,1},//10
                {1,2,2,1,1,1,1,2,2,1,1,2,2,2,2,2,2,1,1,1,1,1,1,1,2,1,2,2,2,1,1,1,1,2,2,2,1,2,1,1,1,2,1,1,1,1,1,1},//11
                {1,2,2,1,1,1,1,2,2,1,1,2,1,1,1,2,2,1,1,1,1,1,1,1,2,2,2,1,1,1,1,1,1,2,2,2,2,2,2,2,2,2,1,1,1,1,1,1},//12
                {1,2,2,2,2,1,1,2,2,2,2,2,1,1,1,2,2,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1},//13
                {1,1,1,1,2,2,2,2,1,1,1,1,1,1,2,2,2,1,1,1,1,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,1,1,1,1,1,1},//14
                {1,1,1,1,2,2,1,2,2,2,2,2,2,2,2,1,2,2,2,2,2,2,3,2,2,2,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2,1,1,1,1,1,1,1},//15
                {1,1,1,1,2,2,2,2,1,2,1,1,2,1,2,2,2,1,1,1,1,2,2,2,1,2,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//16
                {1,1,1,1,1,2,1,1,1,2,1,1,2,1,1,1,2,1,1,1,1,1,2,2,2,2,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//17
                {1,1,1,1,1,2,1,1,1,2,1,1,2,1,1,1,2,1,1,1,1,1,1,1,1,2,2,2,2,2,1,1,1,2,1,2,2,1,1,1,1,1,1,1,1,1,1,1},//18
                {1,1,2,2,2,2,1,1,1,2,1,1,2,1,1,1,2,1,1,1,1,1,1,1,2,2,1,1,1,2,1,1,1,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1},//19
                {1,1,2,1,1,2,2,2,2,2,1,1,2,1,1,1,2,1,1,1,1,2,2,2,2,1,1,1,1,2,1,1,1,1,1,2,1,2,2,2,1,1,1,1,1,1,1,1},//20
                {1,1,2,2,2,2,2,1,1,1,1,1,2,1,1,1,2,2,1,1,1,2,1,2,1,1,1,1,1,2,1,1,1,1,1,2,2,2,1,2,1,1,1,1,1,1,1,1},//21
                {1,1,2,2,2,1,1,1,1,1,1,1,2,1,1,1,1,2,1,1,1,2,1,2,1,1,1,2,2,2,2,2,2,2,2,2,1,1,1,2,2,2,2,2,1,1,1,1},//22
                {1,1,2,2,2,2,2,2,2,2,2,2,2,1,1,1,1,2,2,2,2,2,1,2,2,1,2,2,1,1,1,1,1,1,1,2,1,1,1,2,2,1,1,2,1,1,1,1},//23
                {1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,2,2,2,1,1,1,1,1,2,1,1,2,1,1,1,1,1,1,1,1,2,1,1,1,2,2,1,1,2,1,1,1,1},//24
                {1,1,1,1,1,1,1,1,1,1,1,1,2,2,1,2,2,2,1,1,1,1,1,2,2,2,2,2,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2,2,1,1,1,1},//25
                {1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//26
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//27 should be all ones

            }, 160);
            Level3.Generate(new int[,]
            {
                
                //the far left and right of this also should be ones as well to create an edge around the outside

				{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//1 should be all ones,       
                {1,1,1,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,1,1,1,1,1,1,1,1},//2		1 for trumpets wall
                {1,1,1,2,2,2,2,2,2,2,2,2,2,2,1,1,2,2,2,2,2,2,2,1,1,1,2,2,2,1,1,1,1,2,2,2,2,2,1,2,1,2,2,2,1,1,1,1},//3       2 for block fills on the ground
                {1,1,1,2,2,2,2,2,2,1,1,1,1,2,2,2,2,1,1,1,1,1,2,1,1,1,2,2,2,1,1,2,2,2,1,2,1,2,2,2,1,2,1,2,1,1,1,1},//4       3 for start
                {1,1,1,2,2,2,2,2,2,1,1,1,1,2,1,1,1,1,1,1,1,1,2,1,1,2,2,2,2,1,1,2,1,2,2,2,2,2,1,2,2,2,1,2,1,1,1,1},//5		4 for exit
                {1,1,1,1,1,2,2,1,1,1,2,2,2,2,2,2,1,1,1,2,2,2,2,2,1,2,1,1,1,2,2,2,1,2,1,2,1,2,2,2,1,1,2,2,2,1,1,1},//6
                {1,1,1,1,1,2,2,1,1,1,2,2,2,2,2,2,1,1,1,2,2,2,1,2,2,2,2,2,1,2,1,1,1,2,2,2,2,2,1,1,1,1,2,2,2,1,1,1},//7
                {1,1,1,1,1,2,2,2,2,2,2,1,2,1,2,2,1,1,1,2,2,2,1,1,1,1,2,2,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1},//8
                {1,1,1,1,1,2,2,2,2,2,2,1,2,1,2,2,2,2,1,1,2,2,1,1,1,1,2,2,1,2,2,2,2,2,2,2,2,1,1,1,1,1,1,2,1,1,1,1},//9
                {1,1,1,1,1,2,2,1,1,1,2,2,2,2,2,2,1,2,2,2,2,2,1,1,1,1,2,2,1,2,1,1,1,1,1,1,2,1,1,1,1,2,2,2,1,1,1,1},//10
                {1,1,1,1,2,2,2,2,1,1,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,1,1,1,2,1,1,1,1,2,1,1,1,1,1,1},//11
                {1,1,1,1,2,2,2,2,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1,2,2,2,1,1,1,1,2,1,2,2,2,1,1},//12
                {1,1,1,1,2,3,2,2,1,1,1,1,2,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,1,2,2,2,1,1,2,2,2,2,1,1,1,2,2,2,1,2,1,1},//13
                {1,1,1,1,2,2,2,2,2,2,2,2,2,2,2,1,1,1,1,1,1,2,2,2,2,1,1,1,1,1,2,2,2,1,2,1,1,2,2,2,1,1,2,2,2,2,1,1},//14
                {1,1,1,2,2,2,2,2,1,1,1,1,1,1,2,2,2,2,1,1,1,2,1,1,2,2,2,2,2,2,2,1,2,2,2,1,1,2,1,2,1,1,2,1,2,1,1,1},//15
                {1,1,1,2,1,1,1,2,1,1,1,1,1,1,2,1,1,2,1,2,2,2,2,1,2,1,1,1,1,1,1,1,2,2,1,1,1,2,1,2,2,2,2,2,2,1,1,1},//16
                {1,1,1,2,1,1,1,2,1,2,2,2,2,2,2,1,1,2,1,2,1,2,2,2,2,2,2,2,2,1,1,1,2,2,2,1,1,2,1,1,2,1,2,2,1,1,1,1},//17
                {1,1,2,2,2,1,1,2,1,2,1,1,2,1,1,1,1,2,2,2,1,1,2,1,1,1,1,1,2,2,2,1,1,2,2,1,1,2,1,1,2,1,2,2,2,1,1,1},//18
                {1,1,2,1,2,1,1,2,2,2,1,1,2,1,1,1,1,1,1,2,2,2,2,2,2,1,1,2,2,1,2,1,1,2,2,1,1,2,2,1,2,1,1,2,2,1,1,1},//19
                {1,1,2,2,2,1,1,2,1,1,1,2,2,2,2,1,1,1,1,2,1,1,2,1,2,1,1,2,2,2,2,1,1,2,2,1,1,1,2,2,2,2,1,1,2,1,1,1},//20
                {1,1,1,2,2,1,1,2,1,1,1,2,2,2,2,1,1,1,1,2,2,2,2,1,2,1,1,2,1,1,1,1,1,2,2,1,2,2,2,1,1,2,1,1,2,1,1,1},//21
                {1,1,1,1,2,2,2,2,1,1,1,2,2,2,2,1,1,1,1,1,1,2,1,1,2,1,1,2,1,1,1,1,1,2,2,2,2,4,2,1,1,2,1,1,2,1,1,1},//22
                {1,1,1,1,1,1,1,2,1,1,2,2,2,2,2,1,1,1,1,1,1,2,2,1,2,1,2,2,1,1,1,1,1,2,2,2,2,2,2,1,1,2,1,2,2,1,1,1},//23
                {1,1,1,1,1,1,1,2,1,1,2,2,2,1,1,1,1,1,1,1,1,1,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,2,1,1,2,2,2,2,1,1,1},//24
                {1,1,1,1,1,1,1,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,1,1,1,1,1,1},//25
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//26
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//27 should be all ones

            }, 160);


            for (int i = 0; i < RNG.Next(MinEnemies, MaxEnemies); i++)
            {
                Enemies.Add(new Enemy(RNG, Tutorial.Width, Tutorial.Height, Tutorial.CollisionTiles));

            }

            for (int i = 0; i < 1; i++)
            {
                Intellegence.Add(new Intel(Content.Load<Texture2D>("intel"), Content.Load<Texture2D>("intel_Collect"), new Vector2(RNG.Next(160, Tutorial.Width - 160), RNG.Next(160, Tutorial.Height - 160)), RNG, Tutorial.CollisionTiles, new Vector2(Tutorial.Width, Tutorial.Height)));
            }
            for (int i = 0; i < 1; i++)
            {
                Ammunition.Add(new Ammo(Content.Load<Texture2D>("Bullet-Ground"), new Vector2(RNG.Next(160, Tutorial.Width - 160), RNG.Next(160, Tutorial.Height - 160)), RNG, Tutorial.CollisionTiles, new Vector2(Tutorial.Width, Tutorial.Height)));
            }
            LevelExit = new Exit(Content.Load<Texture2D>("hatch-door"), new Vector2(0, 0), RNG, Tutorial.CollisionTiles, new Vector2(0, 0));

            player = new Player(4, Tutorial.CollisionTiles, Ammunition, Intellegence, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);


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
                    if (CurrPad.Buttons.A == ButtonState.Pressed && Oldpad.Buttons.A == ButtonState.Released)
                    {
                        if (Main.ReturnSelection() == 0)
                            CurrentState = GameStates.Controls;
                        else if (Main.ReturnSelection() == 1)
                            CurrentState = GameStates.Instructions;
                        else if (Main.ReturnSelection() == 2)
                            Exit();
                    }
                    break;

                case GameStates.Instructions:
                    Instructions.Update(gameTime);
                    if (Instructions.Seconds > 5 && CurrPad.Buttons.A == ButtonState.Pressed && Oldpad.Buttons.A == ButtonState.Released)
                    {
                        CurrentState = GameStates.Controls;
                        Instructions.Seconds = 0;
                    }
                    break;

                case GameStates.Controls:
                    Controls.Update(gameTime);
                    if (Controls.Seconds > 5 && CurrPad.Buttons.A == ButtonState.Pressed && Oldpad.Buttons.A == ButtonState.Released)
                    {
                        CurrentState = GameStates.Playing;
                        Controls.Seconds = 0;
                    }

                    break;

                case GameStates.Playing:

                    playing(gameTime);
                    GameOver();


                    break;

                case GameStates.Pause:
                    //Resumes the game
                    if (CurrPad.Buttons.Start == ButtonState.Pressed && Oldpad.Buttons.Start == ButtonState.Released)
                    {
                        CurrentState = GameStates.Playing;
                    }
                    break;
                case GameStates.DisplayIntel:
                    IntelDisplay(gameTime);
                    break;


                case GameStates.Reset:
                    switch (CurrentLevels)
                    {
                        case Levels.Level0:
                            SwitchMap(Level1, 40, 45);
                            CurrentLevels = Levels.Level1;
                            CurrentState = GameStates.Playing; //always at the end so it goes back to playing

                            break;
                        case Levels.Level1:
                            SwitchMap(Level2, 45, 50);
                            CurrentLevels = Levels.Level2;
                            CurrentState = GameStates.Playing; //always at the end so it goes back to playing
                            break;
                        case Levels.Level2:
                            SwitchMap(Level3, 50, 60);
                            CurrentLevels = Levels.Level3;
                            CurrentState = GameStates.Playing; //always at the end so it goes back to playing
                            break;
                    }
                    break;
                case GameStates.Detected:
                    InstanceAlarm.Play();

                    CountDownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Countdown += (int)CountDownTimer;
                    if (CountDownTimer <= -1f)
                    {
                        CountDownTimer = 0f;
                    }

                    if (Countdown < 0)
                    {
                        CurrentState = GameStates.GameOver;
                    }
                    break;
                case GameStates.GameOver:
                    GameOverScreen.Update(gameTime);
                    if (GameOverScreen.Seconds > 5 && CurrPad.Buttons.A == ButtonState.Pressed && Oldpad.Buttons.A == ButtonState.Released)
                    {
                        Exit();
                    }
                    break;

                case GameStates.Completed:
                    Completed.Update(gameTime);
                    if (Completed.Seconds > 5 && CurrPad.Buttons.A == ButtonState.Pressed && Oldpad.Buttons.A == ButtonState.Released)
                    {
                        Exit();
                    }


                    break;
            }


            Oldpad = CurrPad;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(); //this is specfically for drawing the menu stuff
            switch (CurrentState)
            {
                case GameStates.Start:

                    Main.Draw();
                    break;
                case GameStates.Controls:
                    Controls.Draw(spriteBatch);
                    break;
                case GameStates.Instructions:
                    Instructions.Draw(spriteBatch);
                    break;
                case GameStates.DisplayIntel:

                    DisplayIntel[(int)CurrentLevels].Draw(spriteBatch);

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

                    PlayingDraw(gameTime);


                    break;
                case GameStates.Pause: // Keeps eveything drawing whilst not updating
                    PlayingDraw(gameTime);
                    break;

                case GameStates.Detected:
                    PlayingDraw(gameTime);
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
                    player.Hud_Draw(spriteBatch, LevelExit);
                    break;

                case GameStates.Pause:
                    player.Hud_Draw(spriteBatch, LevelExit);
                    Pausing.Draw();
                    break;

                case GameStates.Detected:
                    player.Hud_Draw(spriteBatch, LevelExit);
                    spriteBatch.Draw(Content.Load<Texture2D>("pixel"), new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.Red * 0.5f);
                    break;
                case GameStates.GameOver:

                    GameOverScreen.Draw(spriteBatch);

                    break;

                case GameStates.Completed:
                    Completed.Draw(spriteBatch);
                    break;
            }
            spriteBatch.End();


            base.Draw(gameTime);
        }
        void SwitchMap(Map Current, int min, int max)
        {
            Enemies.Clear();
            Intellegence.Clear();
            Ammunition.Clear();
            MinEnemies = min;
            MaxEnemies = max;
            for (int E = 0; E < RNG.Next(MinEnemies, MaxEnemies); E++)
            {
                Enemies.Add(new Enemy(RNG, Current.Width, Current.Height, Current.CollisionTiles));
            }
            for (int i = 0; i < 6; i++)
            {
                Intellegence.Add(new Intel(Content.Load<Texture2D>("intel"), Content.Load<Texture2D>("intel_Collect"), new Vector2(RNG.Next(160, Current.Width - 160), RNG.Next(160, Current.Height - 160)), RNG, Current.CollisionTiles, new Vector2(Current.Width, Current.Height)));
            }
            for (int A = 0; A < RNG.Next(MinEnemies, MaxEnemies); A++)
            {
                Ammunition.Add(new Ammo(Content.Load<Texture2D>("Bullet-Ground"), new Vector2(RNG.Next(160, Current.Width - 160), RNG.Next(160, Current.Height - 160)), RNG, Current.CollisionTiles, new Vector2(Current.Width, Current.Height)));
            }

            player = new Player(4, Current.CollisionTiles, Ammunition, Intellegence, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            LevelExit = new Exit(Content.Load<Texture2D>("hatch-door"), new Vector2(0, 0), RNG, Current.CollisionTiles, new Vector2(0, 0));
        }
        void IntelDisplay(GameTime gameTime)
        {
            DisplayIntel[(int)CurrentLevels].Update(gameTime);
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
                        CurrentState = GameStates.Completed;
                    }
                    break;
            }


        }

        void GameOver()
        {
            if (player.DetectionMeter >= 1)
            {
                CurrentState = GameStates.Detected;
            }
        }

        void playing(GameTime gameTime)
        {
            if(Enemies.Count < MaxEnemies)
            {
                RespawnCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                Respawn += (int)CountDownTimer;
                if (RespawnCounter <= -1f)
                {
                    RespawnCounter = 0f;
                }
                if(Respawn <= 0)
                {
                    Enemies.Add(new Enemy(RNG, Tutorial.Width, Tutorial.Height, Tutorial.CollisionTiles));
                    Respawn = 5;
                }

            }
            

            LevelExit.Update();

            player.Update(CurrPad, Enemies, CurrentState, LevelExit,gameTime);

            if (player.CollectionRange.Intersects(LevelExit.CollisionRect) && CurrPad.Buttons.A == ButtonState.Pressed && Oldpad.Buttons.A == ButtonState.Released && player.IntelCollected == Intellegence.Count)
            {
                CurrentState = GameStates.DisplayIntel;
            }

            foreach (Enemy enemy in Enemies)
            {
                enemy.Update(gameTime, Intellegence);
            }
            foreach (Intel intel in Intellegence)
            {
                intel.Update(gameTime);
            }
            foreach (Ammo ammo in Ammunition)
            {
                ammo.Update(gameTime);
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
        }
        void PlayingDraw(GameTime gameTime)
        {
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


            foreach (Intel intel in Intellegence)
            {
                intel.Draw(spriteBatch);
            }
            foreach (Ammo ammo in Ammunition)
            {
                ammo.Draw(spriteBatch);
            }
            LevelExit.Draw(spriteBatch);

            foreach (Enemy enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }
            player.Draw(spriteBatch, debugpixel, gameTime);
        }
    }
}
