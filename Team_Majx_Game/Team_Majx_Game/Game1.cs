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
        Controls,
        MapSelect,
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
        private Texture2D map1Picture;
        private Rectangle map1PicBackground;
        private Texture2D map2Picture;
        private Rectangle map2PicBackground;
        private Texture2D map3Picture;
        private Rectangle map3PicBackground;
        private Texture2D mapSquare;
        
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

        // buttons and button color
        private List<Button> buttonList;
        private List<Button> mapButtonList;
        private Color deafultButtonColor;

        // holds all the possible levels
        private List<string> levelList;

        private int p2StockCt;

        private int currentLevel = 1;

        //Temporary game manager class for the first demo
        private GameManager manager1;

        //All needed keys in a list
        private List<Keys> player1Keys;
        private List<Keys> player2Keys;

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
            levelList = new List<string>() {"Level1.txt", "Level2.txt", "Level3.txt"};
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            manager1 = new GameManager();
            manager1.ReadLevelFile(levelList[0]);
            width = manager1.ScreenWidth;
            height = manager1.ScreenHeight - 20;
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();
            // TODO: Add your initialization logic here
            prevkbState = Keyboard.GetState();
            prevMsState = Mouse.GetState();

            //initializes the new button lists and their color
            buttonList = new List<Button>();
            mapButtonList = new List<Button>();
            deafultButtonColor = Color.SlateGray;

            //Each button seperated by 240
            //Buttons split up by 3 (and 1 for the middle)
            buttonList.Add(new Button(new Rectangle(260, 600, 200, 75), deafultButtonColor));
            buttonList.Add(new Button(new Rectangle(620, 600, 200, 75), deafultButtonColor));
            buttonList.Add(new Button(new Rectangle(980, 600, 200, 75), deafultButtonColor));
            //Buttons split up by 2
            buttonList.Add(new Button(new Rectangle(466, 600, 200, 75), deafultButtonColor));
            buttonList.Add(new Button(new Rectangle(774, 600, 200, 75), deafultButtonColor));

            //Map button list (higher than buttons above)
            mapButtonList.Add(new Button(new Rectangle(160, 450, 200, 75), deafultButtonColor));
            mapButtonList.Add(new Button(new Rectangle(620, 450, 200, 75), deafultButtonColor));
            mapButtonList.Add(new Button(new Rectangle(1080, 450, 200, 75), deafultButtonColor));

            //Creating a temporary knight for the purpose of the first demo

            Player1HurtBox = new HurtBox(new Rectangle(manager1.SpawnPoints[1].Position.X, manager1.SpawnPoints[1].Position.Y, 80, 80));
            Player2HurtBox = new HurtBox(new Rectangle(manager1.SpawnPoints[0].Position.X, manager1.SpawnPoints[0].Position.Y, 80, 80));

            player1 = new Knight(knight, //texture
                manager1.SpawnPoints[1].Position.X, // x starting position
                manager1.SpawnPoints[1].Position.Y + 20, // y starting position
                80, // size
                80,  // size
                false,
                manager1, // reference
                Player1HurtBox,
                Color.LightBlue);

            player2 = new Knight(knight, //texture
               manager1.SpawnPoints[0].Position.X, // x starting position
               manager1.SpawnPoints[0].Position.Y + 20, // y starting position
               80, // size
               80,  // size
               true,
               manager1, // reference
               Player2HurtBox,
               Color.Red);

            //Background variables
            backgroundPosition = new Rectangle(0, 0, width, height);
            map1PicBackground = new Rectangle(75, 190, 370, 223);
            map2PicBackground = new Rectangle(535, 190, 370, 223);
            map3PicBackground = new Rectangle(995, 190, 370, 223);

            //Initializes the list of keys (attack, special, dodge, strong)
            player1Keys = new List<Keys>();
            player1Keys.Add(Keys.P);
            player1Keys.Add(Keys.O);
            player1Keys.Add(Keys.I);
            player1Keys.Add(Keys.L);
            player2Keys = new List<Keys>();
            player2Keys.Add(Keys.Y);
            player2Keys.Add(Keys.T);
            player2Keys.Add(Keys.R);
            player2Keys.Add(Keys.G);

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
            map1Picture = Content.Load<Texture2D>("Map1Pic");
            map2Picture = Content.Load<Texture2D>("Map2Pic");
            map3Picture = Content.Load<Texture2D>("Map3Pic");
            mapSquare = Content.Load<Texture2D>("red square cropped");

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
                        currentState = GameState.Controls;
                    }
                    else if (buttonList[1].ClickButton(msState, prevMsState))
                    {
                        currentState = GameState.Battle;
                    }
                    else if (buttonList[2].ClickButton(msState, prevMsState))
                    {
                        currentState = GameState.MapSelect;
                    }
                    break;

                case GameState.Controls:
                    if (buttonList[1].ClickButton(msState, prevMsState))
                    {
                        currentState = GameState.Menu;
                    }
                    else if (buttonList[0].ClickButton(msState, prevMsState))
                    {
                        //Change Player 1's keybinds 
                    }
                    else if (buttonList[2].ClickButton(msState, prevMsState))
                    {
                        //Change Player 2's keybinds
                    }
                    break;

                case GameState.MapSelect:
                    if (buttonList[1].ClickButton(msState, prevMsState))
                    {
                        currentState = GameState.Menu;
                    }
                    else if (mapButtonList[0].ClickButton(msState, prevMsState))
                    {
                        // loads the first level file
                        manager1.ReadLevelFile(levelList[0]);

                        // sets the players position to the new location
                        player2.PlayerPositionX = manager1.SpawnPoints[0].Position.X;
                        player2.PlayerPositionY = manager1.SpawnPoints[0].Position.Y + 20;
                        player1.PlayerPositionX = manager1.SpawnPoints[1].Position.X;
                        player1.PlayerPositionY = manager1.SpawnPoints[1].Position.Y + 20;

                        currentLevel = 1;
                    }
                    else if (mapButtonList[1].ClickButton(msState, prevMsState))
                    {
                        manager1.ReadLevelFile(levelList[1]);
                        player2.PlayerPositionX = manager1.SpawnPoints[0].Position.X;
                        player2.PlayerPositionY = manager1.SpawnPoints[0].Position.Y + 20;
                        player1.PlayerPositionX = manager1.SpawnPoints[1].Position.X;
                        player1.PlayerPositionY = manager1.SpawnPoints[1].Position.Y + 20;
                        currentLevel = 2;
                    }
                    else if (mapButtonList[2].ClickButton(msState, prevMsState))
                    {
                        manager1.ReadLevelFile(levelList[2]);
                        player2.PlayerPositionX = manager1.SpawnPoints[0].Position.X;
                        player2.PlayerPositionY = manager1.SpawnPoints[0].Position.Y - 45;
                        player1.PlayerPositionX = manager1.SpawnPoints[1].Position.X - 15;
                        player1.PlayerPositionY = manager1.SpawnPoints[1].Position.Y - 45;
                        currentLevel = 3;
                    }
                    break;

                case GameState.Battle:
                    player1.update(gameTime, Keys.Up, Keys.Down, Keys.Left, Keys.Right, player1Keys[0], player1Keys[1],
                        player1Keys[2], player1Keys[3], _spriteBatch);
                    player1.DealDamage(player2);
                    player2.update(gameTime, Keys.W, Keys.S, Keys.A, Keys.D, player2Keys[0], player2Keys[1], 
                        player2Keys[2], player2Keys[3], _spriteBatch);
                    player2.DealDamage(player1);
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
                    ShapeBatch.Box(buttonList[0].Postion, buttonList[0].ButtonColor);
                    ShapeBatch.Box(buttonList[1].Postion, buttonList[1].ButtonColor);
                    ShapeBatch.Box(buttonList[2].Postion, buttonList[2].ButtonColor);

                    //Draws the menu text and button text
                    _spriteBatch.DrawString(bigMedievalFont, "Medieval Kombat", new Vector2
                        ((width/2) - (bigMedievalFont.MeasureString("Medieval Kombat").X/2), 250), Color.Black);
                    buttonList[0].Draw(_spriteBatch, "Controls", medievalFont);
                    buttonList[1].Draw(_spriteBatch, "To Battle!", medievalFont);
                    buttonList[2].Draw(_spriteBatch, "Map Select", medievalFont);
                    break;

                // draws all of the Controls items
                case GameState.Controls:
                    //Draws the Controls menu and buttons
                    _spriteBatch.DrawString(bigMedievalFont, "Controls", new Vector2
                        ((width / 2) - (bigMedievalFont.MeasureString("Controls").X/2), 50), Color.Black);

                    //Draws all of the controls
                    /*
                    _spriteBatch.DrawString(medievalFont, "Be the first knight to bring your opponents health to 0! Try not to fall off!", 
                        CenterFont("Be the first knight to bring your opponents health to 0! Try not to fall off!",
                        medievalFont, 125), Color.Black);
                    */
                    _spriteBatch.DrawString(medievalFont, "Pause - Q", CenterFont("Pause - Q",
                        medievalFont, 160, 0.5f), Color.Black);
                    _spriteBatch.DrawString(medievalFont, "Player 1:", CenterFont("Player 1:",
                        medievalFont, 250, 0.25f), Color.Black);
                    _spriteBatch.DrawString(medievalFont, "Move - Arrow Keys", CenterFont("Move - Arrow Keys",
                        medievalFont, 300, 0.25f), Color.Black);
                    _spriteBatch.DrawString(medievalFont, "Jab - P", CenterFont("Jab - P",
                        medievalFont, 350, 0.25f), Color.Black);
                 //   _spriteBatch.DrawString(medievalFont, "Special - O", CenterFont("Special - O",
                 //       medievalFont, 290), Color.Black);
                 //   _spriteBatch.DrawString(medievalFont, "Strong Attack - I", CenterFont("Strong Attack - I",
                 //       medievalFont, 320), Color.Black);
                    _spriteBatch.DrawString(medievalFont, "Dodge - L", CenterFont("Dodge - L",
                        medievalFont, 400, 0.25f), Color.Black); //CHANGE TO 350 
                       _spriteBatch.DrawString(medievalFont, "Double Jump - Up x 2", CenterFont("Double Jump - Up x 2",
                        medievalFont, 450, 0.25f), Color.Black);

                    _spriteBatch.DrawString(medievalFont, "Player 2:", CenterFont("Player 2:",
                        medievalFont, 250, 0.75f), Color.Black);
                    _spriteBatch.DrawString(medievalFont, "Move - WASD", CenterFont("Move - WASD",
                        medievalFont, 300, 0.75f), Color.Black);
                    _spriteBatch.DrawString(medievalFont, "Jab - Y", CenterFont("Jab - Y",
                        medievalFont, 350, 0.75f), Color.Black);
                 //   _spriteBatch.DrawString(medievalFont, "Special - T", CenterFont("Special - T",
                 //       medievalFont, 480), Color.Black);
                 //   _spriteBatch.DrawString(medievalFont, "Strong Attack - R", CenterFont("Strong Attack - R",
                  //      medievalFont, 510), Color.Black);
                    _spriteBatch.DrawString(medievalFont, "Dodge - G", CenterFont("Dodge - G",
                        medievalFont, 400, 0.75f), Color.Black); // CHANGE TO 540
                    _spriteBatch.DrawString(medievalFont, "Double Jump - W x 2", CenterFont("Double Jump - W x 2",
                        medievalFont, 450, 0.75f), Color.Black);

                    //Buttons and button boxes
                    //ShapeBatch.Box(buttonList[0].Postion, buttonList[0].ButtonColor);
                    ShapeBatch.Box(buttonList[1].Postion, buttonList[1].ButtonColor);
                    //ShapeBatch.Box(buttonList[2].Postion, buttonList[2].ButtonColor);
                    //buttonList[0].Draw(_spriteBatch, "Change P1 Keys", medievalFont);
                    buttonList[1].Draw(_spriteBatch, "Back", medievalFont);
                    //buttonList[2].Draw(_spriteBatch, "Change P2 Keys", medievalFont);
                    break;

                // draws all of the items needed in MapSelect
                case GameState.MapSelect:
                    _spriteBatch.DrawString(bigMedievalFont, "Map Select", new Vector2
                        ((width / 2) - (bigMedievalFont.MeasureString("Map Select").X/2), 50), Color.Black);

                    //Button boxes
                    ShapeBatch.Box(buttonList[1].Postion, buttonList[1].ButtonColor);
                    ShapeBatch.Box(mapButtonList[0].Postion, mapButtonList[0].ButtonColor);
                    ShapeBatch.Box(mapButtonList[1].Postion, mapButtonList[1].ButtonColor);
                    ShapeBatch.Box(mapButtonList[2].Postion, mapButtonList[2].ButtonColor);

                    //Changes the color of the button depending on which level is selected
                    switch (currentLevel)
                    {
                        //Map 1
                        case 1:
                            mapButtonList[0].ButtonColor = Color.AntiqueWhite;
                            mapButtonList[1].ButtonColor = deafultButtonColor;
                            mapButtonList[2].ButtonColor = deafultButtonColor;
                            break;

                        //Map 2
                        case 2:
                            mapButtonList[0].ButtonColor = deafultButtonColor;
                            mapButtonList[1].ButtonColor = Color.AntiqueWhite;
                            mapButtonList[2].ButtonColor = deafultButtonColor;
                            break;

                        //Map 3
                        case 3:
                            mapButtonList[0].ButtonColor = deafultButtonColor;
                            mapButtonList[1].ButtonColor = deafultButtonColor;
                            mapButtonList[2].ButtonColor = Color.AntiqueWhite;
                            break;
                    }

                    //Buttons for MapSelect
                    buttonList[1].Draw(_spriteBatch, "Back", medievalFont);
                    mapButtonList[0].Draw(_spriteBatch, "Map 1", medievalFont);
                    mapButtonList[1].Draw(_spriteBatch, "Map 2", medievalFont);
                    mapButtonList[2].Draw(_spriteBatch, "Map 3", medievalFont);

                    //Pictures of current maps and backgrounds for them
                    _spriteBatch.Draw(mapSquare, map1PicBackground, Color.Black);
                    _spriteBatch.Draw(mapSquare, map2PicBackground, Color.Black);
                    _spriteBatch.Draw(mapSquare, map3PicBackground, Color.Black);
                    _spriteBatch.Draw(map1Picture, new Rectangle(85, 200, 350, 203), Color.White);
                    _spriteBatch.Draw(map2Picture, new Rectangle(545, 200, 350, 203), Color.White);
                    _spriteBatch.Draw(map3Picture, new Rectangle(1005, 200, 350, 203), Color.White);
                    break;

                // draws everything in the battle scene
                case GameState.Battle:
                    GraphicsDevice.Clear(Color.Tan);
                    
                    player1.Draw(_spriteBatch, knight, hitbox, explosion);

                    player2.Draw(_spriteBatch, knight, hitbox, explosion);

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
                   // _spriteBatch.DrawString(medievalFont, player1.ToString(), new Vector2(440, 750), Color.Black);

                    // Draws player 1 hearts
                    for (int i = 0; i < player2.Stocks; i++)
                    {
                        _spriteBatch.Draw(heart, // texture
                            new Rectangle( // new rectangle
                            (550) + (35 * i),
                            702,
                            32,
                            32),
                            Color.Red);
                    }

                    _spriteBatch.DrawString(bigMedievalFont, player2.Health.ToString(), new Vector2(440, 725), Color.Black);

                    _spriteBatch.DrawString(bigMedievalFont,
                       player1.Health.ToString(),
                       new Vector2((_graphics.PreferredBackBufferWidth - (35 * player2.Stocks) - 550), 725),
                       Color.Black);


                    // Draws the words "Player 2: 
                    _spriteBatch.DrawString(medievalFont,
                        "Player 2:",
                        new Vector2((_graphics.PreferredBackBufferWidth - (35 * player2.Stocks) - 550), 700),
                        Color.Black);

                    // Drawing the hearts/ stock for player 2
                    for (int i = 0; i < player1.Stocks; i++)
                    {
                        _spriteBatch.Draw(heart, // texture
                            new Rectangle( // new rectangle
                            // puts every heart to the right of the previous.
                            ((_graphics.PreferredBackBufferWidth - 35 * player1.Stocks - 40) + (35*i)) - 400, 
                            702,
                            32,
                            32),
                            Color.Blue);
                    }



                    //Draws the player 1 state for testing
                    //_spriteBatch.DrawString(medievalFont, player1.ToString(), new Vector2(60, 70), Color.Black);

                    break;

                case GameState.Pause:
                    //Draws the Pause buttons
                    ShapeBatch.Box(buttonList[3].Postion, buttonList[3].ButtonColor);
                    ShapeBatch.Box(buttonList[4].Postion, buttonList[4].ButtonColor);

                    //Draws the Pause menu and button text
                    _spriteBatch.DrawString(bigMedievalFont, "Game Paused", new Vector2
                        ((width / 2) - (bigMedievalFont.MeasureString("Game Paused").X/2), 250), Color.Black);
                    buttonList[3].Draw(_spriteBatch, "Unpause", medievalFont);
                    buttonList[4].Draw(_spriteBatch, "Menu", medievalFont);
                    break;

                case GameState.EndScreen:
                    //Draws the End Screen buttons
                    ShapeBatch.Box(buttonList[1].Postion, buttonList[1].ButtonColor);

                    //Draws the End Screen menu and button text
                    _spriteBatch.DrawString(bigMedievalFont, "Game End", new Vector2
                        ((width / 2) - (bigMedievalFont.MeasureString("Game End").X/2), 150), Color.Black);
                    buttonList[1].Draw(_spriteBatch, "Menu", medievalFont);

                    if (!player2.PlayerAlive)
                    {
                        _spriteBatch.DrawString(medievalFont,
                            "Player 1 Wins!",
                            CenterFont("Player 1 Wins!", medievalFont, 400, 0.5f),
                            Color.Black);
                    }
                    else
                    {
                        _spriteBatch.DrawString(medievalFont,
                            "Player 2 Wins!",
                            CenterFont("Player 2 Wins!", medievalFont, 400, 0.5f),
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
            player2.PlayerPositionX = manager1.SpawnPoints[0].Position.X;
            player2.PlayerPositionY = manager1.SpawnPoints[1].Position.Y - 45;
            player1.PlayerAlive = true;
            player1.XVelocity = 0;
            player1.YVelocity = 0;

            // resets player 2
            player2.Stocks = manager1.Stocks;
            player2.Health = manager1.Health;
            player1.PlayerPositionX = manager1.SpawnPoints[1].Position.X;
            player1.PlayerPositionY = manager1.SpawnPoints[1].Position.Y - 45;
            player2.PlayerAlive = true;
            player2.XVelocity = 0;
            player2.YVelocity = 0;
        }

        public Vector2 CenterFont(string text, SpriteFont currentFont, int textHeight, float dividedByWidth)
        {
            return new Vector2((width * dividedByWidth) - (currentFont.MeasureString(text).X / 2), textHeight);
        }
    }
}
