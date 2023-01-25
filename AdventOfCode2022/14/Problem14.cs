using AdventOfCode2022.Core;

namespace AdventOfCode2022._14;

public class Problem14 : Problem
{
    private OffsetGrid _grid = null!;

    protected override void RunA_Internal(List<string> lines)
    {
        _grid = CreateGrid(lines);

        int count = 0;
        while (RunSimulation())
        {
            count++;
        }

        WriteLine($"Result: {count}");
    }

    protected override void RunB_Internal(List<string> lines)
    {
        _grid = CreateGrid(lines);

        var count = 0;
        while (!IsTileBlocked(500, 0) && RunSimulation())
        {
            count++;
        }

        WriteLine($"Result: {count}");
    }

    private bool RunSimulation()
    {
        int grainX = 500;
        int grainY = -1;

        while (true)
        {
            if (IsOutOfBounds(grainX, grainY + 1))
            {
                return false;
            }
            else if (!IsTileBlocked(grainX, grainY + 1))
            {
                grainY++;
            }
            else if (IsOutOfBounds(grainX - 1, grainY + 1))
            {
                return false;
            }
            else if (!IsTileBlocked(grainX - 1, grainY + 1))
            {
                grainX--;
                grainY++;
            }
            else if (IsOutOfBounds(grainX + 1, grainY + 1))
            {
                return false;
            }
            else if (!IsTileBlocked(grainX + 1, grainY + 1))
            {
                grainX++;
                grainY++;
            }
            else
            {
                _grid.Set('o', grainX, grainY);
                return true;
            }
        }
    }

    private bool IsOutOfBounds(int x, int y)
    {
        return _grid.IsOutOfBounds(x, y);
    }

    private bool IsTileBlocked(int x, int y)
    {
        char tile = _grid.Get(x, y);
        return tile == '#' || tile == 'o';
    }

    private OffsetGrid CreateGrid(List<string> lines)
    {
        List<Path> rocks = ParsePaths(lines);

        var minX = int.MaxValue;
        const int minY = 0;
        var maxX = int.MinValue;
        var maxY = int.MinValue;

        foreach (Path rock in rocks)
        {
            foreach (Point point in rock.Points)
            {
                minX = Math.Min(minX, point.X);
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
            }
        }

        if (ProblemType == EProblemType.B)
        {
            // assume the floor is an infinite horizontal line with a y coordinate equal
            // to two plus the highest y coordinate of any point in your scan

            maxY = 2 + maxY;

            // I'm just going to try to brute force it
            minX -= 200;
            maxX += 200;
        }

        int cols = maxX - minX + 1;
        int rows = maxY - minY + 1;

        OffsetGrid grid = new(rows, cols, new Point() { X = minX, Y = minY });

        if (ProblemType == EProblemType.B)
        {
            // add floor
            for (int i = minX; i <= maxX; i++)
            {
                grid.Set('#', i, maxY);
            }
        }

        AddRocks(rocks, grid);

        return grid;
    }

    private static void AddRocks(List<Path> rocks, OffsetGrid grid)
    {
        foreach (Path rock in rocks)
        {
            for (int i = 0; i < rock.Points.Count - 1; i++)
            {
                Point p1 = rock.Points[i];
                Point p2 = rock.Points[i + 1];

                if (p1.X == p2.X)
                {
                    // vertical
                    int from = p1.Y < p2.Y ? p1.Y : p2.Y;
                    int to = p1.Y < p2.Y ? p2.Y : p1.Y;

                    for (int j = from; j <= to; j++)
                    {
                        grid.Set('#', p1.X, j);
                    }
                }
                else
                {
                    // horizontal
                    int from = p1.X < p2.X ? p1.X : p2.X;
                    int to = p1.X < p2.X ? p2.X : p1.X;

                    for (int j = from; j <= to; j++)
                    {
                        grid.Set('#', j, p1.Y);
                    }
                }
            }
        }
    }

    private List<Path> ParsePaths(List<string> lines)
    {
        List<Path> paths = new List<Path>();

        foreach (string line in lines)
        {
            Path path = new();

            string[] point = line.Split("->");

            foreach (string token in point)
            {
                string[] coords = token.Trim().Split(",");

                int x = int.Parse(coords[0]);
                int y = int.Parse(coords[1]);

                path.Points.Add(new Point { X = x, Y = y });
            }

            paths.Add(path);
        }

        return paths;
    }
}

class OffsetGrid
{
    private readonly char[,] _grid;

    public int Rows => _grid.GetLength(0);

    public int Cols => _grid.GetLength(1);

    private int OffsetX { get; }
    
    private int OffsetY { get; }

    public OffsetGrid(int rows, int cols, Point offset)
    {
        _grid = new char[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                _grid.SetValue('.', i, j);
            }
        }

        OffsetX = offset.X;
        OffsetY = offset.Y;
    }

    public bool IsOutOfBounds(int x, int y)
    {
        x -= OffsetX;
        y -= OffsetY;

        return x < 0 || x >= Cols || y < 0 || y >= Rows;
    }

    public void Set(char value, int x, int y)
    {
        x -= OffsetX;
        y -= OffsetY;

        _grid[y, x] = value;
    }

    public char Get(int x, int y)
    {
        x -= OffsetX;
        y -= OffsetY;

        return _grid[y, x];
    }

    public void Print()
    {
        for (int i = 0; i < Rows; i++)
        {
            Console.Write(i.ToString("00") + " ");

            for (int j = 0; j < Cols; j++)
            {
                Console.Write(_grid.GetValue(i, j) + " ");
            }

            Console.WriteLine();
        }
    }
}

class Path
{
    public List<Point> Points { get; } = new List<Point>();
}

class Point
{
    public int X { get; set; }

    public int Y { get; set; }
}