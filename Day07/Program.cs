/**

--- Day 7: Bridge Repair ---

Input example:
190: 10 19
3267: 81 40 27
83: 17 5
156: 15 6
7290: 6 8 6 15
161011: 16 10 13
192: 17 8 14
21037: 9 7 18 13
292: 11 6 16 20

Explanation for part 1:
    Each line represent an equation missing the operators + or *.
    The left part of the equation is the result of the operation.
    Some equation are not solvable which are invalid.
    The equation do NOT respect the order of operations.
    The equation are read from left to right.

In the example above only these lines are valid:
    190: 10 19
    3267: 81 40 27
    292: 11 6 16 20

The goal is to find the sum of the results of the valid equations.
    In the example above the result is 190 + 3267 + 292 = 3749

Explanation for part 2:
    A new operator is introduced, the || operator.
    The || combines the two numbers on its left and right.
    Eg.: 1 || 2 = 12
    Only some invalid equations can be solved with the || operator.

**/

using System.Data;
using System.Diagnostics;

namespace Day07
{
    class Program
    {
        static double EvaluateLeftToRight(string equation)
        {
            // Split the equation into parts (numbers and operators)
            string[] tokens = equation.Split(' ');
            double result = double.Parse(tokens[0]);

            for (int i = 1; i < tokens.Length; i += 2)
            {
                string op = tokens[i];
                double nextValue = double.Parse(tokens[i + 1]);

                // Perform the operation strictly in order
                switch (op)
                {
                    case "+":
                        result += nextValue;
                        break;
                    case "-":
                        result -= nextValue;
                        break;
                    case "*":
                        result *= nextValue;
                        break;
                    case "/":
                        if (nextValue == 0)
                            throw new DivideByZeroException("Division by zero is not allowed.");
                        result /= nextValue;
                        break;
                    case "||":
                        result = double.Parse(result.ToString() + nextValue.ToString());
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported operator: {op}");
                }
            }

            return result;
        }



        // Utility function to convert a number to a base-3 string with fixed width
        static string ConvertToBase3(int num, int width)
        {
            char[] result = new char[width];
            for (int i = 0; i < width; i++)
            {
                result[width - i - 1] = (char)('0' + (num % 3));
                num /= 3;
            }
            return new string(result);
        }

        static long IsValidPart2(string[] numbers, long result)
        {
            long answer = 0;
            // Number of required operators is the number of numbers - 1
            int operators = numbers.Length - 1;
            // Number of possible combinations is 3^operators
            int combinations = (int)Math.Pow(3, operators);

            object lockObj = new object(); // Lock for thread safety

            // Bruteforce all the possible combinations
            // In parallel baby!!
            Parallel.For(0, combinations, (i, state) =>
            {
                string ternary = ConvertToBase3(i, operators);

                string equation = numbers[0];
                for (int j = 0; j < operators; j++)
                {
                    string op = ternary[j] switch
                    {
                        '0' => "+",
                        '1' => "*",
                        '2' => "||",
                        _ => throw new InvalidOperationException("Invalid operator"),
                    };

                    equation += " " + op + " " + numbers[j + 1];
                }

                double value = EvaluateLeftToRight(equation);
                if ((long)value == result)
                {
                    lock (lockObj) // Ensure only one thread writes at a time
                    {
                        if (answer == 0) // Check if the answer has not been set yet
                        {
                            answer = result;
                            //Console.WriteLine(result + " = " + equation);
                            state.Stop(); // Stop other threads
                        }
                    }
                }
            });

            return answer;
        }

        static long IsValid(string[] numbers, long result)
        {
            long answer = 0;
            // Number of required operators is the number of numbers - 1
            int operators = numbers.Length - 1;
            // Number of possible combinations is 2^operators
            int combinations = (int)Math.Pow(2, operators);

            object lockObj = new object(); // Lock for thread safety

            // FIXME: Break the loop when a part of the combination is greater than the result
            // Bruteforce all the possible combinations
            // In parallel baby!!
            Parallel.For(0, combinations, (i, state) =>
            {
                string binary = Convert.ToString(i, 2).PadLeft(operators, '0');

                string equation = numbers[0];
                for (int j = 0; j < operators; j++)
                {
                    equation += binary[j] == '0' ? " + " : " * ";
                    equation += numbers[j + 1];
                }

                double value = EvaluateLeftToRight(equation);
                if ((long)value == result)
                {
                    lock (lockObj) // Ensure only one thread writes at a time
                    {
                        if (answer == 0) // Check if the answer has not been set yet
                        {
                            answer = result;
                            //Console.WriteLine(result + " = " + equation);
                            state.Stop(); // Stop other threads
                        }
                    }
                }
            });

            return answer;
        }

        static void Main(string[] args)
        {
            // Create a new Stopwatch instance
            Stopwatch stopwatch = Stopwatch.StartNew();


            string[] lines = File.ReadAllLines("justin.txt");
            long sum = 0;

            object lockObj = new object(); // Lock for thread-safe sum updates

            Parallel.ForEach(lines, line =>
            {
                string[] parts = line.Split(':');
                long result = long.Parse(parts[0]);
                string[] numbers = parts[1].Trim().Split(' ');
                long lineResult = IsValid(numbers, result);

                if (lineResult == 0)
                {
                    lineResult = IsValidPart2(numbers, result);
                }

                lock(lockObj) // Ensure only one thread writes at a time
                {
                    sum += lineResult;
                }
            });

            stopwatch.Stop();

            // Get the elapsed time as a TimeSpan
            TimeSpan ts = stopwatch.Elapsed;

            Console.WriteLine($"PartB : {sum} ({ts.TotalMilliseconds} ms)");


            Console.WriteLine(sum);
        }
    }
}