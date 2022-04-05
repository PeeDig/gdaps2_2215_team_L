﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Team_Majx_Game
{
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
        LandingLag,
        JumpSquat
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
        protected int xVelocity;
        protected int lagFrames;
        protected int currentFrame;
        protected bool hasDoubleJump = true;
        private KeyboardState kbState;
        private KeyboardState prevKBState;
        private Tile platform;
        private bool inEndlag;

        public int Stocks
        {
            get { return stockCount; }
        }

        public double Health
        {
            get { return health; }
        }

        //Creates a character object with their position, textures, width, and height.
        protected CommonCharacter(Texture2D texture, int x, int y, int width, int height,
            bool player1, GameManager gameManager, HurtBox hurtBox)
        {
            this.gameManager = gameManager;
            this.texture = texture;
            this.position = new Rectangle(x, y, width, height);
            this.health = gameManager.Health; //GET HEALTH HERE FROM FILE
            this.stockCount = gameManager.Stocks;  //GET STOCKS FROM FILE
            this.player1 = player1;
            this.hurtBox = hurtBox;
            yVelocity = 0;
            xVelocity = 0;
            lagFrames = 0;
            // platform = new Tile(new Rectangle(new Vector2(10, 10), 10, 10, ), TileType.Platform);
        }

        //Does the attacks and updates the state based on input or game environment
        /*
         * Details:
         * Instead of edge cancelling we let them slide off the platform, but they stay in 
         * the endlag of their current move and finish the animation.
         * Could lead to some cool combos, but wouldn't lead to edge cancelling being too strong.
         * Start accelrating when the button is pressed and decelerate when an attack
         * is used, or you stop running or you crouch.
         * Should jumping lower your speed? Do we want a base air speed for each class?
         * 
         * 
         */
        public void update(GameTime gameTime, Keys up, Keys down,
            Keys left, Keys right, Keys attack, Keys special, Keys strong, Keys dodge)
        {
            kbState = Keyboard.GetState();
            switch (currentAttackState)
            {
                case CharacterAttackState.Stand:
                    if (kbState.IsKeyDown(right))
                    {
                        AccelerateRight();
                        currentAttackState = CharacterAttackState.Walk;
                        direction = Direction.Right;
                        break;
                    }
                    else if (kbState.IsKeyDown(left))
                    {
                        AccelerateLeft();
                        currentAttackState = CharacterAttackState.Walk;
                        direction = Direction.Left;
                        break;
                    }
                    else if (KeyPress(up))
                    {
                        currentAttackState = CharacterAttackState.JumpSquat;
                        lagFrames = 4;
                    }
                    else if (KeyPress(down))
                    {
                        currentAttackState = CharacterAttackState.Crouch;
                    }
                    else if (KeyPress(attack))
                    {
                        currentAttackState = CharacterAttackState.Jab;
                        lagFrames = getEndlag(CharacterAttackState.Jab);
                    }
                    else if (KeyPress(special))
                    {
                        currentAttackState = CharacterAttackState.NeutralSpecial;
                    }
                    else if (KeyPress(dodge))
                    {
                        currentAttackState = CharacterAttackState.Dodge;
                    }
                    else if (KeyPress(strong))
                    {
                        currentAttackState = CharacterAttackState.ForwardStrong;
                    }
                    else if(kbState.IsKeyDown(up))
                    {

                    }
                    Decelerate();
                    break;

                case CharacterAttackState.Walk:

                    if (kbState.IsKeyDown(right))
                    {
                        direction = Direction.Right;
                        AccelerateRight();
                    }
                    else if (kbState.IsKeyDown(left))
                    {
                        direction = Direction.Left;
                        AccelerateLeft();
                    }
                    if (kbState.IsKeyDown(right) || kbState.IsKeyDown(left))
                    {

                        if (StandingOnPlatform())
                        {
                            if (KeyPress(up))
                            {
                                currentAttackState = CharacterAttackState.Jump;
                                yVelocity = -16;
                                position.Y += yVelocity;
                            }
                            else if (KeyPress(attack))
                            {
                                Decelerate();
                                currentAttackState = CharacterAttackState.ForwardTilt;
                                lagFrames = getEndlag(CharacterAttackState.ForwardTilt);
                            }
                            else if (KeyPress(special))
                            {
                                Decelerate();
                                currentAttackState = CharacterAttackState.ForwardSpecial;
                            }
                            else if (KeyPress(strong))
                            {
                                Decelerate();
                                currentAttackState = CharacterAttackState.ForwardStrong;
                            }
                            else if (KeyPress(dodge))
                            {
                                Decelerate();
                                currentAttackState = CharacterAttackState.Dodge;
                            }
                            else if(KeyPress(down))
                            {
                                Decelerate();
                                currentAttackState = CharacterAttackState.Crouch;
                            }
                        }
                        //If they are walking and not on a platform, set them to the
                        //jump state but don't give them initial velocity so they will fall.
                        else
                        {
                            currentAttackState = CharacterAttackState.Jump;
                            yVelocity = 1;
                        }
                    }

                    //If they are not holding a direction, then they stand.
                    else
                    {
                        currentAttackState = CharacterAttackState.Stand;
                    }
                    position.X += xVelocity;
                    break;

                case CharacterAttackState.Crouch:
                    if (kbState.IsKeyDown(down))
                    {
                        if (KeyPress(attack))
                        {
                            currentAttackState = CharacterAttackState.DownTilt;
                            lagFrames = getEndlag(CharacterAttackState.DownTilt);
                        }
                        else if (KeyPress(special))
                        {
                            currentAttackState = CharacterAttackState.DownSpecial;
                        }
                        else if (KeyPress(strong))
                        {
                            currentAttackState = CharacterAttackState.DownStrong;
                        }
                    }
                    else
                    {
                        currentAttackState = CharacterAttackState.Stand;
                    }
                    position.X += xVelocity;
                    Decelerate();
                    break;

                //Case for when you press jump on the ground and you "charge" up the jump for a few frames before jumping.
                //Up tilt, up strong, and up special can be inputted during these frames.
                case CharacterAttackState.JumpSquat:
                    if(lagFrames == 0)
                    {
                        yVelocity = -16;
                        position.Y += yVelocity;
                        currentAttackState = CharacterAttackState.Jump;
                    }
                    else if(KeyPress(attack))
                    {
                        currentAttackState = CharacterAttackState.UpTilt;
                        lagFrames = getEndlag(CharacterAttackState.UpTilt);
                    }
                    else if(KeyPress(strong))
                    {
                        currentAttackState = CharacterAttackState.UpStrong;
                        lagFrames = getEndlag(CharacterAttackState.UpStrong);
                    }
                    else if(KeyPress(special))
                    {
                        currentAttackState = CharacterAttackState.UpSpecial;
                        lagFrames = getEndlag(CharacterAttackState.UpSpecial);
                    }
                    else
                    {
                        lagFrames--;
                    }
                    break;

                //Case for anytime the player is in the air
                case CharacterAttackState.Jump:
                    {
                        position.Y += yVelocity;
                        if (StandingOnPlatform())
                        {
                            hasDoubleJump = true;
                            if (kbState.IsKeyDown(left))
                            {
                                direction = Direction.Left;
                                currentAttackState = CharacterAttackState.Walk;
                                AccelerateLeft();
                            }
                            else if (kbState.IsKeyDown(right))
                            {
                                direction = Direction.Right;
                                currentAttackState = CharacterAttackState.Walk;
                                AccelerateRight();
                            }
                            else
                            {
                                currentAttackState = CharacterAttackState.Stand;
                            }
                        }
                        else
                        if (KeyPress(attack))
                        {
                            currentAttackState = CharacterAttackState.NeutralAir;
                            lagFrames = getEndlag(CharacterAttackState.NeutralAir);
                        }
                        else if (KeyPress(special))
                        {
                            currentAttackState = CharacterAttackState.NeutralSpecial;
                        }

                        {
                            if (kbState.IsKeyDown(right))
                            {
                                AerialAccelerateRight();
                                if (KeyPress(attack))
                                {
                                    if (direction == Direction.Right)
                                    {
                                        currentAttackState = CharacterAttackState.ForwardAir;
                                        lagFrames = getEndlag(CharacterAttackState.ForwardAir);
                                    }
                                    else
                                    {
                                        currentAttackState = CharacterAttackState.BackAir;
                                        lagFrames = getEndlag(CharacterAttackState.BackAir);
                                    }
                                }

                                else if(KeyPress(special))
                                {
                                    currentAttackState = CharacterAttackState.ForwardSpecial;
                                    direction = Direction.Right;
                                }
                            }
                            else if (kbState.IsKeyDown(left))
                            {
                                AerialAccelerateLeft();
                                if (KeyPress(attack))
                                {
                                    if (direction == Direction.Left)
                                    {
                                        currentAttackState = CharacterAttackState.ForwardAir;
                                        lagFrames = getEndlag(CharacterAttackState.ForwardAir);
                                    }
                                    else
                                    {
                                        currentAttackState = CharacterAttackState.BackAir;
                                        lagFrames = getEndlag(CharacterAttackState.BackAir);
                                    }
                                }

                                else if (KeyPress(special))
                                {
                                    currentAttackState = CharacterAttackState.ForwardSpecial;
                                    direction = Direction.Left;
                                }
                            }

                            if(KeyPress(up))
                            {
                                if(hasDoubleJump)
                                {
                                    yVelocity = -16;
                                    hasDoubleJump = false;
                                }
                            }
                            
                            if (kbState.IsKeyDown(up))
                            {
                                if(KeyPress(attack))
                                {
                                    currentAttackState = CharacterAttackState.UpAir;
                                    lagFrames = getEndlag(CharacterAttackState.UpAir);
                                }
                                else if(KeyPress(special))
                                {
                                    currentAttackState = CharacterAttackState.UpSpecial;
                                }
                            }

                            else if (kbState.IsKeyDown(down))
                            {
                                if (KeyPress(attack))
                                {
                                    currentAttackState = CharacterAttackState.DownAir;
                                    lagFrames = getEndlag(CharacterAttackState.DownAir);
                                }
                                else if (KeyPress(special))
                                {
                                    currentAttackState = CharacterAttackState.DownSpecial;
                                }
                            }

                            yVelocity += 1;
                        }
                        position.X += xVelocity;
                        break;
                    }

                case CharacterAttackState.Jab:
                case CharacterAttackState.ForwardTilt:
                case CharacterAttackState.UpTilt:
                    yVelocity = 0;
                
                    if (lagFrames == 0)
                    {
                        currentAttackState = CharacterAttackState.Stand;
                        currentFrame = 1;
                    }
                    else
                    {
                        lagFrames -= 1;
                        currentFrame++;
                    }
                    position.X += xVelocity;
                    position.Y += yVelocity;
                    Decelerate();
                    break;

                case CharacterAttackState.Hitstun:
                    if (lagFrames == 0)
                    {
                        currentAttackState = CharacterAttackState.Jump;
                        currentFrame = 1;
                    }
                    else
                    {
                        lagFrames -= 1;
                        currentFrame++;
                    }
                    position.X += xVelocity;
                    position.Y += yVelocity;
                    Decelerate();
                    break;

                case CharacterAttackState.DownTilt:
                    if (lagFrames == 0)
                    {
                        currentAttackState = CharacterAttackState.Crouch;
                        currentFrame = 1;
                    }
                    else
                    {
                        lagFrames -= 1;
                        currentFrame++;
                    }
                    position.X += xVelocity;
                    Decelerate();
                    break;
                case CharacterAttackState.NeutralAir:
                case CharacterAttackState.ForwardAir:
                case CharacterAttackState.UpAir:
                case CharacterAttackState.BackAir:
                case CharacterAttackState.DownAir:
                    position.Y += yVelocity;
                    if(StandingOnPlatform())
                    {
                        currentAttackState = CharacterAttackState.LandingLag;
                        lagFrames = 10;
                        currentFrame = 1;
                    }
                    else if (lagFrames == 0)
                    {
                        currentAttackState = CharacterAttackState.Jump;
                        currentFrame = 1;
                        yVelocity += 1;
                    }
                    else
                    {
                        lagFrames --;
                        currentFrame++;
                        yVelocity += 1;
                    }
                    position.X += xVelocity;
                    break;

                case CharacterAttackState.LandingLag:
                    if(lagFrames <= 0)
                    {
                        currentAttackState = CharacterAttackState.Stand;
                    }
                    else
                    {
                        lagFrames--;
                    }
                    break;
                    

            }
            prevKBState = kbState;

            hurtBox.Position = position;

            Knight temp = new Knight(texture, position.X, position.Y, position.Width, Position.Height, player1, gameManager, hurtBox);
            //Checks if the hurtbox if the
            

            // runs the losestock method and respawn
            LoseStockandRespawn();
        }


       public void gotHit(int hitStun)
        {
            currentAttackState = CharacterAttackState.Hitstun;
            lagFrames = hitStun;
        }


        //Handles drawing the sprite
        public void Draw(SpriteBatch spriteBatch, Texture2D spriteSheet, Texture2D hitboxSprite)
        {
            switch (currentAttackState)
            {
                //Draws character based on their state
                case CharacterAttackState.Stand:
                case CharacterAttackState.Walk:
                case CharacterAttackState.Jump:
                    if (direction == Direction.Left)
                    {
                        spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 510, 510),
                            Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 510, 510),
                            Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    }
                    break;      
                case CharacterAttackState.Crouch:
                case CharacterAttackState.JumpSquat:
                    if (direction == Direction.Left)
                    {
                        spriteBatch.Draw(spriteSheet, new Rectangle(Position.X, Position.Y + Position.Height / 2,
                            Position.Width, Position.Height / 2), new Rectangle(0, 0, 510, 510),
                            Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(spriteSheet, new Rectangle(Position.X, Position.Y + Position.Height / 2,
                            Position.Width, Position.Height / 2), new Rectangle(0, 0, 510, 510), 
                            Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    }
                    break;
                case CharacterAttackState.LandingLag:
                case CharacterAttackState.Hitstun:
                    if (direction == Direction.Left)
                    {
                        spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 510, 510),
                            Color.Gray, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 510, 510),
                            Color.Gray, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    }
                    break;

                //Runs the attack method in the character classes based on the move inputted and the current frame we are on (uses default because the 
                //method for each attack is the same so I can just use one case for all of them.
                default:
                    Attack(currentAttackState, direction, currentFrame, spriteBatch, hitboxSprite, spriteSheet, player1);
                    break;

            }

    }

    //returns character state

        public string ToString()

        {
            return currentAttackState.ToString() + " x: " + xVelocity.ToString() + "y: " + yVelocity.ToString();
        }


        //attack method to cover all the different attacks. It's virtual so the character classes
        //and override them with their respective attacks.
        public virtual void Attack(CharacterAttackState attack, Direction direction,
            int frame, SpriteBatch _spriteBatch, Texture2D hitboxSprite, Texture2D spriteSheet, bool isPlayer1)
        {
            
        }

        //Returns the endlag of the current move. Overridden by each respective character class
        public virtual int getEndlag(CharacterAttackState attack)
        {
            return 0;
        }

        //Handles acceleration to make the update method simpler
        public void AccelerateRight()
        {
            if (xVelocity < 8)
            {
                xVelocity += 3;
                if (xVelocity > 8)
                {
                    xVelocity--;
                }
            }
        }

        public void AccelerateLeft()
        {
            if (xVelocity > -8)
            {
                xVelocity -= 3;
                if (xVelocity < -8)
                {
                    xVelocity++;
                }
            }
        }

        // Determines if the player will lose a life.
        public void LoseStockandRespawn()
        {
            if(health <= 0)
            {
                stockCount--; // takes away life
                health = 100; // resets the health to full


                // ---- TODO ----Have the charcter respawn at a random spawn point ----
                /*
                 * 
                 */



                // ---- TODO ---- Have a quick exposion appear ----
                /*
                 * 
                 */
            }
        }

        // Recursion Method to create an explosion
        public void RecursionExplosion(float size, Vector2 position)
        {

        }

        public void AerialAccelerateRight()
        {
            if (xVelocity < 8)
            {
                xVelocity += 2;
                if (xVelocity > 8)
                {
                    xVelocity--;
                }
            }
        }

        public void AerialAccelerateLeft()
        {
            if (xVelocity > -8)
            {
                xVelocity -= 2;
                if (xVelocity < -8)
                {
                    xVelocity++;
                }
            }
        }

        //Handles deceleration when you use a move while moving (sliding)
        public void Decelerate()
        {
            if (direction == Direction.Right)
            {
                if (xVelocity > 0)
                {
                    xVelocity -= 4;
                    if (xVelocity < 0)
                    {
                        xVelocity = 0;
                    }
                }
            }
            else
            {
                if (xVelocity < 0)
                {
                    xVelocity += 4;
                    if (xVelocity > 0)
                    {
                        xVelocity = 0;
                    }
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
        public bool KeyPress(Keys key)
        {
            if (prevKBState != null)
                return kbState.IsKeyDown(key) && prevKBState.IsKeyUp(key);
            return true;
        }

        //Checks if the character is standing on a platform

        //TODO: Check if character is actually standing on a platform and not touching it from the side or the top.
        public bool StandingOnPlatform()
        {
            foreach (Tile t in gameManager.platforms)
            {
                if (position.Intersects(t.Position))
                {
                    return true;
                }
                /*
                if (position.X - position.Height <= t.Position.Y)
                {
                    return true;
                }
                else if (position.X + position.Y - position.Height <= t.Position.Y)
                {
                    return true;
                }
                */
            }
            return false;
        }

    }
}
