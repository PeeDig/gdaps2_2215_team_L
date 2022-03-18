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
        AirDodge,
        Hitstun,
        LandingLag
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
        public void update(GameTime gameTime, Keys up, Keys down, Keys left, Keys right, Keys attack, Keys special, Keys strong, Keys dodge)
        {
            switch(currentAttackState)
            {
                case CharacterAttackState.Stand:
                    if(KeyPress(right))
                    {
                        position.X += 5;
                        currentAttackState = CharacterAttackState.Walk;
                        direction = Direction.Right;
                    }
                    else if(KeyPress(left))
                    {
                        position.X -= 5;
                        currentAttackState = CharacterAttackState.Walk;
                        direction = Direction.Left;
                    }
                    else if(KeyPress(up))
                    {
                        currentAttackState = CharacterAttackState.Jump;
                        yVelocity = 100;
                        position.Y += yVelocity;
                    }
                    else if (KeyPress(down))
                    {
                        currentAttackState = CharacterAttackState.Crouch;
                    }
                    else if (KeyPress(attack))
                    {
                        currentAttackState = CharacterAttackState.Jab;
                    }
                    else if(KeyPress(special))
                    {
                        currentAttackState = CharacterAttackState.NeutralSpecial;
                    }
                    else if(KeyPress(dodge))
                    {
                        currentAttackState = CharacterAttackState.Dodge;
                    }
                    else if(KeyPress(strong))
                    {
                        currentAttackState = CharacterAttackState.ForwardStrong;
;
                    }
                    break;

                case CharacterAttackState.Walk:

                    if (kbState.IsKeyDown(right))
                    {
                        direction = Direction.Right;
                    }
                    else if(kbState.IsKeyDown(left))
                    {
                        direction = Direction.Left;
                    }
                    if (kbState.IsKeyDown(right) || kbState.IsKeyDown(left))
                    {

                        if (StandingOnPlatform())
                        {
                            if (KeyPress(up))
                            {
                                currentAttackState = CharacterAttackState.Jump;
                                yVelocity = 100;
                                position.Y += yVelocity;
                            }
                            else if (KeyPress(attack))
                            {
                                currentAttackState = CharacterAttackState.ForwardTilt;
                            }
                            else if (KeyPress(special))
                            {
                                currentAttackState = CharacterAttackState.ForwardSpecial;
                            }
                            else if (KeyPress(strong))
                            {
                                currentAttackState = CharacterAttackState.ForwardStrong;
                            }
                            else if (KeyPress(dodge))
                            {
                                currentAttackState = CharacterAttackState.Dodge;
                            }
                            else
                            {
                                if(direction == Direction.Right)
                                {
                                    position.X += 5;
                                }
                                else
                                {
                                    position.X -= 5;
                                }
                            }
                        }
                        //If they are walking and not on a platform, set them to the jump state but don't give them initial velocity so they will fall.
                        else
                        {
                            currentAttackState = CharacterAttackState.Jump;
                        }
                    }

                    //If they are not holding a direction, then they stand.
                    else
                    {
                        currentAttackState = CharacterAttackState.Stand;
                    }
                    

                    break;

                case CharacterAttackState.Jump:
                    {
                        if(StandingOnPlatform())
                        {
                            if(kbState.IsKeyDown(left))
                            {
                                direction = Direction.Left;
                                currentAttackState = CharacterAttackState.Walk;
                                position.X -= 5;
                            }
                            else if (kbState.IsKeyDown(right))
                            {
                                direction = Direction.Right;
                                currentAttackState = CharacterAttackState.Walk;
                                position.X += 5;
                            }
                            else
                            {
                                currentAttackState = CharacterAttackState.Stand;
                            }
                        }
                        else
                        {
                            if(kbState.IsKeyDown(right))
                            {
                                position.X += 5;
                               

                            }
                            else if(kbState.IsKeyDown(left))
                            { 
                                position.X -= 5;
                            }
                            
                        }
                        break;
                    }
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

        //Checks if a key was just pressed
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
