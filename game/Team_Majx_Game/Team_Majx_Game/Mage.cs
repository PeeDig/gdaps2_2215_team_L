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
    class Mage : CommonCharacter
    {
        private int mana;
        public Mage(Texture2D texture, int x, int y, int width, int height, bool player1, GameManager gameManager) : base(texture, x, y, width, height, player1, gameManager)
        {
            this.gameManager = gameManager;
            mana = 100;
            this.texture = texture;
            this.position = new Rectangle(x, y, width, height);
            speed = 1; //Default speed???
            this.player1 = player1;
        }

        //Returns and changes the mana. Sets mana to 100 if the inputted number is greater than it
        public int Mana
        {
            get { return mana; }
            set
            {
                if (value >= 100)
                {
                    mana = 100;
                }
                else if (value <= 100 && value >= 0)
                {
                    mana = value;
                }
            }
        }
    }
}
