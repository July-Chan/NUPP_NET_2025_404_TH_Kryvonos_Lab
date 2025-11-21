using System.ComponentModel.DataAnnotations;

namespace PublicTransit.REST.Models;

/// <summary>
/// Модель для створення павука
/// </summary>
public class SpiderCreateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0, 100)]
    public int Legs { get; set; }

    [Required]
    public bool IsPoisonous { get; set; }

    [Required]
    [Range(0, 100)]
    public double WebStrength { get; set; }

    [Required]
    [Range(0, 10)]
    public int VenomPotency { get; set; }

    public Guid? FoodSourceId { get; set; }
}

