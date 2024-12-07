namespace AdventOfCode2024.Day07;

internal class Day07 : DayBase
{
    public Day07()
    {
        Ready = true;
    }

    private List<(long result, long[] values)> _input = new();

    private async Task Init(int part, bool useTestData)
    {
        _input.Clear();

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        foreach (var line in lines)
        {
            var temp = line.Split(':');
            var result = Convert.ToInt64(temp[0]);
            var values = temp[1].Trim().Split(' ').Select(x => long.Parse(x.Trim())).ToArray();
            _input.Add((result, values));
        }
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        var totalValid = 0L;

        foreach (var (result, values) in _input)
        {
            if (isValid(result, 0, true, values) || isValid(result, 0, false, values))
            {
                totalValid += result;
            }
        }

        AnsiConsole.MarkupLineInterpolated($"Total calibration result: [green]{totalValid}[/]");


        bool isValid(long check, long current, bool multiply, ReadOnlySpan<long> remaining)
        {
            var next = remaining[0];

            if (multiply)
            {
                next *= current;
            }
            else
            {
                next += current;
            }

            // Exit early, we already exceeded the max on this branch.
            if (next > check) return false;

            if (remaining.Length > 1)
            {
                if (isValid(check, next, true, remaining[1..]))
                {
                    return true;
                }

                return isValid(check, next, false, remaining[1..]);
            }

            return check == next;
        }
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, false);

        var totalValid = 0L;

        foreach (var (result, valuesArray) in _input)
        {
            var values = new ReadOnlySpan<long>(valuesArray);
            if (IsValid(result, 1, true, false, ref values, 0) || IsValid(result, 0, false, false, ref values, 0) ||
                IsValid(result, 1, true, true, ref values, 0) || IsValid(result, 0, false, true, ref values, 0))
            {
                //AnsiConsole.MarkupLineInterpolated($"[yellow]{result}[/]:[green]{string.Join(' ', values)}[/]");
                totalValid += result;
            }
            else
            {
                //AnsiConsole.MarkupLineInterpolated($"[yellow]{result}[/]:[red]{string.Join(' ', values)}[/]");
            }
        }

        AnsiConsole.MarkupLineInterpolated($"Total calibration result: [green]{totalValid}[/]");
    }

    private static bool IsValid(long check, long current, bool multiply, bool testConcat, ref ReadOnlySpan<long> values, int position,
        int depth = 0)
    {
        var next = values[position];
        
        //AnsiConsole.Write("".PadLeft(depth * 2));
        //AnsiConsole.MarkupInterpolated($"Testing [white]{check}[/]: op: [green]{(testConcat ? "||" : (multiply ? '*' : '+'))}[/] current: [yellow]{current}[/], next: [red]{next}[/], remaining: [blue]{string.Join(' ', remaining)}[/]");

        if (current == 0 && multiply) return false;

        if (testConcat)
        {
            next = Concat(current, next);
        }else if (multiply)
        {
            next *= current;
        }
        else
        {
            next += current;
        }

        //AnsiConsole.MarkupLineInterpolated($" new: [cyan]{next}[/]");

        // Exit early, we already exceeded the max on this branch.
        if (next > check) return false;

        if (values.Length == position+1)
            return check == next;


        return IsValid(check, next, true, false, ref values, position+1, depth + 1)
               || IsValid(check, next, false, false, ref values, position+1, depth + 1)
               || IsValid(check, next, true, true, ref values, position+1, depth + 1)
                    || IsValid(check, next, false, true,ref values, position+1, depth + 1);
    }
    
    // Didn't like using strings to combine the numbers, so found this handy approach:
    // https://stackoverflow.com/questions/1014292/concatenate-integers-in-c-sharp
    // modified it to be long instead of ulong, which would do weird things if it was ever negative, but w/e.
    static long Concat(long a, long b)
    {
        if (b < 10U) return 10L * a + b;
        if (b < 100U) return 100L * a + b;
        if (b < 1000U) return 1000L * a + b;
        if (b < 10000U) return 10000L * a + b;
        if (b < 100000U) return 100000L * a + b;
        if (b < 1000000U) return 1000000L * a + b;
        if (b < 10000000U) return 10000000L * a + b;
        if (b < 100000000U) return 100000000L * a + b;
        return 1000000000L * a + b;
    }
}