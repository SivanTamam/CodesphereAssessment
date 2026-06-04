namespace StreetMapApp.Services;
using StreetMapApp.Models;

public static class InputResolver
{
    public static string? ResolveNode(StreetMap map, string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        foreach (var key in map.Intersections.Keys)
        {
            if (string.Equals(key, input, StringComparison.OrdinalIgnoreCase))
            {
                return key;
            }
        }
        return null;
    }
}
