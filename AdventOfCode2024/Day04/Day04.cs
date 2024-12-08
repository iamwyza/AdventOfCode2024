using AdventOfCode2024.GridUtilities;

namespace AdventOfCode2024.Day04;

// https://adventofcode.com/2024/day/4
internal class Day04 : DayBase
{
    public Day04()
    {
        Ready = true;
    }

    private Grid<char> _map;
    private Grid<sbyte> _outputGrid;

    [MemberNotNull(nameof(_map))]
    private async Task Init(int part, bool useTestData)
    {
        

        var lines = useTestData ? await GetTestLines(part) : await GetLines();
        _map = new Grid<char>(lines, c => c);

        _outputGrid = _map.CopyGrid((c, v) => (sbyte)0);
        
        _map.PrintMap();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);
        
        var totalWords = 0;
        
        foreach (var (coord, value) in _map)
        {
            if (value != 'X') continue;

            if (!_map.AdjacentTest(c => c == 'M', coord, false, out Coord[]? adjacentCoords)) continue;
            
            foreach (var m in adjacentCoords)
            {
                var direction = DirectionExtensions.GetDirection(m, coord);
                    
                var a = m.Move(direction);
                if (!_map.InBounds(a) || _map[a] != 'A') continue;

                var s = a.Move(direction);
                if (!_map.InBounds(s) || _map[s] != 'S') continue;
                    
                totalWords++;
                _outputGrid[coord] = _outputGrid[m] = _outputGrid[a] = _outputGrid[s] = 1;
            }
        }

        _map.PrintMap((c, coord) => (_outputGrid[coord] == 0 ? '.' : c, _outputGrid[coord] == 0 ? Color.Default : Color.Green, null));

        AnsiConsole.MarkupLineInterpolated($"XMAS found {totalWords} times");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, false);
        
        var totalWords = 0;
        
        foreach (var (coord, value) in _map)
        {
            if (value != 'A') continue;

            var topLeftCoord = coord.Move(Direction.NorthWest);
            var topRightCoord = coord.Move(Direction.NorthEast);
            var bottomLeftCoord = coord.Move(Direction.SouthWest);
            var bottomRightCoord = coord.Move(Direction.SouthEast);
            
            if (!(_map.InBounds(topLeftCoord) && _map.InBounds(topRightCoord) && _map.InBounds(bottomLeftCoord) && _map.InBounds(bottomRightCoord))) continue;
            
            var topLeft = _map[topLeftCoord];
            var topRight = _map[topRightCoord];
            var bottomLeft = _map[bottomLeftCoord];
            var bottomRight = _map[bottomRightCoord];
            
            if (!(topLeft is 'S' or 'M' && topRight is 'S' or 'M' && bottomLeft is 'S' or 'M' && bottomRight is 'S' or 'M')) continue;

            if ((topLeft == topRight && bottomLeft == bottomRight && topLeft != bottomLeft)
                || (topLeft == bottomLeft && topRight == bottomRight && topRight != topLeft))
            {
                _outputGrid[coord] = _outputGrid[topLeftCoord] = _outputGrid[topRightCoord] =
                    _outputGrid[bottomLeftCoord] = _outputGrid[bottomRightCoord] = 1;

                totalWords++;
            }
                
            
        }

        _map.PrintMap((c, coord) => (_outputGrid[coord] == 0 ? '.' : c, _outputGrid[coord] == 0 ? Color.Default : Color.Green, null));

        AnsiConsole.MarkupLineInterpolated($"X-MAS found {totalWords} times");
    }
}