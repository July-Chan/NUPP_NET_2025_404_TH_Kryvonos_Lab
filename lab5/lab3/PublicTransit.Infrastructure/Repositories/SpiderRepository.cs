using Microsoft.EntityFrameworkCore;
using PublicTransit.Infrastructure.Models;

namespace PublicTransit.Infrastructure.Repositories;

/// <summary>
/// Репозиторій для роботи з павуками
/// </summary>
public class SpiderRepository : Repository<SpiderModel>, ISpiderRepository
{
    public SpiderRepository(PublicTransitContext context) : base(context)
    {
    }

    /// <summary>
    /// Отримати отруйних павуків
    /// </summary>
    public async Task<IEnumerable<SpiderModel>> GetPoisonousSpidersAsync()
    {
        return await _dbSet
            .Where(s => s.IsPoisonous)
            .Include(s => s.Habitat)
            .Include(s => s.FoodSource)
            .ToListAsync();
    }

    /// <summary>
    /// Отримати павуків за мінімальною міцністю павутини
    /// </summary>
    public async Task<IEnumerable<SpiderModel>> GetByMinWebStrengthAsync(double minStrength)
    {
        return await _dbSet
            .Where(s => s.WebStrength >= minStrength)
            .ToListAsync();
    }

    /// <summary>
    /// Отримати павука з повною інформацією
    /// </summary>
    public async Task<SpiderModel?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(s => s.Habitat)
            .Include(s => s.FoodSource)
            .Include(s => s.InsectPredators)
                .ThenInclude(ip => ip.Predator)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
}

