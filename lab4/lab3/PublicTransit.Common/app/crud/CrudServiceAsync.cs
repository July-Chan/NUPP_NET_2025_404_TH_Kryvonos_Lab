using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PublicTransit.Common.App.Crud
{
    public class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : IWithId
    {
        private readonly ConcurrentDictionary<Guid, T> _storage = new();
        private readonly string _filePath;
        private static readonly SemaphoreSlim _fileLock = new(1, 1);

        public CrudServiceAsync(string filePath)
        {
            _filePath = filePath;
            LoadFromFileAsync().GetAwaiter().GetResult();
        }

        private async Task LoadFromFileAsync()
        {
            if (!File.Exists(_filePath))
            {
                return;
            }

            await _fileLock.WaitAsync();
            try
            {
                await using var stream = File.OpenRead(_filePath);
                if (stream.Length > 0)
                {
                    var elements = await JsonSerializer.DeserializeAsync<IEnumerable<T>>(stream);
                    if (elements != null)
                    {
                        foreach (var element in elements)
                        {
                            _storage.TryAdd(element.Id, element);
                        }
                    }
                }
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public Task<bool> CreateAsync(T element)
        {
            return Task.FromResult(_storage.TryAdd(element.Id, element));
        }

        public Task<T> ReadAsync(Guid id)
        {
            _storage.TryGetValue(id, out var element);
            return Task.FromResult(element);
        }

        public Task<IEnumerable<T>> ReadAllAsync()
        {
            return Task.FromResult(_storage.Values.AsEnumerable());
        }

        public Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            if (page < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(page), "Page number must be greater than 0.");
            }
            if (amount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than 0.");
            }

            var result = _storage.Values.Skip((page - 1) * amount).Take(amount);
            return Task.FromResult(result.AsEnumerable());
        }

        public Task<bool> UpdateAsync(T element)
        {
            if (!_storage.ContainsKey(element.Id))
            {
                return Task.FromResult(false);
            }
            _storage[element.Id] = element;
            return Task.FromResult(true);
        }

        public Task<bool> RemoveAsync(T element)
        {
            return Task.FromResult(_storage.TryRemove(element.Id, out _));
        }

        public async Task<bool> SaveAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                await using var stream = File.Create(_filePath);
                await JsonSerializer.SerializeAsync(stream, _storage.Values, options);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _storage.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
