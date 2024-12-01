using AdventOfCode2024.GridUtilities;

namespace AdventOfCode2024.Day19;
internal class Day19 : DayBase
{

    public Day19()
    {
        Ready = true;
    }

    private Grid<sbyte> _map;

    [MemberNotNull(nameof(_map))]
    private async Task Init(int part, bool useTestData)
    {
        _map = new Grid<sbyte>();

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        foreach (var line in lines)
        {

        }

        _map.InitMap();

        _map.PrintMap();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, true);

    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, true);
    }
}