using ApplicationLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Tests.Unit.Plans.Fakes;

public class FakeRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly List<TEntity> _store = new();
    private readonly DbSet<TEntity> _dbSet;

    public FakeRepository(DbContextOptions<DbContext>? options = null)
    {
        // For simplicity, expose a fake DbSet via a queryable list wrapper
        var data = _store.AsQueryable();
        _dbSet = new FakeDbSet<TEntity>(_store);
    }

    public IQueryable<TEntity> Query() => _store.AsQueryable();

    public DbSet<TEntity> GetDbSet() => _dbSet;

    public TEntity GetById(int id) => _store.FirstOrDefault();

    public Task<TEntity> GetByIdAsync(int id) => Task.FromResult(_store.FirstOrDefault());

    public TEntity GetByIdIncludeNavigation(int id, params Expression<Func<TEntity, object>>[] includeProperties) => _store.FirstOrDefault();

    public Task<TEntity> GetByIdAsyncIncludeNavigation(int id, params Expression<Func<TEntity, object>>[] includeProperties) => Task.FromResult(_store.FirstOrDefault());

    public IEnumerable<TEntity> GetAll() => _store;

    public IEnumerable<TEntity> GetAllIncludeNavigation(params Expression<Func<TEntity, object>>[] includeProperties) => _store;

    public Task<IEnumerable<TEntity>> GetAllAsyncIncludeNavigation(params Expression<Func<TEntity, object>>[] includeProperties) => Task.FromResult<IEnumerable<TEntity>>(_store);

    public IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicate) => _store.AsQueryable().Where(predicate);

    public void Add(TEntity entity) => _store.Add(entity);

    public Task AddAsync(TEntity entity)
    {
        _store.Add(entity);
        return Task.CompletedTask;
    }

    public void AddRange(IEnumerable<TEntity> entities) => _store.AddRange(entities);

    public Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        _store.AddRange(entities);
        return Task.CompletedTask;
    }

    public void Update(TEntity entity) { /* no-op for list */ }

    public void UpdateRange(IEnumerable<TEntity> entities) { /* no-op */ }

    public void Remove(TEntity entity) => _store.Remove(entity);

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        foreach (var e in entities) _store.Remove(e);
    }

    public void DeleteFromDatabase(TEntity entity) => _store.Remove(entity);

    public void DeleteRangeFromDatabase(IEnumerable<TEntity> entities)
    {
        foreach (var e in entities) _store.Remove(e);
    }

    public IEnumerable<TResult> ExecuteQuery<TResult>(string sql, params object[] parameters) => Enumerable.Empty<TResult>();

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        => Task.FromResult(_store.AsQueryable().Any(predicate));

    public bool Any(Expression<Func<TEntity, bool>> predicate)
        => _store.AsQueryable().Any(predicate);

    public Task UpdateAsync(TEntity entity)
    {
        // no-op for list
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        // no-op
        return Task.CompletedTask;
    }
}

internal class FakeDbSet<TEntity> : DbSet<TEntity> where TEntity : class
{
    private readonly List<TEntity> _list;

    public FakeDbSet(List<TEntity> list)
    {
        _list = list;
    }

    public override Task<TEntity?> FindAsync(params object?[]? keyValues)
    {
        return Task.FromResult(_list.FirstOrDefault());
    }

    public override EntityEntry<TEntity> Add(TEntity entity)
    {
        _list.Add(entity);
        return null!;
    }
}