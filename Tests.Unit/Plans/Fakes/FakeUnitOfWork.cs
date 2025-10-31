using ApplicationLayer.Interfaces;

namespace Tests.Unit.Plans.Fakes;

public class FakeUnitOfWork : IUnitOfWork
{
    public Task BeginTransactionAsync() => Task.CompletedTask;
    public Task CommitAsync() => Task.CompletedTask;
    public Task RollbackAsync() => Task.CompletedTask;
    public int SaveChanges() => 0;
    public int SaveChanges(bool acceptAllChangesOnSuccess) => 0;
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(0);
    public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) => Task.FromResult(0);
}