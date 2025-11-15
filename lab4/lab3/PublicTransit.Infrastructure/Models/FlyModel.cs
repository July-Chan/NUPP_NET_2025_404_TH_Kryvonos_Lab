using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublicTransit.Infrastructure.Models;

/// <summary>
/// Модель мухи (Table-per-Type - окрема таблиця для FlyModel)
/// </summary>
[Table("Flies")]
public class FlyModel : InsectModel
{
    [Required]
    [Range(0.1, 100.0)]
    [Column(TypeName = "decimal(5,2)")]
    public double WingSpan { get; set; }

    [Required]
    [Range(1, 1000)]
    public int FlightSpeed { get; set; } 

    [Required]
    public bool CanHover { get; set; } 
}

