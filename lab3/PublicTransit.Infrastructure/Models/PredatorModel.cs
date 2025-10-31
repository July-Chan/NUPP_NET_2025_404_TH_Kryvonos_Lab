using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublicTransit.Infrastructure.Models;

/// <summary>
/// Модель хижака
/// ЗВ'ЯЗОК БАГАТО-ДО-БАГАТЬОХ: Хижак може полювати на багато комах, 
/// і на одну комаху можуть полювати багато хижаків
/// </summary>
[Table("Predators")]
public class PredatorModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

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
    [Column(TypeName = "decimal(7,2)")]
    public double Size { get; set; }

    // Зв'язок багато-до-багатьох через проміжну таблицю
    public ICollection<InsectPredatorModel> InsectPredators { get; set; } = new List<InsectPredatorModel>();
}

