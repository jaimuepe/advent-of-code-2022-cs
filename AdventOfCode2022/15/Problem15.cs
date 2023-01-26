using System.Text.RegularExpressions;
using AdventOfCode2022.Core;

namespace AdventOfCode2022._15;

public partial class Problem15 : Problem
{
    protected override void RunA_Internal(List<string> lines)
    {
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        List<Sensor> sensors = new();
        Dictionary<Point, Beacon> beacons = new();

        int sensorId = 0;
        int beaconId = 0;

        foreach (var line in lines)
        {
            var match = SensorRegex().Match(line);

            int sensorX = int.Parse(match.Groups[1].Value);
            int sensorY = int.Parse(match.Groups[2].Value);
            int beaconX = int.Parse(match.Groups[3].Value);
            int beaconY = int.Parse(match.Groups[4].Value);

            Point sensorPos = new() { X = sensorX, Y = sensorY };
            Point beaconPos = new() { X = beaconX, Y = beaconY };

            int distance = CalcManhattanDistance(sensorPos, beaconPos);

            if (!beacons.TryGetValue(beaconPos, out var beacon))
            {
                beacon = new Beacon()
                {
                    Id = beaconId++,
                    Position = beaconPos,
                };
                beacons[beaconPos] = beacon;
            }

            sensors.Add(new Sensor()
            {
                Id = sensorId++,
                Position = sensorPos,
                ClosestBeacon = beacon,
                ClosestBeaconDistance = distance,
            });

            minX = Math.Min(minX, Math.Min(sensorX - distance, beaconX));
            maxX = Math.Max(maxX, Math.Max(sensorX + distance, beaconX));
            minY = Math.Min(minY, Math.Min(sensorY - distance, beaconY));
            maxY = Math.Max(maxY, Math.Max(sensorY + distance, beaconY));
        }

        /*int cols = maxX - minX + 1;
        int rows = maxY - minY + 1;

        var grid = new OffsetGrid<char>(
            rows,
            cols,
            new Point() { X = minX, Y = minY },
            '.');

        foreach (var sensor in sensors)
        {
            grid.Set('S', sensor.Position.X, sensor.Position.Y);
        }

        foreach (var beacon in beacons.Values)
        {
            grid.Set('B', beacon.Position.X, beacon.Position.Y);
        }

        for (var y = minY; y < maxY; y++)
        {
            for (var x = minX; x < maxX; x++)
            {
                if (grid.Get(x, y) != '.') continue;

                foreach (var sensor in sensors)
                {
                    var distance = CalcManhattanDistance(sensor.Position, new Point() { X = x, Y = y });
                    if (distance <= sensor.ClosestBeaconDistance)
                    {
                        grid.Set('#', x, y);
                        break;
                    }
                }
            }
        }*/

        int result = 0;

        // sort sensors in x axis
        sensors.Sort((s1, s2) => s1.Position.X.CompareTo(s2.Position.X));

        var y = IsTest ? 10 : 2000000;

        for (int x = minX; x < maxX; x++)
        {
            var p = new Point() { X = x, Y = y };

            if (beacons.ContainsKey(p)) continue;

            foreach (var sensor in sensors)
            {
                if (sensor.Position.Equals(p)) break;

                var distance = CalcManhattanDistance(sensor.Position, p);
                if (distance <= sensor.ClosestBeaconDistance)
                {
                    result++;
                    break;
                }
            }
        }

        WriteLine($"Result: {result}");
    }

    protected override void RunB_Internal(List<string> lines)
    {
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        List<Sensor> sensors = new();
        Dictionary<Point, Beacon> beacons = new();

        int sensorId = 0;
        int beaconId = 0;

        foreach (var line in lines)
        {
            var match = SensorRegex().Match(line);

            int sensorX = int.Parse(match.Groups[1].Value);
            int sensorY = int.Parse(match.Groups[2].Value);
            int beaconX = int.Parse(match.Groups[3].Value);
            int beaconY = int.Parse(match.Groups[4].Value);

            Point sensorPos = new() { X = sensorX, Y = sensorY };
            Point beaconPos = new() { X = beaconX, Y = beaconY };

            int distance = CalcManhattanDistance(sensorPos, beaconPos);

            if (!beacons.TryGetValue(beaconPos, out var beacon))
            {
                beacon = new Beacon()
                {
                    Id = beaconId++,
                    Position = beaconPos,
                };
                beacons[beaconPos] = beacon;
            }

            sensors.Add(new Sensor()
            {
                Id = sensorId++,
                Position = sensorPos,
                ClosestBeacon = beacon,
                ClosestBeaconDistance = distance,
            });

            minX = Math.Min(minX, Math.Min(sensorX - distance, beaconX));
            maxX = Math.Max(maxX, Math.Max(sensorX + distance, beaconX));
            minY = Math.Min(minY, Math.Min(sensorY - distance, beaconY));
            maxY = Math.Max(maxY, Math.Max(sensorY + distance, beaconY));
        }

        int y = IsTest ? 20 : 4000000;
        int x = IsTest ? 20 : 4000000;

        for (int i = 0; i < y; i++)
        {
            WriteLine(i.ToString());
            
            for (int j = 0; j < x; j++)
            {
                Point p = new Point() { X = j, Y = i };

                bool outOfReach = true;

                foreach (var sensor in sensors)
                {
                    if (sensor.Position.Equals(p))
                    {
                        outOfReach = false;
                        break;
                    }

                    var distance = CalcManhattanDistance(sensor.Position, p);
                    if (distance <= sensor.ClosestBeaconDistance)
                    {
                        outOfReach = false;
                        break;
                    }
                }

                if (outOfReach)
                {
                    WriteLine($"Result: {j * 4000000 + i}");
                    return;
                }
            }
        }
    }

    private int CalcManhattanDistance(Point p1, Point p2)
    {
        return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
    }

    [GeneratedRegex(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)")]
    private static partial Regex SensorRegex();
}

class Sensor
{
    public int Id { get; init; }

    public Point Position { get; init; }

    public Beacon ClosestBeacon { get; init; }

    public int ClosestBeaconDistance { get; init; }
}

class Beacon
{
    public int Id { get; init; }

    public Point Position { get; init; }
}