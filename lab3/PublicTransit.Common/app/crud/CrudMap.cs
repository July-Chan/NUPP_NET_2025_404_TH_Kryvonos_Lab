using System.Text.Json;

public class CrudMap<T> : ICrudService<T>
{
    private readonly Dictionary<Guid, T> _map;

    // === КОНСТРУКТОР ===
    public CrudMap()
    {
        _map = new Dictionary<Guid, T>();
    }

    // === СТВОРЕННЯ ===
    public Guid Create(T element)
    {
        var id = Guid.NewGuid();
        _map.Add(id, element);
        return id;
    }

    // === ПОВЕРНЕННЯ ЕЛЕМЕНТУ ===
    public T Read(Guid id)
    {
        if (_map.TryGetValue(id, out var element))
        {
            return element;
        }
        throw new KeyNotFoundException($"Елемент з ID {id} не знайдено");
    }

    // === ПОВЕРНЕННЯ ЕЛЕМЕНТІВ ===
    public IEnumerable<T> ReadAll()
    {
        return _map.Values.ToList();
    }

    // === ВИДАЛЕННЯ ЗА ID ===
    public void Remove(Guid id)
    {
        if (!_map.Remove(id))
        {
            throw new KeyNotFoundException($"Елемент з ID {id} не знайдено для видалення");
        }
    }

    // === ОНОВЛЕННЯ ЗА КЛЮЧЕМ ===
    public void Update(Guid id, T element)
    {
        if (_map.ContainsKey(id))
        {
            _map[id] = element;
        }
        else
        {
            throw new KeyNotFoundException($"Елемент з ID {id} не знайдено для оновлення");
        }
    }

    // === ЗБЕРЕЖЕННЯ ЗА ШЛЯХОМ ===
    public void Save(string path)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(_map, options);
        File.WriteAllText(path, json);
    }

    // === СТАТИЧНЕ ЗАВАНТАЖЕННЯ ЗА ШЛЯХОМ ===
    public static CrudMap<T> Load(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Файл {path} не знайдено");

        string json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<Dictionary<Guid, T>>(json);

        var newCrud = new CrudMap<T>();
        if (data != null)
        {
            foreach (var kvp in data)
            {
                newCrud._map[kvp.Key] = kvp.Value;
            }
        }

        return newCrud;
    }
}