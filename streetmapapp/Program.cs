 using StreetMapApp.Models;
 using StreetMapApp.Services;
 using StreetMapApp.UI;
 
 ConsoleUi.PrintBanner();

// Data file path (run from the project folder for this relative path to work)
var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "data", "map.json");
if (!File.Exists(dataPath))
{
    Console.WriteLine($"Data file not found at: {dataPath}");
    Console.WriteLine("Ensure you're running from the streetmapapp folder or adjust the path.");
    return;
}

// Load graph via service
StreetMap streetMap;
try
{
    streetMap = GraphLoader.Load(dataPath);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    return;
}

ConsoleUi.PrintHelp();
ConsoleUi.PrintNodes(streetMap);


Console.WriteLine("Type 'exit' to quit. Type 'list' to reprint available nodes. Type 'help' for help.\n");
while (true)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("Enter start node (or 'exit'/'list'/'help'): ");
    Console.ResetColor();
    var start = Console.ReadLine()?.Trim();
    if (string.Equals(start, "exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    if (string.Equals(start, "help", StringComparison.OrdinalIgnoreCase))
    {
        ConsoleUi.PrintHelp();
        continue;
    }

    if (string.Equals(start, "list", StringComparison.OrdinalIgnoreCase))
    {
        ConsoleUi.PrintNodes(streetMap);
        continue;
    }

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("Enter end node: ");
    Console.ResetColor();
    var end = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(start) || string.IsNullOrWhiteSpace(end))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Start and end nodes are required.\n");
        Console.ResetColor();
        continue;
    }

    var resolvedStart = InputResolver.ResolveNode(streetMap, start);
    var resolvedEnd = InputResolver.ResolveNode(streetMap, end);

    if (resolvedStart is null || resolvedEnd is null)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("One or both nodes do not exist in the map. Try 'list' to see options.\n");
        Console.ResetColor();
        continue;
    }

    var path = BfsPathFinder.ShortestPath(streetMap, resolvedStart, resolvedEnd);

    if (path.Count == 0)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"No path found from '{resolvedStart}' to '{resolvedEnd}'.\n");
        Console.ResetColor();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Shortest path from '{resolvedStart}' to '{resolvedEnd}' (hops: {path.Count - 1}):");
        Console.ResetColor();
        // Pretty print path
        Console.WriteLine(string.Join(" -> ", path));
        Console.WriteLine();
    }
}

