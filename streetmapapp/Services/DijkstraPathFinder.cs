using System;
using System.Collections.Generic;
using StreetMapApp.Models;

namespace StreetMapApp.Services;

public static class DijkstraPathFinder
{
    // Returns the weighted shortest path and its total distance. Empty path if unreachable.
    public static (List<string> Path, int Distance) ShortestPath(StreetMap map, string start, string end)
    {
        if (!map.Intersections.ContainsKey(start) || !map.Intersections.ContainsKey(end))
        {
            return (new List<string>(), 0);
        }

        var dist = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var prev = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        var comparer = StringComparer.OrdinalIgnoreCase;

        foreach (var key in map.Intersections.Keys)
        {
            dist[key] = int.MaxValue;
            prev[key] = null;
        }
        dist[start] = 0;

        var pq = new PriorityQueue<string, int>();
        pq.Enqueue(start, 0);

        while (pq.Count > 0)
        {
            pq.TryDequeue(out var u, out var d);
            if (d > dist[u]) continue; // stale entry
            if (string.Equals(u, end, StringComparison.OrdinalIgnoreCase)) break;

            foreach (var road in map.Intersections[u].Roads)
            {
                var v = road.Destination.Name;
                var nd = d + Math.Max(road.Distance, 0);
                if (nd < dist[v])
                {
                    dist[v] = nd;
                    prev[v] = u;
                    pq.Enqueue(v, nd);
                }
            }
        }

        if (dist[end] == int.MaxValue)
        {
            return (new List<string>(), 0);
        }

        var path = new List<string>();
        string? node = end;
        while (node != null)
        {
            path.Add(node);
            node = prev.TryGetValue(node, out var p) ? p : null;
        }
        path.Reverse();
        return (path, dist[end]);
    }
}
