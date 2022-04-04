using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Team_Majx_Game
{
    /// <summary>
    ///  This method inherits from the common character class
    /// </summary>
    class Knight : CommonCharacter
    {
        private int shieldHealth;
        private KeyboardState kbState;
        private KeyboardState prevKBState;
        private Hitbox currentHitbox;
        private Color tempColor;
        private SpriteEffects SpriteEffect;

        public Knight(Texture2D texture, int x, int y, int width, int height, bool player1, GameManager gameManager, HurtBox hurtBox) : base(texture, x, y, width, height, player1, gameManager, hurtBox)
        {
            this.gameManager = gameManager;
            shieldHealth = 100; //Default value???
            this.texture = texture;
            this.position = new Rectangle(x, y, width, height);
            speed = 1; //Default speed???
            this.player1 = player1;
            this.hurtBox = hurtBox;
        }

        //Returns and changes the shield health. Sets the shield health to 100 if the inputted number is greater than it
        public int ShieldHealth
        {
            get { return shieldHealth; }
            set
            {
                if (value >= 100)
                {
                    shieldHealth = 100;
                }
                else if (value <= 100 && value >= 0)
                {
                    shieldHealth = value;
                }
                else
                {
                    shieldHealth = 0;
                }
            }
        }

        //Draws the hitboxes and the player's current frame based on the number of frames in the attack.

        public override void Attack(CharacterAttackState attack, Direction direction, int frame, SpriteBatch _spriteBatch, Texture2D hitboxSprite, Texture2D spriteSheet)
        {
            switch (attack)
            {
                case CharacterAttackState.Jab:
                    if (frame > 2 && frame < 7 )
                    {
                        tempColor = Color.White;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                            currentHitbox = new Hitbox(new Rectangle(position.X - 25, position.Y, 50, 50), 10, new Vector2(-10, 10));
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                            currentHitbox = new Hitbox(new Rectangle(position.X + position.Width - 25 , Position.Y, 50, 50), 10, new Vector2(10, 10));
                        }
                        currentHitbox.Draw(_spriteBatch, hitboxSprite);
                    }

                    else if (frame < 2 || frame > 6)
                    {
                        tempColor = Color.Gray;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                        }
                    }
                    _spriteBatch.Draw(spriteSheet, position, new Rectangle(0, 0, 510, 510), tempColor, 0, Vector2.Zero, SpriteEffect, 0);
                    break;

                case CharacterAttackState.ForwardTilt:
                    if (frame > 3 && frame < 10)
                    {
                        tempColor = Color.White;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                            currentHitbox = new Hitbox(new Rectangle(position.X - 50, position.Y, 75, 50), 10, new Vector2(-14, -4));
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                            currentHitbox = new Hitbox(new Rectangle(position.X + position.Width - 25, Position.Y, 75, 50), 10, new Vector2(14, -4));
                        }
                        currentHitbox.Draw(_spriteBatch, hitboxSprite);
                    }
                    else if(frame < 3 || frame > 9)
                    {
                        tempColor = Color.Gray;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                        }
                    }
                    _spriteBatch.Draw(spriteSheet, position, new Rectangle(0, 0, 510, 510), tempColor, 0, Vector2.Zero, SpriteEffect, 0);
                    break;
                case CharacterAttackState.DownTilt:
                    if(frame > 4 && frame < 14)
                    {
                        tempColor = Color.White;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                            currentHitbox = new Hitbox(new Rectangle(position.X - 60, position.Y + 50, 60, 40), 10, new Vector2(-12, -2));
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                            currentHitbox = new Hitbox(new Rectangle(position.X + position.Width, Position.Y + 50, 60, 40), 10, new Vector2(12, -2));
                        }
                        currentHitbox.Draw(_spriteBatch, hitboxSprite);
                    }
                    else if (frame < 5 || frame > 13)
                    {
                        tempColor = Color.Gray;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                        }
                    }
                    _spriteBatch.Draw(spriteSheet, new Rectangle(Position.X, Position.Y + Position.Height / 2, Position.Width, Position.Height / 2), 
                        new Rectangle(0, 0, 510, 510), tempColor, 0, Vector2.Zero, SpriteEffect, 0);
                    break;

                case CharacterAttackState.UpTilt:
                    if (frame > 6 && frame < 12)
                    {
                        tempColor = Color.White;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                            currentHitbox = new Hitbox(new Rectangle(position.X, Position.Y - 80, 30, 80), 10, new Vector2(2, -15));
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                            currentHitbox = new Hitbox(new Rectangle(position.X + position.Width - 30, position.Y - 80, 30, 80), 10, new Vector2(-2, -15));
                        }
                        currentHitbox.Draw(_spriteBatch, hitboxSprite);
                    }
                    else if (frame < 7 || frame > 11)
                    {
                        tempColor = Color.Gray;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                        }
                    }
                    _spriteBatch.Draw(spriteSheet, position,
                        new Rectangle(0, 0, 510, 510), tempColor, 0, Vector2.Zero, SpriteEffect, 0);
                    break;

                case CharacterAttackState.NeutralAir:
                    if (frame > 4 && frame < 15)
                    {
                        tempColor = Color.White;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                            currentHitbox = new Hitbox(new Rectangle(position.X - 10, position.Y - 10 , position.Width + 20, position.Height + 20), 10, new Vector2(-5,-5));
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                            currentHitbox = new Hitbox(new Rectangle(position.X - 10, position.Y - 10, position.Width + 20, position.Height + 20), 10, new Vector2(5,-5));
                        }
                        currentHitbox.Draw(_spriteBatch, hitboxSprite);
                    }
                    else if (frame < 5 || frame > 14)
                    {
                        tempColor = Color.Gray;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                        }
                    }
                    _spriteBatch.Draw(spriteSheet, position,
                        new Rectangle(0, 0, 510, 510), tempColor, 0, Vector2.Zero, SpriteEffect, 0);
                    break;

                case CharacterAttackState.ForwardAir:
                    if (frame > 3 && frame < 10)
                    {
                        tempColor = Color.White;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                            currentHitbox = new Hitbox(new Rectangle(position.X - 55, position.Y, 80, 50), 10, new Vector2(-10, -4));
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                            currentHitbox = new Hitbox(new Rectangle(position.X + position.Width - 30, Position.Y, 80, 50), 10, new Vector2(10, -4));
                        }
                        currentHitbox.Draw(_spriteBatch, hitboxSprite);
                    }
                    else if (frame < 4 || frame > 9)
                    {
                        tempColor = Color.Gray;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                        }
                    }
                    _spriteBatch.Draw(spriteSheet, position,
                        new Rectangle(0, 0, 510, 510), tempColor, 0, Vector2.Zero, SpriteEffect, 0);
                    break;

                case CharacterAttackState.UpAir:
                    if (frame > 3 && frame < 8)
                    {
                        tempColor = Color.White;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                            currentHitbox = new Hitbox(new Rectangle(position.X - 10, position.Y - 10, position.Width + 20, 40), 10, new Vector2(-2, -10));
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                            currentHitbox = new Hitbox(new Rectangle(position.X - 10, position.Y - 10, position.Width + 20, 40), 10, new Vector2(2, -10));
                        }
                        currentHitbox.Draw(_spriteBatch, hitboxSprite);
                    }
                    else if (frame < 4 || frame > 7)
                    {
                        tempColor = Color.Gray;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                        }
                    }
                    _spriteBatch.Draw(spriteSheet, position,
                        new Rectangle(0, 0, 510, 510), tempColor, 0, Vector2.Zero, SpriteEffect, 0);
                    break;

                case CharacterAttackState.BackAir:
                    if (frame > 7 && frame < 17)
                    {
                        tempColor = Color.White;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                            currentHitbox = new Hitbox(new Rectangle(position.X + position.Width, position.Y + Position.Height/2 - 20, 50, 40), 20, new Vector2(16, -7));
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                            currentHitbox = new Hitbox(new Rectangle(position.X - 50, position.Y + Position.Height / 2 - 20, 50, 40), 20, new Vector2(16, -7));
                        }
                        currentHitbox.Draw(_spriteBatch, hitboxSprite);
                    }
                    else if (frame < 8 || frame > 16)
                    {
                        tempColor = Color.Gray;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                        }
                    }
                    _spriteBatch.Draw(spriteSheet, position,
                        new Rectangle(0, 0, 510, 510), tempColor, 0, Vector2.Zero, SpriteEffect, 0);
                    break;

                case CharacterAttackState.DownAir:
                    if (frame > 4 && frame < 10)
                    {
                        tempColor = Color.White;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                            currentHitbox = new Hitbox(new Rectangle(position.X + 10, position.Y + Position.Height, position.Width - 20, 50), 215, new Vector2(-1, 12));
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                            currentHitbox = new Hitbox(new Rectangle(position.X + 10, position.Y + Position.Height, position.Width - 20, 50), 15, new Vector2(1, 12));
                        }
                        currentHitbox.Draw(_spriteBatch, hitboxSprite);
                    }
                    else if (frame < 5 || frame > 9)
                    {
                        tempColor = Color.Gray;
                        if (direction == Direction.Left)
                        {
                            SpriteEffect = SpriteEffects.None;
                        }
                        else
                        {
                            SpriteEffect = SpriteEffects.FlipHorizontally;
                        }
                    }
                    _spriteBatch.Draw(spriteSheet, position,
                        new Rectangle(0, 0, 510, 510), tempColor, 0, Vector2.Zero, SpriteEffect, 0);
                    break;
            }

        }

        //Returns the number of endlag fram
        public override int getEndlag(CharacterAttackState attack)
        {
            switch (attack)
            {
                case CharacterAttackState.Jab:
                    return 9;
                case CharacterAttackState.ForwardTilt:
                    return 13;
                case CharacterAttackState.DownTilt:
                    return 18;

                case CharacterAttackState.ForwardAir:
                    return 14;
                case CharacterAttackState.NeutralAir:
                    return 19;
                case CharacterAttackState.UpAir:
                    return 11;
                case CharacterAttackState.BackAir:
                    return 20;
                case CharacterAttackState.DownAir:
                    return 14;
                case CharacterAttackState.UpTilt:
                    return 16;
                    
            }
            return 0;

        }
    }
}
