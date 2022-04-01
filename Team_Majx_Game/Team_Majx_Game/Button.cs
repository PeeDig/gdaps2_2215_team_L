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

        //Property for rectangle of the button
        public Rectangle Postion
        {
            get { return postion; }
        }

        public string ButtonTxt
        {
            get { return buttonTxt; }
            set { buttonTxt = value; }
        }

        //Parameterized constructor
        public Button(Rectangle postion, SpriteFont medievalFont)
        {
            this.postion = postion;
            this.medievalFont = medievalFont;
            //this.buttonTxt = buttonTxt;
        }

        // button's text location
        public void TextLocation()
        {
            Vector2 textSize = medievalFont.MeasureString(buttonTxt);
            buttonTxtLoc = new Vector2(
                (postion.X + postion.Width / 2) - textSize.X / 2,
                (postion.Y + postion.Height / 2) - textSize.Y / 2);
        }

        // method controls a single mouse button click
        public bool SingleMousePress(MouseState mState, MouseState prevMsState)
        {
            if (mState.LeftButton == ButtonState.Released && prevMsState.LeftButton == ButtonState.Pressed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // controls the button clicks in monogame
        public bool ClickButton(MouseState mState, MouseState prevMsState)
        {
            if (SingleMousePress(mState, prevMsState) && postion.Contains(mState.X, mState.Y))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Draws the button's text
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(medievalFont, buttonTxt, buttonTxtLoc, Color.Black);
        }
    }
}
