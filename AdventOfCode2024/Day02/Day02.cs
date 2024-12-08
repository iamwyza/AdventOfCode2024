namespace AdventOfCode2024.Day02;

// https://adventofcode.com/2024/day/2
internal class Day02 : DayBase
{
    public Day02()
    {
        Ready = true;
    }

    private List<int[]> _data;

    [MemberNotNull(nameof(_data))]
    private async Task Init(int part, bool useTestData)
    {
        _data = new();

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        foreach (var line in lines)
        {
            _data.Add(line.Split(' ').Select(x => Convert.ToInt32(x)).ToArray());
        }
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        int safeReports = 0;
        foreach (var report in _data)
        {
            bool? decreasing = null;
            bool safe = true;
            
            for (int i = 1; i < report.Length; i++)
            {
                decreasing ??= report[i] < report[i - 1];
                
               safe = IsSafe(decreasing.Value, report[i], report[i - 1]);
               if (!safe) { break; }
            }

            if (safe) { safeReports++; }
        }

        AnsiConsole.MarkupLineInterpolated($"There were [green]{safeReports}[/] safe reports.");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, false);
        
        int safeReports = 0;
        for (var index = 0; index < _data.Count; index++)
        {
            var report = _data[index];
            bool? decreasing = null;
            bool safe = true;
            bool hasSkipped = false;
            int skippedOn = 0;

            // this is not pretty, it sucks, but i'm tired. and it works. so. yeah.
            
            for (int i = 1; i < report.Length; i++)
            {
                decreasing ??= report[i] < report[i - 1];

                safe = IsSafe(decreasing.Value, report[i], report[i - 1]);

                if (!safe && i > 1)
                {
                    if (hasSkipped) break;
                    skippedOn = i;

                    hasSkipped = true;

                    if (i == report.Length - 1)
                    {
                        safe = true;
                        break;
                    }
                    
                    if (i == 2)
                        decreasing = report[i] < report[i - 2];

                    safe = IsSafe(decreasing.Value, report[i], report[i - 2]);

                    if (!safe) break;
                }else if (!safe)
                {
                    if (hasSkipped) break;
                    
                    hasSkipped = true;
                    skippedOn = i;
                    safe = true;
                }
            }

            if (!safe)
            {
                var brute = BruteForce(report);
                if (brute.Item1)
                {
                    safe = true;
                    AnsiConsole.MarkupLineInterpolated($"Found a line that was brute forced as safe, but wasn't marked as safe otherwise.  Removing [yellow]{brute.Item2}[/] fixes it."  );
                }
            }

            AnsiConsole.Write(string.Join(' ', report).PadRight(30) + " ");
            if (safe)
            {
                safeReports++;
                AnsiConsole.MarkupInterpolated($"Line [yellow]{index+1}[/] is [green]safe[/]. ");
                if (hasSkipped)
                {
                    AnsiConsole.MarkupInterpolated($"Skipped: [yellow]{skippedOn+1}[/] ");
                }
            }
            else
            {
                AnsiConsole.MarkupInterpolated($"Line [yellow]{index+1}[/] is [red]unsafe[/]. Skipped: [yellow]{skippedOn+1}[/] ");
            }
            
            AnsiConsole.MarkupLineInterpolated($"Decreasing: [yellow]{decreasing}[/]");

        }

        AnsiConsole.MarkupLineInterpolated($"There were [green]{safeReports}[/] safe reports.");
    }

    private (bool, int) BruteForce(int[] report)
    {
      
        for (int z = 0; z < report.Length; z++)
        {
            bool? decreasing = null;
            bool safe = true;
            var subreport = report.ToList();
            subreport.RemoveAt(z);
            
            for (int i = 1; i < subreport.Count; i++)
            {
                decreasing ??= subreport[i] < subreport[i - 1];
                
                safe = IsSafe(decreasing.Value, subreport[i], subreport[i - 1]);
                if (!safe) { break; }
            }

            if (safe)
            {
                return (true, z);
            } 
        }

        return (false, -1);
    }

    private bool IsSafe(bool decreasing, int start, int end)
    {
        if (start < end && !decreasing)
        {
            return false;
        }
                
        if (start > end && decreasing)
        {
            return false;
        }

        return Math.Abs(start - end) is >= 1 and <= 3;
    }
}