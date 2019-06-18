using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                int width;
                int height;
                int borderWidth = 1;
                int delay;

                int minNeighbour;
                int maxNeighbour;
                int reqNeighbour;

                char livingCell = 'X';
                char deadCell = ' ';

                Console.Write("Board widht:");
                width = Convert.ToInt32(Console.ReadLine());
                Console.Write("Board height:");
                height = Convert.ToInt32(Console.ReadLine());
                Console.Write("Delay(in ms):");
                delay = Convert.ToInt32(Console.ReadLine());

                Console.Write("Minimum neighbour:");
                minNeighbour = Convert.ToInt32(Console.ReadLine());
                Console.Write("Maximum neighbour:");
                maxNeighbour = Convert.ToInt32(Console.ReadLine());
                Console.Write("Neighbour(s) required to reproduce:");
                reqNeighbour = Convert.ToInt32(Console.ReadLine());

                Console.Clear();

                GameOfLife.CreateInstance(width, height, borderWidth, ConsoleColor.Black, ConsoleColor.DarkGreen, livingCell, deadCell, delay, minNeighbour, maxNeighbour, reqNeighbour);
                GameOfLife.StartGame();

                //Population is stagnant / died
                Console.WriteLine("Press any key to restart");
                Console.ReadLine();
                Console.Clear();

                
                
            }
        }
    }

    static class GameOfLife
    {
        /*
         * FIELD & PROPERTIES
         */
        public static int Width { get; set; } //Console Width
        public static int Height { get; set; } //Console Height

        public static int BorderWidth { get; set; } //Border

        /* Colour */
        public static ConsoleColor BackgroundColour { get; set; }

        public static ConsoleColor LivingCellColour { get; set; }
        public static ConsoleColor DeadCellColour { get; set; }

        /* Cell */
        public static char LivingCell { get; set; }
        public static char DeadCell { get; set; }

        /* Arrays */
        static char[,] current; // Current state of the game
        static char[,] next; // Next state of the game

        /* Speed */
        public static int Delay { get; set; }

        /* Evolution Rules */
        public static int MinNeighbour { get; set; }
        public static int MaxNeighbour { get; set; }
        public static int NeighbourReqToReproduce { get; set; }

        /* Manual Pointer*/
        public static char ManualPointer { get; set; }
        public static ConsoleColor PointerColour { get; set; }
        public static ConsoleColor PointerColour2 { get; set; }
        /*
         * END OF FIELD AND PROPERTIES
         */


        /* Constructor */
        public static void CreateInstance(int width, int height, int borderWidth,
            ConsoleColor backgroundColour, ConsoleColor livingColour,
            char livingCell, char deadCell,
            int delay,
            int minNeighbour, int maxNeighbour, int reqNeighbour)
        {
            Width = width;
            Height = height;
            BorderWidth = borderWidth;
            BackgroundColour = backgroundColour;
            LivingCellColour = livingColour;
            LivingCell = livingCell;
            DeadCell = deadCell;
            Delay = delay;
            MinNeighbour = minNeighbour;
            MaxNeighbour = maxNeighbour;
            NeighbourReqToReproduce = reqNeighbour;

            /* Console Setup*/
            Console.CursorVisible = false;
            Console.SetWindowSize(Width, Height);
            Console.SetBufferSize(Width, Height);
            Console.BackgroundColor = BackgroundColour;
            Console.ForegroundColor = LivingCellColour;

            ManualPointer = 'X';
            PointerColour = ConsoleColor.DarkGreen;
            PointerColour2 = ConsoleColor.DarkRed;

            /* 2D arrays*/
            current = new char[Width, Height];
            next = new char[Width, Height];

            /* Initialise Dead cell */
            for (int x = BorderWidth; x < Width - BorderWidth; x++)
            {
                for (int y = BorderWidth; y < Height - BorderWidth; y++)
                {
                    current[x, y] = DeadCell;
                }
            }
        }


        /* Method to Start Game */
        public static void StartGame()
        {
            int livingNeighbourCount;

            /* Choose pattern */
            Console.WriteLine("1.Random" +
                    "\n2.Box" +
                    "\n3.Beacon" +
                    "\n4.Glider" +
                    "\n5.Pulsar" +
                    "\n6.MANUAL");

            Console.Write("Choose your pattern: ");

            switch (Convert.ToInt32(Console.ReadLine()))
            {
                case 1:
                    InitialiseRandomLive();
                    break;
                case 2:
                    InitialiseBox();
                    break;
                case 3:
                    InitialiseBeacon();
                    break;
                case 4:
                    InitialiseGlider();
                    break;
                case 5:
                    InitialisePulsar();
                    break;
                case 6:
                    Console.Clear();
                    GetDrawing();
                    break;
            }

            Console.Clear(); //Clear the menu


            while (true)
            {
                Array.Copy(current, next, Width * Height); // Copy current array to next array

                for (int x = BorderWidth; x < Width - BorderWidth; x++)
                {
                    for (int y = BorderWidth; y < Height - BorderWidth; y++)
                    {
                        /* Display current array */
                        Console.SetCursorPosition(x, y);
                        Console.ForegroundColor = LivingCellColour;
                        Console.WriteLine(current[x, y]);

                        /*Count the neighbour*/
                        livingNeighbourCount = CountNeighbour(x, y);

                        //Judging whether this cell will alive for the next round
                        if (current[x, y] == LivingCell) //Condition if it is a living cell
                        {
                            if (livingNeighbourCount < MinNeighbour || livingNeighbourCount > MaxNeighbour)
                            {
                                // Underpopulation and Overpopulation problem
                                WillDie(x, y);// Gray the cell that are going to die
                                next[x, y] = DeadCell; //This cell is dead in the next round
                            }
                        }
                        else if (current[x, y] == DeadCell && livingNeighbourCount == NeighbourReqToReproduce)
                        {
                            next[x, y] = LivingCell;
                        }
                    }
                }

                if (IsDead())
                {
                    Console.WriteLine("Population has all died");
                    break;
                }
                    

                if (IsStagnant())
                {
                    Console.WriteLine("Population is stagnant");
                    break;
                }

                    Array.Copy(next, current, Width * Height); //Update the current array
                System.Threading.Thread.Sleep(Delay);
            }

        }

        /* Method to count the number of neighbour */
        public static int CountNeighbour(int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++) //For x
            {
                for (int j = -1; j < 2; j++) //For y
                {
                    if (!(i == 0 && j == 0) //Not itself
                        && (x + i >= BorderWidth && x + i <= Width - BorderWidth) //Not at the border
                        && (y + j >= BorderWidth && y + j < Height - BorderWidth)) //Not at the border
                    {
                        if (current[x + i, y + j] == LivingCell)
                            count++;
                    }
                }
            }
            return count;
        }

        /* Method to check if the population is dead*/
        public static bool IsDead()
        {
            for (int x = BorderWidth; x < Width - BorderWidth; x++)
            {
                for (int y = BorderWidth; y < Height - BorderWidth; y++)
                {
                    if (current[x, y] == LivingCell)
                        return false;
                }
            }
            return true;
        }

        /* Method to check if the population is stagnant*/
        public static bool IsStagnant()
        {
            if (current.Rank == next.Rank &&
            Enumerable.Range(0, current.Rank).All(dimension => current.GetLength(dimension) == next.GetLength(dimension)) &&
            current.Cast<char>().SequenceEqual(next.Cast<char>()))
                return true;
            return false;
        }

        public static void InitialiseRandomLive()
        {
            Console.Clear();
            Console.Write("Enter the spawn number (higher the number, spawn less): ");
            int number = Convert.ToInt32(Console.ReadLine());

            /* Probabilty for random spawning */
            Random rand = new Random();

            for (int x = BorderWidth; x < Width - BorderWidth; x++)
            {
                for (int y = BorderWidth; y < Height - BorderWidth; y++)
                {
                    if (rand.Next(number) == 0)
                    {
                        current[x, y] = LivingCell;
                    }

                }
            }
        }

        public static void InitialiseBeacon()
        {
            int x = Width / 2;
            int y = Height / 2;

            current[x - 2, y - 1] = LivingCell;

            current[x - 1, y - 2] = LivingCell;
            current[x - 2, y - 2] = LivingCell;

            current[x + 1, y] = LivingCell;
            current[x + 1, y + 1] = LivingCell;
            current[x, y + 1] = LivingCell;
        }

        public static void InitialiseBox()
        {
            int x = Width / 2;
            int y = Height / 2;

            current[x, y] = LivingCell;
            current[x + 1, y] = LivingCell;
            current[x + 1, y + 1] = LivingCell;
            current[x, y + 1] = LivingCell;
        }

        public static void InitialiseGlider()
        {
            int x = Width / 2;
            int y = Height / 2;

            current[x, y] = LivingCell;
            current[x - 1, y] = LivingCell;
            current[x + 1, y] = LivingCell;
            current[x + 1, y - 1] = LivingCell;
            current[x, y - 2] = LivingCell;
        }

        public static void InitialisePulsar()
        {
            int x = Width / 2;
            int y = Height / 2;

            //Top Left
            current[x - 1, y - 2] = LivingCell;
            current[x - 1, y - 3] = LivingCell;
            current[x - 1, y - 4] = LivingCell;

            current[x - 6, y - 2] = LivingCell;
            current[x - 6, y - 3] = LivingCell;
            current[x - 6, y - 4] = LivingCell;

            current[x - 2, y - 1] = LivingCell;
            current[x - 3, y - 1] = LivingCell;
            current[x - 4, y - 1] = LivingCell;

            current[x - 2, y - 6] = LivingCell;
            current[x - 3, y - 6] = LivingCell;
            current[x - 4, y - 6] = LivingCell;

            //Top Right
            current[x + 1, y - 2] = LivingCell;
            current[x + 1, y - 3] = LivingCell;
            current[x + 1, y - 4] = LivingCell;

            current[x + 6, y - 2] = LivingCell;
            current[x + 6, y - 3] = LivingCell;
            current[x + 6, y - 4] = LivingCell;

            current[x + 2, y - 1] = LivingCell;
            current[x + 3, y - 1] = LivingCell;
            current[x + 4, y - 1] = LivingCell;

            current[x + 2, y - 6] = LivingCell;
            current[x + 3, y - 6] = LivingCell;
            current[x + 4, y - 6] = LivingCell;

            //Bottom Left
            current[x - 1, y + 2] = LivingCell;
            current[x - 1, y + 3] = LivingCell;
            current[x - 1, y + 4] = LivingCell;

            current[x - 6, y + 2] = LivingCell;
            current[x - 6, y + 3] = LivingCell;
            current[x - 6, y + 4] = LivingCell;

            current[x - 2, y + 1] = LivingCell;
            current[x - 3, y + 1] = LivingCell;
            current[x - 4, y + 1] = LivingCell;

            current[x - 2, y + 6] = LivingCell;
            current[x - 3, y + 6] = LivingCell;
            current[x - 4, y + 6] = LivingCell;

            //Bottom Right
            current[x + 1, y + 2] = LivingCell;
            current[x + 1, y + 3] = LivingCell;
            current[x + 1, y + 4] = LivingCell;

            current[x + 6, y + 2] = LivingCell;
            current[x + 6, y + 3] = LivingCell;
            current[x + 6, y + 4] = LivingCell;

            current[x + 2, y + 1] = LivingCell;
            current[x + 3, y + 1] = LivingCell;
            current[x + 4, y + 1] = LivingCell;

            current[x + 2, y + 6] = LivingCell;
            current[x + 3, y + 6] = LivingCell;
            current[x + 4, y + 6] = LivingCell;


        }

        public static void WillDie(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(current[x, y]);
        }

        public static void GetDrawing()
        {
            Console.WriteLine("Press SPACE to insert / delete cell");
            Console.WriteLine("Press ENTER to start the simulation");

            ConsoleKeyInfo input = new ConsoleKeyInfo();
            int x = Width / 2, y = Height / 2; // Center coordinates

            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = PointerColour;
            //if (current[x, y] == LivingCell)
            //{
            //    Console.ForegroundColor = PointerColour;
            //    Console.BackgroundColor = LivingCellColour;
            //}
            Console.Write(ManualPointer);

            while (input.Key != ConsoleKey.Enter) // as long as the user doesn't press enter the loop goes on
            {
                // Getting the keypress
                input = Console.ReadKey(true);

                // Drawing empty space where the selection previously was
                Console.ForegroundColor = LivingCellColour;
                Console.BackgroundColor = BackgroundColour;
                Console.SetCursorPosition(x, y);
                Console.Write(current[x, y]);

                // Simple input checking
                switch (input.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (y > BorderWidth)
                            y -= 1;
                        break;

                    case ConsoleKey.DownArrow:
                        if (y + 1 < Height - BorderWidth)
                            y += 1;
                        break;

                    case ConsoleKey.RightArrow:
                        if (x + 1 < Width - BorderWidth)
                            x += 1;
                        break;

                    case ConsoleKey.LeftArrow:
                        if (x > BorderWidth)
                            x -= 1;
                        break;

                    case ConsoleKey.Spacebar: // Sets the cell to dead if it's alive and alive if it's dead
                        current[x, y] = current[x, y] == DeadCell ? LivingCell : DeadCell;
                        break;
                }

                // Drawing the selection
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = PointerColour;
                if (current[x, y] == LivingCell)
                {
                    Console.ForegroundColor = PointerColour2;
                    Console.BackgroundColor = LivingCellColour;
                }
                Console.Write(ManualPointer);
            }

            /*Make sure colour is correct*/
            Console.ForegroundColor = LivingCellColour;
            Console.BackgroundColor = BackgroundColour;

            // If the user pressed enter he got out of the while loop
            // which means he finished and so the game starts:
        }
    }
}
