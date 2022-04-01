using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Team_Majx_Game
{
    // Enums control the Game State
    enum GameState
    {
        Menu,
        Rules,
        Settings,
        CharSelect,
        Battle,
        Pause,
        EndScreen
    }
    public class Game1 : Game
    {
        // monogame necessities
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // enum
        private GameState currentState;

        // imput objects
        private KeyboardState prevkbState;
        private MouseState prevMsState;
        private List<Rectangle> buttonList;

        // font
        private SpriteFont font;
        
        // textures
        private Texture2D hitbox;
        private Texture2D knight;
        private Texture2D heart;

        // knight objects
        private Knight player1;
        private Knight player2;
        private HurtBox player1HurtBox;
        private int width;
        private int height;
        private SpriteFont medievalFont;
        private Texture2D tempSquare;
        private List<Button> buttons = new List<Button>();

        // controls the game
        private bool player1Alive = true;
        private bool player2Alive = true;

        private int p2StockCt;

        private string levelFile = "Level1.txt";

        //Temporary game manager class for the first demo
        private GameManager manager1;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            width = 1440;
            height = 810;
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();
            // TODO: Add your initialization logic here
            prevkbState = Keyboard.GetState();
            prevMsState = Mouse.GetState();
            buttonList = new List<Rectangle>();
            //Each button seperated by 240
            //Buttons split up by 3
            buttonList.Add(new Rectangle(260, 600, 200, 75));
            buttonList.Add(new Rectangle(620, 600, 200, 75));
            buttonList.Add(new Rectangle(980, 600, 200, 75));
            //Buttons split up by 2
            buttonList.Add(new Rectangle(500, 600, 200, 75));
            buttonList.Add(new Rectangle(720, 600, 200, 75));

            //Creating a temporary knight for the purpose of the first demo
            player1HurtBox = new HurtBox(new Rectangle(720, 405, 100, 100));
            manager1 = new GameManager();
            manager1.ReadLevelFile(levelFile);

            player1 = new Knight(knight, //texture
                manager1.SpawnPoints[0].Position.X, // x starting position
                manager1.SpawnPoints[0].Position.Y - 50, // y starting position
                100, // size
                100,  // size
                true,
                manager1, // reference
                player1HurtBox);

            player2 = new Knight(knight, //texture
                manager1.SpawnPoints[1].Position.X, // x starting position
                manager1.SpawnPoints[1].Position.Y - 50, // y starting position
                100, // size
                100,  // size
                false,
                manager1, // reference
                player1HurtBox);

            base.Initialize();
        }

        // Load all of the Textures and all of the fonts
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            hitbox = Content.Load<Texture2D>("hitbox");
            knight = Content.Load<Texture2D>("knight1");
            heart = Content.Load<Texture2D>("heart");
            tempSquare = Content.Load<Texture2D>("red square");
            font = Content.Load<SpriteFont>("arial");
            medievalFont = Content.Load<SpriteFont>("dutchMediaeval");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState kbState = Keyboard.GetState();
            MouseState msState = Mouse.GetState();

            // FSM for what controls will go on in each state
            switch (currentState)
            {
                case GameState.Menu:
                    if (ClickButton(buttonList[0], msState))
                    {
                        currentState = GameState.Rules;
                    }
                    else if (ClickButton(buttonList[1], msState))
                    {
                        currentState = GameState.CharSelect;
                    }
                    else if (ClickButton(buttonList[2], msState))
                    {
                        currentState = GameState.Settings;
                    }
                    break;

                case GameState.Rules:
                    if (ClickButton(buttonList[1], msState))
                    {
                        currentState = GameState.Menu;
                    }
                    break;

                case GameState.Settings:
                    if (ClickButton(buttonList[1], msState))
                    {
                        currentState = GameState.Menu;
                    }
                    break;

                case GameState.CharSelect:
                    if (ClickButton(buttonList[0], msState))
                    {
                        currentState = GameState.Menu;
                    }
                    else if (ClickButton(buttonList[1], msState))
                    {
                        currentState = GameState.Battle;
                    }
                    else if (ClickButton(buttonList[2], msState))
                    {
                        currentState = GameState.Settings;
                    }
                    break;

                case GameState.Battle:
                    if (SingleKeyPress(Keys.Q, kbState))
                    {
                        currentState = GameState.Pause;
                    }
                    else if (SingleKeyPress(Keys.Escape, kbState))
                    {
                        currentState = GameState.EndScreen;
                    }

                    // If a player dies, switch to the end screen
                    if(!player2Alive || !player1Alive)
                    {
                        currentState = GameState.EndScreen;
                    }
                    break;

                case GameState.Pause:
                    if (ClickButton(buttonList[3], msState))
                    {
                        currentState = GameState.Battle;
                    }
                    else if (ClickButton(buttonList[4], msState))
                    {
                        currentState = GameState.Menu;
                    }
                    break;

                case GameState.EndScreen:
                    if (ClickButton(buttonList[3], msState))
                    {
                        currentState = GameState.CharSelect;
                    }
                    else if (ClickButton(buttonList[4], msState))
                    {
                        currentState = GameState.Menu;
                    }
                    break;
            }

            prevkbState = kbState;
            prevMsState = msState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            ShapeBatch.Begin(GraphicsDevice);

            // FSM to control what will be
            // drawn to the screen at a specific state
            switch (currentState)
            {
                // draws all of the menu items
                case GameState.Menu:
                    ShapeBatch.Box(buttonList[0], Color.PapayaWhip);
                    ShapeBatch.Box(buttonList[1], Color.PapayaWhip);
                    ShapeBatch.Box(buttonList[2], Color.PapayaWhip);
                    _spriteBatch.DrawString(medievalFont, "Gameplay demo", new Vector2(width/2, height/2), Color.Black);
                    break;

                // draws all of the rules items
                case GameState.Rules:
                    _spriteBatch.DrawString(medievalFont, "Rules", new Vector2(width / 2, height / 2), Color.Black);
                    ShapeBatch.Box(buttonList[1], Color.PapayaWhip);
                    break;

                // draws all of the items needed in settings
                case GameState.Settings:
                    _spriteBatch.DrawString(medievalFont, "Settings", new Vector2(width / 2, height / 2), Color.Black);
                    ShapeBatch.Box(buttonList[1], Color.PapayaWhip);
                    break;

                // draws all of the character select items
                case GameState.CharSelect:
                    ShapeBatch.Box(buttonList[0], Color.PapayaWhip);
                    ShapeBatch.Box(buttonList[1], Color.PapayaWhip);
                    ShapeBatch.Box(buttonList[2], Color.PapayaWhip);
                    _spriteBatch.DrawString(medievalFont, "Character Select", new Vector2(width / 2, height / 2), Color.Black);
                    break;

                // draws everything in the battle scene
                case GameState.Battle:
                    player1.update(gameTime, Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.P, Keys.O, Keys.I, Keys.L);
                    player1.Draw(_spriteBatch, knight, hitbox);


                    player2.update(gameTime, Keys.W, Keys.S, Keys.A, Keys.D, Keys.Y, Keys.T, Keys.R, Keys.G);
                    player2.Draw(_spriteBatch, knight, hitbox);
                    // _spriteBatch.Draw(hitbox, new Rectangle(720, 505, 400, 100), Color.White);


                    // Draws the map
                    for (int r = 0; r < manager1.MapArray.GetLength(0); r++)
                    {
                        for(int c = 0; c < manager1.MapArray.GetLength(1); c++)
                        {
                            manager1.MapArray[r,c].Draw(_spriteBatch, tempSquare);
                        }
                    }

                    // Draws the words "Player 1: 
                    _spriteBatch.DrawString(medievalFont, "Player 1:", new Vector2(40, 38), Color.Black);

                    // Draws player 1 hearts
                    for (int i = 0; i < player1.Stocks; i++)
                    {
                        _spriteBatch.Draw(heart, // texture
                            new Rectangle( // new rectangle
                            (150) + (35 * i),
                            40,
                            32,
                            32),
                            Color.White);
                    }

                    // Draws the words "Player 2: 
                    _spriteBatch.DrawString(medievalFont,
                        "Player 2:",
                        new Vector2((_graphics.PreferredBackBufferWidth - (35 * player2.Stocks) - 150), 38),
                        Color.Black);


                    // Drawing the hearts/ stock for player 2
                    for (int i = 0; i < player1.Stocks; i++)
                    {
                        _spriteBatch.Draw(heart, // texture
                            new Rectangle( // new rectangle
                            // puts every heart to the right of the previous.
                            (_graphics.PreferredBackBufferWidth - 35*player1.Stocks - 40) + (35*i), 
                            40,
                            32,
                            32),
                            Color.White);
                    }

                    break;

                case GameState.Pause:
                    ShapeBatch.Box(buttonList[3], Color.PapayaWhip);
                    ShapeBatch.Box(buttonList[4], Color.PapayaWhip);
                    _spriteBatch.DrawString(medievalFont, "Game Paused", new Vector2(width / 2, height / 2), Color.Black);
                    break;

                case GameState.EndScreen:
                    ShapeBatch.Box(buttonList[3], Color.PapayaWhip);
                    ShapeBatch.Box(buttonList[4], Color.PapayaWhip);
                    _spriteBatch.DrawString(medievalFont, "Game End", new Vector2(width / 2, height / 2), Color.Black);

                    if (!player2Alive)
                    {
                        // Implement the player 1 win
                    }
                    else
                    {
                        // Implement the player 2 win
                    }
                    break;
            }

            ShapeBatch.End(); // ends shapebatch
            _spriteBatch.End(); // ends spritebatch
            base.Draw(gameTime);
        }


        // method for controling a key press
        public bool SingleKeyPress(Keys key, KeyboardState kbState)
        {
            if (kbState.IsKeyUp(key) && prevkbState.IsKeyDown(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // method controls a single mouse button click
        public bool SingleMousePress(MouseState mState)
        {
            if (mState.LeftButton == ButtonState.Released && prevMsState.LeftButton == ButtonState.Pressed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // controls the button clicks in monogame
        public bool ClickButton(Rectangle button, MouseState mState)
        {
            if (SingleMousePress(mState) && button.Contains(mState.X, mState.Y))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
