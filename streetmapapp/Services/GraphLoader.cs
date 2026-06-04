using System.Text.Json;
using StreetMapApp.Models;

namespace StreetMapApp.Services;

public static class GraphLoader
{
    public static StreetMap Load(string dataPath)
    {
        var json = GraphFileReader.Read(dataPath);
        using var doc = JsonDocument.Parse(json);
        var intersections = GraphJsonParser.Parse(doc.RootElement);
        return StreetMapBuilder.Build(intersections);
    }
}
