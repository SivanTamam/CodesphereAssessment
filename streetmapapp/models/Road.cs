using System.ComponentModel.DataAnnotations;
namespace StreetMapApp.Models;

public enum RoadDirection
{
    OneWay = 0,
    Bidirectional = 1
}

public class Road
{
    [Required]
    public Intersection Destination { get; set; }
    public int Distance { get; set; } // Needed for bonus section
    public RoadDirection Direction { get; set; } = RoadDirection.Bidirectional;
}