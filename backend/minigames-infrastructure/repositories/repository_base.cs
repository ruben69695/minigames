using Microsoft.EntityFrameworkCore;
using Minigames.Core.Repositories;

namespace Minigames.Infrastructure.Repositories;

public class RepositoryBase<T, TKey> : IRepository<T, TKey> where T : class
{
    private readonly DbContext _dbContext;

    public RepositoryBase(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(T entity)
    {
        _dbContext.Set<T>().Add(entity);
    }

    public async Task<T?> GetAsync(TKey id)
    {
       return await _dbContext.Set<T>().FindAsync(id);
    }

    public void Remove(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
       return await _dbContext.SaveChangesAsync() == 0;
    }

    public void Update(T entity)
    {
        _dbContext.Set<T>().Update(entity);
    }
}