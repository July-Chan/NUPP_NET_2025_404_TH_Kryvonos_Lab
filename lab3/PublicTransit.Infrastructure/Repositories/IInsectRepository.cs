using PublicTransit.Infrastructure.Models;

namespace PublicTransit.Infrastructure.Repositories;

/// <summary>
/// Інтерфейс репозиторію для роботи з комахами
/// </summary>
public interface IInsectRepository : IRepository<InsectModel>
{
    /// <summary>
    /// Отримати комах за кількістю ніг
    /// </summary>
    Task<IEnumerable<InsectModel>> GetByLegsCountAsync(int legs);

    /// <summary>
    /// Отримати комах за типом їжі
    /// </summary>
    Task<IEnumerable<InsectModel>> GetByFoodSourceAsync(Guid foodSourceId);

    /// <summary>
    /// Отримати комаху з повною інформацією (включаючи зв'язані дані)
    /// </summary>
    Task<InsectModel?> GetByIdWithDetailsAsync(Guid id);

    /// <summary>
    /// Отримати всіх комах з повною інформацією
    /// </summary>
    Task<IEnumerable<InsectModel>> GetAllWithDetailsAsync();
}

