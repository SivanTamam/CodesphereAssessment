using System.Text.Json;
using StreetMapApp.Models;

namespace StreetMapApp.Services;

internal static class GraphJsonParser
{
    public static Dictionary<string, List<(string Destination, int Distance, RoadDirection Direction)>> Parse(JsonElement root)
    {
        if (!root.TryGetProperty("intersections", out var intersectionsEl) || intersectionsEl.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException("Map is empty or failed to load.");
        }

        var intersections = new Dictionary<string, List<(string Destination, int Distance, RoadDirection Direction)>>(StringComparer.OrdinalIgnoreCase);

        foreach (var prop in intersectionsEl.EnumerateObject())
        {
            intersections[prop.Name] = ParseRoads(prop.Value);
        }

        return intersections;
    }

    private static List<(string Destination, int Distance, RoadDirection Direction)> ParseRoads(JsonElement intersectionEl)
    {
        var roads = new List<(string Destination, int Distance, RoadDirection Direction)>();

        if (!intersectionEl.TryGetProperty("roads", out var roadsEl) || roadsEl.ValueKind != JsonValueKind.Array)
        {
            return roads;
        }

        foreach (var roadEl in roadsEl.EnumerateArray())
        {
            if (!roadEl.TryGetProperty("destination", out var destEl)) continue;
            var destName = destEl.GetString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(destName)) continue;

            roads.Add((destName, ParseDistance(roadEl), ParseDirection(roadEl)));
        }

        return roads;
    }

    private static int ParseDistance(JsonElement roadEl)
    {
        int distance = 0;
        if (roadEl.TryGetProperty("distance", out var distEl) && distEl.ValueKind == JsonValueKind.Number)
        {
            try
            {
                distance = distEl.GetInt32();
            }
            catch
            {
                distance = 0;
            }
        }

        return distance;
    }

    private static RoadDirection ParseDirection(JsonElement roadEl)
    {
        var direction = "Bidirectional";
        if (roadEl.TryGetProperty("direction", out var dirEl) && dirEl.ValueKind == JsonValueKind.String)
        {
            direction = dirEl.GetString() ?? direction;
        }

        return direction.Equals("Bidirectional", StringComparison.OrdinalIgnoreCase)
            ? RoadDirection.Bidirectional
            : RoadDirection.OneWay;
    }
}
