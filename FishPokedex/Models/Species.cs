namespace FishPokedex.Models;

public class Species
{
    public int Id { get; set; }
    public required string CommonName { get; set; }
    public string? ScientificName { get; set; }
    public ICollection<Catch> Catches { get; set; } = new List<Catch>();
}
