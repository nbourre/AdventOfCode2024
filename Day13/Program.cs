/**

--- Day 13: Claw Contraption ---
Src : https://adventofcode.com/2024/day/13

Part A:
    Button A : Cost 3 tokens to push
    Button B : Cost 1 tokens to push

    Minimize the cost to reach the prize

    

Example input:
    Button A: X+94, Y+34
    Button B: X+22, Y+67
    Prize: X=8400, Y=5400

    Button A: X+26, Y+66
    Button B: X+67, Y+21
    Prize: X=12748, Y=12176

    Button A: X+17, Y+86
    Button B: X+84, Y+37
    Prize: X=7870, Y=6450

    Button A: X+69, Y+23
    Button B: X+27, Y+71
    Prize: X=18641, Y=10279

Solution probable : Système d'équations linéaires


**/

using System;
using System.Diagnostics;
using Microsoft.VisualBasic;

namespace Day13
{
    class Program
    {

        struct Vector {
            public long X;
            public long Y;
        }

        struct ClawMachine {
            public Vector ButtonA;
            public Vector ButtonB;
            public Vector Prize;
        }

        /// <summary>
        /// Read the input and parse it into a list of ClawMachines
        /// All input is in format:
        ///   Button A: X+94, Y+34
        ///   Button B: X+22, Y+67
        ///   Prize: X=8400, Y=5400
        /// 
        /// </summary>
        /// <param name="input">the whole file</param>
        /// <returns></returns>

        static List<ClawMachine> ParseInput(string [] input)
        {
            List<ClawMachine> clawMachines = new List<ClawMachine>();

            for (int i=0; i < input.Length; i+=4)
            {
                ClawMachine clawMachine = new ClawMachine();
                clawMachine.ButtonA = new Vector();
                clawMachine.ButtonB = new Vector();
                clawMachine.Prize = new Vector();

                string [] buttonA = input[i].Split(" ");
                // Button A: X+94, Y+34
                // Remove the ","
                clawMachine.ButtonA.X = int.Parse(buttonA[2].Split("+")[1].Split(",")[0]);
                clawMachine.ButtonA.Y = int.Parse(buttonA[3].Split("+")[1]);

                string [] buttonB = input[i+1].Split(" ");
                clawMachine.ButtonB.X = int.Parse(buttonB[2].Split("+")[1].Split(",")[0]);
                clawMachine.ButtonB.Y = int.Parse(buttonB[3].Split("+")[1]);

                string [] prize = input[i+2].Split(" ");
                clawMachine.Prize.X = int.Parse(prize[1].Split("=")[1].Split(",")[0]);
                clawMachine.Prize.Y = int.Parse(prize[2].Split("=")[1]);                

                clawMachines.Add(clawMachine);
            }

            return clawMachines;
        }

        /// <summary>
        /// Greater Common Divisor
        /// </summary>
        static long GCD(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        /// <summary>
        /// Two equations in the form of:
        ///   a1 * x + b1 * y = c1
        ///   a2 * x + b2 * y = c2
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="b1"></param>
        /// <param name="c1"></param>
        /// <param name="a2"></param>
        /// <param name="b2"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        static (long x, long y)? SolveTwoEquations(long a1, long b1, long c1, long a2, long b2, long c2)
        {
            // Step 1: Eliminate one variable
            // Multiply the equations to align coefficients of `x` or `y`
            long lcmX = Math.Abs(a1 * a2) / GCD(Math.Abs(a1), Math.Abs(a2)); // LCM of a1 and a2
            long factor1 = lcmX / Math.Abs(a1);
            long factor2 = lcmX / Math.Abs(a2);

            // Adjust signs based on the coefficients
            factor1 = a1 > 0 ? factor1 : -factor1;
            factor2 = a2 > 0 ? -factor2 : factor2;

            // New equation: factor1 * Eq1 + factor2 * Eq2
            var newB = factor1 * b1 + factor2 * b2;
            var newC = factor1 * c1 + factor2 * c2;

            // If the new coefficient of `y` is zero, the equations are inconsistent or dependent
            if (newB == 0)
            {
                return null;
            }

            // Step 2: Solve for `y`
            if (newC % newB != 0)
            {
                return null; // No integer solution exists
            }

            var y = newC / newB;

            // Step 3: Substitute `y` into one of the original equations to solve for `x`
            long x;
            if (a1 != 0)
            {
                if ((c1 - b1 * y) % a1 != 0)
                {
                    return null; // No integer solution exists
                }
                x = (c1 - b1 * y) / a1;
            }
            else if (a2 != 0)
            {
                if ((c2 - b2 * y) % a2 != 0)
                {
                    return null; // No integer solution exists
                }
                x = (c2 - b2 * y) / a2;
            }
            else
            {
                return null; // No solution
            }

            return (x, y);
        }        
        static long PartA(string [] input, bool partB = false, bool verbose = false)
        {
            var clawMachines = ParseInput(input);
            
            long sum = 0;

            foreach (var clawMachine in clawMachines)
            {
                long prizeX = clawMachine.Prize.X;
                long prizeY = clawMachine.Prize.Y;

                if (partB) {
                    prizeX += 10000000000000;
                    prizeY += 10000000000000;
                }

                var solution = SolveTwoEquations(clawMachine.ButtonA.X, clawMachine.ButtonB.X, prizeX, clawMachine.ButtonA.Y, clawMachine.ButtonB.Y, prizeY);

                // A solution is valid if the solution gives integer values for x and y
                if (solution != null) {
                    // Check if item1 and item2 are integers
                    if (solution.Value.x % 1 == 0 && solution.Value.y % 1 == 0)
                    {
                        sum += solution.Value.x * 3 + solution.Value.y;

                        if (verbose)
                        {
                            Console.WriteLine("Button A: " + solution.Value.x + " Button B: " + solution.Value.y);
                        }
                    }
                }
            }

            return sum;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Day 13 - Claw Contraption");

            string filename = "input.txt";
            string[] input = System.IO.File.ReadAllLines(filename);

            var partA = PartA(input, false);
            var partB = PartA(input, true);

            Console.WriteLine("Part A: " + partA);
            Console.WriteLine("Part B: " + partB);
        }
    }
}
