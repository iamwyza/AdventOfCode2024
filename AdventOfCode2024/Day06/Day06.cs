using AdventOfCode2024.GridUtilities;
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

namespace AdventOfCode2024.Day06;
internal class Day06 : DayBase
{

    public Day06()
    {
        Ready = true;
    }

    private Grid<sbyte> _map;

    [MemberNotNull(nameof(_map))]
    private async Task Init(int part, bool useTestData)
    {

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        _map = new Grid<sbyte>(lines, c =>
            c switch
            {
                '.' => 0,
                '#' => 1,
                '^' => 2
            }
        )
        {
            DefaultPrintConfig = (val, _) => (val switch
            {
                0 => '.',
                1 => '#',
                2 => '^',
                3 => 'X',
                4 => '0'
            }, val switch
            {
                0 => Color.Grey,
                1 => Color.White,
                2 => Color.Red,
                3 => Color.Green,
                4 => Color.Blue
            }, null)
        };


        _map.PrintMap();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        var result = GetOnlyPossiblyPaths();
       
        result.PrintMap();

        var moves = result.Count(x => x.Value == 3);

        AnsiConsole.MarkupLineInterpolated($"Total guard moves is [green]{moves}[/]");


    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, false);

        var max = _map.Count();
        var outputMap = _map.CopyGrid((_, val) => val);
        outputMap.DefaultPrintConfig = _map.DefaultPrintConfig;
        
        foreach (var spotInMap in GetOnlyPossiblyPaths().Where(x => x.Value == 3))
        {
            
            var direction = Direction.North;
            var current = _map.Single(x => x.Value == 2).Coordinate;
            var next = current.Move(direction);
            
            var limiter = 0;
            
            var workingMap = _map.CopyGrid((_, val) => val);
            workingMap.DefaultPrintConfig = _map.DefaultPrintConfig;
            
            workingMap[spotInMap.Item1] = 1;
            
            while (workingMap.InBounds(next) && limiter <= max)
            {
                limiter++;
                if (workingMap[next] == 1)
                {
                    direction = direction switch
                    {
                        Direction.North => Direction.East,
                        Direction.East => Direction.South,
                        Direction.South => Direction.West,
                        Direction.West => Direction.North
                    };
                }
                else
                {
                    workingMap[current] = 3;
                    current = next;
                }
            
                next = current.Move(direction);    
            }
            
            //workingMap.PrintMap();

            if (limiter >= max)
            {
                outputMap[spotInMap.Item1] = 4;
            }
        }
        
       
        
        outputMap.PrintMap();

        var moves = outputMap.Count(x => x.Value == 4);

        AnsiConsole.MarkupLineInterpolated($"Total guard moves is [green]{moves}[/]");
    }

    private Grid<sbyte> GetOnlyPossiblyPaths()
    {
        var direction = Direction.North;
        var current = _map.Single(x => x.Value == 2).Coordinate;
        var next = current.Move(direction);
        
        var result = _map.CopyGrid((_, val) => val);
        
        while (result.InBounds(next))
        {
            if (result[next] == 1)
            {
                direction = direction switch
                {
                    Direction.North => Direction.East,
                    Direction.East => Direction.South,
                    Direction.South => Direction.West,
                    Direction.West => Direction.North
                };
            }
            else
            {
                result[current] = 3;
                current = next;
            }
            
            next = current.Move(direction);    
        }

        result[current] = 3;

        return result;
    }
}