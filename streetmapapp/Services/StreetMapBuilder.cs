using StreetMapApp.Models;

namespace StreetMapApp.Services;

internal static class StreetMapBuilder
{
    public static StreetMap Build(Dictionary<string, List<(string Destination, int Distance, RoadDirection Direction)>> intersections)
    {
        var map = new StreetMap();

        foreach (var name in intersections.Keys)
        {
            map.Intersections[name] = new Intersection { Name = name };
        }

        foreach (var intersection in intersections)
        {
            var from = map.Intersections[intersection.Key];

            foreach (var road in intersection.Value)
            {
                if (!map.Intersections.TryGetValue(road.Destination, out var dest))
                {
                    dest = new Intersection { Name = road.Destination };
                    map.Intersections[road.Destination] = dest;
                }

                from.Roads.Add(new Road { Destination = dest, Distance = road.Distance, Direction = road.Direction });
            }
        }

        return map;
    }
}
