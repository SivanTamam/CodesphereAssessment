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

    [Fact]
    public void ShortestPath_Returns_Empty_When_No_Path()
    {
        // Arrange: A and B are disconnected
        var a = new Intersection { Name = "A" };
        var b = new Intersection { Name = "B" };

        var map = new StreetMap
        {
            Intersections = new Dictionary<string, Intersection>
            {
                ["A"] = a,
                ["B"] = b
            }
        };

        // Act
        var path = BfsPathFinder.ShortestPath(map, "A", "B");

        // Assert
        Assert.Empty(path);
    }

    [Fact]
    public void ShortestPath_Start_Equals_End_Returns_Single_Node()
    {
        // Arrange
        var a = new Intersection { Name = "A" };
        var map = new StreetMap { Intersections = new Dictionary<string, Intersection> { ["A"] = a } };

        // Act
        var path = BfsPathFinder.ShortestPath(map, "A", "A");

        // Assert
        Assert.Equal(new[] { "A" }, path);
    }

    [Fact]
    public void ShortestPath_Unknown_Start_Throws()
    {
        // Arrange: start node missing
        var a = new Intersection { Name = "A" };
        var map = new StreetMap { Intersections = new Dictionary<string, Intersection> { ["A"] = a } };

        // Act + Assert
        Assert.Throws<KeyNotFoundException>(() => BfsPathFinder.ShortestPath(map, "X", "A"));
    }

    [Fact]
    public void ShortestPath_Unknown_End_Returns_Empty()
    {
        // Arrange: end node missing
        var a = new Intersection { Name = "A" };
        var map = new StreetMap { Intersections = new Dictionary<string, Intersection> { ["A"] = a } };

        // Act
        var path = BfsPathFinder.ShortestPath(map, "A", "Z");

        // Assert
        Assert.Empty(path);
    }

    [Fact]
    public void ShortestPath_Respects_OneWay_Roads_Direction()
    {
        var map = new StreetMap();
        map.Intersections["A"] = new Intersection { Name = "A" };
        map.Intersections["B"] = new Intersection { Name = "B" };
        map.Intersections["A"].Roads.Add(new Road { Destination = map.Intersections["B"], Direction = RoadDirection.OneWay });

        Assert.Equal(new[] { "A", "B" }, BfsPathFinder.ShortestPath(map, "A", "B"));
        Assert.Empty(BfsPathFinder.ShortestPath(map, "B", "A"));
    }

    [Fact]
    public void ShortestPath_Allows_Reverse_Traversal_For_Bidirectional_Road()
    {
        var map = new StreetMap();
        map.Intersections["A"] = new Intersection { Name = "A" };
        map.Intersections["B"] = new Intersection { Name = "B" };
        map.Intersections["A"].Roads.Add(new Road { Destination = map.Intersections["B"], Direction = RoadDirection.Bidirectional });

        Assert.Equal(new[] { "B", "A" }, BfsPathFinder.ShortestPath(map, "B", "A"));
    }
}
