namespace AdventOfCode2024.GridUtilities;
internal class StateGraph : IGraph<StateGraph.State>
{
    public readonly struct State : IEquatable<State>, IComparable<long>
    {
        private readonly long _value;
        public Coord Position => new((uint)((_value >> 32) & 0xffffffff));
        public Direction Direction => (Direction)(_value >> 19 & 0x1fff);
        public int Distance => (int)(_value & 0xfffff);


        public State(long value)
        {
            _value = value;
        }
        
        public State(Coord position, Direction direction, int distance)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(distance, 0xfffff);

            _value = ((long)position.RawValue << 32)
                     + ((long)((int)direction & 0x1fff) << 19)
                     + distance;
        }

        public bool Equals(State other)
        {
            return Position == other.Position && Direction == other.Direction;
        }

        public override bool Equals(object? obj)
        {
            return obj is State other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Position, Direction);
        }


        public static bool operator ==(State left, State right)
        {
            return left.Position == right.Position && left.Direction == right.Direction;
        }

        public static bool operator !=(State left, State right)
        {
            return left.Position != right.Position || left.Direction != right.Direction;
        }

        public static bool operator ==(State left, long right)
        {
            return left.Position.RawValue == (uint)((right >> 32) & 0xffffffff) && left.Direction == (Direction)(right >> 19 & 0x1fff);
        }

        public static bool operator !=(State left, long right)
        {
            return left.Position.RawValue != (uint)((right >> 32) & 0xffffffff) || left.Direction != (Direction)(right >> 19 & 0x1fff);
        }

        public int CompareTo(long other)
        {
            return Position.RawValue.CompareTo((uint)((other >> 32) & 0xffffffff)) + Direction.CompareTo((Direction)(other >> 19 & 0x1fff));
        }
    }

    public readonly Grid<sbyte> Grid;

    public StateGraph(Grid<sbyte> grid)
    {
        Grid = grid;
    }

    public virtual IEnumerable<State> GetVertices()
    {
        return Grid.Select(x => new State(x.Coordinate, Direction.None, 0)).ToList();
    }

    public virtual IEnumerable<State> GetNeighbors(State vertex, Func<State, Direction, bool>? neighborFilter)
    {
        int row = vertex.Position.Y;
        int col = vertex.Position.X;

        if (row + 1 < Grid.Rows + 1 && vertex.Direction != Direction.North)
        {
            if (neighborFilter is null || neighborFilter.Invoke(vertex, Direction.South))
                yield return new State(vertex.Position.Move(Direction.South), Direction.South, vertex.Distance + 1);
        }
        if (row - 1 >= 0 && vertex.Direction != Direction.South)
        {
            if (neighborFilter is null || neighborFilter.Invoke(vertex, Direction.North))
                yield return new State(vertex.Position.Move(Direction.North), Direction.North, vertex.Distance + 1);
        }
        if (col - 1 >= 0 && vertex.Direction != Direction.East)
        {
            if (neighborFilter is null || neighborFilter.Invoke(vertex, Direction.West))
                yield return new State(vertex.Position.Move(Direction.West), Direction.West, vertex.Distance + 1);
        }
        if (col + 1 < Grid.Columns + 1 && vertex.Direction != Direction.West)
        {
            if (neighborFilter is null || neighborFilter.Invoke(vertex, Direction.East))
                yield return new State(vertex.Position.Move(Direction.East), Direction.East, vertex.Distance + 1);
        }
    }

    public virtual int GetWeight(State u, State v)
    {
        return Grid[v.Position];
    }

}

internal class UniqueStateGraph : IGraph<UniqueStateGraph.State>
{
    public readonly struct State : IEquatable<State>, IComparable<long>
    {
        private readonly long _value;
        public Coord Position => new((uint)((_value >> 32) & 0xffffffff));
        public Direction Direction => (Direction)(_value >> 19 & 0x1fff);
        public int Distance => (int)(_value & 0xfffff);


        public State(Coord position, Direction direction, int distance)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(distance, 0xfffff);

            _value = ((long)position.RawValue << 32)
                     + ((long)((int)direction & 0x1fff) << 19)
                     + distance;
        }

        public bool Equals(State other)
        {
            return _value == other._value;
        }

        public override bool Equals(object? obj)
        {
            return obj is State other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(State left, State right)
        {
            return left._value == right._value;
        }

        public static bool operator !=(State left, State right)
        {
            return left._value != right._value;
        }

        public static bool operator ==(State left, long right)
        {
            return left._value == right;
        }

        public static bool operator !=(State left, long right)
        {
            return left._value != right;
        }

        public int CompareTo(long other)
        {
            return _value.CompareTo(other);
        }
    }

    public readonly Grid<sbyte> Grid;

    public UniqueStateGraph(Grid<sbyte> grid)
    {
        Grid = grid;
    }

    public virtual IEnumerable<State> GetVertices()
    {
        return Grid.Select(x => new State(x.Coordinate, Direction.None, 0)).ToList();
    }

    public virtual IEnumerable<State> GetNeighbors(State vertex, Func<State, Direction, bool>? neighborFilter)
    {
        int row = vertex.Position.Y;
        int col = vertex.Position.X;

        if (row + 1 < Grid.Rows + 1 && vertex.Direction != Direction.North)
        {
            if (neighborFilter is null || neighborFilter.Invoke(vertex, Direction.South))
                yield return new State(vertex.Position.Move(Direction.South), Direction.South, vertex.Distance + 1);
        }
        if (row - 1 >= 0 && vertex.Direction != Direction.South)
        {
            if (neighborFilter is null || neighborFilter.Invoke(vertex, Direction.North))
                yield return new State(vertex.Position.Move(Direction.North), Direction.North, vertex.Distance + 1);
        }
        if (col - 1 >= 0 && vertex.Direction != Direction.East)
        {
            if (neighborFilter is null || neighborFilter.Invoke(vertex, Direction.West))
                yield return new State(vertex.Position.Move(Direction.West), Direction.West, vertex.Distance + 1);
        }
        if (col + 1 < Grid.Columns + 1 && vertex.Direction != Direction.West)
        {
            if (neighborFilter is null || neighborFilter.Invoke(vertex, Direction.East))
                yield return new State(vertex.Position.Move(Direction.East), Direction.East, vertex.Distance + 1);
        }
    }

    public virtual int GetWeight(State u, State v)
    {
        return Grid[v.Position];
    }

}

public interface IGraph<TVertexType> where TVertexType : IEquatable<TVertexType>
{
    IEnumerable<TVertexType> GetVertices();
    IEnumerable<TVertexType> GetNeighbors(TVertexType vertex, Func<TVertexType, Direction, bool>? neighborFilter);
    int GetWeight(TVertexType u, TVertexType v);
}

internal static partial class Algorithms<TVertexType> where TVertexType : IEquatable<TVertexType>
{
    public static (TVertexType end, double distance, TVertexType[][] Path)?
        Dijkstra(IGraph<TVertexType> graph, TVertexType source, Func<TVertexType, double, bool> isTarget, 
            Func<TVertexType, bool>? continuationLogic = null,
            Func<TVertexType, Direction, bool>? neighborFilter = null, bool continueAfterTargetFound = false)
    {
        var distance = new Dictionary<TVertexType, double>();
        var remaining = new PriorityQueue<(TVertexType, TVertexType[]), double>();
        HashSet<TVertexType> visited = new HashSet<TVertexType>();
        var path = new Dictionary<TVertexType, TVertexType?>();

        distance[source] = graph.GetWeight(source, source);

        foreach (var vertex in graph.GetVertices())
        {
            if (!vertex.Equals(source))
            {
                distance[vertex] = double.PositiveInfinity;
                path[vertex] = default;
            }
        }

        foreach (var vertex in graph.GetNeighbors(source, neighborFilter))
        {

            if (!continuationLogic?.Invoke(vertex) ?? false)
                continue;

            var measure = graph.GetWeight(source, vertex);
            distance[vertex] = measure;
            path[vertex] = source;
            remaining.Enqueue((vertex, [source]), measure);

        }

        var histories = new List<TVertexType[]>();


        (TVertexType subject, TVertexType[] history) pair;
        double cost;
        
        while (remaining.TryDequeue(out pair, out cost))
        {
            var subject = pair.subject;
            var history = pair.history;

            if (isTarget.Invoke(subject, cost))
            {
                if (!continueAfterTargetFound)
                    return (subject, cost, [[..history, subject]]);

                histories.Add([..history, subject]);
                continue;
            }

            foreach (var vertex in graph.GetNeighbors(subject, neighborFilter))
            {

                if (visited.Contains(vertex))
                    continue;
                
                visited.Add(vertex);

                if (!continuationLogic?.Invoke(vertex) ?? false)
                    continue;

                var measure = cost + graph.GetWeight(subject, vertex);
                remaining.Enqueue((vertex,[subject, ..history]), measure);

                if (!distance.ContainsKey(vertex))
                {
                    distance.Add(vertex, measure);
                    path[vertex] = subject;
                }


                if (measure < distance[vertex])
                {
                    distance[vertex] = measure;
                    path[vertex] = subject;
                }

            }
        }

        if (isTarget.Invoke(pair.subject, cost))
        {
            histories.Add([pair.subject, ..pair.history]);
        }

        if (histories.Any())
            return (default!, -1, histories.ToArray());

        return null;
    }
}
