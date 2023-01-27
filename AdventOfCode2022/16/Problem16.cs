#define _16_VERBOSE

using System.Text.RegularExpressions;
using AdventOfCode2022.Core;

namespace AdventOfCode2022._16;

public partial class Problem16 : Problem
{
    private Dictionary<string, Valve> _valves = null!;

    private Dictionary<string, IList<string>> _cachedShortestPaths = null!;

    protected override void RunA_Internal(List<string> lines)
    {
        const int TOTAL_TIME = 30;

        _valves = ParseValves(lines);

        _cachedShortestPaths = new Dictionary<string, IList<string>>();

        string currentValveId = "AA";

        int pressure = 0;

        for (int i = 0; i < TOTAL_TIME; i++)
        {
            int remainingTime = TOTAL_TIME - i;

#if _16_VERBOSE
            WriteLine($"== Minute {i + 1} ==");
#endif

            var openValves = _valves.Values
                .Where(valve => valve.IsOpened)
                .ToArray();

            if (openValves.Length == 0)
            {
#if _16_VERBOSE
                WriteLine("No valves are open.");
#endif
            }
            else
            {
                int pressureToAdd = openValves.Sum(valve => valve.FlowRate);
#if _16_VERBOSE
                WriteLine(
                    $"Valves {string.Join(", ", openValves.Select(valve => valve.Id))} are open, releasing {pressureToAdd} pressure.");
#endif
                pressure += pressureToAdd;
            }

            string? nextValveId = FindBestMove(currentValveId, null, remainingTime);

            if (nextValveId != null)
            {
                if (nextValveId == currentValveId)
                {
                    _valves[currentValveId].IsOpened = true;
#if _16_VERBOSE
                    WriteLine($"You open valve {nextValveId}");
#endif
                }
                else
                {
                    IList<string> shortestPath = GetShortestPathToValve(currentValveId, nextValveId);
                    currentValveId = shortestPath[0];
#if _16_VERBOSE
                    WriteLine($"You move to valve {currentValveId}");
#endif
                }
            }

#if _16_VERBOSE
            WriteLine();
#endif
        }

        WriteLine($"Result: {pressure}");
    }

    protected override void RunB_Internal(List<string> lines)
    {
    }

    private string? FindBestMove(string currentValveId, string? valveIdToIgnore, int remainingTime)
    {
        string? bestValveId = null;
        int bestScore = 0;

        foreach (Valve valve in _valves.Values)
        {
            if (valve.IsOpened) continue;
            if (valve.Id == valveIdToIgnore) continue;

            int score = GetMoveScore(currentValveId, valve.Id, remainingTime);

            if (score > bestScore)
            {
                bestScore = score;
                bestValveId = valve.Id;
            }
        }

        return bestValveId;
    }

    private int GetMoveScore(string currentValveId, string targetValveId, int remainingTime)
    {
        int score;

        Valve targetValve = _valves[targetValveId];

        if (targetValveId == currentValveId)
        {
            // activate this valve?
            score = targetValve.FlowRate * (remainingTime - 1);
        }
        else
        {
            IList<string> shortestPath = GetShortestPathToValve(currentValveId, targetValveId);

            // how many minutes will it take me to get there?
            int timeToGetThere = shortestPath.Count;

            int activeTime = remainingTime - timeToGetThere - 1;

            if (activeTime > 0)
            {
                if (ProblemType == EProblemType.A)
                {
                    score = activeTime * targetValve.FlowRate / timeToGetThere;
                }
                else
                {
                    score = activeTime * targetValve.FlowRate;
                }
            }
            else
            {
                // no point in getting there
                score = 0;
            }
        }

        return score;
    }

    private IList<string> GetShortestPathToValve(string fromId, string toId)
    {
        string key = fromId + "_" + toId;

        if (_cachedShortestPaths.ContainsKey(key)) return _cachedShortestPaths[key];

        PathNode destNode = BFS(fromId, toId);

        List<string> shortestPath = new();

        PathNode? node = destNode;
        while (node != null && node.Id != fromId)
        {
            shortestPath.Insert(0, node.Id);
            node = node.Parent;
        }

        _cachedShortestPaths[key] = shortestPath;

        return _cachedShortestPaths[key];
    }

    private PathNode BFS(
        string fromId,
        string toId)
    {
        Queue<PathNode> queue = new();
        HashSet<string> seenValues = new();

        seenValues.Add(fromId);
        queue.Enqueue(new PathNode() { Id = fromId });

        while (queue.Count > 0)
        {
            PathNode node = queue.Dequeue();
            if (node.Id == toId)
            {
                return node;
            }

            Valve valve = _valves[node.Id];

            foreach (string exit in valve.Exits)
            {
                if (!seenValues.Contains(exit))
                {
                    PathNode nextNode = new() { Id = exit, Parent = node };
                    queue.Enqueue(nextNode);

                    seenValues.Add(exit);
                }
            }
        }

        return new PathNode();
    }

    private Dictionary<string, Valve> ParseValves(List<string> lines)
    {
        Dictionary<string, Valve> valves = new();

        foreach (string line in lines)
        {
            Match m = LineRegex().Match(line);

            string id = m.Groups["id"].Value;
            int flowRate = int.Parse(m.Groups["flow_rate"].Value);

            string[] exits = m.Groups["exits"].Value.Split(", ");

            Valve valve = new()
            {
                Id = id,
                FlowRate = flowRate,
                Exits = exits,
            };

            valves[id] = valve;
        }

        return valves;
    }

    [GeneratedRegex(
        @"Valve (?<id>[aA-zZ]+) has flow rate=(?<flow_rate>\d+); tunnel(?:s?) lead(?:s?) to valve(?:s?) (?<exits>.+)")]
    private static partial Regex LineRegex();
}

internal class PathNode
{
    public string Id { get; init; } = null!;

    public PathNode? Parent { get; init; }
}

internal class Valve
{
    public string Id { get; init; } = null!;

    public int FlowRate { get; init; }

    public bool IsOpened { get; set; }

    public string[] Exits { get; init; } = null!;
}