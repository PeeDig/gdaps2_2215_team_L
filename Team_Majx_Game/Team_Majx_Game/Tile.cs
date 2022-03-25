using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Team_Majx_Game
{

    enum TileType
    {
        Platform,
        Wall
    }

    class Tile
    {
        private Rectangle position;
        private TileType tileType;


        public Tile(Rectangle position, TileType tileType)
        {
            this.position = position;
            this.tileType = tileType;
        }

        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }

        public TileType TileType
        {
            get { return tileType; }
            set { tileType = value; ; }
        }


    }
}
