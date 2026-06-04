using System.Collections.Generic;
using StreetMapApp.Models;
using StreetMapApp.Services;
using Xunit;

namespace StreetMapApp.Tests;

public class DijkstraPathFinderTests
{
    private static StreetMap BuildMap(System.Action<StreetMap>? customize = null)
    {
        var map = new StreetMap();
        // Create intersections
        foreach (var name in new[] { "A", "B", "C", "D", "E" })
        {
            map.Intersections[name] = new Intersection { Name = name };
        }

        // Helper to add directed or bidirectional edges
        void Add(string from, string to, int dist, bool bidi = true)
        {
            var a = map.Intersections[from];
            var b = map.Intersections[to];
            a.Roads.Add(new Road { Destination = b, Distance = dist, Direction = bidi ? RoadDirection.Bidirectional : RoadDirection.OneWay });
            if (bidi)
            {
                b.Roads.Add(new Road { Destination = a, Distance = dist, Direction = RoadDirection.Bidirectional });
            }
        }

        // Graph:
        // A -1- B -10- C
        //  \5  |      
        //    E -1- D -1- C
        Add("A", "B", 1, bidi: true);
        Add("B", "C", 10, bidi: true);
        Add("A", "E", 5, bidi: true);
        Add("E", "D", 1, bidi: true);
        Add("D", "C", 1, bidi: true);

        customize?.Invoke(map);
        return map;
    }

    [Fact]
    public void ShortestPath_Weighted_Prefers_Lower_Total_Distance_Not_Fewer_Hops()
    {
        var map = BuildMap();
        var (path, distance) = DijkstraPathFinder.ShortestPath(map, "A", "C");
        Assert.Equal(new List<string> { "A", "E", "D", "C" }, path); // 5 + 1 + 1 = 7 < 1+10=11
        Assert.Equal(7, distance);
    }

    [Fact]
    public void ShortestPath_Returns_Empty_When_Unreachable()
    {
        var map = BuildMap(m =>
        {
            // Remove all connections to C so it is unreachable from A
            m.Intersections["B"].Roads.RemoveAll(r => r.Destination.Name == "C");
            m.Intersections["D"].Roads.RemoveAll(r => r.Destination.Name == "C");
            // Remove reverse edges from C as well (bidirectional edges that were added)
            m.Intersections["C"].Roads.RemoveAll(r => r.Destination.Name == "B" || r.Destination.Name == "D");
        });
        var (path, distance) = DijkstraPathFinder.ShortestPath(map, "A", "C");
        Assert.Empty(path);
        Assert.Equal(0, distance);
    }

    [Fact]
    public void ShortestPath_Start_Equals_End_Returns_Zero_Distance()
    {
        var map = BuildMap();
        var (path, distance) = DijkstraPathFinder.ShortestPath(map, "A", "A");
        Assert.Equal(new List<string> { "A" }, path);
        Assert.Equal(0, distance);
    }

    [Fact]
    public void ShortestPath_Respects_OneWay_Roads_Direction()
    {
        var map = new StreetMap();
        map.Intersections["A"] = new Intersection { Name = "A" };
        map.Intersections["B"] = new Intersection { Name = "B" };
        // One-way A->B distance 1, no reverse edge
        map.Intersections["A"].Roads.Add(new Road { Destination = map.Intersections["B"], Distance = 1, Direction = RoadDirection.OneWay });

        var (pathAB, distAB) = DijkstraPathFinder.ShortestPath(map, "A", "B");
        Assert.Equal(new List<string> { "A", "B" }, pathAB);
        Assert.Equal(1, distAB);

        var (pathBA, distBA) = DijkstraPathFinder.ShortestPath(map, "B", "A");
        Assert.Empty(pathBA); // cannot traverse backward on one-way
        Assert.Equal(0, distBA);
    }

    [Fact]
    public void ShortestPath_Allows_Reverse_Traversal_For_Bidirectional_Road()
    {
        var map = new StreetMap();
        map.Intersections["A"] = new Intersection { Name = "A" };
        map.Intersections["B"] = new Intersection { Name = "B" };
        map.Intersections["A"].Roads.Add(new Road { Destination = map.Intersections["B"], Distance = 3, Direction = RoadDirection.Bidirectional });

        var (path, distance) = DijkstraPathFinder.ShortestPath(map, "B", "A");

        Assert.Equal(new List<string> { "B", "A" }, path);
        Assert.Equal(3, distance);
    }

    [Fact]
    public void ShortestPath_Tie_On_Distance_Returns_A_Valid_Min_Path()
    {
        var map = new StreetMap();
        foreach (var n in new[] { "A", "B", "C", "D" })
        {
            map.Intersections[n] = new Intersection { Name = n };
        }
        // Two equal-cost paths A->B->D (1+2=3) and A->C->D (2+1=3)
        map.Intersections["A"].Roads.Add(new Road { Destination = map.Intersections["B"], Distance = 1, Direction = RoadDirection.Bidirectional });
        map.Intersections["B"].Roads.Add(new Road { Destination = map.Intersections["D"], Distance = 2, Direction = RoadDirection.Bidirectional });
        map.Intersections["A"].Roads.Add(new Road { Destination = map.Intersections["C"], Distance = 2, Direction = RoadDirection.Bidirectional });
        map.Intersections["C"].Roads.Add(new Road { Destination = map.Intersections["D"], Distance = 1, Direction = RoadDirection.Bidirectional });
        // Add reverse for bidirectional
        map.Intersections["B"].Roads.Add(new Road { Destination = map.Intersections["A"], Distance = 1, Direction = RoadDirection.Bidirectional });
        map.Intersections["D"].Roads.Add(new Road { Destination = map.Intersections["B"], Distance = 2, Direction = RoadDirection.Bidirectional });
        map.Intersections["C"].Roads.Add(new Road { Destination = map.Intersections["A"], Distance = 2, Direction = RoadDirection.Bidirectional });
        map.Intersections["D"].Roads.Add(new Road { Destination = map.Intersections["C"], Distance = 1, Direction = RoadDirection.Bidirectional });

        var (path, distance) = DijkstraPathFinder.ShortestPath(map, "A", "D");
        Assert.Equal(3, distance);
        Assert.True(
            path.Count == 3 &&
            ((path[0] == "A" && path[1] == "B" && path[2] == "D") ||
             (path[0] == "A" && path[1] == "C" && path[2] == "D"))
        );
    }
}
