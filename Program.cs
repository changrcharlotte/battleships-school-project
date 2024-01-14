using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace battleships
{

    class Program
    {
        const char SEA = '~';
        const int GRIDLENGTH = 9;
        const int GRIDWIDTH = 9;
        const string FILENAME = "battleships.txt";

        //data
        static void saveProgress(char[,] boatGrid, char[,] CompBoatGrid, char[,] UserTargetTracker, char[,] compTargetTracker, int playersHits, int compsHits)
        {
            
            char[][,] boats = { boatGrid, CompBoatGrid, UserTargetTracker, compTargetTracker };
            using (StreamWriter sw = new StreamWriter(FILENAME))
            {
                foreach (var item in boats)
                {
                    for (int i = 0; i < GRIDLENGTH; i++)
                    {
                        for (int j = 0; j < GRIDWIDTH; j++)
                        {
                            sw.Write(item[i, j]);

                            if (j < GRIDLENGTH-1)
                            {
                                sw.Write(",");
                            }
                        }


                        sw.WriteLine();
                    }

                }
                sw.WriteLine("PH:" + playersHits);
                sw.WriteLine("CH:" + compsHits);
            }
            

        }
        static void resumeGame()
        {
            List<string[]> splittedList = new List<string[]>();

            string[] list = File.ReadAllLines(FILENAME);//each item in the list is one line

            foreach (string s in list)
            {
                splittedList.Add(s.Split(','));//each item in the list should be one char
            }
            
            char[,] boatGrid = new char[GRIDLENGTH, GRIDLENGTH];
            char[,] CompBoatGrid = new char[GRIDLENGTH, GRIDLENGTH];
            char[,] UserTargetTracker = new char[GRIDLENGTH, GRIDLENGTH];
            char[,] compTargetTracker = new char[GRIDLENGTH, GRIDLENGTH];

            int row1 = 0;
            for(row1 = 0; row1<GRIDLENGTH; row1++)
            {
                for(int col = 0; col< GRIDWIDTH; col ++)
                {
                    char container = Convert.ToChar(splittedList[row1][col]);
                    boatGrid[row1, col] = container;
                }
            }

            for (int row2 = GRIDLENGTH; row2 < GRIDLENGTH*2; row2++)
            {
                for (int col = 0; col < GRIDWIDTH; col++)
                {
                    char container = Convert.ToChar(splittedList[row2][col]);
                    CompBoatGrid[row2-GRIDLENGTH, col] = container;
                }
            }

            for (int row3 = GRIDLENGTH*2; row3 < GRIDLENGTH *3; row3++)
            {
                for (int col = 0; col < GRIDWIDTH; col++)
                {
                    char container = Convert.ToChar(splittedList[row3][col]);
                    UserTargetTracker[row3-(GRIDLENGTH*2), col] = container;
                }
            }

            for (int row4 = GRIDLENGTH * 3; row4 < GRIDLENGTH * 4; row4++)
            {
                for (int col = 0; col < GRIDWIDTH; col++)
                {
                    char container = Convert.ToChar(splittedList[row4][col]);
                    compTargetTracker[row4 - (GRIDLENGTH * 3), col] = container;
                }
            }
            string pContainer = list[36];
            string cContainer = list[37];
            int playersHits = Convert.ToInt32(pContainer.Substring(3,1));
            int compsHits = Convert.ToInt32(cContainer.Substring(3,1));

                Console.WriteLine("This is your fleet grid from where you left off");
            displayGrid(boatGrid);

            Console.WriteLine("testing: here is the computer's grid");
            displayGrid(CompBoatGrid);

            Console.WriteLine("This is your target tracker from where you left off: ");
            displayGrid(UserTargetTracker);


            playGame(boatGrid, CompBoatGrid, UserTargetTracker, compTargetTracker, playersHits, compsHits);

        }
        static void newGame()
        {
            char[,] boatGrid = setUpGrid(false);
            char[,] CompBoatGrid = setUpGrid(false);
            char[,] UserTargetTracker = setUpGrid(true);
            char[,] compTargetTracker = setUpGrid(true);

            makeCompFleet(CompBoatGrid);

            //Console.WriteLine("testing: here is the computer's grid");
            //displayGrid(CompBoatGrid);


            Console.WriteLine("Here is your fleet grid:");
            displayGrid(boatGrid);

            makeFleet(boatGrid);



            Console.WriteLine("Here is your target Tracker: ");
            displayGrid(UserTargetTracker);

            playGame(boatGrid, CompBoatGrid, UserTargetTracker, compTargetTracker,0,0);
           

        }

        static void makeFleet(char[,] Usergrid)
        {
            Console.WriteLine("First you will have to make your fleet. You will have 5 boats. Please enter the coordinates of the upper leftmost part of your boat");

            string[] boatTypes = { "destroyer", "destroyer", "submarine", "submarine", "carrier" };
            for (int count = 0; count < 5; count++)
            {
                Console.WriteLine("which column would you like to put your " + boatTypes[count] + ", please enter a letter");
                string inputString = Console.ReadLine();
                if (string.IsNullOrEmpty(inputString) == false && inputString.Length == 1)
                
                {
                    char container1 = Char.ToUpper(Convert.ToChar(inputString));
                    if (char.IsDigit(container1) == false)
                    {
                        int col = Convert.ToInt32(container1) - 64;

                        Console.WriteLine("which row would you like to put your " + boatTypes[count] + ", please enter a number");
                        string text = Console.ReadLine();
                        if (int.TryParse(text, out int row))
                        {

                            if (Usergrid[row, col] == 'd' || Usergrid[row, col] == 's' || Usergrid[row, col] == 'c')
                            {
                                Console.WriteLine("There is already a boat here.Try again");
                                count--;
                            }
                            else
                            {
                                if (count == 0 || count == 1)//destroyer
                                {
                                    Usergrid[row, col] = 'd';
                                    displayGrid(Usergrid);
                                }
                                else if (count == 2 || count == 3)//submarine
                                {
                                    Console.WriteLine("What orientation would you like your" + boatTypes[count] + " ? type in (1) for horizontal and (2) for vertical");
                                    if (int.TryParse(Console.ReadLine(), out int orientation))
                                    {
                                        if ((orientation ==1 & col == 8) || (orientation ==2 && row == 8)){
                                            Console.WriteLine("invalid,out of range,  try again");
                                            count--;
  
                                        }
                                        else
                                        {
                                            if (orientation == 1 && Usergrid[row, col + 1] != 'd' && Usergrid[row, col + 1] != 's' && Usergrid[row, col + 1] != 'c')
                                            {
                                                Usergrid[row, col] = 's';
                                                Usergrid[row, col + 1] = 's';
                                            }
                                            else if (orientation == 2 && Usergrid[row + 1, col] != 'd' && Usergrid[row + 1, col] != 's' && Usergrid[row + 1, col] != 'c')
                                            {
                                                Usergrid[row, col] = 's';
                                                Usergrid[row + 1, col] = 's';
                                            }
                                            else
                                            {
                                                Console.WriteLine("This option is unavailable. Try again");
                                                count--;
                                            }
                                        }
                                        
                                        displayGrid(Usergrid);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Try again");
                                        count--;
                                    }

                                }
                                else if (count == 4)//carrier
                                {
                                    Console.WriteLine("What orientation would you like your" + boatTypes[count] + " ? type in (1) for horizontal and (2) for vertical");
                                    if (int.TryParse(Console.ReadLine(), out int orientation))
                                    {
                                        if (((container1 == 'G' || container1 == 'H') && orientation == 1) || ((row == 7 || row == 8) && orientation == 2))
                                        {
                                            Console.WriteLine("This option is off the grid. Try again");
                                            count--;
                                        }
                                        else if (orientation == 1 && Usergrid[row, col + 1] != 'd' && Usergrid[row, col + 1] != 's' && Usergrid[row, col + 1] != 'c' && Usergrid[row, col + 2] != 'd' && Usergrid[row, col + 2] != 's' && Usergrid[row, col + 2] != 'c')
                                        {
                                            Usergrid[row, col] = 'c';
                                            Usergrid[row, col + 1] = 'c';
                                            Usergrid[row, col + 2] = 'c';
                                        }
                                        else if (orientation == 2 && Usergrid[row + 1, col] != 'd' && Usergrid[row + 1, col] != 's' && Usergrid[row + 1, col] != 'c' && Usergrid[row + 2, col] != 'd' && Usergrid[row + 2, col] != 's' && Usergrid[row + 2, col] != 'c')
                                        {
                                            Usergrid[row, col] = 'c';
                                            Usergrid[row + 1, col] = 'c';
                                            Usergrid[row + 2, col] = 'c';
                                        }
                                        else
                                        {
                                            Console.WriteLine("This option is unavailable. Try again");
                                            count--;
                                        }
                                        displayGrid(Usergrid);
                                    }
                                    else
                                    {
                                        Console.WriteLine("wrong format. Try again");
                                        count--;
                                    }

                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Incorrect format, try again");
                            count--;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incorrect format, try again");
                        count--;
                    }
                }

                else
                {
                    Console.WriteLine("Incorrect format try again");
                    count--;
                }
                
            }
        }
        static void makeCompFleet(char[,] compGrid)
        {

            for (int i = 0; i < 2; i++)//destroyers
            {
                Random random = new Random();
                int row = random.Next(1, 9);
                int col = random.Next(1, 9);
                if (compGrid[row, col] != 'd' && compGrid[row, col] != 's' && compGrid[row, col] != 'c')
                {
                    compGrid[row, col] = 'd';
                }
                else
                {
                    i--;
                }
            }

            for (int i = 0; i < 2; i++)//submarines
            {
                Random random = new Random();
                int row = random.Next(1, 9);
                int col = random.Next(1, 9);

                int orientation = random.Next(1, 3);
                if (compGrid[row, col] != 'd' && compGrid[row, col] != 's' && compGrid[row, col] != 'c')
                {
                    
                    if ((col ==8 && orientation == 1) || (row ==8 && orientation == 2))
                    {
                        i--;
                    }
                    else if (orientation == 1 && compGrid[row, col+1] != 'd' && compGrid[row, col+1] != 's' && compGrid[row, col+1] != 'c')//horizontal
                    {
                        compGrid[row, col] = 's';
                        compGrid[row, col + 1] = 's';
                    }
                    else if (orientation == 2 && compGrid[row+1, col] != 'd' && compGrid[row+1, col] != 's' && compGrid[row+1, col] != 'c')//vertical
                    {
                        compGrid[row, col] = 's';
                        compGrid[row + 1, col] = 's';
                    }
                    else
                    {
                        i--;
                    }

                }
                else
                {
                    i--;
                }
            }

            bool run = true;
            while (run) //carrier
            {
                Random random = new Random();
                int row = random.Next(1, 9);
                int col = random.Next(1, 9);
                int orientation = random.Next(1, 3);

                char givenCoordinate = compGrid[row, col];
                if ((compGrid[row, col] != 'd' && compGrid[row, col] != 's' && compGrid[row, col] != 'c'))
                {
                    if ((col > 6 && orientation == 1) || row > 6 && orientation == 2)
                    {
                        //exception capturing(will run code again)
                    }
                    else if (orientation == 1 && compGrid[row, col + 1] != 'd' && compGrid[row, col + 1] != 's' && compGrid[row, col + 1] != 'c' && compGrid[row, col + 2] != 'd' && compGrid[row, col + 2] != 's' && compGrid[row, col + 2] != 'c')//horizontal
                    {
                        compGrid[row, col] = 'c';
                        compGrid[row, col + 1] = 'c';
                        compGrid[row, col + 2] = 'c';
                        run = false;
                    }
                    else if (orientation == 2 && compGrid[row + 1, col] != 'd' && compGrid[row + 1, col] != 's' && compGrid[row + 1, col] != 'c' && compGrid[row + 2, col] != 'd' && compGrid[row + 2, col] != 's' && compGrid[row + 2, col] != 'c')//vertical
                    {
                        compGrid[row, col] = 'c';
                        compGrid[row + 1, col] = 'c';
                        compGrid[row + 2, col] = 'c';
                        run = false;
                    }
                }


            }

        }

        //play game
        static void playGame(char[,] boatGrid, char[,] CompBoatGrid, char[,] UserTargetTracker, char[,] compTargetTracker, int playersHits, int compsHits)
        {
            Console.WriteLine("starting game....");
            if(playersHits == 9 || compsHits == 9)
            {
                Console.WriteLine("The game has already been won!");
            }
            while (playersHits < 9 & compsHits < 9)
            {
                playersTurn(UserTargetTracker, CompBoatGrid, ref playersHits);
                checkIfSunk(UserTargetTracker, CompBoatGrid);
                Console.WriteLine("target Tracker: ");
                displayGrid(UserTargetTracker);
                saveProgress(boatGrid, CompBoatGrid, UserTargetTracker, compTargetTracker, playersHits, compsHits);

                if (playersHits == 9)
                {
                    Console.WriteLine("Congratulations, You have won!");

                }
                else
                {

                    CompsTurn(boatGrid, compTargetTracker, ref compsHits);
                    saveProgress(boatGrid, CompBoatGrid, UserTargetTracker, compTargetTracker, playersHits, compsHits);

                    if (compsHits >= 9)
                    {
                        Console.WriteLine("You lost, the computer has won!");
                    }
                }
            }
        }


        static void CompsTurn(char[,] fleetGrid, char[,] compTracker, ref int compHits)
        {
            Console.WriteLine("Computer's turn:");
            Console.WriteLine("Firing....");
            bool go = true;
            while (go)
            {
                Random random = new Random();
                int x = random.Next(1, 9);
                int y = random.Next(1, 9);
                if (compTracker[x, y] != 'H' & compTracker[x, y] != 'M')
                {
                    if (fleetGrid[y, x] == 'd' || fleetGrid[y,x] == 's' || fleetGrid[y,x] == 'c')
                    {
                        fleetGrid[y, x] = 'X';
                        compTracker[y, x] = 'H';
                        compHits++;
                    }
                    else
                    {
                        compTracker[y,x] = 'M';
                    }
                    go = false;
                }
            }
            Console.WriteLine("Your updated fleet grid:");
            displayGrid(fleetGrid);


        }
        static void playersTurn(char[,] UserTarget, char[,] CompGrid, ref int playerHits)
        {
            bool go = true;
            while (go)
            {
                Console.WriteLine("Your turn:");
                Console.WriteLine("What is the letter corresponding to your target coordinate?");
                char container1 = Char.ToUpper(Convert.ToChar(Console.ReadLine()));
                int col = Convert.ToInt32(container1) - 64;
                Console.WriteLine("What is the number corresponding to your target coordinate");
                int row = Convert.ToInt32(Console.ReadLine());
                if (UserTarget[row,col] == 'H' || UserTarget[row, col] == 'M')//checks the user hasn't written the same thing in already
                {
                    Console.WriteLine("Try again");
                }
                else
                {
                    if (CompGrid[row, col] == 'd' || CompGrid[row, col] == 's' || CompGrid[row, col] == 'c')
                    {
                        UserTarget[row, col] = 'H';
                        playerHits++;
                    }
                    else
                    {
                        UserTarget[row, col] = 'M';
                    }
                    go = false;
                }
            }
            
        }



       


        //irelevant
        static void displayGrid(char[,] myGrid)
        {
            for (int i = 0; i < GRIDLENGTH; i++)
            {
                for (int j = 0; j < GRIDWIDTH; j++)
                {
                    Console.Write(myGrid[i, j]);
                }
                Console.WriteLine("");
            }
        }
        static void checkIfSunk(char[,] UserTarget, char[,] compGrid)
        {
            for(int row =  0; row < GRIDLENGTH; row++)
            {
                for(int col =0; col < GRIDWIDTH; col++)
                {

                    if (UserTarget[row, col] == 'H' && compGrid[row, col] == 'd')
                    {
                        UserTarget[row, col] = 'S';
                    }
                }
            }//destroyer
            for (int row = 0; row < GRIDLENGTH - 1; row++)//submarines
            {
                for (int col = 0; col < GRIDWIDTH - 1; col++)
                {
                    if (UserTarget[row, col + 1] == 'H' && UserTarget[row, col] == 'H' && compGrid[row, col] == 's' && compGrid[row, col + 1] == 's')
                    {
                        UserTarget[row, col] = 'S';
                        UserTarget[row, col + 1] = 'S';

                    }
                    else if (UserTarget[row + 1, col] == 'H' && UserTarget[row, col] == 'H' && compGrid[row, col] == 's' && compGrid[row + 1, col] == 's')
                    {
                        UserTarget[row, col] = 'S';
                        UserTarget[row + 1, col] = 'S';

                    }
                }

            }//submarines

            for (int row = 0; row < GRIDLENGTH-2; row++)
            {
                for (int col = 0; col < GRIDWIDTH-2; col++)
                {
                    if (UserTarget[row, col + 1] == 'H' && UserTarget[row, col] == 'H' && UserTarget[row, col + 2] == 'H' && compGrid[row, col] == 'c' && compGrid[row, col + 1] == 'c' && compGrid[row, col + 2] == 'c')
                    {
                        UserTarget[row, col] = 'S';
                        UserTarget[row, col + 1] = 'S';
                        UserTarget[row, col + 2] = 'S';
                    }
                    else if (UserTarget[row + 1, col] == 'H' && UserTarget[row, col] == 'H' && UserTarget[row + 2, col] == 'H' && compGrid[row, col] == 'c' && compGrid[row + 1, col] == 'c' && compGrid[row + 2, col] == 'c')
                    {
                        UserTarget[row, col] = 'S';
                        UserTarget[row + 1, col] = 'S';
                        UserTarget[row + 2, col] = 'S';
                    }

                }
            }//carrier


        }
        static char[,] setUpGrid(bool tracker)
        {
            char[,] grid = new char[GRIDLENGTH, GRIDLENGTH];//made through row, column logic

            int row = 0;
            int col = 0;
            for (row = 0; row < GRIDLENGTH; row++)
            {
                for (col = 0; col < GRIDWIDTH; col++)
                {
                    if (row == 0 & col == 0)//if this is in the upmost left corner, it can be disregarded
                    {
                        grid[row, col] = 'x';
                    }
                    else if (row == 0)
                    {
                        int letter1 = col + 64;
                        char letter = (Char)letter1;
                        grid[row, col] = letter;
                    }
                    else if (col == 0)//on the left side, it must be a number?
                    {
                        //im trying to convert an int to a char in a way which it can fit on the grid..
                        grid[row, col] = Convert.ToChar(Convert.ToString(row));
                    }
                    else
                    {
                        if (tracker)
                        {
                            grid[row, col] = '-';
                        }
                        else
                        {
                            grid[row, col] = SEA;
                        }


                    }

                }

            }
            return grid;

        }

        static void readInstructions()
        {
            Console.WriteLine("Battle boats instructions:");
            Console.WriteLine("");
            Console.WriteLine("Battle boats is a board game where each player receives an 8 by 8 grid in which they \nposition boats and strategically guess the coordinates of your enemy’s boats in an attempt to \nsink them. Whichever player sinks all of the enemy’s boats first is the winner. In this \nadapted computer console version, you will be playing against the computer.");
            Console.WriteLine("");
            Console.WriteLine("You will receive a fleet grid described by a letter on the x axis and a number on the y axis \nwhere you will be prompted to enter the uppermost left coordinates of where you would like to \nposition each of your  5 boats. You are given 2 destroyers, which are one cell boats,\n2 Submarines, which are two cell boats and a single carrier which is a three cell boat. You will \nalso be prompted whether you would like your boat placed horizontally or vertically \nrelative to the front of the boat. Once you have created your fleet, the game will begin.");
            Console.WriteLine("");
            Console.WriteLine("You and the computer will take turns guessing the coordinates of each other’s boats. If you \ncorrectly guess the coordinates of one of the computer’s fleet during your turn , a ‘H’ will \nappear on your target tracker, signifying ‘Hit’. Otherwise, there will be a ‘M’, signifying \n'Miss'. During the computer’s turn, if you receive a ‘X’ mark in the place of one of your \nboats on your fleet grid, The computer has hit one of your part of your ship, but it is \nnot sunk untill all parts of your ship have been hit. When a ship has been sunk, an row of\nS's will replace the H's on your target tracker.The game will continue alternating between turns\nuntil either you or the computer have no ships left and there will be a winner. ");
            Console.WriteLine("");
            Console.WriteLine("Every turn, progress is saved externally. When ‘resume game’ is selected, progress will be \npresented from the player’s previous game and will continue from that point.");
            Console.WriteLine("");
        }
        static void Main(string[] args)
        {
            bool go = true;
            while (go)
            {
                
                Console.WriteLine("Would you like to:");
                Console.WriteLine("(1). play a new game");
                Console.WriteLine("(2). Resume your game");
                Console.WriteLine("(3). Read instructions");
                Console.WriteLine("(4). Quit the game");
                int choice;
                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    if (choice == 1)
                    {
                        newGame();
                    }
                    else if (choice == 2)
                    {
                        resumeGame();
                    }
                    else if (choice == 3)
                    {
                        readInstructions();
                    }
                    else if (choice == 4)
                    {
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("That is not an option. Try again");
                    }
                }
                else
                {
                    Console.WriteLine("That is not an option. Try again");
                }
            }


        }

    }
}
