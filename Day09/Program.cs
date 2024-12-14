/**
Src : https://adventofcode.com/2024/day/9

--- Day 9: Disk Fragmenter ---

Input example:
2333133121414131402

Explanation - Part A:
    Every even index represents the number of files
    Every odd index represents the free space
    Each file ID is the index / 2 - 1

    If we represents the files with their ID and free space with dot, the input example will be:
    00...111...2...333.44.5555.6666.777.888899

    After we have to defragment the disk, by:
    - Moving the files from the end to the leftmost free space

    After defragmenting the disk, the output will be:
    0099811188827773336446555566..............

Part B :
    Everything is the same except the compression of the disk map
    - We swap full blocks of ids into the free spaces

    Output example:
    00...111...2...333.44.5555.6666.777.888899
    0099.111...2...333.44.5555.6666.777.8888..
    0099.1117772...333.44.5555.6666.....8888..
    0099.111777244.333....5555.6666.....8888..
    00992111777.44.333....5555.6666.....8888..
    
**/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Day09
{
    class Program
    {
        static long checkSum(int[] input)
        {
            long sum = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] != -1)
                    sum += input[i] * i;
            }
            return sum;
        }
        // Swap every -1 with the last digit in the diskMap
        static int[] CompressDiskMap(List<int> diskMap)
        {
            int[] compressedDiskMap = new int[diskMap.Count];
            int count = 0;
            int lastDigitIndex = diskMap.Count - 1;
            int lastValue = -1;
            int nbSpaces = diskMap.Count(x => x == -1);
            int nbSwaps = 0;
            foreach (int n in diskMap)
            {
                if (n == -1)
                {
                    // This is a free space and needs to be swapped with the last digit
                    // Keep track of the last digit index of the diskMap
                    lastValue = diskMap[lastDigitIndex--];
                    while (lastValue == -1)
                    {
                        lastValue = diskMap[lastDigitIndex];
                        lastDigitIndex--;
                    }
                    compressedDiskMap[count] = lastValue;
                    nbSwaps++;
                }
                else
                {
                    compressedDiskMap[count] = n;
                }
                count++;

                if (count >= diskMap.Count - nbSpaces)
                {
                    // We have swapped all the free spaces
                    break;
                }
            }

            return compressedDiskMap;
        }

        // Find the next free space in the diskMap and returns the index and the number of free spaces
        static (int, int) FindNextFreeSpace(List<int> diskMap, int startIndex)
        {
            int nbFreeSpaces = 0;
            int index = startIndex;

            // Find the next free space (-1) starting from the startIndex
            while (index < diskMap.Count && diskMap[index] != -1)
            {
                index++;
            }

            int firstFreeSpaceIndex = index;

            // Count the number of free spaces
            while (index < diskMap.Count && diskMap[index] == -1)
            {
                nbFreeSpaces++;
                index++;
            }

            if (index >= diskMap.Count)
            {
                // We have reached the end of the diskMap
                firstFreeSpaceIndex = -1;
            }

            return (firstFreeSpaceIndex, nbFreeSpaces);
        }

        // Swap the series of files with the free space
        static void SwapFilesWithFreeSpace(List<int> diskMap, int firstDigitIndex, int lastDigitIndex, int nextFreeSpaceIndex, int nbFiles)
        {
            for (int i = 0; i < nbFiles; i++)
            {
                diskMap[nextFreeSpaceIndex + i] = diskMap[firstDigitIndex + i];
                diskMap[firstDigitIndex + i] = -1;
            }
        }
        static int[] CompressDiskMapV2(List<int> diskMap)
        {
            int[] compressedDiskMap = new int[diskMap.Count];

            int lastDigitIndex = diskMap.Count - 1;
            int nbSpaces = diskMap.Count(x => x == -1);

            int maxSpaceLeft = -1;

            // Starting from the end of the diskMap
            // Find the first series of files
            // Find the first spot of free space that fits the series of files
            // Swap the series of files with the free space
            while (lastDigitIndex >= 0)
            {
                int lastValue = diskMap[lastDigitIndex];
                if (lastValue != -1)
                {
                    // This is a file
                    int firstDigitIndex = lastDigitIndex;

                    while (firstDigitIndex >= 1 && diskMap[firstDigitIndex] != -1 && diskMap[firstDigitIndex] == diskMap[firstDigitIndex - 1])
                    {
                        firstDigitIndex--;
                    }

                    int nbFiles = lastDigitIndex - firstDigitIndex + 1;
                    int nextFreeSpaceIndex = 0;
                    int nextFreeSpaceCount = 0;

                    // Find the next free space that fits the series of files
                    while (nextFreeSpaceCount < nbFiles && nextFreeSpaceIndex < diskMap.Count && nextFreeSpaceIndex != -1 && nextFreeSpaceIndex < firstDigitIndex)
                    {
                        (nextFreeSpaceIndex, nextFreeSpaceCount) = FindNextFreeSpace(diskMap, nextFreeSpaceIndex);

                        if (nextFreeSpaceCount > maxSpaceLeft)
                        {
                            maxSpaceLeft = nextFreeSpaceCount;
                        }

                        if (nextFreeSpaceIndex == -1)
                        {
                            // We have reached the end of the diskMap
                            break;
                        }

                        if (nextFreeSpaceIndex >= diskMap.Count)
                        {
                            // We have reached the end of the diskMap
                            nextFreeSpaceIndex = -1;
                            break;
                        }

                        if (nextFreeSpaceIndex + 1 >= firstDigitIndex)
                        {
                            nextFreeSpaceIndex = -1;
                            break;
                        }


                        if (nextFreeSpaceCount < nbFiles)
                        {
                            // The free space is not big enough
                            nextFreeSpaceIndex += nextFreeSpaceCount;
                            nextFreeSpaceCount = 0;
                        }
                    }

                    if (nextFreeSpaceIndex > 0)
                    {
                        // We have found a free space that fits the series of files
                        // Swap the series of files with the free space
                        SwapFilesWithFreeSpace(diskMap, firstDigitIndex, lastDigitIndex, nextFreeSpaceIndex, nbFiles);
                    }

                    lastDigitIndex -= nbFiles;
                }
                else
                {
                    lastDigitIndex--;
                }
            }

            // Convert the diskMap to an array
            for (int i = 0; i < diskMap.Count; i++)
            {
                compressedDiskMap[i] = diskMap[i];
            }

            return compressedDiskMap;
        }

        static List<int> GetDiskMap(string input)
        {
            List<int> diskMap = new List<int>();

            // Each char is a digit which represents the number of files or free space
            int pos = 0;
            foreach (char c in input)
            {
                int n = c - '0';
                for (int i = 0; i < n; i++)
                {
                    if (pos % 2 == 0)
                    {
                        diskMap.Add(pos / 2);
                    }
                    else
                    {
                        diskMap.Add(-1);
                    }
                }
                pos++;
            }
            return diskMap;
        }

        static void RedditMain(string filename) {
            try
            {
                // Read the file and parse the inputs
                string input = File.ReadAllLines(filename)[0];

                // Process the inputs
                long result = 0;

                try
                {
                    // Step 1: Parse input into integers
                    var parsedInput = input.Select(x => x).ToList();
                    Console.WriteLine("Parsed Input: " + string.Join(", ", parsedInput));

                    // Step 2: Wrap each integer in a single-item list
                    var wrappedNumbers = parsedInput.Select(x => new List<int> { x }).ToList();
                    Console.WriteLine("Wrapped Numbers: " + string.Join(" | ", wrappedNumbers.Select(w => $"[{string.Join(", ", w)}]")));

                    // Step 3: Get end numbers for each single-item list
                    var endNumbers = wrappedNumbers.Select(GetEndNumbers).ToList();
                    Console.WriteLine("End Numbers: " + string.Join(" | ", endNumbers.Select(ends => 
                        string.Join(", ", ends.Select(e => $"({e.first}, {e.last})")))));

                    // Step 4: Extract first numbers from each end number list
                    var firstNumbers = endNumbers.Select(ends => ends.Select(e => e.first)).ToList();
                    Console.WriteLine("First Numbers: " + string.Join(" | ", firstNumbers.Select(f => $"[{string.Join(", ", f)}]")));

                    // Step 5: Reverse the list of first numbers
                    var reversedFirstNumbers = firstNumbers.Select(f => f.Reverse().ToList()).ToList();
                    Console.WriteLine("Reversed First Numbers: " + string.Join(" | ", reversedFirstNumbers.Select(f => $"[{string.Join(", ", f)}]")));

                    // Step 6: Aggregate first numbers by subtracting them (b - a)
                    var aggregatedNumbers = reversedFirstNumbers.Select(f => f.Aggregate((a, b) => b - a)).ToList();
                    Console.WriteLine("Aggregated Numbers: " + string.Join(", ", aggregatedNumbers));

                    // Step 7: Sum the aggregated numbers
                    result = aggregatedNumbers.Sum();
                    Console.WriteLine("Final Result: " + result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }


                Console.WriteLine($"Result: {result}");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Error: File '{filename}' not found.");
            }
            catch (FormatException)
            {
                Console.WriteLine("Error: Input file contains invalid numbers.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates the first and last numbers of each transformation step.
        /// </summary>
        private static List<(int first, int last)> GetEndNumbers(List<int> input)
        {
            var endNumbers = new List<(int first, int last)> { (input.First(), input.Last()) };

            while (input.Count > 1) // Continue until only one number remains
            {
                // Calculate differences between consecutive elements
                var nextInput = input.Take(input.Count - 1)
                                     .Select((val, index) => input[index + 1] - val)
                                     .ToList();

                if (nextInput.Any())
                {
                    endNumbers.Add((nextInput.First(), nextInput.Last()));
                }

                input = nextInput; // Update input for the next iteration
            }

            return endNumbers;
        }
        

        static void Main(string[] args)
        {

            bool debug = true;
            string filename = "example.txt";
            //MrAudet(filename);
            RedditMain(filename); // Not working
            string[] input = File.ReadAllLines(filename);
            List<int> diskMap = GetDiskMap(input[0]);

            if (debug)
            {
                // Save the disk map to a file
                string diskMapStr = string.Join(".", diskMap);
                File.WriteAllText("diskMap.txt", diskMapStr);
            }


            int[] compressedDiskMap = CompressDiskMap(diskMap);
            long cs = checkSum(compressedDiskMap);

            int[] compressedDiskMapV2 = CompressDiskMapV2(diskMap);
            long cs2 = checkSum(compressedDiskMapV2);

            if (debug)
            {
                // Save the compressed disk map to a file
                string compressedDiskMapStr = string.Join(".", compressedDiskMap);
                File.WriteAllText("compressedDiskMap.txt", compressedDiskMapStr);
                // Console.WriteLine("CompressedDiskMap : " + compressedDiskMapStr);

                // Save the compressed disk map to a file
                File.WriteAllText("compressedDiskMapV2.txt", string.Join(".", compressedDiskMapV2));
            }

            Console.WriteLine("Part A: " + cs);
            Console.WriteLine("Part B: " + cs2);
        }
    }
}