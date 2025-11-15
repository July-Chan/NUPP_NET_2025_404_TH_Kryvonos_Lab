using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublicTransit.Infrastructure.Models;

/// <summary>
/// Модель павука (Table-per-Type - окрема таблиця для SpiderModel)
/// </summary>
[Table("Spiders")]
public class SpiderModel : InsectModel
{
    [Required]
    public bool IsPoisonous { get; set; }

    [Required]
    [Range(0, 100)]
    [Column(TypeName = "decimal(5,2)")]
    public double WebStrength { get; set; }

    [Required]
    [Range(0, 10)]
    public int VenomPotency { get; set; }
}

