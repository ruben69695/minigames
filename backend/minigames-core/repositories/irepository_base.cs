namespace Minigames.Core.Repositories;

public interface IRepository<T, Tkey> where T : class
{
    Task<T?> GetAsync(Tkey id);
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
}