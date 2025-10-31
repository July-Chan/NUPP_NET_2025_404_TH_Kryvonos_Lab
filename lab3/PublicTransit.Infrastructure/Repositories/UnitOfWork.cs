using Microsoft.EntityFrameworkCore.Storage;

namespace PublicTransit.Infrastructure.Repositories;

/// <summary>
/// Unit of Work для координації роботи репозиторіїв
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly PublicTransitContext _context;
    private IDbContextTransaction? _transaction;

    private IInsectRepository? _insects;
    private IFlyRepository? _flies;
    private ISpiderRepository? _spiders;

    public UnitOfWork(PublicTransitContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Репозиторій комах
    /// </summary>
    public IInsectRepository Insects
    {
        get
        {
            _insects ??= new InsectRepository(_context);
            return _insects;
        }
    }

    /// <summary>
    /// Репозиторій мух
    /// </summary>
    public IFlyRepository Flies
    {
        get
        {
            _flies ??= new FlyRepository(_context);
            return _flies;
        }
    }

    /// <summary>
    /// Репозиторій павуків
    /// </summary>
    public ISpiderRepository Spiders
    {
        get
        {
            _spiders ??= new SpiderRepository(_context);
            return _spiders;
        }
    }

    /// <summary>
    /// Зберегти всі зміни в базі даних
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Почати транзакцію
    /// </summary>
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Підтвердити транзакцію
    /// </summary>
    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();

            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    /// <summary>
    /// Відкотити транзакцію
    /// </summary>
    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

