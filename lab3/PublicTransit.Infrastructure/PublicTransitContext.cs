using Microsoft.EntityFrameworkCore;
using PublicTransit.Infrastructure.Models;

namespace PublicTransit.Infrastructure;

/// <summary>
/// Контекст бази даних для системи PublicTransit
/// Використовує Entity Framework Core та підхід Table-per-Type (TPT) для ієрархії комах
/// </summary>
public class PublicTransitContext : DbContext
{
    /// <summary>
    /// Конструктор за замовчуванням
    /// </summary>
    public PublicTransitContext()
    {
    }

    /// <summary>
    /// Конструктор з параметрами
    /// </summary>
    /// <param name="options">Опції конфігурації DbContext</param>
    public PublicTransitContext(DbContextOptions<PublicTransitContext> options)
        : base(options)
    {
    }

    // === DbSet для всіх моделей ===

    /// <summary>
    /// Набір комах (базова таблиця для ієрархії)
    /// </summary>
    public DbSet<InsectModel> Insects { get; set; } = null!;

    /// <summary>
    /// Набір мух (Table-per-Type)
    /// </summary>
    public DbSet<FlyModel> Flies { get; set; } = null!;

    /// <summary>
    /// Набір павуків (Table-per-Type)
    /// </summary>
    public DbSet<SpiderModel> Spiders { get; set; } = null!;

    /// <summary>
    /// Набір середовищ проживання (зв'язок 1:1 з InsectModel)
    /// </summary>
    public DbSet<HabitatModel> Habitats { get; set; } = null!;

    /// <summary>
    /// Набір джерел їжі (зв'язок 1:N з InsectModel)
    /// </summary>
    public DbSet<FoodSourceModel> FoodSources { get; set; } = null!;

    /// <summary>
    /// Набір хижаків (зв'язок N:M з InsectModel)
    /// </summary>
    public DbSet<PredatorModel> Predators { get; set; } = null!;

    /// <summary>
    /// Набір зв'язків комаха-хижак (проміжна таблиця для N:M)
    /// </summary>
    public DbSet<InsectPredatorModel> InsectPredators { get; set; } = null!;

    /// <summary>
    /// Налаштування підключення до бази даних
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Конфігурація за замовчуванням для локальної розробки PostgreSQL
            // У production середовищі рядок підключення має передаватися через конструктор
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=PublicTransitDb;Username=postgres;Password=somepass1223",
                options => options.EnableRetryOnFailure());

            // Включити детальні помилки (тільки для розробки!)
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
            
            // Ігнорувати попередження про pending model changes
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }
    }

    /// <summary>
    /// Налаштування моделі бази даних за допомогою Fluent API
    /// Доповнює конфігурацію, зроблену через анотації
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ═══════════════════════════════════════════════════════════════
        // КОНФІГУРАЦІЯ TABLE-PER-TYPE (TPT) ДЛЯ ІЄРАРХІЇ КОМАХ
        // ═══════════════════════════════════════════════════════════════

        // Базова таблиця для InsectModel
        modelBuilder.Entity<InsectModel>()
            .ToTable("Insects")
            .HasKey(i => i.Id);

        // Окрема таблиця для FlyModel з власним первинним ключем
        modelBuilder.Entity<FlyModel>()
            .ToTable("Flies");

        // Окрема таблиця для SpiderModel з власним первинним ключем
        modelBuilder.Entity<SpiderModel>()
            .ToTable("Spiders");

        // ═══════════════════════════════════════════════════════════════
        // ЗВ'ЯЗОК ОДИН-ДО-ОДНОГО: InsectModel <-> HabitatModel
        // ═══════════════════════════════════════════════════════════════

        modelBuilder.Entity<InsectModel>()
            .HasOne(i => i.Habitat)
            .WithOne(h => h.Insect)
            .HasForeignKey<HabitatModel>(h => h.InsectId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Унікальний індекс для InsectId в Habitats (один-до-одного)
        modelBuilder.Entity<HabitatModel>()
            .HasIndex(h => h.InsectId)
            .IsUnique();

        // ═══════════════════════════════════════════════════════════════
        // ЗВ'ЯЗОК ОДИН-ДО-БАГАТЬОХ: FoodSourceModel -> InsectModel
        // ═══════════════════════════════════════════════════════════════

        modelBuilder.Entity<InsectModel>()
            .HasOne(i => i.FoodSource)
            .WithMany(f => f.Insects)
            .HasForeignKey(i => i.FoodSourceId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // ═══════════════════════════════════════════════════════════════
        // ЗВ'ЯЗОК БАГАТО-ДО-БАГАТЬОХ: InsectModel <-> PredatorModel
        // Через проміжну таблицю InsectPredatorModel
        // ═══════════════════════════════════════════════════════════════

        // Налаштування зв'язку Insect -> InsectPredator
        modelBuilder.Entity<InsectPredatorModel>()
            .HasOne(ip => ip.Insect)
            .WithMany(i => i.InsectPredators)
            .HasForeignKey(ip => ip.InsectId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Налаштування зв'язку Predator -> InsectPredator
        modelBuilder.Entity<InsectPredatorModel>()
            .HasOne(ip => ip.Predator)
            .WithMany(p => p.InsectPredators)
            .HasForeignKey(ip => ip.PredatorId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Індекс для швидкого пошуку зв'язків
        modelBuilder.Entity<InsectPredatorModel>()
            .HasIndex(ip => new { ip.InsectId, ip.PredatorId })
            .IsUnique();

        // ═══════════════════════════════════════════════════════════════
        // ДОДАТКОВІ НАЛАШТУВАННЯ ТА ІНДЕКСИ
        // ═══════════════════════════════════════════════════════════════

        // Індекс для швидкого пошуку комах за назвою
        modelBuilder.Entity<InsectModel>()
            .HasIndex(i => i.Name);

        // Індекс для фільтрації за датою створення
        modelBuilder.Entity<InsectModel>()
            .HasIndex(i => i.CreatedAt);

        // Індекс для пошуку джерел їжі за типом
        modelBuilder.Entity<FoodSourceModel>()
            .HasIndex(f => f.Type);

        // Індекс для пошуку хижаків за видом
        modelBuilder.Entity<PredatorModel>()
            .HasIndex(p => p.Species);

        // ═══════════════════════════════════════════════════════════════
        // ПОЧАТКОВІ ДАНІ (SEED DATA) - для демонстрації
        // ═══════════════════════════════════════════════════════════════

        SeedData(modelBuilder);
    }

    /// <summary>
    /// Додає початкові дані до бази даних
    /// </summary>
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Джерела їжі
        var nectarFood = new FoodSourceModel
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Нектар квітів",
            Type = "Нектар",
            NutritionalValue = 85,
            IsAvailableYearRound = false
        };

        var organicFood = new FoodSourceModel
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Органічні відходи",
            Type = "Органіка",
            NutritionalValue = 45,
            IsAvailableYearRound = true
        };

        var bloodFood = new FoodSourceModel
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Name = "Кров тварин",
            Type = "Кров",
            NutritionalValue = 95,
            IsAvailableYearRound = true
        };

        modelBuilder.Entity<FoodSourceModel>().HasData(nectarFood, organicFood, bloodFood);

        // Мухи
        var fly1 = new FlyModel
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Name = "Муха звичайна",
            Legs = 6,
            CreatedAt = DateTime.UtcNow,
            WingSpan = 3.5,
            FlightSpeed = 8,
            CanHover = true,
            FoodSourceId = organicFood.Id
        };

        var fly2 = new FlyModel
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Name = "Дрозофіла",
            Legs = 6,
            CreatedAt = DateTime.UtcNow,
            WingSpan = 1.2,
            FlightSpeed = 5,
            CanHover = false,
            FoodSourceId = nectarFood.Id
        };

        modelBuilder.Entity<FlyModel>().HasData(fly1, fly2);

        // Павуки
        var spider1 = new SpiderModel
        {
            Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            Name = "Павук-хрестовик",
            Legs = 8,
            CreatedAt = DateTime.UtcNow,
            IsPoisonous = false,
            WebStrength = 75.5,
            VenomPotency = 2
        };

        var spider2 = new SpiderModel
        {
            Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
            Name = "Чорна вдова",
            Legs = 8,
            CreatedAt = DateTime.UtcNow,
            IsPoisonous = true,
            WebStrength = 90.0,
            VenomPotency = 9
        };

        modelBuilder.Entity<SpiderModel>().HasData(spider1, spider2);

        // Середовища проживання
        var habitat1 = new HabitatModel
        {
            Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
            Location = "Київ, Голосіївський парк",
            Climate = "Помірно-континентальний",
            Temperature = 18.5,
            Humidity = 65,
            InsectId = fly1.Id
        };

        var habitat2 = new HabitatModel
        {
            Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
            Location = "Львів, ботанічний сад",
            Climate = "Помірний",
            Temperature = 16.0,
            Humidity = 70,
            InsectId = spider1.Id
        };

        modelBuilder.Entity<HabitatModel>().HasData(habitat1, habitat2);

        // Хижаки
        var predator1 = new PredatorModel
        {
            Id = Guid.Parse("11111111-aaaa-aaaa-aaaa-111111111111"),
            Name = "Горобець",
            Species = "Птах",
            HuntingSuccess = 65,
            Size = 14.5
        };

        var predator2 = new PredatorModel
        {
            Id = Guid.Parse("22222222-bbbb-bbbb-bbbb-222222222222"),
            Name = "Богомол",
            Species = "Комаха",
            HuntingSuccess = 85,
            Size = 8.0
        };

        var predator3 = new PredatorModel
        {
            Id = Guid.Parse("33333333-cccc-cccc-cccc-333333333333"),
            Name = "Ящірка звичайна",
            Species = "Рептилія",
            HuntingSuccess = 70,
            Size = 18.0
        };

        modelBuilder.Entity<PredatorModel>().HasData(predator1, predator2, predator3);

        // Зв'язки комаха-хижак
        modelBuilder.Entity<InsectPredatorModel>().HasData(
            new InsectPredatorModel
            {
                Id = Guid.Parse("aaaabbbb-cccc-dddd-eeee-111122223333"),
                InsectId = fly1.Id,
                PredatorId = predator1.Id,
                RiskLevel = 7,
                FirstEncounterDate = DateTime.UtcNow.AddDays(-30),
                DefenseMechanism = "Швидкий політ"
            },
            new InsectPredatorModel
            {
                Id = Guid.Parse("bbbbcccc-dddd-eeee-ffff-222233334444"),
                InsectId = fly1.Id,
                PredatorId = predator3.Id,
                RiskLevel = 6,
                FirstEncounterDate = DateTime.UtcNow.AddDays(-45),
                DefenseMechanism = "Маневрування"
            },
            new InsectPredatorModel
            {
                Id = Guid.Parse("ccccdddd-eeee-ffff-aaaa-333344445555"),
                InsectId = spider1.Id,
                PredatorId = predator1.Id,
                RiskLevel = 8,
                FirstEncounterDate = DateTime.UtcNow.AddDays(-60),
                DefenseMechanism = "Павутина"
            },
            new InsectPredatorModel
            {
                Id = Guid.Parse("ddddeeee-ffff-aaaa-bbbb-444455556666"),
                InsectId = spider2.Id,
                PredatorId = predator3.Id,
                RiskLevel = 5,
                FirstEncounterDate = DateTime.UtcNow.AddDays(-20),
                DefenseMechanism = "Отрута"
            }
        );
    }

    /// <summary>
    /// Перевизначення SaveChanges для автоматичного встановлення CreatedAt
    /// </summary>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Асинхронна версія SaveChanges
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Оновлює мітки часу для нових сутностей
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is InsectModel && e.State == EntityState.Added);

        foreach (var entry in entries)
        {
            if (entry.Entity is InsectModel insect)
            {
                insect.CreatedAt = DateTime.UtcNow;
            }
        }
    }
}

