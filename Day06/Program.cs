/**

--- Day 6: Guard Gallivant ---

Input example:
....#.....
.........#
..........
..#.......
.......#..
..........
.#..^.....
........#.
#.........
......#...

Explanation:
- # are obstacles
- ^ is the starting point of the guard pointing up
- . are empty spaces
- The guard moves in the direction it is pointing until it hits an obstacle where it turns right
- The guard will keep moving until it get out of the grid by moving outside the grid

Goal : Count the number of steps the guard takes before it leaves the grid
Example answer: 41

**/

using System;

namespace Day06
{
    class Program
    {
        /// <summary>
        /// Structure to store the visited points of the guard
        /// </summary>
        class VisitedPoint
        {
            public VisitedPoint(int x, int y, int direction)
            {
                this.x = x;
                this.y = y;
                this.direction = direction;
            }
            public int x;
            public int y;
            public int direction;
        }

        static List<VisitedPoint> visitedPoints = new List<VisitedPoint>();
       
        /// <summary>
        /// Check if the guard can be blocked if we put a obstacle at the given position
        /// Algorithm:
        /// If we put an obstacle at the given position, we need to check if the guard can be blocked
        /// To check if the guard can be blocked, if the guard is moving back to the same position, it can be blocked
        /// </summary>
       
        static bool CanBlockGuard(char[,] grid, int x, int y, int direction)
        {
            return false;
        }

        static Tuple<char [,], int, int> ConvertToGrid(string[] input)
        {
            char[,] grid = new char[input.Length, input[0].Length];
            int x = -1;
            int y = -1;

            for (int i = 0; i < input.Length; i++)
            {
                for (int j = 0; j < input[i].Length; j++)
                {
                    grid[i, j] = input[i][j];
                    if (input[i][j] == '^')
                    {
                        x = j;
                        y = i;
                    }
                }
            }

            return Tuple.Create(grid, x, y);
        }
        static int PossibleInfiniteLoops(string[] input)
        {                        
            int x = 0;
            int y = 0;
            int direction = 0; // 0 = up, 1 = right, 2 = down, 3 = left

            char[,] grid;
            char[,] originalGrid;
            
            // Convert the input to a grid
            var result = ConvertToGrid(input);
            grid = result.Item1;
            x = result.Item2;
            y = result.Item3;

            // Copy the original grid
            originalGrid = new char[grid.GetLength(0), grid.GetLength(1)];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    originalGrid[i, j] = grid[i, j];
                }
            }

            // Build the current path of the guard
            // Add current position to the list of visited points
            VisitedPoint visitedPoint = new(x, y, direction);
            while (true)
            {
                // Check if there is a obstacle in front of the guard
                if (direction == 0)
                {
                    // Check if the guard is going out of the grid
                    if (y - 1 < 0)
                    {
                        break;
                    }
                    
                    if (grid[y - 1, x] == '#')
                    {
                        direction = (direction + 1) % 4;
                    }
                }
                else if (direction == 1)
                {
                    if (x + 1 >= grid.GetLength(1))
                    {
                        break;
                    }

                    if (grid[y, x + 1] == '#')
                    {
                        direction = (direction + 1) % 4;
                    }
                }
                else if (direction == 2)
                {
                    if (y + 1 >= grid.GetLength(0))
                    {
                        break;
                    }
                    if (grid[y + 1, x] == '#')
                    {
                        direction = (direction + 1) % 4;
                    }
                }
                else if (direction == 3)
                {
                    if (x - 1 < 0)
                    {
                        break;
                    }
                    if (grid[y, x - 1] == '#')
                    {
                        direction = (direction + 1) % 4;
                    }
                }

                if (direction == 0)
                {
                    y--;
                }
                else if (direction == 1)
                {
                    x++;
                }
                else if (direction == 2)
                {
                    y++;
                }
                else if (direction == 3)
                {
                    x--;
                }

                if (x < 0 || x >= grid.GetLength(1) || y < 0 || y >= grid.GetLength(0))
                {
                    break;
                }

                visitedPoints.Add(new VisitedPoint(x, y, direction));
            }

            // Starting from the last visited point, check if the guard can be blocked
            for (int i = visitedPoints.Count - 1; i >= 0; i--)
            {
                // Put a obstacle at the current position
                grid[visitedPoints[i].y, visitedPoints[i].x] = '#';

            }


            return -1;
        }
    


        static void Main(string[] args)
        {
            Console.WriteLine("Day 06 - Guard Gallivant");
            string[] input = System.IO.File.ReadAllLines("example.txt");
            char[,] pathGrid;

            // Part A
            int result = GuardGallivant(input, out pathGrid, false);
            Console.WriteLine($"The guard took {result} steps before leaving the grid.");

            //drawGrid(pathGrid, 45, 86, 0);
            // Part B : Block the guard in an infinite loop by adding obstacles
            // Get the number of places where we can add obstacles to create an infinite loop
            result = PossibleInfiniteLoops(input);
            Console.WriteLine($"The guard can be blocked in {result} places to create an infinite loop.");
        }

        static void drawGrid(char[,] grid, int x, int y, int direction)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (i == y && j == x)
                    {
                        if (direction == 0)
                        {
                            Console.Write('^');
                        }
                        else if (direction == 1)
                        {
                            Console.Write('>');
                        }
                        else if (direction == 2)
                        {
                            Console.Write('v');
                        }
                        else if (direction == 3)
                        {
                            Console.Write('<');
                        }
                    }
                    else
                    {
                        Console.Write(grid[i, j]);
                    }
                }
                Console.WriteLine();
            }
        }

        static int GuardGallivant(string[] input, out char[,] pathGrid, bool debug = false)
        {
            int x = 0;
            int y = 0;
            int steps = 1;
            int direction = 0; // 0 = up, 1 = right, 2 = down, 3 = left
            char[,] grid = new char[input.Length, input[0].Length];
            pathGrid = new char[input.Length, input[0].Length];

            for (int i = 0; i < input.Length; i++)
            {
                for (int j = 0; j < input[i].Length; j++)
                {
                    grid[i, j] = input[i][j];
                    pathGrid[i, j] = input[i][j];
                    if (input[i][j] == '^')
                    {
                        x = j;
                        y = i;

                        // Replace the starting point with a visited point
                        grid[i, j] = 'X';
                        // Put the direction of the guard in the pathGrid
                        pathGrid[i, j] = direction.ToString()[0];
                    }
                }
            }

            while (true)
            {
                // Check if there is a obstacle in front of the guard
                if (direction == 0)
                {
                    // Check if the guard is going out of the grid
                    if (y - 1 < 0)
                    {
                        break;
                    }
                    
                    if (grid[y - 1, x] == '#')
                    {
                        direction = (direction + 1) % 4;
                    }
                }
                else if (direction == 1)
                {
                    if (x + 1 >= grid.GetLength(1))
                    {
                        break;
                    }

                    if (grid[y, x + 1] == '#')
                    {
                        direction = (direction + 1) % 4;
                    }
                }
                else if (direction == 2)
                {
                    if (y + 1 >= grid.GetLength(0))
                    {
                        break;
                    }
                    if (grid[y + 1, x] == '#')
                    {
                        direction = (direction + 1) % 4;
                    }
                }
                else if (direction == 3)
                {
                    if (x - 1 < 0)
                    {
                        break;
                    }
                    if (grid[y, x - 1] == '#')
                    {
                        direction = (direction + 1) % 4;
                    }
                }

                if (direction == 0)
                {
                    y--;
                }
                else if (direction == 1)
                {
                    x++;
                }
                else if (direction == 2)
                {
                    y++;
                }
                else if (direction == 3)
                {
                    x--;
                }

                if (x < 0 || x >= grid.GetLength(1) || y < 0 || y >= grid.GetLength(0))
                {
                    break;
                }

                // Put an X in the grid to mark the path of the guard
                grid[y, x] = 'X';
                pathGrid[y, x] = direction.ToString()[0];
                steps++;
            }

            if (debug)
            {
                drawGrid(grid, x, y, direction);
            }

            // LINQ : Count the number of X in the grid
            int count = 0;
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == 'X')
                    {
                        count++;
                    }
                }
            }


            return count;
        }
    }
}