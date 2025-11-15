using System.ComponentModel.DataAnnotations;

namespace PublicTransit.REST.Models;

/// <summary>
/// Модель для оновлення мухи
/// </summary>
public class FlyUpdateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0, 100)]
    public int Legs { get; set; }

    [Required]
    [Range(0.1, 100.0)]
    public double WingSpan { get; set; }

    [Required]
    [Range(1, 1000)]
    public int FlightSpeed { get; set; }

    [Required]
    public bool CanHover { get; set; }

    public Guid? FoodSourceId { get; set; }
}

