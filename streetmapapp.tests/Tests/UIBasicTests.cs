using StreetMapApp.Models;
using StreetMapApp.UI;
using Xunit;

namespace StreetMapApp.Tests;

public class UIBasicTests
{
    [Fact]
    public void PrintBanner_DoesNotThrow()
    {
        var ex = Record.Exception(() => ConsoleUi.PrintBanner());
        Assert.Null(ex);
    }

    [Fact]
    public void PrintHelp_DoesNotThrow()
    {
        var ex = Record.Exception(() => ConsoleUi.PrintHelp());
        Assert.Null(ex);
    }

    [Fact]
    public void PrintNodes_DoesNotThrow_With_SampleMap()
    {
        var map = new StreetMap
        {
            Intersections = new Dictionary<string, Intersection>
            {
                ["A"] = new Intersection { Name = "A" },
                ["B"] = new Intersection { Name = "B" }
            }
        };

        var ex = Record.Exception(() => ConsoleUi.PrintNodes(map));
        Assert.Null(ex);
    }
}
