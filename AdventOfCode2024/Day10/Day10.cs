using AdventOfCode2024.GridUtilities;

namespace AdventOfCode2024.Day10;
internal class Day10 : DayBase
{

    public Day10()
    {
        Ready = true;
    }

    private Grid<sbyte> _map;

    [MemberNotNull(nameof(_map))]
    private async Task Init(int part, bool useTestData)
    {


        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        _map = new Grid<sbyte>(lines, c => sbyte.Parse(c.ToString()));
        
        _map.PrintMap();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        var total = 0;
        
        foreach (var trailhead in _map.Where(x => x.Value == 0))
        {
            var numPaths = CountPaths(trailhead.Coordinate, distinct: false);
            
            AnsiConsole.MarkupLineInterpolated($"Trailhead at [yellow]{trailhead.Coordinate}[/] has [green]{numPaths}[/]");
            
            total += numPaths;
        }
        
        AnsiConsole.MarkupLineInterpolated($"Total number of paths: {total}");

        
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, false);
        
        var total = 0;
        
        foreach (var trailhead in _map.Where(x => x.Value == 0))
        {
            var numPaths = CountPaths(trailhead.Coordinate, distinct: true);
            
            AnsiConsole.MarkupLineInterpolated($"Trailhead at [yellow]{trailhead.Coordinate}[/] has [green]{numPaths}[/]");
            
            total += numPaths;
        }
        
        AnsiConsole.MarkupLineInterpolated($"Total number of paths: {total}");

       
    }
    
    int CountPaths(Coord start, bool distinct)
    {
        Queue<(Coord step,Coord[] history)> queue = new();
            
        queue.Enqueue((start,[]));

        int result = 0;
            
        HashSet<Coord> visited = new();

        while (queue.TryDequeue(out (Coord step, Coord[] history) item))
        {
            var (step, history) = item;
            if (_map[step] == 9 && (distinct || visited.Add(step)))
            {
                //history = [step, ..history];
                //AnsiConsole.MarkupLineInterpolated($"Path [yellow]{string.Join(" -> ", history)}[/])");
                //_map.PrintMap((val, coord) => ( val.ToString()[0],  history.Contains(coord) ? Color.Green : Color.Grey, null));
                result++;
                continue;
            }
                
            var checkValue = _map[step] + 1;
                
            if (_map.AdjacentTest(x => x == checkValue, step, cardinalOnly: true, out Coord[]? coords))
            {
                foreach(var coord in coords)
                    queue.Enqueue((coord, [step, ..history]));
            }
        }

        return result;
    }
}