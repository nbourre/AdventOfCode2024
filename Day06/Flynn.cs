using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static readonly (int, int)[] Directions = { (-1, 0), (0, 1), (1, 0), (0, -1) }; // Up, Right, Down, Left

    static void MainFlynn()
    {
        var grid = File.ReadAllLines("input.txt").Select(line => line.ToCharArray()).ToArray();

        // Part 1
        var (guardPos, guardDirection) = GetGuardInfo(grid);
        var (visited, _) = Traverse(grid, guardPos, guardDirection);
        Console.WriteLine(visited.Count);

        // Part 2
        var possibleObstaclePositions = new HashSet<(int, int)>(visited);
        possibleObstaclePositions.Remove(guardPos);

        int count = 0;
        int totalPositions = possibleObstaclePositions.Count;
        int progress = 0;

        foreach (var pop in possibleObstaclePositions)
        {
            progress++;
            Console.WriteLine($"\r{100.0 * progress / totalPositions:0.0}% Complete");

            var newGrid = grid.Select(row => (char[])row.Clone()).ToArray();
            newGrid[pop.Item1][pop.Item2] = '#';
            var (_, isInfinite) = Traverse(newGrid, guardPos, guardDirection);
            if (isInfinite) count++;
        }

        Console.WriteLine();
        Console.WriteLine(count);
    }

    static ((int, int), int) GetGuardInfo(char[][] grid)
    {
        var guardChars = new[] { '^', '>', 'v', '<' };
        for (int row = 0; row < grid.Length; row++)
        {
            for (int direction = 0; direction < guardChars.Length; direction++)
            {
                int col = Array.IndexOf(grid[row], guardChars[direction]);
                if (col != -1)
                {
                    return ((row, col), direction);
                }
            }
        }
        throw new Exception("Guard not found");
    }

    static (HashSet<(int, int)>, bool) Traverse(char[][] grid, (int, int) startPos, int startDirection)
    {
        var visited = new Dictionary<(int, int), HashSet<int>>();
        var pos = startPos;
        var direction = startDirection;
        bool infiniteLoop = true;

        while (true)
        {
            // Ensure we track directions visited for this position
            if (!visited.ContainsKey(pos))
            {
                visited[pos] = new HashSet<int>();
            }

            // Check if we have been here in the same direction before
            if (visited[pos].Contains(direction))
            {
                break; // Infinite loop detected
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
                if (nextPos.Item1 < 0 || nextPos.Item2 < 0 || nextPos.Item1 >= grid.Length || nextPos.Item2 >= grid[0].Length)
                {
                    infiniteLoop = false; // Guard exits the map
                    return (new HashSet<(int, int)>(visited.Keys), infiniteLoop);
                }

                // Check if the next position is not an obstacle
                if (grid[nextPos.Item1][nextPos.Item2] != '#')
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

}
