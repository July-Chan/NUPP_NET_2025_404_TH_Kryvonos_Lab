public interface ICrudService<T>
{
    public Guid Create(T element);
    public T Read(Guid id);
    public IEnumerable<T> ReadAll();
    public void Update(Guid id, T element);
    public void Remove(Guid id);
    public void Save(string path);
}
