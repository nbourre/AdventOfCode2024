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
        static long checkSum(int[] input) {
            long sum = 0;
            for (int i = 0; i < input.Length; i++) {
                sum += input[i] * i;
            }
            return sum;
        }
        // Swap every -1 with the last digit in the diskMap
        static int[] CompressDiskMap(List<int> diskMap) {
            int[] compressedDiskMap = new int[diskMap.Count];
            int count = 0;
            int lastDigitIndex = diskMap.Count - 1;
            int lastValue = -1;
            int nbSpaces = diskMap.Count(x => x == -1);
            int nbSwaps = 0;
            foreach (int n in diskMap) {
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
                } else {
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

        static int[] CompressDiskMapV2(List<int> diskMap) {
            int[] compressedDiskMap = new int[diskMap.Count];
            
            int lastDigitIndex = diskMap.Count - 1;
            int nbSpaces = diskMap.Count(x => x == -1);
            int lastSwappedIndex = 0;
            int firstFreeSpaceIndex = 0;

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

                    int nbFiles = (lastDigitIndex - firstDigitIndex) + 1;

                    // Find the first free space that fits the series of files
                    while (firstFreeSpaceIndex < diskMap.Count && diskMap[firstFreeSpaceIndex] != -1)
                    {
                        compressedDiskMap[firstFreeSpaceIndex] = diskMap[firstFreeSpaceIndex];
                        firstFreeSpaceIndex++;
                    }

                    // Count the number of free spaces
                    int nbFreeSpaces = 0;
                    while (firstFreeSpaceIndex < diskMap.Count && diskMap[firstFreeSpaceIndex] == -1)
                    {
                        nbFreeSpaces++;
                        firstFreeSpaceIndex++;
                    }

                    // Check if the number of free spaces is enough to fit the series of files
                    if (nbFreeSpaces >= nbFiles)
                    {
                        // Swap the series of files with the free space
                        for (int i = 0; i < nbFiles; i++)
                        {
                            compressedDiskMap[firstFreeSpaceIndex - nbFreeSpaces + i] = diskMap[firstDigitIndex + i];
                        }
                    }
                    else
                    {
                        // Not enough free space to fit the series of files
                        // Swap the series of files with the free space
                        for (int i = 0; i < nbFiles; i++)
                        {
                            compressedDiskMap[lastSwappedIndex + i] = diskMap[firstDigitIndex + i];
                        }
                        lastSwappedIndex += nbFiles;
                    }
                    
                    lastDigitIndex -= nbFiles;
                }
                else
                {
                    lastDigitIndex--;
                }
            }

            
            

            return compressedDiskMap;
        }

        static List<int> GetDiskMap(string input) {
            List<int> diskMap = new List<int>();

            // Each char is a digit which represents the number of files or free space
            int pos = 0;
            foreach (char c in input) {
                int n = c - '0';
                for (int i = 0; i < n; i++) {
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

        static void Main(string[] args)
        {

            bool debug = false;
            string[] input = File.ReadAllLines("example.txt");
            List<int> diskMap = GetDiskMap(input[0]);

            int[] compressedDiskMap = CompressDiskMap(diskMap);
            int[] compressedDiskMapV2 = CompressDiskMapV2(diskMap);
            long cs = checkSum(compressedDiskMap);

            if (debug)
            {
                // Save the disk map to a file
                string diskMapStr = string.Join(".", diskMap);
                File.WriteAllText("diskMap.txt", diskMapStr);

                // Save the compressed disk map to a file
                string compressedDiskMapStr = string.Join(".", compressedDiskMap);
                File.WriteAllText("compressedDiskMap.txt", compressedDiskMapStr);
                // Console.WriteLine("CompressedDiskMap : " + compressedDiskMapStr);

                // Save the compressed disk map to a file
                File.WriteAllText("compressedDiskMapV2.txt", string.Join(".", compressedDiskMapV2));
            }
            
            Console.WriteLine("Part A: " + cs);
            Console.WriteLine("Part B: " + checkSum(compressedDiskMapV2));
        }
    }
}