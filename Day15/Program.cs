/**

--- Day 15: Warehouse Woes ---
Src : https://adventofcode.com/2024/day/15

Input file example:
########
#..O.O.#
##@.O..#
#...O..#
#.#.O..#
#...O..#
#......#
########

<^^>>>vv<v>>v<<

File content description
    - The content is separated in two blocks by an empty line
    - The first block is the map of the warehouse
        - The map is a rectangle of '#' and '.' characters
        - The map is surrounded by '#' characters
        - The map contains the following characters:
            - '#' - Wall
            - '.' - Empty space
            - '@' - The player
            - 'O' - A box
    - The second block is the list of moves. Ignore the new lines and spaces

Problem description
    - The player can move in four directions: up, down, left, right
    - The player can push a box in the same directions
    - The player can't move through walls
    - The player can't push a box through walls or other boxes

Goal :
    - Find the final state of the warehouse after all the moves are executed
    - Calculate the position of each box in the warehouse
        - The row number is multiplied by 100 and the column number is added
    - The box value is 100 * row + column

In the :
    - example above, the result is 2028
    - example.txt, the result is 10092

Approach :
    - Like a game, we will simulate the moves of the player
    - Create a class for each object in the warehouse
        - Player (position)
        - Box (position, canMove)
        - Wall (position)
        - Map (player, boxes, walls)
    - Create a class for the warehouse

Part B:
    - Every thing is twice as wide
    - If the tile is #, the new map contains ## instead.
    - If the tile is O, the new map contains [] instead.
    - If the tile is ., the new map contains .. instead.
    - If the tile is @, the new map contains @. instead.
    - A box can now overlap two boxes and push them both



**/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;

namespace WarehouseSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "example3.txt"; // Replace with your file path
            string[] fileContent = File.ReadAllText(filePath).Split(new[] { "\r\n\r\n" }, StringSplitOptions.None);
            
            string[] mapInput = fileContent[0].Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            string movesInput = fileContent[1].Replace("\r\n", "").Replace(" ", "");

            var warehouse = new Warehouse(mapInput, true);
            Console.WriteLine("Initial Map:");
            warehouse.PrintMap();

            warehouse.SimulateMoves(movesInput);

            Console.WriteLine("Final Map:");
            warehouse.PrintMap();

            int result = warehouse.CalculateBoxPositions();
            Console.WriteLine($"Result: {result}");
        }
    }

    class Warehouse
    {
        private char[,] map;
        private (int row, int col) playerPosition;
        private List<(int row, int col)> boxes;

        public Warehouse(string[] mapInput, bool grow = false)
        {
            int rows = mapInput.Length;
            int cols = mapInput[0].Length;

            if (grow)
            {
                map = new char[rows, cols * 2];
            } else {
                map = new char[rows, cols];
            }

            
            boxes = new List<(int row, int col)>();

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    var currentChar = mapInput[r][c];

                    if (grow)
                    {
                        switch (currentChar)
                        {
                            case '#':
                                map[r, c * 2] = '#';
                                map[r, c * 2 + 1] = '#';
                                break;
                            case 'O':
                                map[r, c * 2] = '[';
                                map[r, c * 2 + 1] = ']';
                                break;
                            case '.':
                                map[r, c * 2] = '.';
                                map[r, c * 2 + 1] = '.';
                                break;
                            case '@':
                                playerPosition = (r, c * 2);
                                map[r, c * 2] = '@';
                                map[r, c * 2 + 1] = '.';
                                break;
                        }
                        

                    }
                    else
                    {
                        map[r, c] = mapInput[r][c];
                        if (map[r, c] == '@')
                        {
                            playerPosition = (r, c);
                        }
                        else if (map[r, c] == 'O')
                        {
                            boxes.Add((r, c));
                        }
                    }
                }
            }
        }

        public void SimulateMoves(string moves)
        {
            foreach (char move in moves)
            {
                MakeMove(move);
                DisplayMap(false, move);
            }
        }

        private void MakeMove(char direction)
        {
            (int dr, int dc) = GetDirection(direction);
            (int newRow, int newCol) = (playerPosition.row + dr, playerPosition.col + dc);

            // If the next position is a wall, do nothing
            if (map[newRow, newCol] == '#') {

            } else if (map[newRow, newCol] == 'O')
            {
                if (CanMoveBoxChain(newRow, newCol, dr, dc))
                {
                    MoveBoxChain(newRow, newCol, dr, dc);
                    // Move the player to the box's previous position
                    map[newRow, newCol] = '@';
                    map[playerPosition.row, playerPosition.col] = '.';
                    playerPosition = (newRow, newCol);
                }
            } else if (map[newRow, newCol] == '[' || map[newRow, newCol] == ']')
            {
                if (CanMoveBoxChainWide(newRow, newCol, dr, dc))
                {
                    MoveBoxChainWide(newRow, newCol, dr, dc);
                    // Move the player to the box's previous position
                    map[newRow, newCol] = '@';
                    map[playerPosition.row, playerPosition.col] = '.';
                    playerPosition = (newRow, newCol);
                }
            }
            else
            {
                // If the next position is empty, move the player
                map[newRow, newCol] = '@';
                map[playerPosition.row, playerPosition.col] = '.';
                playerPosition = (newRow, newCol);
            }
        }

        private (int dr, int dc) GetDirection(char move)
        {
            return move switch
            {
                '^' => (-1, 0),
                'v' => (1, 0),
                '<' => (0, -1),
                '>' => (0, 1),
                _ => (0, 0),
            };
        }

        /// <summary>
        /// Now the boxes are represented by two characters '[]'
        /// They can now overlap between two cells when going up or down
        /// </summary>
        /// <param name="row">Current box row</param>
        /// <param name="col">Current box col</param>
        /// <param name="dr">Movement row direction</param>
        /// <param name="dc">Movement col direction</param>
        /// <returns></returns>      
        private bool CanMoveBoxChainWide(int row, int col, int dr, int dc)
        {
            // Calculate the position the box would move to
            int nextRow = row + dr;
            int nextCol = col + dc;

            if(map[nextRow, nextCol] == '#') {
                return false;
            } else if (map[nextRow, nextCol] == '[' || map[nextRow, nextCol] == ']') {
                if (dr == 0) {
                    return CanMoveBoxChainWide(nextRow, nextCol, dr, dc);
                }
                // We need to check both sides of the box
                if (map[nextRow, nextCol] == '[') {
                    return CanMoveBoxChainWide(nextRow, nextCol, dr, dc) && CanMoveBoxChainWide(nextRow, nextCol + 1, dr, dc);
                } else {
                    return CanMoveBoxChainWide(nextRow, nextCol, dr, dc) && CanMoveBoxChainWide(nextRow, nextCol - 1, dr, dc);
                }
            }


            // If the next position is empty, the chain can move
            return map[nextRow, nextCol] == '.';
        }

        private void MoveBoxChainWide(int row, int col, int dr, int dc) {
            /// This is a recursive function that moves the box chain
            /// First we need to now if it's a vertical or horizontal movement
            /// If it's vertical, we need to move boxes that are splitted on half box
            /// If it's horizontal, we need to move boxes that are splitted on half box
            
            int nextRow = row + dr;
            int nextCol = col + dc;

            char currentChar = map[row, col];
            char nextChar = map[nextRow, nextCol];

            if (dc != 0) {
                // Horizontal movement
                // if the nextChar is a space, put the end of the box there put the start of the box in the current position
                // This depends on the direction of the movement

                // Console.WriteLine($"Before moving box chain");
                // PrintRow(row);
                if (nextChar == '.') {
                    map[nextRow, nextCol] = currentChar;
                    map[row, col] = '.';
                } else {
                    // If the nextChar is a box, move it first
                    MoveBoxChainWide(nextRow, nextCol, dr, dc);
                    // Move the current box to the next position
                    map[nextRow, nextCol] = currentChar;
                    map[row, col] = '.';
                }
                // Console.WriteLine($"After moving box chain");
                // PrintRow(nextRow);
            }
            else {
                // Vertical movement
                // We need to check if the next box is splitted in two
                if (nextChar == ']')
                {
                    /// We are on the right side of the box, we need to move the left side first
                    /// Then we move the right side
                    MoveBoxChainWide(nextRow, nextCol - 1, dr, dc);
                    MoveBoxChainWide(nextRow, nextCol, dr, dc);
                }
                else if (nextChar == '[')
                {
                    /// We are on the left side of the box, we need to move the right side first
                    /// Then we move the left side
                    /// We need to move the right side first
                    /// Then we move the left side
                    MoveBoxChainWide(nextRow, nextCol, dr, dc);
                    MoveBoxChainWide(nextRow, nextCol + 1, dr, dc);
                }
                else
                {
                    // The character is a space, we can move the box
                    map[nextRow, nextCol] = currentChar;
                    map[row, col] = '.';
                }                
            }

            

            


        }

        private bool CanMoveBoxChain(int row, int col, int dr, int dc)
        {
            // Calculate the position the box would move to
            int nextRow = row + dr;
            int nextCol = col + dc;

            if(map[nextRow, nextCol] == '#') {
                return false;
            } else if (map[nextRow, nextCol] == 'O') {
                return CanMoveBoxChain(nextRow, nextCol, dr, dc);
            }            

            // If the next position is empty, the chain can move
            return map[nextRow, nextCol] == '.';
        }


        private void MoveBoxChain(int row, int col, int dr, int dc)
        {
            int nextRow = row + dr;
            int nextCol = col + dc;

            // If the next position is a box, recursively move it first
            if (map[nextRow, nextCol] == 'O')
            {
                MoveBoxChain(nextRow, nextCol, dr, dc);
            }

            // Move the current box to the next position
            map[nextRow, nextCol] = 'O';
            map[row, col] = '.';

            // Update the box position in the list
            boxes.Remove((row, col));
            boxes.Add((nextRow, nextCol));
        }


        public int CalculateBoxPositions()
        {
            return boxes.Sum(box => 100 * box.row + box.col);
        }

        #region Display functions

        public void PrintRow(int row)
        {
            for (int c = 0; c < map.GetLength(1); c++)
            {
                Console.Write(map[row, c]);
            }
            Console.WriteLine();
        }

        public void PrintMap()
        {
            for (int r = 0; r < map.GetLength(0); r++)
            {
                for (int c = 0; c < map.GetLength(1); c++)
                {
                    Console.Write(map[r, c]);
                }
                Console.WriteLine();
            }
        }



        // Part B : Double the width of the map
        public void GrowMap()
        {
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);
            char[,] newMap = new char[rows, cols * 2];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    char currentChar = map[r, c];
                    int newCol = c * 2;

                    switch (currentChar)
                    {
                        case '#':
                            newMap[r, newCol] = '#';
                            newMap[r, newCol + 1] = '#';
                            break;
                        case 'O':
                            newMap[r, newCol] = '[';
                            newMap[r, newCol + 1] = ']';
                            break;
                        case '.':
                            newMap[r, newCol] = '.';
                            newMap[r, newCol + 1] = '.';
                            break;
                        case '@':
                            newMap[r, newCol] = '@';
                            newMap[r, newCol + 1] = '.';
                            break;
                    }
                }
            }
            map = newMap;
        }

        public void DisplayMap(bool debug = false, char move = ' ')
        {
            if (!Console.IsOutputRedirected) // Skip if no valid console handle
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
            }

            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map.GetLength(1); col++)
                {
                    Console.Write(map[row, col]);
                }
                Console.WriteLine(); // Move to the next line after each row
            }

            // if debug true wait for user next key press
            if (debug)
            {   
                Console.WriteLine($"Last move: {move}");
                Console.Read();
            }
            else {
                Thread.Sleep(200); // Pause to simulate animation
            }   
        }

        private void SaveMapAsImage(string outputDir, int frameIndex)
        {
            // Create an image with the same dimensions as the map
            int cellSize = 20; // Size of each "cell" in the map for the image
            int width = map.GetLength(1) * cellSize;
            int height = map.GetLength(0) * cellSize;

            using (Bitmap bitmap = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White); // Set background color

                // Set font and brush for drawing characters
                using (Font font = new Font("Courier New", 10))
                using (Brush brush = new SolidBrush(Color.Black))
                {
                    for (int row = 0; row < map.GetLength(0); row++)
                    {
                        for (int col = 0; col < map.GetLength(1); col++)
                        {
                            char currentChar = map[row, col];
                            string text = currentChar.ToString();

                            // Draw each character as text
                            g.DrawString(text, font, brush, col * cellSize, row * cellSize);
                        }
                    }
                }

                // if the output directory does not exist, create it
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                // Save the frame as a PNG image
                string fileName = Path.Combine(outputDir, $"frame_{frameIndex}.png");
                bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        #endregion

    }

    
}
