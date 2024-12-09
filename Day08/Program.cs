/**

--- Day 8: Resonant Collinearity ---

Input example:
......#....#
...#....0...
....#0....#.
..#....0....
....0....#..
.#....A.....
...#........
#......#....
........A...
.........A..
..........#.
..........#.

Explanation Part A:
    Count the number of antinodes within the grid.
    They are collinear points with antennas pairs.
    A pair of antennas is represented by a digit, lowercase or uppercase letter.
    Antinodes can overlap with antennas.
    When overlapping, only the antennas are displayed in the grid.

    In the example, there are 14 antinodes. One which is overlapped by the top-most A-antenna.

**/

using System;
using System.Runtime.ExceptionServices;

namespace Day08
{
    class Program
    {
        struct Point
        {
            public int X;
            public int Y;

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static bool operator ==(Point a, Point b)
            {
                return a.X == b.X && a.Y == b.Y;
            }

            public static bool operator !=(Point a, Point b)
            {
                return a.X != b.X || a.Y != b.Y;
            }

            public override string ToString()
            {
                return $"({X}, {Y})";
            }
        }

        struct Pairs 
        {
            // Represented by a digit, lowercase or uppercase letter
            public char Frequency;
            public Point A;
            public Point B;
            public Point? AntinodeA;
            public Point? AntinodeB;

            public override string ToString()
            {
                return $"Frequency: {Frequency}\t A: {A}, B: {B}\t AntinodeA: {AntinodeA}\t AntinodeB: {AntinodeB}";
            }
        }

        // Print the input and the pairs for debugging
        static void PrintGridComparison(string[] input, List<Pairs> pairs)
        {
            for (int i = 0; i < input.Length; i++)
            {
                string line = input[i];
                Console.Write(line + "\t");

                for (int j = 0; j < line.Length; j++)
                {
                    Point currentPoint = new Point(j, i);
                    // Check if there's an antenna or antinode at this position
                    // if ( pairs.Select(pair => pair.A).Contains(currentPoint) || 
                    //      pairs.Select(pair => pair.B).Contains(currentPoint)) 
                    // {
                    //     // Draw frequency
                    //     Console.Write(pairs.Where(pair => pair.A == currentPoint || pair.B == currentPoint).First().Frequency);
                    // } 
                    // else 
                    if ( pairs.Select(pair => pair.AntinodeA).Contains(currentPoint) || 
                              pairs.Select(pair => pair.AntinodeB).Contains(currentPoint)) 
                    {
                        // Draw antinode
                        Console.Write("#");
                    } 
                    else 
                    {
                        Console.Write(".");
                    }
                }
                Console.WriteLine();
            }            

            foreach (Pairs pair in pairs)
            {
                Console.WriteLine(pair);
            }
            
        }

        static int PartA(string [] input) {
            // Find all the pairs of antennas
            List<Pairs> pairs = new List<Pairs>();
            int countAntiNodes = 0;

            int lineCount = 0;
            foreach (string line in input)
            {
                
                // Horizontal scan
                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];
                    
                    if (char.IsLetterOrDigit(c))
                    {
                        bool scanRestOfLine = true;
                        // Find the other antenna from the current one

                        for (int k = lineCount; k < input.Length; k++)
                        {
                            string nextLine = input[k];
                            for (int j = 0; j < nextLine.Length; j++)
                            {
                                if (scanRestOfLine)
                                {
                                    j = i + 1;
                                    scanRestOfLine = false;
                                    if (j >= nextLine.Length)
                                    {
                                        break;
                                    }
                                }

                                char d = nextLine[j];

                                if (d == c) {
                                    // Found a pair of antennas
                                    Pairs pair = new Pairs();
                                    pair.Frequency = c;
                                    pair.A = new Point(i, lineCount);
                                    pair.B = new Point(j, k);

                                    // get dx and dy of pairs
                                    int dx = j - i;
                                    int dy = k - lineCount;

                                    // Calculate the top antinode position
                                    int x = i - dx;
                                    int y = lineCount - dy;

                                    if (x >= 0 && x < nextLine.Length && y >= 0 && y < input.Length)
                                    {
                                        pair.AntinodeA = new Point(x, y);
                                    }

                                    // Calculate the bottom antinode position
                                    x = j + dx;
                                    y = k + dy;

                                    if (x >= 0 && x < nextLine.Length && y >= 0 && y < input.Length)
                                    {
                                        pair.AntinodeB = new Point(x, y);
                                    }

                                    pairs.Add(pair);
                                }
                            }
                        }
                    }
                }
                lineCount++;                
            }

            // Count the number of antinodes
            // Join the list of antinodes A and B
            List<Point> antinodes = new List<Point>();
            foreach (Pairs pair in pairs)
            {
                if (pair.AntinodeA.HasValue)
                {
                    if (!antinodes.Contains(pair.AntinodeA.Value))
                    {   
                        antinodes.Add(pair.AntinodeA.Value);
                        countAntiNodes++;
                    }
                    
                }

                if (pair.AntinodeB.HasValue)
                {
                    if (!antinodes.Contains(pair.AntinodeB.Value))
                    {
                        antinodes.Add(pair.AntinodeB.Value);
                        countAntiNodes++;
                    }
                }
            }

            PrintGridComparison(input, pairs);

            return countAntiNodes;
        }

        static void Main(string[] args)
        {
            string[] input = File.ReadAllLines("input.txt");
            Console.WriteLine("Part A: " + PartA(input)); 
        }


    }
}