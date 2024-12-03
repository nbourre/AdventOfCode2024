/**
Day03: Regular Expressions
Input example:
xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))

Generic stuff
Read the input.txt file
Send the data to the function partA
print the result

Part A:
Consider only the regular expression /mul\(\d+,\d+\)/
This regular expression matches any string that starts with "mul(" followed by two numbers separated by a comma and ends with a closing parenthesis.

mul is a function that multiplies two numbers. You need to implement this function.

Part B:
The regex is the same as part A, except there are now conditional statements that need to be implemented.
- There are do() and don't() in the inputs.
- When a do() is found the following mul() functions must be enabled.
- When a don't() is found the following mul() functions must be disabled.
- The first mul() function before the do() or don't() is always enabled.

Input example for B:
xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))

In this example mul(2,4) and mul(8, 5) are enabled, the rest are disabled.

**/

using System;
using System.Text.RegularExpressions;

namespace Day03
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = System.IO.File.ReadAllLines("input.txt");

            int result = partA(lines);

            Console.WriteLine($"Part A : {result}");
        }

        static int partB(string[] lines)
        {
            int result = 0;

            // Regular expression to extract all the mul(2,3) type strings
            string patternMul = @"mul\((\d+),(\d+)\)";
            Regex reMul = new Regex(patternMul, RegexOptions.Compiled);

            string patternDo = @"do\(\)";
            string patternDont = @"dont\(\)";

            Regex reDo = new Regex(patternDo, RegexOptions.Compiled);
            Regex reDont = new Regex(patternDont, RegexOptions.Compiled);

            bool enabled = true;

            MatchCollection matchesMul;
            MatchCollection matchesDo;
            MatchCollection matchesDont;

            foreach (string line in lines)
            {
                matchesDo = Regex.Matches(line, patternDo);
                matchesDont = Regex.Matches(line, patternDont);
                matchesMul = Regex.Matches(line, patternMul);

                // Get first dont() positions and set the flags
                var firstDont = matchesDont[0].Index;

                // Execute all mul() before the first dont()
                foreach (Match match in matchesMul)
                {
                    if (match.Index < firstDont)
                    {
                        int num1 = int.Parse(match.Groups[1].Value);
                        int num2 = int.Parse(match.Groups[2].Value);

                        result += num1 * num2;
                    }
                }
            }


            return result;
        }

        static int partA(string[] lines)
        {
            int result = 0;

            // Regular expression to extract all the mul(2,3) type strings
            string pattern = @"mul\((\d+),(\d+)\)";

            Regex regex = new Regex(pattern, RegexOptions.Compiled);

            MatchCollection matches;
            foreach (string line in lines)
            {
                matches = Regex.Matches(line, pattern);

                foreach (Match match in matches)
                {
                    int num1 = int.Parse(match.Groups[1].Value);
                    int num2 = int.Parse(match.Groups[2].Value);

                    result += num1 * num2;
                }
            }

            return result;
        }
    }
}


