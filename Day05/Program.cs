/**

--- Day 5: Print Queue ---

Input example:
47|53
97|13
97|61
97|47
75|29
61|13
75|53
29|13
97|29
53|29
61|53
97|53
61|29
47|13
75|47
97|75
47|61
75|61
47|29
75|13
53|13

75,47,61,53,29
97,61,53,29,13
75,29,13
75,97,47,61,53
61,13,29
97,13,75,29,47

// Rules
The first part X|Y are the rules, where X is the page number that must be printed before Y.
The second part X,Y,Z... are the pages that must be printed.

X must always be printed before Y and respect all the rules of the first part.

// Generic stuff
Read the input file
Parse the rules
Parse the list of pages to print

Part A:
Check if the rules are respected for the list of pages to print. Return a boolean.
If the rules are respected, get the middle page of the list.
Sum all middle page numbers.

Display the sum of all middle page numbers.

**/

using System;
using System.Data;
using System.Formats.Tar;
using System.Runtime.InteropServices;

namespace Day05
{
    class Program
    {
        struct Rule {
            public int X;
            public int Y;
        }
        // Parse rules
        static List<Rule> ParseRule(string[] lines) {
            List<Rule> rules = new List<Rule>();

            foreach (string line in lines) {
                if (line == "") {
                    break;
                }

                string[] parts = line.Split('|');
                Rule rule = new Rule();
                rule.X = int.Parse(parts[0]);
                rule.Y = int.Parse(parts[1]);
                rules.Add(rule);
                Console.WriteLine("Rule: " + rule.X + " | " + rule.Y);
            }
            return rules;
        }

        // Parse pages
        // Must return a list of pages to print
        static string[] ParsePrintQueue(string[] lines) {
            List<string> pages = new List<string>();

            bool start = false;
            foreach (string line in lines) {
                if (line == "") {
                    start = true;
                    continue;
                }

                if (start) {
                    pages.Add(line);
                    Console.WriteLine("Page: " + line);
                }
            }
            return pages.ToArray();
        }

        static int[] ParsePages(string page) {
            string[] parts = page.Split(',');
            int[] pages = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++) {
                pages[i] = int.Parse(parts[i]);
            }
            return pages;
        }

        static bool ValidateQueue(List<Rule> rules, int[] pages) {
            foreach (int page in pages)
            {
                // Find the rules that contains the page
                List<Rule> foundRules = rules.FindAll(r => r.Y == page);
                foreach (Rule rule in foundRules) {
                    // In the remaining pages after the current page,
                    // check if the pages break the rule
                    
                    // LINQ : Get the remaining pages
                    int[] remainingPages = pages.SkipWhile(p => p != page).ToArray();

                    // if the remaining pages contains the page X
                    if (Array.IndexOf(remainingPages, rule.X) > -1) {
                        return false;
                    }
                }
            }
            return true;
        }

        static int PartA(List<Rule> rules, string[] queue) {
            int sum = 0;
            foreach (string page in queue) {
                int[] pages = ParsePages(page);
                if (ValidateQueue(rules, pages)) {
                    int middle = pages[pages.Length / 2];
                    sum += middle;
                } 
            }
            return sum;
        }

        // Fix the queue by applying the rules
        static int[] FixQueue(List<Rule> rules, int[] pages) {
            // LINQ to clone the array
            int[] fixedPages = [.. pages];

            bool allFixed = false;

            while (!allFixed) {
                allFixed = true;
                foreach (Rule rule in rules) {
                    int indexX = Array.IndexOf(fixedPages, rule.X);
                    int indexY = Array.IndexOf(fixedPages, rule.Y);

                    if (indexX > -1 && indexY > -1) {
                        if (indexX > indexY) {
                            int temp = fixedPages[indexX];
                            fixedPages[indexX] = fixedPages[indexY];
                            fixedPages[indexY] = temp;
                            allFixed = false;
                        }
                    }
                }
            }
            return fixedPages;
        }

        // For each of the incorrectly-ordered updates, use the page ordering rules to put the page numbers in the right order.
        static int PartB(List<Rule> rules, string[] queue) {
            int sum = 0;
            foreach (string page in queue) {
                int[] pages = ParsePages(page);
                if (!ValidateQueue(rules, pages)) {
                    pages = FixQueue(rules, pages);
                    int middle = pages[pages.Length / 2];
                    sum += middle;
                
                } 
            }
            return sum;
        }

        static void Main(string[] args)
        {
            string filename = "input.txt";
            string[] lines = System.IO.File.ReadAllLines(filename);

            List<Rule> rules = ParseRule(lines);
            string[] pages = ParsePrintQueue(lines);

            int result = 0; 
            result = PartA(rules, pages);
            Console.WriteLine("Part A: " + result);

            result = PartB(rules, pages);
            Console.WriteLine("Part B: " + result);
        }
    }
}