using System.ComponentModel.DataAnnotations;

namespace PublicTransit.REST.Models;

/// <summary>
/// Модель для створення джерела їжі
/// </summary>
public class FoodSourceCreateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;

    [Required]
    [Range(0, 100)]
    public int NutritionalValue { get; set; }

    [Required]
    public bool IsAvailableYearRound { get; set; }
}

