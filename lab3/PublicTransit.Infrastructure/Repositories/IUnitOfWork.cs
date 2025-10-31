namespace PublicTransit.Infrastructure.Repositories;

/// <summary>
/// Інтерфейс Unit of Work для координації роботи репозиторіїв
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Репозиторій комах
    /// </summary>
    IInsectRepository Insects { get; }

    /// <summary>
    /// Репозиторій мух
    /// </summary>
    IFlyRepository Flies { get; }

    /// <summary>
    /// Репозиторій павуків
    /// </summary>
    ISpiderRepository Spiders { get; }

    /// <summary>
    /// Зберегти всі зміни в базі даних
    /// </summary>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Почати транзакцію
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Підтвердити транзакцію
    /// </summary>
    Task CommitTransactionAsync();

    /// <summary>
    /// Відкотити транзакцію
    /// </summary>
    Task RollbackTransactionAsync();
}

