using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublicTransit.Infrastructure.Models;

/// <summary>
/// Базова модель для комах (Table-per-Type підхід)
/// </summary>
[Table("Insects")]
public abstract class InsectModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0, 100)]
    public int Legs { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // === ЗВ'ЯЗОК ОДИН-ДО-ОДНОГО ===
    // Кожна комаха має одне унікальне середовище проживання
    [ForeignKey(nameof(Habitat))]
    public Guid? HabitatId { get; set; }
    public HabitatModel? Habitat { get; set; }

    // === ЗВ'ЯЗОК ОДИН-ДО-БАГАТЬОХ ===
    // Кожна комаха харчується одним типом їжі, але одна їжа може бути для багатьох комах
    [ForeignKey(nameof(FoodSource))]
    public Guid? FoodSourceId { get; set; }
    public FoodSourceModel? FoodSource { get; set; }

    // === ЗВ'ЯЗОК БАГАТО-ДО-БАГАТЬОХ ===
    // Комаха може мати багато хижаків, і один хижак може полювати на багато комах
    public ICollection<InsectPredatorModel> InsectPredators { get; set; } = new List<InsectPredatorModel>();
}

