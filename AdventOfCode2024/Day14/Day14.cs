using System.Diagnostics;
using AdventOfCode2024.GridUtilities;

namespace AdventOfCode2024.Day14;
internal partial class Day14 : DayBase
{

    private class Robot(Coord coord, int vx, int vy)
    {
        public Coord Coord { get; set; } = coord;
        public int vX { get; set; } = vx;
        public int vY { get; set; } = vy;
    }
    
    public Day14()
    {
        Ready = true;
    }
    private Grid<sbyte>? _map;

    List<Robot> robots = new List<Robot>();

    private async Task Init(int part, bool useTestData)
    {
        robots.Clear();

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        var parseRegex = ParserRegex();
        
        foreach (var line in lines)
        {
            var result = parseRegex.Match(line);
            robots.Add(new Robot(new Coord(int.Parse(result.Groups[1].Value), int.Parse(result.Groups[2].Value)), 
                int.Parse(result.Groups[3].Value), int.Parse(result.Groups[4].Value)));
        }
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        var useTestData = false;
        await Init(1, useTestData);
        int maxX = 11;
        int maxY = 7;
        
        if (!useTestData)
        {
            maxX = 101;
            maxY = 103;
        }

        PrintBotMap(maxX, maxY);

        foreach (var robot in robots)
        {
            AnsiConsole.MarkupLineInterpolated($"Robot starting: [yellow]{robot.Coord.X}[/],[yellow]{robot.Coord.Y}[/]");
            var temp = robot.Coord.X + (robot.vX * 100);
            var endX = temp % maxX;

            if (endX < 0)
                endX = maxX + endX;
            
            temp = robot.Coord.Y + (robot.vY * 100);
            var endY = temp % maxY;
            
            if (endY < 0)
                endY = maxY + endY;
            robot.Coord = new Coord(endX, endY);
            AnsiConsole.MarkupLineInterpolated($"Robot ending: [yellow]{robot.Coord.X}[/],[yellow]{robot.Coord.Y}[/]");
        }

        
        
        PrintBotMap(maxX, maxY);
        
       
        var locations = robots.GroupBy(x => x.Coord).ToDictionary(x => x.Key, y => y.Count());

        var midX = (maxX-1) / 2;
        var midY = (maxY-1) / 2;

        Dictionary<int, int> quadrantCounts = new()
        {
            { 0, 0 },
            { 1, 0 },
            { 2, 0 },
            { 3, 0 }
        };

        foreach (var location in locations)
        {
            var quadrant = location.Key switch
            {
                _ when location.Key.X < midX && location.Key.Y < midY => 0,
                _ when location.Key.X < midX && location.Key.Y > midY => 1,
                _ when location.Key.X > midX && location.Key.Y > midY => 2,
                _ when location.Key.X > midX && location.Key.Y < midY => 3,
                _ => -1
            };
            if (quadrant == -1) continue;
            
            quadrantCounts[quadrant]+= location.Value;
        }

        var total = quadrantCounts[0] * quadrantCounts[1] * quadrantCounts[2] * quadrantCounts[3];

        AnsiConsole.MarkupLineInterpolated($"Total: [green]{total}[/]");
    }

    private void PrintBotMap(int maxX, int maxY, bool skipMids = true)
    {
        if (_map == null)
        {
            _map = new Grid<sbyte>(0, 0, maxX-1, maxY-1);
            _map.InitMap();
        }
        

        var locations = robots.GroupBy(x => x.Coord).ToDictionary(x => x.Key, y => y.Count());

        var midX = (maxX-1) / 2;
        var midY = (maxY-1) / 2;
        
        _map.PrintMap((_, coord) =>
        {
            var letter = '.';
            locations.TryGetValue(coord, out int botCount);
            
            if (skipMids && (coord.X == midX || coord.Y == midY) )
            {
                letter = ' ';
            }else if (botCount > 0)
            {
                letter = botCount.ToString()[0];
            }
            
            return (letter,
                botCount > 0 ? Color.FromInt32(1 + botCount) : Color.Grey, null);
        });


    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, false);
        
        var maxX = 101;
        var maxY = 103;
        var midX = (maxX - 1) / 2;
        var midY = (maxY - 1) / 2;

        PrintBotMap(maxX, maxY, false);

       int iterations = 0;
       int[] rowCounts = new int[maxX];
       int[] colCounts = new int[maxY];

       for (int i = 0; i < maxX; i++)
       {
           rowCounts[i] = 0;
       }

       for (int i = 0; i < maxY; i++)
       {
           colCounts[i] = 0;
       }
        
        while (true)
        {
            for (int i = 0; i < maxX; i++)
            {
                rowCounts[i] = 0;
            }

            for (int i = 0; i < maxY; i++)
            {
                colCounts[i] = 0;
            }
            
            iterations++;
            if (iterations % 100000 == 0)
            {
                AnsiConsole.MarkupLineInterpolated($"Ran [yellow]{iterations}[/] so far.");
            }
         
            foreach (var robot in robots)
            {
                //AnsiConsole.MarkupLineInterpolated($"Robot starting: [yellow]{robot.Coord.X}[/],[yellow]{robot.Coord.Y}[/]");
                var temp = robot.Coord.X + robot.vX;
                var endX = temp % maxX;

                if (endX < 0)
                    endX = maxX + endX;
            
                temp = robot.Coord.Y + robot.vY;
                var endY = temp % maxY;
            
                if (endY < 0)
                    endY = maxY + endY;
                robot.Coord = new Coord(endX, endY);

                rowCounts[robot.Coord.X]++;
                colCounts[robot.Coord.Y]++;
              
                //AnsiConsole.MarkupLineInterpolated($"Robot ending: [yellow]{robot.Coord.X}[/],[yellow]{robot.Coord.Y}[/]");
            }

            if (rowCounts.OrderByDescending(x => x).Take(10).Sum() > 200
                && colCounts.OrderByDescending(x => x).Take(10).Sum() > 200)
            {
                PrintBotMap(maxX, maxY, false);
                Debugger.Break();                
            }

            //PrintBotMap(maxX, maxY, false);
            //Thread.Sleep(100);
        }
       
    }

    [GeneratedRegex(@"p=(\d+),(\d+) v=(-?\d+),(-?\d+)")]
    private static partial Regex ParserRegex();
}