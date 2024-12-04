/**
Day 04 : Word convolution

Part A:
- The word XMAS appears many times in the input.
- It can be in any 8 directions. 4 cardinal directions and 4 diagonal directions.
- It can be written backwards.

Input example:
MMMSXXMASM
MSAMXMSMSA
AMXSXMAAMM
MSAMASMSMX
XMASAMXAMM
XXAMMXXAMA
SMSMSASXSS
SAXAMASAAA
MAMMMXMMMM
MXMXAXMASX

Output example: 18
The word XMAS appears 18 times in the input.

Probable solution:
- Read the input and store it in a 2D array.
- Create a point spread filter for the word XMAS
- Convolve the word XMAS in all 8 directions.
- Count the number of times the word appears.

Part B:
Find this filter in any directions

M.S
.A.
M.S

**/

using System;
using System.Net;
using System.Reflection.Metadata;

namespace Day04
{
    static class ArrayExtensions
    {
        // Flip the array horizontally (reverse each row)
        public static char[,] FlipHorizontally(this char[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            char[,] flipped = new char[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    flipped[i, j] = array[i, cols - 1 - j];

            return flipped;
        }

        // Flip the array vertically (reverse the order of rows)
        public static char[,] FlipVertically(this char[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            char[,] flipped = new char[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    flipped[i, j] = array[rows - 1 - i, j];

            return flipped;
        }

        // Helper method to print a 2D array
        public static void PrintArray(this char[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    Console.Write(array[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
    }
    class Program
    {
        private static readonly char[] XmasPattern = { 'X', 'M', 'A', 'S'};
        private static readonly int filterSize = 4;

        private static readonly char[,] Mask = {
            {'M', '.', 'S'},
            {'.', 'A', '.'},
            {'M', '.', 'S'}
        };

        private static char[,] debugOutput;

        static void Main(string[] args)
        {
            string[] lines = System.IO.File.ReadAllLines("example.txt");

            // Convert lines to a 2D array of characters
            char[,] input = new char[lines.Length, lines[0].Length];
            char[,] debugOutput = new char[lines.Length, lines[0].Length];

            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    input[i, j] = lines[i][j];
                    debugOutput[i, j] = lines[i][j];
                }
            }

            int result = partA(input);

            Console.WriteLine($"Part A : {result}");

            result = partB(input);
            Console.WriteLine($"Part B : {result}");
        }

        static int partB(char [,] input) {
            
            int count = 0;

            for (int i = 0; i < input.GetLength(0); i++)
            {
                for (int j = 0; j < input.GetLength(1); j++)
                {
                    count += convolveMask(input, i, j);
                }
            }

            return count;
        }

        static int convolveMask(char[,] input, int i, int j) {
            int count = 0;

            // There are 2 possible flip directions which gives 4 possible orientations
            // 1. No flip
            // 2. Horizontal flip
            // 3. Vertical flip
            // 4. Horizontal flip + Vertical flip

            // 1. No flip
            if (convolveMaskPattern(input, i, j, Mask))
            {
                count++;
            }

            // 2. Horizontal flip
            char[,] hFlipped = Mask.FlipHorizontally();
            if (convolveMaskPattern(input, i, j, hFlipped))
            {
                count++;
            }

            // 3. Vertical flip
            char[,] vFlipped = Mask.FlipVertically();
            if (convolveMaskPattern(input, i, j, vFlipped))
            {
                count++;
            }

            // 4. Horizontal flip + Vertical flip
            char[,] hvFlipped = hFlipped.FlipVertically();
            if (convolveMaskPattern(input, i, j, hvFlipped))
            {
                count++;
            }


            return count;
        }

        static bool convolveMaskPattern(char[,] input, int i, int j, char[,] mask) {

            // Check if the mask can be convolved in the input
            int maxI = i + mask.GetLength(0);
            int maxJ = j + mask.GetLength(1);

            if (maxI >= input.GetLength(0) || maxJ >= input.GetLength(1))
            {
                return false;
            }

            // Start from the mask size / 2 position
            int maskI = mask.GetLength(0);
            int maskJ = mask.GetLength(1);

            for (int k = 0; k < maskI; k++)
            {
                for (int l = 0; l < maskJ; l++)
                {
                    if (mask[k, l] == '.')
                    {
                        continue;
                    }

                    if (mask[k, l] != input[i + k, j + l])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        static int partA(char[,] input)
        {
            // Starting from the top left corner of the input and the center of the filter
            // Rotate the word XMAS in all 8 directions.

            int count = 0;
            for (int i = 0; i < input.GetLength(0); i++)
            {
                for (int j = 0; j < input.GetLength(1); j++)
                {
                    count += convolve(input, i, j);
                }
            }


            return count;
        }

        static int convolve(char[,] input, int i, int j)
        {
            int count = 0;

            // Convolve the XMAS pattern in all 8 directions starting from the top left corner

            // 1. Top to bottom
            if (convolvePattern(input, i, j, 1, 0))
            {
                count++;
            }

            // 2. Bottom to top
            if (convolvePattern(input, i, j, -1, 0))
            {
                count++;
            }

            // 3. Left to right
            if (convolvePattern(input, i, j, 0, 1))
            {
                count++;
            }

            // 4. Right to left
            if (convolvePattern(input, i, j, 0, -1))
            {
                count++;
            }

            // 5. Top left to bottom right
            if (convolvePattern(input, i, j, 1, 1))
            {
                count++;
            }

            // 6. Bottom right to top left
            if (convolvePattern(input, i, j, -1, -1))
            {
                count++;
            }

            // 7. Top right to bottom left
            if (convolvePattern(input, i, j, 1, -1))
            {
                count++;
            }

            // 8. Bottom left to top right
            if (convolvePattern(input, i, j, -1, 1))
            {
                count++;
            }

            return count;
        }

        static bool convolvePattern(char[,] input, int i, int j, int di, int dj)
        {
            // Convolve the XMAS pattern in the input starting from the top left corner
            // and moving in the direction di, dj
            int originalI = i;
            int originalJ = j;

            // Check if it's possible to convolve the pattern in the input
            int maxI = i + di * filterSize;
            int maxJ = j + dj * filterSize;

            int inputI = input.GetLength(0);
            int inputJ = input.GetLength(1);

            // if (maxI >= inputI || maxI < -1 ||
            //     maxJ >= inputJ || maxJ < -1)
            // {
            //     return false;
            // }

            // FML : There's something I'm missing here. The above condition is not working as expected.
            try {
                for (int k = 0; k < filterSize; k++)
                {
                    if (input[i, j] != XmasPattern[k])
                    {
                        return false;
                    }

                    i += di;
                    j += dj;
                }

                return true;
            } catch (Exception e) {
                return false;
            }
        }



    }
}
