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

        // Next two properties allow for other classes
        // to access the character spawn points
        public List<Tile> RandomSpawnPoints
        {
            get { return randomSpawnList; }
        }

        public List<Tile> SpawnPoints
        {
            get { return spawnPoints; }
        }

        // will be used for setting the screen size
        public int ScreenHeight
        {
            get { return levelHeight * 32; }
        }

        public int ScreenWidth
        {
            get { return levelWidth * 32; }
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
            // platforms.Add(new Tile(new Rectangle(720, 505, 400, 100), TileType.Platform));
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
                input = new StreamReader("../../../" + filename); //+ filename);

                // Reads the size of the map
                string line = input.ReadLine();
                string[] data = line.Split(',');
                levelWidth = int.Parse(data[0]);
                levelHeight = int.Parse(data[1]);

                // reset the map array
                mapArray = null;
                platforms.Clear();
                randomSpawnList.Clear();
                spawnPoints.Clear();

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
                        if (boardCode[c] == '1') // wall tile
                        {
                            mapArray[r, c] = new Tile( // creates a new tile
                                new Rectangle // makes a new rectangle for each
                                (c * 32 - 7, // puts it at the correct x location
                                r * 32 - 7, // puts it at the correct y location
                                size, // specified x size in a var.
                                size) // specified y size in a var.
                                , TileType.Wall); // makes it a wall tile

                            platforms.Add(mapArray[r, c]);
                        }                                                   
                        else if (boardCode[c] == 'z') // platform                    
                        {                                                   
                            mapArray[r, c] = new Tile(new Rectangle(c * 32 - 7, r * 32 - 7, size, size), TileType.Platform);
                            platforms.Add(mapArray[r, c]);
                        }                                                   
                        else if (boardCode[c] == 'S') // original spawn point                      
                        {                                                   
                            mapArray[r, c] = new Tile(new Rectangle(c * 32 - 7, r * 32 - 7, size, size), TileType.StartingSpawnPoint);
                            spawnPoints.Add(mapArray[r, c]);
                        }                                                   
                        else if (boardCode[c] == 'R') // random spawn point A.D                   
                        {                                                   
                            mapArray[r, c] = new Tile(new Rectangle(c * 32 - 7, r * 32 - 7, size, size), TileType.RandomSpawnPoint);
                            randomSpawnList.Add(mapArray[r, c]);
                        }                                                   
                        else if (boardCode[c] == 'M') // death floor                   
                        {                                                   
                            mapArray[r, c] = new Tile(new Rectangle(c * 32 - 7, r * 32 - 7, size, size), TileType.Death);
                        }                                                   
                        else // air                                                
                        {                                                   
                            mapArray[r, c] = new Tile(new Rectangle(c * 32 - 7, r * 32 - 7, size, size), TileType.Air);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                // i dont know what to put here...
                Console.WriteLine("Something went wrong: " + e.Message); 
                // this doesnt prints to console, but monogame....
            }
        }
    }
}
