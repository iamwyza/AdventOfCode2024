namespace AdventOfCode2024.Day01;

// https://adventofcode.com/2024/day/1
internal class Day01 : DayBase
{

    public Day01()
    {
        Ready = true;
    }

    private int[] _listOne;
    private int[] _listTwo;

    private async Task Init(int part, bool useTestData)
    {

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        _listOne = new int[lines.Length];
        _listTwo = new int[lines.Length];

        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            var temp = line.Split("   ");
            _listOne[index] = Convert.ToInt32(temp[0]);
            _listTwo[index] = Convert.ToInt32(temp[1]);
        }
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);
        
        _listOne = _listOne.Order().ToArray();
        _listTwo = _listTwo.Order().ToArray();
        var distances = new int[_listOne.Length];
        
        for (var index = 0; index < _listOne.Length; index++)
        {
            distances[index] = Math.Abs(_listOne[index] - _listTwo[index]);
        }
        
        AnsiConsole.MarkupLineInterpolated($"The sum is [green]{distances.Sum()}[/]");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, false);

        var similarityTotal = 0;

        var lookup = _listTwo.Distinct().ToDictionary(x => x, y => _listTwo.Count(z => y == z));
        
        foreach (var t in _listOne)
        {
            if (lookup.TryGetValue(t, out var value))
            {
                similarityTotal += value * t;
            }
        }
        
        AnsiConsole.MarkupLineInterpolated($"Similarity total: [green]{similarityTotal}[/]");
    }
}