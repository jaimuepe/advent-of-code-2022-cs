using AdventOfCode2022.Core;

namespace AdventOfCode2022._13;

public class Problem13 : Problem
{
    protected override void RunA_Internal(List<string> lines)
    {
        var result = 0;

        var pairIndex = 1;
        var i = 0;

        while (i < lines.Count)
        {
            WriteLine();
            WriteLine($"Pair {pairIndex}");
            WriteLine();

            string firstPacketRaw = lines[i];
            string secondPacketRaw = lines[i + 1];

            Packet firstPacket = ParsePacket(firstPacketRaw);
            Packet secondPacket = ParsePacket(secondPacketRaw);

            if (CompareTo(firstPacket.Data, secondPacket.Data) <= 0)
            {
                result += pairIndex;
                WriteLine("ORDERED");
            }
            else
            {
                WriteLine("UNORDERED");
            }

            i += 3;
            pairIndex++;
        }

        WriteLine();
        WriteLine($"Result: {result}");
    }

    protected override void RunB_Internal(List<string> lines)
    {
        const string DIVIDER_PACKET_1 = "[[2]]";
        const string DIVIDER_PACKET_2 = "[[6]]";

        lines.Add(DIVIDER_PACKET_1);
        lines.Add(DIVIDER_PACKET_2);

        var packets = new List<Packet>();

        foreach (string line in lines)
        {
            if (line.Trim().Length == 0) continue;
            packets.Add(ParsePacket(line));
        }

        packets.Sort((p1, p2) => CompareTo(p1.Data, p2.Data));

        // indices start at 1, not 0
        int firstIndex = packets.FindIndex(p => p.RawData == DIVIDER_PACKET_1) + 1;
        int secondIndex = packets.FindIndex(p => p.RawData == DIVIDER_PACKET_2) + 1;

        int decoderKey = firstIndex * secondIndex;

        WriteLine($"Result: {decoderKey}");
    }

    private static Packet ParsePacket(string rawData)
    {
        var packet = new Packet
        {
            RawData = rawData
        };

        int i = 1;
        packet.Data = ParseListData(rawData, ref i);

        return packet;
    }

    private static ListData ParseListData(string rawData, ref int i)
    {
        var listData = new ListData();

        while (i < rawData.Length)
        {
            char c = rawData[i];

            if (c == '[')
            {
                // skip opening brace
                i++;
                listData.Packets.Add(ParseListData(rawData, ref i));
            }
            else if (c == ']')
            {
                // skip closing brace and close list
                i++;
                break;
            }
            else if (c == ',')
            {
                // skip
                i++;
            }
            else
            {
                listData.Packets.Add(ParseIntData(rawData, ref i));
                i++;
            }
        }

        return listData;
    }

    private static IntData ParseIntData(string rawData, ref int i)
    {
        var endIdx = i + 1;
        while (rawData[endIdx] != ',' && rawData[endIdx] != ']') endIdx++;

        var num = int.Parse(rawData.Substring(i, endIdx - i));
        i = endIdx - 1;

        return new IntData() { Value = num };
    }

    private static int CompareTo(IData left, IData right)
    {
        if (left is IntData && right is IntData)
        {
            return CompareTo_Impl((IntData)left, (IntData)right);
        }

        if (left is ListData && right is ListData)
        {
            return CompareTo_Impl((ListData)left, (ListData)right);
        }

        // If exactly one value is an integer, convert the integer to a list which contains that
        // integer as its only value, then retry the comparison

        if (left is IntData)
        {
            // right is list, left is int
            var tempLeft = new ListData();
            tempLeft.Packets.Add(left);

            return CompareTo_Impl(tempLeft, (ListData)right);
        }

        // right is int, left is list
        var tempRight = new ListData();
        tempRight.Packets.Add(right);

        return CompareTo_Impl((ListData)left, tempRight);
    }

    private static int CompareTo_Impl(IntData left, IntData right)
    {
        if (left.Value < right.Value)
        {
            // If the left integer is lower than the right integer, the inputs are in the right order
            return -1;
        }

        if (left.Value > right.Value)
        {
            // If the left integer is higher than the right integer, the inputs are not in the right order
            return 1;
        }

        // Otherwise, the inputs are the same integer
        return 0;
    }

    private static int CompareTo_Impl(ListData left, ListData right)
    {
        var n = int.Min(left.Packets.Count, right.Packets.Count);

        // If both values are lists, compare the first value of each list, then the second value, and so on
        for (int i = 0; i < n; i++)
        {
            int comparision = CompareTo(left.Packets[i], right.Packets[i]);
            if (comparision != 0)
            {
                return comparision;
            }
        }

        if (left.Packets.Count == right.Packets.Count)
        {
            // If the lists are the same length and no comparison makes a decision about the order,
            // continue checking the next part of the input
            return 0;
        }

        if (left.Packets.Count < right.Packets.Count)
        {
            // If the left list runs out of items first, the inputs are in the right order
            return -1;
        }

        // If the right list runs out of items first, the inputs are not in the right order
        return 1;
    }
}

internal class Packet
{
    public string RawData { get; init; } = null!;

    public IData Data { get; set; } = null!;

    public void Print()
    {
        Data.Print();
        Console.WriteLine();
    }
}

internal interface IData
{
    void Print();
}

internal class ListData : IData
{
    public List<IData> Packets { get; }

    public ListData()
    {
        Packets = new List<IData>();
    }

    public void Print()
    {
        Console.Write('[');
        for (var i = 0; i < Packets.Count; i++)
        {
            Packets[i].Print();
            if (i < Packets.Count - 1)
            {
                Console.Write(',');
            }
        }

        Console.Write(']');
    }
}

internal class IntData : IData
{
    public int Value { get; init; }

    public void Print()
    {
        Console.Write(Value.ToString());
    }
}