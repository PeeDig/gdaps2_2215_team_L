using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Team_Majx_Game
{
    //The hitboxes of the moves is what actually does damage
    //this holds hte position and knockback values of each hitbox
    class Hitbox
    {
        private Rectangle position;
        private double damage;
        private Vector2 knockback;
        private int hitStun;

        public Hitbox(Rectangle position, double damage, Vector2 knockback, int hitStun)
        {
            this.position = position;
            this.damage = damage;
            this.knockback = knockback;
            this.hitStun = hitStun;
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

        public Vector2 Knockback
        {
            get { return knockback; }
            set { knockback = value; }
        }

        public int HitStun
        {
            get { return hitStun; }
        }

        //Draw the hurtbox for the demo and potential future testing
        public void Draw(SpriteBatch spriteBatch, Texture2D sprite)
        {
            spriteBatch.Draw(sprite, position, Color.White);
        }
    }
}
