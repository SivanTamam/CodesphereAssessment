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
  \"intersections\": {
    \"A\": { \"name\": \"A\", \"roads\": [ { \"destination\": \"B\", \"distance\": 1 } ] },
    \"B\": { \"name\": \"B\", \"roads\": [] }
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
}
