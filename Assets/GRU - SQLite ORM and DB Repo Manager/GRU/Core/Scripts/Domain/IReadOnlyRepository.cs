using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;

namespace SpatiumInteractive.Libraries.Unity.GRU.Domain
{
    /// <summary>
    /// Defines methods for reading data from the database
    /// </summary>
    /// <typeparam name="TEntity">Entity whose data will be read</typeparam>
    /// <typeparam name="TId">Entity's generic identificator</typeparam>
    public interface IReadOnlyRepository<TEntity, TId> : IReadOnlyRepositoryBase<TEntity, TId>
        where TEntity : EntityBase<TId>, IAggregateRoot
        where TId : struct
    {
        /// <summary>
        /// Reads entity of TEntity type from the database by identificator
        /// </summary>
        /// <param name="id">entity's identificator</param>
        /// <returns>entity of type TEntity</returns>
        TEntity Get(TId id);

        /// <summary>
        /// Reads entity of TEntity type from the database by identificator
        /// </summary>
        /// <param name="id">entity's identificator</param>
        /// <param name="queryMetadata">query metadata for select and where (Include not supported here)</param>
        /// <returns>entity of type TEntity</returns>
        TEntity Get(TId id, QueryExpression<TEntity> queryMetadata);

        /// <summary>
        /// Reads entity of TEntity type from the database
        /// </summary>
        /// <param name="queryMetadata">query metadata for select and where (Include not supported here)</param>
        /// <returns>entity of type TEntity</returns>
        TEntity Get(QueryExpression<TEntity> queryMetadata);

        /// <summary>
        /// Reads all entities of TEntity type from the database
        /// </summary>
        /// <returns>list of entitites of TEntity type</returns>
        IReadOnlyCollection<TEntity> GetAll();

        /// <summary>
        /// Reads all entities of TEntity type from the database
        /// </summary>
        /// <param name="queryMetadata">query metadata for select and where (Include not supported here) </param>
        /// <returns>list of entitites of TEntity type</returns>
        IReadOnlyCollection<TEntity> GetAll(QueryExpression<TEntity> queryMetadata);

        /// <summary>
        /// Reads all entities of TEntity type from the database
        /// </summary>
        /// <param name="queryMetadata">query metadata for select and where. (Include not supported here)</param>
        /// <returns>list of entitites of TEntity type</returns>
        IReadOnlyCollection<TEntity> Find(QueryExpression<TEntity> queryMetadata);

        /// <summary>
        /// Reads entity of TEntity type from the database
        /// </summary>
        /// <param name="predicate">Expression for select</param>
        /// <param name="includeItems">String representation of members to be included</param>
        /// <returns>Entitity of TEntity type</returns>
        TEntity FindBy(Expression<Func<TEntity, bool>> predicate, params string[] includeItems);
    }
}
