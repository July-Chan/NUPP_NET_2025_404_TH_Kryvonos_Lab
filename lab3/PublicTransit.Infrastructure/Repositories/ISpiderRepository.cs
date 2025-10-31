using PublicTransit.Infrastructure.Models;

namespace PublicTransit.Infrastructure.Repositories;

/// <summary>
/// Інтерфейс репозиторію для роботи з павуками
/// </summary>
public interface ISpiderRepository : IRepository<SpiderModel>
{
    /// <summary>
    /// Отримати отруйних павуків
    /// </summary>
    Task<IEnumerable<SpiderModel>> GetPoisonousSpidersAsync();

    /// <summary>
    /// Отримати павуків за мінімальною міцністю павутини
    /// </summary>
    Task<IEnumerable<SpiderModel>> GetByMinWebStrengthAsync(double minStrength);

    /// <summary>
    /// Отримати павука з повною інформацією
    /// </summary>
    Task<SpiderModel?> GetByIdWithDetailsAsync(Guid id);
}

