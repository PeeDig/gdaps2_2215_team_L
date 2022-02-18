using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Team_Majx_Game
{
    /// <summary>
    /// This class is used for each of the different player classes
    /// </summary>
    abstract class CommonCharacter
    {
        protected Texture2D texture;
        protected Rectangle position;
        protected double speed;
        protected double health;

        //Creates a character object with their position, textures, width, and height.
        protected CommonCharacter(Texture2D texture, int x, int y, int width, int height)
        {
            this.texture = texture;
            this.position = new Rectangle(x, y, width, height);
            this.health = 100; //GET HEALTH HERE FROM FILE
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

    }
}
