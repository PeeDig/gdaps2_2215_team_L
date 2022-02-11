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
        private GameState State;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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

            // FSM for what controls will go on in each state
            switch (State)
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // FSM to control what will be
            // drawn to the screen at a specific state
            switch (State)
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
    }
}
