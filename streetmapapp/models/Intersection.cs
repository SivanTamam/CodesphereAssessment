using System.ComponentModel.DataAnnotations;
namespace StreetMapApp.Models;

public class Intersection
{
    [Required]
    public string Name { get; set; }

    public List<Road> Roads { get; set; } = new();
}