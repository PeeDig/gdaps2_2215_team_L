using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Team_Majx_Game
{

    class Hitbox
    {
        private Rectangle position;
        private double damage;
        private double knockback;
        private double knockbackDirection;

        public Hitbox(Rectangle position, double damage, double knockback, double knockbackDirection)
        {
            this.position = position;
            this.damage = damage;
            this.knockback = knockback;
            this.knockbackDirection = knockbackDirection;
        }

        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }

        public double Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        public double Knockback
        {
            get { return knockback; }
            set { knockback = value; }
        }

        public double KnockbackDireciton
        {
            get { return knockbackDirection; }
            set { knockbackDirection = value; }
        }

        //Draw the hurtbox for the demo and potential future testing
        public void Draw(SpriteBatch spriteBatch, Texture2D sprite)
        {
            spriteBatch.Draw(sprite, position, Color.White);
        }
    }
}
