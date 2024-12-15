using AdventOfCode2024.GridUtilities;

namespace AdventOfCode2024.Day15;

internal class Day15 : DayBase
{
    private enum MapState
    {
        Wall = '#',
        Empty = '.',
        Box = 'O',
        Robot = '@'
    }

    private enum DoubleMapState
    {
        Wall = '#',
        Empty = '.',
        LBox = '[',
        RBox = ']',
        Robot = '@'
    }

    public Day15()
    {
        Ready = true;
    }

    private Grid<MapState> _map;
    private List<Direction> _moves;
    private Coord _robot;

    [MemberNotNull(nameof(_map))]
    [MemberNotNull(nameof(_moves))]
    private async Task Init(int part, bool useTestData)
    {
        _moves = new List<Direction>();

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        List<string> mapLines = new List<string>();
        foreach (var line in lines)
        {
            if (line.StartsWith('#'))
            {
                mapLines.Add(line);
            }
            else
            {
                _moves.AddRange(line.Select(x => x switch
                {
                    '<' => Direction.West,
                    'v' => Direction.South,
                    '^' => Direction.North,
                    '>' => Direction.East
                }));
            }
        }

        _map = new Grid<MapState>(mapLines.ToArray(), c => (MapState)c);
        _map.DefaultPrintConfig = (state, coord) => ((char)state, state switch
        {
            MapState.Wall => Color.White,
            MapState.Empty => Color.Grey,
            MapState.Box => Color.Blue,
            MapState.Robot => Color.Red,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        }, null);

        _robot = _map.Single(x => x.Value == MapState.Robot).Coordinate;

        _map.PrintMap();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);


        foreach (var direction in _moves)
        {
            var moveTo = _robot.Move(direction);
            if (_map[moveTo] == MapState.Empty)
            {
                _map[_robot] = MapState.Empty;
                _map[moveTo] = MapState.Robot;
                _robot = moveTo;
                continue;
            }

            if (_map[moveTo] == MapState.Wall)
            {
                continue;
            }

            var nextMoveTo = moveTo.Move(direction);
            while (_map[nextMoveTo] != MapState.Empty && _map[nextMoveTo] != MapState.Wall)
            {
                nextMoveTo = nextMoveTo.Move(direction);
            }

            if (_map[nextMoveTo] == MapState.Empty)
            {
                _map[nextMoveTo] = MapState.Box;
                _map[_robot] = MapState.Empty;
                _map[moveTo] = MapState.Robot;
                _robot = moveTo;
            }
        }

        _map.PrintMap();
        int total = 0;
        foreach (var location in _map.Where(x => x.Value == MapState.Box))
        {
            total += (location.Coordinate.Y * 100) + location.Coordinate.X;
        }

        AnsiConsole.MarkupLineInterpolated($"The GPS Box value is [green]{total}[/].");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, false);

        var bigMap = new Grid<DoubleMapState>(0, 0, ((_map.Columns) * 2) + 1, _map.Rows);

        bigMap.InitMap();
        bigMap.ResetMap(DoubleMapState.Empty);

        Dictionary<Coord, int> coordToBoxLookup = new Dictionary<Coord, int>();
        Dictionary<int, (Coord, Coord)> boxToCoordLookup = new();
        int boxId = 0;

        bigMap.DefaultPrintConfig = (state, coord) => ((char)state, state switch
        {
            DoubleMapState.Wall => Color.White,
            DoubleMapState.Empty => Color.Grey,
            DoubleMapState.LBox => Color.Blue,
            DoubleMapState.RBox => Color.Blue,
            DoubleMapState.Robot => Color.Red,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        }, null);
        for (int y = 0; y <= _map.Rows; y++)
        {
            int offset = 0;

            for (int x = 0; x <= _map.Columns; x++)
            {
                var mapState = _map[x, y];

                var left = new Coord(x + offset, y);
                var right = new Coord(x + offset + 1, y);
                
                switch (mapState)
                {
                    case MapState.Wall:
                        bigMap[left] = DoubleMapState.Wall;
                        bigMap[right] = DoubleMapState.Wall;
                        break;
                    case MapState.Box:
                        bigMap[left] = DoubleMapState.LBox;
                        bigMap[right] = DoubleMapState.RBox;
                        boxToCoordLookup.Add(boxId, (left, right));
                        coordToBoxLookup.Add(left, boxId);
                        coordToBoxLookup.Add(right, boxId);
                        boxId++;
                        break;
                    case MapState.Robot:
                        bigMap[left] = DoubleMapState.Robot;
                        _robot = left;
                        break;
                }

                offset++;
            }
        }

        boxId = 0;
        bigMap.PrintMap();

        foreach (var direction in _moves)
        {
            var moveTo = _robot.Move(direction);
            if (bigMap[moveTo] == DoubleMapState.Empty)
            {
                bigMap[_robot] = DoubleMapState.Empty;
                bigMap[moveTo] = DoubleMapState.Robot;
                _robot = moveTo;
               // bigMap.PrintMap(printRowLabel:true);
                continue;
            }

            if (bigMap[moveTo] == DoubleMapState.Wall)
            {
                continue;
            }

            var canMove = true;
            Queue<(Coord LBox, Coord RBox, int qid)> boxesToCheck = new();
            int? initialBoxId = null;

            if (bigMap[moveTo] is DoubleMapState.LBox or DoubleMapState.RBox)
            {
                (Coord, Coord, int)? initial = CheckBox(moveTo);
                if (initial is null)
                    continue;
                
                initialBoxId = initial.Value.Item3;
                
                boxesToCheck.Enqueue(initial.Value);
            }

            var boxesToMove = new HashSet<int>();

            while (boxesToCheck.TryDequeue(out (Coord LBox, Coord RBox, int qid) boxPairs))
            {
                if (initialBoxId.HasValue && boxPairs.qid == initialBoxId.Value)
                {
                    if (bigMap[boxPairs.LBox.Move(direction)] == DoubleMapState.Wall ||
                        bigMap[boxPairs.LBox.Move(direction)] == DoubleMapState.Wall)
                    {
                        canMove = false;
                        boxesToCheck.Clear();
                        boxesToMove.Clear();
                        break;
                    }
                }
                
                var iBoxL = boxPairs.LBox;
                var iBoxR = boxPairs.RBox;
                boxId = coordToBoxLookup[iBoxL];



                if (!boxesToMove.Add(boxId))
                {
                    continue;
                }

                
                var boxL = boxPairs.LBox.Move(direction);
                var boxR = boxPairs.RBox.Move(direction);
                

               // bigMap.PrintMap(printRowLabel: true,config:((state, coord) => ((char)state, coord == boxL || coord == boxR ? Color.Red : coord == iBoxL || coord == iBoxR ? Color.Blue : Color.Grey, null)));
                

                if (bigMap[boxL] == DoubleMapState.Wall ||
                    bigMap[boxR] == DoubleMapState.Wall)
                {
                    canMove = false;
                    break;
                }

                if (bigMap[boxL] == DoubleMapState.Empty && bigMap[boxR] == DoubleMapState.Empty)
                {
                    continue;
                }

                (Coord lbox, Coord rbox, int id)? toAddL = CheckBox(boxL);
                (Coord lbox, Coord rbox, int id)? toAddR = CheckBox(boxR);

                if (toAddL.HasValue)
                    boxesToCheck.Enqueue(toAddL.Value);
                
                if (toAddR.HasValue)
                    boxesToCheck.Enqueue(toAddR.Value);
            
            }

            if (canMove)
            {
                var toaddback = new Dictionary<int, (Coord, Coord)>();

                foreach (var id in boxesToMove)
                {
                    var (lbox, rbox) = boxToCoordLookup[id];
                    bigMap[lbox] = DoubleMapState.Empty;
                    bigMap[rbox] = DoubleMapState.Empty;
                }
                
                foreach (var id in boxesToMove)
                {
                    var (lbox, rbox) = boxToCoordLookup[id];
                    coordToBoxLookup.Remove(lbox);
                    coordToBoxLookup.Remove(rbox);
                    lbox = lbox.Move(direction);
                    rbox = rbox.Move(direction);
                    bigMap[lbox] = DoubleMapState.LBox;
                    bigMap[rbox] = DoubleMapState.RBox;
                    boxToCoordLookup[id] = (lbox, rbox);
                    toaddback.Add(id, (lbox, rbox));
                    
                }
                
                foreach (var pair in toaddback)
                {
                    coordToBoxLookup[pair.Value.Item1] = pair.Key;
                    coordToBoxLookup[pair.Value.Item2] = pair.Key;
                }

             
                
                bigMap[_robot] = DoubleMapState.Empty;
                bigMap[moveTo] = DoubleMapState.Robot;
                _robot = moveTo;
                
               // bigMap.PrintMap(printRowLabel: true);

            }
        }

        bigMap.PrintMap(printRowLabel: true);
        int total = 0;
        foreach (var location in boxToCoordLookup.Values)
        {
            int aY = location.Item1.Y;
            int aX = location.Item1.X; 
            
            AnsiConsole.MarkupLineInterpolated($"Box at [yellow]{location.Item1}:{location.Item2}[/] has a value of Y=[blue]{aY}[/], X=[blue]{aX}[/] ([green]{(aY*100) + aX}[/])");
            total += (aY * 100) + aX;
        }

        AnsiConsole.MarkupLineInterpolated($"The GPS Box value is [green]{total}[/].");

        (Coord, Coord, int)? CheckBox(Coord box)
        {
            if (bigMap[box] is not (DoubleMapState.LBox or DoubleMapState.RBox)) return null;
            if (coordToBoxLookup.TryGetValue(box, out int id))
            {
                var (lbox, rbox) = boxToCoordLookup[id];
                return (lbox, rbox, id);
            }

            return null;
        }
    }
}