using System.ComponentModel.DataAnnotations;

namespace PublicTransit.REST.Models;

/// <summary>
/// Модель для створення хижака
/// </summary>
public class PredatorCreateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Species { get; set; } = string.Empty;

    [Required]
    [Range(0, 100)]
    public int HuntingSuccess { get; set; }

    [Required]
    [Range(0.1, 1000.0)]
    public double Size { get; set; }
}

