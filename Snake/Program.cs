using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Media;
using System.IO;

namespace Snake
{
    //stores the position of objects on the console
    struct Position
    {
        public int row;
        public int col;
        public Position(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }

    class Program
    {
        static void PlayMusic()
        {
            System.Media.SoundPlayer bgm = new System.Media.SoundPlayer
            {
                SoundLocation = "../../bgm.wav"
            };
            bgm.PlayLooping();
        }
        
        static void Main(string[] args)
        {
            //PlayMusic();
            byte right = 0;
            byte left = 1;
            byte down = 2;
            byte up = 3;
            //means the last time the snakeElement ate
            int lastFoodTime;
            //the time till the food spawns again
            int foodDissapearTime = 16000;
            int userPoints;
            int negativePoints = 0;
            double sleepTime = 100;
            int direction = right;
            bool gameFlag = false;
            bool menuFlag = true;
            int foodCounter = 0;
            int specialFoodCounter = 0;

            //random number generator
            Random randomNumbersGenerator = new Random();

            //makes the number of rows that can be accessed on a console equal to the height of the window
            Console.BufferHeight = Console.WindowHeight;

            //store the last time the snake ate to time since the console started
            lastFoodTime = Environment.TickCount;

            //array storing the direction of snake movement
            Position[] directions = new Position[]
            {
                new Position(0, 1), // right
                new Position(0, -1), // left
                new Position(1, 0), // down
                new Position(-1, 0), // up
            };

            //create Position objects and stores them in obstacles
            //These represent the obstacles on screen
            List<Position> obstacles = new List<Position>()
            {
                //-1 prevents the obstacle from spawaning on the userpoints display
                newRandomPosition(),
                newRandomPosition(),
                newRandomPosition(),
                newRandomPosition(),
                newRandomPosition(),
            };
            

            //Creates the snake using a queue data structure of length 3
            //Queue operates as a first-in first-out array
            Queue<Position> snakeElements = new Queue<Position>();

            //sets the length of snake equal to 3
            for (int i = 0; i <= 3; i++)
            {
                snakeElements.Enqueue(new Position(0, i));
            }
            //snake head position
            Position snakeHead;
            //snake head new position when the snake moves
            Position snakeNewHead;
            //position of the next direction the snake moves
            Position nextDirection;
            //creates the food position that is randomly generated as long as the snake has not eaten the food
            //or the food was generated in place of an obstacle
            Position food;
            do
            {
                food = newRandomPosition();
            }
            while (snakeElements.Contains(food) || obstacles.Contains(food));

            Position specialFood;
            do
            {
                specialFood = newRandomPosition();
            }
            while (snakeElements.Contains(food) || obstacles.Contains(food));


            int index = 0;
            List<string> menuItem = new List<string>()
            {
                "Play",
                "Difficulty",
                "Quit"
            };
            Console.CursorVisible = false;
            while (menuFlag)
            {
                string selectedMenuItem = drawMenu(menuItem);
                if(selectedMenuItem == "Play")
                {
                    menuFlag = false;
                    gameFlag = true;
                    Console.Clear();
                }else if(selectedMenuItem == "Quit")
                {
                    Environment.Exit(0);
                }
            }
             
            //main game loop
            while (gameFlag)
            {
                //hides cursor
                Console.CursorVisible = false;
                negativePoints++;

                //This draws the obstacles on the screen
                foreach (Position obstacle in obstacles)
                {
                    Draw(obstacle, "=", ConsoleColor.Cyan);
                }
                //Draws the snake on the console
                foreach (Position position in snakeElements)
                {
                    Draw(position, "*", ConsoleColor.DarkGray);
                }
                //Draws the food on the console
                Draw(food, "@", ConsoleColor.Yellow);
                if(foodCounter >= 2)
                {
                    Draw(specialFood, "$", ConsoleColor.Green);
                }

                //checks if user can input values through keyboard     
                if (Console.KeyAvailable)
                {
                    //The controls of the snake
                    ConsoleKeyInfo userInput = Console.ReadKey();
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
                calculateUserPoints();

                //displays points while playing game
                displayPoints();
                
                //checks snake collision with obstacles and ends the game
                if (snakeElements.Contains(snakeNewHead) || obstacles.Contains(snakeNewHead))
                {
                    
                    endGame("lose");
                    gameFlag = false;
                }
                //winning game logic
                if (userPoints >= 500)
                {
                    endGame("win");
                    gameFlag = false;
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
                        food = newRandomPosition();
                    }
                    while (snakeElements.Contains(food) || obstacles.Contains(food));
                    lastFoodTime = Environment.TickCount;
                    Draw(food, "@", ConsoleColor.Yellow);
                    foodCounter++;
                    sleepTime--;

                    //spawns obstacles and ensures the obstacle do not spawn on food

                    Position obstacle;
                    do
                    {
                        obstacle = newRandomPosition();
                    }
                    while (snakeElements.Contains(obstacle) || obstacles.Contains(obstacle) || (food.row != obstacle.row && food.col != obstacle.row));
                    //adds obstacle in the list of obstacles and draw the obstacle
                    obstacles.Add(obstacle);
                    Draw(obstacle, "=", ConsoleColor.Cyan);
                }
                else if (snakeNewHead.col == specialFood.col && snakeNewHead.row == specialFood.row)
                {
                    Console.Beep();
                    foodCounter = 0;
                    specialFoodCounter++;
                    do
                    {
                        specialFood = newRandomPosition();
                    } while (snakeElements.Contains(specialFood) || obstacles.Contains(specialFood));
                    Draw(specialFood, " ");
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
                        food = newRandomPosition();
                    }
                    while (snakeElements.Contains(food) || obstacles.Contains(food));
                    lastFoodTime = Environment.TickCount;
                }
                Draw(food, "@", ConsoleColor.Yellow);
                sleepTime -= 0.01;

                Thread.Sleep((int)sleepTime);
              
            }
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
            //displays ending screen depending on outcome
            void endGame(string outcome)
            {
                string points = $"Your points are: {userPoints}";
                string congratulation = "Congratulations!!! You Win !!!";
                string lose = "Game Over!";
                Position gameOver = new Position((Console.WindowHeight - 1) / 2, ((Console.WindowWidth - 1) / 2) - points.Length / 2);
                Position pointsPos = new Position(((Console.WindowHeight - 1) / 2) + 1, ((Console.WindowWidth - 1) / 2) - points.Length / 2);
                
                if (outcome == "win")
                {
                    Draw(gameOver, congratulation, ConsoleColor.Green);
                    Draw(pointsPos, points, ConsoleColor.Green);
                }
                else if(outcome == "lose")
                {
                    Draw(gameOver, lose , ConsoleColor.Red);
                    Draw(pointsPos, points, ConsoleColor.Red);
                }
                
                storeValues(points);
            }
            
            //moves snake
            void snakeMoves()
            {
                if (direction == right) Draw(snakeNewHead, ">", ConsoleColor.Gray); ;
                if (direction == left) Draw(snakeNewHead, "<", ConsoleColor.Gray); ;
                if (direction == up) Draw(snakeNewHead, "^", ConsoleColor.Gray); ;
                if (direction == down) Draw(snakeNewHead, "v", ConsoleColor.Gray); ;
            }
            void snakeExitScreen()
            {
                if (snakeNewHead.col < 0) snakeNewHead.col = Console.WindowWidth - 1;
                if (snakeNewHead.row < 0) snakeNewHead.row = Console.WindowHeight - 1;
                if (snakeNewHead.row >= Console.WindowHeight) snakeNewHead.row = 0;
                if (snakeNewHead.col >= Console.WindowWidth) snakeNewHead.col = 0;
            }
            void calculateUserPoints()
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
            //returns a new random position inside the console
            Position newRandomPosition()
            {
                Position newPosition = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight),
                        randomNumbersGenerator.Next(2, Console.WindowWidth));
                return newPosition;
            }
            void Draw(Position pos, string drawable, ConsoleColor color = ConsoleColor.Yellow)
            {
                Console.ForegroundColor = color;
                Console.SetCursorPosition(pos.col, pos.row);
                Console.Write(drawable);
            }
            //draws menu
            string drawMenu(List<string> items)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Snake Game");
                Console.ResetColor();
                for (int i = 0; i < items.Count; i++)
                {
                    if (i == index)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine(items[i]);
                    }
                    else
                    {
                        Console.WriteLine(items[i]);
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
                else if(ckey.Key == ConsoleKey.Enter)
                {
                    return items[index];
                }
                Console.Clear();
                return "";
            }
            void storeValues(string points)
            {
                Console.ReadLine();
                using (StreamWriter sw = File.CreateText("..\\..\\user.txt"))
                {
                    sw.WriteLine(points);
                }
            }
        }
    }
}
