using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Team_Majx_Game
{
    //Controls the buttons for the game's menu
    class Button
    {
        //Declares all needed fields
        private SpriteFont medievalFont;
        private string buttonTxt;
        private Rectangle postion;
        private Vector2 buttonTxtLoc;

        //Parameterized constructor
        public Button(GraphicsDevice device, Rectangle postion, string buttonTxt, SpriteFont medievalFont)
        {
            this.postion = postion;
            this.medievalFont = medievalFont;
            this.buttonTxt = buttonTxt;
            Vector2 textSize = medievalFont.MeasureString(buttonTxt);
            buttonTxtLoc = new Vector2(
                (postion.X + postion.Width / 2) - textSize.X / 2,
                (postion.Y + postion.Height / 2) - textSize.Y / 2);
        }
    }
}
