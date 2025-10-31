using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublicTransit.Infrastructure.Models;

/// <summary>
/// Модель середовища проживання комахи
/// ЗВ'ЯЗОК ОДИН-ДО-ОДНОГО: Одна комаха - одне унікальне середовище проживання
/// </summary>
[Table("Habitats")]
public class HabitatModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Climate { get; set; } = string.Empty;

    [Required]
    [Range(-50, 60)]
    [Column(TypeName = "decimal(5,2)")]
    public double Temperature { get; set; }

    [Required]
    [Range(0, 100)]
    public int Humidity { get; set; }

    // Зворотній зв'язок один-до-одного
    [ForeignKey(nameof(Insect))]
    public Guid? InsectId { get; set; }
    public InsectModel? Insect { get; set; }
}

