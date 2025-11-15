using Microsoft.EntityFrameworkCore;
using PublicTransit.Infrastructure.Models;

namespace PublicTransit.Infrastructure.Repositories;

/// <summary>
/// Репозиторій для роботи з комахами
/// </summary>
public class InsectRepository : Repository<InsectModel>, IInsectRepository
{
    public InsectRepository(PublicTransitContext context) : base(context)
    {
    }

    /// <summary>
    /// Отримати комах за кількістю ніг
    /// </summary>
    public async Task<IEnumerable<InsectModel>> GetByLegsCountAsync(int legs)
    {
        return await _dbSet
            .Where(i => i.Legs == legs)
            .ToListAsync();
    }

    /// <summary>
    /// Отримати комах за типом їжі
    /// </summary>
    public async Task<IEnumerable<InsectModel>> GetByFoodSourceAsync(Guid foodSourceId)
    {
        return await _dbSet
            .Where(i => i.FoodSourceId == foodSourceId)
            .Include(i => i.FoodSource)
            .ToListAsync();
    }

    /// <summary>
    /// Отримати комаху з повною інформацією (включаючи зв'язані дані)
    /// </summary>
    public async Task<InsectModel?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(i => i.Habitat)
            .Include(i => i.FoodSource)
            .Include(i => i.InsectPredators)
                .ThenInclude(ip => ip.Predator)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    /// <summary>
    /// Отримати всіх комах з повною інформацією
    /// </summary>
    public async Task<IEnumerable<InsectModel>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(i => i.Habitat)
            .Include(i => i.FoodSource)
            .Include(i => i.InsectPredators)
                .ThenInclude(ip => ip.Predator)
            .ToListAsync();
    }
}

