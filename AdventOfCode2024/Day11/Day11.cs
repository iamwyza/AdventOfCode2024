using AdventOfCode2024.GridUtilities;

namespace AdventOfCode2024.Day11;
internal class Day11 : DayBase
{

    public Day11()
    {
        Ready = true;
    }

    private readonly List<long> _stones = new();
    private readonly Dictionary<long, Dictionary<long, long>> _generations = new();

    private async Task Init(int part, bool useTestData)
    {
        _stones.Clear();

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        _stones.AddRange(lines[0].Split(' ').Select(long.Parse));
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        CountStones(25);

        var lastGen = _generations.Last();
        var total = lastGen.Value.Values.Sum();

        AnsiConsole.MarkupLineInterpolated($"The total number of stones is: [green]{total}[/]");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, false);

        CountStones(75);
        var lastGen = _generations.Last();

        var total = lastGen.Value.Values.Sum();
        AnsiConsole.MarkupLineInterpolated($"The total number of stones is: [green]{total}[/]");
    }

    private void PrintDebugTable()
    {
        var table = new Table();
        table.AddColumn("Value");
        var lastGen = _generations.Last();
        for (int i = 0; i <= 25; i++)
        {
            table.AddColumn(i.ToString());
        }

        
        foreach(var value in lastGen.Value.Keys.Order())
        {
            string[] row =
            [
                value.ToString(),
                .._generations.OrderBy(x => x.Key).Select(x => _generations[x.Key].ContainsKey(value) && _generations[x.Key][value] > 0 ? _generations[x.Key][value].ToString() : "")
            ];
            table.AddRow(row);
        }

        string[] row2 =
        [
            "Total",
            .._generations.OrderBy(x => x.Key).Select(x => x.Value.Values.Sum().ToString())
        ];
        table.AddRow(row2);

        var total = lastGen.Value.Values.Sum();

        AnsiConsole.Write(table);
    }

    private void CountStones(int blinks)
    {
        _generations.Clear();
        Dictionary<long, long> counts = _stones.GroupBy(x => x).ToDictionary(x => x.Key, x => (long)x.Count());
        _generations.Add(0, counts.ToDictionary(x => x.Key, x => x.Value));

        counts.TryAdd(1, 0);
        
        var countsToAdd = new Dictionary<long, long>();
        
        for (int i = 0; i < blinks; i++)
        {
            countsToAdd.Clear();
            foreach (var stone in counts.Where(x =>x.Value > 0).Select(x => x.Key).ToList()) 
            {
                
                if (stone == 0)
                {
                    countsToAdd.Add(1, counts[0]);
                }
                else
                {
                    var digits = GetNumberOfDigits(stone);
                    if (digits % 2 == 0)
                    {
                        var divisor = digits switch
                        {
                            2 => 10L,
                            4 => 100L,
                            6 => 1000L,
                            8 => 10000L,
                            10 => 100000L,
                            12 => 1000000L,
                            14 => 10000000L,
                            16 => 100000000L,
                            18 => 1000000000L
                        };
                        
                        var right = stone % divisor;
                        var left = (stone - right) / divisor;
                        //AnsiConsole.MarkupLineInterpolated($"Stone: [yellow]{stone}[/].  Left: [white]{left}[/].  Right: [white]{right}[/]");

                        if (!countsToAdd.TryAdd(left, counts[stone]))
                            countsToAdd[left] += counts[stone];
                        
                        
                        if (!countsToAdd.TryAdd(right, counts[stone]))
                            countsToAdd[right] += counts[stone];
                    }
                    else
                    {
                        var val = stone * 2024;
                        if (!countsToAdd.TryAdd(val, counts[stone]))
                            countsToAdd.Add(val, counts[stone]);
                        
                    }

                }
                counts[stone] = 0;

            }

            //AnsiConsole.MarkupLineInterpolated($"[yellow]{i+1}[/] blinks [green]{_stones.Count}[/][red]+{_stones.Count-lastCount}[/] Stones: [blue]{string.Join(' ', _stones)}[/]"); 

            
            foreach (var count in countsToAdd)
            {
                if (!counts.TryAdd(count.Key, count.Value))
                {
                    counts[count.Key] += count.Value;
                }
            }
            _generations.Add(i+1, counts.ToDictionary(x => x.Key, x => x.Value));
        }

        
    }
    
    public static int GetNumberOfDigits(long number) =>
        (number < 0 ? -number : number) switch
        {
            > 999999999999999999 => 19,
            > 99999999999999999 => 18,
            > 9999999999999999 => 17,
            > 999999999999999 => 16,
            > 99999999999999 => 15,
            > 9999999999999 => 14,
            > 999999999999 => 13,
            > 99999999999 => 12,
            > 9999999999 => 11,
            > 999999999 => 10,
            > 99999999 => 9,
            > 9999999 => 8,
            > 999999 => 7,
            > 99999 => 6,
            > 9999 => 5,
            > 999 => 4,
            > 99 => 3,
            > 9 => 2,
            _ => 1
        };
}