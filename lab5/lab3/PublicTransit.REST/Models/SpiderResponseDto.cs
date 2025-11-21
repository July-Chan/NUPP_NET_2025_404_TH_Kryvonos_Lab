namespace PublicTransit.REST.Models;

/// <summary>
/// Модель для відповіді з інформацією про павука
/// </summary>
public class SpiderResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Legs { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsPoisonous { get; set; }
    public double WebStrength { get; set; }
    public int VenomPotency { get; set; }
    public Guid? FoodSourceId { get; set; }
    public string? FoodSourceName { get; set; }
    public Guid? HabitatId { get; set; }
    public string? HabitatLocation { get; set; }
}

