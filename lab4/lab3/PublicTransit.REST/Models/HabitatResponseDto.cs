namespace PublicTransit.REST.Models;

/// <summary>
/// Модель для відповіді з інформацією про середовище проживання
/// </summary>
public class HabitatResponseDto
{
    public Guid Id { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Climate { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public int Humidity { get; set; }
    public Guid? InsectId { get; set; }
    public string? InsectName { get; set; }
}

