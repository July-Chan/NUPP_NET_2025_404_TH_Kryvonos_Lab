using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublicTransit.Infrastructure.Models;

/// <summary>
/// Модель джерела їжі
/// ЗВ'ЯЗОК ОДИН-ДО-БАГАТЬОХ: Одне джерело їжі може годувати багато комах
/// </summary>
[Table("FoodSources")]
public class FoodSourceModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

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

    // Зворотній зв'язок один-до-багатьох: одна їжа для багатьох комах
    public ICollection<InsectModel> Insects { get; set; } = new List<InsectModel>();
}

