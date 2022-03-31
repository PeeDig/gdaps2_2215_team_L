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
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameState currentState;
        private KeyboardState prevkbState;
        private MouseState prevMsState;
        private List<Rectangle> buttonList;
        private SpriteFont font;
        private Texture2D hitbox;
        private Texture2D knight;
        private Knight player1;
        private HurtBox player1HurtBox;
        private SpriteFont medievalFont;

        private Texture2D tempSquare;

        private string levelFile = "level1.txt";

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
            _graphics.PreferredBackBufferWidth = 1440;
            _graphics.PreferredBackBufferHeight = 810;
            _graphics.ApplyChanges();
            // TODO: Add your initialization logic here
            prevkbState = Keyboard.GetState();
            prevMsState = Mouse.GetState();
            buttonList = new List<Rectangle>();
            buttonList.Add(new Rectangle(260, 600, 200, 75));
            buttonList.Add(new Rectangle(620, 600, 200, 75));
            buttonList.Add(new Rectangle(980, 600, 200, 75));

            //Creating a temporary knight for the purpose of the first demo
            player1HurtBox = new HurtBox(new Rectangle(720, 405, 100, 100));
            manager1 = new GameManager();
            

            player1 = new Knight(knight, 720, 405, 100, 100, true, manager1, player1HurtBox);


            manager1.ReadLevelFile(levelFile);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            hitbox = Content.Load<Texture2D>("hitbox");
            knight = Content.Load<Texture2D>("knight1");
            tempSquare = Content.Load<Texture2D>("red square");
            font = Content.Load<SpriteFont>("arial");
            //medievalFont = Content.Load<SpriteFont>("dutchMediaeval");

            // TODO: use this.Content to load your game content here
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
                        currentState = GameState.Battle;
                    }
                    else if (ClickButton(buttonList[2], msState))
                    {
                        currentState = GameState.CharSelect;
                    }
                    break;

                case GameState.Rules:
                    if (SingleKeyPress(Keys.Back, kbState))
                    {
                        currentState = GameState.Menu;
                    }
                    break;

                case GameState.Settings:
                    if (SingleKeyPress(Keys.Back, kbState))
                    {
                        currentState = GameState.Menu;
                    }
                    break;

                case GameState.CharSelect:
                    if (SingleKeyPress(Keys.Back, kbState))
                    {
                        currentState = GameState.Menu;
                    }
                    else if (SingleKeyPress(Keys.S, kbState))
                    {
                        currentState = GameState.Settings;
                    }
                    else if (SingleKeyPress(Keys.Enter, kbState))
                    {
                        currentState = GameState.Battle;
                    }
                    break;

                case GameState.Battle:
                    if (SingleKeyPress(Keys.P, kbState))
                    {
                        currentState = GameState.Pause;
                    }
                    else if (SingleKeyPress(Keys.W, kbState))
                    {
                        currentState = GameState.EndScreen;
                    }
                    break;

                case GameState.Pause:
                    if (SingleKeyPress(Keys.P, kbState))
                    {
                        currentState = GameState.Battle;
                    }
                    else if (SingleKeyPress(Keys.M, kbState))
                    {
                        currentState = GameState.Menu;
                    }
                    break;

                case GameState.EndScreen:
                    if (SingleKeyPress(Keys.Enter, kbState))
                    {
                        currentState = GameState.CharSelect;
                    }
                    else if (SingleKeyPress(Keys.M, kbState))
                    {
                        currentState = GameState.Menu;
                    }
                    break;
            }

            kbState = prevkbState;
            msState = prevMsState;

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
                case GameState.Menu:
                    ShapeBatch.Box(buttonList[0], Color.PapayaWhip);
                    ShapeBatch.Box(buttonList[1], Color.PapayaWhip);
                    _spriteBatch.DrawString(font, "Gameplay demo", new Vector2(620, 620), Color.Black);
                    _spriteBatch.DrawString(font, "test", new Vector2(0, 0), Color.Black);
                    ShapeBatch.Box(buttonList[2], Color.PapayaWhip);
                    break;

                case GameState.Rules:

                    break;

                case GameState.Settings:

                    break;

                case GameState.CharSelect:

                    break;

                case GameState.Battle:
                    player1.update(gameTime, Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.P, Keys.O, Keys.I, Keys.L);
                    player1.Draw(_spriteBatch, knight, hitbox);
                    _spriteBatch.Draw(hitbox, new Rectangle(720, 505, 400, 100), Color.White);

                    /*
                    // Draws the map
                    for(int r = 0; r < manager1.MapArray.GetLength(0); r++)
                    {
                        for(int c = 0; c < manager1.MapArray.GetLength(1); c++)
                        {
                            manager1.MapArray[r,c].Draw(_spriteBatch, tempSquare);
                        }
                    }
                    */


                    break;

                case GameState.Pause:

                    break;

                case GameState.EndScreen:

                    break;
            }

            ShapeBatch.End();
            _spriteBatch.End();
            base.Draw(gameTime);
        }

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

        public bool SingleMousePress(MouseState mState)
        {
            if (mState.LeftButton == ButtonState.Pressed && prevMsState.LeftButton == ButtonState.Released)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

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
