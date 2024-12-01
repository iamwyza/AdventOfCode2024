//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Linq;

//namespace AdventOfCode2024.GridUtilities;

//internal static partial class Algorithms<TVertexType> where TVertexType : IEquatable<TVertexType>
//{
//    public static Stack<TVertexType>? FindPath(IGraph<TVertexType> graph, TVertexType source,
//        Func<TVertexType, TVertexType, bool>? parentValidationLogic = null,
//        Func<TVertexType, Direction, bool>? neighborFilter = null,
//        Action<TVertexType>? debugAction = null)
//    {
//        TVertexType start = source,
//        TVertexType end = new TVertexType(new Coord(endVector.X, endVector.Y), true);

//        Stack<TVertexType> path = new Stack<TVertexType>();
//        PriorityQueue<TVertexType, float> openList = new PriorityQueue<TVertexType, float>();
//        List<TVertexType> closedList = new();
//        IEnumerable<TVertexType> adjacencies;
//        TVertexType current = start;

//        // add start TVertexType to Open List
//        openList.Enqueue(start, 0);

//        while (openList.Count != 0)
//        {
//            current = openList.Dequeue();
//            closedList.Add(current);
//            adjacencies = graph.GetNeighbors(current, neighborFilter);

//            foreach (TVertexType n in adjacencies)
//            {
//                if (!closedList.Contains(n))
//                {

//                    if (parentValidationLogic != null && !parentValidationLogic(current, n)) continue;

//                    bool isFound = false;
//                    foreach (var oLTVertexType in openList.UnorderedItems)
//                    {
//                        if (oLTVertexType.Element.Equals(n))
//                        {
//                            isFound = true;
//                        }
//                    }

//                    if (!isFound)
//                    {
//                        n.Parent = current;
//                        //n.DistanceToTarget = Math.Abs(n.Position.X - end.Position.X) + Math.Abs(n.Position.Y - end.Position.Y);
//                        n.Cost = n.Weight + n.Parent.Cost;
//                        openList.Enqueue(n, n.Cost);
//                    }
//                }
//            }
//        }

//        // construct path, if end was not closed return null
//        if (!closedList.Exists(x => x.Position == end.Position))
//        {
//            return null;
//        }

//        // if all good, return path
//        TVertexType temp = closedList[closedList.IndexOf(current)];
//        if (temp == null) return null;
//        do
//        {
//            path.Push(temp);
//            temp = temp.Parent;
//        } while (temp != start && temp != null);

//        return path;
//    }

//}