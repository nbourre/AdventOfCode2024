    //     static void MrAudet(string filename)
    //     {
    //         var start = DateTime.Now;

    //         // Parse input
    //         var input = ParseInput(filename);

    //         // Generate the array, files, and free space
    //         var (stringArray, files, freeSpace) = GetArrayWithSpace(input);

    //         // Sort the string array
    //         var sortedArray = SortStringArray(stringArray, files, freeSpace);

    //         // Calculate the solution
    //         var solution = GetSolution(sortedArray);

    //         Console.WriteLine($"Time elapsed: {DateTime.Now - start}");
    //         Console.WriteLine($"Solution: {solution}");
    //     }

    //     static List<int> ParseInput(string filePath)
    //     {
    //         return File.ReadAllText(filePath).Trim().Select(c => int.Parse(c.ToString())).ToList();
    //     }

    //     static (List<object>, List<FileInfo>, List<FreeSpaceInfo>) GetArrayWithSpace(List<int> array)
    //     {
    //         var stringArray = new List<object>();
    //         var files = new List<FileInfo>();
    //         var freeSpace = new List<FreeSpaceInfo>();
    //         int indexAt = 0;

    //         for (int i = 0; i < array.Count; i++)
    //         {
    //             int number = array[i];

    //             if (i % 2 == 0)
    //             {
    //                 // Files
    //                 files.Add(new FileInfo
    //                 {
    //                     Value = indexAt,
    //                     Size = number,
    //                     StartAt = stringArray.Count
    //                 });

    //                 for (int j = 0; j < number; j++)
    //                 {
    //                     stringArray.Add(indexAt);
    //                 }

    //                 indexAt++;
    //             }
    //             else
    //             {
    //                 // Free space
    //                 freeSpace.Add(new FreeSpaceInfo
    //                 {
    //                     Size = number,
    //                     StartAt = stringArray.Count
    //                 });

    //                 for (int j = 0; j < number; j++)
    //                 {
    //                     stringArray.Add(".");
    //                 }
    //             }
    //         }

    //         files.Reverse(); // Reverse the files list as in Ruby
    //         return (stringArray, files, freeSpace);
    //     }

    //     static List<object> SortStringArray(List<object> stringArray, List<FileInfo> files, List<FreeSpaceInfo> freeSpace)
    //     {
    //         foreach (var file in files)
    //         {
    //             foreach (var space in freeSpace)
    //             {
    //                 if (space.Size >= file.Size && file.StartAt >= space.StartAt)
    //                 {
    //                     for (int i = 0; i < file.Size; i++)
    //                     {
    //                         stringArray[space.StartAt + i] = file.Value;
    //                         stringArray[file.StartAt + i] = ".";
    //                     }

    //                     space.Size -= file.Size;
    //                     space.StartAt += file.Size;

    //                     break;
    //                 }
    //             }
    //         }

    //         return stringArray;
    //     }

    //     static int GetSolution(List<object> sortedArray)
    //     {
    //         int count = 0;

    //         for (int i = 0; i < sortedArray.Count; i++)
    //         {
    //             if (sortedArray[i] is int number)
    //             {
    //                 count += number * i;
    //             }
    //         }

    //         return count;
    //     }
    // }

    // // Helper class for file information
    // class FileInfo
    // {
    //     public int Value { get; set; }
    //     public int Size { get; set; }
    //     public int StartAt { get; set; }
    // }

    // // Helper class for free space information
    // class FreeSpaceInfo
    // {
    //     public int Size { get; set; }
    //     public int StartAt { get; set; }
    // }
