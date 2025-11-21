using Microsoft.EntityFrameworkCore;
using PublicTransit.Infrastructure.Models;

namespace PublicTransit.Infrastructure.Repositories;

/// <summary>
/// Репозиторій для роботи з мухами
/// </summary>
public class FlyRepository : Repository<FlyModel>, IFlyRepository
{
    public FlyRepository(PublicTransitContext context) : base(context)
    {
    }

    /// <summary>
    /// Отримати мух, які можуть зависати в повітрі
    /// </summary>
    public async Task<IEnumerable<FlyModel>> GetHoveringFliesAsync()
    {
        return await _dbSet
            .Where(f => f.CanHover)
            .Include(f => f.Habitat)
            .Include(f => f.FoodSource)
            .ToListAsync();
    }

    /// <summary>
    /// Отримати мух за діапазоном швидкості польоту
    /// </summary>
    public async Task<IEnumerable<FlyModel>> GetByFlightSpeedRangeAsync(int minSpeed, int maxSpeed)
    {
        return await _dbSet
            .Where(f => f.FlightSpeed >= minSpeed && f.FlightSpeed <= maxSpeed)
            .ToListAsync();
    }

    /// <summary>
    /// Отримати муху з повною інформацією
    /// </summary>
    public async Task<FlyModel?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(f => f.Habitat)
            .Include(f => f.FoodSource)
            .Include(f => f.InsectPredators)
                .ThenInclude(ip => ip.Predator)
            .FirstOrDefaultAsync(f => f.Id == id);
    }
}

