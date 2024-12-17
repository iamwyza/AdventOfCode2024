using System.Diagnostics;
using AdventOfCode2024.GridUtilities;

namespace AdventOfCode2024.Day16;
internal class Day16 : DayBase
{
    private class WeightGraph : StateGraph
    {
        public WeightGraph(Grid<sbyte> grid) : base(grid)
        {
        }

        public override int GetWeight(State u, State v)
        {
            var numberOfRotations = u.Direction.NumberOfRotations(v.Direction);
            return numberOfRotations == 0 ? 1 : (numberOfRotations * 1000) + 1;
        }
    }
    
    private class UniqueWeightGraph : UniqueStateGraph
    {
        public UniqueWeightGraph(Grid<sbyte> grid) : base(grid)
        {
        }

        public override int GetWeight(State u, State v)
        {
            var numberOfRotations = u.Direction.NumberOfRotations(v.Direction);
            return numberOfRotations == 0 ? 1 : (numberOfRotations * 1000) + 1;
        }
    }

    public Day16()
    {
        Ready = true;
    }

    private WeightGraph _graph;
    private UniqueWeightGraph _uniqueGraph;

    [MemberNotNull(nameof(_graph))]
    private async Task Init(int part, bool useTestData)
    {
        
        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        var map = new Grid<sbyte>(lines, c => c switch
        {
            '#' => 1,
            'S' => 2,
            'E' => 3,
            '.' => 0
        });

        map.PrintMap();
        _graph = new WeightGraph(map);
        _uniqueGraph = new UniqueWeightGraph(map);
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        var start = _graph.Grid.Single(x => x.Value == 2);
        var target = _graph.Grid.Single(x => x.Value == 3);
        var max = _graph.Grid.Count(x => x.Value == 0) * 3000;

        var result = Algorithms<StateGraph.State>.Dijkstra(_graph, new StateGraph.State(start.Coordinate, Direction.East, 0),
            (state, _) =>
            {
                // _graph.Grid.PrintMap((arg1, coord) => (coord == state.Position ? 'S' :  arg1 switch
                // {
                //     1 => '#',
                //     2 => 'S',
                //     3 => 'E',
                //     0 => '.'
                // }, coord == state.Position ? Color.Red : Color.Grey, null));
                return state.Position == target.Coordinate;
            }, (state => state.Distance < max),
            (state, direction) => (_graph.Grid[state.Position.Move(direction)] == 0 ||_graph.Grid[state.Position.Move(direction)] == 3) 
            && direction != state.Direction.Turn(2));
        
        AnsiConsole.MarkupLineInterpolated($"Path cost is [green]{result.Value.distance}[/]");
        
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, true);
        
        var start = _graph.Grid.Single(x => x.Value == 2);
        var target = _graph.Grid.Single(x => x.Value == 3);
        double max = _graph.Grid.Count(x => x.Value == 0) * 3000;
        
        var calculateBest = Algorithms<StateGraph.State>.Dijkstra(_graph, new StateGraph.State(start.Coordinate, Direction.East, 0),
            (state, _) => state.Position == target.Coordinate, (state => state.Distance < int.MaxValue),
            (state, direction) => (_graph.Grid[state.Position.Move(direction)] == 0 ||_graph.Grid[state.Position.Move(direction)] == 3) 
                                  && direction != state.Direction.Turn(2));


        max = calculateBest.Value.distance;
        AnsiConsole.MarkupLineInterpolated($"Path cost is [green]{max}[/]");

        var result = Algorithms<UniqueStateGraph.State>.Dijkstra(_uniqueGraph, new UniqueStateGraph.State(start.Coordinate, Direction.East, 0),
            (state, cost) =>
            {
                return state.Position == target.Coordinate && cost == max;
            }, (state => state.Distance <= max),
            (state, direction) => (_uniqueGraph.Grid[state.Position.Move(direction)] == 0 ||_uniqueGraph.Grid[state.Position.Move(direction)] == 3) 
                                  && direction != state.Direction.Turn(2), continueAfterTargetFound:true);

        foreach (var value in result.Value.Path.SelectMany(x => x))
        {
            _graph.Grid[value.Position] = 4;
        }
       
        _graph.Grid.PrintMap((arg1, coord) => ( arg1 switch
        {
            1 => '#',
            2 => 'S',
            3 => 'E',
            0 => '.',
            4 => '0'
        }, arg1 == 4 ? Color.Green : Color.Grey, null));
        
        AnsiConsole.MarkupLineInterpolated($"Path best seats count is [green]{_graph.Grid.Count(x => x.Value == 4)}[/]");
    }
}