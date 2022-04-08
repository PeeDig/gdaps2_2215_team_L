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
        private bool attackDidDamage = false;
        private List<Hitbox> allHitboxes = new List<Hitbox>();

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

        public List<Hitbox> AllHitboxes
        {
            get { return allHitboxes; }
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

        public override void Attack(CharacterAttackState attack, Direction direction,
            int frame, SpriteBatch _spriteBatch, Texture2D hitboxSprite, Texture2D spriteSheet, bool isPlayer1)
        {
            switch (attack)
            {
                case CharacterAttackState.Jab:
                    if (frame > 2 && frame < 7 )
                    {
                        if (!attackDidDamage)
                        {
                            tempColor = Color.White;
                            if (direction == Direction.Left)
                            {
                                SpriteEffect = SpriteEffects.None;
                                currentHitbox = new Hitbox(new Rectangle(position.X - 25, position.Y, 50, 50), 10, new Vector2(-2, -2), 10);
                            }
                            else
                            {
                                SpriteEffect = SpriteEffects.FlipHorizontally;
                                currentHitbox = new Hitbox(new Rectangle(position.X + position.Width - 25, Position.Y, 50, 50), 10, new Vector2(2, -2), 10);
                            }

                            currentHitbox.Draw(_spriteBatch, hitboxSprite);
                            if (!allHitboxes.Contains(currentHitbox))
                                allHitboxes.Add(currentHitbox);
                        }
                    }

                    else if (frame < 2 || frame > 6)
                    {
                        allHitboxes.Clear();
                        currentHitbox = null;
                        attackDidDamage = false;
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
                        if (!attackDidDamage)
                        {
                            tempColor = Color.White;
                            if (direction == Direction.Left)
                            {
                                SpriteEffect = SpriteEffects.None;
                                currentHitbox = new Hitbox(new Rectangle(position.X - 50, position.Y, 75, 50), 10, new Vector2(-4, -2), 12);
                            }
                            else
                            {
                                SpriteEffect = SpriteEffects.FlipHorizontally;
                                currentHitbox = new Hitbox(new Rectangle(position.X + position.Width - 25, Position.Y, 75, 50), 10, new Vector2(4, -2), 12);
                            }
                            currentHitbox.Draw(_spriteBatch, hitboxSprite);
                            if (!allHitboxes.Contains(currentHitbox))
                                allHitboxes.Add(currentHitbox);
                        }
                    }
                    else if (frame < 3 || frame > 9)
                    {
                        allHitboxes.Clear();
                        attackDidDamage = false;
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
                    if (frame > 4 && frame < 14)
                    {
                        if (!attackDidDamage)
                        {
                            tempColor = Color.White;
                            if (direction == Direction.Left)
                            {
                                SpriteEffect = SpriteEffects.None;
                                currentHitbox = new Hitbox(new Rectangle(position.X - 60, position.Y + 50, 60, 40), 10, new Vector2(-5, -1), 14);
                            }
                            else
                            {
                                SpriteEffect = SpriteEffects.FlipHorizontally;
                                currentHitbox = new Hitbox(new Rectangle(position.X + position.Width, Position.Y + 50, 60, 40), 10, new Vector2(5, -1), 14);
                            }
                            currentHitbox.Draw(_spriteBatch, hitboxSprite);
                            if (!allHitboxes.Contains(currentHitbox))
                                allHitboxes.Add(currentHitbox);
                        }
                    }
                    else if (frame < 5 || frame > 13)
                    {
                        allHitboxes.Clear();
                        attackDidDamage = false;
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
                        if (!attackDidDamage)
                        {
                            tempColor = Color.White;
                            if (direction == Direction.Left)
                            {
                                SpriteEffect = SpriteEffects.None;
                                currentHitbox = new Hitbox(new Rectangle(position.X, Position.Y - 80, 30, 80), 10, new Vector2(-2, -5), 7);
                            }
                            else
                            {
                                SpriteEffect = SpriteEffects.FlipHorizontally;
                                currentHitbox = new Hitbox(new Rectangle(position.X + position.Width - 30, position.Y - 80, 30, 80), 10, new Vector2(2, -5), 7);
                            }
                            currentHitbox.Draw(_spriteBatch, hitboxSprite);
                            if (!allHitboxes.Contains(currentHitbox))
                                allHitboxes.Add(currentHitbox);
                        }
                    }
                    else if (frame < 7 || frame > 11)
                    {
                        allHitboxes.Clear();
                        attackDidDamage = false;
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
                        if (!attackDidDamage)
                        {
                            tempColor = Color.White;
                            if (direction == Direction.Left)
                            {
                                SpriteEffect = SpriteEffects.None;
                                currentHitbox = new Hitbox(new Rectangle(position.X - 10, position.Y - 10, position.Width + 20, position.Height + 20), 10, new Vector2(-3, -3), 8);
                            }
                            else
                            {
                                SpriteEffect = SpriteEffects.FlipHorizontally;
                                currentHitbox = new Hitbox(new Rectangle(position.X - 10, position.Y - 10, position.Width + 20, position.Height + 20), 10, new Vector2(3, -3), 8);
                            }
                            currentHitbox.Draw(_spriteBatch, hitboxSprite);
                            if (!allHitboxes.Contains(currentHitbox))
                                allHitboxes.Add(currentHitbox);
                        }
                    }
                    else if (frame < 5 || frame > 14)
                    {
                        allHitboxes.Clear();
                        attackDidDamage = false;
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
                        if (!attackDidDamage)
                        {
                            tempColor = Color.White;
                            if (direction == Direction.Left)
                            {
                                SpriteEffect = SpriteEffects.None;
                                currentHitbox = new Hitbox(new Rectangle(position.X - 55, position.Y, 80, 50), 10, new Vector2(-4, -3), 6);
                            }
                            else
                            {
                                SpriteEffect = SpriteEffects.FlipHorizontally;
                                currentHitbox = new Hitbox(new Rectangle(position.X + position.Width - 30, Position.Y, 80, 50), 10, new Vector2(4, -3), 6);
                            }
                            currentHitbox.Draw(_spriteBatch, hitboxSprite);
                            if (!allHitboxes.Contains(currentHitbox))
                                allHitboxes.Add(currentHitbox);
                        }
                    }
                    else if (frame < 4 || frame > 9)
                    {
                        allHitboxes.Clear();
                        attackDidDamage = false;
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
                        if (!attackDidDamage)
                        {
                            tempColor = Color.White;
                            if (direction == Direction.Left)
                            {
                                SpriteEffect = SpriteEffects.None;
                                currentHitbox = new Hitbox(new Rectangle(position.X - 10, position.Y - 10, position.Width + 20, 40), 10, new Vector2(-1, -6), 9);
                            }
                            else
                            {
                                SpriteEffect = SpriteEffects.FlipHorizontally;
                                currentHitbox = new Hitbox(new Rectangle(position.X - 10, position.Y - 10, position.Width + 20, 40), 10, new Vector2(1, -6), 9);
                            }
                            currentHitbox.Draw(_spriteBatch, hitboxSprite);
                            if (!allHitboxes.Contains(currentHitbox))
                                allHitboxes.Add(currentHitbox);
                        }
                    }
                    else if (frame < 4 || frame > 7)
                    {
                        allHitboxes.Clear();
                        attackDidDamage = false;
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
                        if (!attackDidDamage)
                        {
                            tempColor = Color.White;
                            if (direction == Direction.Left)
                            {
                                SpriteEffect = SpriteEffects.None;
                                currentHitbox = new Hitbox(new Rectangle(position.X + position.Width, position.Y + Position.Height / 2 - 20, 50, 40), 20, new Vector2(-7, -3), 16);
                            }
                            else
                            {
                                SpriteEffect = SpriteEffects.FlipHorizontally;
                                currentHitbox = new Hitbox(new Rectangle(position.X - 50, position.Y + Position.Height / 2 - 20, 50, 40), 20, new Vector2(7, -3), 16);
                            }
                            currentHitbox.Draw(_spriteBatch, hitboxSprite);
                            if (!allHitboxes.Contains(currentHitbox))
                                allHitboxes.Add(currentHitbox);
                        }
                    }
                    else if (frame < 8 || frame > 16)
                    {
                        allHitboxes.Clear();
                        attackDidDamage = false;
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
                        if (!attackDidDamage)
                        {
                            tempColor = Color.White;
                            if (direction == Direction.Left)
                            {
                                SpriteEffect = SpriteEffects.None;
                                currentHitbox = new Hitbox(new Rectangle(position.X + 10, position.Y + Position.Height, position.Width - 20, 50), 215, new Vector2(-1, 6), 12);
                            }
                            else
                            {
                                SpriteEffect = SpriteEffects.FlipHorizontally;
                                currentHitbox = new Hitbox(new Rectangle(position.X + 10, position.Y + Position.Height, position.Width - 20, 50), 15, new Vector2(1, 6), 12);
                            }
                            currentHitbox.Draw(_spriteBatch, hitboxSprite);
                            if (!allHitboxes.Contains(currentHitbox))
                                allHitboxes.Add(currentHitbox);
                        }
                    }
                    else if (frame < 5 || frame > 9)
                    {
                        allHitboxes.Clear();
                        attackDidDamage = false;
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
                default:
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


        public void DealDamage(Knight player)
        {
            if (allHitboxes.Count != 0)
            {
                foreach (Hitbox h in allHitboxes)
                {
                    if (player.hurtBox.Position.Intersects(h.Position))
                    {
                        player.health -= h.Damage;
                        player.xVelocity = (int)h.Knockback.X;
                        player.yVelocity = (int)h.Knockback.Y;
                        player.gotHit(h.HitStun);
                        AllHitboxes.Remove(h);
                        attackDidDamage = true;
                        break;
                    }
                }
            }
        }
    }
}
