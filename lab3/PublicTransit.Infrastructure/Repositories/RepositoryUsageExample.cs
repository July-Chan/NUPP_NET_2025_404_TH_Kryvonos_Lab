using PublicTransit.Infrastructure.Models;

namespace PublicTransit.Infrastructure.Repositories;

/// <summary>
/// Приклади використання репозиторіїв
/// </summary>
public class RepositoryUsageExample
{
    /// <summary>
    /// Приклад роботи з репозиторіями через Unit of Work
    /// </summary>
    public static async Task ExampleWithUnitOfWork()
    {
        using var context = new PublicTransitContext();
        using var unitOfWork = new UnitOfWork(context);

        var allFlies = await unitOfWork.Flies.GetAllAsync();
        Console.WriteLine($"Всього мух: {allFlies.Count()}");

        var hoveringFlies = await unitOfWork.Flies.GetHoveringFliesAsync();
        foreach (var fly in hoveringFlies)
        {
            Console.WriteLine($"Муха '{fly.Name}' може зависати в повітрі");
        }

        var poisonousSpiders = await unitOfWork.Spiders.GetPoisonousSpidersAsync();
        Console.WriteLine($"Знайдено отруйних павуків: {poisonousSpiders.Count()}");

        var newFly = new FlyModel
        {
            Name = "Нова муха",
            Legs = 6,
            WingSpan = 4.0,
            FlightSpeed = 12,
            CanHover = true
        };

        await unitOfWork.Flies.AddAsync(newFly);
        await unitOfWork.SaveChangesAsync();
        Console.WriteLine($"Додано нову муху з ID: {newFly.Id}");

        var flyToUpdate = await unitOfWork.Flies.GetByIdAsync(newFly.Id);
        if (flyToUpdate != null)
        {
            flyToUpdate.FlightSpeed = 15;
            await unitOfWork.Flies.UpdateAsync(flyToUpdate);
            await unitOfWork.SaveChangesAsync();
            Console.WriteLine("Муху оновлено");
        }

        var flyToDelete = await unitOfWork.Flies.GetByIdAsync(newFly.Id);
        if (flyToDelete != null)
        {
            await unitOfWork.Flies.DeleteAsync(flyToDelete);
            await unitOfWork.SaveChangesAsync();
            Console.WriteLine("Муху видалено");
        }
    }

    /// <summary>
    /// Приклад роботи з транзакціями
    /// </summary>
    public static async Task ExampleWithTransaction()
    {
        using var context = new PublicTransitContext();
        using var unitOfWork = new UnitOfWork(context);

        try
        {
            await unitOfWork.BeginTransactionAsync();

            var fly = new FlyModel
            {
                Name = "Транзакційна муха",
                Legs = 6,
                WingSpan = 3.0,
                FlightSpeed = 10,
                CanHover = false
            };
            await unitOfWork.Flies.AddAsync(fly);

            var spider = new SpiderModel
            {
                Name = "Транзакційний павук",
                Legs = 8,
                IsPoisonous = false,
                WebStrength = 50.0,
                VenomPotency = 1
            };
            await unitOfWork.Spiders.AddAsync(spider);

            await unitOfWork.CommitTransactionAsync();
            Console.WriteLine("Транзакція успішно виконана");
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync();
            Console.WriteLine($"Помилка: {ex.Message}. Транзакцію відкочено.");
        }
    }

    /// <summary>
    /// Приклад роботи з включенням зв'язаних даних
    /// </summary>
    public static async Task ExampleWithRelatedData()
    {
        using var context = new PublicTransitContext();
        using var unitOfWork = new UnitOfWork(context);

        var insectsWithDetails = await unitOfWork.Insects.GetAllWithDetailsAsync();

        foreach (var insect in insectsWithDetails)
        {
            Console.WriteLine($"\n=== {insect.Name} ===");
            Console.WriteLine($"Ніг: {insect.Legs}");

            if (insect.Habitat != null)
            {
                Console.WriteLine($"Середовище: {insect.Habitat.Location}");
                Console.WriteLine($"Температура: {insect.Habitat.Temperature}°C");
            }

            if (insect.FoodSource != null)
            {
                Console.WriteLine($"Їжа: {insect.FoodSource.Name} ({insect.FoodSource.Type})");
            }

            if (insect.InsectPredators.Any())
            {
                Console.WriteLine("Хижаки:");
                foreach (var ip in insect.InsectPredators)
                {
                    Console.WriteLine($"  - {ip.Predator.Name} (загроза: {ip.RiskLevel}/10)");
                    Console.WriteLine($"    Захист: {ip.DefenseMechanism}");
                }
            }
        }
    }

    /// <summary>
    /// Приклад фільтрації даних
    /// </summary>
    public static async Task ExampleWithFiltering()
    {
        using var context = new PublicTransitContext();
        using var unitOfWork = new UnitOfWork(context);

        var sixLeggedInsects = await unitOfWork.Insects.GetByLegsCountAsync(6);
        Console.WriteLine($"Комах з 6 ногами: {sixLeggedInsects.Count()}");

        var mediumSpeedFlies = await unitOfWork.Flies.GetByFlightSpeedRangeAsync(5, 10);
        Console.WriteLine($"Мух зі швидкістю 5-10 км/год: {mediumSpeedFlies.Count()}");

        var strongWebSpiders = await unitOfWork.Spiders.GetByMinWebStrengthAsync(70.0);
        Console.WriteLine($"Павуків з міцною павутиною: {strongWebSpiders.Count()}");
    }
}

