using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;

namespace SpatiumInteractive.Libraries.Unity.GRU.Domain
{
    /// <summary>
    /// Defines methods for changing data in the database
    /// </summary>
    /// <typeparam name="TEntity">Entity whose data will be read</typeparam>
    /// <typeparam name="TId">Entity's generic identificator</typeparam>
    public interface IRepository<TEntity, TId> : IRepositoryBase<TEntity, TId>, IReadOnlyRepository<TEntity, TId>
        where TEntity : EntityBase<TId>, IAggregateRoot
        where TId : struct
    {
    }
}
