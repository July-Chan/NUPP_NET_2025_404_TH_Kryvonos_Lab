namespace PublicTransit.REST.Models;

/// <summary>
/// Модель для відповіді з інформацією про муху
/// </summary>
public class FlyResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Legs { get; set; }
    public DateTime CreatedAt { get; set; }
    public double WingSpan { get; set; }
    public int FlightSpeed { get; set; }
    public bool CanHover { get; set; }
    public Guid? FoodSourceId { get; set; }
    public string? FoodSourceName { get; set; }
    public Guid? HabitatId { get; set; }
    public string? HabitatLocation { get; set; }
}

