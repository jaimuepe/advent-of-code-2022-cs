using System.Text.RegularExpressions;

namespace AdventOfCode2022.Core;

public abstract partial class Problem
{
    private int ProblemIndex => int.Parse(ProblemRegex().Match(GetType().Name).Groups[1].Value);

    private string ProblemIndexStr => ProblemIndex.ToString("00");

    protected bool IsTest { get; private set; }

    protected EProblemType ProblemType { get; private set; }

    public void RunA(bool isTest)
    {
        ProblemType = EProblemType.A;
        Run_Internal(isTest, RunA_Internal);
    }

    public void RunB(bool isTest)
    {
        ProblemType = EProblemType.B;
        Run_Internal(isTest, RunB_Internal);
    }

    protected abstract void RunA_Internal(List<string> lines);

    protected abstract void RunB_Internal(List<string> lines);

    private void Run_Internal(bool isTest, Action<List<string>> internalFcn)
    {
        IsTest = isTest;

        WriteLine();
        if (IsTest)
        {
            WriteLine($"### DAY {ProblemIndexStr} {ProblemType} - TEST ###");
        }
        else
        {
            WriteLine($"### DAY {ProblemIndexStr} {ProblemType} ###");
        }
        WriteLine();

        var fileName = (isTest ? ProblemIndexStr + "_TEST" : ProblemIndexStr) + ".txt";

        var lines = FileUtils.ReadLines(fileName);
        internalFcn(lines);
    }

    protected void Write(char c)
    {
        SetupConsole();
        Console.Write(c);
    }

    protected void Write(string line)
    {
        SetupConsole();
        Console.Write(line);
    }

    protected void WriteLine(char c)
    {
        SetupConsole();
        Console.WriteLine(c);
    }

    protected void WriteLine()
    {
        Console.WriteLine();
    }

    protected void WriteLine(string line)
    {
        SetupConsole();
        Console.WriteLine(line);
    }

    private void SetupConsole()
    {
        Console.ForegroundColor = IsTest ? ConsoleColor.Yellow : ConsoleColor.White;
    }

    [GeneratedRegex(@"Problem(\d+)")]
    private static partial Regex ProblemRegex();
}