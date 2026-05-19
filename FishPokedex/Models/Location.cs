namespace FishPokedex.Models;

public class Location
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<Catch> Catches { get; set; } = new List<Catch>();
}
