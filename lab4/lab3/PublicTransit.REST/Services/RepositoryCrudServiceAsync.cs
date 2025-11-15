using PublicTransit.Common.App.Crud;
using PublicTransit.Infrastructure.Repositories;

namespace PublicTransit.REST.Services;

/// <summary>
/// Реалізація ICrudServiceAsync, яка використовує репозиторій для доступу до даних
/// </summary>
/// <typeparam name="T">Тип сутності</typeparam>
public class RepositoryCrudServiceAsync<T> : ICrudServiceAsync<T> where T : class
{
    private readonly IRepository<T> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RepositoryCrudServiceAsync(IRepository<T> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> CreateAsync(T element)
    {
        if (element == null)
            return false;

        try
        {
            await _repository.AddAsync(element);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<T> ReadAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);
        if (result == null)
        {
            throw new KeyNotFoundException($"Entity with id {id} not found.");
        }
        return result;
    }

    public async Task<IEnumerable<T>> ReadAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        if (page < 1)
            throw new ArgumentOutOfRangeException(nameof(page), "Page number must be greater than 0.");
        if (amount < 1)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than 0.");

        var allItems = await _repository.GetAllAsync();
        return allItems.Skip((page - 1) * amount).Take(amount);
    }

    public async Task<bool> UpdateAsync(T element)
    {
        if (element == null)
            return false;

        try
        {
            await _repository.UpdateAsync(element);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveAsync(T element)
    {
        if (element == null)
            return false;

        try
        {
            await _repository.DeleteAsync(element);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SaveAsync()
    {
        try
        {
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ReadAllAsync().GetAwaiter().GetResult().GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

