/**

--- Day 21: Keypad Conundrum ---

Keypad representation:

+---+---+---+
| 7 | 8 | 9 |
+---+---+---+
| 4 | 5 | 6 |
+---+---+---+
| 1 | 2 | 3 |
+---+---+---+
    | 0 | A |
    +---+---+

Direction keypad representation:
    +---+---+
    | ^ | A |
+---+---+---+
| < | v | > |
+---+---+---+

Input example:
029A
980A
179A
456A
379A

In front of a keypad there is a robot. It can move in 4 directions: ^, v, <, >.
On each keypad, A is the confirmation button to send the command.
It is not possible to move to the blank space.

In chain there is 4 robots.
- 1 robot in front of the number keypad
- 1 robot in front of the direction keypad which controls the robot in front of the number keypad (high levels of radiation)
- 1 robot in front of the direction keypad which controls the robot in front of the direction keypad (-40 degrees)
- 1 human in front of the direction keypad which controls the robot in front of the direction keypad

Each robot starts at the position A.

Goal find the shortest path to type the given code for the human user.

For the example above the shortest path is for each line:
029A: <vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A
980A: <v<A>>^AAAvA^A<vA<AA>>^AvAA<^A>A<v<A>A>^AAAvA<^A>A<vA>^A<A>A
179A: <v<A>>^A<vA<A>>^AAvAA<^A>A<v<A>>^AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A
456A: <v<A>>^AA<vA<A>>^AAvAA<^A>A<vA>^A<A>A<vA>^A<A>A<v<A>A>^AAvA<^A>A
379A: <v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A

Solution :
  - The minimum number of moves between two keys is the Manhattan distance.
  - Generate a dictionnary of string and int with the shortest path to each button on the keypad
  - Eg : 0 to A will be denoted as 0A and value of 1, 2 to 9 will be denoted as "29" and a value of 3 (">^^").
**/

using System;

namespace Day21
{
    class Program
    {
        static (int row, int col) FindKey(char[,] keys, char key)
        {
            int rows = keys.GetLength(0);
            int cols = keys.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (keys[i, j] == key)
                    {
                        return (i, j);
                    }
                }
            }

            return (-1, -1);
        }

        static int DistanceBetweenKeys((int row, int col) start, (int row, int col) end)
        {
            return Math.Abs(start.row - end.row) + Math.Abs(start.col - end.col);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Day 21 - Keypad Conundrum");
            Console.WriteLine("Shortest path to type the code:");

            char [,] keypads = new char[,] {
                { '7', '8', '9' },
                { '4', '5', '6' },
                { '1', '2', '3' },
                { ' ', '0', 'A' }
            };

            char [,] directions = new char[,] {
                { ' ', '^', 'A' },
                { '<', 'v', '>' }
            };

            Dictionary<string, int> shortestPathNum = new Dictionary<string, int>();
            Dictionary<string, int> shortestPathDir = new Dictionary<string, int>();

            // For each key in the keypad, find the shortest path to all other keys
            for (int i = 0; i < keypads.GetLength(0); i++)
            {
                for (int j = 0; j < keypads.GetLength(1); j++)
                {
                    char key = keypads[i, j];
                    (int row, int col) start = (i, j);

                    for (int k = 0; k < keypads.GetLength(0); k++)
                    {
                        for (int l = 0; l < keypads.GetLength(1); l++)
                        {
                            char endKey = keypads[k, l];
                            (int row, int col) end = (k, l);

                            string keyPair = $"{key}{endKey}";
                            int distance = DistanceBetweenKeys(start, end);
                            shortestPathNum[keyPair] = distance;
                        }
                    }
                }
            }

            // Same for the direction keypad
            for (int i = 0; i < directions.GetLength(0); i++)
            {
                for (int j = 0; j < directions.GetLength(1); j++)
                {
                    char key = directions[i, j];
                    (int row, int col) start = (i, j);

                    for (int k = 0; k < directions.GetLength(0); k++)
                    {
                        for (int l = 0; l < directions.GetLength(1); l++)
                        {
                            char endKey = directions[k, l];
                            (int row, int col) end = (k, l);

                            string keyPair = $"{key}{endKey}";
                            int distance = DistanceBetweenKeys(start, end);
                            shortestPathDir[keyPair] = distance;
                        }
                    }
                }
            }

            /// To press 0 on the 1st robot
            /// the 2nd robot has to press < and A
            /// the 3rd robot can press v, <, < and A to make the 2nd robot press the "<" key
            /// The human can press v, <, < and A to make the 3rd robot press the "<" key 
            /// The shortest path for the human is the distance between
            /// 

            string[] codes = new string[] { "029A", "980A", "179A", "456A", "379A" };

            foreach (string code in codes)
            {
                int totalDistance = 0;
                for (int i = 0; i < code.Length - 1; i++)
                {
                    string keyPair = $"{code[i]}{code[i + 1]}";

                    // TODO : Complete day 21
                    // Idea : For each character in the code, reculsively find the shortest path to the next character
                    // using a depth value to know which robot is in front of the keypad

                    // Robot in front of the number keypad
                    if (i == 0)
                    {
                        totalDistance += shortestPathNum[keyPair];
                    }                    
                }

                Console.WriteLine($"{code}: {totalDistance}");
            }
        }
    }
}