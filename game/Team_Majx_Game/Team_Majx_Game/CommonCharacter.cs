using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Team_Majx_Game
{
    /// <summary>
    /// This class is used for each of the different player classes
    /// </summary>
    /// 
    enum CharacterAttackState
    {
        Stand,
        Walk,
        Jump,
        Crouch,
        Jab,
        ForwardTilt,
        UpTilt,
        DownTilt,
        NeutralAir,
        ForwardAir,
        BackAir,
        DownAir,
        UpAir,
        NeutralSpecial,
        UpSpecial,
        ForwardSpecial,
        DownSpecial,
        ForwardStrong,
        UpStrong,
        DownStrong,
        Dodge,
        AirDodge
    }
    enum Direction
    {
    Right,
    Left
    }

    abstract class CommonCharacter
    {
        protected Texture2D texture;
        protected Rectangle position;
        protected double speed;
        protected double health;
        protected bool player1;
        protected CharacterAttackState currentAttackState;
        protected Direction direction;
        protected int stockCount;
        protected GameManager gameManager;
        protected HurtBox hurtBox;
        protected int yVelocity;
        private KeyboardState kbState;
        private KeyboardState prevKBState;
        private Tile platform;


        //Creates a character object with their position, textures, width, and height.
        protected CommonCharacter(Texture2D texture, int x, int y, int width, int height, bool player1, GameManager gameManager, HurtBox hurtBox)
        {
            this.gameManager = gameManager;
            this.texture = texture;
            this.position = new Rectangle(x, y, width, height);
            this.health = gameManager.Health; //GET HEALTH HERE FROM FILE
            this.stockCount = gameManager.Stocks; //GET STOCKS FROM FILE
            this.player1 = player1;
            this.hurtBox = hurtBox;
            yVelocity = 0;
           // platform = new Tile(new Rectangle(new Vector2(10, 10), 10, 10, ), TileType.Platform);
        }
    
        //Does the attacks and updates the state based on input or game environment
        public void update(GameTime gameTime)
        {
            switch(currentAttackState)
            {
                case CharacterAttackState.Stand:
                    if(KeyPress(Keys.Right))
                    {
                        position.X += 5;
                        currentAttackState = CharacterAttackState.Walk;
                        direction = Direction.Right;
                    }
                    else if(KeyPress(Keys.Left))
                    {
                        position.X -= 5;
                        currentAttackState = CharacterAttackState.Walk;
                        direction = Direction.Left;
                    }
                    else if(KeyPress(Keys.Up))
                    {
                        currentAttackState = CharacterAttackState.Jump;
                        yVelocity = 100;
                        position.Y += yVelocity;
                    }
                    else if (KeyPress(Keys.P))
                    {
                        currentAttackState = CharacterAttackState.Jab;
                    }
                    else if(KeyPress(Keys.O))
                    {
                        currentAttackState = CharacterAttackState.NeutralSpecial;
                    }
                    else if(KeyPress(Keys.L))
                    {
                        currentAttackState = CharacterAttackState.Dodge;
                    }
                    break;

                case CharacterAttackState.Walk:
                    if(direction == Direction.Right)
                    {
                        if(kbState.IsKeyDown(Keys.Right))
                        {
                            if (StandingOnPlatform())
                            {
                                if (KeyPress(Keys.Up))
                                {
                                    currentAttackState = CharacterAttackState.Jump;
                                    yVelocity = 100;
                                    position.Y += yVelocity;
                                }
                                else if (KeyPress(Keys.P))
                                {
                                    currentAttackState = CharacterAttackState.ForwardTilt;
                                }
                                else
                                {
                                    currentAttackState = CharacterAttackState.Stand;
                                }
                            }
                        }
                    }

                    break;
            }
        }

        //Returns and changes the position, width, and height of the character
        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }
        public double Speed
        {
            get { return speed; }
        }

        private bool KeyPress(Keys key)
        {
            if (prevKBState != null)
                return kbState.IsKeyDown(key) && prevKBState.IsKeyUp(key);
            return true;
        }

        //Checks if the character is standing on a platofrm
        public bool StandingOnPlatform()
        {
            foreach (Tile t in platform.Platforms)
            {
                if (position.X - position.Height <= t.Position.Y)
                {
                    return true;
                }
                else if (position.X + position.Y - position.Height <= t.Position.Y)
                {
                    return true;
                }
            }
            return false;
        }


    }
}
