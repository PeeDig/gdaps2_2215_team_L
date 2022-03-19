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
        protected int lagFrames;
        private KeyboardState kbState;
        private KeyboardState prevKBState;
        private Tile platform;
        private Hitbox jabHitbox;


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
            lagFrames = 0;
            // platform = new Tile(new Rectangle(new Vector2(10, 10), 10, 10, ), TileType.Platform);
        }

        //Does the attacks and updates the state based on input or game environment
        public void update(GameTime gameTime, Keys up, Keys down, Keys left, Keys right, Keys attack, Keys special, Keys strong, Keys dodge)
        {
            kbState = Keyboard.GetState();
            switch (currentAttackState)
            {
                case CharacterAttackState.Stand:
                    if (KeyPress(right))
                    {
                        position.X += 8;
                        currentAttackState = CharacterAttackState.Walk;
                        direction = Direction.Right;
                    }
                    else if (KeyPress(left))
                    {
                        position.X -= 8;
                        currentAttackState = CharacterAttackState.Walk;
                        direction = Direction.Left;
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
                        lagFrames = 20;
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
                        ;
                    }
                    break;

                case CharacterAttackState.Walk:

                    if (kbState.IsKeyDown(right))
                    {
                        direction = Direction.Right;
                    }
                    else if (kbState.IsKeyDown(left))
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
                                yVelocity = -20;
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
                                if (direction == Direction.Right)
                                {
                                    position.X += 8;
                                }
                                else
                                {
                                    position.X -= 8;
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

                //Do aerials and specials
                case CharacterAttackState.Jump:
                    {
                        position.Y += yVelocity;
                        if (StandingOnPlatform())
                        {
                            if (kbState.IsKeyDown(left))
                            {
                                direction = Direction.Left;
                                currentAttackState = CharacterAttackState.Walk;
                                position.X -= 8;
                            }
                            else if (kbState.IsKeyDown(right))
                            {
                                direction = Direction.Right;
                                currentAttackState = CharacterAttackState.Walk;
                                position.X += 8;
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
                                position.X += 8;
                            }
                            else if (kbState.IsKeyDown(left))
                            {
                                position.X -= 8;
                            }
                            yVelocity += 1;
                        }

                        break;
                    }

                case CharacterAttackState.Jab:
                    if (lagFrames == 0)
                    {
                        currentAttackState = CharacterAttackState.Stand;
                    }
                    else
                    {
                        lagFrames -= 1;
                    }
                    break;
            }
            prevKBState = kbState;
        }

        //Handles drawing the sprite
        public void Draw(SpriteBatch spriteBatch, Texture2D spriteSheet, Texture2D hitboxSprite)
        {
            switch (currentAttackState)
            {
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
                case CharacterAttackState.Jab:

                    if (direction == Direction.Left)
                    {
                        spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 900, 660), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        jabHitbox = new Hitbox(new Rectangle(position.X - 50, position.Y, 50, 50), 10, 10, 20);
                    }
                    else
                    {
                        spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 900, 660), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                        jabHitbox = new Hitbox(new Rectangle(position.X+ position.Width, Position.Y, 50, 50), 10, 10, 20);
                    }

                    jabHitbox.Draw(spriteBatch, hitboxSprite);
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