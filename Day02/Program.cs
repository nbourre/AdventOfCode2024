// Input example
// 7 6 4 2 1
// 1 2 7 8 9
// 9 7 6 2 1
// 1 3 2 4 5
// 8 6 4 4 1
// 1 3 6 7 9

// Generic stuff
// Read the input.txt file
// Send the data to the function partA
// print the result

// Part A
// Rules
// A row is valid if:
//   - The numbers are either all increasing or all decreasing.
//   - Any two adjacent numbers differ by at least one and at most three.
// 
// Return the number of valid rows.

// Part B
// Rules
// A row is valid if:
//   - Same as part A, except there can be one error

using System.Diagnostics;

namespace Day02
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = System.IO.File.ReadAllLines("input.txt");

            // Create a new Stopwatch instance
            Stopwatch stopwatch = Stopwatch.StartNew();

            int result = partA(lines);


            stopwatch.Stop();

            // Get the elapsed time as a TimeSpan
            TimeSpan ts = stopwatch.Elapsed;

            Console.WriteLine($"PartA : {result} ({ts.TotalMilliseconds} ms)");

            stopwatch.Reset();

            stopwatch.Start();
            result = partB(lines, true);

            stopwatch.Stop();
            ts = stopwatch.Elapsed;
            Console.WriteLine($"PartB : {result} ({ts.TotalMilliseconds} ms)");
        }

        // Function that receives a list of numbers and returns if it's valid or not
        // -1 if it's valid
        // if not, it returns the index where the error is
        static int isValidRow(int[] numbers, bool debug = false)
        {
            bool increasing = true;
            bool decreasing = true;

            int result = -1;

            for (int i = 0; i < numbers.Length - 1; i++)
            {
                if (numbers[i] < numbers[i + 1])
                {
                    decreasing = false;
                }
                else if (numbers[i] > numbers[i + 1])
                {
                    increasing = false;
                }

                int diff = Math.Abs(numbers[i] - numbers[i + 1]);

                result = i;

                if (diff > 3 || diff < 1)
                {
                    increasing = false;
                    decreasing = false;
                    return result;
                }

                if (!increasing && !decreasing)
                {
                    if (i == numbers.Length - 2)
                    {
                        result = i + 1;
                    }

                    if (i == 1) {
                        result = 0;
                    }

                    if (debug)
                    {
                        Console.WriteLine($"Invalid row: {string.Join(" ", numbers)}");
                    }
                    return result;
                }
            }
            return -1;            
        }

        static int partA(string[] lines)
        {
            int validRows = 0;
            foreach (string line in lines)
            {
                string[] numbers = line.Split(' ');
                int[] intNumbers = Array.ConvertAll(numbers, int.Parse);

                var result = isValidRow(intNumbers);

                if (result == -1)
                {
                    validRows++;
                }
            }
            return validRows;
        }

        static int partB(string[] lines, bool debug = false) 
        {
            int validRows = 0;
            foreach (string line in lines)
            {
                string[] numbers = line.Split(' ');
                int[] intNumbers = Array.ConvertAll(numbers, int.Parse);

                var result = isValidRow(intNumbers);

                if (result == -1)
                {
                    validRows++;
                } else {
                    // Check if we can fix the error
                    List<int> newNumbers = new List<int>();

                    // There can be only one error
                    // if after the fix the row still invalid, we can't fix it
                    for (int i = 0; i < intNumbers.Length; i++)
                    {
                        if ((i == result))
                        {
                            continue;
                        } 
                        newNumbers.Add(intNumbers[i]);
                    }

                    var newResult = isValidRow(newNumbers.ToArray());
                    if (newResult == -1)
                    {
                        validRows++;
                    } else
                    {
                        if (debug)
                        {
                            // put a * where the error is
                            string newLine = "";
                            for (int i = 0; i < intNumbers.Length; i++)
                            {
                                // if (i == result + 1)
                                // {
                                //     newLine += "*";
                                // }
                                newLine += $"{intNumbers[i]} ";
                            }

                            Console.WriteLine($"Invalid row: {newLine}");
                            //Console.WriteLine(newLine);
                        }
                    }
                    
                }
            }
            return validRows;
        }
    }
}


