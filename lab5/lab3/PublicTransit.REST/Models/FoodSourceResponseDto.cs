namespace PublicTransit.REST.Models;

/// <summary>
/// Модель для відповіді з інформацією про джерело їжі
/// </summary>
public class FoodSourceResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int NutritionalValue { get; set; }
    public bool IsAvailableYearRound { get; set; }
}

