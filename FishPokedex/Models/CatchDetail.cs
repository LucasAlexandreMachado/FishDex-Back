namespace FishPokedex.Models;

public class CatchDetail
{
    public int Id { get; set; }
    public decimal? WeightKg { get; set; }
    public decimal? LengthCm { get; set; }
    public string? BaitUsed { get; set; }
    public string? WeatherCondition { get; set; }
    public int CatchId { get; set; }
    public Catch? Catch { get; set; }
}
