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
    /// 
    enum CharacterAttackState
    {
        Stand,
        Walk,
        Jump,
        Crouch,
        Jab,
        ForwardTilt,
        UpTilt,
        DownTilt,
        NeutralAir,
        ForwardAir,
        BackAir,
        DownAir,
        UpAir,
        NeutralSpecial,
        UpSpecial,
        ForwardSpecial,
        DownSpecial,
        ForwardStrong,
        UpStrong,
        DownStrong,
    }
    enum Direction
    {
    Right,
    Left
    }

    abstract class CommonCharacter
    {
        protected Texture2D texture;
        protected Rectangle position;
        protected double speed;
        protected double health;
        protected bool player1;
        protected CharacterAttackState characterAttackState;
        protected Direction direction;
        protected int stockCount;
        protected GameManager gameManager;

        //Creates a character object with their position, textures, width, and height.
        protected CommonCharacter(Texture2D texture, int x, int y, int width, int height, bool player1, GameManager gameManager)
        {
            this.gameManager = gameManager;
            this.texture = texture;
            this.position = new Rectangle(x, y, width, height);
            this.health = gameManager.Health; //GET HEALTH HERE FROM FILE
            this.stockCount = gameManager.Stocks; //GET STOCKS FROM FILE
            this.player1 = player1;
        }
    
        //Does the attacks and updates the state based on input or game environment
        public void update(GameTime gameTime)
        {
            switch(characterAttackState)
            {
                case CharacterAttackState.Stand:
                    if(direction == Direction.Right)
                    {

                    }
                    break;
            }
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
