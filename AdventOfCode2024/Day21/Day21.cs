using System.Diagnostics;
using AdventOfCode2024.GridUtilities;

namespace AdventOfCode2024.Day21;
internal class Day21 : DayBase
{

    public Day21()
    {
        Ready = true;
    }

    private Grid<sbyte> _map;

    [MemberNotNull(nameof(_map))]
    private async Task Init(int part, bool useTestData)
    {
        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        _map = new Grid<sbyte>(lines, c => c switch
        {
            '.' => 0,
            '#' => 1,
            'S' => 2
        });

        _map.DefaultPrintConfig = (value, coord) =>
            (value switch
            {
                0 => '.',
                1 => '#',
                2 => 'S'
            }, value switch
            {
                0 => Color.Default,
                1 => Color.Red,
                2 => Color.Green
            }, "");
        _map.PrintMap();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        var overlayGrid = _map.CopyGrid((_, input) => false);

        overlayGrid.DefaultPrintConfig = (value, coord) =>
            (_map[coord] == 1 ? '#' : (value ? '0' : '.'), value ? Color.Green : Color.Default, "");

        var pathFindingGrid = _map.CopyGrid((coord, input) =>
            (sbyte)1
        );

        

        Coord start = new Coord();

        foreach (var cell in _map)
        {
            if (cell.Item2 != 2) continue;

            start = cell.Item1;
            break;
        }

        overlayGrid[start] = true;

        var graph = new StateGraph(pathFindingGrid);
        
        var results = Algorithms<StateGraph.State>.Dijkstra(graph: graph,
            source: new StateGraph.State(start, Direction.None, 0),
            isTarget: _ => false,
            continuationLogic: node =>
            {
                if (node.Distance <= 64)
                {
                    if (node.Distance %2 == 0) overlayGrid[node.Position] = true;
                    return true;
                }

                return false;
            },
            neighborFilter: (state, direction) => _map[state.Position.Move(direction)] != 1);

        overlayGrid.PrintMap();
        var spots = overlayGrid.Count(x => x.Value); //Math.Round(overlayGrid.Count(x => x.Value) / 2D, MidpointRounding.AwayFromZero) +1;

        AnsiConsole.MarkupLineInterpolated($"Can reach... [green]{spots}[/] spots?");

    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, true);
        int stepCount = 50;

        var biggerMap = new Grid<sbyte>(0,0,((_map.Bounds.maxX+1)*3)-1, ((_map.Bounds.maxY+1)*3)-1);

        foreach (var cell in _map)
        {
            biggerMap[cell.Item1] = cell.Item2 == 2 ? (sbyte)0 : cell.Item2 ;

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    var value = cell.Item2;
                    if ((x != 1 || y != 1) && value == 2)
                    {
                        value = 0;
                    }
                    biggerMap[cell.Item1.X + ((_map.Bounds.maxX + 1) * x), cell.Item1.Y + ((_map.Bounds.maxY + 1) * y)] = value;
                }
            }

        }

        _map = biggerMap;
        _map.PrintMap();

        var overlayGrid = _map.CopyGrid((_, input) => false);

        overlayGrid.DefaultPrintConfig = (value, coord) =>
            (_map[coord] == 1 ? '#' : (value ? '0' : '.'), value ? Color.Green : Color.Default, "");

        var pathFindingGrid = _map.CopyGrid((coord, input) =>
            (sbyte)1
        );



        Coord start = new Coord();

        foreach (var cell in _map)
        {
            if (cell.Item2 != 2) continue;

            start = cell.Item1;
            break;
        }

        overlayGrid[start] = true;

        var graph = new StateGraph(pathFindingGrid);
        var upperLeftReached = false;
        var upperRightReached = false;
        var lowerLeftReached = false;
        var lowerRightReached = false;

        var upperLeft = new Coord();
        var upperRight = new Coord(_map.Bounds.maxX, 0);
        var lowerLeft = new Coord(0, _map.Bounds.maxY);
        var lowerRight = new Coord(_map.Bounds.maxX, _map.Bounds.maxY);

        var results = Algorithms<StateGraph.State>.Dijkstra(graph: graph,
            source: new StateGraph.State(start, Direction.None, 0),
            isTarget: state =>
            {

                if (state.Position == upperLeft) upperLeftReached = true;
                if (state.Position == upperRight) upperRightReached = true;
                if (state.Position == lowerLeft) lowerLeftReached = true;
                if (state.Position == lowerRight) lowerRightReached = true;

                if (upperLeftReached && upperRightReached && lowerRightReached && lowerLeftReached)
                {
                    return true;
                }


                return false;
            },
            continuationLogic: node =>
            {
                if (node.Distance <= stepCount)
                {
                    if (node.Distance % 2 == 0) overlayGrid[node.Position] = true;
                    return true;
                }

                return false;
            },
            neighborFilter: (state, direction) => _map[state.Position.Move(direction)] != 1 && !overlayGrid[state.Position.Move(direction)] );

        overlayGrid.PrintMap();
        var spots = overlayGrid.Count(x => x.Value); //Math.Round(overlayGrid.Count(x => x.Value) / 2D, MidpointRounding.AwayFromZero) +1;

        var total = spots * (stepCount / (results?.distance ?? 1));

        AnsiConsole.MarkupLineInterpolated($"Distance min distance to edges: [green]{results?.distance}[/]");
        AnsiConsole.MarkupLineInterpolated($"Number of spots reached within that distance was [green]{spots}[/]");
        AnsiConsole.MarkupLineInterpolated($"Can reach... [green]{total}[/] spots?");
    }
}