using System.Text.RegularExpressions;
using AdventOfCode2022.Core;

namespace AdventOfCode2022._15;

public partial class Problem15 : Problem
{
    protected override void RunA_Internal(List<string> lines)
    {
        ParsedInput input = Parse(lines);

        int minX = input.MinX;
        int maxX = input.MaxX;

        List<Sensor> sensors = input.Sensors;

        // sort sensors in x axis
        sensors.Sort((s1, s2) => s1.Position.X.CompareTo(s2.Position.X));

        var refY = IsTest ? 10 : 2000000;

        int result = 0;

        for (int x = minX; x < maxX; x++)
        {
            var p = new Point() { X = x, Y = refY };

            if (input.Beacons.ContainsKey(p)) continue;

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
        ParsedInput input = Parse(lines);

        List<Sensor> sensors = input.Sensors;

        for (int i = 0; i < sensors.Count; i++)
        {
            Sensor sensor = sensors[i];

            List<Point> perimeter = GetPerimeter(sensor);

            foreach (Point p in perimeter)
            {
                bool outOfReach = true;

                for (int j = 0; j < sensors.Count; j++)
                {
                    if (i == j) continue;

                    Sensor otherSensor = sensors[j];

                    int distance = CalcManhattanDistance(p, otherSensor.Position);
                    if (distance <= otherSensor.ClosestBeaconDistance)
                    {
                        outOfReach = false;
                        break;
                    }
                }

                if (outOfReach)
                {
                    var tuningFrequency = (long)p.X * 4000000 + p.Y;
                    WriteLine($"Result: {tuningFrequency}");
                    return;
                }
            }
        }
    }

    private List<Point> GetPerimeter(Sensor sensor)
    {
        const int minX = 0;
        int maxX = IsTest ? 20 : 4000000;
        const int minY = 0;
        int maxY = IsTest ? 20 : 4000000;

        int distance = sensor.ClosestBeaconDistance + 1;

        var perimeter = new List<Point>();

        for (int i = -distance; i <= distance; i++)
        {
            int x = sensor.Position.X + i;
            if (x < minX || x > maxX) continue;

            int j = distance - Math.Abs(i);

            int y1 = sensor.Position.Y + j;
            int y2 = sensor.Position.Y - j;

            if (y1 >= minY && y1 <= maxY)
            {
                perimeter.Add(new Point()
                {
                    X = x,
                    Y = y1,
                });
            }

            if (y2 >= minY && y2 <= maxX)
            {
                perimeter.Add(new Point()
                {
                    X = x,
                    Y = y2,
                });
            }
        }

        return perimeter;
    }

    private ParsedInput Parse(List<string> lines)
    {
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        List<Sensor> sensors = new();
        Dictionary<Point, Beacon> beacons = new();

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

            sensors.Add(new Sensor
            {
                Position = sensorPos,
                ClosestBeaconDistance = distance,
            });

            if (!beacons.ContainsKey(beaconPos))
            {
                beacons.Add(beaconPos, new Beacon { Position = beaconPos, });
            }

            minX = Math.Min(minX, Math.Min(sensorX - distance, beaconX));
            maxX = Math.Max(maxX, Math.Max(sensorX + distance, beaconX));
            minY = Math.Min(minY, Math.Min(sensorY - distance, beaconY));
            maxY = Math.Max(maxY, Math.Max(sensorY + distance, beaconY));
        }

        return new ParsedInput()
        {
            Sensors = sensors,
            Beacons = beacons,
            MinX = minX,
            MaxX = maxX,
            MinY = minY,
            MaxY = maxY,
        };
    }

    private int CalcManhattanDistance(Point p1, Point p2)
    {
        return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
    }

    [GeneratedRegex(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)")]
    private static partial Regex SensorRegex();
}

class ParsedInput
{
    public List<Sensor> Sensors { get; init; }

    public Dictionary<Point, Beacon> Beacons { get; init; }

    public int MinX { get; init; }

    public int MaxX { get; init; }

    public int MinY { get; init; }

    public int MaxY { get; init; }
}

class SensorPerimeterData
{
    public Sensor Sensor { get; init; }

    public List<Point> Points { get; } = new List<Point>();
}

class Sensor
{
    public Point Position { get; init; }

    public int ClosestBeaconDistance { get; init; }
}

class Beacon
{
    public Point Position { get; init; }
}