namespace AdventOfCode2024.Day05;

// https://adventofcode.com/2024/day/5
internal class Day05 : DayBase
{
    public Day05()
    {
        Ready = true;
    }

    private readonly List<KeyValuePair<int, int>> _rules = new();
    private readonly List<List<int>> _pages = new();
    
    private async Task Init(int part, bool useTestData)
    {

        _rules.Clear();
        _pages.Clear();
        
        var lines = useTestData ? await GetTestLines(part) : await GetLines();
        
        foreach (var line in lines)
        {
            if (line.Contains('|'))
            {
                var parts = line.Split("|").Select(int.Parse).ToArray();
                _rules.Add(new KeyValuePair<int, int>(parts[0], parts[1]));
            }
            else if (line.Contains(','))
            {
                var pages = line.Split(",").Select(int.Parse).ToList();
                _pages.Add(pages);
            }
        }

        _rules.Sort((a, b) => a.Key > b.Key ? -1 : 1);

    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        int middleSum = 0;
        
        foreach (var manual in _pages)
        {
            (bool isValid, _, _) = IsOrderedCorrectly(manual);

            if (isValid)
            {
                middleSum += manual[manual.Count/2];
            }
        }
        
        AnsiConsole.MarkupLineInterpolated($"The total sum of the middle pages is: [green]{middleSum}[/]");

    }
    
    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, false);
        
        int middleSum = 0;
        
        foreach (var manual in _pages)
        {
            var (isValid, _, _) = IsOrderedCorrectly(manual);

            if (isValid) continue;
            
            //AnsiConsole.MarkupLineInterpolated($"Failing line: [yellow]{string.Join(',', manual)}[/]");

            while (!isValid)
            {
                (isValid, int failedAt, KeyValuePair<int, int> failedRule) = IsOrderedCorrectly(manual);

                if (isValid) break;
                
                //AnsiConsole.MarkupLineInterpolated($"Failed on rule: [red]{failedRule.Key}|{failedRule.Value}[/]");
                var toInsertLocation = manual.FindIndex(x => x == failedRule.Value);
                //AnsiConsole.MarkupLineInterpolated($"Moving [yellow]{manual[failedAt]}[/] ([green]{failedAt}[/]) to [red]{toInsertLocation}[/] ");

                manual.RemoveAt(failedAt);
                manual.Insert(toInsertLocation, failedRule.Key);
            }
            
            middleSum += manual[manual.Count/2];    
            
        }
        
        AnsiConsole.MarkupLineInterpolated($"The total sum of the fixed middle pages is: [green]{middleSum}[/]");

    }


    private (bool IsValid, int FailedAt, KeyValuePair<int, int> failedRule) IsOrderedCorrectly(List<int> manual)
    {
        for (var index = 0; index < manual.Count; index++)
        {
            var page = manual[index];
            var rules = _rules.Where(x => x.Key == page).ToArray();
            if (rules.Length == 0) continue;
            
            foreach (var rule in rules)
            {
                if (manual.Take(index).Any(y => y == rule.Value))
                {
                    return (false, index, rule);
                }
            }
        }
        
        return (true,default, default);
    }
}