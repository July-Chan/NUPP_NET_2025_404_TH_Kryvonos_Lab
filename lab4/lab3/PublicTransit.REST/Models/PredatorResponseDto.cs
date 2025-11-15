namespace PublicTransit.REST.Models;

/// <summary>
/// Модель для відповіді з інформацією про хижака
/// </summary>
public class PredatorResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public int HuntingSuccess { get; set; }
    public double Size { get; set; }
}

