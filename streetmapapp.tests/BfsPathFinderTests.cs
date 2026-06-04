using StreetMapApp.Models;
using StreetMapApp.Services;
using Xunit;

namespace StreetMapApp.Tests;

public class BfsPathFinderTests
{
    [Fact]
    public void ShortestPath_Finds_Path_In_Unweighted_Graph()
    {
        // Arrange: A -> B -> C, A -> D -> C (both length 2)
        var a = new Intersection { Name = "A" };
        var b = new Intersection { Name = "B" };
        var c = new Intersection { Name = "C" };
        var d = new Intersection { Name = "D" };

        a.Roads.Add(new Road { Destination = b });
        b.Roads.Add(new Road { Destination = c });
        a.Roads.Add(new Road { Destination = d });
        d.Roads.Add(new Road { Destination = c });

        var map = new StreetMap
        {
            Intersections = new Dictionary<string, Intersection>
            {
                ["A"] = a, ["B"] = b, ["C"] = c, ["D"] = d
            }
        };

        // Act
        var path = BfsPathFinder.ShortestPath(map, "A", "C");

        // Assert
        Assert.NotEmpty(path);
        Assert.Equal("A", path.First());
        Assert.Equal("C", path.Last());
        Assert.Equal(3, path.Count); // 2 hops
    }
}
