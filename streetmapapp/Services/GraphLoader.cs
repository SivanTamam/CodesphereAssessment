using System.Text.Json;
using StreetMapApp.Models;

namespace StreetMapApp.Services;

public static class GraphLoader
{
    public static StreetMap Load(string dataPath)
    {
        if (!File.Exists(dataPath))
        {
            throw new FileNotFoundException($"Data file not found at: {dataPath}");
        }

        var json = File.ReadAllText(dataPath);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (!root.TryGetProperty("intersections", out var intersectionsEl) || intersectionsEl.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException("Map is empty or failed to load.");
        }

        var map = new StreetMap();

        // First pass: create all intersections
        foreach (var prop in intersectionsEl.EnumerateObject())
        {
            var name = prop.Name;
            map.Intersections[name] = new Intersection { Name = name };
        }

        // Second pass: wire roads using destination by name
        foreach (var prop in intersectionsEl.EnumerateObject())
        {
            var from = map.Intersections[prop.Name];
            var interObj = prop.Value;
            if (interObj.TryGetProperty("roads", out var roadsEl) && roadsEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var roadEl in roadsEl.EnumerateArray())
                {
                    if (!roadEl.TryGetProperty("destination", out var destEl)) continue;
                    var destName = destEl.GetString() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(destName)) continue;

                    if (!map.Intersections.TryGetValue(destName, out var dest))
                    {
                        dest = new Intersection { Name = destName };
                        map.Intersections[destName] = dest;
                    }

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

                    // Direction handling (default: Bidirectional)
                    var direction = "Bidirectional";
                    if (roadEl.TryGetProperty("direction", out var dirEl) && dirEl.ValueKind == JsonValueKind.String)
                    {
                        direction = dirEl.GetString() ?? direction;
                    }

                    // Always add forward edge
                    var forwardDirection = direction.Equals("Bidirectional", StringComparison.OrdinalIgnoreCase)
                        ? RoadDirection.Bidirectional
                        : RoadDirection.OneWay;
                    from.Roads.Add(new Road { Destination = dest, Distance = distance, Direction = forwardDirection });

                    // If bidirectional, add reverse edge as well
                    if (direction.Equals("Bidirectional", StringComparison.OrdinalIgnoreCase))
                    {
                        dest.Roads.Add(new Road { Destination = from, Distance = distance, Direction = RoadDirection.Bidirectional });
                    }
                }
            }
        }

        return map;
    }
}
