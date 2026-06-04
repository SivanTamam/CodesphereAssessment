using StreetMapApp.Models;

namespace StreetMapApp.UI;

public static class ConsoleUi
{
    public static void PrintBanner()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("============================================");
        Console.WriteLine("  StreetMapApp — Shortest Path (BFS)");
        Console.WriteLine("============================================\n");
        Console.ResetColor();
    }

    public static void PrintHelp()
    {
        Console.WriteLine("Instructions:");
        Console.WriteLine("- Enter a start and end node to compute the shortest path (unweighted).");
        Console.WriteLine("- Commands: 'list' to view nodes, 'help' for this help, 'exit' to quit.\n");
    }

    public static void PrintNodes(StreetMap map)
    {
        var keys = map.Intersections.Keys.OrderBy(k => k).ToList();
        Console.WriteLine("Available nodes:");

        // Print in columns that adapt to console width (be robust in test runners)
        int width;
        try
        {
            width = Console.WindowWidth;
            if (width <= 0) width = 80;
        }
        catch
        {
            width = 80;
        }
        var colWidth = Math.Min(Math.Max(keys.Max(k => k.Length) + 3, 16), 32);
        var cols = Math.Max(1, width / colWidth);

        for (int i = 0; i < keys.Count; i++)
        {
            Console.Write(keys[i].PadRight(colWidth));
            if ((i + 1) % cols == 0) Console.WriteLine();
        }
        if (keys.Count % cols != 0) Console.WriteLine();
        Console.WriteLine();
    }
}
