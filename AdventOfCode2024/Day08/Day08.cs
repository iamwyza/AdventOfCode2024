using AdventOfCode2024.GridUtilities;

namespace AdventOfCode2024.Day08;

// https://adventofcode.com/2024/day/8
internal class Day08 : DayBase
{
    public Day08()
    {
        Ready = true;
    }

    private Grid<char> _map;
    private Grid<char> _outputMap;

    [MemberNotNull(nameof(_map))]
    [MemberNotNull(nameof(_outputMap))]
    private async Task Init(int part, bool useTestData)
    {
        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        _map = new Grid<char>(lines, c => c)
        {
            DefaultPrintConfig = (c, _) => (c, c != '.' ? Color.Green : Color.Grey, null)
        };

        _outputMap = _map.CopyGrid((_, c) => c);
        _outputMap.DefaultPrintConfig = (c, _) => (c, c switch
        {
            '.' => Color.Grey,
            '#' => Color.Red,
            _ => Color.Green
        }, null);

        _map.PrintMap();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        List<(Coord, Coord)> frequencyPairs = GetFrequencyPairs();

        foreach (var (start, end) in frequencyPairs)
        {
            _map.DebugPrintMap((c, coord) => (c, coord == end || coord == start ? Color.Green : Color.Grey, null),
                printRowLabel: true);

            var antiNode = FindAntiNodes(start, end, 1);

            if (antiNode == Array.Empty<Coord>()) continue;

            foreach (var coord in antiNode)
            {
                if (_map.InBounds(coord))
                {
                    _outputMap[coord] = '#';
                }
            }

            _outputMap.DebugPrintMap();
        }

        _outputMap.PrintMap();
        var uniqueAntiNodes = _outputMap.Count(x => x.Value == '#');

        AnsiConsole.MarkupLineInterpolated($"The number of unique antiNodes is [green]{uniqueAntiNodes}[/]");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, false);

        List<(Coord, Coord)> frequencyPairs = GetFrequencyPairs();

        foreach (var (start, end) in frequencyPairs)
        {
            _map.DebugPrintMap((c, coord) => (c, coord == end || coord == start ? Color.Green : Color.Grey, null),
                printRowLabel: true);

            // We automatically add the pair themselves as outputs as they are resonant anti-nodes to eachother.
            _outputMap[start] = '#';
            _outputMap[end] = '#';

            var antiNode = FindAntiNodes(start, end, int.MaxValue);

            if (antiNode == Array.Empty<Coord>()) continue;

            foreach (var coord in antiNode)
            {
                _outputMap[coord] = '#';
            }

            _outputMap.DebugPrintMap();
        }

        _outputMap.PrintMap();
        var uniqueAntiNodes = _outputMap.Count(x => x.Value == '#');

        AnsiConsole.MarkupLineInterpolated($"The number of unique antiNodes is [green]{uniqueAntiNodes}[/]");
    }

    // Works for both part 1 and part 2 just by changing how far out the anti-nodes could be.
    private Coord[] FindAntiNodes(Coord first, Coord second, int maxMultiplier)
    {
        DebugAnsiConsole.MarkupLineInterpolated($"First is [yellow]{first}[/]. Second is [yellow]{second}[/]");
        var xDiff = Math.Abs(second.X - first.X);
        var yDiff = Math.Abs(second.Y - first.Y);

        DebugAnsiConsole.MarkupLineInterpolated($"X difference is [red]{xDiff}[/]. Y difference is [red]{yDiff}[/]");

        List<Coord> results = new();

        var firstXOffset = first.X > second.X ? xDiff : -1 * xDiff;
        var secondXOffset = first.X > second.X ? -1 * xDiff : xDiff;

        var firstYOffset = first.Y > second.Y ? yDiff : -1 * yDiff;
        var secondYOffset = first.Y > second.Y ? -1 * yDiff : yDiff;

        var multiplier = 1;

        while (multiplier <= maxMultiplier)
        {
            Coord check1 = new Coord(first.X + multiplier * firstXOffset, first.Y + multiplier * firstYOffset);
            Coord check2 = new Coord(second.X + multiplier * secondXOffset, second.Y + multiplier * secondYOffset);

            var anyInBounds = false;

            if (_map.InBounds(check1))
            {
                results.Add(check1);
                anyInBounds = true;
            }

            if (_map.InBounds(check2))
            {
                results.Add(check2);
                anyInBounds = true;
            }

            if (anyInBounds)
            {
                multiplier++;
            }
            else
            {
                break;
            }
        }

        if (DebugOutput)
        {
            DebugAnsiConsole.Write("Resulting coords:");

            for (var index = 0; index < results.Count; index++)
            {
                var coord = results[index];
                DebugAnsiConsole.MarkupInterpolated($"[yellow]{coord}[/]");
                if (index > 0)
                    DebugAnsiConsole.Write(", ");
            }
        }

        return results.ToArray();
    }

    private List<(Coord, Coord)> GetFrequencyPairs()
    {
        Dictionary<char, Coord[]> frequencies =
            _map.Where(x => x.Value != '.').GroupBy(x => x.Value)
                .ToDictionary(group => group.Key,
                    group => group.Select(x => x.Coordinate).ToArray());


        List<(Coord, Coord)> frequencyPairs = [];

        foreach (var (_, coords) in frequencies.Where(frequency => frequency.Value.Length > 1))
        {
            // Get all the possible pairs of frequencies
            for (var index = 0; index < coords.Length; index++)
            {
                var coord1 = coords[index];

                for (var otherIndex = 0; otherIndex < coords.Length; otherIndex++)
                {
                    if (otherIndex == index) continue;
                    
                    var coord2 = coords[otherIndex];

                    if (frequencyPairs.Contains((coord1, coord2)) || frequencyPairs.Contains((coord2, coord1)))
                        continue;

                    frequencyPairs.Add((coord1, coord2));
                }
            }
        }

        return frequencyPairs;
    }
}