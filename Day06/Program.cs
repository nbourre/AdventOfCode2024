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
using Microsoft.VisualBasic;

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
                this.X = x;
                this.Y = y;
                this.Direction = direction;
            }
            public int X;
            public int Y;
            public int Direction;
        }

        static readonly (int, int)[] Directions = { (-1, 0), (0, 1), (1, 0), (0, -1) }; // Up, Right, Down, Left
       
        /// <summary>
        /// Traverse the grid to find the path of the guard
        /// </summary>
        /// <param name="grid">The grid to traverse</param>
        /// <param name="startPos">The starting position of the guard in X, Y</param>
        /// <param name="startDirection">The starting direction of the guard</param>
        /// <returns>A tuple containing the path and a boolean indicating if the guard is in an infinite loop</returns>
        static (HashSet<(int, int)>, bool) Traverse(char[,] grid, (int, int) startPos, int startDirection)
        {
            var visitedPoint = new VisitedPoint(startPos.Item1, startPos.Item2, startDirection);
            var visited = new Dictionary<(int, int), HashSet<int>>();
            var pos = startPos;
            var direction = startDirection;
            bool infiniteLoop = true;

            while (true)
            {
                if (!visited.ContainsKey(pos))
                {
                    visited[pos] = new HashSet<int>();
                }

                // Check if we have been here in the same direction before
                // if so, it means the guard is moving back to the same position
                if (visited[pos].Contains(direction))
                {
                    break;
                }

                visited[pos].Add(direction);

                // Try to move in the current direction or turn if blocked
                bool moved = false;
                for (int i = 0; i < 4; i++)
                {
                    // Calculate the next position
                    var nextPos = (
                        pos.Item1 + Directions[direction].Item1,
                        pos.Item2 + Directions[direction].Item2
                    );

                    // Check bounds
                    if (nextPos.Item1 < 0 || nextPos.Item2 < 0 || nextPos.Item1 >= grid.GetLength(1) || nextPos.Item2 >= grid.GetLength(0))
                    {
                        infiniteLoop = false; // Guard exits the map
                        return (new HashSet<(int, int)>(visited.Keys), infiniteLoop);
                    }

                    // Check if the next position is not an obstacle
                    if (grid[nextPos.Item1, nextPos.Item2] != '#')
                    {
                        pos = nextPos; // Move to the next position
                        moved = true;
                        break;
                    }

                    // Turn right if the way is blocked
                    direction = (direction + 1) % 4;
                }

                // If no move was made, the guard is stuck (all directions blocked)
                if (!moved)
                {
                    break;
                }
            }


            return (new HashSet<(int, int)>(visited.Keys), infiniteLoop);
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
            y = result.Item2;
            x = result.Item3;

            // Copy the original grid
            originalGrid = new char[grid.GetLength(0), grid.GetLength(1)];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    originalGrid[i, j] = grid[i, j];
                }
            }

            var startPos = (x, y);
            var traverse = Traverse(grid, (x, y), direction);
            var visitedPoints = traverse.Item1;           

            var possibilities = new HashSet<(int, int)>(visitedPoints);
            possibilities.Remove(startPos);

            int count = 0;

            foreach (var obstacle in possibilities) {
                char [,] updatedGrid = new char[grid.GetLength(0), grid.GetLength(1)];
                for (int i = 0; i < grid.GetLength(0); i++)
                {
                    for (int j = 0; j < grid.GetLength(1); j++)
                    {
                        updatedGrid[i, j] = grid[i, j];
                    }
                }

                updatedGrid[obstacle.Item2, obstacle.Item1] = '#';

                var (dgaf, isInfinite) = Traverse(updatedGrid, startPos, direction);
                if (isInfinite)
                {
                    count++;
                }
            }

            Console.WriteLine(count);


            return count;
        }
    


        static void Main(string[] args)
        {
            Console.WriteLine("Day 06 - Guard Gallivant");
            string[] input = System.IO.File.ReadAllLines("input.txt");
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

/**

// Build the current path of the guard
// Add current position to the list of visited points
VisitedPoint visitedPoint = new(x, y, direction);
while (true)
{
    // Check if visitedPoint is already in the list
    // if so, it means the guard is moving back to the same position
    if (visitedPoints.Count(p => p.X == visitedPoint.X && p.Y == visitedPoint.Y && p.Direction == visitedPoint.Direction) > 0)
    {
        break;
    } else {
        visitedPoints.Add(visitedPoint);
    }

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

    visitedPoint = new(x, y, direction);                
}

// Starting from the last visited point, check if the guard can be blocked
for (int i = visitedPoints.Count - 1; i >= 0; i--)
{
    // Put a obstacle at the current position
    grid[visitedPoints[i].Y, visitedPoints[i].X] = '#';

}
**/