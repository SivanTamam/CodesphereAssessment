using System.Text.Json;
using StreetMapApp.Models;
using StreetMapApp.Services;
using Xunit;

namespace StreetMapApp.Tests;

public class GraphLoaderTests
{
    [Fact]
    public void Load_Builds_Intersections_And_Roads_From_Json()
    {
        // Arrange - create a temp JSON file
        var json = @"{
  ""intersections"": {
    ""A"": { ""name"": ""A"", ""roads"": [ { ""destination"": ""B"", ""distance"": 1 } ] },
    ""B"": { ""name"": ""B"", ""roads"": [] }
  }
}";
        var temp = Path.GetTempFileName();
        File.WriteAllText(temp, json);

        try
        {
            // Act
            var map = GraphLoader.Load(temp);

            // Assert
            Assert.True(map.Intersections.ContainsKey("A"));
            Assert.True(map.Intersections.ContainsKey("B"));
            Assert.Single(map.Intersections["A"].Roads);
            Assert.Equal("B", map.Intersections["A"].Roads[0].Destination.Name);
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [Fact]
    public void Load_Treats_NonInteger_Distance_As_Zero()
    {
        var json = @"{
  ""intersections"": {
    ""A"": { ""name"": ""A"", ""roads"": [ { ""destination"": ""B"", ""distance"": ""far"" } ] },
    ""B"": { ""name"": ""B"", ""roads"": [] }
  }
}";
        var temp = Path.GetTempFileName();
        File.WriteAllText(temp, json);

        try
        {
            var map = GraphLoader.Load(temp);
            Assert.Equal(0, map.Intersections["A"].Roads.Single().Distance);
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [Fact]
    public void Load_Skips_Road_Without_Destination()
    {
        var json = @"{
  ""intersections"": {
    ""A"": { ""name"": ""A"", ""roads"": [ { ""distance"": 1 } ] }
  }
}";
        var temp = Path.GetTempFileName();
        File.WriteAllText(temp, json);

        try
        {
            var map = GraphLoader.Load(temp);
            Assert.Empty(map.Intersections["A"].Roads);
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [Fact]
    public void Load_Ignores_Roads_When_Not_Array()
    {
        var json = @"{
  ""intersections"": {
    ""A"": { ""name"": ""A"", ""roads"": { ""x"": 1 } }
  }
}";
        var temp = Path.GetTempFileName();
        File.WriteAllText(temp, json);

        try
        {
            var map = GraphLoader.Load(temp);
            Assert.Empty(map.Intersections["A"].Roads);
        }
        finally
        {
            File.Delete(temp);
        }
    }


    [Fact]
    public void Load_Throws_On_Malformed_Json()
    {
        var badJson = "{ intersections: { ";
        var temp = Path.GetTempFileName();
        File.WriteAllText(temp, badJson);

        try
        {
            Assert.ThrowsAny<JsonException>(() => GraphLoader.Load(temp));
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [Fact]
    public void Load_Throws_When_Intersections_Missing()
    {
        var json = "{ \"nodes\": {} }";
        var temp = Path.GetTempFileName();
        File.WriteAllText(temp, json);

        try
        {
            Assert.Throws<InvalidOperationException>(() => GraphLoader.Load(temp));
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [Fact]
    public void Load_Adds_Destination_That_Was_Not_Declared()
    {
        // Destination C isn't declared but is referenced by A -> C
        var json = @"{
  ""intersections"": {
    ""A"": { ""name"": ""A"", ""roads"": [ { ""destination"": ""C"", ""distance"": 1 } ] }
  }
}";
        var temp = Path.GetTempFileName();
        File.WriteAllText(temp, json);

        try
        {
            var map = GraphLoader.Load(temp);
            Assert.True(map.Intersections.ContainsKey("A"));
            Assert.True(map.Intersections.ContainsKey("C")); // created during load
            Assert.Equal("C", map.Intersections["A"].Roads.Single().Destination.Name);
        }
        finally
        {
            File.Delete(temp);
        }
    }
}
