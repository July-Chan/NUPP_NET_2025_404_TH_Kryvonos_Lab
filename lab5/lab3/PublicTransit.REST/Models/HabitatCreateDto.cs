using System.ComponentModel.DataAnnotations;

namespace PublicTransit.REST.Models;

/// <summary>
/// Модель для створення середовища проживання
/// </summary>
public class HabitatCreateDto
{
    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Climate { get; set; } = string.Empty;

    [Required]
    [Range(-50, 60)]
    public double Temperature { get; set; }

    [Required]
    [Range(0, 100)]
    public int Humidity { get; set; }

    public Guid? InsectId { get; set; }
}

