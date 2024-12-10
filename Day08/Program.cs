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
using System.Data;
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

            public List<Point> HarmonicAntinodes = new List<Point>();

            public Pairs()
            {
            }

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

        static int PartA(string [] input, bool hasHarmonic = false) {
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

                                    if (!hasHarmonic)
                                    {
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
                                    } else {
                                        // Count the harmonic antinodes until the edge of the grid
                                        bool topLeftOutOfBounds = false;
                                        bool bottomRightOutOfBounds = false;
                                        int count = 1;
                                        while (!(topLeftOutOfBounds && bottomRightOutOfBounds))
                                        {
                                            int x = count * dx;
                                            int y = count * dy;

                                            Point antinodeTL = new Point(i - x, lineCount - y);
                                            Point antinodeBR = new Point(j + x, k + y);

                                            if (antinodeTL.X >= 0 && antinodeTL.X < nextLine.Length && antinodeTL.Y >= 0 && antinodeTL.Y < input.Length)
                                            {
                                                pair.HarmonicAntinodes.Add(antinodeTL);
                                            }
                                            else
                                            {
                                                topLeftOutOfBounds = true;
                                            }

                                            if (antinodeBR.X >= 0 && antinodeBR.X < nextLine.Length && antinodeBR.Y >= 0 && antinodeBR.Y < input.Length)
                                            {
                                                pair.HarmonicAntinodes.Add(antinodeBR);
                                            }
                                            else
                                            {
                                                bottomRightOutOfBounds = true;
                                            }

                                            count++;
                                        }                              
                                    }

                                    pairs.Add(pair);
                                }
                            }
                        }
                    }
                }
                lineCount++;                
            }

            if (!hasHarmonic)
            {
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
            } else {
                // create a grid of spaces the same size as the input
                // for each pair, add the antinodes to the grid
                // count the number of antinodes
                char[,] grid = new char[input.Length, input[0].Length];

                // LINQ to fill the grid with spaces
                for (int i = 0; i < input.Length; i++)
                {
                    for (int j = 0; j < input[i].Length; j++)
                    {
                        grid[i, j] = ' ';
                    }
                }

                foreach (Pairs pair in pairs)
                {
                    grid[pair.A.Y, pair.A.X] = '#';
                    grid[pair.B.Y, pair.B.X] = '#';

                    if (pair.HarmonicAntinodes.Count > 0)
                    {
                        foreach (Point antinode in pair.HarmonicAntinodes)
                        {
                            grid[antinode.Y, antinode.X] = '#';
                        }
                    }

                }

                // Count the number of antinodes by counting the number of '#' in the grid
                for (int i = 0; i < input.Length; i++)
                {
                    for (int j = 0; j < input[i].Length; j++)
                    {
                        if (grid[i, j] == '#')
                        {
                            countAntiNodes++;
                        }

                        //Console.Write(grid[i, j]);
                    }
                    //Console.WriteLine();
                }
                
                
            }

            //PrintGridComparison(input, pairs);

            return countAntiNodes;
        }

        static void Main(string[] args)
        {
            string[] input = File.ReadAllLines("input.txt");
            Console.WriteLine("Part A: " + PartA(input)); 
            Console.WriteLine("Part A: " + PartA(input, true)); 
        }


    }
}