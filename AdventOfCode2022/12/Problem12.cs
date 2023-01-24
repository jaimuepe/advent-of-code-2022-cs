#define STORE_CHAIN

using AdventOfCode2022.Core;

namespace AdventOfCode2022._12;

public class Problem12 : Problem
{
    private Heightmap Heightmap { get; set; } = null!;

    protected override void RunA_Internal(List<string> lines)
    {
        Heightmap = new Heightmap(lines);

        var startNode = Heightmap.Grid[Heightmap.StartY][Heightmap.StartX];
        Heightmap.Solve(startNode);

        PrintSolution();
    }

    protected override void RunB_Internal(List<string> lines)
    {
        Heightmap = new Heightmap(lines);

        var startNodes = new List<NodeInfo>();

        for (int y = 0; y < Heightmap.Rows; y++)
        {
            for (int x = 0; x < Heightmap.Cols; x++)
            {
                char height = Heightmap.Grid[y][x].Height;
                if (height == 'a')
                {
                    startNodes.Add(Heightmap.Grid[y][x]);
                }
            }
        }

        startNodes.ForEach(startNode => Heightmap.Solve(startNode));

        PrintSolution();
    }

    private void PrintSolution()
    {
        if (Heightmap.ShortestPath == -1)
        {
            Console.Error.WriteLine("Could not find any solution");
            return;
        }

#if STORE_CHAIN
        Heightmap.PrintSequence(Heightmap.BestSolution!);
#endif

        int steps = Heightmap.ShortestPath;
        WriteLine($"Solution: {steps}");
    }
}

internal class Heightmap
{
    public int ShortestPath { get; private set; } = -1;

#if STORE_CHAIN
    public List<NodeInfo>? BestSolution { get; private set; }
#endif

    public NodeInfo[][] Grid { get; }

    public int Rows => Grid.Length;

    public int Cols => Grid[0].Length;

    public int StartX { get; private set; }
    public int StartY { get; private set; }

    private int EndX { get; set; }
    private int EndY { get; set; }

    public Heightmap(List<string> lines)
    {
        int rows = lines.Count;

        Grid = new NodeInfo[rows][];
        PopulateGrid(lines);
    }

    public void Solve(NodeInfo startNode)
    {
        startNode.Cost = 0;
#if STORE_CHAIN
        List<NodeInfo> visitedNodes = new List<NodeInfo>() { startNode };
        VisitNeighbors(startNode, visitedNodes);
#else
        VisitNeighbors(startNode);
#endif
    }

#if STORE_CHAIN
    private void VisitNeighbors(NodeInfo node, List<NodeInfo> chain)
#else
    private void VisitNeighbors(NodeInfo node)
#endif
    {
        int cost = node.Cost + 1;

        List<NodeInfo> neighbors = GetNeighbors(node);

        foreach (NodeInfo neighbor in neighbors)
        {
            if (!CanVisitNeighbor(node, neighbor)) continue;

            if (neighbor.Y == EndY && neighbor.X == EndX)
            {
                // reached end
                if (ShortestPath == -1 || ShortestPath > cost)
                {
                    ShortestPath = cost;
#if STORE_CHAIN
                    BestSolution = chain;
#endif
                }

                break;
            }

            if (neighbor.Cost == -1 || neighbor.Cost > cost)
            {
                // not visited or shorter path
                neighbor.Cost = cost;
#if STORE_CHAIN
                var newChain = new List<NodeInfo>(chain) { neighbor };
                VisitNeighbors(neighbor, newChain);
#else
                VisitNeighbors(neighbor);
#endif
            }
        }
    }

    private static bool CanVisitNeighbor(NodeInfo node, NodeInfo neighbor)
    {
        return neighbor.Height <= node.Height + 1;
    }

    private List<NodeInfo> GetNeighbors(NodeInfo node)
    {
        var neighbors = new List<NodeInfo>();

        int col = node.X;
        int row = node.Y;

        if (row > 0) neighbors.Add(Grid[row - 1][col]);
        if (row < Rows - 1) neighbors.Add(Grid[row + 1][col]);

        if (col > 0) neighbors.Add(Grid[row][col - 1]);
        if (col < Cols - 1) neighbors.Add(Grid[row][col + 1]);

        return neighbors;
    }

    public void PrintSequence(IReadOnlyList<NodeInfo> result)
    {
        Console.WriteLine();
        Console.WriteLine($"Number of steps: {result.Count}");
        Console.WriteLine();

        char[][] printMatrix = new char[Rows][];

        for (int y = 0; y < Rows; y++)
        {
            printMatrix[y] = new char[Cols];
            for (int x = 0; x < Cols; x++)
            {
                if (y == EndY && x == EndX)
                {
                    printMatrix[y][x] = 'E';
                }
                else
                {
                    printMatrix[y][x] = '.';
                }
            }
        }

        for (int i = 0; i < result.Count; i++)
        {
            NodeInfo curr = result[i];

            NodeInfo next;
            if (i == result.Count - 1)
            {
                next = Grid[EndY][EndX];
            }
            else
            {
                next = result[i + 1];
            }

            char direction;
            if (next.X > curr.X)
            {
                direction = '>';
            }
            else if (next.X < curr.X)
            {
                direction = '<';
            }
            else if (next.Y > curr.Y)
            {
                direction = 'v';
            }
            else
            {
                direction = '^';
            }

            printMatrix[curr.Y][curr.X] = direction;
        }

        for (int y = 0; y < Rows; y++)
        {
            for (int x = 0; x < Cols; x++)
            {
                Console.Write(printMatrix[y][x]);
            }

            Console.WriteLine();
        }
    }

    private void PopulateGrid(List<string> lines)
    {
        int rows = lines.Count;
        int cols = lines[0].Length;

        for (int y = 0; y < rows; y++)
        {
            Grid[y] = new NodeInfo[cols];

            for (int x = 0; x < cols; x++)
            {
                char h = lines[y][x];

                NodeInfo node = new NodeInfo()
                {
                    X = x,
                    Y = y,
                    Height = h,
                };

                if (h == 'S')
                {
                    node.Height = 'a';
                    StartX = x;
                    StartY = y;
                }
                else if (h == 'E')
                {
                    node.Height = 'z';
                    EndX = x;
                    EndY = y;
                }

                Grid[y][x] = node;
            }
        }
    }
}

internal class NodeInfo
{
    public int X { get; init; }

    public int Y { get; init; }

    public char Height { get; set; }

    public int Cost { get; set; } = -1;
}