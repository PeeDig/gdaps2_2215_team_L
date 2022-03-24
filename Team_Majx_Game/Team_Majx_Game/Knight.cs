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

        public override bool Attack(CharacterAttackState attack, Direction direction, int frame, SpriteBatch _spriteBatch, Texture2D hitboxSprite, Texture2D spriteSheet)
        {
            switch (attack)
            {
                case CharacterAttackState.Jab:
                    if (frame > 2 && frame < 7 )
                    {
                        if (direction == Direction.Left)
                        {
                            _spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 900, 660), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                            currentHitbox = new Hitbox(new Rectangle(position.X - 50, position.Y, 50, 50), 10, 10, 20);
                        }
                        else
                        {
                            _spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 900, 660), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                            currentHitbox = new Hitbox(new Rectangle(position.X + position.Width, Position.Y, 50, 50), 10, 10, 20);
                        }
                        currentHitbox.Draw(_spriteBatch, hitboxSprite);
                    }
                    else if(frame > 6)
                    {
                        return true;
                    }
                    return false;
                case CharacterAttackState.ForwardTilt:
                    if (frame > 3 && frame < 10)
                    {
                        if (direction == Direction.Left)
                        {
                            _spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 900, 660), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                            currentHitbox = new Hitbox(new Rectangle(position.X - 75, position.Y, 75, 50), 10, 10, 20);
                        }
                        else
                        {
                            _spriteBatch.Draw(spriteSheet, Position, new Rectangle(0, 0, 900, 660), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                            currentHitbox = new Hitbox(new Rectangle(position.X + position.Width, Position.Y, 75, 50), 10, 10, 20);
                        }
                        currentHitbox.Draw(_spriteBatch, hitboxSprite);
                    }
                    else if(frame > 9)
                    {
                        return true;
                    }
                    return false;

            }
            return false;

        }

        public override int getEndlag(CharacterAttackState attack)
        {
            switch (attack)
            {
                case CharacterAttackState.Jab:
                    return 9;
                case CharacterAttackState.ForwardTilt:
                    return 13;
                    
            }
            return 0;

        }
    }
}
