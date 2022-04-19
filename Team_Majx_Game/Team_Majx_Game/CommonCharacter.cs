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
        LandingLag,
        JumpSquat,
        SpecialFall
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
        protected Color color;
        private KeyboardState kbState;
        private KeyboardState prevKBState;
        private bool showExplosion;
        private Rectangle explosion;
        private int explosionFrames;
        private int aerialDecelerateCooldown;

        // controls the game
        private bool playerAlive = true;
        private bool playerDied = false;

        Random rng = new Random();

        public int Stocks
        {
            get { return stockCount; }
            set { stockCount = value; }
        }

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

        public double Health
        {
            get { return health; }
            set { health = value; }
        }


        //Creates a character object with their position, textures, width, and height.
        protected CommonCharacter(Texture2D texture, int x, int y, int width, int height,
            bool player1, GameManager gameManager, HurtBox hurtBox, Color color)
        {
            this.gameManager = gameManager;
            this.texture = texture;
            this.position = new Rectangle(x, y, width, height);
            this.health = gameManager.Health; //GET HEALTH HERE FROM FILE
            this.stockCount = gameManager.Stocks;  //GET STOCKS FROM FILE
            this.player1 = player1;
            this.hurtBox = hurtBox;
            this.color = color;

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
                        lagFrames = 10;
                    }
                    else if (KeyPress(strong))
                    {
                        currentAttackState = CharacterAttackState.ForwardStrong;
                        lagFrames = getEndlag(CharacterAttackState.ForwardStrong);
                    }
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
                    TouchingWall();
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
                    if (lagFrames == 0)
                    {
                        yVelocity = -16;
                        position.Y += yVelocity;
                        currentAttackState = CharacterAttackState.Jump;
                    }
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
                    if(StandingOnPlatform())
                    {
                        currentAttackState = CharacterAttackState.Walk;
                        hasDoubleJump = true;
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
                    
                    YVelocity += 1;
                    position.Y += yVelocity;
                    TouchingWall();
                    position.X += xVelocity;

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

            if (currentAttackState == CharacterAttackState.Dodge && lagFrames < 13 && lagFrames > 2)
            {
                hurtBox.Position = new Rectangle(0, 0, 0, 0);
            }
            else if (currentAttackState != CharacterAttackState.Crouch)
                hurtBox.Position = position;

            else
                hurtBox.Position = new Rectangle(position.X, position.Y - position.Height / 2, position.Height / 2, position.Width);

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
                //Draws character based on their state
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
            if(showExplosion)
                spriteBatch.Draw(ex, explosion, Color.White);
            
            ;

        }

        //returns character state

        public string ToString()

        {
            return currentAttackState.ToString();
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

        // Determines if the player will lose a life.
        public void LoseStockandRespawn()
        {
            playerDied = true;
            stockCount--; // takes away life
            health = 100; // resets the health to full

            // ---- TODO ---- Have a quick exposion appear ----
            // SpriteBatch.Draw(game1Object.Explosion, position, Color.White);  // draws the explosion

            //RecursionExplosion(Position);
            // ---- TODO ----Have the charcter respawn at a random spawn point ----
            Tile spawnTile = gameManager.RandomSpawnPoints[rng.Next(0, gameManager.RandomSpawnPoints.Count)];
            position.X = spawnTile.Position.X;
            position.Y = spawnTile.Position.Y - 64;
            xVelocity = 0;
            YVelocity = 0;

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

        //Handles deceleration when you use a move while moving (sliding)
        public void Decelerate()
        {
                if (xVelocity > 0)
                {
                    xVelocity -= 2;
                    if (xVelocity < 0)
                    {
                        xVelocity = 0;
                    }
                }
            
                else if (xVelocity < 0)
                {
                    xVelocity += 2;
                    if (xVelocity > 0)
                    {
                        xVelocity = 0;
                    }
                }
            
        }

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

        public int XVelocity
        {
            get { return xVelocity; }
            set { xVelocity = value; }
        }

        public int YVelocity
        {
            get { return yVelocity; }
            set { yVelocity = value; }
        }

        //Checks if a key was just pressed
        public bool KeyPress(Keys key)
        {
            if (prevKBState != null)
                return kbState.IsKeyDown(key) && prevKBState.IsKeyUp(key);
            return false;
        }

        //Checks if the character is standing on a platform

        //TODO: Check if character is actually standing on a platform and not touching it from the side or the top.
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

        public bool TouchingWall()
        {
            foreach (Tile t in gameManager.platforms)
            {
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
