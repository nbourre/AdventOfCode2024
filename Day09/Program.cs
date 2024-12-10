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
    
**/

using System;

namespace Day09
{
    class Program
    {
        static List<char> GetDiskMap(string input) {
            List<char> diskMap = new List<char>();

            return diskMap;
        }

        static void Main(string[] args)
        {
            string[] input = File.ReadAllLines("example.txt");
        }
    }
}