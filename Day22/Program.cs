/**

--- Day 22: Monkey Market ---

Input example
1
10
100
2024

Each line contains the initial secret numbers of each buyer.
The algorithm to calculate the next secret number is :
  - Calculate the result of multiplying the secret number by 64. Then, mix this result into the secret number. Finally, prune the secret number.
  - Calculate the result of dividing the secret number by 32. Round the result down to the nearest integer. Then, mix this result into the secret number. Finally, prune the secret number.
  - Calculate the result of multiplying the secret number by 2048. Then, mix this result into the secret number. Finally, prune the secret number.

Each step of the above process involves mixing and pruning:
  - To mix a value into the secret number, calculate the bitwise XOR of the given value and the secret number. Then, the secret number becomes the result of that operation. (If the secret number is 42 and you were to mix 15 into the secret number, the secret number would become 37.)
  - To prune the secret number, calculate the value of the secret number modulo 16777216. Then, the secret number becomes the result of that operation. (If the secret number is 100000000 and you were to prune the secret number, the secret number would become 16113920.)

Part A : Goal
  - Find the 2000th secret number of each buyer.
  - Sum the 2000th secret numbers of all buyers.

The result of the example input is :
1: 8685429
10: 4700978
100: 15273692
2024: 8667524

The sum of the 2000th secret numbers of all buyers is 37327623.

Part B: Steps
  - Generate all the first 2000 secret numbers of each buyer.
  - Keep the last digit of each secret number, which represents the price in banana the buyer is willing to pay.
  - Calculate the variation between each last digit.
  -   

**/

using System;

namespace Day22
{
    
    class Program
    {
        struct Buyer
        {
            public ulong secretNumber;
            public ulong secretNumber2000;
            public short[] variations;

            public Buyer(ulong secretNumber)
            {
                this.secretNumber = secretNumber;
                this.secretNumber2000 = 0;
            }
        }

        static ulong Mix(ulong value, ulong secretNumber)
        {
            return value ^ secretNumber;
        }

        static ulong Prune(ulong secretNumber)
        {
            return secretNumber % 16777216;
        }

        static ulong NextSecretNumber(ulong secretNumber)
        {
            ulong result = secretNumber * 64;
            secretNumber = Mix(result, secretNumber);
            secretNumber = Prune(secretNumber);

            result = secretNumber / 32;
            secretNumber = Mix(result, secretNumber);
            secretNumber = Prune(secretNumber);

            result = secretNumber * 2048;
            secretNumber = Mix(result, secretNumber);
            secretNumber = Prune(secretNumber);

            return secretNumber;
        }

        static ulong MonkeyMarket(ulong secretNumber, Buyer buyer)
        {
            for (int i = 0; i < 2000; i++)
            {
                buyer.variations[i] = (short)(secretNumber % 10);
                secretNumber = NextSecretNumber(secretNumber);
            }

            return secretNumber;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Day 22: Monkey Market");
            
            var filename = "example2.txt";
            var content = File.ReadAllLines(filename);

            ulong sum = 0;
            Buyer[] buyers = new Buyer[content.Length];
            for (int i = 0; i < content.Length; i++)
            {
                buyers[i].secretNumber = ulong.Parse(content[i]);
                buyers[i].variations = new short[2000];
            }

            // For debugging purpose display data from small files
            if (filename.StartsWith("example")) {
                for (int i = 0; i < content.Length; i++)
                {
                    buyers[i].secretNumber2000 = MonkeyMarket(buyers[i].secretNumber, buyers[i]);
                    sum += buyers[i].secretNumber2000;

                    // Print the 20 first secret numbers
                    for (int j = 0; j < 20; j++)
                    {
                        Console.Write($"{buyers[i].variations[j]}, ");
                    }

                    Console.WriteLine();
                    
                }

            } else {
                Parallel.For(0, content.Length, i =>
                {
                    buyers[i].secretNumber2000 = MonkeyMarket(buyers[i].secretNumber, buyers[i]);
                    sum += buyers[i].secretNumber2000;
                });
            }


            // Improve the performance by using parallel for loop


            Console.WriteLine($"Part A : The sum of the 2000th secret numbers of all buyers is {sum}");

        }
    }
}

/** CHEAT here
// SRC : https://shorturl.at/qQEq6

var input = File.ReadAllLines(args[0]);
MonkeyMarket(input, out var part1, out var part2, iterations: 2000);

Console.WriteLine($"Part 1: {part1}");
Console.WriteLine($"Part 2: {part2}");

// Part 1 can be super small, actually:
var part1OneLiner = input.Select(int.Parse).Sum(seed => (long) Mrng(seed).Skip(2_000).First());
Console.WriteLine($"Part 1 (short): {part1OneLiner}");

static IEnumerable<int> Mrng(int seed)
{
    var next = seed;
    while (true)
    {
        yield return next;
        next = (next ^ (next << 6)) & 0x00FFFFFF;
        next = (next ^ (next >> 5)) & 0x00FFFFFF;
        next = (next ^ (next << 11)) & 0x00FFFFFF;
    }
}

void MonkeyMarket(IEnumerable<string> seeds, out long part1, out int part2, int iterations = 2000)
{
    var intSeeds = seeds.Select(int.Parse).ToArray();
    var tt = new Dictionary<MarketChanges, int>();
    part1 = 0;
    
    foreach (var seed in intSeeds)
    {
        using var rng = Mrng(seed).GetEnumerator();
        var cc = new HashSet<MarketChanges>();
        var carry = new sbyte[4];
        int? last = default;
        for (int i = 0; i <= iterations && rng.MoveNext(); ++i)
        {
            int curr = rng.Current;
            var value = curr % 10;
            if (last.HasValue)
            {
                carry[i%4] = (sbyte)(value - last.Value);
            }

            if (i >= 4)
            {
                var key = new MarketChanges(carry[i%4], carry[(i-1)%4], carry[(i-2)%4], carry[(i-3)%4]);
                if (cc.Add(key))
                {
                    tt[key] = tt.GetValueOrDefault(key) + value;
                }
            }

            last = value;
        }
        part1 += rng.Current;
    }

    part2 = tt.Values.Max();
}

internal readonly record struct MarketChanges(sbyte A, sbyte B, sbyte C, sbyte D);

**/