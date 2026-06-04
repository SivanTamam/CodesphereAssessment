using StreetMapApp.Models;

namespace StreetMapApp.Services;

internal static class RoadTraversal
{
    public static IEnumerable<(string Destination, int Distance)> GetReachableRoads(StreetMap map, string current)
    {
        if (!map.Intersections.TryGetValue(current, out var intersection))
        {
            yield break;
        }

        foreach (var road in intersection.Roads)
        {
            yield return (road.Destination.Name, road.Distance);
        }

        foreach (var source in map.Intersections.Values)
        {
            foreach (var road in source.Roads)
            {
                if (road.Direction == RoadDirection.Bidirectional && string.Equals(road.Destination.Name, current, StringComparison.OrdinalIgnoreCase))
                {
                    yield return (source.Name, road.Distance);
                }
            }
        }
    }
}
