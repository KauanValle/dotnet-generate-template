using ApiTemplate.Application.Common.Interfaces;
using ApiTemplate.Domain.Common;
using ApiTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiTemplate.Infrastructure.Persistence;

public sealed class Repository<TEntity>(AppDbContext context) : IRepository<TEntity>
    where TEntity : BaseEntity
{
    public Task<TEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Set<TEntity>().FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => context.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);

    public void Add(TEntity entity) => context.Set<TEntity>().Add(entity);

    public void Update(TEntity entity) => context.Set<TEntity>().Update(entity);

    public void Remove(TEntity entity) => context.Set<TEntity>().Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
