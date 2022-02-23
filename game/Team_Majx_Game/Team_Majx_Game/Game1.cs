using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            prevkbState = Keyboard.GetState();
            prevMsState = Mouse.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

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
                    if (SingleKeyPress(Keys.R, kbState))
                    {
                        currentState = GameState.Rules;
                    }
                    else if (SingleKeyPress(Keys.S, kbState))
                    {
                        currentState = GameState.Settings;
                    }
                    else if (SingleKeyPress(Keys.Enter, kbState))
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
                    
                    break;

                case GameState.Rules:

                    break;

                case GameState.Settings:

                    break;

                case GameState.CharSelect:

                    break;

                case GameState.Battle:

                    break;

                case GameState.Pause:

                    break;

                case GameState.EndScreen:

                    break;
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private bool SingleKeyPress(Keys key, KeyboardState kbState)
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
    }
}
