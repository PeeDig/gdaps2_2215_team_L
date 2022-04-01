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

        private Tile[,] mapArray;
        private List<Tile> randomSpawnList;
        private List<Tile> spawnPoints;

        // File IO fields
        private int stocks;
        private double health;
        private double gravity;
        private double timer;
        private double damage;
        private double speedX;

        private int size = 32 + 32/2;


        // Level fields
        int levelWidth;
        int levelHeight;
        string boardLine;

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

        // allows Game1 to draw the array
        public Tile[,] MapArray
        {
            get { return mapArray; }
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
            randomSpawnList = new List<Tile>();
            platforms = new List<Tile>();
            spawnPoints = new List<Tile>();
            platforms.Add(new Tile(new Rectangle(720, 505, 400, 100), TileType.Platform));
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

        // Reads in the files
        public void ReadLevelFile(string filename)
        {
            try
            {
                input = new StreamReader("../../../" + filename);

                // Reads the size of the map
                string line = input.ReadLine();
                string[] data = line.Split(',');
                levelWidth = int.Parse(data[0]);
                levelHeight = int.Parse(data[1]);

                // creates the array to hold all of the tiles
                mapArray = new Tile[levelHeight, levelWidth];

                for(int r = 0; r < levelHeight; r++)
                {
                    // gets a line ready to be read in the for loop
                    
                    boardLine = input.ReadLine();
                    char[] boardCode = boardLine.ToCharArray();
                    

                    for (int c = 0; c < levelWidth; c++)
                    {
                        
                        // This if else chuck will create the specific tile,
                        // with the specific tiletype depending on what it reads from the file
                        if (boardCode[c] == '1')
                        {
                            mapArray[r, c] = new Tile(new Rectangle(c * 32 - 7, r * 32 - 7, size, size), TileType.Wall);
                        }                                                   
                        else if (boardCode[c] == 'z')                       
                        {                                                   
                            mapArray[r, c] = new Tile(new Rectangle(c * 32 - 7, r * 32 - 7, size, size), TileType.Platform);
                            platforms.Add(mapArray[r, c]);
                        }                                                   
                        else if (boardCode[c] == 'S')                       
                        {                                                   
                            mapArray[r, c] = new Tile(new Rectangle(c * 32 - 7, r * 32 - 7, size, size), TileType.StartingSpawnPoint);
                            spawnPoints.Add(mapArray[r, c]);
                        }                                                   
                        else if (boardCode[c] == 'R')                       
                        {                                                   
                            mapArray[r, c] = new Tile(new Rectangle(c * 32 - 7, r * 32 - 7, size, size), TileType.RandomSpawnPoint);
                            randomSpawnList.Add(mapArray[r, c]);
                        }                                                   
                        else if (boardCode[c] == 'M')                       
                        {                                                   
                            mapArray[r, c] = new Tile(new Rectangle(c * 32 - 7, r * 32 - 7, size, size), TileType.Death);
                        }                                                   
                        else                                                
                        {                                                   
                            mapArray[r, c] = new Tile(new Rectangle(c * 32 - 7, r * 32 - 7, size, size), TileType.Air);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Something went wrong: " + e.Message);
            }
        }
    }
}
