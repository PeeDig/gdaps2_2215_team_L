using System;
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
        protected int xVelocity;
        protected int lagFrames;
        protected int currentFrame;
        protected bool hasDoubleJump = true;
        private KeyboardState kbState;
        private KeyboardState prevKBState;
        private Tile platform;
        private bool inEndlag;


        //Creates a character object with their position, textures, width, and height.
        protected CommonCharacter(Texture2D texture, int x, int y, int width, int height, bool player1, GameManager gameManager, HurtBox hurtBox)
        {
            this.gameManager = gameManager;
            this.texture = texture;
            this.position = new Rectangle(x, y, width, height);
            this.health = 100; //gameManager.Health; //GET HEALTH HERE FROM FILE
            this.stockCount = 3; //gameManager.Stocks; //GET STOCKS FROM FILE
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
         * Instead of edge cancelling we let them slide off the platform, but they stay in the endlag of their current move and finish the animation.
         * Could lead to some cool combos, but wouldn't lead to edge cancelling being too strong.
         * Start accelrating when the button is pressed and decelerate when an attack is used, or you stop running or you crouch.
         * Should jumping lower your speed? Do we want a base air speed for each class?
         * 
         * 
         */
        public void update(GameTime gameTime, Keys up, Keys down, Keys left, Keys right, Keys attack, Keys special, Keys strong, Keys dodge)
        {
            kbState = Keyboard.GetState();
            switch (currentAttackState)
            {
                case CharacterAttackState.Stand:
                    if (kbState.IsKeyDown(Keys.Right))
                    {
                        AccelerateRight();
                        currentAttackState = CharacterAttackState.Walk;
                        direction = Direction.Right;
                        break;
                    }
                    else if (kbState.IsKeyDown(Keys.Left))
                    {
                        AccelerateLeft();
                        currentAttackState = CharacterAttackState.Walk;
                        direction = Direction.Left;
                        break;
                    }
                    else if (KeyPress(up))
                    {
                        currentAttackState = CharacterAttackState.Jump;
                        yVelocity = -20;
                        position.Y += yVelocity;
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
                                yVelocity = -20;
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
                        //If they are walking and not on a platform, set them to the jump state but don't give them initial velocity so they will fall.
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
                        {
                            if (kbState.IsKeyDown(right))
                            {
                                AerialAccelerateRight();
                                if (KeyPress(attack))
                                {
                                    if (direction == Direction.Right)
                                    {
                                        currentAttackState = CharacterAttackState.ForwardAir;
                                    }
                                    else
                                    {
                                        currentAttackState = CharacterAttackState.BackAir;
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
                                    }
                                    else
                                    {
                                        currentAttackState = CharacterAttackState.BackAir;
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
                                    yVelocity = -20;
                                    hasDoubleJump = false;
                                }
                            }
                            
                            if (kbState.IsKeyDown(up))
                            {
                                if(KeyPress(attack))
                                {
                                    currentAttackState = CharacterAttackState.UpAir;
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
                                }
                                else if (KeyPress(special))
                                {
                                    currentAttackState = CharacterAttackState.DownSpecial;
                                }
                            }

                            else if (KeyPress(attack) && !kbState.IsKeyDown(right) || !kbState.IsKeyDown(left))
                            {
                                currentAttackState = CharacterAttackState.NeutralAir;
                            }
                            else if (KeyPress(special))
                            {
                                currentAttackState = CharacterAttackState.NeutralSpecial;
                            }

                            yVelocity += 1;
                        }
                        position.X += xVelocity;
                        break;
                    }

                case CharacterAttackState.Jab:
                case CharacterAttackState.ForwardTilt:
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
            }
            prevKBState = kbState;
        }

        //Handles drawing the sprite
        public void Draw(SpriteBatch spriteBatch, Texture2D spriteSheet, Texture2D hitboxSprite)
        {
            switch (currentAttackState)
            {
                //Draws character based on their state
                case CharacterAttackState.Stand:
                    if (direction == Direction.Left)
                    {
                        spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 900, 660), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 900, 660), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    }
                    break;
                case CharacterAttackState.Walk:
                    if (direction == Direction.Left)
                    {
                        spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 900, 660), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 900, 660), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    }
                    break;
                case CharacterAttackState.Crouch:
                    if (direction == Direction.Left)
                    {
                        spriteBatch.Draw(spriteSheet, new Rectangle(Position.X, Position.Y + Position.Height / 2, Position.Width, Position.Height / 2), new Rectangle(0, 0, 900, 660), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(spriteSheet, new Rectangle(Position.X, Position.Y + Position.Height / 2, Position.Width, Position.Height / 2), new Rectangle(0, 0, 900, 660), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    }
                    break;
                case CharacterAttackState.Jump:
                    if (direction == Direction.Left)
                    {
                        spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 900, 660), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 900, 660), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    }
                    break;
                    //Runs the attack method in the character classes based on the move inputted and the current frame we are on.
                case CharacterAttackState.Jab:
                case CharacterAttackState.ForwardTilt:
                case CharacterAttackState.DownTilt:
                    Attack(currentAttackState, direction, currentFrame, spriteBatch, hitboxSprite, spriteSheet);
                    break;

                    /*
                case CharacterAttackState.DownTilt:
                   Attack(CharacterAttackState.DownTilt, direction, currentFrame, spriteBatch, hitboxSprite, spriteSheet)
                        tempColor = Color.White;
                    if (direction == Direction.Left)
                    {
                        spriteBatch.Draw(spriteSheet, new Rectangle(Position.X, Position.Y + Position.Height / 2, Position.Width, Position.Height / 2), new Rectangle(0, 0, 900, 660), tempColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(spriteSheet, new Rectangle(Position.X, Position.Y + Position.Height / 2, Position.Width, Position.Height / 2), new Rectangle(0, 0, 900, 660), tempColor, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    }
                    break;
                    */

            }

    }

    

        //attack method to cover all the different attacks. It's virtual so the character classes and override them with their respective attacks.
        public virtual void Attack(CharacterAttackState attack, Direction direction, int frame, SpriteBatch _spriteBatch, Texture2D hitboxSprite, Texture2D spriteSheet)
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
