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

        // font and backgrounds
        private SpriteFont font;
        private SpriteFont medievalFont;
        private SpriteFont bigMedievalFont;
        private Texture2D castleBackground;
        private Rectangle backgroundPosition;
        
        // textures
        private Texture2D hitbox;
        private Texture2D knight;
        private Texture2D heart;

        // knight objects
        private Knight player1;
        private Knight player2;
        private HurtBox Player1HurtBox;
        private HurtBox Player2HurtBox;
        private int width;
        private int height;
        private Texture2D tempSquare;
        private Texture2D explosion;

        // buttons
        private List<Button> buttonList;

        // holds all the possible levels
        private List<string> levelList;

        private int p2StockCt;

        private string currentLevel;

        private string levelFile = "Level1.txt";

        //Temporary game manager class for the first demo
        private GameManager manager1;

        // property so the cc class can get texturd
        public Texture2D Explosion
        {
            get { return explosion; }
        }

        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            levelList = new List<string>() {"Level1", "Level2", "Level3"};
            currentLevel = levelList[1];
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            manager1 = new GameManager();
            manager1.ReadLevelFile("Leve1");
            width = manager1.ScreenWidth;
            height = manager1.ScreenHeight - 20;
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();
            // TODO: Add your initialization logic here
            prevkbState = Keyboard.GetState();
            prevMsState = Mouse.GetState();

            //buttonList = new List<Rectangle>();
            buttonList = new List<Button>();

            //Each button seperated by 240
            //Buttons split up by 3 (and 1 for the middle)
            buttonList.Add(new Button(new Rectangle(260, 600, 200, 75)));
            buttonList.Add(new Button(new Rectangle(620, 600, 200, 75)));
            buttonList.Add(new Button(new Rectangle(980, 600, 200, 75)));
            //Buttons split up by 2
            buttonList.Add(new Button(new Rectangle(466, 600, 200, 75)));
            buttonList.Add(new Button(new Rectangle(774, 600, 200, 75)));

            //Creating a temporary knight for the purpose of the first demo
           
            manager1 = new GameManager();
            manager1.ReadLevelFile(levelFile);

            Player1HurtBox = new HurtBox(new Rectangle(manager1.SpawnPoints[0].Position.X, manager1.SpawnPoints[0].Position.Y, 80, 80));
            Player2HurtBox = new HurtBox(new Rectangle(manager1.SpawnPoints[1].Position.X, manager1.SpawnPoints[1].Position.Y, 80, 80));

            player1 = new Knight(knight, //texture
                manager1.SpawnPoints[0].Position.X, // x starting position
                manager1.SpawnPoints[0].Position.Y - 50, // y starting position
                80, // size
                80,  // size
                true,
                manager1, // reference
                Player1HurtBox);

            player2 = new Knight(knight, //texture
                manager1.SpawnPoints[1].Position.X, // x starting position
                manager1.SpawnPoints[1].Position.Y - 50, // y starting position
                80, // size
                80,  // size
                false,
                manager1, // reference
                Player2HurtBox);

            

            //Background variables
            backgroundPosition = new Rectangle(0, 0, width, height);

            base.Initialize();
        }

        // Load all of the Textures and all of the fonts
        protected override void LoadContent()
        {
            //Textures
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            hitbox = Content.Load<Texture2D>("hitbox");
            knight = Content.Load<Texture2D>("knight1");
            heart = Content.Load<Texture2D>("heart");
            tempSquare = Content.Load<Texture2D>("red square");
            castleBackground = Content.Load<Texture2D>("castle");

            //Fonts
            font = Content.Load<SpriteFont>("arial");
            medievalFont = Content.Load<SpriteFont>("dutchMediaeval");
            bigMedievalFont = Content.Load<SpriteFont>("bigDutchMediaeval");
            explosion = Content.Load<Texture2D>("explosion");
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
                    if (buttonList[0].ClickButton(msState, prevMsState))
                    {
                        currentState = GameState.Rules;
                    }
                    else if (buttonList[1].ClickButton(msState, prevMsState))
                    {
                        currentState = GameState.Battle;
                    }
                    else if (buttonList[2].ClickButton(msState, prevMsState))
                    {
                        currentState = GameState.Settings;
                    }
                    break;

                case GameState.Rules:
                    if (buttonList[1].ClickButton(msState, prevMsState))
                    {
                        currentState = GameState.Menu;
                    }
                    break;

                case GameState.Settings:
                    if (buttonList[1].ClickButton(msState, prevMsState))
                    {
                        currentState = GameState.Menu;
                    }
                    break;

                    /*
                case GameState.CharSelect:
                    if (buttonList[0].ClickButton(msState, prevMsState))
                    {
                        currentState = GameState.Menu;
                    }
                    else if (buttonList[1].ClickButton(msState, prevMsState))
                    {
                        currentState = GameState.Battle;
                    }
                    else if (buttonList[2].ClickButton(msState, prevMsState))
                    {
                        currentState = GameState.Settings;
                    }
                    break;
                    */

                case GameState.Battle:
                    if (SingleKeyPress(Keys.Q, kbState))
                    {
                        currentState = GameState.Pause;
                    }
                    else if (SingleKeyPress(Keys.Enter, kbState))
                    {
                        currentState = GameState.EndScreen;
                    }

                    // If a player dies, switch to the end screen
                    if(!player1.PlayerAlive || !player2.PlayerAlive)
                    {
                        currentState = GameState.EndScreen;
                    }
                    
                    break;

                case GameState.Pause:
                    if (buttonList[3].ClickButton(msState, prevMsState))
                    {
                        currentState = GameState.Battle;
                    }
                    else if (buttonList[4].ClickButton(msState, prevMsState))
                    {
                        currentState = GameState.Menu;
                        ResetPlayers();
                    }
                    break;

                case GameState.EndScreen:
                    if (buttonList[1].ClickButton(msState, prevMsState))
                    {
                        currentState = GameState.Menu;
                        ResetPlayers();
                    }
                    break;
            }

            prevkbState = kbState;
            prevMsState = msState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Tan);

            _spriteBatch.Begin();
            ShapeBatch.Begin(GraphicsDevice);

            // FSM to control what will be
            // drawn to the screen at a specific state
            switch (currentState)
            {
                // draws all of the menu items
                case GameState.Menu:
                    //Background image
                    //_spriteBatch.Draw(castleBackground, backgroundPosition, Color.White);
                    
                    //Draws all the Buttons for the menu
                    ShapeBatch.Box(buttonList[0].Postion, Color.SlateGray);
                    ShapeBatch.Box(buttonList[1].Postion, Color.SlateGray);
                    ShapeBatch.Box(buttonList[2].Postion, Color.SlateGray);

                    //Draws the menu text and button text
                    _spriteBatch.DrawString(bigMedievalFont, "Medieval Kombat", new Vector2
                        ((width/2) - (bigMedievalFont.MeasureString("Medieval Kombat").X/2), 250), Color.Black);
                    buttonList[0].Draw(_spriteBatch, "Rules", medievalFont);
                    buttonList[1].Draw(_spriteBatch, "To Battle!", medievalFont);
                    buttonList[2].Draw(_spriteBatch, "Settings", medievalFont);
                    break;

                // draws all of the rules items
                case GameState.Rules:
                    //Draws the rules menu and buttons
                    _spriteBatch.DrawString(bigMedievalFont, "Rules", new Vector2
                        ((width / 2) - (bigMedievalFont.MeasureString("Rules").X/2), 50), Color.Black);

                    //Draws all of the controls
                    _spriteBatch.DrawString(medievalFont, "Be the first knight to bring your opponents health to 0! Try not to fall off!", 
                        CenterFont("Be the first knight to bring your opponents health to 0! Try not to fall off!",
                        medievalFont, 125), Color.Black);
                    _spriteBatch.DrawString(medievalFont, "Pause - Q", CenterFont("Pause - Q",
                        medievalFont, 160), Color.Black);
                    _spriteBatch.DrawString(medievalFont, "Player 1:", CenterFont("Player 1:",
                        medievalFont, 200), Color.Black);
                    _spriteBatch.DrawString(medievalFont, "Move - Arrow Keys", CenterFont("Move - Arrow Keys",
                        medievalFont, 230), Color.Black);
                    _spriteBatch.DrawString(medievalFont, "Jab - P", CenterFont("Jab - P",
                        medievalFont, 260), Color.Black);
                 //   _spriteBatch.DrawString(medievalFont, "Special - O", CenterFont("Special - O",
                 //       medievalFont, 290), Color.Black);
                 //   _spriteBatch.DrawString(medievalFont, "Strong Attack - I", CenterFont("Strong Attack - I",
                 //       medievalFont, 320), Color.Black);
                    _spriteBatch.DrawString(medievalFont, "Dodge - L", CenterFont("Dodge - L",
                        medievalFont, 290), Color.Black); //CHANGE TO 350 
                       _spriteBatch.DrawString(medievalFont, "Jump - Up arrow", CenterFont("Jump - Up arrow",
                        medievalFont, 320), Color.Black);

                    _spriteBatch.DrawString(medievalFont, "Player 2:", CenterFont("Player 2:",
                        medievalFont, 390), Color.Black);
                    _spriteBatch.DrawString(medievalFont, "Move - WASD", CenterFont("Move - WASD",
                        medievalFont, 420), Color.Black);
                    _spriteBatch.DrawString(medievalFont, "Jab - Y", CenterFont("Jab - Y",
                        medievalFont, 450), Color.Black);
                 //   _spriteBatch.DrawString(medievalFont, "Special - T", CenterFont("Special - T",
                 //       medievalFont, 480), Color.Black);
                 //   _spriteBatch.DrawString(medievalFont, "Strong Attack - R", CenterFont("Strong Attack - R",
                  //      medievalFont, 510), Color.Black);
                    _spriteBatch.DrawString(medievalFont, "Dodge - G", CenterFont("Dodge - G",
                        medievalFont, 480), Color.Black); // CHANGE TO 540
                    _spriteBatch.DrawString(medievalFont, "Jump - W", CenterFont("Jump - W",
                        medievalFont, 510), Color.Black);

                    ShapeBatch.Box(buttonList[1].Postion, Color.SlateGray);
                    buttonList[1].Draw(_spriteBatch, "Back", medievalFont);
                    break;

                // draws all of the items needed in settings
                case GameState.Settings:
                    _spriteBatch.DrawString(bigMedievalFont, "Settings", new Vector2
                        ((width / 2) - (bigMedievalFont.MeasureString("Settings").X/2), 50), Color.Black);
                    _spriteBatch.DrawString(medievalFont, "To Be Implemented!", CenterFont("To Be Implemented!",
                        medievalFont, 300), Color.Black);
                    ShapeBatch.Box(buttonList[1].Postion, Color.SlateGray);
                    buttonList[1].Draw(_spriteBatch, "Back", medievalFont);
                    break;

                    /*
                // draws all of the character select items
                case GameState.CharSelect:
                    //Draws all the character select buttons
                    ShapeBatch.Box(buttonList[0].Postion, Color.PapayaWhip);
                    ShapeBatch.Box(buttonList[1].Postion, Color.PapayaWhip);
                    ShapeBatch.Box(buttonList[2].Postion, Color.PapayaWhip);

                    //Draws the character select menu and button text
                    _spriteBatch.DrawString(bigMedievalFont, "Character Select", new Vector2(width / 2, height / 2), Color.Black);
                    buttonList[0].Draw(_spriteBatch, "Menu", medievalFont);
                    buttonList[1].Draw(_spriteBatch, "To Battle!", medievalFont);
                    buttonList[2].Draw(_spriteBatch, "Settings", medievalFont);
                    break;
                    */

                // draws everything in the battle scene
                case GameState.Battle:
                    GraphicsDevice.Clear(Color.Tan);
                    player1.update(gameTime, Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.P, Keys.O, Keys.I, Keys.L, _spriteBatch);
                    player1.Draw(_spriteBatch, knight, hitbox, explosion);
                    player1.DealDamage(player2);


                    player2.update(gameTime, Keys.W, Keys.S, Keys.A, Keys.D, Keys.Y, Keys.T, Keys.R, Keys.G, _spriteBatch);
                    player2.Draw(_spriteBatch, knight, hitbox, explosion);
                    player2.DealDamage(player1);
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
                    _spriteBatch.DrawString(medievalFont, "Player 1:", new Vector2(440, 700), Color.Black);

                    // Draws player 1 hearts
                    for (int i = 0; i < player1.Stocks; i++)
                    {
                        _spriteBatch.Draw(heart, // texture
                            new Rectangle( // new rectangle
                            (550) + (35 * i),
                            702,
                            32,
                            32),
                            Color.White);
                    }

                    _spriteBatch.DrawString(bigMedievalFont, player1.Health.ToString(), new Vector2(440, 725), Color.Black);

                    _spriteBatch.DrawString(bigMedievalFont,
                       player2.Health.ToString(),
                       new Vector2((_graphics.PreferredBackBufferWidth - (35 * player2.Stocks) - 550), 725),
                       Color.Black);


                    // Draws the words "Player 2: 
                    _spriteBatch.DrawString(medievalFont,
                        "Player 2:",
                        new Vector2((_graphics.PreferredBackBufferWidth - (35 * player2.Stocks) - 550), 700),
                        Color.Black);

                    // Drawing the hearts/ stock for player 2
                    for (int i = 0; i < player2.Stocks; i++)
                    {
                        _spriteBatch.Draw(heart, // texture
                            new Rectangle( // new rectangle
                            // puts every heart to the right of the previous.
                            ((_graphics.PreferredBackBufferWidth - 35*player2.Stocks - 40) + (35*i)) - 400, 
                            702,
                            32,
                            32),
                            Color.White);
                    }



                    //Draws the player 1 state for testing
                    //_spriteBatch.DrawString(medievalFont, player1.ToString(), new Vector2(60, 70), Color.Black);

                    break;

                case GameState.Pause:
                    //Draws the Pause buttons
                    ShapeBatch.Box(buttonList[3].Postion, Color.SlateGray);
                    ShapeBatch.Box(buttonList[4].Postion, Color.SlateGray);

                    //Draws the Pause menu and button text
                    _spriteBatch.DrawString(bigMedievalFont, "Game Paused", new Vector2
                        ((width / 2) - (bigMedievalFont.MeasureString("Game Paused").X/2), 250), Color.Black);
                    buttonList[3].Draw(_spriteBatch, "Unpause", medievalFont);
                    buttonList[4].Draw(_spriteBatch, "Menu", medievalFont);
                    break;

                case GameState.EndScreen:
                    //Draws the End Screen buttons
                    ShapeBatch.Box(buttonList[1].Postion, Color.SlateGray);

                    //Draws the End Screen menu and button text
                    _spriteBatch.DrawString(bigMedievalFont, "Game End", new Vector2
                        ((width / 2) - (bigMedievalFont.MeasureString("Game End").X/2), 150), Color.Black);
                    buttonList[1].Draw(_spriteBatch, "Menu", medievalFont);

                    if (!player2.PlayerAlive)
                    {
                        _spriteBatch.DrawString(medievalFont,
                            "Player 1 Wins!",
                            CenterFont("Player 1 Wins!", medievalFont, 300),
                            Color.Black);
                    }
                    else
                    {
                        _spriteBatch.DrawString(medievalFont,
                            "Player 2 Wins!",
                            CenterFont("Player 2 Wins!", medievalFont, 300),
                            Color.Black);
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

        // resets the players
        public void ResetPlayers()
        {
            // resets player 1
            player1.Stocks = manager1.Stocks;
            player1.Health = manager1.Health;
            player1.PlayerPositionX = manager1.SpawnPoints[0].Position.X;
            player1.PlayerPositionY = manager1.SpawnPoints[1].Position.Y - 40;
            player1.PlayerAlive = true;
            player1.XVelocity = 0;
            player1.YVelocity = 0;

            // resets player 2
            player2.Stocks = manager1.Stocks;
            player2.Health = manager1.Health;
            player2.PlayerPositionX = manager1.SpawnPoints[1].Position.X;
            player2.PlayerPositionY = manager1.SpawnPoints[1].Position.Y - 40;
            player2.PlayerAlive = true;
            player2.XVelocity = 0;
            player2.YVelocity = 0;
        }

        public Vector2 CenterFont(string text, SpriteFont currentFont, int textHeight)
        {
            return new Vector2((width / 2) - (currentFont.MeasureString(text).X / 2), textHeight);
        }
    }
}
