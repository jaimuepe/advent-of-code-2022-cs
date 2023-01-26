namespace AdventOfCode2022.Core;

public class OffsetGrid<T>
{
    private readonly T[,] _grid;

    public int Rows => _grid.GetLength(0);

    public int Cols => _grid.GetLength(1);

    private int OffsetX { get; }

    private int OffsetY { get; }

    public OffsetGrid(int rows, int cols, Point offset, T defaultValue)
    {
        _grid = new T[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                _grid.SetValue(defaultValue, i, j);
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

    public void SetLocal(T value, int x, int y)
    {
        _grid[y, x] = value;
    }

    public void Set(T value, int x, int y)
    {
        x -= OffsetX;
        y -= OffsetY;

        _grid[y, x] = value;
    }

    public T GetLocal(int x, int y)
    {
        return _grid[y, x];
    }

    public T Get(int x, int y)
    {
        x -= OffsetX;
        y -= OffsetY;

        return _grid[y, x];
    }

    public void Print()
    {
        for (int i = 0; i < Rows; i++)
        {
            int rowIndex = i + OffsetY;
            Console.Write(rowIndex.ToString().PadLeft(4) + " ");

            for (int j = 0; j < Cols; j++)
            {
                Console.Write(_grid.GetValue(i, j) + " ");
            }

            Console.WriteLine();
        }
    }
}