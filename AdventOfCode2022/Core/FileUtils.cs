namespace AdventOfCode2022.Core;

public static class FileUtils
{
    public static List<string> ReadLines(string fileName)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string filePath = System.IO.Path.Combine(currentDirectory, "Resources", fileName);
        
        return new List<string>(File.ReadAllLines(filePath));
    }
}