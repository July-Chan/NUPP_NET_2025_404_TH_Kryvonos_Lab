using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublicTransit.Infrastructure.Models;

/// <summary>
/// Проміжна модель для зв'язку багато-до-багатьох між комахами та хижаками
/// ЗВ'ЯЗОК БАГАТО-ДО-БАГАТЬОХ: Містить додаткову інформацію про взаємодію
/// </summary>
[Table("InsectPredators")]
public class InsectPredatorModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    // Зовнішній ключ до комахи
    [Required]
    [ForeignKey(nameof(Insect))]
    public Guid InsectId { get; set; }
    public InsectModel Insect { get; set; } = null!;

    // Зовнішній ключ до хижака
    [Required]
    [ForeignKey(nameof(Predator))]
    public Guid PredatorId { get; set; }
    public PredatorModel Predator { get; set; } = null!;

    // Додаткові властивості зв'язку
    [Required]
    [Range(0, 10)]
    public int RiskLevel { get; set; }

    [Required]
    public DateTime FirstEncounterDate { get; set; }

    [Required]
    [MaxLength(200)]
    public string DefenseMechanism { get; set; } = string.Empty;
}

