// Read the file input.txt
// The file contains a list of numbers separated by spaces
// For example:
// 3   4
// 4   3
// 2   5
// 1   3
// 3   9
// 3   3

// Part A
// We must split the numbers into two lists
// Sort each list
// Substract the the second list from the first list
// Sum the result
// The result in the example above is 2 + 1 + 0 + 1 + 2 + 5 = 11

// Part B
// We must count the number of times the number in the first list appears in the second list
// And then multiply the number of times by the number
// Repeat for all numbers in the first list
// Sum the result
// Optimization : Save the number of times a number appears in the second list in a dictionary
//   Eg : 3 appears 3 times in the second list
// The result in the example above is 9 + 4 + 0 + 0 + 9 + 9


using System;

namespace Day01
{
    class Program
    {

        static int partA(int[] list1, int[] list2)
        {
            int sum = 0;

            // No need to sort the lists since the numbers are already sorted
            // Array.Sort(list1);
            // Array.Sort(list2);

            // Part A
            for (int i = 0; i < list1.Length; i++)
            {
                sum += Math.Abs(list1[i] - list2[i]);
            }

            return sum;
        }

        static int partB(int[] list1, int[] list2)
        {
            int sum = 0;
            Dictionary<int, int> dict = new Dictionary<int, int>();

            // Part B
            for (int i = 0; i < list1.Length; i++)
            {
                if (dict.ContainsKey(list1[i]))
                {
                    sum += dict[list1[i]] * list1[i];
                    continue;
                }else {
                    int count = list2.Count(x => x == list1[i]);
                    dict[list1[i]] = count;
                    sum += count * list1[i];
                }                
            }

            return sum;
        }

        static void Main(string[] args)
        {
            // Read the file input.txt in the same folder as the program
            string[] lines = System.IO.File.ReadAllLines("input.txt");

            int[] list1 = new int[lines.Length];
            int[] list2 = new int[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] numbers = lines[i].Split("   ");
                list1[i] = int.Parse(numbers[0]);
                list2[i] = int.Parse(numbers[1]);
            }

            Array.Sort(list1);
            Array.Sort(list2);

            int sum = partA(list1, list2);
            Console.WriteLine(sum);

            sum = partB(list1, list2);
            Console.WriteLine(sum);
        }
    }
}