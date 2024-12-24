/**

--- Day 24: Crossed Wires ---
Src : https://adventofcode.com/2024/day/24

Input example :
x00: 1
x01: 1
x02: 1
y00: 0
y01: 1
y02: 0

x00 AND y00 -> z00
x01 XOR y01 -> z01
x02 OR y02 -> z02

The input file is separated in two parts.
The first part are the wire names with their initial values which are binary.

The second part are binary equations using the wire names and bitwise operations.
The operations are AND, OR and XOR. The output of the equation is a wire name.

Part 1:
- Find all the values of the wires starting with 'z'
- Merge the values of the wires starting with 'z' to get the final value.
- z00 is the least significant bit and so on.

The example above will output 100 which is 4 in decimal (The answer).

**/

using System;
using System.Formats.Asn1;

namespace Day24
{
    class Program
    {
        // Equation class
        class Equation
        {
            public string Wire1;
            public string Wire2;
            public string Wire3;
            public string Operation;

            public Equation(string wire1, string wire2, string wire3, string operation)
            {
                Wire1 = wire1;
                Wire2 = wire2;
                Wire3 = wire3;
                Operation = operation;
            }

            public bool IsSolvable(Dictionary<string, ulong> wires)
            {
                return (int.TryParse(Wire1, out int value1) || wires.ContainsKey(Wire1)) &&
                       (int.TryParse(Wire2, out int value2) || wires.ContainsKey(Wire2));
            }

            public bool IsSolved(Dictionary<string, ulong> wires)
            {
                return wires.ContainsKey(Wire1) && wires.ContainsKey(Wire2);
            }

            public ulong Calculate(Dictionary<string, ulong> wires)
            {
                ulong value1 = 0;
                ulong value2 = 0;
                if (ulong.TryParse(Wire1, out value1) == false)
                {
                    value1 = wires[Wire1];
                }
                if (ulong.TryParse(Wire2, out value2) == false)
                {
                    value2 = wires[Wire2];
                }

                switch (Operation)
                {
                    case "AND":
                        return value1 & value2;
                    case "OR":
                        return value1 | value2;
                    case "XOR":
                        return value1 ^ value2;
                    default:
                        return 0;
                }
            }
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Day 24 - Advent of Code 2024");

            // Read the input file
            string[] lines = File.ReadAllLines("input.txt");

            // Read the first part of the input
            Dictionary<string, ulong> wires = new Dictionary<string, ulong>();
            int i = 0;
            while (lines[i] != "")
            {
                string[] parts = lines[i].Split(": ");
                wires[parts[0]] = Convert.ToUInt64(parts[1]);
                i++;
            }

            // Read the equations of the input
            List<Equation> equations = new List<Equation>();
            for (int j = i + 1; j < lines.Length; j++)
            {
                string[] parts = lines[j].Split(" ");
                equations.Add(new Equation(parts[0], parts[2], parts[4], parts[1]));
            }
            
            // Solve the equations by adding the values to the wires
            while (equations.Count > 0)
            {
                for (int j = 0; j < equations.Count; j++)
                {
                    if (equations[j].IsSolved(wires))
                    {
                        // if the wire doesn't exist, add it
                        if (wires.ContainsKey(equations[j].Wire3) == false)
                        {
                            wires[equations[j].Wire3] = equations[j].Calculate(wires);
                        }

                        equations.RemoveAt(j);
                    }
                }
            }

            // Sort the wires by name
            wires = wires.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            wires.ToList().ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));

            // Merge the values of the wires starting with 'z' to get the final value
            ulong result = 0;
            int bit = 0;
            foreach (var wire in wires)
            {
                if (wire.Key.StartsWith("z"))
                {
                    result += wire.Value * (ulong)Math.Pow(2, bit);
                    bit++;
                }
            }

            Console.WriteLine($"Result: {result}");
        }

    }
}