namespace FishPokedex.Models;

public class Catch
{
    public int Id { get; set; }
    public required string Location { get; set; }
    public DateTime CatchDate { get; set; }
    public int SpeciesId { get; set; }
    public Species? Species { get; set; }
    public CatchDetail? CatchDetail { get; set; }
}
