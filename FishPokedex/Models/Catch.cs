namespace FishPokedex.Models;

public class Catch
{
    public int Id { get; set; }
    public int LocationId { get; set; }
    public Location? Location { get; set; }
    public DateTime CatchDate { get; set; }
    public int SpeciesId { get; set; }
    public Species? Species { get; set; }
    public string? ImageUrl { get; set; }
    public CatchDetail? CatchDetail { get; set; }
}
