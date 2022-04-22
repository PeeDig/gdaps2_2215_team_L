using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Team_Majx_Game
{
    //this handles all the game states of the character.
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
        JumpSquat,
        SpecialFall
    }
    //this handles the character's direction and will mirror their image if they are facing right.
    enum Direction
    {
        Right,
        Left
    }
    abstract class CommonCharacter
    {
        //Sets all the inherited variables for each character to the knight class
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
        protected Color color;

        //Creates the variables to check for the keyboard state
        private KeyboardState kbState;
        private KeyboardState prevKBState;

        //Variables for the explosion when a character dies
        private bool showExplosion;
        private Rectangle explosion;
        private int explosionFrames;

        //Controls how fast the characters decelerate while in the air and not holding a direction
        private int aerialDecelerateCooldown;

        // controls the game
        private bool playerAlive = true;
        private bool playerDied = false;

        Random rng = new Random();

        //Property to return or change the stock count
        public int Stocks
        {
            get { return stockCount; }
            set { stockCount = value; }
        }

        //Property to return or change whether or not the player is alive
        public bool PlayerAlive
        {
            get { return playerAlive; }
            set { playerAlive = value; }
        }

        // Next 2 properties get the x and y of the characters
        public int PlayerPositionX
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public int PlayerPositionY
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        //Property to return or change the health value
        public double Health
        {
            get { return health; }
            set { health = value; }
        }


        //Returns and changes the position, width, and height of the character
        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }

        //Property to return or change the speed value
        public double Speed
        {
            get { return speed; }
        }

        //Property to return or change the current x velocity
        public int XVelocity
        {
            get { return xVelocity; }
            set { xVelocity = value; }
        }

        //Property to return or change the current y velocity
        public int YVelocity
        {
            get { return yVelocity; }
            set { yVelocity = value; }
        }

        //Creates a character object with their position, textures, width, and height.
        //Also sets their base varaibles to their base values.
        protected CommonCharacter(Texture2D texture, int x, int y, int width, int height,
            bool player1, GameManager gameManager, HurtBox hurtBox, Color color)
        {
            this.gameManager = gameManager;
            this.texture = texture;
            this.position = new Rectangle(x, y, width, height);
            this.health = 100;
            this.stockCount = 3;
            this.player1 = player1;
            this.hurtBox = hurtBox;
            this.color = color;

            //Makes player 2 face left on spawn in
            if(player1 != true)
            {
                direction = Direction.Left;
            }
            yVelocity = 0;
            xVelocity = 0;
            lagFrames = 0;
            aerialDecelerateCooldown = 5;
        }

        //Does the attacks and updates the state based on input or game environment

        public void update(GameTime gameTime, Keys up, Keys down,
            Keys left, Keys right, Keys attack, Keys special, Keys strong, Keys dodge, SpriteBatch sb)
        {
            kbState = Keyboard.GetState();
            switch (currentAttackState)
            {
                case CharacterAttackState.Stand:
                    //checks for if the right key is held down and sets the attack state to walk, direction to right, and begins to accelerate right.
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
                    //checks for the up key being pressed and puts the character into jump squat. This is a short animation
                    //of anticipation before the jump and allows the user to input up attack, up strong, or up special.
                    else if (KeyPress(up))
                    {
                        currentAttackState = CharacterAttackState.JumpSquat;
                        lagFrames = 4;
                    }
                    //checks for the down key being pressed and puts the character into crouching state if it is
                    else if (KeyPress(down))
                    {
                        currentAttackState = CharacterAttackState.Crouch;
                    }
                    //checks for the attack key being pressed. Since the character is in the standing state and none of the 
                    //directional inputs are being pressed, the character will enter the jab state, which is the neutral attack on the ground.
                    else if (KeyPress(attack))
                    {
                        currentAttackState = CharacterAttackState.Jab;
                        lagFrames = getEndlag(CharacterAttackState.Jab);
                    }
                    //Neutral special
                    else if (KeyPress(special))
                    {
                        currentAttackState = CharacterAttackState.NeutralSpecial;
                    }
                    //Dodge
                    else if (KeyPress(dodge))
                    {
                        currentAttackState = CharacterAttackState.Dodge;
                        lagFrames = 32;
                    }
                    //Neutral strong
                    else if (KeyPress(strong))
                    {
                        currentAttackState = CharacterAttackState.ForwardStrong;
                        lagFrames = getEndlag(CharacterAttackState.ForwardStrong);
                    }
                    //Checks for if the up key was already held down. This will allow the player to use their up attack, up special,
                    //and up strong while on the ground without jumping.
                    else if (kbState.IsKeyDown(up))
                    {
                        if(KeyPress(attack))
                        {
                            currentAttackState = CharacterAttackState.UpTilt;
                            lagFrames = getEndlag(CharacterAttackState.UpTilt);
                        }
                        else if(KeyPress(special))
                        {
                            currentAttackState = CharacterAttackState.UpSpecial;
                            lagFrames = getEndlag(CharacterAttackState.UpSpecial);
                        }
                        else if(KeyPress(strong))
                        {
                            currentAttackState = CharacterAttackState.UpStrong;
                            lagFrames = getEndlag(CharacterAttackState.UpStrong);
                        }
                    }
                    xVelocity = 0;

                    //if the player falls off a platform, put them into the jump state without giving them initial y velocity.
                    //this will make them fall.
                    if(!StandingOnPlatform())
                    {
                        currentAttackState = CharacterAttackState.Jump;
                    }
                    TouchingWall();
                    position.X += xVelocity;
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
                                lagFrames = getEndlag(CharacterAttackState.ForwardStrong);
                            }
                            else if (KeyPress(dodge))
                            {
                                Decelerate();
                                currentAttackState = CharacterAttackState.Dodge;
                                lagFrames = 10;
                            }
                            else if (KeyPress(down))
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
                            lagFrames = getEndlag(CharacterAttackState.DownStrong);
                        }
                    }
                    else
                    {
                        currentAttackState = CharacterAttackState.Stand;
                    }
                    Decelerate();
                    position.X += xVelocity;
                    
                    break;

                //Case for when you press jump on the ground and you "charge" up the jump for a few frames before jumping.
                //Up tilt, up strong, and up special can be inputted during these frames.
                case CharacterAttackState.JumpSquat:
                    //Once jump squat ends, the character will jump. We set their vertical velocity to -16 so that they move upwards,
                    //and we change their y position by this value every frame. We increase this value by 1 every frame until it becomes positive and
                    //then then jump is over and the character will begin to fall.
                    if (lagFrames == 0)
                    {
                        yVelocity = -16;
                        position.Y += yVelocity;
                        currentAttackState = CharacterAttackState.Jump;
                    }
                    //checks for inputs for up attack, up strong, and up special.
                    else if (KeyPress(attack))
                    {
                        currentAttackState = CharacterAttackState.UpTilt;
                        lagFrames = getEndlag(CharacterAttackState.UpTilt);
                    }
                    else if (KeyPress(strong))
                    {
                        currentAttackState = CharacterAttackState.UpStrong;
                        lagFrames = getEndlag(CharacterAttackState.UpStrong);
                    }
                    else if (KeyPress(special))
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
                        TouchingCeiling();
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
                        if (KeyPress(dodge))
                        {
                            currentAttackState = CharacterAttackState.AirDodge;
                            lagFrames = 17;
                        }
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

                                else if (KeyPress(special))
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
                            else
                            {

                                aerialDecelerate();
                            }

                            if (KeyPress(up))
                            {
                                if (hasDoubleJump)
                                {
                                    yVelocity = -16;
                                    hasDoubleJump = false;
                                }
                            }

                            if (kbState.IsKeyDown(up))
                            {
                                if (KeyPress(attack))
                                {
                                    currentAttackState = CharacterAttackState.UpAir;
                                    lagFrames = getEndlag(CharacterAttackState.UpAir);
                                }
                                else if (KeyPress(special))
                                {
                                    currentAttackState = CharacterAttackState.UpSpecial;
                                    lagFrames = getEndlag(CharacterAttackState.UpSpecial);
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
                        TouchingWall();

                        position.X += xVelocity;
                        break;
                    }

                case CharacterAttackState.Jab:
                case CharacterAttackState.ForwardTilt:
                case CharacterAttackState.UpTilt:
                case CharacterAttackState.ForwardStrong:
                case CharacterAttackState.UpStrong:
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
                    Decelerate();
                    break;
                case CharacterAttackState.Dodge:
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
                    
                    TouchingCeiling();
                    position.X += xVelocity;
                    TouchingWall();
                    position.Y += yVelocity;
                    if (yVelocity > 0)
                    {
                        if(StandingOnPlatform())
                        {
                            Decelerate();
                        }
                    }
                    
                    aerialDecelerate();
                    yVelocity += 1;
                    break;

                case CharacterAttackState.DownTilt:
                case CharacterAttackState.DownStrong:
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
                    TouchingWall();
                    position.X += xVelocity;
                    Decelerate();
                    break;
                case CharacterAttackState.NeutralAir:
                case CharacterAttackState.ForwardAir:
                case CharacterAttackState.UpAir:
                case CharacterAttackState.BackAir:
                case CharacterAttackState.DownAir:
                case CharacterAttackState.AirDodge:
                    TouchingCeiling();
                    position.Y += yVelocity;
                    if (StandingOnPlatform())
                    {
                        currentAttackState = CharacterAttackState.LandingLag;
                        lagFrames = 8;
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
                        lagFrames--;
                        currentFrame++;
                        yVelocity += 1;
                    }
                    TouchingWall();
                    aerialDecelerate();
                    position.X += xVelocity;
                    break;
                case CharacterAttackState.UpSpecial:
                    TouchingCeiling();
                    position.Y += yVelocity;
                    if (lagFrames == 0)
                    {
                        currentAttackState = CharacterAttackState.SpecialFall;
                        currentFrame = 1;
                        yVelocity += 1;
                    }
                    else
                    {
                        lagFrames--;
                        currentFrame++;

                        if(yVelocity > 0)
                        yVelocity += 1;
                    }
                    TouchingWall();
                    aerialDecelerate();
                    position.X += xVelocity;
                    break;

                case CharacterAttackState.LandingLag:
                    if (lagFrames <= 0)
                    {
                        currentAttackState = CharacterAttackState.Stand;
                    }
                    else
                    {
                        lagFrames--;
                    }
                    break;

                case CharacterAttackState.SpecialFall:
                    TouchingCeiling();
                    if(!StandingOnPlatform())
                    {
                        YVelocity += 1;
                        position.Y += yVelocity;
                        TouchingWall();
                        position.X += xVelocity;
                    }
                    else
                    {
                        hasDoubleJump = true;
                        currentAttackState = CharacterAttackState.Stand;
                    }
                    
                    if(kbState.IsKeyDown(right))
                    {
                        AerialAccelerateRight();
                    }
                    else if(kbState.IsKeyDown(left))
                    {
                        AerialAccelerateLeft();
                    }
                    else
                    {
                        aerialDecelerate();
                    }
                    
                  

                    break;

                default:
                    currentAttackState = CharacterAttackState.Jump;
                    break;
            }

            // keeps track if the player is alive
            if (stockCount <= 0)
            {
                playerAlive = false;
            }

            if (currentAttackState == CharacterAttackState.Dodge && lagFrames < 30 && lagFrames > 2)
            {
                hurtBox.Position = new Rectangle(0, 0, 0, 0);
            }
            else if (currentAttackState != CharacterAttackState.Crouch)
                hurtBox.Position = position;

            else
                hurtBox.Position = new Rectangle(position.X, position.Y + position.Height / 2, position.Width, position.Height/2);

            // controls death and respawn
            if (health <= 0 || position.Y >= gameManager.ScreenHeight)
            {
                // runs the losestock method and respawn
                showExplosion = true;
                explosionFrames = 10;
                explosion = new Rectangle(position.X, position.Y - position.Height, position.Width, position.Height);
                LoseStockandRespawn();  
            }

            if(showExplosion)
            {
                if(explosionFrames > 0)
                {
                    explosionFrames -= 1;
                }
                else
                {
                    showExplosion = false;
                }
                
            }
            prevKBState = kbState;
        }

        //this runs from the knight class when the player gets hit. It puts the character into hitstun state so they cannot do anytinh
        //and sets the amount of frames that they are in hitstun for.
        public void gotHit(int hitStun)
        {
            currentAttackState = CharacterAttackState.Hitstun;
            lagFrames = hitStun;
        }

        //Handles drawing the sprite
        public void Draw(SpriteBatch spriteBatch, Texture2D spriteSheet, Texture2D hitboxSprite, Texture2D ex)
        {
            switch (currentAttackState)
            {
                //Draws the character in the neutral position when they are not attacking
                case CharacterAttackState.Stand:
                case CharacterAttackState.Walk:
                case CharacterAttackState.Jump:
                case CharacterAttackState.NeutralSpecial:
                case CharacterAttackState.DownSpecial:
                case CharacterAttackState.ForwardSpecial:
                    if (direction == Direction.Left)
                    {
                        spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 510, 510),
                            color, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 510, 510),
                            color, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    }
                    break;
                    //Draws the character crouching if they're in the crouch or jumpsquat state
                case CharacterAttackState.Crouch:
                case CharacterAttackState.JumpSquat:
                    if (direction == Direction.Left)
                    {
                        spriteBatch.Draw(spriteSheet, new Rectangle(Position.X, Position.Y + Position.Height / 2,
                            Position.Width, Position.Height / 2), new Rectangle(0, 0, 510, 510),
                            color, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(spriteSheet, new Rectangle(Position.X, Position.Y + Position.Height / 2,
                            Position.Width, Position.Height / 2), new Rectangle(0, 0, 510, 510),
                            color, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    }
                    break;
                    //Draws the character gray if they're in endlag, hitstun, or dodging
                case CharacterAttackState.LandingLag:
                case CharacterAttackState.Hitstun:
                case CharacterAttackState.Dodge:
                case CharacterAttackState.AirDodge:
                case CharacterAttackState.SpecialFall:
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
            //Draws the explosion if it should be drawn.
            if(showExplosion)
                spriteBatch.Draw(ex, explosion, Color.White);
        }

        //returns character state for testing purposes
        public string ToString()

        {
            return currentAttackState.ToString() + xVelocity.ToString();
        }

        //attack method to cover all the different attacks. It's virtual so the character classes can override them with 
        //their respective attacks. This also draws the character, so it is ran through the draw class.
        public virtual void Attack(CharacterAttackState attack, Direction direction,
            int frame, SpriteBatch _spriteBatch, Texture2D hitboxSprite, Texture2D spriteSheet, bool isPlayer1)
        {

        }

        //Returns the endlag of the current move. Overridden by each respective character class, and  returns 0 as a default.
        public virtual int getEndlag(CharacterAttackState attack)
        {
            return 0;
        }

        //Handles acceleration towards the right side of the screen to make the update method simpler.
        //Increases speed by 3 every frame until it reaches the max of 8.
        public void AccelerateRight()
        {
            if (!TouchingWall())
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
        }

        //Handles acceleration towards the left side of the screen to make the update method simpler.
        //Increases speed by -3 every frame until it reaches the max of -8.
        public void AccelerateLeft()
        {
            if (!TouchingWall())
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
        }

        //Runs if the player loses a life, respawns them, and sets related variables to their base value.
        public void LoseStockandRespawn()
        {
            playerDied = true;
            stockCount--; // takes away life
            health = 100; // resets the health to full

            Tile spawnTile = gameManager.RandomSpawnPoints[rng.Next(0, gameManager.RandomSpawnPoints.Count)];
            position.X = spawnTile.Position.X;
            position.Y = spawnTile.Position.Y - 64;
            xVelocity = 0;
            yVelocity = 0;

            playerDied = false;

        }

        // Recursion Method to create an explosion
        public void RecursionExplosion(Rectangle explosionPosition)
        {
            explosion = explosionPosition;
            if (explosionPosition.Width > 60)
            {
                explosion = new Rectangle(0,0,0,0);
            }
            else
            {
                RecursionExplosion(new Rectangle(explosionPosition.X - 5, explosionPosition.Y - 5, explosionPosition.Width + 5, explosionPosition.Y + 5));
            }
            
        }

        //Handles aerial acceleration towards the right side of the screen to make the update method simpler.
        //Increases speed by 2 every frame until it reaches the max of 8.
        public void AerialAccelerateRight()
        {
            TouchingWall();
            if (xVelocity < 8)
            {
                xVelocity += 2;
                if (xVelocity > 8)
                {
                    xVelocity--;
                }
            }
        }

        //Handles aerial acceleration towards the left side of the screen to make the update method simpler.
        //Increases speed by -2 every frame until it reaches the max of -8.
        public void AerialAccelerateLeft()
        {
            TouchingWall();
            if (xVelocity > -8)
            {
                xVelocity -= 2;
                if (xVelocity < -8)
                {
                    xVelocity++;
                }
            }
        }

        //Handles deceleration when you use a move (or stop) while moving (sliding)
        public void Decelerate()
        {
                if (xVelocity > 0)
                {
                    xVelocity -= 4;
                    if (xVelocity < 0)
                    {
                        xVelocity = 0;
                    }
                }
            
                else if (xVelocity < 0)
                {
                    xVelocity += 4;
                    if (xVelocity > 0)
                    {
                        xVelocity = 0;
                    }
                }
            
        }
        
        //Handles when the player stops holding a direction in the air and they need to slowly decelerate. If their aerial deceleration cooldown
        //is 5, then decrease their speed by 1 and set it to 0. If it's less than 5, increase it by 1 and do not change the x velocity.
        public void aerialDecelerate()
        {
            if (aerialDecelerateCooldown >= 5)
            {
                aerialDecelerateCooldown = 0;
                if (xVelocity > 0)
                {
                    xVelocity -= 1;
                    if (xVelocity < 0)
                    {
                        xVelocity = 0;
                    }
                }

                else if (xVelocity < 0)
                {
                    xVelocity += 1;
                    if (xVelocity > 0)
                    {
                        xVelocity = 0;
                    }
                }
            }
            else
            {
                aerialDecelerateCooldown += 1;
            }
            

        }

        //Checks if a key was just pressed
        public bool KeyPress(Keys key)
        {
            if (prevKBState != null)
                return kbState.IsKeyDown(key) && prevKBState.IsKeyUp(key);
            return false;
        }

        //Checks if the character is standing on a platform
        //Checks the bottom of the character and if it's below the top of the platform, then we stop all momentum in the y direction
        //and move them to the top of the platform.
        public bool StandingOnPlatform()
        {
            foreach (Tile t in gameManager.platforms)
            {
                if (t.TileType == TileType.Platform && position.X + position.Width >= t.Position.X && position.X <= t.Position.X + t.Position.Width &&
                    position.Y + position.Height >= t.Position.Y && position.Y <= t.Position.Y - 30)
                {
                    yVelocity = 0;
                    position.Y = t.Position.Y - position.Height + 4;
                    return true;
                }

            }
            return false;
        }

        //checks if the character is touching a wall and bounces them off of it.
        public bool TouchingWall()
        {
            foreach (Tile t in gameManager.platforms)
            {
                //Checks x position and if it's outside of the walls of the stage then we move the characer towards the center of the screen.
                //the right side had some bugs so we had to move them inwards more on that side.
                if(position.X <= 32)
                {
                    xVelocity = Math.Abs(xVelocity/2);
                    position.X += 4;
                    return true;
                }
                if (position.X + position.Width >= 1406)
                {
                    xVelocity = -(Math.Abs(xVelocity));
                    position.X -= 7;
                    return true;
                }
                //We check the side of the character that their moving on and if it's inside the side of the platform or the walls at the end of the 
                //screen, we move them in the opposite direction based on their x velocity. This prevents them getting stuck in the walls,
                //but bounces them off the platforms.
                else if (xVelocity <= 0)
                {
                    if ((t.TileType == TileType.Platform || t.TileType == TileType.Wall) && position.X <= t.Position.X + t.Position.Width && position.X + position.Width >= t.Position.X &&
                        position.Y <= t.Position.Y + t.Position.Height && position.Y + position.Height >= t.Position.Y + 5)
                    {
                        if(xVelocity < -8)
                            xVelocity = 8;
                        else
                            xVelocity = Math.Abs(xVelocity);
                        position.X += 2;
                        return true;
                    }
                }
                if(xVelocity >= 0)
                {
                    if((t.TileType == TileType.Platform || t.TileType == TileType.Wall) && position.X + position.Width >= t.Position.X && position.X <= t.Position.X + t.Position.Width &&
                        position.Y <= t.Position.Y + t.Position.Height && position.Y + position.Height >= t.Position.Y + 5)
                    {
                        if (xVelocity > 8)
                            xVelocity = -8;
                        else
                            xVelocity = -(Math.Abs(xVelocity));
                        position.X -= 2;
                        return true;
                    }
                }
            }
            return false;

        }

        //Checks if the character is touching the bottom of a platform and stops all momentum in the y direction. Checks the top of the
        //character and stops them if it's above the bottom of any of the platforms.
        public bool TouchingCeiling()
        {
            foreach (Tile t in gameManager.platforms)
            {
                if (t.TileType == TileType.Platform && position.X + position.Width >= t.Position.X && position.X <= t.Position.X + t.Position.Width &&
                    position.Y <= t.Position.Y + t.Position.Height && position.Y + position.Height >= t.Position.Y + t.Position.Height)
                {
                    yVelocity = 2;
                    return true;
                }
            }
            return false;
        }
    }
}
