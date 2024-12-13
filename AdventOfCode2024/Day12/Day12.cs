using AdventOfCode2024.GridUtilities;

namespace AdventOfCode2024.Day12;

internal class Day12 : DayBase
{
    public Day12()
    {
        Ready = true;
    }

    private Grid<char> _map;

    [MemberNotNull(nameof(_map))]
    private async Task Init(int part, bool useTestData)
    {
        var lines = useTestData ? await GetTestLines(part) : await GetLines();
        _map = new Grid<char>(lines, c => c);


        _map.PrintMap();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        var (regions, _) = GetRegions();

        var total = 0;
        foreach (var region in regions)
        {
            int perimeter = 0;

            foreach (var coord in region.Value)
            {
                if (_map.AdjacentTest(c => c == _map[coord], coord, true, out Coord[]? coords))
                {
                    perimeter += 4 - coords.Length;
                }
                else
                {
                    perimeter = 4;
                }
            }

            AnsiConsole.MarkupLineInterpolated(
                $"Size of [red]{region.Key}[/] is [green]{perimeter}[/] * [yellow]{region.Value.Length}[/]");
            total += perimeter * region.Value.Length;
        }

        AnsiConsole.MarkupLineInterpolated($"Total Area: [green]{total}[/]");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, false);

        var (regions, coordToRegion) = GetRegions();

        var total = 0;

        foreach (var region in regions)
        {
            var horizontalEdgeCount =  BoundedIteration(true, region);
            //AnsiConsole.MarkupLineInterpolated($"Horizontal Edges: [green]{horizontalEdgeCount}[/]");
            
            var verticalEdgeCount = BoundedIteration(false, region);

            //AnsiConsole.MarkupLineInterpolated($"Vertical Edges: [green]{verticalEdgeCount}[/]");
            var edgeCount = verticalEdgeCount + horizontalEdgeCount;


            //AnsiConsole.MarkupLineInterpolated($"Horizontal Edges: [blue]{edgeCount}[/]");
            AnsiConsole.MarkupLineInterpolated(
                $"Size of [red]{region.Key}[/] is [yellow]{region.Value.Length}[/] * [green]{edgeCount}[/]");
            total += edgeCount * region.Value.Length;
        }

        AnsiConsole.MarkupLineInterpolated($"Total Area: [green]{total}[/]");
    }

    int BoundedIteration(bool isXAxis, KeyValuePair<int,Coord[]> region)
    {
        int edgeCount = 0;
        var minX = region.Value.Select(c => c.X).Min();
        var minY = region.Value.Select(c => c.Y).Min();
        var maxX = region.Value.Select(c => c.X).Max();
        var maxY = region.Value.Select(c => c.Y).Max();
        
        bool inEdgeOne = false;
        bool inEdgeTwo = false;

        Direction one;
        Direction two;
        int minA, minB, maxA, maxB;
        if (isXAxis)
        {
            one = Direction.North;
            two = Direction.South;
            minA = minY;
            minB = minX;
            maxA = maxY;
            maxB = maxX;
            
        }
        else
        {
            one = Direction.East;
            two = Direction.West;
            minA = minX;
            minB = minY;
            maxA = maxX;
            maxB = maxY;

        }
        
         
        
        for (int a = minA; a <= maxA; a++)
        {
            for (int b = minB; b <= maxB; b++)
            {
                
                var coord = isXAxis ? new Coord(b, a) : new Coord(a, b);
                if (!region.Value.Contains(coord))
                {
                    if (inEdgeOne)
                        edgeCount++;
                    
                    if (inEdgeTwo)
                        edgeCount++;

                    inEdgeOne = inEdgeTwo = false;
                    continue;
                };
                // _map.PrintMap(((c, cc) => (c,
                //     cc == coord ? Color.Red : region.Value.Any(z => z == cc) ? Color.Green : Color.Grey, null)), printRowLabel:true);
                
                if (region.Value.Contains(coord.Move(one)))
                {
                    if (inEdgeOne)
                    {
                        inEdgeOne = false;
                        edgeCount++;
                    }
                }
                else
                {
                    inEdgeOne = true;
                }
                
                if (region.Value.Contains(coord.Move(two)))
                {
                    if (inEdgeTwo)
                    {
                        inEdgeTwo = false;
                        edgeCount++;
                    }
                }
                else
                {
                    inEdgeTwo = true;
                }
                //AnsiConsole.MarkupLineInterpolated($"Edge Count: [blue]{edgeCount}[/]");
            }
                
            if (inEdgeOne)
                edgeCount++;
                
            if (inEdgeTwo)
                edgeCount++;

            inEdgeOne = inEdgeTwo = false;
            
            //AnsiConsole.MarkupLineInterpolated($"Edge Count: [blue]{edgeCount}[/]");
        }

        return edgeCount;
    }

    private (Dictionary<int, Coord[]> RegionToCoord, Dictionary<Coord, int> CoordToRegion) GetRegions()
    {
        Queue<(Coord next, Coord? last)> queue = new();

        queue.Enqueue((new Coord(0, 0), null));

        int regionId = 0;

        var coordToRegionMap = new Dictionary<Coord, int>();
        var totalCoords = _map.Count();

        List<Coord> currentCoords = new();

        while (queue.TryDequeue(out (Coord next, Coord? history) item))
        {
            var (next, history) = item;

            //AnsiConsole.MarkupLineInterpolated($"Region:[red]{regionId}[/]: [yellow]{next}[/] - [white]{string.Join(' ', currentCoords)}[/]");
            bool hasMore = false;
            var letter = history is null ? _map[next] : _map[history.Value];
            if (_map.AdjacentTest(x => letter == x, next, cardinalOnly: true, out Coord[]? coords))
            {
                //AnsiConsole.MarkupLineInterpolated($"Adjacent Coords: [green]{string.Join(' ', coords)}[/] ");
                foreach (var coord in coords)
                {
                    if (!coordToRegionMap.ContainsKey(coord) && !currentCoords.Contains(coord))
                    {
                        queue.Enqueue((coord, next));
                        currentCoords.Add(coord);
                    }
                }
            }
            else if (history is null)
            {
                // when there are single entry regions
                currentCoords.Add(next);
            }

            if (queue.Count != 0) continue;

            if (currentCoords.Count != 0)
            {
                regionId++;
                foreach (var coord in currentCoords)
                {
                    coordToRegionMap.Add(coord, regionId);
                }

                currentCoords.Clear();
            }

            if (coordToRegionMap.Count != totalCoords)
            {
                queue.Enqueue((_map.First(x => !coordToRegionMap.ContainsKey(x.Coordinate)).Coordinate, null));
            }
            else
            {
                break;
            }
        }


        _map.PrintMap((c, coord) => (c, Color.FromInt32(coordToRegionMap[coord] % 256), null));

        return (coordToRegionMap.GroupBy(x => x.Value).ToDictionary(x => x.Key, x => x.Select(y => y.Key).ToArray()),
            coordToRegionMap);
    }
}