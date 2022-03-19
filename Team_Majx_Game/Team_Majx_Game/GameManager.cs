using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Team_Majx_Game
{
    /// <summary>
    /// This will be a singleton class that controls most of the game
    /// </summary>
    class GameManager
    {
        // Stream objects
        StreamReader input;
        StreamWriter output;

        //List of all the platforms
        public List<Tile> platforms;


        // File IO fields
        int stocks;
        double health;
        double gravity;
        double timer;
        double damage;
        double speedX;

        // properties for each IO field
        public int Stocks
        {
            get { return stocks; }
        }

        public double Health
        {
            get { return health; }
        }

        public double Gravity
        {
            get { return gravity; }
        }

        public double Timer
        {
            get { return timer; }
        }

        public double Damage
        {
            get { return damage; }
        }

        public double SpeedX
        {
            get { return speedX; }
        }

        public GameManager()
        {
            // Defualt values
            stocks = 3;
            health = 100;
            gravity = 1;
            timer = 300;
            damage = 1;
            speedX = 1;
            platforms = new List<Tile>();
            platforms.Add(new Tile(new Rectangle(720, 505, 200, 100), TileType.Platform));
        }

        // Method for writing to the settings file
        public void ReadFile(string fileName)
        {
            try
            {
                // Will read from the settings file
                input = new StreamReader("../../" + fileName);

                // Sets each of the settings accordingly
                stocks = int.Parse(input.ReadLine());
                health = double.Parse(input.ReadLine());
                gravity = double.Parse(input.ReadLine());
                timer = double.Parse(input.ReadLine());
                damage = double.Parse(input.ReadLine());
                speedX = double.Parse(input.ReadLine());

                input.Close();
            }
            catch
            {
                // Write the error here to somewhere
            }
        }


        public void SaveFile()
        {
            try
            {
                output = new StreamWriter("GameSettings.txt");

                output.WriteLine(stocks);
                output.WriteLine(health);
                output.WriteLine(gravity);
                output.WriteLine(timer);
                output.WriteLine(damage);
                output.WriteLine(speedX);

                output.Close();
            }
            catch
            {
                // Implement the catch here
            }
        }
    }
}
