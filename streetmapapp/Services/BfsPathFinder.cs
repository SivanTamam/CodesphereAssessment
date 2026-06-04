using StreetMapApp.Models;

namespace StreetMapApp.Services;

public static class BfsPathFinder
{
    public static List<string> ShortestPath(StreetMap map, string start, string end)
    {
        _ = map.Intersections[start];

        var queue = new Queue<string>();
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var prev = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        queue.Enqueue(start);
        visited.Add(start);
        prev[start] = null;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (string.Equals(current, end, StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            foreach (var road in RoadTraversal.GetReachableRoads(map, current))
            {
                var neighbor = road.Destination;
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    prev[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }

        if (!prev.ContainsKey(end))
        {
            return new List<string>();
        }

        var path = new List<string>();
        string? node = end;
        while (node != null)
        {
            path.Add(node);
            node = prev.TryGetValue(node, out var previous) ? previous : null;
        }
        path.Reverse();
        return path;
    }
}
