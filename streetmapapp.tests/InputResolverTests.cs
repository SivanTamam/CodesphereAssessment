using StreetMapApp.Models;
using StreetMapApp.Services;
using Xunit;

namespace StreetMapApp.Tests;

public class InputResolverTests
{
    [Theory]
    [InlineData("a", "A")]
    [InlineData("A", "A")]
    [InlineData("mainstreet", "MainStreet")]
    public void ResolveNode_Is_Case_Insensitive(string input, string expected)
    {
        // Arrange
        var map = new StreetMap
        {
            Intersections = new Dictionary<string, Intersection>
            {
                ["A"] = new Intersection { Name = "A" },
                ["MainStreet"] = new Intersection { Name = "MainStreet" }
            }
        };

        // Act
        var actual = InputResolver.ResolveNode(map, input);

        // Assert
        Assert.Equal(expected, actual);
    }
}
