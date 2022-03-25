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
    class Archer : CommonCharacter
    {
        private int arrows;

        public Archer(Texture2D texture, int x, int y, int width, int height, bool player1, GameManager gameManager, HurtBox hurtBox) : base(texture, x, y, width, height, player1, gameManager, hurtBox)
        {
            this.gameManager = gameManager;
            arrows = 10;
            this.texture = texture;
            this.position = new Rectangle(x, y, width, height);
            speed = 1; //Default speed???
            this.player1 = player1;
            this.hurtBox = hurtBox;
        }

        public int Arrows
        {
            get { return arrows; }
            set
            {

                if (value >= 10)
                {
                    arrows = 10;
                }
                else if (value <= 10 && value >= 0)
                {
                    arrows = value;
                }
            }
        }
    }
}
