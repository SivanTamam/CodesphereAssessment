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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ResolveNode_NullOrWhitespace_Returns_Null(string? input)
    {
        var map = new StreetMap { Intersections = new Dictionary<string, Intersection>() };
        var result = InputResolver.ResolveNode(map, input);
        Assert.Null(result);
    }

    [Fact]
    public void ResolveNode_Unknown_Returns_Null()
    {
        var map = new StreetMap
        {
            Intersections = new Dictionary<string, Intersection>
            {
                ["A"] = new Intersection { Name = "A" }
            }
        };
        var result = InputResolver.ResolveNode(map, "ZZZ");
        Assert.Null(result);
    }
}
