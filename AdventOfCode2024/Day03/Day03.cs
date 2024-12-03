namespace AdventOfCode2024.Day03;
internal partial class Day03 : DayBase
{

    public Day03()
    {
        Ready = true;
    }

    private string[] _lines;

    [MemberNotNull(nameof(_lines))]
    private async Task Init(int part, bool useTestData)
    {

        _lines = useTestData ? await GetTestLines(part) : await GetLines();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);
        var regex = MulRegex();

        var total = 0;
        foreach (var line in _lines)
        {
            var matches = regex.Matches(line);

            foreach (Match match in matches)
            {
                var first = Convert.ToInt32(match.Groups[1].Value);
                var second = Convert.ToInt32(match.Groups[2].Value);
                total += first * second;
            }
        }
        
        AnsiConsole.MarkupLineInterpolated($"Total is [green]{total}[/]");

    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);
        
        var regex = MulWithConditionalRegex();

        var total = 0;
        var enabled = true;
        foreach (var line in _lines)
        {
            var matches = regex.Matches(line);

            foreach (Match match in matches)
            {
                switch (match.Groups[0].Value)
                {
                    case "do()":
                        enabled = true;
                        break;
                    case "don't()":
                        enabled = false;
                        break;
                    default:
                    {
                        if (enabled)
                        {
                            var first = Convert.ToInt32(match.Groups[3].Value);
                            var second = Convert.ToInt32(match.Groups[4].Value);
                            total += first * second;    
                        }

                        break;
                    }
                }
            }
        }
        
        AnsiConsole.MarkupLineInterpolated($"Total is [green]{total}[/]");
    }

    [GeneratedRegex(@"((mul\((\d{1,3}),(\d{1,3})\))|(do\(\))|(don't\(\)))")]
    private static partial Regex MulWithConditionalRegex();
    
    [GeneratedRegex(@"mul\((\d{1,3}),(\d{1,3})\)")]
    private static partial Regex MulRegex();
}