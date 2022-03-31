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

        // draws the correct block
        public void Draw(SpriteBatch spriteBatch, Texture2D tempSquare)
        {
            switch (tileType)
            {
                case TileType.Platform:
                    spriteBatch.Draw(tempSquare, position, Color.Red);
                    break;

                case TileType.Wall:
                    spriteBatch.Draw(tempSquare, position, Color.Blue);
                    break;

                case TileType.StartingSpawnPoint:
                    spriteBatch.Draw(tempSquare, position, Color.Green);
                    break;

                case TileType.RandomSpawnPoint:
                    spriteBatch.Draw(tempSquare, position, Color.Yellow);
                    break;

                case TileType.Air:
                    spriteBatch.Draw(tempSquare, position, Color.LightBlue);
                    break;

                case TileType.Death:
                    spriteBatch.Draw(tempSquare, position, Color.Orange);
                    break;
            }
        }
    }
}
