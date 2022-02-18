using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Team_Majx_Game
{
    // Enums control the Game State
    enum GameState
    {
        menu,
        rules,
        settings,
        charSelect,
        battle,
        pause,
        endScreen
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameState currentState;
        private KeyboardState prevkbState;

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

            // FSM for what controls will go on in each state
            switch (currentState)
            {
                case GameState.menu:
                    if (SingleKeyPress(Keys.R, kbState))
                    {
                        currentState = GameState.rules;
                    }
                    else if (SingleKeyPress(Keys.S, kbState))
                    {
                        currentState = GameState.settings;
                    }
                    else if (SingleKeyPress(Keys.Enter, kbState))
                    {
                        currentState = GameState.charSelect;
                    }
                    break;

                case GameState.rules:
                    if (SingleKeyPress(Keys.Back, kbState))
                    {
                        currentState = GameState.menu;
                    }
                    break;

                case GameState.settings:
                    if (SingleKeyPress(Keys.Back, kbState))
                    {
                        currentState = GameState.menu;
                    }
                    break;

                case GameState.charSelect:
                    if (SingleKeyPress(Keys.Back, kbState))
                    {
                        currentState = GameState.menu;
                    }
                    else if (SingleKeyPress(Keys.S, kbState))
                    {
                        currentState = GameState.settings;
                    }
                    else if (SingleKeyPress(Keys.Enter, kbState))
                    {
                        currentState = GameState.battle;
                    }
                    break;

                case GameState.battle:
                    if (SingleKeyPress(Keys.P, kbState))
                    {
                        currentState = GameState.pause;
                    }
                    else if (SingleKeyPress(Keys.W, kbState))
                    {
                        currentState = GameState.endScreen;
                    }
                    break;

                case GameState.pause:
                    if (SingleKeyPress(Keys.P, kbState))
                    {
                        currentState = GameState.battle;
                    }
                    else if (SingleKeyPress(Keys.M, kbState))
                    {
                        currentState = GameState.menu;
                    }
                    break;

                case GameState.endScreen:
                    if (SingleKeyPress(Keys.Enter, kbState))
                    {
                        currentState = GameState.charSelect;
                    }
                    else if (SingleKeyPress(Keys.M, kbState))
                    {
                        currentState = GameState.menu;
                    }
                    break;
            }

            kbState = prevkbState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // FSM to control what will be
            // drawn to the screen at a specific state
            switch (currentState)
            {
                case GameState.menu:
                    break;

                case GameState.rules:
                    break;

                case GameState.settings:
                    break;

                case GameState.charSelect:
                    break;

                case GameState.battle:
                    break;

                case GameState.pause:
                    break;

                case GameState.endScreen:
                    break;
            }

            base.Draw(gameTime);
        }

        private bool SingleKeyPress(Keys key, KeyboardState kbState)
        {
            if (kbState.IsKeyDown(key) && prevkbState.IsKeyUp(key))
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
