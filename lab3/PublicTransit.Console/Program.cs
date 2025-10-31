using System;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using PublicTransit.Infrastructure;
using PublicTransit.Infrastructure.Models;
using PublicTransit.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace PublicTransit.ConsoleApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            using var context = new PublicTransitContext();
            using var unitOfWork = new UnitOfWork(context);

            // Перевірка підключення до БД
            Console.WriteLine("Перевірка підключення до бази даних...");
            var canConnect = await context.Database.CanConnectAsync();
            if (!canConnect)
            {
                Console.WriteLine("Не вдалося підключитися до бази даних!");
                return;
            }
            Console.WriteLine("Підключення до PostgreSQL успішне!\n");

            await ShowAllInsects(unitOfWork);
            await ShowAllFlies(unitOfWork);
            await ShowAllSpiders(unitOfWork);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nПомилка: {ex.Message}");
        }
    }

    static async Task ShowAllInsects(IUnitOfWork unitOfWork)
    {
        Console.WriteLine("\nВСІ КОМАХИ:\n");

        var insects = await unitOfWork.Insects.GetAllWithDetailsAsync();

        if (!insects.Any())
        {
            Console.WriteLine("Комах не знайдено. База даних порожня.");
            return;
        }

        foreach (var insect in insects)
        {
            Console.WriteLine($"{insect.Name} (ID: {insect.Id:N})");
            Console.WriteLine($"Тип: {insect.GetType().Name.Replace("Model", "")}");
            Console.WriteLine($"Ніг: {insect.Legs}");
            Console.WriteLine($"Створено: {insect.CreatedAt:g}");

            if (insect is FlyModel fly)
            {
                Console.WriteLine($"Розмах крил: {fly.WingSpan} см");
                Console.WriteLine($"Швидкість: {fly.FlightSpeed} км/год");
                Console.WriteLine($"Зависання: {(fly.CanHover ? "Так" : "Ні")}");
            }
            else if (insect is SpiderModel spider)
            {
                Console.WriteLine($"Отруйний: {(spider.IsPoisonous ? "Так" : "Ні")}");
                Console.WriteLine($"Міцність павутини: {spider.WebStrength}");
                Console.WriteLine($"Сила отрути: {spider.VenomPotency}/10");
            }

            if (insect.FoodSource != null)
            {
                Console.WriteLine($"Їжа: {insect.FoodSource.Name} ({insect.FoodSource.Type})");
            }

            if (insect.Habitat != null)
            {
                Console.WriteLine($"Місце: {insect.Habitat.Location}");
            }
        }

        Console.WriteLine($"\nВсього комах: {insects.Count()}");
    }

    static async Task ShowAllFlies(IUnitOfWork unitOfWork)
    {
        Console.WriteLine("\nВСІ МУХИ:\n");

        var flies = await unitOfWork.Flies.GetAllAsync();

        if (!flies.Any())
        {
            Console.WriteLine("Мух не знайдено.");
            return;
        }

        foreach (var fly in flies)
        {
            Console.WriteLine($"{fly.Name}");
            Console.WriteLine($"Розмах крил: {fly.WingSpan} см");
            Console.WriteLine($"Швидкість польоту: {fly.FlightSpeed} км/год");
            Console.WriteLine($"Може зависати: {(fly.CanHover ? "Так" : "Ні")}");
        }

        Console.WriteLine($"\nВсього мух: {flies.Count()}");

        // Статистика
        var avgWingSpan = flies.Average(f => f.WingSpan);
        var avgSpeed = flies.Average(f => f.FlightSpeed);
        var hoveringCount = flies.Count(f => f.CanHover);

        Console.WriteLine($"\nСТАТИСТИКА МУХ:");
        Console.WriteLine($"Середній розмах крил: {avgWingSpan:F2} см");
        Console.WriteLine($"Середня швидкість: {avgSpeed:F2} км/год");
        Console.WriteLine($"Можуть зависати: {hoveringCount}/{flies.Count()}");
    }

    static async Task ShowAllSpiders(IUnitOfWork unitOfWork)
    {
        Console.WriteLine("\nВСІ ПАВУКИ:\n");

        var spiders = await unitOfWork.Spiders.GetAllAsync();

        if (!spiders.Any())
        {
            Console.WriteLine("Павуків не знайдено.");
            return;
        }

        foreach (var spider in spiders)
        {
            Console.WriteLine($"{spider.Name}");
            Console.WriteLine($"Отруйний: {(spider.IsPoisonous ? "Так" : "Ні")}");
            Console.WriteLine($"Міцність павутини: {spider.WebStrength}/100");
            Console.WriteLine($"Сила отрути: {spider.VenomPotency}/10");
        }

        Console.WriteLine($"\nВсього павуків: {spiders.Count()}");

        // Статистика
        var poisonousCount = spiders.Count(s => s.IsPoisonous);
        var avgWebStrength = spiders.Average(s => s.WebStrength);

        Console.WriteLine($"\nСТАТИСТИКА ПАВУКІВ:");
        Console.WriteLine($"Отруйних: {poisonousCount}/{spiders.Count()}");
        Console.WriteLine($"Середня міцність павутини: {avgWebStrength:F2}");
    }
}
