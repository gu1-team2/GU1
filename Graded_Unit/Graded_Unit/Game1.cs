using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

enum GameStates
{
    Start,
    Instructions,
    Playing,
    Pause,
    Reset,
}
enum Levels
{

    Level0, //tutorial level
    Level1,
    Level2,
    Level3,

    Switch_Level,
}

namespace Graded_Unit
{

    public class Game1 : Game
    {
        Texture2D debugpixel;

        GameStates CurrentState = GameStates.Start;
        Levels CurrentLevels = Levels.Level1;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Random RNG;
        GamePadState CurrPad, Oldpad;

        Player player;

        Map Tutorial, Level1, Level2, Level3;

        int timer;

        Main_Menu Menu;

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
            Player.Content = Content;

            Menu = new Main_Menu(spriteBatch,graphics);



            Tutorial.Generate(new int[,]
            {
                {1,1,1,1,1,1,1,1,1,1,1,1},//1
                {1,1,1,1,1,1,1,1,1,1,1,1},//2
                {1,1,1,1,1,1,1,1,1,1,1,1},//3
                {1,1,1,1,1,1,1,1,1,1,1,1},//4
                {1,1,1,1,1,1,1,1,1,1,1,1},//5
                {1,1,1,1,1,1,1,1,1,1,1,1},//6
                {1,1,1,1,1,1,1,1,1,1,1,1},//7
                {1,1,1,1,1,1,1,1,1,1,1,1},//8
                {1,1,1,1,1,1,1,1,1,1,1,1},//9
                {1,1,1,1,1,1,1,1,1,1,1,1},//10

            }, 160);


            Level1.Generate(new int[,]
            {
                // This is how the levels map will be generated. if the value is greater than 0 this will place a block
                
                //the far left and right of this also should be ones as well to create an edge around the outside

                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//1 should be all ones,       48 x 27 grid
                {1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//2
                {1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//3       2 sets the player start, put 2 back in when done
                {1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//4
                {1,1,1,0,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//5
                {1,1,1,0,1,1,1,0,0,0,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//6
                {1,1,1,0,1,1,1,0,0,0,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//7
                {1,1,1,0,1,1,1,0,0,0,0,0,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//8
                {1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//9
                {1,1,1,0,1,1,1,0,0,0,0,0,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//10
                {1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//11
                {1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//12
                {1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//13
                {1,1,1,0,1,0,1,1,1,1,0,0,0,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//14
                {1,1,1,1,1,0,1,1,1,1,0,0,0,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//15
                {1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//16
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//17
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//18
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//19
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//20
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//21
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//22
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//23
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//24
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//25
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//26
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},//27 should be all ones
                
            }, 160);


            player = new Player(3, Level1.CollisionTiles,graphics.PreferredBackBufferWidth,graphics.PreferredBackBufferHeight); // CHANGE THE LIST WHEN FINISHED 

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
                    Menu.Update();
                    break;

                case GameStates.Instructions:

                    break;

                case GameStates.Playing:
                    player.Update(CurrPad);
                    //Pauses the game
                    if (CurrPad.Buttons.Start == ButtonState.Pressed && Oldpad.Buttons.Start == ButtonState.Released)
                    {
                        CurrentState = GameStates.Pause;
                    }
                    break;

                case GameStates.Pause:
                    //Resumes the game
                    if (CurrPad.Buttons.Start == ButtonState.Pressed && Oldpad.Buttons.Start == ButtonState.Released)
                    {
                        CurrentState = GameStates.Playing;
                    }
                    break;



                case GameStates.Reset:
                    switch (CurrentLevels)
                    {
                        case Levels.Level0:
                            player.ResetPlayer(Level1.CollisionTiles);
                            CurrentLevels = Levels.Level1;
                            CurrentState = GameStates.Playing; //always at the end so it goes back to playing

                            break;
                        case Levels.Level1:
                            player.ResetPlayer(Level2.CollisionTiles);
                            CurrentLevels = Levels.Level2;
                            CurrentState = GameStates.Playing; //always at the end so it goes back to playing
                            break;
                        case Levels.Level2:
                            player.ResetPlayer(Level3.CollisionTiles);
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

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateTranslation(new Vector3(player.getPos().X, player.getPos().Y, 0)));

            switch (CurrentState)
            {
                case GameStates.Start:
                    Menu.Draw();
                    break;
                case GameStates.Instructions:

                    break;
                case GameStates.Playing:
                    Level1.Draw(spriteBatch);

                    player.Draw(spriteBatch, debugpixel);



                    break;
                case GameStates.Pause:

                    player.Draw(spriteBatch, debugpixel);
                    break;
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
