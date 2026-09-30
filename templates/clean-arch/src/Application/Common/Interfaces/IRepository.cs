using ApiTemplate.Domain.Common;

namespace ApiTemplate.Application.Common.Interfaces;

public interface IRepository<TEntity>
    where TEntity : BaseEntity
{
    Task<TEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    void Add(TEntity entity);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
