/**

--- Day 14: Restroom Redoubt ---
Src : https://adventofcode.com/2024/day/14

Input example :
p=0,4 v=3,-3
p=6,3 v=-1,-3
p=10,3 v=-1,2
p=2,0 v=2,-1
p=0,0 v=1,3
p=3,0 v=-2,-2
p=7,6 v=-1,-3
p=3,0 v=-1,-2
p=9,3 v=2,3
p=7,3 v=-1,2
p=2,4 v=2,-3
p=9,5 v=-3,-3

p <- position
v <- velocity

each line represents a robot, and each robot has a position and a velocity in tiles per second.
The robots wrap around the grid, so the top and bottom of the grid are connected, and the left and right sides are also connected.

We need to predict the position of the robots after 100 seconds.

To get the result, we need to split the grid into quadrants, but ignoring the robots in the middle axis (horizontal or vertical).
The answer is counting the number of robots in each quadrant and multiply the obtained values together.


**/

using System;
using System.Drawing;

namespace Day14
{
    class Program
    {
        struct Robot
        {
            public int x, y, vx, vy;
        }

        static void Move(ref Robot robot, int gridWidth, int gridHeight)
        {
            robot.x += robot.vx;
            robot.y += robot.vy;

            if (robot.x < 0)
            {
                robot.x += gridWidth;
            }
            else if (robot.x >= gridWidth)
            {
                robot.x -= gridWidth;
            }

            if (robot.y < 0)
            {
                robot.y += gridHeight;
            }
            else if (robot.y >= gridHeight)
            {
                robot.y -= gridHeight;
            }            
        }

        static List<Robot> GetRobots(string[] input)
        {
            List<Robot> robots = new List<Robot>();

            foreach (string line in input)
            {
                string[] parts = line.Split(' ');

                Robot robot = new Robot();
                var position = parts[0].Split('=')[1];
                robot.x = int.Parse(position.Split(',')[0]);
                robot.y = int.Parse(position.Split(',')[1]);

                var velocity = parts[1].Split('=')[1];
                robot.vx = int.Parse(velocity.Split(',')[0]);
                robot.vy = int.Parse(velocity.Split(',')[1]);

                robots.Add(robot);
            }

            return robots;
        }

        static void printGrid(List<Robot> robots, int gridWidth, int gridHeight)
        {
            int[,] grid = new int[gridWidth, gridHeight];

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    grid[x, y] = 0;
                }
            }

            foreach (Robot robot in robots)
            {
                grid[robot.x, robot.y]++;
            }

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    if (grid[x, y] == 0)
                    {
                        Console.Write(".");
                    }
                    else
                    {
                        Console.Write(grid[x, y]);
                    }
                }
                Console.WriteLine();
            }
        }

        static void GenerateImage(List<Robot> robots, int gridWidth, int gridHeight, int seconds)
        {
            using (Bitmap bmp = new Bitmap(gridWidth, gridHeight))
            {

                for (int y = 0; y < gridHeight; y++)
                {
                    for (int x = 0; x < gridWidth; x++)
                    {
                        bmp.SetPixel(x, y, Color.Black);
                    }
                }

                foreach (Robot robot in robots)
                {
                    bmp.SetPixel(robot.x, robot.y, Color.White);
                }

                bmp.Save($"output_{seconds}.png");
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Day 14: Restroom Redoubt");

            // Load the input data and split it by lines
            string[] input = System.IO.File.ReadAllLines("input.txt");
            int gridWidth = 101;
            int gridHeight = 103;

            List<Robot> robots = GetRobots(input);

            int seconds = 10000;
            for (int i = 0; i < seconds; i++)
            {
                for (int j = 0; j < robots.Count; j++)
                {
                    Robot robot = robots[j];
                    Move(ref robot, gridWidth, gridHeight);
                    robots[j] = robot;
                }

                //Console.WriteLine("Seconds : " + i);
                // Fucking brute force to find the message
                // Better solution with chinese remainder theorem
                // GenerateImage(robots, gridWidth, gridHeight, i);
                // Part B : Answer is 7501 + 1 Because index starts from 1
            }

            int middleX = gridWidth / 2;
            int middleY = gridHeight / 2;

            // Sum the robots in each quadrant by removing the robots in the middle axis
            int q1 = 0, q2 = 0, q3 = 0, q4 = 0;
            foreach (Robot robot in robots)
            {
                if (robot.x < middleX && robot.y < middleY)
                {
                    q1++;
                }
                else if (robot.x > middleX && robot.y < middleY)
                {
                    q2++;
                }
                else if (robot.x < middleX && robot.y > middleY)
                {
                    q3++;
                }
                else if (robot.x > middleX && robot.y > middleY)
                {
                    q4++;
                }
            }

            var result = q1 * q2 * q3 * q4;

            // // Zero the middle vertical axis
            // int middleX = gridWidth / 2;
            // for (int y = 0; y < gridHeight; y++)
            // {
            //     robots.RemoveAll(robot => robot.x == middleX && robot.y == y);
            // }

            // // Zero the middle horizontal axis
            // int middleY = gridHeight / 2;
            // for (int x = 0; x < gridWidth; x++)
            // {
            //     robots.RemoveAll(robot => robot.x == x && robot.y == middleY);
            // }

            // printGrid(robots, gridWidth, gridHeight);
            Console.WriteLine("Part A : " + result);

            // Part B : Find the easter egg by display the grid and find the message
            Console.WriteLine("Part B : 7502");


        }
    }
}