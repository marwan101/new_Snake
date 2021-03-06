﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Media;
using System.IO;

namespace Snake
{


    //returns a new random position inside the console

    public class Program
    {
        //random number generator
        public static Random randomNumbersGenerator = new Random();
        //stores the position of objects on the console
        public struct Position
        {
            public int row;
            public int col;
            public Position(int row, int col)
            {
                this.row = row;
                this.col = col;
            }
        }
        static void PlayMusic(string type)
        {
            if(type == "game")
            {
                System.Media.SoundPlayer bgm = new System.Media.SoundPlayer
                {
                    SoundLocation = "../../bgm.wav"
                };
                bgm.PlayLooping();
            }
            else if(type == "main")
            {
                System.Media.SoundPlayer bgm = new System.Media.SoundPlayer
                {
                    SoundLocation = "../../menumusic.wav"
                };
                bgm.PlayLooping();
            }
        }
        public static Position newRandomPosition(int windowHeight, int windowWidth)
        {
            Position newPosition = new Position(randomNumbersGenerator.Next(2, windowHeight - 1),
                    randomNumbersGenerator.Next(2, windowWidth - 1));
            return newPosition;
        }

        //Calculate points
        public static int calculateUserPoints(Queue<Position> Snake, int pointsBehind, int specialFood)
        {
            int Points = (Snake.Count - 4) * 100 - pointsBehind;
            Points += specialFood * 300;
            if (Points < 0) Points = 0;
            Points = Math.Max(Points, 0);
            return Points;
        }


        

        static void Main(string[] args)
        {
            PlayMusic("main");
            byte right = 0;
            byte left = 1;
            byte down = 2;
            byte up = 3;
            //means the last time the snakeElement ate
            int lastFoodTime;
            //the time till the food spawns again
            int foodDissapearTime = 10000;
            int highScoreValue = highScore();
            int negativePoints = 0;
            double sleepTime = 100;
            bool gameFlag = false;
            bool menuFlag = true;
            int foodCounter = 0;
            int specialFoodCounter = 0;
            string username = "";
            int direction;
            int remainingLives;
            int prevUserPoints;
            List<string> scoreboard = new List<string>();
            int windowHeight;
            int windowWidth;



            //makes the number of rows that can be accessed on a console equal to the height of the window
            Console.BufferHeight = Console.WindowHeight;

            //store the last time the snake ate to time since the console started
            lastFoodTime = Environment.TickCount;

            //initialising important components of the game

            //array storing the direction of snake movement
            Position[] directions;
            //create Position objects and stores them in obstacles
            //These represent the obstacles on screen
            List<Position> obstacles;
            List<Position> Nobstacles;
            //Creates the snake using a queue data structure of length 3
            //Queue operates as a first-in first-out array
            Queue<Position> snakeElements;
            snakeElements = new Queue<Position>();
            //boundaries
            List<Position> leftBoundary;
            List<Position> rightBoundary;
            List<Position> bottomBoundary;

            //snake head position
            Position snakeHead;
            //snake head new position when the snake moves
            Position snakeNewHead;
            //position of the next direction the snake moves
            Position nextDirection;
            //creates the food position that is randomly generated as long as the snake has not eaten the food
            //or the food was generated in place of an obstacle
            Position food;
            //stores specialFood location
            Position specialFood;
            //stores trap location
            Position trap;
            //stores reward location
            Position reward;
            //store menu items
            List<string> menuItem;
            //selected menu item
            int index = 0;
            //User points
            int userPoints;
            //initialises variables before game starts
            initialise();

            //This method contains the menu logic which handles the whole game
            menu();

            //END OF GAME 

            //METHOD DEFINITIONS 

            

            //Draws objects on the console
            void Draw(Position pos, string drawable, ConsoleColor color = ConsoleColor.Yellow)
            {
                Console.ForegroundColor = color;
                Console.SetCursorPosition(pos.col, pos.row);
                Console.Write(drawable);
            }

            //initialisation of important variables needed to run the game
            void initialise()
            {
                windowHeight = Console.WindowHeight;
                windowWidth = Console.WindowWidth;
                remainingLives = 3;
                prevUserPoints = 0;
                direction = right;
                //array storing the direction of snake movement
                directions = new Position[]
                {
                new Position(0, 1), // right
                new Position(0, -1), // left
                new Position(1, 0), // down
                new Position(-1, 0), // up
                };

                //create Position objects and stores them in obstacles
                //These represent the obstacles on screen
                obstacles = new List<Position>()
                {
                //-1 prevents the obstacle from spawaning on the userpoints display
                    newRandomPosition(windowHeight, windowWidth),
                    newRandomPosition(windowHeight, windowWidth),
                    newRandomPosition(windowHeight, windowWidth),
                    newRandomPosition(windowHeight, windowWidth),
                    newRandomPosition(windowHeight, windowWidth),
                };

                //create Position objects and stores them in Nobstacles
                //These represent the Nobstacles on screen
                Nobstacles = new List<Position>()
                {
                //-1 prevents the Nobstacle from spawaning on the userpoints display
                    newRandomPosition(windowHeight, windowWidth),
                    newRandomPosition(windowHeight, windowWidth),
                    newRandomPosition(windowHeight, windowWidth),
                    newRandomPosition(windowHeight, windowWidth),
                    newRandomPosition(windowHeight, windowWidth),
                };

                //Creates the snake using a queue data structure of length 3
                //sets the length of snake equal to 3
                for (int i = 0; i <= 3; i++)
                {
                    snakeElements.Enqueue(new Position(1, i));
                }
                //boundary
                Boundary();

                //create food
                do
                {
                    food = newRandomPosition(windowHeight, windowWidth);
                }
                while (snakeElements.Contains(food) || obstacles.Contains(food) || Nobstacles.Contains(food));

                //create special food
                do
                {
                    specialFood = newRandomPosition(windowHeight, windowWidth);
                }
                while (snakeElements.Contains(food) || obstacles.Contains(food) || Nobstacles.Contains(food));

                //create trap
                do
                {
                    trap = newRandomPosition(windowHeight, windowWidth);
                }
                while (snakeElements.Contains(trap) || obstacles.Contains(trap) || Nobstacles.Contains(trap));
                
                //create reward
                do
                {
                    reward = newRandomPosition(windowHeight, windowWidth);
                }
                while (snakeElements.Contains(reward) || obstacles.Contains(reward) || Nobstacles.Contains(reward));
            }

            //MENU
            void menu()
            {
                //menu
                menuItem = new List<string>()
                    {
                        "Play",
                        "Username",
                        "Scoreboard",
                        "Quit"
                    };
                Console.CursorVisible = false;

                while (menuFlag)
                {
                    string selectedMenuItem = drawMenu(menuItem);
                    if (selectedMenuItem == "Play")
                    {
                        Console.Clear();
                        menuFlag = false;
                        gameFlag = true;
                        play();

                    }
                    else if (selectedMenuItem == "Username")
                    {
                        Console.Clear();
                        Position userNamePos = new Position((Console.WindowHeight - 1) / 2, ((Console.WindowWidth - 1) / 2) - 10);
                        Draw(userNamePos, "Enter your username: ", ConsoleColor.Green);
                        username = Console.ReadLine();
                    }
                    else if (selectedMenuItem == "Scoreboard")
                    {
                        Console.Clear();
                        if (scoreboard.Any())
                        {
                            for (int i = 0; i < 10 && i < scoreboard.Count; i++)
                            {
                                Position scoreboardPos = new Position((Console.WindowHeight / 2) + i, (Console.WindowWidth - 1) / 2);
                                Draw(scoreboardPos, scoreboard[i], ConsoleColor.Green);
                            }
                        }
                        else
                        {
                            Position userNamePos = new Position((Console.WindowHeight - 1) / 2, ((Console.WindowWidth - 1) / 2) - 10);
                            Draw(userNamePos, "No stored user scores...", ConsoleColor.Green);
                        }
                        Console.ReadLine();

                    }
                    else if (selectedMenuItem == "Quit")
                    {
                        Environment.Exit(0);
                    }
                }
            }
            //draws menu
            string drawMenu(List<string> items)
            {
                Console.Clear();
                //the line to draw the menu after title
                int menuLine = 0;
                Position menuPos = new Position((Console.WindowHeight - 1) / 2 + menuLine, ((Console.WindowWidth - 1) / 2));
                Draw(menuPos, "Snake Game", ConsoleColor.Green);
                menuLine++;
                Position hsPos = new Position((Console.WindowHeight - 1) / 2 + menuLine, ((Console.WindowWidth - 1) / 2));
                Draw(hsPos, $"HighScore To Beat: {highScoreValue}", ConsoleColor.Yellow);
                menuLine++;
                Console.ResetColor();
                if (username != "")
                {
                    prevUserPoints = userExists(username);
                    Draw(new Position((Console.WindowHeight - 1) / 2 + menuLine, ((Console.WindowWidth - 1) / 2)), $"Username: {username} , Previous Points: {prevUserPoints}", ConsoleColor.Green);
                    menuLine++;
                }
                for (int i = 0; i < items.Count; i++)
                {
                    Position menuItemPos = new Position((Console.WindowHeight - 1) / 2 + i + menuLine, ((Console.WindowWidth - 1) / 2));
                    if (i == index)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Draw(menuItemPos, items[i], ConsoleColor.Black);
                    }
                    else
                    {
                        Draw(menuItemPos, items[i], ConsoleColor.Gray);
                    }
                    Console.ResetColor();

                }
                ConsoleKeyInfo ckey = Console.ReadKey();
                if (ckey.Key == ConsoleKey.DownArrow)
                {
                    if (index == items.Count - 1) { index = 0; }
                    else { index++; }
                }
                else if (ckey.Key == ConsoleKey.UpArrow)
                {
                    if (index <= 0) { index = items.Count - 1; }
                    else { index--; }
                }
                else if (ckey.Key == ConsoleKey.Enter)
                {
                    return items[index];
                }
                Console.Clear();
                return "";
            }

            void Boundary()
            {
                leftBoundary = new List<Position>();
                rightBoundary = new List<Position>();
                bottomBoundary = new List<Position>();

                for (int i = 1; i < Console.WindowHeight - 1; i++)
                {
                    leftBoundary.Add(new Position(i, 0));
                }
                for (int i = 1; i < Console.WindowHeight - 1; i++)
                {
                    rightBoundary.Add(new Position(i, Console.WindowWidth - 1));
                }
                for (int i = 0; i < Console.WindowWidth - 1; i++)
                {
                    bottomBoundary.Add(new Position(Console.WindowHeight - 1, i));
                }
            }
            //main game loop
            void play()
            {
                PlayMusic("game");
                //initialises variables
                initialise();
                while (gameFlag)
                {
                    //hides cursor
                    Console.CursorVisible = false;
                    negativePoints++;
                    foreach (Position boundary in leftBoundary)
                    {
                        Draw(boundary, "▌", ConsoleColor.Green);
                    }
                    foreach (Position boundary in rightBoundary)
                    {
                        Draw(boundary, "▌", ConsoleColor.Green);
                    }
                    foreach (Position boundary in bottomBoundary)
                    {
                        Draw(boundary, "▀", ConsoleColor.Green);
                    }

                    //This draws the obstacles on the screen
                    foreach (Position obstacle in obstacles)
                    {
                        Draw(obstacle, "▒", ConsoleColor.Cyan);
                    }
                    //This draws the Nobstacles on the screen
                    foreach (Position Nobstacle in Nobstacles)
                    {
                        Draw(Nobstacle, "#", ConsoleColor.Red);
                    }
                    //Draws the snake on the console
                    foreach (Position position in snakeElements)
                    {
                        Draw(position, "*", ConsoleColor.DarkGray);
                    }
                    //Draws the food,trap,reward on the console
                    Draw(food, "♥♥", ConsoleColor.Yellow);
                    Draw(trap, "♥♥", ConsoleColor.DarkYellow);
                    Draw(reward, "♥♥", ConsoleColor.DarkYellow);
                    if (foodCounter >= 5)
                    {
                        Draw(specialFood, "&", ConsoleColor.Green);
                    }

                    //checks if user can input values through keyboard     
                    if (Console.KeyAvailable)
                    {
                        //The controls of the snake
                        ConsoleKeyInfo userInput = Console.ReadKey(true);
                        snakeMove(userInput);
                    }

                    //return the last element in the snakebody
                    snakeHead = snakeElements.Last();
                    //sets the direction the snake will move
                    nextDirection = directions[direction];

                    //changes the direction of the snakehead when the snake direction changes
                    snakeNewHead = new Position(snakeHead.row + nextDirection.row,
                        snakeHead.col + nextDirection.col);

                    //allows the snake to exit the window and enter at the opposite side               
                    snakeExitScreen();

                    //user points calculation
                    calculatePoints();

                    //displays points while playing game
                    displayPoints();

                    userLives();

                    //checks snake collision with obstacles and Nobstacles and ends the game
                    if (snakeElements.Contains(snakeNewHead) || obstacles.Contains(snakeNewHead))
                    {
                        Console.Beep(3000, 1000);
                        remainingLives--;
                        if (remainingLives < 0)
                        {
                            endGame("lose");
                            restart();
                        }
                    }
                    //checks snake collision with obstacles and Nobstacles and ends the game
                    if (Nobstacles.Contains(snakeNewHead))
                    {
                        Console.Beep(2000, 1500);
                        remainingLives = remainingLives-1;
                        if (remainingLives <= 0)
                        {
                            endGame("lose");
                            restart();
                        }
                    }

                    //winning game logic
                    if (userPoints >= 1000)
                    {
                        endGame("win");
                        restart();

                    }

                    //sets the last element in the queue to be *
                    Draw(snakeHead, "*", ConsoleColor.DarkGray);

                    //sets the snake head by adding < and changing its direction depending on the direction the snake is moving 
                    //inside the queue as first element
                    snakeElements.Enqueue(snakeNewHead);
                    //contains logic for snake movement
                    snakeMoves();



                    //game main logic//
                    //Snake eating on the food @ or is moving
                    if (snakeNewHead.col == food.col && snakeNewHead.row == food.row)
                    {
                        Console.Beep();
                        // spawns new food at a random position if the snake ate the food

                        do
                        {
                            food = newRandomPosition(windowHeight, windowWidth);
                        }
                        while (snakeElements.Contains(food) || obstacles.Contains(food) || Nobstacles.Contains(food));
                        lastFoodTime = Environment.TickCount;
                        Draw(food, "♥♥", ConsoleColor.Yellow);
                        foodCounter++;
                        sleepTime--;

                        //spawns obstacles and ensures the obstacle do not spawn on food

                        Position obstacle;
                        do
                        {
                            obstacle = newRandomPosition(windowHeight, windowWidth);
                        }
                        while (snakeElements.Contains(obstacle) || obstacles.Contains(obstacle) || (food.row != obstacle.row && food.col != obstacle.row));
                        //adds obstacle in the list of obstacles and draw the obstacle
                        obstacles.Add(obstacle);
                        Draw(obstacle, "▒", ConsoleColor.Cyan);

                        //spawns Nobstacles and ensures the Nobstacle do not spawn on food
                        Position Nobstacle;
                        do
                        {
                            Nobstacle = newRandomPosition(windowHeight, windowWidth);
                        }
                        while (snakeElements.Contains(Nobstacle) || Nobstacles.Contains(Nobstacle) || (food.row != Nobstacle.row && food.col != Nobstacle.row));
                        //adds obstacle in the list of obstacles and draw the obstacle
                        Nobstacles.Add(Nobstacle);
                        Draw(Nobstacle, "#", ConsoleColor.Red);
                    }
                    else if (snakeNewHead.col == specialFood.col && snakeNewHead.row == specialFood.row)
                    {
                        Console.Beep();
                        foodCounter = 0;
                        specialFoodCounter++;
                        do
                        {
                            specialFood = newRandomPosition(windowHeight, windowWidth);
                        } while (snakeElements.Contains(specialFood) || obstacles.Contains(specialFood) || Nobstacles.Contains(specialFood));
                        Draw(specialFood, "   ");
                    }
                    else if (snakeNewHead.col == trap.col && snakeNewHead.row == trap.row)
                    {
                        Console.Beep();
                        remainingLives = remainingLives - 1;
                        Draw(reward, "  ");
                        do
                        {
                            trap = newRandomPosition(windowHeight, windowWidth);
                            reward = newRandomPosition(windowHeight, windowWidth);
                        } while (snakeElements.Contains(trap) || obstacles.Contains(trap) || Nobstacles.Contains(trap));
                        Draw(trap, "♥♥", ConsoleColor.DarkYellow);
                        Draw(reward, "♥♥", ConsoleColor.DarkYellow);
                        if (remainingLives <= 0)
                        {
                            endGame("lose");
                            restart();
                        }
                    }
                    else if (snakeNewHead.col == reward.col && snakeNewHead.row == reward.row)
                    {
                        Console.Beep();
                        remainingLives = remainingLives + 1;
                        Draw(trap, "  ");
                        do
                        {
                            trap = newRandomPosition(windowHeight, windowWidth);
                            reward = newRandomPosition(windowHeight, windowWidth);
                        } while (snakeElements.Contains(reward) || obstacles.Contains(reward) || Nobstacles.Contains(reward));
                        Draw(trap, "♥♥", ConsoleColor.DarkYellow);
                        Draw(reward, "♥♥", ConsoleColor.DarkYellow);
                    }
                    else
                    {
                        //dequeue removes the first element added in the queue and returns it
                        //
                        // moving...
                        Position last = snakeElements.Dequeue();
                        Draw(last, " ");
                    }



                    //dispawns the food and spawns it on another place
                    //if the snake does not eat food before it despawns the user points are penalised by increasing the negative points
                    if (Environment.TickCount - lastFoodTime >= foodDissapearTime)
                    {
                        negativePoints += 10;
                        Draw(food, " ");
                        do
                        {
                            food = newRandomPosition(windowHeight, windowWidth);
                        }
                        while (snakeElements.Contains(food) || obstacles.Contains(food) || Nobstacles.Contains(food));
                        lastFoodTime = Environment.TickCount;
                    }
                    Draw(food, "♥♥", ConsoleColor.Yellow);
                    sleepTime -= 0.01;

                    Thread.Sleep((int)sleepTime);

                }
            }
            //FUNCTIONS USED IN PLAY (CONTAINS GAME LOGIC)

            //changes direction of snake
            void snakeMove(ConsoleKeyInfo key)
            {
                if (key.Key == ConsoleKey.LeftArrow)
                {
                    if (direction != right) direction = left;
                }
                if (key.Key == ConsoleKey.RightArrow)
                {
                    if (direction != left) direction = right;
                }
                if (key.Key == ConsoleKey.UpArrow)
                {
                    if (direction != down) direction = up;
                }
                if (key.Key == ConsoleKey.DownArrow)
                {
                    if (direction != up) direction = down;
                }
            }

            //moves snake logic
            void snakeMoves()
            {
                if (direction == right) Draw(snakeNewHead, ">", ConsoleColor.Gray); ;
                if (direction == left) Draw(snakeNewHead, "<", ConsoleColor.Gray); ;
                if (direction == up) Draw(snakeNewHead, "^", ConsoleColor.Gray); ;
                if (direction == down) Draw(snakeNewHead, "v", ConsoleColor.Gray); ;
            }

            //added boundary
            void snakeExitScreen()
            {
                if (snakeNewHead.col <= 1) snakeNewHead.col = Console.WindowWidth - 2;
                if (snakeNewHead.row <= 0) snakeNewHead.row = Console.WindowHeight - 2;
                if (snakeNewHead.row >= Console.WindowHeight - 1) snakeNewHead.row = 1;
                if (snakeNewHead.col >= Console.WindowWidth - 1) snakeNewHead.col = 2;
            }

            void calculatePoints()
            {
                userPoints = (snakeElements.Count - 4) * 100 - negativePoints;
                userPoints += specialFoodCounter * 300;
                if (userPoints < 0) userPoints = 0;
                userPoints = Math.Max(userPoints, 0);
            }

            //displays the user points during gameplay
            void displayPoints()
            {
                string displaypoints = $" Points:{userPoints}";
                int pos = Console.WindowWidth - displaypoints.Length;
                Position pointsPosition = new Position(0, pos);
                Draw(pointsPosition, displaypoints);
            }

            //display remaining lives during gameplay
            void userLives()
            {
                string displaylives = $" Lives:{remainingLives}";
                int pos = Console.WindowWidth - (displaylives.Length + 20);
                Position livesPosition = new Position(0, pos);
                Draw(livesPosition, displaylives);
            }


            //METHODS HANDLING END GAME

            //displays ending screen depending on outcome
            void endGame(string outcome)
            {
                if (userPoints > highScoreValue) highScoreValue = userPoints;
                string points = $"Your points are: {userPoints}";
                string highScoreString = $"High Score: {highScoreValue}";
                string congratulation = $"Congratulations {username} !!! You Win !!!";
                string lose = $"Game Over! {username}";
                Position gameOver = new Position((Console.WindowHeight - 1) / 2, ((Console.WindowWidth - 1) / 2) - points.Length / 2);
                Position pointsPos = new Position(((Console.WindowHeight - 1) / 2) + 1, ((Console.WindowWidth - 1) / 2) - points.Length / 2);
                Position scorePos = new Position(((Console.WindowHeight - 1) / 2) + 2, ((Console.WindowWidth - 1) / 2) - points.Length / 2);
                
                if (outcome == "win")
                {
                    PlayMusic("main");
                    Draw(gameOver, congratulation, ConsoleColor.Green);
                    Draw(pointsPos, points, ConsoleColor.Green);
                    Draw(scorePos, highScoreString, ConsoleColor.Green);
                    Console.WriteLine("\n");
                    storeValues(userPoints, username, highScoreValue);
                }
                else if (outcome == "lose")
                {
                    PlayMusic("main");
                    Draw(gameOver, lose, ConsoleColor.Red);
                    Draw(pointsPos, points, ConsoleColor.Red);
                    Draw(scorePos, highScoreString, ConsoleColor.Red);
                    storeValues(userPoints, username, highScoreValue);
                }

                if (username != "")
                {
                    scoreboard.Insert(0, $"Username: {username} , Score: {userPoints}");
                }
            }

            //restarts the game
            void restart()
            {
                Console.ReadKey();
                gameFlag = false;
                menuFlag = true;
                menu();
            }

            //DATA HANDLING METHODS

            //for writing users and their points earned
            void storeValues(int points, string user, int hs)
            {
                if (user.Length > 0)
                {
                    if (points > userExists(user))
                    {
                        var us = File.Open($"..\\..\\{user}.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        using (StreamWriter sw = new StreamWriter(us))
                        {
                            {
                                sw.WriteLine($"{points}");
                                sw.Close();
                            }
                        }
                    }
                    var fhs = File.Open("..\\..\\highScore.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    using (StreamWriter sw = new StreamWriter(fhs))
                    {
                        sw.WriteLine(hs);
                        sw.Close();
                    }
                }

            }
            //checks if user exists and returns their previous score or else returns 0;
            int userExists(string user)
            {
                if (File.Exists($"..\\..\\{user}.txt"))
                {
                    FileStream userfile = File.Open($"..\\..\\{user}.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    using (StreamReader sr = new StreamReader(userfile))
                    {
                        string highscore = sr.ReadLine();
                        if (highscore != null)
                        {
                            highscore.Trim();
                            sr.Close();
                            return Int32.Parse(highscore);
                        }
                        else
                        {
                            sr.Close();
                            return 0;
                        }
                    }
                }
                return 0;
            }
            //stores highscore in a text file
            int highScore()
            {
                var fs = File.Open("..\\..\\highScore.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                using (StreamReader sr = new StreamReader(fs))
                {
                    string highscore = sr.ReadLine();
                    if (highscore != null)
                    {
                        highscore.Trim();
                        sr.Close();
                        return Int32.Parse(highscore);
                    }
                    else
                    {
                        sr.Close();
                        return 0;
                    }
                }
            }
        }
    }
}
