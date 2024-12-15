/**

--- Day 12: Garden Groups ---
Source : https://adventofcode.com/2024/day/12

Input example A :
RRRRIICCFF
RRRRIICCCF
VVRRRCCFFF
VVRCCCJFFF
VVVVCJJCFE
VVIVCCJJEE
VVIIICJJEE
MIIIIIJJEE
MIIISIJEEE
MMMISSJEEE

Answer : 140

Example B:
OOOOO
OXOXO
OOOOO
OXOXO
OOOOO

Answer : 772

Count the perimeters of each region in the garden. A region is a group of connected cells (letter) with the same value.
Only sides (left, right, top and bottom) are considered as perimeter

Probable Solution:
Treat the problem as a binary image processing problem.
1. Filter each letters and create a list of regions
2. For each region, calculate the area and perimeter
3. Multiply the area by the perimeter to calculate the cost
3. Sum all the costs to get the total cost

**/

using System;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace Day12
{
    class Program
    {
        struct CropType {
            public char Letter;
            public int Area;
            public int[,] Garden;

            public List<Region> Regions;
        }

        struct Region {
            public int startRow;
            public int startCol;
            public int Area;
            public int Perimeter;
            public int Sides;

            public int Cost() => Area * Perimeter;
        }

        static void Main(string[] args)
        {
            List<CropType> cropTypes = new List<CropType>();
            string[] input = File.ReadAllLines("example.txt");

            char[,] garden = new char[input.Length, input[0].Length];
            for (int i = 0; i < input.Length; i++)
            {
                for (int j = 0; j < input[i].Length; j++)
                {
                    garden[i, j] = input[i][j];
                }
            }

            // For each letter of the alphabet, create a temp garden with -1
            for (char letter = 'A'; letter <= 'Z'; letter++)
            {
                CropType cropType = new CropType();
                
                cropType.Letter = letter;
                cropType.Garden = new int[garden.GetLength(0), garden.GetLength(1)];
                for (int i = 0; i < garden.GetLength(0); i++)
                {
                    for (int j = 0; j < garden.GetLength(1); j++)
                    {
                        if (garden[i, j] == letter)
                        {
                            cropType.Garden[i, j] = 1;
                            cropType.Area++;
                        }
                        else
                        {
                            cropType.Garden[i, j] = 0;
                        }
                    }
                }

                cropTypes.Add(cropType);                
            }

            // // Test with a specific crop
            // var crop = cropTypes.Where(c => c.Letter == 'R').First();
            // crop.Regions = FindConnectedComponents(crop.Garden);
            // crop.Area = crop.Regions.Sum(r => r.Area);

            // // Print the garden
            // PrintPlot(crop.Garden);         
            int totalCost = 0;
            for (int i = 0; i < cropTypes.Count; i++)
            {
                var crop = cropTypes[i];
                crop.Regions = FindConnectedComponents(cropTypes[i].Garden);

                totalCost += crop.Regions.Sum(r => r.Cost());
            }

            // Print the answer
            Console.WriteLine(totalCost);
        }

        static List<Region> FindConnectedComponents(int[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            bool[,] visited = new bool[rows, cols];
            List<Region> regions = new List<Region>();

            // Directions for moving in 4-connected grid (up, down, left, right)
            int[] dRow = { -1, 1, 0, 0 };
            int[] dCol = { 0, 0, -1, 1 };

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (grid[i, j] == 1 && !visited[i, j])
                    {
                        Region region = new Region();
                        // Start DFS for a new region
                        region.Area = DFS(grid, visited, i, j, dRow, dCol, out region.Perimeter, out region.Sides);
                        region.startRow = i;
                        region.startCol = j;
                        regions.Add(region);
                    }
                }
            }

            return regions;
        }

        static void PrintPlot(int [,] garden)
        {
            for (int i = 0; i < garden.GetLength(0); i++)
            {
                for (int j = 0; j < garden.GetLength(1); j++)
                {
                    Console.Write(garden[i, j]);
                }
                Console.WriteLine();
            }
        }


        // Count the corners of a cell
        static int CountCorners (int [,] garden, int i, int j)
        {
            int corners = 0;

            if (garden[i, j] == 1)
            {
                int left = j - 1;
                int right = j + 1;
                int up = i - 1;
                int down = i + 1;

                // All top left cases
                if (left >= 0 && up >= 0 && garden[up, left] == 0)
                {
                    corners++;
                }
            }

            return corners;
        }

        // Depth First Search
        static int DFS(int[,] grid, bool[,] visited, int row, int col, int[] dRow, int[] dCol, out int perimeter, out int sides)
        {
            Stack<(int, int)> stack = new Stack<(int, int)>();
            stack.Push((row, col));
            visited[row, col] = true;
            int size = 0;
            perimeter = 0;
            sides = 0;

            while (stack.Count > 0)
            {
                var (currentRow, currentCol) = stack.Pop();
                size++;

                // Check all 4 directions (up, down, left, right)
                for (int d = 0; d < 4; d++)
                {
                    int newRow = currentRow + dRow[d];
                    int newCol = currentCol + dCol[d];

                    if (IsValid(grid, visited, newRow, newCol))
                    {
                        visited[newRow, newCol] = true;
                        stack.Push((newRow, newCol));
                    }
                }
                // Count the sides
                sides += CountCorners(grid, currentRow, currentCol);
            }

            return size;
        }

        static bool IsValid(int[,] grid, bool[,] visited, int row, int col)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            return row >= 0 && row < rows &&
                col >= 0 && col < cols &&
                grid[row, col] == 1 &&
                !visited[row, col];
        }

        static int CalculatePerimeter(char [,] garden, int i, int j)
        {
            return -1;
        }
    }
}