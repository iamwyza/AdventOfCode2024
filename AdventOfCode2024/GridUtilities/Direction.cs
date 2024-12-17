namespace AdventOfCode2024.GridUtilities;

[Flags]
public enum Direction : int
{
    None = 0,
    Special = 1,
    North = 2,
    NorthEast = 4,
    East = 8,
    SouthEast = 16,
    South = 32,
    SouthWest = 64,
    West = 128,
    NorthWest = 256,
    NorthSouth = 512,
    EastWest = 1024,
}

internal static class DirectionExtensions
{

    private static ReadOnlySpan<int> Directions =>
        Enum.GetValues<Direction>().Where(d => (int)d > 1 && (int)d < 512).Select(x => (int)x).ToArray().AsSpan();
    
    public static Direction GetDirection(Coord from, Coord to)
    {
       
            if (from.X > to.X)
            {
                if (from.Y == to.Y ) return Direction.East;
                return from.Y > to.Y ? Direction.SouthEast : Direction.NorthEast;
            }

            if (from.X < to.X)
            {
                if (from.Y == to.Y ) return Direction.West;
                return from.Y > to.Y ? Direction.SouthWest : Direction.NorthWest;
            }

            if (from.Y > to.Y)
            {
                return Direction.South;
            }

            if (from.Y < to.Y)
            {
                return Direction.North;
            }

            if (from == to)
            {
                return Direction.None;
            }

            throw new Exception($"Attempted to move from {from} to {to}");
    }


    public static Direction GetDirection(int fromX, int fromY, int toX, int toY)
    {

        if (fromX > toX)
        {
            return Direction.East;
        }

        if (fromX < toX)
        {
            return Direction.West;
        }

        if (fromY > toY)
        {
            return Direction.South;
        }

        if (fromY < toY)
        {
            return Direction.North;
        }

        if (fromX == toX && fromY == toY)
        {
            return Direction.None;
        }

        throw new Exception($"Attempted to move from {fromX},{fromY} to {toX},{toY}");
    }

    public static Direction Turn(this Direction direction, int turns, bool clockwise = true, bool cardinalOnly = true)
    {
        turns = cardinalOnly ? turns * 2 : turns;
		
        if (clockwise)
        {
            var temp = (Directions.IndexOf((int)direction) + turns) % Directions.Length;
            
            return (Direction)Directions[temp];
        }
        else
        {
            var temp = (Directions.IndexOf((int)direction) - turns) % Directions.Length;
            if (temp < 0) 
                temp += Directions.Length;
            
            return (Direction)Directions[temp];
        }
    }

    public static int NumberOfRotations(this Direction from, Direction to)
    {
        if (from == to) return 0;
        
        switch (from)
        {
            case Direction.North:
                return to switch
                {
                    Direction.East => 1,
                    Direction.South => 2,
                    Direction.West => 1
                };
            case Direction.East:
                return to switch
                {
                    Direction.North => 1,
                    Direction.South => 1,
                    Direction.West => 2
                };
            case Direction.South:
                return to switch
                {
                    Direction.West => 1,
                    Direction.North => 2,
                    Direction.East => 1,
                };
            case Direction.West:
                return to switch
                {
                    Direction.North => 1,
                    Direction.South => 1,
                    Direction.East => 2
                };
            default:
                throw new Exception("Only Cardinal Directions supported");
        }
    }

    public static char DirectionString(this Direction direction)
    {
        return direction switch
        {
            Direction.None => '.',
            Direction.Special => 'S',
            Direction.North => '^',
            Direction.East => '>',
            Direction.South => 'v',
            Direction.West => '<',
            Direction.NorthEast => 'F',
            Direction.NorthWest => '7',
            Direction.NorthSouth => '|',
            Direction.EastWest => '-',
            Direction.SouthEast => 'J',
            Direction.SouthWest => 'L',
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}
