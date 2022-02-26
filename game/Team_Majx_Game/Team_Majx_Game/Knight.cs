using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Team_Majx_Game
{
    /// <summary>
    ///  This method inherits from the common character class
    /// </summary>
    class Knight : CommonCharacter
    {
        public int shieldHealth;

        public Knight(Texture2D texture, int x, int y, int width, int height) : base(texture, x, y, width, height)
        {
            shieldHealth = 100; //Default value???
            this.texture = texture;
            this.position = new Rectangle(x, y, width, height);
            speed = 1; //Default speed???
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
    }
}
