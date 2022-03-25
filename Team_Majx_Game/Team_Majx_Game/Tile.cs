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
        Wall,
        StartingSpawnPoint,
        RandomSpawnPoint,
        Air,
        Death
    }

    class Tile
    {
        // tile fields
        private Rectangle position;
        private TileType tileType;

        // parameterized constructor
        public Tile(Rectangle position, TileType tileType)
        {
            this.position = position;
            this.tileType = tileType;
        }

        // property
        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }

        // enum property
        public TileType TileType
        {
            get { return tileType; }
            set { tileType = value; ; }
        }


    }
}
