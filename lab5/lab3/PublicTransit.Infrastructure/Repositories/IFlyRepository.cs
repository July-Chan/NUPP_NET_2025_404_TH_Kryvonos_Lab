using PublicTransit.Infrastructure.Models;

namespace PublicTransit.Infrastructure.Repositories;

/// <summary>
/// Інтерфейс репозиторію для роботи з мухами
/// </summary>
public interface IFlyRepository : IRepository<FlyModel>
{
    /// <summary>
    /// Отримати мух, які можуть зависати в повітрі
    /// </summary>
    Task<IEnumerable<FlyModel>> GetHoveringFliesAsync();

    /// <summary>
    /// Отримати мух за діапазоном швидкості польоту
    /// </summary>
    Task<IEnumerable<FlyModel>> GetByFlightSpeedRangeAsync(int minSpeed, int maxSpeed);

    /// <summary>
    /// Отримати муху з повною інформацією
    /// </summary>
    Task<FlyModel?> GetByIdWithDetailsAsync(Guid id);
}

